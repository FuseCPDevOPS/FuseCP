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

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using WebAuthn.Net.Models;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Services.Common.AttestationStatementDecoder.Models.AttestationStatements;
using WebAuthn.Net.Services.Common.AttestationStatementVerifier.Implementation.Packed;
using WebAuthn.Net.Services.Common.AttestationStatementVerifier.Models.AttestationStatementVerifier;
using WebAuthn.Net.Services.Common.AttestationStatementVerifier.Models.Enums;
using WebAuthn.Net.Services.Common.AuthenticatorDataDecoder.Models;
using WebAuthn.Net.Services.Cryptography.Sign;
using WebAuthn.Net.Services.FidoMetadata;
using WebAuthn.Net.Services.FidoMetadata.Models.FidoMetadataService;
using WebAuthn.Net.Services.Providers;
using WebAuthn.Net.Services.Serialization.Asn1;

namespace FuseCP.Portal.WebAuthn
{
	//
	// The portal does not ingest FIDO MDS data (see WebAuthnServices.Register),
	// so the default packed attestation verifier, which consults MDS to anchor a
	// "packed" statement with x5c to a known attestation root, fails for every
	// such authenticator. On a Windows PC that means Windows Hello / TPM passkeys
	// can never be registered. This verifier keeps the default behavior when MDS
	// data is available and, when it is not, verifies the attestation signature
	// with the attestation certificate and accepts the statement as self-attested
	// (accepted by the default trust path validator and by the default
	// AttestationTypes.Self.IsAcceptable policy).
	//
	public class FuseCPWebAuthnPackedAttestationStatementVerifier : DefaultPackedAttestationStatementVerifier<FuseCPWebAuthnContext>
	{
		public FuseCPWebAuthnPackedAttestationStatementVerifier(
			ITimeProvider timeProvider,
			IDigitalSignatureVerifier signatureVerifier,
			IAsn1Deserializer asn1Deserializer,
			IFidoMetadataSearchService<FuseCPWebAuthnContext> fidoMetadataSearchService)
			: base(timeProvider, signatureVerifier, asn1Deserializer, fidoMetadataSearchService)
		{
		}

		protected override async Task<Result<VerifiedAttestationStatement>> VerifyPackedWithX5CAsync(
			FuseCPWebAuthnContext context,
			PackedAttestationStatement attStmt,
			AttestedAuthenticatorData authenticatorData,
			byte[] clientDataHash,
			IReadOnlyCollection<X509Certificate2> certificates,
			byte[][] trustPath,
			CancellationToken cancellationToken)
		{
			FidoMetadataResult metadata = await FidoMetadataSearchService.FindMetadataByAaguidAsync(
				context,
				authenticatorData.AttestedCredentialData.Aaguid,
				cancellationToken);

			if (metadata != null)
				return await base.VerifyPackedWithX5CAsync(
					context,
					attStmt,
					authenticatorData,
					clientDataHash,
					certificates,
					trustPath,
					cancellationToken);

			X509Certificate2 attestationCertificate = certificates.First();
			byte[] dataToVerify = Concat(authenticatorData.Raw, clientDataHash);
			if (!SignatureVerifier.IsValidCertificateSign(attestationCertificate, attStmt.Alg, dataToVerify, attStmt.Sig))
				return Result<VerifiedAttestationStatement>.Fail();

			return Result<VerifiedAttestationStatement>.Success(
				new VerifiedAttestationStatement(AttestationStatementFormat.Packed, AttestationType.Self, null, null));
		}
	}
}
