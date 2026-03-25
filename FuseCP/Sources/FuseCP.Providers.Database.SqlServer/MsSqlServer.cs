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
using System.IO;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Versioning;

using FuseCP.Server.Utils;
using FuseCP.Providers.Utils;
using Microsoft.Win32;

namespace FuseCP.Providers.Database
{
	public class MsSqlServerMicrosoftData : HostingServiceProviderBase, IDatabaseServer
	{
		#region Properties
		protected string ServerName
		{
			get { return ProviderSettings["InternalAddress"]; }
		}

		protected string DatabaseCollation
		{
			get { return ProviderSettings["DatabaseCollation"]; }
		}

		protected string SaLogin
		{
			get { return ProviderSettings["SaLogin"]; }
		}

		protected string SaPassword
		{
			get { return ProviderSettings["SaPassword"]; }
		}

		protected bool UseTrustedConnection
		{
			get { return ProviderSettings.GetBool("UseTrustedConnection"); }
		}

		protected bool TrustServerCertificate
		{
			get { return ProviderSettings.GetBool("TrustServerCertificate"); }
		}

		protected string ConnectionString
		{
			get
			{
				string connectionString = String.Format("Server={0};User id={1};Password={2};Database=master;{3}",
					 ServerName, SaLogin, SaPassword, TrustServerCertificate ? "TrustServerCertificate=true" : "");

				if (UseTrustedConnection)
					connectionString = String.Format("Server={0};Integrated security=SSPI;Database=master;{1}",
					ServerName, TrustServerCertificate ? "TrustServerCertificate=true" : "");

				return connectionString;
			}
		}

		protected string DatabaseBackupLocation
		{
			get { return ProviderSettings["DatabaseBackupLocation"]; }
		}

		protected string DatabaseBackupNetworkPath
		{
			get { return ProviderSettings["DatabaseBackupNetworkPath"]; }
		}
		#endregion

		#region Databases
		private string GetSafeConnectionString(string databaseName, string username, string password, bool trustServerCertificate)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));
			ValidateSqlLoginInput(username, nameof(username));

			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
			{
				DataSource = ServerName,
				InitialCatalog = databaseName,
				TrustServerCertificate = trustServerCertificate,
				IntegratedSecurity = false,
				UserID = username,
				Password = password ?? String.Empty
			};

			return builder.ConnectionString;
		}

		public virtual bool CheckConnectivity(string databaseName, string username, string password)
		{
			SqlConnection conn = new SqlConnection(GetSafeConnectionString(databaseName, username, password, TrustServerCertificate));
			try
			{
				conn.Open();
			}
			catch
			{
				return false;
			}
			conn.Close();
			return true;
		}

		public virtual DataSet ExecuteSqlQuerySafe(string databaseName, string username, string password, string commandText)
		{
			return ExecuteSqlQuery(databaseName, commandText,
				 GetSafeConnectionString(databaseName, username, password, TrustServerCertificate));
		}

		public virtual void ExecuteSqlNonQuerySafe(string databaseName, string username, string password, string commandText)
		{
			ExecuteSqlNonQuery(databaseName, commandText,
				 GetSafeConnectionString(databaseName, username, password, TrustServerCertificate));
		}

		public virtual DataSet ExecuteSqlQuery(string databaseName, string commandText)
		{
			return ExecuteSqlQuery(databaseName, commandText, ConnectionString);
		}

		public virtual DataSet ExecuteSqlQuery(string databaseName, string commandText, string connectionString)
		{
			connectionString = NormalizeConnectionString(connectionString);
			commandText = "USE " + QuoteSqlIdentifier(databaseName) + "; " + commandText;
			return ExecuteQuery(commandText, connectionString);
		}

		public virtual void ExecuteSqlNonQuery(string databaseName, string commandText)
		{
			ExecuteSqlNonQuery(databaseName, commandText, ConnectionString);
		}

		public virtual void ExecuteSqlNonQuery(string databaseName, string commandText, string connectionString)
		{
			connectionString = NormalizeConnectionString(connectionString);
			commandText = "USE " + QuoteSqlIdentifier(databaseName) + "\nGO\n" + commandText;

			SqlConnection connection = new SqlConnection(connectionString);

			try
			{
				// iterate through "GO" delimited command text
				StringReader reader = new StringReader(commandText);

				using SqlCommand command = new SqlCommand();

				connection.Open();
				command.Connection = connection;
				command.CommandType = System.Data.CommandType.Text;
				command.CommandTimeout = 600; // 10 minutes

				string sql = "";
				while (null != (sql = ReadNextStatementFromStream(reader)))
				{
					command.CommandText = sql;
					command.ExecuteNonQuery();
				}

				reader.Close();
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				throw new Exception("Can't run SQL script", ex);
			}

			connection.Close();
		}

		private string ReadNextStatementFromStream(StringReader reader)
		{
			StringBuilder sb = new StringBuilder();
			string lineOfText;

			while (true)
			{
				lineOfText = reader.ReadLine();
				if (lineOfText == null)
				{
					if (sb.Length > 0)
					{
						return sb.ToString();
					}
					else
					{
						return null;
					}
				}

				if (lineOfText.TrimEnd().ToUpper() == "GO")
				{
					break;
				}

				sb.Append(lineOfText + Environment.NewLine);
			}

			return sb.ToString();
		}

		public virtual bool DatabaseExists(string databaseName)
		{
			return ExecuteQuery(
				"select name from master..sysdatabases where name = @DatabaseName",
				CreateNVarCharParameter("@DatabaseName", databaseName)).Tables[0].Rows.Count > 0;
		}

		public virtual string[] GetDatabases()
		{
			DataTable dt = ExecuteQuery("select name from master..sysdatabases where name not in ('master', 'tempdb', 'model', 'msdb')").Tables[0];
			List<string> databases = new List<string>();
			foreach (DataRow dr in dt.Rows)
				databases.Add(dr["name"].ToString());
			return databases.ToArray();
		}

		public virtual SqlDatabase GetDatabase(string databaseName)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));

			if (!DatabaseExists(databaseName))
				return null;

			SqlDatabase database = new SqlDatabase();
			database.Name = databaseName;

			// get database size
			DataView dvFiles = ExecuteQuery(String.Format(
				 "SELECT Status, (Size * 8) AS DbSize, Name, FileName FROM {0}..sysfiles", QuoteSqlIdentifier(databaseName))).Tables[0].DefaultView;

			foreach (DataRowView drFile in dvFiles)
			{
				int status = (int)drFile["Status"];
				if ((status & 64) == 0)
				{
					// data file
					database.DataName = ((string)drFile["Name"]).Trim();
					database.DataPath = ((string)drFile["FileName"]).Trim();
					database.DataSize = (int)drFile["DbSize"];
				}
				else
				{
					// log file
					database.LogName = ((string)drFile["Name"]).Trim();
					database.LogPath = ((string)drFile["FileName"]).Trim();
					database.LogSize = (int)drFile["DbSize"];
				}
			}

			// get database uzers
			database.Users = GetDatabaseUsers(databaseName);
			return database;
		}

		private string CreateFileNameString(string fileName, int fileSize)
		{
			string str = fileSize == 0 ? string.Format(" FILENAME = '{0}' ", EscapeSql(fileName)) :
				 string.Format(" FILENAME = '{0}', MAXSIZE = {1} ", EscapeSql(fileName), fileSize);

			return str;
		}

		public virtual void CreateDatabase(SqlDatabase database)
		{
			ValidateSqlIdentifierInput(database.Name, nameof(database.Name));
			ValidateSqlLoginInputs(database.Users, nameof(database.Users));

			if (database.Users == null)
				database.Users = new string[0];

			string commandText = "";
			if (String.IsNullOrEmpty(database.Location))
			{
				// load default location
				SqlDatabase dbMaster = GetDatabase("master");
				database.Location = Path.GetDirectoryName(dbMaster.DataPath);
			}
			else
			{
				// subst vars
				database.Location = FileUtils.EvaluateSystemVariables(database.Location);

				// verify folder exists
				if (!Directory.Exists(database.Location))
					Directory.CreateDirectory(database.Location);
			}

			string collation = GetValidatedCollationClause();

			// create command
			string dataFile = Path.Combine(database.Location, database.Name) + "_data.mdf";
			string logFile = Path.Combine(database.Location, database.Name) + "_log.ldf";


			commandText = string.Format("CREATE DATABASE {0}" +
					  " ON ( NAME = '{1}_data', {2})" +
					  " LOG ON ( NAME = '{3}_log', {4}){5};",
					  QuoteSqlIdentifier(database.Name),
					  EscapeSql(database.Name),
					  CreateFileNameString(dataFile, database.DataSize),
					  EscapeSql(database.Name),
					  CreateFileNameString(logFile, database.LogSize),
					  collation);


			// create database
			ExecuteNonQuery(commandText);

			// grant users access
			UpdateDatabaseUsers(database.Name, database.Users);
		}

		public virtual void UpdateDatabase(SqlDatabase database)
		{
			ValidateSqlIdentifierInput(database.Name, nameof(database.Name));
			ValidateSqlLoginInputs(database.Users, nameof(database.Users));

			if (database.Users == null)
				database.Users = new string[0];

			// grant users access
			UpdateDatabaseUsers(database.Name, database.Users);
		}

		public virtual void DeleteDatabase(string databaseName)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));

			if (!DatabaseExists(databaseName))
				return;

			// get database details
			SqlDatabase db = GetDatabase(databaseName);

			// remove all users from database
			try
			{
				string[] users = GetDatabaseUsers(databaseName);
				foreach (string user in users)
					RemoveUserFromDatabase(databaseName, user);
			}
			catch
			{
				// ignore user deletion
			}

			// close all connection
			CloseDatabaseConnections(databaseName);

			// drop database
			ExecuteNonQuery(String.Format("DROP DATABASE {0}", QuoteSqlIdentifier(databaseName)));

			// drop database folder if empty
			string dbFolder = Path.GetDirectoryName(db.DataPath);
			try
			{
				if (Directory.GetFileSystemEntries(dbFolder).Length == 0)
					Directory.Delete(dbFolder);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				Log.WriteError(String.Format("Error deleting '{0}' database folder", dbFolder), ex);
			}
		}

		#endregion

		#region Users
		public virtual bool UserExists(string username)
		{
			ValidateSqlLoginInput(username, nameof(username));

			return ExecuteQuery(
				"select name from master..syslogins where name = @UserName",
				CreateNVarCharParameter("@UserName", username)).Tables[0].Rows.Count > 0;
		}

		public virtual string[] GetUsers()
		{
			DataTable dt = ExecuteQuery("select name from master..syslogins where isntname = 0 and sysadmin = 0 and password is not null").Tables[0];
			List<string> users = new List<string>();
			foreach (DataRow dr in dt.Rows)
				users.Add(dr["name"].ToString());
			return users.ToArray();
		}

		public virtual SqlUser GetUser(string username, string[] allDatabases)
		{
			ValidateSqlLoginInput(username, nameof(username));

			// get user information
			SqlUser user = new SqlUser();

			DataView dvUser = ExecuteQuery("select dbname from master..syslogins where name = @UserName",
				CreateNVarCharParameter("@UserName", username)).Tables[0].DefaultView;

			user.Name = username;
			user.DefaultDatabase = "";
			if (dvUser.Count > 0)
			{
				object dbname = dvUser[0]["dbname"];
				user.DefaultDatabase = (dbname != null && dbname != DBNull.Value) ? (string)dbname : "";
			}

			// get user databases
			user.Databases = GetUserDatabases(username, allDatabases);

			return user;
		}

		public virtual void CreateUser(SqlUser user, string password)
		{
			ValidateSqlLoginInput(user.Name, nameof(user.Name));

			if (user.Databases == null)
				user.Databases = new string[0];

			ValidateSqlIdentifierInputs(user.Databases, nameof(user.Databases));

			// create user account
			if (user.DefaultDatabase == null || user.DefaultDatabase == "")
				user.DefaultDatabase = "master";

			ValidateSqlIdentifierInput(user.DefaultDatabase, nameof(user.DefaultDatabase));

			//ExecuteNonQuery(String.Format("EXEC sp_addlogin '{0}', '{1}', '{2}'",
			//    user.Name, password, user.DefaultDatabase));
			//Fixed create login with "Enforce password policy" disabled.
			ExecuteNonQuery(
				 String.Format("CREATE LOGIN {0} WITH PASSWORD=@Password, DEFAULT_DATABASE={1}, CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF",
					  QuoteSqlIdentifier(user.Name), QuoteSqlIdentifier(user.DefaultDatabase)),
				 CreateNVarCharParameter("@Password", password));

			// add access to databases
			foreach (string database in user.Databases)
				AddUserToDatabase(database, user.Name);
		}


		public virtual void UpdateUser(SqlUser user, string[] allDatabases)
		{
			ValidateSqlLoginInput(user.Name, nameof(user.Name));

			if (user.Databases == null)
				user.Databases = new string[0];

			ValidateSqlIdentifierInputs(user.Databases, nameof(user.Databases));

			// update user's default database
			if (user.DefaultDatabase == null || user.DefaultDatabase == "")
				user.DefaultDatabase = "master";

			ValidateSqlIdentifierInput(user.DefaultDatabase, nameof(user.DefaultDatabase));

			ExecuteNonQuery("EXEC sp_defaultdb @UserName, @DefaultDatabase",
				CreateNVarCharParameter("@UserName", user.Name),
				CreateNVarCharParameter("@DefaultDatabase", user.DefaultDatabase));


			// update user databases access
			UpdateUserDatabases(user.Name, user.Databases, allDatabases);

			// change user password if required
			if (user.Password != "")
				ChangeUserPassword(user.Name, user.Password);
		}
		public virtual void DeleteUser(string username, string[] allDatabases)
		{
			ValidateSqlLoginInput(username, nameof(username));

			// remove user from databases
			string[] userDatabases = GetUserDatabases(username, allDatabases);
			foreach (string database in userDatabases)
				RemoveUserFromDatabase(database, username);

			// close all user connection
			CloseUserConnections(username);

			// drop login
			ExecuteNonQuery("EXEC sp_droplogin @UserName",
				CreateNVarCharParameter("@UserName", username));
		}

		public virtual void ChangeUserPassword(string username, string password)
		{
			ValidateSqlLoginInput(username, nameof(username));

			// change user password
			ExecuteNonQuery("EXEC sp_password @new=@NewPassword, @loginame=@LoginName",
				CreateNVarCharParameter("@NewPassword", password),
				CreateNVarCharParameter("@LoginName", username));
		}

		#endregion

		#region Database log routines
		public virtual void TruncateDatabase(string databaseName)
		{
			SqlDatabase database = GetDatabase(databaseName);
			ExecuteNonQuery(String.Format(@"USE {0};DBCC SHRINKFILE ({1}, 1);",
				QuoteSqlIdentifier(databaseName), QuoteSqlStringLiteral(database.LogName)));
		}
		#endregion

		#region Backup databases
		public virtual byte[] GetTempFileBinaryChunk(string path, int offset, int length)
		{
			CheckTempPath(path);

			byte[] buffer = FileUtils.GetFileBinaryChunk(path, offset, length);

			// delete temp file
			if (buffer.Length < length)
				FileUtils.DeleteFile(path);
			return buffer;
		}

		public virtual string AppendTempFileBinaryChunk(string fileName, string path, byte[] chunk)
		{
			if (path == null)
			{
				path = Path.Combine(Path.GetTempPath(), fileName);
				if (FileUtils.FileExists(path))
					FileUtils.DeleteFile(path);
			}
			else
			{
				CheckTempPath(path);
			}

			FileUtils.AppendFileBinaryContent(path, chunk);

			return path;
		}

		public virtual string BackupDatabase(string databaseName, string backupFileName, bool zipBackupFile)
		{
			string bakFile = BackupBak(databaseName, (zipBackupFile ? null : backupFileName));

			/*
			if (createBackup)
			{
				 files = BackupBak(databaseName, (zipBackup ? null : backupName));
			}
			else
			{
				 files = BackupMdf(databaseName);
			}
			 * */

			// zip backup file
			if (zipBackupFile)
			{
				string zipFile = Path.Combine(Path.GetTempPath(), backupFileName);
				string zipRoot = Path.GetDirectoryName(bakFile);

				// zip files
				FileUtils.ZipFiles(zipFile, zipRoot, new string[] { Path.GetFileName(bakFile) });

				// delete data files
				if (String.Compare(bakFile, zipFile, true) != 0)
					FileUtils.DeleteFile(bakFile);

				bakFile = zipFile;
			}

			return bakFile;
		}

		private string BackupBak(string databaseName, string backupName)
		{
			if (backupName == null)
				backupName = databaseName + ".bak";

			string bakFile;
			if (DatabaseBackupLocation != "" && DatabaseBackupNetworkPath != "")
			{
				bakFile = Path.Combine(DatabaseBackupLocation, backupName);
				string networkBakFile = DatabaseBackupNetworkPath + "/" + backupName;

				// backup database
				Log.WriteInfo("Backing up Database {0} \n Network disk {1} \n Local Disk {2}", databaseName, networkBakFile, bakFile);
				ExecuteNonQuery(String.Format(@"BACKUP DATABASE {0} TO DISK = {1}",
					 QuoteSqlIdentifier(databaseName), QuoteSqlUnicodeLiteral(networkBakFile)));

			}
			else
			{
				string tempPath = Path.GetTempPath();
				bakFile = Path.Combine(tempPath, backupName);
				//string backupName = databaseName + " Database Backup";

				// backup database
				Log.WriteInfo("Backing up Database {0} \n Local Disk {2}", databaseName, bakFile);
				ExecuteNonQuery(String.Format(@"BACKUP DATABASE {0} TO DISK = {1}",
					 QuoteSqlIdentifier(databaseName), QuoteSqlUnicodeLiteral(bakFile)));
			}


			return bakFile;
		}

		private string[] BackupMdf(string databaseName)
		{
			string tempPath = Path.GetTempPath();

			// get database files
			SqlDatabase db = GetDatabase(databaseName);
			string[] files = new string[] { db.DataPath, db.LogPath };

			// close current database connections
			CloseDatabaseConnections(databaseName);

			// Detach database
			DetachDatabase(databaseName);

			try
			{

				// copy database files
				string[] destFiles = new string[files.Length];
				for (int i = 0; i < files.Length; i++)
				{
					destFiles[i] = Path.Combine(tempPath, Path.GetFileName(files[i]));
					FileUtils.CopyFile(files[i], destFiles[i]);
				}
				return destFiles;
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				throw new Exception("Can't detach/copy database", ex);
			}
			finally
			{
				// Attach Database
				AttachDatabase(databaseName, files);
			}
		}

		private void DetachDatabase(string databaseName)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));
			ExecuteNonQuery("EXEC sp_detach_db @DbName",
				CreateNVarCharParameter("@DbName", databaseName));
		}

		private void AttachDatabase(string databaseName, string[] files)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));

			List<SqlParameter> parameters = new List<SqlParameter>();
			string[] sqlFiles = new string[files.Length];
			for (int i = 0; i < files.Length; i++)
			{
				sqlFiles[i] = "@File" + i;
				parameters.Add(CreateNVarCharParameter(sqlFiles[i], files[i]));
			}

			// create command
			string cmdText = String.Format("EXEC sp_attach_db @DbName, {0}",
				String.Join(",", sqlFiles));
			parameters.Insert(0, CreateNVarCharParameter("@DbName", databaseName));

			ExecuteNonQuery(cmdText, parameters.ToArray());
		}

		private void AttachSingleFileDatabase(string databaseName, string file)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));

			// execute command
			ExecuteNonQuery("EXEC sp_attach_single_file_db @dbname=@DbName, @physname=@PhysName",
				CreateNVarCharParameter("@DbName", databaseName),
				CreateNVarCharParameter("@PhysName", file));
		}
		#endregion

		#region Restore databases
		public virtual void RestoreDatabase(string databaseName, string[] files)
		{
			string tempPath = Path.GetTempPath();

			//create folder with unique name to avoid getting all files from temp directory
			string zipPath = Path.Combine(tempPath, Guid.NewGuid().ToString());

			// store original database information
			SqlDatabase database = GetDatabase(databaseName);

			// unzip uploaded files if required
			List<string> expandedFiles = new List<string>();
			foreach (string file in files)
			{
				if (Path.GetExtension(file).ToLower() == ".zip")
				{
					// unpack file
					expandedFiles.AddRange(FileUtils.UnzipFiles(file, zipPath));

					// delete zip archive
					FileUtils.DeleteFile(file);
				}
				else
				{
					// just add file to the collection
					expandedFiles.Add(file);
				}
			}

			files = new string[expandedFiles.Count];
			expandedFiles.CopyTo(files, 0);

			// analyze uploaded files
			bool fromBackup = true;
			foreach (string file in files)
			{
				if (Path.GetExtension(file).ToLower() == ".mdf")
				{
					fromBackup = false;
					break;
				}
			}

			// restore database
			if (fromBackup)
			{
				// restore from .BAK file
				RestoreFromBackup(database, files);
				//delete temporary folder for zip contents
				if (FileUtils.DirectoryExists(zipPath))
				{
					FileUtils.DeleteDirectoryAdvanced(zipPath);
				}
			}
			else
			{
				// restore from .MDF, .LDF
				//RestoreFromDataFiles(database, files);
			}
		}

		private void RestoreFromBackup(SqlDatabase database, string[] files)
		{
			// close current database connections
			CloseDatabaseConnections(database.Name);

			if (files.Length == 0)
				throw new ApplicationException("No backup files were uploaded"); // error: no backup files were uploaded

			if (files.Length > 1)
				throw new ApplicationException("Too many files were uploaded"); // error: too many files were uploaded

			string bakFile = files[0];

			try
			{
				// restore database
				// get file list from backup file
				string[][] backupFiles = GetBackupFiles(bakFile);

				if (backupFiles.Length < 1)
					throw new ApplicationException("Backup set should contain at least 1 logical file");

				// map backup files to existing ones
				string[] movings = new string[backupFiles.Length];
				for (int i = 0; i < backupFiles.Length; i++)
				{
					string name = backupFiles[i][0];
					string path = backupFiles[i][1];
					path = Path.GetExtension(path).ToLower() == ".mdf" ? database.DataPath : database.LogPath;




					movings[i] = String.Format("MOVE '{0}' TO '{1}'", EscapeSql(name), EscapeSql(path));
				}

				// restore database
				Log.WriteInfo("RESTORE DATABASE {0} FROM DISK = {1} WITH REPLACE, {2}",
					 QuoteSqlIdentifier(database.Name), QuoteSqlStringLiteral(bakFile), String.Join(", ", movings));
				ExecuteNonQuery(String.Format(@"RESTORE DATABASE {0} FROM DISK = {1} WITH REPLACE, {2}",
					 QuoteSqlIdentifier(database.Name), QuoteSqlStringLiteral(bakFile), String.Join(", ", movings)));


				// restore original database users
				UpdateDatabaseUsers(database.Name, database.Users);
			}
			finally
			{
				// delete uploaded files
				FileUtils.DeleteFiles(files);

			}
		}

		private void RestoreFromDataFiles(SqlDatabase database, string[] files)
		{
			// close current database connections
			CloseDatabaseConnections(database.Name);

			// detach database
			DetachDatabase(database.Name);

			string[] originalFiles = new string[] { database.DataPath, database.LogPath };

			try
			{
				// backup (rename) original database files
				BackupFiles(originalFiles);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				AttachDatabase(database.Name, files);
				throw new Exception("Can't restore database", ex);
			}

			try
			{
				// replace original database files with uploaded ones
				for (int i = 0; i < files.Length; i++)
				{
					if (Path.GetExtension(files[i]).ToLower() == ".mdf")
						FileUtils.MoveFile(files[i], database.DataPath);
					else
						FileUtils.MoveFile(files[i], database.LogPath);
				}

				// attach database
				if (files.Length == 1)
				{
					AttachSingleFileDatabase(database.Name, originalFiles[0]);
				}
				else
				{
					AttachDatabase(database.Name, originalFiles);
				}
			}
			catch (SqlException ex)
			{
				if (ex.Number != 5105)
					throw new ApplicationException("Can't attach database!", ex);

			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				// restore original database files
				RollbackFiles(originalFiles);

				// attach old database
				AttachDatabase(database.Name, files);
				throw new Exception("Can't rollback original database files", ex);
			}
			finally
			{
				// restore original database users
				UpdateDatabaseUsers(database.Name, database.Users);

				// remove old backed up files
				for (int i = 0; i < originalFiles.Length; i++)
					FileUtils.DeleteFile(originalFiles[i] + "_bak");
			}
		}

		private void BackupFiles(string[] files)
		{
			// just rename old database files
			foreach (string file in files)
				FileUtils.MoveFile(file, file + "_bak");
		}

		private void RollbackFiles(string[] files)
		{
			// just rename old files back
			foreach (string file in files)
				FileUtils.MoveFile(file + "_bak", file);
		}

		private string[][] GetBackupFiles(string file)
		{
			DataView dvFiles = ExecuteQuery(
				 String.Format("RESTORE FILELISTONLY FROM DISK = {0}", QuoteSqlStringLiteral(file))).Tables[0].DefaultView;

			string[][] files = new string[dvFiles.Count][];
			for (int i = 0; i < dvFiles.Count; i++)
			{
				files[i] = new string[]{
											(string)dvFiles[i]["LogicalName"],
											(string)dvFiles[i]["PhysicalName"]
										};
			}
			return files;
		}

		#endregion

		#region private helper methods

		protected int ExecuteNonQuery(string commandText)
		{
			return ExecuteNonQuery(commandText, null);
		}

		protected int ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
		{
			SqlConnection conn = null;
			try
			{
				conn = new SqlConnection(ConnectionString);
				using (SqlCommand cmd = new SqlCommand(commandText, conn))
				{
					cmd.CommandTimeout = 300;
					if (parameters != null && parameters.Length > 0)
						cmd.Parameters.AddRange(parameters);
					conn.Open();
					int ret = cmd.ExecuteNonQuery();
					conn.Close();
					return ret;
				}
			}
			finally
			{
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		protected DataSet ExecuteQuery(string commandText)
		{
			return ExecuteQuery(commandText, ConnectionString, null);
		}

		protected DataSet ExecuteQuery(string commandText, params SqlParameter[] parameters)
		{
			return ExecuteQuery(commandText, ConnectionString, parameters);
		}

		private DataSet ExecuteQuery(string commandText, string connectionString)
		{
			return ExecuteQuery(commandText, connectionString, null);
		}

		private DataSet ExecuteQuery(string commandText, string connectionString, params SqlParameter[] parameters)
		{
			SqlConnection conn = null;
			try
			{
				conn = new SqlConnection(NormalizeConnectionString(connectionString));
				using (SqlCommand cmd = new SqlCommand(commandText, conn))
				using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
				{
					cmd.CommandTimeout = 600; // 10 minutes
					if (parameters != null && parameters.Length > 0)
						cmd.Parameters.AddRange(parameters);
					DataSet ds = new DataSet();
					adapter.Fill(ds);
					return ds;
				}
			}
			finally
			{
				// close connection if required
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
			}
		}

		private void UpdateDatabaseUsers(string databaseName, string[] users)
		{
			// current users
			string[] arrCurrentUsers = GetDatabaseUsers(databaseName);
			StringDictionary currentUsers = new StringDictionary();
			foreach (string user in arrCurrentUsers)
				currentUsers.Add(user, user);

			// new users
			StringDictionary newUsers = new StringDictionary();
			foreach (string user in users)
				newUsers.Add(user, user);

			// users to add
			List<string> addedUsers = new List<string>();
			foreach (string user in users)
			{
				if (currentUsers[user] == null)
					addedUsers.Add(user);
			}

			// users to remove
			List<string> removedUsers = new List<string>();
			foreach (string user in arrCurrentUsers)
			{
				if (newUsers[user] == null)
					removedUsers.Add(user);
			}

			// grant/revoke DB access
			AddUsersToDatabase(databaseName, addedUsers);
			RemoveUsersFromDatabase(databaseName, removedUsers);
		}

		private void UpdateUserDatabases(string username, string[] databases, string[] allDatabases)
		{
			// current databases
			string[] arrCurrentDatabases = GetUserDatabases(username, allDatabases);
			StringDictionary currentDatabases = new StringDictionary();
			foreach (string database in arrCurrentDatabases)
				currentDatabases.Add(database, database);

			// new databases
			StringDictionary newDatabases = new StringDictionary();
			foreach (string database in databases)
				newDatabases.Add(database, database);

			// databases to add
			StringCollection addedDatabases = new StringCollection();
			foreach (string database in databases)
			{
				if (currentDatabases[database] == null)
					addedDatabases.Add(database);
			}

			// databases to remove
			StringCollection removedDatabases = new StringCollection();
			foreach (string database in arrCurrentDatabases)
			{
				if (newDatabases[database] == null)
					removedDatabases.Add(database);
			}

			// grant/revoke DB access
			foreach (string database in addedDatabases)
				AddUserToDatabase(database, username);
			foreach (string database in removedDatabases)
				RemoveUserFromDatabase(database, username);
		}

		private string[] GetDatabaseUsers(string databaseName)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));

			string cmdText = String.Format(@"
				select su.name FROM {0}..sysusers as su
				inner JOIN master..syslogins as sl on su.sid = sl.sid
				where su.hasdbaccess = 1 AND su.islogin = 1 AND su.issqluser = 1 AND su.name <> 'dbo'",
					  QuoteSqlIdentifier(databaseName));
			DataView dvUsers = ExecuteQuery(cmdText).Tables[0].DefaultView;

			string[] users = new string[dvUsers.Count];
			for (int i = 0; i < dvUsers.Count; i++)
			{
				users[i] = (string)dvUsers[i]["Name"];
			}
			return users;
		}

		private string[] GetUserDatabases(string username, string[] allDatabases)
		{
			ValidateSqlLoginInput(username, nameof(username));
			ValidateSqlIdentifierInputs(allDatabases, nameof(allDatabases));

			string filter = "";
			if (allDatabases != null)
			{
				if (allDatabases.Length == 0)
					return new string[] { };

				filter = String.Format(" AND name IN ({0})", BuildSqlStringLiteralList(allDatabases));
			}

			string cmdText = String.Format(@"
					DECLARE @Username nvarchar(100)
					SET @Username = @LookupUsername

					CREATE TABLE #UserDatabases
					(
						Name nvarchar(100) collate database_default
					)

					DECLARE @DbName nvarchar(100)
					DECLARE DatabasesCursor CURSOR FOR
					SELECT name FROM master..sysdatabases
					WHERE (status & 256) = 0 AND (status & 512) = 0 {0}

					OPEN DatabasesCursor

					WHILE (10 = 10)
					BEGIN    --LOOP 10: thru Databases
						FETCH NEXT FROM DatabasesCursor
						INTO @DbName

					--print @DbName

						IF (@@fetch_status <> 0)
						BEGIN
							DEALLOCATE DatabasesCursor
							BREAK
						END

						DECLARE @sql nvarchar(1000)
	                SET @sql = N'if exists (select 1 from ' + QUOTENAME(@DbName) + N'..sysusers where name = @DynamicUserName) insert into #UserDatabases (Name) values (@DynamicDbName)'

						EXEC sp_executesql @sql, N'@DynamicUserName nvarchar(100), @DynamicDbName nvarchar(100)', @DynamicUserName = @Username, @DynamicDbName = @DbName

					END

					SELECT Name FROM #UserDatabases

					DROP TABLE #UserDatabases
					", filter);
			DataView dvDatabases = ExecuteQuery(cmdText,
				CreateNVarCharParameter("@LookupUsername", username)).Tables[0].DefaultView;

			string[] databases = new string[dvDatabases.Count];
			for (int i = 0; i < dvDatabases.Count; i++)
				databases[i] = (string)dvDatabases[i]["Name"];
			return databases;
		}

		private void AddUsersToDatabase(string databaseName, List<string> users)
		{
			foreach (string user in users)
				AddUserToDatabase(databaseName, user);
		}

		private void AddUserToDatabase(string databaseName, string user)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));
			ValidateSqlLoginInput(user, nameof(user));

			// grant database access
			try
			{
				ExecuteNonQueryInDatabase(databaseName, "EXEC sp_grantdbaccess @UserName;",
					CreateNVarCharParameter("@UserName", user));
			}
			catch (SqlException ex)
			{
				if (ex.Number == 15023)
				{
					// the user already exists in the database
					// so, try to auto fix his login in the database
					ExecuteNonQueryInDatabase(databaseName, "EXEC sp_change_users_login 'Auto_Fix', @UserName;",
						CreateNVarCharParameter("@UserName", user));
				}
				else
				{
					throw new Exception("Can't add user to database", ex);
				}
			}

			// add database owner
			ExecuteNonQueryInDatabase(databaseName, "EXEC sp_addrolemember 'db_owner', @UserName;",
				CreateNVarCharParameter("@UserName", user));
		}

		private void RemoveUsersFromDatabase(string databaseName, List<string> users)
		{
			foreach (string user in users)
				RemoveUserFromDatabase(databaseName, user);
		}

		private void RemoveUserFromDatabase(string databaseName, string user)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));
			ValidateSqlLoginInput(user, nameof(user));

			// change ownership of user's objects
			string[] userObjects = GetUserDatabaseObjects(databaseName, user);
			foreach (string userObject in userObjects)
			{
				try
				{
					ExecuteNonQueryInDatabase(databaseName, "EXEC sp_changeobjectowner @ObjectName, 'dbo'",
						CreateNVarCharParameter("@ObjectName", user + "." + userObject));
				}
				catch (SqlException ex)
				{
					if (ex.Number == 15505)
					{
						// Cannot change owner of object 'user.ObjectName' or one of its child objects because
						// the new owner 'dbo' already has an object with the same name.

						// try to rename object before changing owner
						string renamedObject = user + DateTime.Now.Ticks + "_" + userObject;
						ExecuteNonQueryInDatabase(databaseName, "EXEC sp_rename @OldObjectName, @NewObjectName",
							CreateNVarCharParameter("@OldObjectName", user + "." + userObject),
							CreateNVarCharParameter("@NewObjectName", renamedObject));

						// change owner
						ExecuteNonQueryInDatabase(databaseName, "EXEC sp_changeobjectowner @ObjectName, 'dbo'",
							CreateNVarCharParameter("@ObjectName", user + "." + renamedObject));
					}
					else
					{
						throw new Exception("Can't change database object owner", ex);
					}
				}
			}

			// revoke db access
			ExecuteNonQueryInDatabase(databaseName, "EXEC sp_revokedbaccess @UserName;",
				CreateNVarCharParameter("@UserName", user));
		}

		private string[] GetUserDatabaseObjects(string databaseName, string user)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));
			ValidateSqlLoginInput(user, nameof(user));

			DataView dvObjects = ExecuteQuery(String.Format("select so.name from {0}..sysobjects as so" +
				 " inner join {0}..sysusers as su on so.uid = su.uid" +
				 " where su.name = @UserName", QuoteSqlIdentifier(databaseName)),
				CreateNVarCharParameter("@UserName", user)).Tables[0].DefaultView;
			string[] objects = new string[dvObjects.Count];
			for (int i = 0; i < dvObjects.Count; i++)
			{
				objects[i] = (string)dvObjects[i]["Name"];
			}
			return objects;
		}

		private void CloseDatabaseConnections(string databaseName)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));

			DataView dv = ExecuteQuery(
				"SELECT spid FROM master..sysprocesses WHERE dbid = DB_ID(@DatabaseName)",
				CreateNVarCharParameter("@DatabaseName", databaseName)).Tables[0].DefaultView;

			// kill processes
			for (int i = 0; i < dv.Count; i++)
				KillProcess((short)(dv[i]["spid"]));
		}

		private void CloseUserConnections(string userName)
		{
			ValidateSqlLoginInput(userName, nameof(userName));

			DataView dv = ExecuteQuery(
				"SELECT spid FROM master..sysprocesses WHERE loginame = @UserName",
				CreateNVarCharParameter("@UserName", userName)).Tables[0].DefaultView;

			// kill processes
			for (int i = 0; i < dv.Count; i++)
				KillProcess((short)(dv[i]["spid"]));
		}

		private void KillProcess(short spid)
		{
			ExecuteNonQuery(String.Format("KILL {0}", spid));
		}

		private SqlParameter CreateNVarCharParameter(string name, string value)
		{
			return new SqlParameter(name, SqlDbType.NVarChar)
			{
				Value = (object)value ?? DBNull.Value
			};
		}

		private int ExecuteNonQueryInDatabase(string databaseName, string commandText, params SqlParameter[] parameters)
		{
			ValidateSqlIdentifierInput(databaseName, nameof(databaseName));
			return ExecuteNonQuery(String.Format("USE {0};{1}", QuoteSqlIdentifier(databaseName), commandText), parameters);
		}

		private string BuildSqlStringLiteralList(IEnumerable<string> values)
		{
			List<string> literals = new List<string>();
			foreach (string value in values)
				literals.Add(QuoteSqlStringLiteral(value));

			return String.Join(", ", literals);
		}

		private string QuoteSqlIdentifier(string identifier)
		{
			if (String.IsNullOrWhiteSpace(identifier))
				throw new ArgumentException("SQL identifier cannot be null or empty.", nameof(identifier));

			return "[" + identifier.Replace("]", "]]" ) + "]";
		}

		private string QuoteSqlStringLiteral(string value)
		{
			return "'" + EscapeSql(value) + "'";
		}

		private string QuoteSqlUnicodeLiteral(string value)
		{
			return "N" + QuoteSqlStringLiteral(value);
		}

		private string GetValidatedCollationClause()
		{
			if (String.IsNullOrWhiteSpace(DatabaseCollation))
				return "";

			ValidateSqlIdentifierInput(DatabaseCollation, nameof(DatabaseCollation));
			return " COLLATE " + DatabaseCollation;
		}

		private string NormalizeConnectionString(string connectionString)
		{
			SqlConnectionStringBuilder parsed = new SqlConnectionStringBuilder(connectionString);
			if (!String.Equals(parsed.DataSource, ServerName, StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException("Connection string server is not allowed.", nameof(connectionString));

			if (String.IsNullOrWhiteSpace(parsed.InitialCatalog))
				parsed.InitialCatalog = "master";

			ValidateSqlIdentifierInput(parsed.InitialCatalog, nameof(connectionString));
			parsed.TrustServerCertificate = TrustServerCertificate;

			return parsed.ConnectionString;
		}

		private void ValidateSqlIdentifierInput(string value, string paramName)
		{
			if (String.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[A-Za-z0-9_\-.$#]+$"))
				throw new ArgumentException("SQL identifier contains invalid characters.", paramName);
		}

		private void ValidateSqlLoginInput(string value, string paramName)
		{
			if (String.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[A-Za-z0-9_\-.$#@\\]+$"))
				throw new ArgumentException("SQL login contains invalid characters.", paramName);
		}

		private void ValidateSqlIdentifierInputs(IEnumerable<string> values, string paramName)
		{
			if (values == null)
				return;

			foreach (string value in values)
				ValidateSqlIdentifierInput(value, paramName);
		}

		private void ValidateSqlLoginInputs(IEnumerable<string> values, string paramName)
		{
			if (values == null)
				return;

			foreach (string value in values)
				ValidateSqlLoginInput(value, paramName);
		}

		private string EscapeSql(string s)
		{
			return (s != null) ? s.Replace("'", "''") : null;
		}

		#endregion

		#region IHostingServiceProvier methods
		public override string[] Install()
		{
			List<string> messages = new List<string>();

			// check connectivity
			SqlConnection conn = new SqlConnection(ConnectionString);
			try
			{
				conn.Open();
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				messages.Add("Could not connect to the specified SQL Server: " + ex.Message);
			}
			finally
			{
				if (conn.State == ConnectionState.Open)
					conn.Close();
			}
			return messages.ToArray();
		}

		public override void ChangeServiceItemsState(ServiceProviderItem[] items, bool enabled)
		{
			foreach (ServiceProviderItem item in items)
			{
				try
				{
					if (item is SqlUser)
					{
						// enable/disable user access to all his databases
						string[] databases = GetUserDatabases(item.Name, null);
						foreach (string database in databases)
						{
							if (enabled)
							{
								// enable access
								ExecuteNonQueryInDatabase(database, "EXEC sp_addrolemember 'db_owner', @UserName;",
									CreateNVarCharParameter("@UserName", item.Name));
								ExecuteNonQueryInDatabase(database, "EXEC sp_droprolemember 'db_denydatareader', @UserName;",
									CreateNVarCharParameter("@UserName", item.Name));
								ExecuteNonQueryInDatabase(database, "EXEC sp_droprolemember 'db_denydatawriter', @UserName;",
									CreateNVarCharParameter("@UserName", item.Name));
							}
							else
							{
								// disable access
								ExecuteNonQueryInDatabase(database, "EXEC sp_droprolemember 'db_owner', @UserName;",
									CreateNVarCharParameter("@UserName", item.Name));
								ExecuteNonQueryInDatabase(database, "EXEC sp_addrolemember 'db_denydatareader', @UserName;",
									CreateNVarCharParameter("@UserName", item.Name));
								ExecuteNonQueryInDatabase(database, "EXEC sp_addrolemember 'db_denydatawriter', @UserName;",
									CreateNVarCharParameter("@UserName", item.Name));
							}
						}
					}
				}
				catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
				{
					Log.WriteError(String.Format("Error switching '{0}' MS SQL database", item.Name), ex);
				}
			}
		}

		public override void DeleteServiceItems(ServiceProviderItem[] items)
		{
			foreach (ServiceProviderItem item in items)
			{
				if (item is SqlDatabase)
				{
					try
					{
						// delete database
						DeleteDatabase(item.Name);
					}
					catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
					{
						Log.WriteError(String.Format("Error deleting '{0}' MS SQL Database", item.Name), ex);
					}
				}
				else if (item is SqlUser)
				{
					try
					{
						// delete user
						DeleteUser(item.Name, null);
					}
					catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
					{
						Log.WriteError(String.Format("Error deleting '{0}' MS SQL User", item.Name), ex);
					}
				}
			}
		}

		public override ServiceProviderItemDiskSpace[] GetServiceItemsDiskSpace(ServiceProviderItem[] items)
		{
			List<ServiceProviderItemDiskSpace> itemsDiskspace = new List<ServiceProviderItemDiskSpace>();

			// update items with diskspace
			foreach (ServiceProviderItem item in items)
			{
				if (item is SqlDatabase)
				{
					try
					{
						// get database details

						Log.WriteStart(String.Format("Calculating '{0}' database size", item.Name));

						SqlDatabase db = GetDatabase(item.Name);

						// calculate disk space
						ServiceProviderItemDiskSpace diskspace = new ServiceProviderItemDiskSpace();
						diskspace.ItemId = item.Id;
						diskspace.DiskSpace = (((long)db.DataSize) * 1024) + (((long)db.LogSize) * 1024);
						itemsDiskspace.Add(diskspace);

						Log.WriteEnd(String.Format("Calculating '{0}' database size", item.Name));
					}
					catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
					{
						Log.WriteError(String.Format("Error calculating '{0}' SQL Server database size", item.Name), ex);
					}
				}
			}

			return itemsDiskspace.ToArray();
		}

		[SupportedOSPlatform("windows")]
		protected bool IsStringRegistryValueStartWith(string path, string paramName, string paramValue)
		{
			string value = string.Empty;
			RegistryKey root = Registry.LocalMachine;
			RegistryKey rk = root.OpenSubKey(path);
			if (rk != null)
			{
				value = (string)rk.GetValue(paramName, null);
			}

			bool res = !string.IsNullOrEmpty(value) && value.StartsWith(paramValue);

			return res;
		}

		[SupportedOSPlatform("windows")]
		protected virtual bool CheckWindowsVersion(string version)
		{
			bool res = IsStringRegistryValueStartWith("SOFTWARE\\Microsoft\\MSSQLServer\\MSSQLServer\\CurrentVersion", "CurrentVersion", version);

			if (!res)
				res = IsStringRegistryValueStartWith("SOFTWARE\\Wow6432Node\\Microsoft\\MSSQLServer\\MSSQLServer\\CurrentVersion", "CurrentVersion", version);

			//Check instances
			if (!res)
			{
				RegistryKey root = Registry.LocalMachine;
				RegistryKey rk = root.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SQL Server");
				if (rk == null)
					rk = root.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Microsoft SQL Server");

				if (rk != null)
				{
					string[] instances = null;
					object value = rk.GetValue("InstalledInstances");
					if (value != null && value is string[])
					{
						instances = (string[])rk.GetValue("InstalledInstances");
					}

					if (instances != null)
					{
						foreach (string instance in instances)
						{
							string registryPath =
								 string.Format(
									  "SOFTWARE\\Microsoft\\Microsoft SQL Server\\{0}\\MSSQLServer\\CurrentVersion",
									  instance);

							res = IsStringRegistryValueStartWith(registryPath, "CurrentVersion", version);

							if (!res)
							{
								registryPath =
									 string.Format(
										  "SOFTWARE\\Wow6432Node\\Microsoft\\Microsoft SQL Server\\{0}\\MSSQLServer\\CurrentVersion",
										  instance);
								res = IsStringRegistryValueStartWith(registryPath, "CurrentVersion", version);

							}

							if (res)
								break;
						}
					}
				}
			}
			return res;

		}

		protected virtual bool TryCheckServerVersionByQuery(string version)
		{
			try
			{
				DataTable dt = ExecuteQuery("SELECT CAST(SERVERPROPERTY('ProductVersion') AS nvarchar(128))").Tables[0];
				if (dt.Rows.Count == 0)
					return false;

				string productVersion = dt.Rows[0][0]?.ToString();
				return !String.IsNullOrEmpty(productVersion) &&
					productVersion.StartsWith(version, StringComparison.Ordinal);
			}
			catch
			{
				return false;
			}
		}

		protected virtual bool CheckUnixVersion(string version) => false;
		protected virtual bool CheckVersion(string version)
		{
			if (TryCheckServerVersionByQuery(version))
				return true;

			if (OperatingSystem.IsWindows())
				return CheckWindowsVersion(version);

			return CheckUnixVersion(version);
		}

		public override bool IsInstalled()
		{
			return CheckVersion("8.");
		}

		public long CalculateDatabaseSize(string database)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}


