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
using System.Threading;
using System.Threading.Tasks;
using System.Web.Security;
using WebAuthn.Net.Models.Abstractions;
using WebAuthn.Net.Services.Context;

namespace FuseCP.Portal.WebAuthn
{
	//
	// The WebAuthn.Net ceremony services create a fresh IWebAuthnContext per call
	// through IWebAuthnContextFactory and commit/dispose it internally. The portal
	// has no per-ceremony state to flush, so CommitAsync is a no-op and the context
	// simply carries the ASP.NET Core HttpContext for the lifetime of the call.
	//
	// The FormsAuthenticationTicket is captured on the WebForms request thread (where
	// HttpContext.Current is available) so the credential storage can authenticate its
	// EnterpriseServer calls even if the ceremony service hops to a thread-pool thread.
	//
	public class FuseCPWebAuthnContext : IWebAuthnContext
	{
		public const string TicketItemKey = "FuseCPWebAuthnTicket";

		public FuseCPWebAuthnContext(Microsoft.AspNetCore.Http.HttpContext httpContext, FormsAuthenticationTicket ticket)
		{
			HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			Ticket = ticket;
		}

		public Microsoft.AspNetCore.Http.HttpContext HttpContext { get; }

		public FormsAuthenticationTicket Ticket { get; }

		public Task CommitAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public ValueTask DisposeAsync()
		{
			return ValueTask.CompletedTask;
		}
	}

	public class WebAuthnContextFactory : IWebAuthnContextFactory<FuseCPWebAuthnContext>
	{
		public Task<FuseCPWebAuthnContext> CreateAsync(Microsoft.AspNetCore.Http.HttpContext httpContext, CancellationToken cancellationToken)
		{
			FormsAuthenticationTicket ticket = httpContext?.Items[FuseCPWebAuthnContext.TicketItemKey] as FormsAuthenticationTicket;
			return Task.FromResult(new FuseCPWebAuthnContext(httpContext, ticket));
		}
	}
}
