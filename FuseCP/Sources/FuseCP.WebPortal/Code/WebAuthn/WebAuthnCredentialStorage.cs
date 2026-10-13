// Copyright (C) 2026 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuseCP.EnterpriseServer;
using WebAuthn.Net.Models.Protocol;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Services.Serialization.Cose.Models.Enums;
using WebAuthn.Net.Services.Serialization.Cose.Models.Enums.EC2;
using WebAuthn.Net.Services.Serialization.Cose.Models.Enums.OKP;
using WebAuthn.Net.Storage.Credential;
using WebAuthn.Net.Storage.Credential.Models;

namespace FuseCP.Portal.WebAuthn
{
	//
	// Bridges the WebAuthn.Net ceremony services to the FuseCP EnterpriseServer
	// WebAuthn credentials table. The credential records WebAuthn.Net produces are
	// self-describing (UserHandle carries the user id), so the storage only maps
	// them to the Enterprise WebAuthnCredentialInfo DTO and persists them through
	// the authenticated (My Account) SOAP endpoints.
	//
	public class WebAuthnCredentialStorage : ICredentialStorage<FuseCPWebAuthnContext>
	{
		public Task<PublicKeyCredentialDescriptor[]> FindDescriptorsAsync(FuseCPWebAuthnContext context, string rpId, byte[] userHandle, CancellationToken cancellationToken)
		{
			WebAuthnCredentialInfo[] credentials = PortalUtils.GetMyWebAuthnCredentials(context?.Ticket);
			// Existing descriptors are used to exclude already-registered keys, so a
			// fresh passkey is always created rather than reusing an old credential id.
			PublicKeyCredentialDescriptor[] descriptors = (credentials ?? Array.Empty<WebAuthnCredentialInfo>())
				.Where(c => c != null && c.CredentialId != null && c.CredentialId.Length > 0)
				.Select(c => new PublicKeyCredentialDescriptor((PublicKeyCredentialType)c.CredentialType, c.CredentialId, ToTransports(c.Transports)))
				.ToArray();

			return Task.FromResult(descriptors);
		}

		public Task<UserCredentialRecord> FindExistingCredentialForAuthenticationAsync(FuseCPWebAuthnContext context, string rpId, byte[] userHandle, byte[] credentialId, CancellationToken cancellationToken)
		{
			if (context?.Ticket == null || credentialId == null || credentialId.Length == 0)
			{
				System.Diagnostics.Trace.TraceError("WebAuthn sign-in: FindExistingCredentialForAuthentication skipped (ticket="
					+ (context?.Ticket == null ? "null" : "set") + ", credentialId length=" + (credentialId == null ? 0 : credentialId.Length) + ")");
				return Task.FromResult<UserCredentialRecord>(null);
			}

			// The ceremony ticket is a synthetic login ticket (machine-key protected,
			// carrying the verified username + password). The EnterpriseServer resolves
			// a ticket's Basic-auth (SecurityContext) principal to the supervisor
			// account, so the ticket-based "My" endpoint returns no credentials.
			// Instead, look the stored credential up through the explicit username +
			// SHA256(password) endpoint, which the login flow already proved works.
			string username = context.Ticket.Name ?? String.Empty;
			string password = GetTicketPassword(context.Ticket);
			string ip = context.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? String.Empty;

			WebAuthnCredentialInfo[] credentials;
			try
			{
				credentials = PortalUtils.GetWebAuthnCredentials(username, password, ip);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				// A failure here (e.g. the ES could not resolve the proven credentials
				// to a user) aborts the ceremony with a generic "failed"; surface the
				// real cause.
				System.Diagnostics.Trace.TraceError("WebAuthn sign-in: GetWebAuthnCredentials threw for user "
					+ username + ": " + ex);
				throw;
			}

			int storedCount = credentials == null ? 0 : credentials.Length;
			System.Diagnostics.Trace.TraceError("WebAuthn sign-in: GetWebAuthnCredentials returned " + storedCount
				+ " credential(s) for user " + username
				+ " (rpId=" + (rpId ?? String.Empty)
				+ ", requested credentialId=" + Convert.ToBase64String(credentialId) + ")");

			WebAuthnCredentialInfo match = (credentials ?? Array.Empty<WebAuthnCredentialInfo>())
				.FirstOrDefault(c => c != null
					&& c.CredentialId != null
					&& c.CredentialId.SequenceEqual(credentialId)
					&& (userHandle == null || userHandle.Length == 0 || (c.UserHandle != null && c.UserHandle.SequenceEqual(userHandle)))
					&& string.Equals(c.RpId, rpId, StringComparison.Ordinal));

			if (match == null)
			{
				if (storedCount > 0)
				{
					string storedIds = String.Join(", ", (credentials ?? Array.Empty<WebAuthnCredentialInfo>())
						.Where(c => c != null && c.CredentialId != null)
						.Select(c => Convert.ToBase64String(c.CredentialId) + "(rpId=" + c.RpId + ")"));
					System.Diagnostics.Trace.TraceError("WebAuthn sign-in: no credential matched (stored: " + storedIds + ")");
				}
				else
				{
					System.Diagnostics.Trace.TraceError("WebAuthn sign-in: no credential matched because the password-based lookup returned no credentials");
				}
				return Task.FromResult<UserCredentialRecord>(null);
			}

			return Task.FromResult(ToRecord(match));
		}

		public Task<bool> SaveIfNotRegisteredForOtherUserAsync(FuseCPWebAuthnContext context, UserCredentialRecord credential, CancellationToken cancellationToken)
		{
			bool saved = PortalUtils.SaveMyWebAuthnCredential(ToInfo(credential), context?.Ticket);
			return Task.FromResult(saved);
		}

		public Task<bool> UpdateCredentialAsync(FuseCPWebAuthnContext context, UserCredentialRecord credential, CancellationToken cancellationToken)
		{
			// The ES exposes a dedicated atomic UpdateWebAuthnCredential endpoint,
			// authenticated with the proven username + password (the login flow).
			// The ceremony ticket is a synthetic login ticket, and the ES resolves
			// its Basic-auth (SecurityContext) principal to the supervisor account,
			// so the ticket-based "My" delete + save endpoints cannot find the
			// user's credential. Use the password-proven endpoint instead: it
			// validates the credentials and updates the sign count / flags in a
			// single SaveChanges (atomic, no delete-then-save window).
			WebAuthnCredentialInfo info = ToInfo(credential);
			string credentialIdBase64 = info.CredentialId != null && info.CredentialId.Length > 0
				? Convert.ToBase64String(info.CredentialId) : String.Empty;
			string username = context?.Ticket?.Name ?? String.Empty;
			string password = context?.Ticket != null ? GetTicketPassword(context.Ticket) : String.Empty;
			string ip = context?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? String.Empty;

			bool updated = PortalUtils.UpdateWebAuthnCredential(username, password, ip, info);
			System.Diagnostics.Trace.TraceError("WebAuthn sign-in: UpdateCredential " + (updated ? "succeeded" : "FAILED")
				+ " (user=" + username + ", credentialId=" + credentialIdBase64
				+ ", signCount=" + info.SignCount + ")");
			return Task.FromResult(updated);
		}

		// The ticket's UserData carries "password + NewLine + role" (see
		// PortalUtils.CreateAuthTicket); only the password part is needed here,
		// and only server-side.
		private static string GetTicketPassword(System.Web.Security.FormsAuthenticationTicket ticket)
		{
			string userData = ticket?.UserData ?? String.Empty;
			int newline = userData.IndexOf(Environment.NewLine);
			return newline >= 0 ? userData.Substring(0, newline) : userData;
		}

		private static AuthenticatorTransport[] ToTransports(int[] transports)
		{
			if (transports == null || transports.Length == 0)
				return Array.Empty<AuthenticatorTransport>();
			return transports.Select(t => (AuthenticatorTransport)t).ToArray();
		}

		private static int[] ToTransportsInt(AuthenticatorTransport[] transports)
		{
			if (transports == null || transports.Length == 0)
				return Array.Empty<int>();
			return transports.Select(t => (int)t).ToArray();
		}

		private static WebAuthnCredentialInfo ToInfo(UserCredentialRecord record)
		{
			byte[] userHandle = record?.UserHandle ?? Array.Empty<byte>();
			// The portal binds the user handle to a 4-byte little-endian user id.
			int userId = userHandle.Length == 4 ? BitConverter.ToInt32(userHandle, 0) : 0;

			CredentialRecord credential = record?.CredentialRecord;
			CredentialPublicKeyRecord publicKey = credential?.PublicKey;

			WebAuthnCredentialInfo info = new WebAuthnCredentialInfo
			{
				UserId = userId,
				RpId = record?.RpId,
				UserHandle = userHandle,
				CredentialId = credential?.Id,
				CredentialType = credential != null ? (int)credential.Type : 0,
				Kty = publicKey != null ? (int)publicKey.Kty : 0,
				Alg = publicKey != null ? (int)publicKey.Alg : 0,
				SignCount = credential != null ? (long)credential.SignCount : 0,
				Transports = ToTransportsInt(credential?.Transports),
				UvInitialized = credential?.UvInitialized ?? false,
				BackupEligible = credential?.BackupEligible ?? false,
				BackupState = credential?.BackupState ?? false,
				AttestationObject = credential?.AttestationObject,
				AttestationClientDataJson = credential?.AttestationClientDataJSON,
				DisplayName = record?.Description
			};

			if (publicKey?.Rsa != null)
			{
				info.RsaModulusN = publicKey.Rsa.ModulusN;
				info.RsaExponentE = publicKey.Rsa.ExponentE;
			}
			else if (publicKey?.Ec2 != null)
			{
				info.Ec2Crv = (int)publicKey.Ec2.Crv;
				info.Ec2X = publicKey.Ec2.X;
				info.Ec2Y = publicKey.Ec2.Y;
			}
			else if (publicKey?.Okp != null)
			{
				info.OkpCrv = (int)publicKey.Okp.Crv;
				info.OkpX = publicKey.Okp.X;
			}

			return info;
		}

		private static UserCredentialRecord ToRecord(WebAuthnCredentialInfo info)
		{
			CredentialPublicKeyRecord publicKey = null;
			if (info.Kty != 0)
			{
				CredentialPublicKeyRsaParametersRecord rsa = null;
				if (info.RsaModulusN != null && info.RsaModulusN.Length > 0)
					rsa = new CredentialPublicKeyRsaParametersRecord(info.RsaModulusN, info.RsaExponentE);

				CredentialPublicKeyEc2ParametersRecord ec2 = null;
				if (info.Ec2X != null && info.Ec2X.Length > 0 && info.Ec2Crv.HasValue)
					ec2 = new CredentialPublicKeyEc2ParametersRecord((CoseEc2EllipticCurve)info.Ec2Crv.Value, info.Ec2X, info.Ec2Y);

				CredentialPublicKeyOkpParametersRecord okp = null;
				if (info.OkpX != null && info.OkpX.Length > 0 && info.OkpCrv.HasValue)
					okp = new CredentialPublicKeyOkpParametersRecord((CoseOkpEllipticCurve)info.OkpCrv.Value, info.OkpX);

				publicKey = new CredentialPublicKeyRecord((CoseKeyType)info.Kty, (CoseAlgorithm)info.Alg, rsa, ec2, okp);
			}

			CredentialRecord credential = new CredentialRecord(
				(PublicKeyCredentialType)info.CredentialType,
				info.CredentialId,
				publicKey,
				(uint)info.SignCount,
				ToTransports(info.Transports),
				info.UvInitialized,
				info.BackupEligible,
				info.BackupState,
				info.AttestationObject,
				info.AttestationClientDataJson);

			return new UserCredentialRecord(info.UserHandle ?? Array.Empty<byte>(), info.RpId, info.DisplayName, credential);
		}
	}
}
