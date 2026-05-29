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
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using FuseCP.Server.Utils;

namespace FuseCP.Providers.DNS
{
	/// <summary>This class is a generic helper hosting the PowerShell runtime.</summary>
	/// <remarks>It's probably a good idea to move to some utility module.</remarks>
	public class PowerShellHelper: IDisposable
	{
		private static readonly InitialSessionState s_session = null;
		private static readonly string s_dnsModuleManifestPath = Path.Combine(
			Environment.GetFolderPath( Environment.SpecialFolder.Windows ),
			"System32",
			"WindowsPowerShell",
			"v1.0",
			"Modules",
			"DnsServer",
			"DnsServer.psd1" );

		static PowerShellHelper()
		{
			s_session = InitialSessionState.CreateDefault2();
			s_session.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;
			Environment.SetEnvironmentVariable( "PSExecutionPolicyPreference", "Bypass", EnvironmentVariableTarget.Process );
		}

		public PowerShellHelper()
		{
			Log.WriteStart( "PowerShellHelper::ctor" );

			Runspace rs = RunspaceFactory.CreateRunspace( s_session );
			rs.Open();
			EnsureDnsServerCommandsAvailable( rs );
			// rs.SessionStateProxy.SetVariable( "ConfirmPreference", "none" );

			this.runSpace = rs;
			Log.WriteEnd( "PowerShellHelper::ctor" );
		}

		public void Dispose()
		{
			try
			{
				if( this.runSpace == null )
					return;
				if( this.runSpace.RunspaceStateInfo.State == RunspaceState.Opened )
					this.runSpace.Close();
				this.runSpace = null;
			}
			catch( InvalidOperationException ex )
			{
				Log.WriteError( "Runspace error", ex );
			}
			catch( RuntimeException ex )
			{
				Log.WriteError( "Runspace error", ex );
			}
		}

		public Runspace runSpace { get; private set; }

		private static void EnsureDnsServerCommandsAvailable( Runspace runspace )
		{
			using PowerShell ps = PowerShell.Create();
			ps.Runspace = runspace;

			if( HasDnsServerCommand( ps ) )
				return;

			bool preferCompatibility = IsPowerShellCore( ps );

			if( preferCompatibility )
			{
				// DNS cmdlets run reliably from a WinPS compatibility session in PowerShell Core hosts.
				ImportDnsServerModule( ps, true );
				if( HasDnsServerCommand( ps ) )
					return;

				ImportDnsServerModule( ps, false );
			}
			else
			{
				ImportDnsServerModule( ps, false );
				if( HasDnsServerCommand( ps ) )
					return;

				ImportDnsServerModule( ps, true );
			}

			if( HasDnsServerCommand( ps ) )
				return;

			throw new InvalidOperationException( "DnsServer module is loaded, but Get-DnsServerZone is still unavailable." );
		}

		private static bool IsPowerShellCore( PowerShell ps )
		{
			ps.Commands.Clear();
			ps.AddCommand( "Get-Variable" )
				.AddParameter( "Name", "PSEdition" )
				.AddParameter( "ValueOnly" )
				.AddParameter( "ErrorAction", "SilentlyContinue" );

			Collection<PSObject> result = ps.Invoke();
			if( ps.HadErrors )
			{
				ps.Streams.Error.Clear();
			}

			string edition = result?.FirstOrDefault()?.BaseObject?.ToString();
			return string.Equals( edition, "Core", StringComparison.OrdinalIgnoreCase );
		}

		private static bool HasDnsServerCommand( PowerShell ps )
		{
			ps.Commands.Clear();
			ps.AddCommand( "Get-Command" )
				.AddParameter( "Name", "Get-DnsServerZone" )
				.AddParameter( "ErrorAction", "SilentlyContinue" );

			Collection<PSObject> commands = ps.Invoke();
			if( ps.HadErrors )
			{
				ps.Streams.Error.Clear();
			}

			return commands != null && commands.Count > 0;
		}

		private static void ImportDnsServerModule( PowerShell ps, bool useWindowsPowerShellCompatibility )
		{
			ps.Commands.Clear();

			if( useWindowsPowerShellCompatibility )
			{
				ps.AddCommand( "Import-Module" )
					.AddParameter( "Name", "DnsServer" )
					.AddParameter( "UseWindowsPowerShell" )
					.AddParameter( "Force" )
					.AddParameter( "ErrorAction", "Stop" );
			}
			else if( File.Exists( s_dnsModuleManifestPath ) )
			{
				ps.AddCommand( "Import-Module" )
					.AddParameter( "Name", s_dnsModuleManifestPath )
					.AddParameter( "Force" )
					.AddParameter( "ErrorAction", "Stop" );
			}
			else
			{
				ps.AddCommand( "Import-Module" )
					.AddParameter( "Name", "DnsServer" )
					.AddParameter( "Force" )
					.AddParameter( "ErrorAction", "Stop" );
			}

			ps.Invoke();
			if( ps.HadErrors )
			{
				string importErrors = string.Join( " | ", ps.Streams.Error.Select( x => x.ToString() ) );
				throw new InvalidOperationException( "Failed to import DnsServer module. " + importErrors );
			}
		}

		public Collection<PSObject> RunPipeline( params Command[] pipelineCommands )
		{
			Collection<PSObject> results = null;
			using( Pipeline pipeLine = runSpace.CreatePipeline() )
			{
				foreach( Command cmd in pipelineCommands )
				{
					pipeLine.Commands.Add( cmd );
				}

				// Execute the pipeline and save the objects returned.
				results = pipeLine.Invoke();

				// Only non-terminating errors are delivered here.
				// Terminating errors raise exceptions instead.
				if( null != pipeLine.Error && pipeLine.Error.Count > 0 )
				{
					foreach( object item in pipeLine.Error.ReadToEnd() )
					{
						Log.WriteWarning( string.Format( "Invoke error: {0}", item ) );
					}
				}
			}
			// errors = errorList.ToArray();
			return results;
		}
	}
}
