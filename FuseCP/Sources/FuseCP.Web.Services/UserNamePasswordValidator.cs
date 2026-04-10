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
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using CoreWCF;
using System.Text;
using System.Threading.Tasks;
using BaseUserNamePasswordValidator = CoreWCF.IdentityModel.Selectors.UserNamePasswordValidator;

namespace FuseCP.Web.Services
{
	public class UserNamePasswordValidator : BaseUserNamePasswordValidator
	{

		public PolicyAttribute Policy;

		public override ValueTask ValidateAsync(string userName, string password)
		{
			if (Policy != null)
			{
				if (Policy.Policy == PolicyAttribute.ServerAuthenticated)
				{
					if (ValidateServer == null) throw new NotSupportedException("Server authentication callback is not configured.");
					if (!ValidateServer(password)) throw new FaultException("Invalid server password");
				}
				else if (Policy.Policy == PolicyAttribute.EnterpriseServerAuthenticated)
				{
					if (ValidateEnterpriseServer == null) throw new NotSupportedException("Enterprise authentication callback is not configured.");
					if (!ValidateEnterpriseServer(userName, password)) throw new FaultException("Invalid user or password");
				}
				else if (Policy.Policy == PolicyAttribute.Encrypted) { } // do not require username & password
				else throw new NotSupportedException($"Unuspported policy {Policy.Policy} on service.");
				
			}

			return ValueTask.CompletedTask;
		}

		public static Func<string, bool> ValidateServer;
		public static Func<FuseCP.Providers.ServerRequestAuthenticationData, bool> ValidateServerRequestAuthentication;
		public static Func<string, string, bool> ValidateEnterpriseServer;

	}
}
