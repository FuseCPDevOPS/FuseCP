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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebAuthn.Net.Configuration.DependencyInjection;
using WebAuthn.Net.Services.Common.AttestationStatementVerifier.Abstractions.Packed;
using WebAuthn.Net.Storage.AuthenticationCeremony.Implementation;
using WebAuthn.Net.Storage.RegistrationCeremony.Implementation;

namespace FuseCP.Portal.WebAuthn
{
	//
	// Central registration point for the WebAuthn.Net ceremony services used by the
	// portal. Register() is invoked during service configuration and SetProvider()
	// is invoked once the WebApplication is built, so page code-behind can resolve
	// the ceremony services through Provider without touching DI directly.
	//
	public static class WebAuthnServices
	{
		private static IServiceProvider _provider;

		public static IServiceProvider Provider
		{
			get
			{
				IServiceProvider provider = _provider;
				if (provider == null)
					throw new InvalidOperationException("WebAuthn services are not initialized. Call WebAuthnServices.SetProvider from Program.ConfigureApp.");
				return provider;
			}
		}

		public static void Register(IServiceCollection services)
		{
			// The page code-behind bridges ASP.NET WebForms to the ASP.NET Core
			// HttpContext through IHttpContextAccessor, which the ceremony services
			// also depend on internally.
			services.AddHttpContextAccessor();

			// The portal does not ingest FIDO MDS3 metadata, so the default packed
			// attestation verifier (which consults MDS to anchor a "packed" statement
			// with x5c to a known attestation root) would reject every such
			// authenticator, e.g. Windows Hello / TPM passkeys. Register a verifier
			// that verifies the attestation signature and accepts such statements as
			// self-attested when no MDS data is available. It must be registered
			// before AddWebAuthnCore, whose TryAddSingleton calls would otherwise keep
			// the default verifier.
			services.TryAddSingleton<IPackedAttestationStatementVerifier<FuseCPWebAuthnContext>, FuseCPWebAuthnPackedAttestationStatementVerifier>();

			// Self-attestation passkeys (attestation preference "none") do not require
			// FIDO MDS3 metadata.
			//
			// The default ceremony storages persist the registration/authentication
			// ceremony state (challenge, expected origin, rpId) in the "webauthnr"
			// and "webauthna" cookies with SecurePolicy.Always and SameSite.None.
			// Browsers refuse to store or send a Secure cookie set from a plain HTTP
			// page, so on any non-HTTPS deployment the completion postback never
			// returns the ceremony cookie and every passkey registration and sign-in
			// fails. SameSite=None without Secure is also rejected by browsers, so
			// SameSite is switched to Lax: both postbacks are same-site requests.
			// SecurePolicy.SameAsRequest keeps the Secure attribute on HTTPS
			// deployments and drops it on HTTP, so both work.
			services.AddWebAuthnCore<FuseCPWebAuthnContext>(configureBackgroundIngest: ingest => ingest.ThrowExceptionOnFailure = false)
				.AddContextFactory<FuseCPWebAuthnContext, WebAuthnContextFactory>()
				.AddCredentialStorage<FuseCPWebAuthnContext, WebAuthnCredentialStorage>()
				.AddDefaultStorages<FuseCPWebAuthnContext>(
					configureRegistration: options => ConfigureCeremonyCookie(options.Cookie),
					configureAuthentication: options => ConfigureCeremonyCookie(options.Cookie));

			// The FIDO Metadata Service background ingest is a hosted service that
			// downloads fidoalliance.org at startup. It has no disable flag, and its
			// StartAsync escapes network failures (e.g. HTTP 429) and crashes the host.
			// Since self-attested passkeys never consult MDS data, remove its
			// registration. IFidoMetadataSearchService stays registered and simply
			// returns no data.
			Type ingestHostedService = typeof(global::WebAuthn.Net.Services.FidoMetadata.Implementation.FidoMetadataBackgroundIngest.FidoMetadataBackgroundIngestHostedService);
			for (int i = services.Count - 1; i >= 0; i--)
			{
				if (services[i].ImplementationType == ingestHostedService)
					services.RemoveAt(i);
			}
		}

		// Shared cookie settings for the registration ("webauthnr") and
		// authentication ("webauthna") ceremony cookies. See the comment in
		// Register for why the WebAuthn.Net defaults do not work on HTTP.
		private static void ConfigureCeremonyCookie(CookieBuilder cookie)
		{
			cookie.SameSite = SameSiteMode.Lax;
			cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
		}

		public static void SetProvider(IServiceProvider provider)
		{
			_provider = provider;
		}
	}
}
