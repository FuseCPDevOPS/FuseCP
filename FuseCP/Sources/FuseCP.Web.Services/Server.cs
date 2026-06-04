// Copyright (C) 2025 FuseCP
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
using System.Net;
using FuseCP.Providers.OS;

using CoreWCF;
using CoreWCF.Channels;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FuseCP.Web.Services
{
	public static class Server
	{
		private const string LoopbackAddress = "127.0.0.1";

		public static string WebRoot { get; set; } = null;
		public static string ContentRoot { get; set; } = null;
		public static string MapPath(string path) => path.Replace("~", ContentRoot);

		private static bool IsLoopback(string address)
		{
			if (string.IsNullOrWhiteSpace(address))
				return false;

			if (!IPAddress.TryParse(address, out var ipAddress))
				return false;

			return IPAddress.IsLoopback(ipAddress);
		}

		private static string ExtractIp(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			var candidate = value.Trim().Trim('"');
			if (candidate.Equals("unknown", StringComparison.OrdinalIgnoreCase))
				return null;

			if (candidate.StartsWith("[", StringComparison.Ordinal) && candidate.Contains("]", StringComparison.Ordinal))
			{
				candidate = candidate.Substring(1, candidate.IndexOf(']') - 1);
			}

			if (IPAddress.TryParse(candidate, out var ip))
				return ip.ToString();

			var lastColon = candidate.LastIndexOf(':');
			if (lastColon > 0)
			{
				var withoutPort = candidate.Substring(0, lastColon);
				if (withoutPort.Count(c => c == ':') == 0 && IPAddress.TryParse(withoutPort, out ip))
					return ip.ToString();
			}

			return null;
		}

		private static string GetForwardedAddress(HttpRequestMessageProperty request)
		{
			if (request?.Headers == null)
				return null;

			var xForwardedFor = request.Headers["X-Forwarded-For"];
			if (!string.IsNullOrWhiteSpace(xForwardedFor))
			{
				foreach (var segment in xForwardedFor.Split(','))
				{
					var ip = ExtractIp(segment);
					if (!string.IsNullOrWhiteSpace(ip))
						return ip;
				}
			}

			var xRealIp = ExtractIp(request.Headers["X-Real-IP"]);
			if (!string.IsNullOrWhiteSpace(xRealIp))
				return xRealIp;

			var forwarded = request.Headers["Forwarded"];
			if (!string.IsNullOrWhiteSpace(forwarded))
			{
				foreach (var item in forwarded.Split(','))
				{
					foreach (var token in item.Split(';'))
					{
						var pair = token.Split(new[] { '=' }, 2);
						if (pair.Length != 2 || !pair[0].Trim().Equals("for", StringComparison.OrdinalIgnoreCase))
							continue;

						var ip = ExtractIp(pair[1]);
						if (!string.IsNullOrWhiteSpace(ip))
							return ip;
					}
				}
			}

			return null;
		}

		public static string UserHostAddress
		{
			get
			{
				OperationContext context = OperationContext.Current;
				if (context == null) return LoopbackAddress;

				MessageProperties prop = context.IncomingMessageProperties;
				if (prop == null) return LoopbackAddress;

				var remoteAddress = LoopbackAddress;
				if (prop.TryGetValue(RemoteEndpointMessageProperty.Name, out var endpointValue))
				{
					RemoteEndpointMessageProperty endpoint = endpointValue as RemoteEndpointMessageProperty;
					if (!string.IsNullOrWhiteSpace(endpoint?.Address))
						remoteAddress = endpoint.Address;
				}

				if (!IsLoopback(remoteAddress))
					return remoteAddress;

				if (prop.TryGetValue(HttpRequestMessageProperty.Name, out var requestValue))
				{
					var forwardedAddress = GetForwardedAddress(requestValue as HttpRequestMessageProperty);
					if (!string.IsNullOrWhiteSpace(forwardedAddress))
						return forwardedAddress;
				}

				return remoteAddress;
			}
		}

		public static Action<WebApplication> ConfigureApp = null;
		public static Action ConfigurationComplete = null;
		public static Action<WebApplicationBuilder> ConfigureBuilder = null;

		public static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();
	}
}
