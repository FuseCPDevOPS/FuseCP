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

namespace FuseCP.EnterpriseServer
{
    /// <summary>
    /// WebAuthn (passkey) public key credential record.
    /// </summary>
    [Serializable]
    public class WebAuthnCredentialInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RpId { get; set; }
        public byte[] UserHandle { get; set; }
        public byte[] CredentialId { get; set; }
        public int CredentialType { get; set; }
        public int Kty { get; set; }
        public int Alg { get; set; }
        public int? Ec2Crv { get; set; }
        public byte[] Ec2X { get; set; }
        public byte[] Ec2Y { get; set; }
        public byte[] RsaModulusN { get; set; }
        public byte[] RsaExponentE { get; set; }
        public int? OkpCrv { get; set; }
        public byte[] OkpX { get; set; }
        public long SignCount { get; set; }
        public int[] Transports { get; set; }
        public bool UvInitialized { get; set; }
        public bool BackupEligible { get; set; }
        public bool BackupState { get; set; }
        public byte[] AttestationObject { get; set; }
        public byte[] AttestationClientDataJson { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
