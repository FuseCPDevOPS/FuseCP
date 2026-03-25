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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CoreWCF;
using System.Text;
using System.Threading.Tasks;

namespace FuseCP.Web.Services
{
	public class ServiceType
	{
		public Type Service;
		public Type Contract;

		public ServiceType(Type type)
		{
			Service = type;
			Contract = type.GetInterfaces().FirstOrDefault(i => i.GetCustomAttribute<ServiceContractAttribute>() != null);
		}
	}

	public class ServiceTypes: KeyedCollection<string, ServiceType>
	{

		static string exposeWebServices = null;
		public static string ExposeWebServices
		{
			get
			{
				if (exposeWebServices == null)
				{
					exposeWebServices = Configuration.ExposeWebServices.ToLower();
				}
				return exposeWebServices;
			}
		}

		public static Assembly[] assemblies = null;

        public static Assembly[] ExposedAssemblies
		{
			get
			{
				if (assemblies != null) return assemblies;

				Assembly eserver = null, server = null;

				if (ExposeWebServices == "" || ExposeWebServices == "all" || ExposeWebServices == "true" ||
					ExposeWebServices.Split(';', ',').Any(s => s.Trim() == "enterpriseserver"))
				{
					try
					{
						eserver = Assembly.Load("FuseCP.EnterpriseServer");
					}
					catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
				}

				if (ExposeWebServices == "" || ExposeWebServices == "all" || ExposeWebServices == "true" ||
					ExposeWebServices.Split(';', ',').Any(s => s.Trim() == "server"))
				{
					try
					{
						server = Assembly.Load("FuseCP.Server");
					}
					catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
				}

				assemblies = new Assembly[]
				{
					eserver, server
				}
				.Where(a => a != null)
				.ToArray();

				return assemblies;
			}
		}

        public bool IsServerLoaded => AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.GetName().Name == "FuseCP.Server");
        public bool IsEnterpriseServerLoaded => AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.GetName().Name == "FuseCP.EnterpriseServer");
        public bool IsPortalLoaded => AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.GetName().Name == "FuseCP.WebPortal");

        public static IEnumerable<Type> GetWebServices()
		{
			var local_types = ExposedAssemblies
				.SelectMany(a => {
					var attrTypes = a.GetCustomAttribute<WCFServiceTypesAttribute>()?.Types;
					return attrTypes ?? new Type[0];
				});
			return local_types;
		}

		protected override string GetKeyForItem(ServiceType type) => type.Service.Name;

		static ServiceTypes types = null;
		public static ServiceTypes Types
		{
			get
			{
				if (types == null)
				{
					types = new ServiceTypes();
					foreach (var type in GetWebServices())
					{
						types.Add(new ServiceType(type));
					}
				}
				return types;
			}
		}
	}
}


