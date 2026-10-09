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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#if NetCore
using Microsoft.EntityFrameworkCore;
#endif

namespace FuseCP.EnterpriseServer.Data.Entities
{
	[Table("WebAuthnCredentials")]
#if NetCore
	[Index("UserId", Name = "WebAuthnCredentialsIdx_UserId")]
	[Index("CredentialId", Name = "WebAuthnCredentialsIdx_CredentialId")]
#endif
	public class WebAuthnCredential
	{
		[Key]
		[Column("ID")]
		public int Id { get; set; }

		[Column("UserId")]
		public int UserId { get; set; }

		[StringLength(255)]
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

		[StringLength(50)]
		public string Transports { get; set; }

		public bool UvInitialized { get; set; }

		public bool BackupEligible { get; set; }

		public bool BackupState { get; set; }

		public byte[] AttestationObject { get; set; }

		public byte[] AttestationClientDataJson { get; set; }

		[StringLength(255)]
		public string DisplayName { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
