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
using FuseCP.EnterpriseServer.Data;
using FuseCP.EnterpriseServer.Data.Entities;

namespace FuseCP.EnterpriseServer
{
	public class WebAuthnController : ControllerBase
	{
		public WebAuthnController(ControllerBase provider) : base(provider) { }

		#region Login flow (caller proven with username + password)

		public WebAuthnCredentialInfo[] GetWebAuthnCredentials(string username, string password, string ip)
		{
			UserInfo user = GetUserByPassword(username, password, ip);
			if (user == null)
				return new WebAuthnCredentialInfo[0];

			return Database.WebAuthnCredentials
				.Where(c => c.UserId == user.UserId)
				.Select(ToInfo)
				.ToArray();
		}

		public WebAuthnCredentialInfo GetWebAuthnCredential(string username, string password, string ip, string credentialIdBase64)
		{
			UserInfo user = GetUserByPassword(username, password, ip);
			if (user == null)
				return null;

			byte[] credentialId = FromBase64(credentialIdBase64);
			if (credentialId == null)
				return null;

			WebAuthnCredential entity = FindEntity(user.UserId, credentialId);
			return entity == null ? null : ToInfo(entity);
		}

		public bool UpdateWebAuthnCredential(string username, string password, string ip, WebAuthnCredentialInfo info)
		{
			UserInfo user = GetUserByPassword(username, password, ip);
			if (user == null || info == null)
				return false;

			WebAuthnCredential entity = FindEntity(user.UserId, info.CredentialId);
			if (entity == null)
				return false;

			entity.SignCount = info.SignCount;
			entity.UvInitialized = info.UvInitialized;
			entity.BackupEligible = info.BackupEligible;
			entity.BackupState = info.BackupState;
			entity.UpdatedAt = DateTime.UtcNow;
			return Database.SaveChanges() > 0;
		}

		#endregion

		#region Authenticated flow (My Account, SecurityContext user)

		public WebAuthnCredentialInfo[] GetMyWebAuthnCredentials()
		{
			if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0)
				return new WebAuthnCredentialInfo[0];
			if (SecurityContext.User == null)
				return new WebAuthnCredentialInfo[0];

			return Database.WebAuthnCredentials
				.Where(c => c.UserId == SecurityContext.User.UserId)
				.Select(ToInfo)
				.ToArray();
		}

		public bool SaveMyWebAuthnCredential(WebAuthnCredentialInfo info)
		{
			if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0)
				return false;
			if (SecurityContext.User == null)
				return false;

			int userId = SecurityContext.User.UserId;
			if (info == null || info.UserId != userId)
				return false;

			byte[] credentialId = info.CredentialId;
			if (credentialId == null || credentialId.Length == 0)
				return false;

			// Reject if the credential is already registered for any user.
			bool exists = Database.WebAuthnCredentials
				.Where(c => c.RpId == info.RpId)
				.ToList()
				.Any(c => c.CredentialId != null && c.CredentialId.SequenceEqual(credentialId));
			if (exists)
				return false;

			WebAuthnCredential entity = ToEntity(info);
			entity.UserId = userId;
			entity.CreatedAt = DateTime.UtcNow;
			entity.UpdatedAt = DateTime.UtcNow;
			Database.WebAuthnCredentials.Add(entity);
			return Database.SaveChanges() > 0;
		}

		public bool UpdateMyWebAuthnCredentialDisplayName(string credentialIdBase64, string displayName)
		{
			if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0)
				return false;
			if (SecurityContext.User == null)
				return false;

			byte[] credentialId = FromBase64(credentialIdBase64);
			if (credentialId == null)
				return false;

			string name = displayName == null ? null : displayName.Trim();
			if (name != null && name.Length > 255)
				return false;

			WebAuthnCredential entity = FindEntity(SecurityContext.User.UserId, credentialId);
			if (entity == null)
				return false;

			entity.DisplayName = string.IsNullOrEmpty(name) ? null : name;
			entity.UpdatedAt = DateTime.UtcNow;
			return Database.SaveChanges() > 0;
		}

		public bool DeleteMyWebAuthnCredential(string credentialIdBase64)
		{
			if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0)
				return false;
			if (SecurityContext.User == null)
				return false;

			byte[] credentialId = FromBase64(credentialIdBase64);
			if (credentialId == null)
				return false;

			WebAuthnCredential entity = FindEntity(SecurityContext.User.UserId, credentialId);
			if (entity == null)
				return false;

			Database.WebAuthnCredentials.Remove(entity);
			return Database.SaveChanges() > 0;
		}

		#endregion

		#region Helpers

		private UserInfo GetUserByPassword(string username, string password, string ip)
		{
			return UserController.GetUserByUsernamePassword(username, password, ip, false);
		}

		private WebAuthnCredential FindEntity(int userId, byte[] credentialId)
		{
			if (credentialId == null)
				return null;
			return Database.WebAuthnCredentials
				.Where(c => c.UserId == userId)
				.ToList()
				.FirstOrDefault(c => c.CredentialId != null && c.CredentialId.SequenceEqual(credentialId));
		}

		private static byte[] FromBase64(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			try
			{
				return Convert.FromBase64String(value);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		private static string TransportsToString(int[] transports)
		{
			if (transports == null || transports.Length == 0)
				return null;
			return string.Join(",", transports);
		}

		private static int[] TransportsFromString(string value)
		{
			if (string.IsNullOrEmpty(value))
				return new int[0];
			return value.Split(',')
				.Select(s => int.Parse(s.Trim()))
				.ToArray();
		}

		private static WebAuthnCredentialInfo ToInfo(WebAuthnCredential e)
		{
			return new WebAuthnCredentialInfo
			{
				Id = e.Id,
				UserId = e.UserId,
				RpId = e.RpId,
				UserHandle = e.UserHandle,
				CredentialId = e.CredentialId,
				CredentialType = e.CredentialType,
				Kty = e.Kty,
				Alg = e.Alg,
				Ec2Crv = e.Ec2Crv,
				Ec2X = e.Ec2X,
				Ec2Y = e.Ec2Y,
				RsaModulusN = e.RsaModulusN,
				RsaExponentE = e.RsaExponentE,
				OkpCrv = e.OkpCrv,
				OkpX = e.OkpX,
				SignCount = e.SignCount,
				Transports = TransportsFromString(e.Transports),
				UvInitialized = e.UvInitialized,
				BackupEligible = e.BackupEligible,
				BackupState = e.BackupState,
				AttestationObject = e.AttestationObject,
				AttestationClientDataJson = e.AttestationClientDataJson,
				DisplayName = e.DisplayName,
				CreatedAt = e.CreatedAt,
				UpdatedAt = e.UpdatedAt
			};
		}

		private static WebAuthnCredential ToEntity(WebAuthnCredentialInfo info)
		{
			return new WebAuthnCredential
			{
				RpId = info.RpId,
				UserHandle = info.UserHandle,
				CredentialId = info.CredentialId,
				CredentialType = info.CredentialType,
				Kty = info.Kty,
				Alg = info.Alg,
				Ec2Crv = info.Ec2Crv,
				Ec2X = info.Ec2X,
				Ec2Y = info.Ec2Y,
				RsaModulusN = info.RsaModulusN,
				RsaExponentE = info.RsaExponentE,
				OkpCrv = info.OkpCrv,
				OkpX = info.OkpX,
				SignCount = info.SignCount,
				Transports = TransportsToString(info.Transports),
				UvInitialized = info.UvInitialized,
				BackupEligible = info.BackupEligible,
				BackupState = info.BackupState,
				AttestationObject = info.AttestationObject,
				AttestationClientDataJson = info.AttestationClientDataJson,
				DisplayName = info.DisplayName
			};
		}

		#endregion
	}
}
