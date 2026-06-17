/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

package org.apache.guacamole.auth;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Map;
import java.util.HashMap;
import java.text.SimpleDateFormat;  
import java.util.Date;
import java.text.ParseException;
import org.apache.guacamole.GuacamoleException;
import org.apache.guacamole.net.auth.simple.SimpleAuthenticationProvider;
import org.apache.guacamole.net.auth.Credentials;
import org.apache.guacamole.protocol.GuacamoleConfiguration;
import org.apache.guacamole.environment.Environment;
import org.apache.guacamole.environment.LocalEnvironment;
import org.apache.guacamole.properties.StringGuacamoleProperty;

import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import javax.crypto.Cipher;
import javax.crypto.spec.GCMParameterSpec;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

import org.bouncycastle.crypto.engines.RijndaelEngine;
import org.bouncycastle.crypto.modes.CBCBlockCipher;
import org.bouncycastle.crypto.paddings.PaddedBufferedBlockCipher;
import org.bouncycastle.crypto.paddings.PKCS7Padding;
import org.bouncycastle.crypto.params.KeyParameter;
import org.bouncycastle.crypto.params.ParametersWithIV;
import org.bouncycastle.crypto.CipherParameters;
import org.bouncycastle.util.encoders.Base64;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import org.json.*;

public class FuseCPAuthenticationProvider extends SimpleAuthenticationProvider {
	
	private static final Logger logger = LoggerFactory.getLogger(FuseCPAuthenticationProvider.class);

	final StringGuacamoleProperty solidKey = new StringGuacamoleProperty() {
		@Override
		public String getName() { return "fusecp-key"; }
	};

	final StringGuacamoleProperty linkTime = new StringGuacamoleProperty() {
		@Override
		public String getName() { return "fusecp-link-exp-time"; }
	};

	final StringGuacamoleProperty serverLayout = new StringGuacamoleProperty() {
		@Override
		public String getName() { return "server-layout"; }
	};

	@Override
	public String getIdentifier() {
		return "fusecp-auth";
	}

	@Override
	public Map<String, GuacamoleConfiguration> getAuthorizedConfigurations(Credentials credentials)throws GuacamoleException {

		log("debug", "requesting authentication from " + credentials.getRemoteHostname() + " [" + credentials.getRemoteAddress() + "]");

		Map<String, String[]> paramMap = credentials.getRequest().getParameterMap();
		String eStr;

		try {

			eStr = paramMap.get("e")[0];

		}catch (Exception e) {

			log("debug", "fusecp parameter map does not exist and will not be read");
			return null;

		}

		Environment environment = LocalEnvironment.getInstance();
		String key = environment.getRequiredProperty(solidKey);
		eStr = decrypt(key, eStr);

		if (eStr == null || eStr.isEmpty()) {
			
			log("debug", "invalid parameter map (abort)");
			return null;

		}

		int lTime = 60000;
		try {

			lTime = Integer.parseInt(environment.getRequiredProperty(linkTime))*1000;

		}catch(Exception e) {

			log("warn", "link expiration exception: " + e.toString());
			log("warn", "link expiration failing to default " + String.valueOf(lTime) + "ms");

		}

		String protocol, hypervHost, userName, password, domain, port, security, vmId, vmHostname, timestamp;
		try {

			JSONObject obj = new JSONObject(eStr);
			protocol = obj.getString("protocol");
			hypervHost = obj.getString("hostname");
			userName = obj.getString("username");
			password = obj.getString("password");
			domain = obj.getString("domain");
			port = obj.getString("port");
			security = obj.getString("security");
			vmId = obj.getString("preconnectionblob");
			vmHostname = obj.getString("vmhostname");
			timestamp = obj.getString("timestamp");

		} catch (JSONException e) {

			log("error", "JSON exception (abort): " + e.toString());
			return null;

		}

		Date linkDate;
		try {

			linkDate = new SimpleDateFormat("yyyy-MM-dd_HH:mm:ss").parse(timestamp);

		}catch(ParseException e) {

			log("error", "timestamp exception (abort): " + e.toString());
			return null;

		}

		Date currDate = new Date();
		Long currTime = currDate.getTime();
		Long linkTime = linkDate.getTime();

		log("debug", "current time: " + currTime.toString() + ", stamped time: " + linkTime + ", link exp: " + lTime + "ms");

		if (currTime-linkTime > lTime) {

			log("warn", "link expired (abort)");
			return null;

		}

		Map<String, GuacamoleConfiguration> configs = new HashMap<String, GuacamoleConfiguration>();
		GuacamoleConfiguration config = new GuacamoleConfiguration();
		config.setProtocol(protocol);
		config.setParameter("hostname", hypervHost);
		config.setParameter("port", "2179");
		config.setParameter("username", userName);
		config.setParameter("password", password);
		config.setParameter("domain", domain);
		config.setParameter("security", "vmconnect");
		config.setParameter("ignore-cert", "true");
		config.setParameter("disable-auth", "false");
		config.setParameter("preconnection-id", "");
		config.setParameter("preconnection-blob", vmId);

		String layout = environment.getProperty(serverLayout);
		if (layout != null && !layout.isEmpty()) config.setParameter("server-layout", layout);
		configs.put(vmHostname, config);

		if (logger.isDebugEnabled()) {
			log("debug", "authorized connection from " + domain + "/" + userName + " @ " + credentials.getRemoteHostname() + " [" + credentials.getRemoteAddress() + "] to " + vmHostname + " [" + vmId + "] on " + hypervHost + ":" + port + " using " + security);
		}
		else {
			log("info", "authorized connection from " + credentials.getRemoteHostname() + " [" + credentials.getRemoteAddress() + "] to " + vmHostname + " located at " + hypervHost + ":" + port + " using " + security);
		}
		
		return configs;

	}

	private void log(String logType, String msg) {
		try {
			if (logType == "info" && logger.isInfoEnabled()) { logger.info(msg); }
			else if (logType == "warn" && logger.isWarnEnabled()) { logger.warn(msg); }
			else if (logType == "error" && logger.isErrorEnabled()) { logger.error(msg); }
			else if (logType == "debug" && logger.isDebugEnabled()) { logger.debug(msg); }
			else if (logType == "trace" && logger.isTraceEnabled()) { logger.trace(msg); }
		}catch (Exception e) {}
	}

	private static final String MODERN_PREFIX = "v2:";
	private static final int GCM_NONCE_LEN = 12;
	private static final int GCM_TAG_LEN = 16;

	private String decrypt(String key, String encrypted)
	{
		String[] split = key.split(":");
		if (split.length != 2) return "";
		String keyPart = split[0];
		String ivPart  = split[1];

		if (encrypted.startsWith(MODERN_PREFIX)) {
			String result = decryptAesGcm(keyPart, ivPart, encrypted.substring(MODERN_PREFIX.length()));
			if (result != null && !result.isEmpty()) return result;
		}

		// Fallback: try standard AES-256-CBC (16-byte IV)
		String cbcResult = decryptAesCbc(keyPart, ivPart, encrypted);
		if (cbcResult != null && !cbcResult.isEmpty()) return cbcResult;

		// Legacy fallback: Rijndael-256 via BouncyCastle (32-byte IV)
		return decryptRijndaelCBC(keyPart, ivPart, encrypted);
	}

	/**
	 * AES-256-GCM decrypt. Payload layout: [12-byte nonce][16-byte tag][ciphertext].
	 * The IV from the key config is used as AAD (additional authenticated data).
	 */
	private String decryptAesGcm(String keyB64, String ivB64, String payloadB64)
	{
		try {
			byte[] payloadBin = urlSafeBase64Decode(payloadB64);
			if (payloadBin == null || payloadBin.length < GCM_NONCE_LEN + GCM_TAG_LEN) return "";

			byte[] nonce      = new byte[GCM_NONCE_LEN];
			byte[] tagAndCipher = new byte[payloadBin.length - GCM_NONCE_LEN];
			System.arraycopy(payloadBin, 0, nonce, 0, nonce.length);
			System.arraycopy(payloadBin, nonce.length, tagAndCipher, 0, tagAndCipher.length);

			byte[] keyBytes = Base64.decode(keyB64);
			byte[] aad      = Base64.decode(ivB64);

			SecretKeySpec keySpec = new SecretKeySpec(keyBytes, "AES");
			GCMParameterSpec gcmSpec = new GCMParameterSpec(GCM_TAG_LEN * 8, nonce);

			Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding");
			cipher.init(Cipher.DECRYPT_MODE, keySpec, gcmSpec);
			cipher.updateAAD(aad);

			byte[] plain = cipher.doFinal(tagAndCipher);
			return new String(plain, StandardCharsets.UTF_8);
		} catch (Exception e) {
			log("error", "decryptAesGcm exception: " + e.toString());
			return "";
		}
	}

	/**
	 * AES-256-CBC decrypt using JCA (requires 16-byte IV).
	 */
	private String decryptAesCbc(String keyB64, String ivB64, String encryptedB64)
	{
		try {
			byte[] keyBytes  = Base64.decode(keyB64);
			byte[] ivBytes   = Base64.decode(ivB64);
			if (ivBytes.length != 16) return ""; // Not a standard AES IV

			byte[] encBytes  = urlSafeBase64Decode(encryptedB64);
			if (encBytes == null) return "";

			SecretKeySpec keySpec = new SecretKeySpec(keyBytes, "AES");
			Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
			cipher.init(Cipher.DECRYPT_MODE, keySpec, new IvParameterSpec(ivBytes));

			byte[] plain = cipher.doFinal(encBytes);
			return new String(plain, StandardCharsets.UTF_8);
		} catch (Exception e) {
			log("error", "decryptAesCbc exception: " + e.toString());
			return "";
		}
	}

	/**
	 * Legacy Rijndael-256 via BouncyCastle (32-byte IV, old FuseCP payloads).
	 */
	private String decryptRijndaelCBC(String keyB64, String ivB64, String encryptedB64)
	{
		try {
			String innerDecoded = new String(Base64.decode(encryptedB64));
			byte[] bEncrypted = Base64.decode(innerDecoded);
			byte[] bKey = Base64.decode(keyB64);
			byte[] bIv  = Base64.decode(ivB64);

			PaddedBufferedBlockCipher aes = new PaddedBufferedBlockCipher(
				new CBCBlockCipher(new RijndaelEngine(256)), new PKCS7Padding());
			CipherParameters ivAndKey = new ParametersWithIV(new KeyParameter(bKey), bIv);
			aes.init(false, ivAndKey);

			return new String(cipherData(aes, bEncrypted), StandardCharsets.UTF_8);
		} catch (Exception e) {
			log("error", "decryptRijndaelCBC exception: " + e.toString());
			return "";
		}
	}

	/** Decodes both URL-safe and standard Base64. */
	private byte[] urlSafeBase64Decode(String input)
	{
		if (input == null) return null;
		String normalized = input.replace('-', '+').replace('_', '/');
		int mod4 = normalized.length() % 4;
		if (mod4 > 0) normalized += "====".substring(mod4);
		try {
			return java.util.Base64.getDecoder().decode(normalized);
		} catch (Exception e) {
			try { return Base64.decode(normalized); } catch (Exception ex) { return null; }
		}
	}

	private byte[] cipherData(PaddedBufferedBlockCipher cipher, byte[] data)
	{
		try {
			int minSize = cipher.getOutputSize(data.length);
			byte[] outBuf = new byte[minSize];
			int length1 = cipher.processBytes(data, 0, data.length, outBuf, 0);
			int length2 = cipher.doFinal(outBuf, length1);
			int actualLength = length1 + length2;
			byte[] cipherArray = new byte[actualLength];
			for (int x = 0; x < actualLength; x++) {
				cipherArray[x] = outBuf[x];
			}
			return cipherArray;
		} catch (Exception e) {
			log("error", "cipherData exception: " + e.toString());
			return null;
		}
	}
}
