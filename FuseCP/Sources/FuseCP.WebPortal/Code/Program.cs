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

#if NETCOREAPP

using System.Reflection;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using FuseCP.Web.Services;
using FuseCP.Web.Clients;
public class Program
{

	public static void Main(string[] args)
	{
		Configuration.IsPortal = true;
		Server.ConfigurationComplete = () => AssemblyLoader.Init(Configuration.ProbingPaths, Configuration.ExposeWebServices, true);
		Server.ConfigureApp = app =>
		{
			app.UseWebForms(options => options.AddHandleExtensions(".less"));
		};
		Server.ConfigureBuilder = builder =>
		{
			var es = TryLoadEnterpriseServerAssembly();
			if (es != null)
			{
				var initializer = es.GetType("FuseCP.EnterpriseServer.Code.Initializer");
				var init = initializer.GetMethod("Init");
				init?.Invoke(null, new[] { builder.Services });
			}
		};
		StartupCore.Init(args);
		
		/*var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		//builder.Services.AddRazorPages();
		//builder.Services.AddControllersWithViews();

		var app = builder.Build();

		app.UseStaticFiles();

		//app.UseAuthorization();

		app.UseWebForms();

		//app.MapDefaultControllerRoute();

		app.Run();*/

	}

	private static Assembly TryLoadEnterpriseServerAssembly()
	{
		try
		{
			return Assembly.Load("FuseCP.EnterpriseServer");
		}
		catch
		{
			// Fall back to explicit probe paths for split Portal/EnterpriseServer layouts.
		}

		var probePaths = (Configuration.ProbingPaths ?? string.Empty)
			.Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries)
			.Select(p => p.Trim())
			.Where(p => p.Length > 0);

		foreach (var probePath in probePaths)
		{
			try
			{
				var basePath = Path.IsPathRooted(probePath)
					? probePath
					: Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, probePath));
				var candidate = Path.Combine(basePath, "FuseCP.EnterpriseServer.dll");
				if (!File.Exists(candidate)) continue;

				return AssemblyLoadContext.Default.LoadFromAssemblyPath(candidate);
			}
			catch
			{
				// Try next configured path.
			}
		}

		return null;
	}
}
#endif
