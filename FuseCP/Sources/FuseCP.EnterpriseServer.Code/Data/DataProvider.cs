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

/// <summary>TODO</summary>
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.ConstrainedExecution;

#if !EF64
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

#if !NETFRAMEWORK && !NETSTANDARD
using Microsoft.EntityFrameworkCore;
#elif NETFRAMEWORK
using System.Data.Entity;
#endif
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.Win32;
using FuseCP.Providers.OS;
using FuseCP.Providers.RemoteDesktopServices;
using FuseCP.Providers.DNS;
using FuseCP.Providers.DomainLookup;
using FuseCP.Providers.StorageSpaces;
using FuseCP.EnterpriseServer.Data;
using FuseCP.EnterpriseServer.Code;
using FuseCP.EnterpriseServer.Base.HostedSolution;
using FuseCP.Providers.HostedSolution;
using MySqlX.XDevAPI.Common;
//using Humanizer.Localisation;

namespace FuseCP.EnterpriseServer
{
	/// <summary>
	/// Summary description for DataProvider.
	/// The source code of the Stored Procedures can be found in the FuseCP.EnterpriseServer.Data project in the
	/// Migrations\SqlServer\StoredProcedures folder.
	/// </summary>
	/// <summary>TODO</summary>
	public class DataProvider : Data.DbContext
	{
		/// <summary>Auto-generated member.</summary>
		public const long MB = 1024 * 1024;

#if UseEntityFramework
		/// <summary>Auto-generated member.</summary>
		public bool? useEntityFramework = null;
		/// <summary>Auto-generated member.</summary>
		public static bool? alwaysUseEntityFramework = null;
		int queryAlwaysUseEFStarted = 0;
		public bool AlwaysUseEntityFramework
		{
			get
			{
				if (alwaysUseEntityFramework == null)
				{
					if (IsSqlServer)
					{
						if (Interlocked.Exchange(ref queryAlwaysUseEFStarted, 1) == 0)
						{
							Task.Run(async () =>
							{
								using (var context = Context)
								{
									try
									{
										alwaysUseEntityFramework = (await context.SystemSettings
											.Where(s => s.SettingsName == EnterpriseServer.SystemSettings.DEBUG_SETTINGS &&
												s.PropertyName == EnterpriseServer.SystemSettings.ALWAYS_USE_ENTITYFRAMEWORK)
											.Select(p => p.PropertyValue)
											.FirstOrDefaultAsync()
											.ConfigureAwait(false))?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false; 
									}
									catch (DbException swallowedEx)
									{
									    System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
									}
									catch (InvalidOperationException swallowedEx)
									{
									    System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
									}
								}
							});
						}
					}
					else alwaysUseEntityFramework = true;
				}
				/// <summary>TODO</summary>
				return alwaysUseEntityFramework ?? false;
			}
			set
			{
				alwaysUseEntityFramework = value;
				useEntityFramework = null;
			}
		}
		public bool UseEntityFramework
		{
			get
			{
				return !IsSqlServer || !HasProcedures ||
					(useEntityFramework ??= AlwaysUseEntityFramework);
			}
			set { useEntityFramework = value; }
		}
#else
		/// <summary>Auto-generated member.</summary>
		public const bool UseEntityFramework = false;
#endif

		/// <summary>TODO</summary>
		readonly ControllerBase Provider;
		/// <summary>TODO</summary>
		ServerController serverController;
		/// <summary>Auto-generated member.</summary>
		protected ServerController ServerController => serverController ??= new ServerController(Provider);
		/// <summary>Auto-generated member.</summary>

		public DataProvider() : base() { Provider = null; }
		/// <summary>Auto-generated member.</summary>
		public DataProvider(ControllerBase provider) : base() { Provider = provider; }
		/// <summary>Auto-generated member.</summary>
		public DataProvider(string connectionString) : base(connectionString) { Provider = null; }
		/// <summary>Auto-generated member.</summary>

		private DataProvider clone = null;
		/// <summary>Auto-generated member.</summary>
		public new DataProvider Clone => clone ??= Context;
		/// <summary>Auto-generated member.</summary>
		public new DataProvider Context => new DataProvider(ConnectionString);
		/// <summary>Auto-generated member.</summary>
		public override void Dispose()
		{
			clone?.Dispose();
			base.Dispose();
		}

		//public string ConnectionString => ConfigSettings.ConnectionString;
		/// <summary>Auto-generated member.</summary>
		private string ObjectQualifier => "";

		#region System Settings
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetSystemSettings(string settingsName)
		{
			if (UseEntityFramework)
			{
				var settings = SystemSettings
					.Where(s => s.SettingsName == settingsName)
					.Select(s => new
					{
						s.PropertyName,
						s.PropertyValue
					});
				/// <summary>TODO</summary>
				return EntityDataReader(settings);
			}
			return SqlHelper.ExecuteReader(
				 NativeConnectionString,
				 CommandType.StoredProcedure,
				 "GetSystemSettings",
				 /// <summary>TODO</summary>
				 new SqlParameter("@SettingsName", settingsName)
			);
		}
		/// <summary>Auto-generated member.</summary>

		public void SetSystemSettings(string settingsName, string xml)
		{
			if (UseEntityFramework)
			{
				using (var transaction = Database.BeginTransaction())
				{
					SystemSettings
						.Where(s => s.SettingsName == settingsName)
						.ExecuteDelete();

					var settings = XElement.Parse(xml)
						.Elements()
						.Select(e => new Data.Entities.SystemSetting
						{
							SettingsName = settingsName,
							PropertyName = (string)e.Attribute("name"),
							PropertyValue = (string)e.Attribute("value")
						});
					SystemSettings.AddRange(settings);
					SaveChanges();

					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					 NativeConnectionString,
					 CommandType.StoredProcedure,
					 "SetSystemSettings",
					 /// <summary>TODO</summary>
					 new SqlParameter("@SettingsName", settingsName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Xml", xml)
				);
			}
		}

		#endregion

		#region Theme Settings
		/// <summary>Auto-generated member.</summary>

		public DataSet GetThemes()
		{
			if (UseEntityFramework)
			{
				var themes = Themes
					.Where(t => t.Enabled == 1)
					.OrderBy(t => t.DisplayOrder)
					.Select(t => new
					{
						t.ThemeId,
						t.DisplayName,
						t.LTRName,
						t.RTLName,
						t.DisplayOrder
					});
				/// <summary>TODO</summary>
				return EntityDataSet(themes);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetThemes");
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetThemeSettings(int ThemeID)
		{
			if (UseEntityFramework)
			{
				var settings = ThemeSettings
					.Where(ts => ts.ThemeId == ThemeID)
					.Select(ts => new
					{
						ts.ThemeId,
						ts.SettingsName,
						ts.PropertyName,
						ts.PropertyValue
					});
				/// <summary>TODO</summary>
				return EntityDataSet(settings);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetThemeSettings",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ThemeID", ThemeID));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetThemeSetting(int ThemeID, string SettingsName)
		{
			if (UseEntityFramework)
			{
				var setting = ThemeSettings
					.Where(ts => ts.ThemeId == ThemeID && ts.SettingsName == SettingsName)
					.Select(ts => new
					{
						ts.ThemeId,
						ts.SettingsName,
						ts.PropertyName,
						ts.PropertyValue
					});
				/// <summary>TODO</summary>
				return EntityDataSet(setting);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetThemeSetting",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ThemeID", ThemeID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SettingsName", SettingsName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckActorUserRights(int actorId, int? userId)
		{
			if (actorId == -1 || userId == null || userId == 0 ||
				// check if the user requests himself
				actorId == userId)
				/// <summary>TODO</summary>
				return true;

			// check if the user requests his owner
			var actor = Users
				.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
				.FirstOrDefault(u => u.UserId == actorId);

			if (actor != null && actor.IsPeer) actorId = actor.OwnerId ?? -1;

			if (actorId == userId) return true;

			var id = userId;
			do // check owners chain
			{
				var user = Users
					.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
					.FirstOrDefault(u => u.UserId == id);
				if (user == null || user.OwnerId == null) return false;
				else
				{
					id = user.OwnerId.Value;
					if (id == actorId) return true;
				}
			} while (true);
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetUserThemeSettings(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				const string SettingsName = "Theme";
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var id = userId;
				var settings = UserSettings
					.Where(s => s.UserId == id && s.SettingsName == SettingsName)
					.Select(ts => new
					{
						ts.UserId,
						ts.PropertyName,
						ts.PropertyValue
					});
				while (!settings.Any())
				{
					var user = Users
						.Select(u => new { u.UserId, u.OwnerId })
						.FirstOrDefault(u => u.UserId == id);
					if (user != null && user.OwnerId != null)
					{
						id = user.OwnerId.Value;
						settings = UserSettings
							.Where(s => s.UserId == id && s.SettingsName == SettingsName)
							.Select(ts => new
							{
								ts.UserId,
								ts.PropertyName,
								ts.PropertyValue
							});
					}
					/// <summary>TODO</summary>
					else break;
				}
				/// <summary>TODO</summary>
				return EntityDataSet(settings);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserSettings",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SettingsName", "Theme"));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateUserThemeSetting(int actorId, int userId, string PropertyName, string PropertyValue)
		{
			if (UseEntityFramework)
			{
				const string SettingsName = "Theme";

				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var setting = UserSettings
					.FirstOrDefault(s => s.UserId == userId &&
						s.SettingsName == SettingsName &&
						s.PropertyName == PropertyName);
				if (setting != null)
				{
					setting.PropertyValue = PropertyValue;
				}
				else
				{
					setting = new Data.Entities.UserSetting()
					{
						UserId = userId,
						SettingsName = SettingsName,
						PropertyName = PropertyName,
						PropertyValue = PropertyValue
					};
					UserSettings.Add(setting);
				}
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdateUserThemeSetting",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PropertyName", PropertyName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PropertyValue", PropertyValue));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteUserThemeSetting(int actorId, int userId, string PropertyName)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				UserSettings
					.Where(s => s.UserId == userId &&
						s.SettingsName == "Theme" && s.PropertyName == PropertyName)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "DeleteUserThemeSetting",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PropertyName", PropertyName));
			}
		}

		#endregion

		#region Users
		/// <summary>Auto-generated member.</summary>
		public bool IsFreshDatabase => Users.Count() == 1;
		/// <summary>Auto-generated member.</summary>
		public bool CheckUserExists(string username)
		{
			if (UseEntityFramework)
			{
				return Users.Any(u => u.Username == username);
			}
			else
			{
				SqlParameter prmExists = new SqlParameter("@Exists", SqlDbType.Bit);
				prmExists.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "CheckUserExists",
					 prmExists,
					 /// <summary>TODO</summary>
					 new SqlParameter("@username", username));

				return Convert.ToBoolean(prmExists.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IEnumerable<int> UserParents(int actorId, int userId)
		{
			var user = Users
				.Select(u => new { u.UserId, u.OwnerId })
				.FirstOrDefault(u => u.UserId == userId);
			while (user != null)
			{
				/// <summary>TODO</summary>
				yield return userId;
				if (user.OwnerId.HasValue)
				{
					userId = user.OwnerId.Value;
					user = Users
						.Select(u => new { u.UserId, u.OwnerId })
						.FirstOrDefault(u => u.UserId == userId);
				}
				/// <summary>TODO</summary>
				else break;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public TempIdSet UserChildren(int ownerId, bool recursive = true)
		{
			var set = new TempIdSet(this);
			set.Add(ownerId);

			var owner = Users
				.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
				.FirstOrDefault(u => u.UserId == ownerId);
			if (owner != null && owner.IsPeer && owner.OwnerId.HasValue)
			{
				ownerId = owner.OwnerId.Value;
				set.Add(ownerId);
			}

			SaveChanges();

			if (recursive)
			{
				int level = 1;
				var children = Users
					.Where(u => u.OwnerId == ownerId)
					.Select(u => u.UserId);
				while (set.AddRange(children, level) > 0)
				{
					children = Users
						.Join(set.OfLevel(level), u => u.OwnerId, child => child, (u, child) => u.UserId);
					level++;
				}
			}
			/// <summary>TODO</summary>
			else SaveChanges();

			/// <summary>TODO</summary>
			return set;
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckUserParent(int ownerId, int? userId)
		{
			if (ownerId == userId) return true;

			var owner = Users
				.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
				.FirstOrDefault(u => u.UserId == ownerId);
			if (owner != null && owner.IsPeer && owner.OwnerId.HasValue) ownerId = owner.OwnerId.Value;

			if (ownerId == userId) return true;

			var id = userId;
			var user = Users
				.Select(u => new { u.UserId, u.OwnerId })
				.FirstOrDefault(u => u.UserId == id);
			while (user != null && user.OwnerId.HasValue && user.OwnerId.Value != ownerId)
			{
				id = user.OwnerId.Value;
				user = Users
					.Select(u => new { u.UserId, u.OwnerId })
					.FirstOrDefault(u => u.UserId == id);
			}

			return user != null && user.OwnerId.HasValue && user.OwnerId.Value == ownerId;
		}
		/// <summary>Auto-generated member.</summary>

		public string GetItemComments(int itemId, string itemTypeId, int actorId)
		{
			var comments = Comments
				.Join(Users, c => c.UserId, u => u.UserId, (com, user) => new
				{
					com.UserId,
					com.ItemId,
					com.ItemTypeId,
					com.CreatedDate,
					com.CommentText,
					user.Username
				})
				//.Where(c => c.ItemId == itemId && c.ItemTypeId == itemTypeId &&
				//	CheckUserParent(actorId, c.UserId));
				.Where(c => c.ItemId == itemId && c.ItemTypeId == itemTypeId)
				.AsEnumerable()
				.Where(c => Clone.CheckUserParent(actorId, c.UserId));

			var sb = new StringBuilder();
			foreach (var comment in comments)
			{
				sb.Append(comment.Username);
				sb.Append(" - ");
				sb.AppendLine(comment.CreatedDate.ToShortDateString());
				sb.AppendLine(comment.CommentText);
				sb.AppendLine("--------------------------------------");
			}
			return sb.ToString();
		}
		/// <summary>Auto-generated member.</summary>

		public string ColumnName(string sortColumn)
		{
			var i = sortColumn.LastIndexOf('.');
			return i >= 0 ? sortColumn.Substring(i + 1) : sortColumn;

		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetUsersPaged(int actorId, int userId, string filterColumn, string filterValue,
			 int statusId, int roleId, string sortColumn, int startRow, int maximumRows, bool recursive)
		{
			if (UseEntityFramework)
			{
				var hasRights = CheckActorUserRights(actorId, userId);
				TempIdSet childUsers = null;
				if (recursive) childUsers = UserChildren(userId, recursive);
				using (childUsers)
				{
					var users = UsersDetailed;
					if (hasRights)
					{
						users = users
							.Where(u => u.UserId != userId && !u.IsPeer &&
								(statusId == 0 || statusId > 0 && statusId == u.StatusId) &&
								(roleId == 0 || roleId > 0 && roleId == u.RoleId));
						users = recursive
							? users.Join(childUsers, u => u.UserId, ch => ch, (u, ch) => u)
							: users.Where(u => u.OwnerId == userId);
					}
					else
					{
						users = users.Where(u => false);
					}

					if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
					{
						users = users.Where(DynamicFunctions.ColumnLike(users, filterColumn, filterValue));
					}
					else if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
							users = users.Where(u => DbFunctions.Like(u.Username, filterValue) ||
								DbFunctions.Like(u.FullName, filterValue) ||
								DbFunctions.Like(u.Email, filterValue));
#else
							users = users.Where(u => EF.Functions.Like(u.Username, filterValue) ||
								EF.Functions.Like(u.FullName, filterValue) ||
								EF.Functions.Like(u.Email, filterValue));
#endif
					}

					if (!string.IsNullOrEmpty(sortColumn))
					{
						users = users.OrderBy(ColumnName(sortColumn));
					}

					var count = users.Count();

					users = users.Skip(startRow).Take(maximumRows);

					var usersSelected = users
						.AsEnumerable()
						.Select(u => new
						{
							u.UserId,
							u.RoleId,
							u.StatusId,
							u.SubscriberNumber,
							u.LoginStatusId,
							u.FailedLogins,
							u.OwnerId,
							u.Created,
							u.Changed,
							u.IsDemo,
							Comments = Clone.GetItemComments(u.UserId, "USER", actorId),
							u.IsPeer,
							u.Username,
							u.FirstName,
							u.LastName,
							u.Email,
							u.FullName,
							u.OwnerUsername,
							u.OwnerFirstName,
							u.OwnerLastName,
							u.OwnerRoleId,
							u.OwnerFullName,
							u.OwnerEmail,
							u.PackagesNumber,
							u.CompanyName,
							u.EcommerceEnabled
						});

					/// <summary>TODO</summary>
					return EntityDataSet(count, usersSelected);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUsersPaged",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@statusId", statusId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@roleId", roleId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@startRow", startRow),
					 /// <summary>TODO</summary>
					 new SqlParameter("@maximumRows", maximumRows),
					 /// <summary>TODO</summary>
					 new SqlParameter("@recursive", recursive));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public class TextSearchItem
		{
			/// <summary>Auto-generated member.</summary>
			public int ItemId;
			/// <summary>Auto-generated member.</summary>
			public string TextSearch;
			/// <summary>Auto-generated member.</summary>
			public string ColumnType;
		}
		/// <summary>Auto-generated member.</summary>

		public class ItemService
		{
			/// <summary>Auto-generated member.</summary>
			public int ItemId;
			/// <summary>Auto-generated member.</summary>
			public int? ItemTypeId;
			/// <summary>Auto-generated member.</summary>
			public int UserId;
			/// <summary>Auto-generated member.</summary>
			public string Username;
			/// <summary>Auto-generated member.</summary>
			public string FullName;
		}
		/// <summary>Auto-generated member.</summary>

		public class ItemsDomain
		{
			/// <summary>Auto-generated member.</summary>
			public int ItemId;
			/// <summary>Auto-generated member.</summary>
			public int UserId;
			/// <summary>Auto-generated member.</summary>
			public string Username;
			/// <summary>Auto-generated member.</summary>
			public string FullName;
		}
		/// <summary>Auto-generated member.</summary>

		public static Expression<Func<TElement, bool>> BuildOrExpression<TElement, TValue>(
			Expression<Func<TElement, TValue>> valueSelector,
			IEnumerable<TValue> values
		)
		{
			if (null == valueSelector)
				/// <summary>TODO</summary>
				throw new ArgumentNullException("valueSelector");

			if (null == values)
				/// <summary>TODO</summary>
				throw new ArgumentNullException("values");

			ParameterExpression p = valueSelector.Parameters.Single();

			if (!values.Any())
				/// <summary>TODO</summary>
				return e => false;

			var equals = values.Select(value =>
				(Expression)Expression.Equal(
					 valueSelector.Body,
					 Expression.Constant(
						 value,
						 typeof(TValue)
					 )
				)
			);
			var body = equals.Aggregate<Expression>(
					 (accumulate, equal) => Expression.Or(accumulate, equal)
			 );

			return Expression.Lambda<Func<TElement, bool>>(body, p);
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetSearchObject(int actorId, int userId, string filterColumn, string filterValue,
			int statusId, int roleId, string sortColumn, int startRow, int maximumRows, string colType, string fullType,
			bool recursive, bool onlyFind)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				if (colType == null) colType = "";

				/*------------------------------------------------Users---------------------------------------------------------------*/

				const string columnUsername = "Username";
				const string columnEmail = "Email";
				const string columnCompanyName = "CompanyName";
				const string columnFullName = "FullName";

				if (string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue)) filterColumn = "TextSearch";

				using (var userChildren = recursive ? UserChildren(userId) : new TempIdSet(this))
				{
					if (!recursive)
					{
						userChildren.AddRange(Users
							.Where(u => u.OwnerId == userId)
							.Select(u => u.UserId));
						SaveChanges();
					}

					var users = Users
						.Where(u => u.UserId != userId && !u.IsPeer &&
							(statusId == 0 || statusId > 0 && u.StatusId == statusId) &&
							(roleId == 0 || roleId > 0 && u.RoleId == roleId))
						.Join(userChildren, id => id.UserId, tid => tid, (u, t) => u);

					var userItems = users.Join(
						Users.Select(u => new TextSearchItem()
						{
							ItemId = u.UserId,
							TextSearch = u.Username,
							ColumnType = columnUsername
						})
						.Union(Users.Select(u => new TextSearchItem()
						{
							ItemId = u.UserId,
							TextSearch = u.Email,
							ColumnType = columnEmail
						}))
						.Union(Users.Select(u => new TextSearchItem()
						{
							ItemId = u.UserId,
							TextSearch = u.CompanyName,
							ColumnType = columnCompanyName
						}))
						.Union(Users.Select(u => new TextSearchItem()
						{
							ItemId = u.UserId,
							TextSearch = u.FirstName + " " + u.LastName,
							ColumnType = columnFullName
						})),
						u => u.UserId, it => it.ItemId, (u, it) => new SearchItem()
						{
							ItemId = u.UserId,
							TextSearch = it.TextSearch,
							ColumnType = it.ColumnType,
							FullType = "AccountHome",
							PackageId = 0,
							AccountId = 0,
							Username = u.Username,
							FullName = u.FirstName + " " + u.LastName,
						})
						.Where(it => !string.IsNullOrEmpty(it.TextSearch));

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						userItems = userItems.Where(it => DbFunctions.Like(it.TextSearch, filterValue));
#else
						userItems = userItems.Where(it => EF.Functions.Like(it.TextSearch, filterValue));
#endif
					}

					userItems = userItems.OrderBy(it => it.TextSearch);

					if (onlyFind) userItems = userItems.Take(maximumRows);

					/*--------------------------------------------Space----------------------------------------------------------*/

					var itemsService = ServiceItems
						.Select(si => new ItemService()
						{
							ItemId = si.ItemId,
							ItemTypeId = si.ItemTypeId,
							UserId = si.Package.UserId,
							Username = si.Package.User.Username,
							FullName = si.Package.User.FirstName + " " + si.Package.User.LastName
						})
						.Join(userChildren, si => si.UserId, ut => ut, (u, ut) => u);

					var itemsDomain = Domains
						.Select(d => new ItemsDomain()
						{
							ItemId = d.DomainId,
							UserId = d.Package.UserId,
							Username = d.Package.User.Username,
							FullName = d.Package.User.FirstName + " " + d.Package.User.LastName
						})
						.Join(userChildren, di => di.UserId, ut => ut, (di, ut) => di);

					var spaceItemsServices = itemsService
						.Join(ServiceItems
							.Where(si => si.ItemType.Searchable == true && si.ItemTypeId != 200 && si.ItemTypeId != 201),
							it => it.ItemId, si => si.ItemId, (it, si) => new SearchItem()
							{
								ItemId = si.ItemId,
								TextSearch = si.ItemName,
								ColumnType = si.ItemType.DisplayName,
								FullType = si.ItemType.DisplayName,
								PackageId = si.PackageId,
								AccountId = 0,
								Username = it.Username,
								FullName = it.FullName
							});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						spaceItemsServices = spaceItemsServices.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						spaceItemsServices = spaceItemsServices.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind) spaceItemsServices = spaceItemsServices.Take(maximumRows);

					var spaceItemsDomains = itemsDomain
						.Join(Domains
							.Where(d => !d.IsDomainPointer),
							si => si.ItemId, d => d.DomainId, (si, d) => new SearchItem()
							{
								ItemId = si.ItemId,
								TextSearch = d.DomainName,
								ColumnType = "Domain",
								FullType = "Domains",
								PackageId = d.PackageId,
								AccountId = 0,
								Username = si.Username,
								FullName = si.FullName
							});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						spaceItemsDomains = spaceItemsDomains.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						spaceItemsDomains = spaceItemsDomains.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind) spaceItemsDomains = spaceItemsDomains.Take(maximumRows);

					var spaceItemsEac = itemsService
						.Join(ServiceItems, it => it.ItemId, si => si.ItemId, (it, si) => new
						{
							IT = it,
							SI = si
						})
						.Join(ExchangeAccounts, si => si.SI.ItemId, ea => ea.ItemId, (si, ea) => new SearchItem()
						{
							ItemId = si.SI.ItemId,
							TextSearch = ea.DisplayName,
							ColumnType = "ExchangeAccount",
							FullType = ea.AccountType == ExchangeAccountType.Mailbox ? "Mailbox" :
								(ea.AccountType == ExchangeAccountType.Contact ? "Contact" :
								(ea.AccountType == ExchangeAccountType.DistributionList ? "DistributionList" :
								(ea.AccountType == ExchangeAccountType.PublicFolder ? "PublicFolder" :
								(ea.AccountType == ExchangeAccountType.Room ? "Room" :
								(ea.AccountType == ExchangeAccountType.Equipment ? "Equipment" :
								(ea.AccountType == ExchangeAccountType.User ? "User" :
								(ea.AccountType == ExchangeAccountType.SecurityGroup ? "SecurityGroup" :
								(ea.AccountType == ExchangeAccountType.DefaultSecurityGroup ? "DefaultSecurityGroup" :
								(ea.AccountType == ExchangeAccountType.SharedMailbox ? "SharedMailbox" :
								(ea.AccountType == ExchangeAccountType.DeletedUser ? "DeletedUser" :
								(ea.AccountType == ExchangeAccountType.JournalingMailbox ? "JournalingMailbox" :
								((int)ea.AccountType).ToString()))))))))))),
							PackageId = si.SI.PackageId,
							AccountId = ea.AccountId,
							Username = si.IT.Username,
							FullName = si.IT.FullName
						});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						spaceItemsEac = spaceItemsEac.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						spaceItemsEac = spaceItemsEac.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind) spaceItemsEac = spaceItemsEac.Take(maximumRows);

					var spaceItemsEacPem = itemsService
						.Join(ServiceItems, it => it.ItemId, si => si.ItemId, (it, si) => new
						{
							IT = it,
							SI = si
						})
						.Join(ExchangeAccounts, si => si.SI.ItemId, ea => ea.ItemId, (si, ea) => new SearchItem()
						{
							ItemId = si.SI.ItemId,
							TextSearch = ea.PrimaryEmailAddress,
							ColumnType = "ExchangeAccount",
							FullType = ea.AccountType == ExchangeAccountType.Mailbox ? "Mailbox" :
								(ea.AccountType == ExchangeAccountType.Contact ? "Contact" :
								(ea.AccountType == ExchangeAccountType.DistributionList ? "DistributionList" :
								(ea.AccountType == ExchangeAccountType.PublicFolder ? "PublicFolder" :
								(ea.AccountType == ExchangeAccountType.Room ? "Room" :
								(ea.AccountType == ExchangeAccountType.Equipment ? "Equipment" :
								(ea.AccountType == ExchangeAccountType.User ? "User" :
								(ea.AccountType == ExchangeAccountType.SecurityGroup ? "SecurityGroup" :
								(ea.AccountType == ExchangeAccountType.DefaultSecurityGroup ? "DefaultSecurityGroup" :
								(ea.AccountType == ExchangeAccountType.SharedMailbox ? "SharedMailbox" :
								(ea.AccountType == ExchangeAccountType.DeletedUser ? "DeletedUser" :
								(ea.AccountType == ExchangeAccountType.JournalingMailbox ? "JournalingMailbox" :
								((int)ea.AccountType).ToString()))))))))))),
							PackageId = si.SI.PackageId,
							AccountId = ea.AccountId,
							Username = si.IT.Username,
							FullName = si.IT.FullName
						});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						spaceItemsEacPem = spaceItemsEacPem.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						spaceItemsEacPem = spaceItemsEacPem.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind) spaceItemsEacPem = spaceItemsEacPem.Take(maximumRows);

					var spaceItemsEacEm = itemsService
						.Where(si => si.ItemTypeId == 29)
						.Join(ExchangeAccounts, si => si.ItemId, ea => ea.ItemId, (si, ea) => new
						{
							IT = si,
							EA = ea
						})
						.Join(ServiceItems, ea => ea.IT.ItemId, si => si.ItemId, (ea, si) => new
						{
							ea.IT,
							ea.EA,
							SI = si
						})
						.Join(ExchangeAccountEmailAddresses, ea => ea.EA.AccountId, em => em.AccountId, (ea, em) => new SearchItem()
						{
							ItemId = ea.IT.ItemId,
							TextSearch = em.EmailAddress,
							ColumnType = "ExchangeAccount",
							FullType = ea.EA.AccountType == ExchangeAccountType.Mailbox ? "Mailbox" :
								(ea.EA.AccountType == ExchangeAccountType.Contact ? "Contact" :
								(ea.EA.AccountType == ExchangeAccountType.DistributionList ? "DistributionList" :
								(ea.EA.AccountType == ExchangeAccountType.PublicFolder ? "PublicFolder" :
								(ea.EA.AccountType == ExchangeAccountType.Room ? "Room" :
								(ea.EA.AccountType == ExchangeAccountType.Equipment ? "Equipment" :
								(ea.EA.AccountType == ExchangeAccountType.User ? "User" :
								(ea.EA.AccountType == ExchangeAccountType.SecurityGroup ? "SecurityGroup" :
								(ea.EA.AccountType == ExchangeAccountType.DefaultSecurityGroup ? "DefaultSecurityGroup" :
								(ea.EA.AccountType == ExchangeAccountType.SharedMailbox ? "SharedMailbox" :
								(ea.EA.AccountType == ExchangeAccountType.DeletedUser ? "DeletedUser" :
								(ea.EA.AccountType == ExchangeAccountType.JournalingMailbox ? "JournalingMailbox" :
								((int)ea.EA.AccountType).ToString()))))))))))),
							PackageId = ea.SI.PackageId,
							AccountId = ea.EA.AccountId,
							Username = ea.IT.Username,
							FullName = ea.IT.FullName
						});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						spaceItemsEacEm = spaceItemsEacEm.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						spaceItemsEacEm = spaceItemsEacEm.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind) spaceItemsEacEm = spaceItemsEacEm.Take(maximumRows);

					var spaceItems = spaceItemsServices
						.Union(spaceItemsDomains)
						.Union(spaceItemsEac)
						.Union(spaceItemsEacPem)
						.Union(spaceItemsEacEm);

					if (onlyFind) spaceItems = spaceItems.OrderBy(it => it.TextSearch);

					/*-------------------------------------------Lync-----------------------------------------------------*/

					var isAdmin = CheckIsUserAdmin(actorId);

					var lyncItems = ExchangeAccounts
						.Join(LyncUsers, ea => ea.AccountId, lu => lu.AccountId, (ea, lu) => new
						{
							EA = ea,
							LU = lu
						})
						.Join(ServiceItems
							.Where(si => isAdmin || si.Package.UserId == userId),
							ea => ea.EA.ItemId, si => si.ItemId, (ea, si) => new SearchItem()
							{
								ItemId = si.ItemId,
								TextSearch = ea.EA.AccountName,
								ColumnType = "LyncAccount",
								FullType = "LyncUsers",
								PackageId = si.PackageId,
								AccountId = ea.EA.AccountId,
								Username = si.Package.User.Username,
								FullName = si.Package.User.FirstName + " " + si.Package.User.LastName
							});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						lyncItems = lyncItems.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						lyncItems = lyncItems.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind)
					{
						lyncItems = lyncItems.OrderBy(li => li.TextSearch);
						lyncItems = lyncItems.Take(maximumRows);
					}

					/*-------------------------------------------SfB-----------------------------------------------------*/

					var sfbItems = ExchangeAccounts
						.Join(SfBUsers, ea => ea.AccountId, su => su.AccountId, (ea, su) => new
						{
							EA = ea,
							SU = su
						})
						.Join(ServiceItems
							.Where(si => isAdmin || si.Package.UserId == userId),
							ea => ea.EA.ItemId, si => si.ItemId, (ea, si) => new SearchItem()
							{
								ItemId = si.ItemId,
								TextSearch = ea.EA.AccountName,
								ColumnType = "LyncAccount",
								FullType = "LyncUsers",
								PackageId = si.PackageId,
								AccountId = ea.EA.AccountId,
								Username = si.Package.User.Username,
								FullName = si.Package.User.FirstName + " " + si.Package.User.LastName
							});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						sfbItems = sfbItems.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						sfbItems = sfbItems.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind)
					{
						sfbItems = sfbItems.OrderBy(si => si.TextSearch);
						sfbItems = sfbItems.Take(maximumRows);
					}

					/*------------------------------------RDS------------------------------------------------*/

					/// <summary>TODO</summary>
					IQueryable<SearchItem> rdsItems;

					if (!isAdmin) rdsItems = Enumerable.Empty<SearchItem>().AsQueryable();
					else
					{
						rdsItems = RdsCollections
							.Join(ServiceItems
									.Where(si => isAdmin || si.Package.UserId == userId),
								r => r.ItemId, si => si.ItemId, (r, si) => new SearchItem()
								{
									ItemId = si.ItemId,
									TextSearch = r.Name,
									ColumnType = "RDSCollection",
									FullType = "RDSCollections",
									PackageId = si.PackageId,
									AccountId = r.Id,
									Username = si.Package.User.Username,
									FullName = si.Package.User.FirstName + " " + si.Package.User.LastName
								});

						if (!string.IsNullOrEmpty(filterValue))
						{
#if NETFRAMEWORK
							rdsItems = rdsItems.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
							rdsItems = rdsItems.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
						}

						if (onlyFind)
						{
							rdsItems = rdsItems.OrderBy(si => si.TextSearch);
							rdsItems = rdsItems.Take(maximumRows);
						}
					}

					/*------------------------------------CRM------------------------------------------------*/

					var crmItems = ExchangeAccounts
						.Join(CrmUsers, ea => ea.AccountId, cu => cu.AccountId, (ea, cu) => new
						{
							EA = ea,
							CU = cu
						})
						.Join(ServiceItems
							.Where(si => isAdmin || si.Package.UserId == userId),
							ea => ea.EA.ItemId, si => si.ItemId, (ea, si) => new SearchItem()
							{
								ItemId = si.ItemId,
								TextSearch = ea.EA.AccountName,
								ColumnType = "CRMSite",
								FullType = "CRMSites",
								PackageId = si.PackageId,
								AccountId = ea.EA.AccountId,
								Username = si.Package.User.Username,
								FullName = si.Package.User.FirstName + " " + si.Package.User.LastName
							});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						crmItems = crmItems.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						crmItems = crmItems.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind)
					{
						crmItems = crmItems.OrderBy(li => li.TextSearch);
						crmItems = crmItems.Take(maximumRows);
					}

					/*------------------------------------VirtualServer------------------------------------------------*/

					/// <summary>TODO</summary>
					IQueryable<SearchItem> vpsItems;

					if (!isAdmin) vpsItems = Enumerable.Empty<SearchItem>().AsQueryable();
					else
					{
						var packageId = Packages
							.Where(p => p.UserId == userId)
							.Min(p => p.PackageId);
						vpsItems = Servers
							.Where(s => s.VirtualServer)
							.Join(Packages, s => s.ServerId, p => p.ServerId, (s, p) => new SearchItem()
							{
								ItemId = userId,
								TextSearch = s.ServerName,
								ColumnType = "VirtualServer",
								FullType = "VirtualServers",
								PackageId = packageId,
								AccountId = 0,
								Username = p.User.Username,
								FullName = p.User.FirstName + " " + p.User.LastName
							});

						if (!string.IsNullOrEmpty(filterValue))
						{
#if NETFRAMEWORK
							vpsItems = vpsItems.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
							vpsItems = vpsItems.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
						}

						if (onlyFind)
						{
							vpsItems = vpsItems.OrderBy(si => si.TextSearch);
							vpsItems = vpsItems.Take(maximumRows);
						}
					}

					/*------------------------------------WebDAVFolder------------------------------------------------*/

					var wdavItems = EnterpriseFolders
						.Join(ServiceItems
							.Where(si => isAdmin || si.Package.UserId == userId),
							ef => ef.ItemId, si => si.ItemId, (ef, si) => new SearchItem()
							{
								ItemId = ef.ItemId,
								TextSearch = ef.FolderName,
								ColumnType = "WebDAVFolder",
								FullType = "Folders",
								PackageId = si.PackageId,
								AccountId = ef.EnterpriseFolderId,
								Username = si.Package.User.Username,
								FullName = si.Package.User.FirstName + " " + si.Package.User.LastName
							});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						wdavItems = wdavItems.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						wdavItems = wdavItems.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind)
					{
						wdavItems = wdavItems.OrderBy(li => li.TextSearch);
						wdavItems = wdavItems.Take(maximumRows);
					}

					/*------------------------------------VPS-IP------------------------------------------------*/

					var vpsIpItems = ServiceItems
						.GroupJoin(PrivateIpAddresses, si => si.ItemId, pip => pip.ItemId, (si, pips) => new
						{
							Item = si,
							Pips = pips
						})
						.SelectMany(it => it.Pips, (si, pip) => new
						{
							si.Item,
							PrivateIp = pip != null ? pip.IpAddress : ""
						})
						.GroupJoin(PackageIpAddresses, si => si.Item.ItemId, paip => paip.ItemId, (si, paips) => new
						{
							si.Item,
							si.PrivateIp,
							Paips = paips
						})
						.SelectMany(si => si.Paips, (si, paip) => new
						{
							si.Item,
							si.PrivateIp,
							ExternalIp = paip != null && paip.Address != null ? paip.Address.ExternalIp : ""
						})
#if NETFRAMEWORK
						.Where(si => (DbFunctions.Like(filterValue, "%.%") || DbFunctions.Like(filterValue, "%:%")) &&
							(DbFunctions.Like(si.PrivateIp, filterValue) || DbFunctions.Like(si.ExternalIp, filterValue)))
#else
						.Where(si => (EF.Functions.Like(filterValue, "%.%") || EF.Functions.Like(filterValue, "%:%")) &&
							(EF.Functions.Like(si.PrivateIp, filterValue) || EF.Functions.Like(si.ExternalIp, filterValue)))
#endif
						.Join(userChildren, si => si.Item.Package.UserId, ut => ut, (si, ut) => si)
						.Join(ServiceItemTypes
							.Where(sit => sit.DisplayName == "VirtualMachine"),
							si => si.Item.ItemTypeId, sit => sit.ItemTypeId, (si, sit) => new SearchItem()
							{
								ItemId = si.Item.ItemId,
								TextSearch = si.Item.ItemName,
								ColumnType = sit.DisplayName,
								FullType = sit.DisplayName,
								PackageId = si.Item.PackageId,
								AccountId = 0,
								Username = si.Item.Package.User.Username,
								FullName = si.Item.Package.User.FirstName + " " + si.Item.Package.User.LastName
							});


					if (onlyFind)
					{
						vpsIpItems = vpsIpItems.OrderBy(li => li.TextSearch);
						vpsIpItems = vpsIpItems.Take(maximumRows);
					}

					/*------------------------------------SharePoint------------------------------------------------*/
					// Disabled, has a complicated RIGHT JOIN, don't know if this is a bug

					/*var shpItemsLeft = ServiceItems
						.Where(si => isAdmin || si.Package.UserId == userId)
						.Join(ServiceItemTypes, si => si.ItemTypeId, sit => sit.ItemTypeId, (si, sit) => new
						{
							Item = si,
							SIT = sit

						})
						.Join(ServiceItemProperties, si => si.Item.ItemId, sip => sip.ItemId, (si, sip) => new
						{
							si.Item,
							si.SIT,
							SIP = sip
						});
						
					var shpItems = ServiceItemProperties
						.GroupJoin(shpItemsLeft, t => t.ItemId, l => l.Item.ItemId, (sip, sis) => new
						{
							T = sip,
							SIS = sis
						})
						.SelectMany(t => t.SIS, (t, sis) => new SearchItem() {
							ItemId = sis != null ? sis.SIS.SIP.PropertyValue : 0,
							TextSearch = ef.FolderName,
							ColumnType = ,
							FullType = "Folders",
							PackageId = si.PackageId,
							AccountId = ef.EnterpriseFolderId,
							Username = si.Package.User.Username,
							FullName = si.Package.User.FirstName + " " + si.Package.User.LastName
						});

					if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
						shpItems = shpItems.Where(si => DbFunctions.Like(si.TextSearch, filterValue));
#else
						shpItems = shpItems.Where(si => EF.Functions.Like(si.TextSearch, filterValue));
#endif
					}

					if (onlyFind)
					{
						shpItems = shpItems.OrderBy(li => li.TextSearch);
						shpItems = shpItems.Take(maximumRows);
					}*/

					/*-------------------------------------------Return-------------------------------------------------------*/

					if (string.IsNullOrEmpty(sortColumn)) sortColumn = "TextSearch";

					if ((string.IsNullOrEmpty(colType) || colType == "AccountHome") && !string.IsNullOrEmpty(fullType))
					{
						userItems = userItems.Where(x => x.FullType == fullType);
					}

					// bug: Needs a call to Take for subquery ordering taking effect in EF6
					var itemsFilter = spaceItems.Take(int.MaxValue)
						.Concat(lyncItems.Take(int.MaxValue))
						.Concat(sfbItems.Take(int.MaxValue))
						.Concat(rdsItems.Take(int.MaxValue))
						.Concat(crmItems.Take(int.MaxValue))
						.Concat(vpsItems.Take(int.MaxValue))
						.Concat(wdavItems.Take(int.MaxValue))
						.Concat(vpsIpItems.Take(int.MaxValue));
					//.Concat(shpItems.Take(int.MaxValue));

					if (!string.IsNullOrEmpty(colType))
					{
						var types = colType
							.Split(',')
							.Select(s => s.Trim().Trim('\''))
							.Where(s => !string.IsNullOrEmpty(s));
						itemsFilter = itemsFilter.Where(BuildOrExpression<SearchItem, string>(x => x.ColumnType, types));
					}
					if (!string.IsNullOrEmpty(fullType))
					{
						itemsFilter = itemsFilter.Where(x => x.FullType == fullType);
					}

					IQueryable<SearchItem> itemsReturn = Enumerable.Empty<SearchItem>().AsQueryable();

					if (string.IsNullOrEmpty(colType) || colType == "AccountHome")
					{
						if (sortColumn == "TextSearch")
						{
							itemsReturn = userItems;
						}
						else
						{
							itemsFilter = itemsFilter.Concat(userItems);
						}
					}

					itemsFilter = itemsFilter.OrderBy(ColumnName(sortColumn));

					itemsReturn = itemsReturn
						// bug: Needs a call to Take for subquery ordering taking effect in EF6
						.Concat(itemsFilter.Take(int.MaxValue));

					var count = itemsReturn.Count();

					var colTypesSet = itemsReturn;
					if (!string.IsNullOrEmpty(fullType)) colTypesSet = colTypesSet.Where(ct => ct.FullType == fullType);
					var colTypes = colTypesSet
						.Select(ct => ct.ColumnType)
						.Distinct()
						.Select(ct => new { ColumnType = ct });

					var result = itemsReturn
						.AsEnumerable()
						.Select((x, i) => new
						{
							ItemPosition = i,
							x.ItemId,
							x.TextSearch,
							x.ColumnType,
							x.FullType,
							x.PackageId,
							x.AccountId,
							x.Username,
							x.FullName
						})
						.ToList();

					if (maximumRows > 0) result = result.Skip(startRow).Take(maximumRows).ToList();

					/// <summary>TODO</summary>
					return EntityDataSet(count, colTypes, result);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetSearchObject",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@StatusId", statusId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@RoleId", roleId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@StartRow", startRow),
					 /// <summary>TODO</summary>
					 new SqlParameter("@MaximumRows", maximumRows),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Recursive", recursive),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ColType", colType),
					 /// <summary>TODO</summary>
					 new SqlParameter("@FullType", fullType),
					 /// <summary>TODO</summary>
					 new SqlParameter("@OnlyFind", onlyFind));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetSearchTableByColumns(string PagedStored, string FilterValue, int MaximumRows,
			 bool Recursive, int PoolID, int ServerID, int ActorID, int StatusID, int PlanID, int OrgID,
			 string ItemTypeName, string GroupName, int PackageID, string VPSType, int RoleID, int UserID,
			 string FilterColumns)
		{
			if (UseEntityFramework)
			{
				//throw new NotImplementedException();

				/// <summary>TODO</summary>
				int vpsTypeId;
				switch (VPSType)
				{
					case "VPS": vpsTypeId = 33; break;
					case "VPS2012": vpsTypeId = 41; break;
					case "Proxmox": vpsTypeId = 143; break;
					case "VPSForPC": vpsTypeId = 35; break;
					default: vpsTypeId = 33; break;
				}

				IQueryable<TextSearchItem> search = null;

				using (var packagesTree = (PagedStored == "Domains" || PagedStored == "Schedules") ?
					PackagesTree(PackageID, Recursive) : null)
				using (var userChildren = PagedStored == "Users" ?
					(Recursive ? UserChildren(UserID, Recursive) : new TempIdSet(this)) : null)
				{
					switch (PagedStored)
					{
						case "Domains":

							var domains = Domains
								.Where(d => !d.IsPreviewDomain && !d.IsDomainPointer)
								.Join(packagesTree, d => d.PackageId, pt => pt, (d, pt) => d)
								.GroupJoin(ServiceItems, d => d.ZoneItemId, si => si.ItemId, (d, sis) => new
								{
									D = d,
									SIS = sis
								})
								.SelectMany(d => d.SIS.DefaultIfEmpty(), (d, si) => new
								{
									d.D,
									SI = si
								})
								.Join(Services
									.Where(s => ServerID == 0 || ServerID > 0 && s.ServerId == ServerID),
									d => d.SI != null ? d.SI.ServiceId : -1, svc => svc.ServiceId, (d, svc) => new
									{
										d.D.DomainId,
										d.D.DomainName,
										d.D.Package.User.Username,
										FullName = d.D.Package.User.FirstName + " " + d.D.Package.User.LastName,
										d.D.Package.User.Email
									});

							search = domains
								.Select(d => new TextSearchItem()
								{
									ItemId = d.DomainId,
									TextSearch = d.DomainName,
									ColumnType = "DomainName"
								})
								.Concat(domains.Select(d => new TextSearchItem()
								{
									ItemId = d.DomainId,
									TextSearch = d.Username,
									ColumnType = "Username",
								}))
								.Concat(domains.Select(d => new TextSearchItem()
								{
									ItemId = d.DomainId,
									TextSearch = d.FullName,
									ColumnType = "FullName"
								}))
								.Concat(domains.Select(d => new TextSearchItem()
								{
									ItemId = d.DomainId,
									TextSearch = d.Email,
									ColumnType = "Email"
								}));
							break;

						case "IPAddresses":

							var isAdmin = CheckIsUserAdmin(ActorID);

							var ipAddresses = IpAddresses
								.Where(ip => isAdmin &&
									(PoolID == 0 || PoolID > 0 && ip.PoolId == PoolID) &&
									(ServerID == 0 || ServerID > 0 && ip.ServerId == ServerID))
								.GroupJoin(PackageIpAddresses, ip => ip.AddressId, pip => pip.AddressId, (ip, pips) => new
								{
									IP = ip,
									PIPS = pips
								})
								.SelectMany(ip => ip.PIPS.DefaultIfEmpty(), (ip, pip) => new
								{
									ip.IP.AddressId,
									ip.IP.ExternalIp,
									ip.IP.InternalIp,
									ip.IP.DefaultGateway,
									ip.IP.Server.ServerName,
									Username = pip != null ? pip.Package.User.Username : null,
									ItemName = pip != null ? pip.Item.ItemName : null
								});

							search = ipAddresses
								.Select(ip => new TextSearchItem()
								{
									ItemId = ip.AddressId,
									TextSearch = ip.ExternalIp,
									ColumnType = "ExternalIP"
								})
								.Concat(ipAddresses.Select(ip => new TextSearchItem()
								{
									ItemId = ip.AddressId,
									TextSearch = ip.InternalIp,
									ColumnType = "InternalIP",
								}))
								.Concat(ipAddresses.Select(ip => new TextSearchItem()
								{
									ItemId = ip.AddressId,
									TextSearch = ip.DefaultGateway,
									ColumnType = "DefaultGateway"
								}))
								.Concat(ipAddresses.Select(ip => new TextSearchItem()
								{
									ItemId = ip.AddressId,
									TextSearch = ip.ServerName,
									ColumnType = "ServerName"
								}))
								.Concat(ipAddresses.Select(ip => new TextSearchItem()
								{
									ItemId = ip.AddressId,
									TextSearch = ip.Username,
									ColumnType = "UserName"
								}))
								.Concat(ipAddresses.Select(ip => new TextSearchItem()
								{
									ItemId = ip.AddressId,
									TextSearch = ip.ItemName,
									ColumnType = "ItemName"
								}));
							break;

						case "Schedules":

							var schedules = Schedules
								.Join(packagesTree, s => s.PackageId, pt => pt, (s, pt) => s)
								.Select(s => new
								{
									s.ScheduleId,
									s.ScheduleName,
									s.Package.User.Username,
									FullName = s.Package.User.FirstName + " " + s.Package.User.LastName,
									s.Package.User.Email
								});

							search = schedules
								.Select(s => new TextSearchItem()
								{
									ItemId = s.ScheduleId,
									TextSearch = s.ScheduleName,
									ColumnType = "ScheduleName"
								})
								.Concat(schedules.Select(s => new TextSearchItem()
								{
									ItemId = s.ScheduleId,
									TextSearch = s.Username,
									ColumnType = "Username",
								}))
								.Concat(schedules.Select(s => new TextSearchItem()
								{
									ItemId = s.ScheduleId,
									TextSearch = s.FullName,
									ColumnType = "FullName"
								}))
								.Concat(schedules.Select(s => new TextSearchItem()
								{
									ItemId = s.ScheduleId,
									TextSearch = s.Email,
									ColumnType = "Email"
								}));

							break;

						case "NestedPackages":

							var packages = Packages
								.Join(HostingPlans, p => p.PlanId, hp => hp.PlanId, (p, hp) => new
								{
									p.PackageId,
									p.PackageName,
									p.User.Username,
									FullName = p.User.FirstName + " " + p.User.LastName,
									p.User.Email
								});

							search = packages
								.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageId,
									TextSearch = p.PackageName,
									ColumnType = "PackageName"
								})
								.Concat(packages.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageId,
									TextSearch = p.Username,
									ColumnType = "Username",
								}))
								.Concat(packages.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageId,
									TextSearch = p.FullName,
									ColumnType = "FullName"
								}))
								.Concat(packages.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageId,
									TextSearch = p.Email,
									ColumnType = "Email"
								}));

							break;

						case "PackageIPAddresses":

							var pips = PackageIpAddresses
								.Where(pip =>
									(PoolID == 0 || PoolID > 0 && pip.Address.PoolId == PoolID) &&
									(OrgID == 0 || OrgID > 0 && pip.OrgId == OrgID))
								.Join(packagesTree, pip => pip.PackageId, pt => pt, (pip, pt) => pip)
								.GroupJoin(ServiceItems, pip => pip.ItemId, si => si.ItemId, (pip, sis) => new
								{
									pip.PackageAddressId,
									pip.Address.ExternalIp,
									pip.Address.InternalIp,
									pip.Address.DefaultGateway,
									SIS = sis,
									pip.Package.User.Username
								})
								.SelectMany(pip => pip.SIS.DefaultIfEmpty(), (pip, si) => new
								{
									pip.PackageAddressId,
									pip.ExternalIp,
									pip.InternalIp,
									pip.DefaultGateway,
									ItemName = si != null ? si.ItemName : null,
									pip.Username
								});

							search = pips
								.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageAddressId,
									TextSearch = p.ExternalIp,
									ColumnType = "ExternalIP"
								})
								.Concat(pips.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageAddressId,
									TextSearch = p.InternalIp,
									ColumnType = "InternalIP",
								}))
								.Concat(pips.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageAddressId,
									TextSearch = p.DefaultGateway,
									ColumnType = "DefaultGateway"
								}))
								.Concat(pips.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageAddressId,
									TextSearch = p.ItemName,
									ColumnType = "ItemName"
								}))
								.Concat(pips.Select(p => new TextSearchItem()
								{
									ItemId = p.PackageAddressId,
									TextSearch = p.Username,
									ColumnType = "UserName"
								}));

							break;

						case "ServiceItems":

							if (!CheckActorPackageRights(ActorID, PackageID))
								/// <summary>TODO</summary>
								throw new AccessViolationException("You are not allowed to access this package");

							var groupId = ResourceGroups
								.Where(g => g.GroupName == GroupName)
								.Select(g => (int?)g.GroupId)
								.FirstOrDefault();
							var itemTypeId = ServiceItemTypes
								.Where(sit => sit.TypeName == ItemTypeName && (groupId == null || sit.GroupId == groupId))
								.Select(sit => sit.ItemTypeId)
								.FirstOrDefault();

							var srvcItems = ServiceItems
								.Where(si => si.ItemTypeId == itemTypeId &&
									(ServerID == 0 || si.Package.ServerId == ServerID))
								.Join(packagesTree, si => si.PackageId, pt => pt, (si, pt) => si)
								.Select(si => new
								{
									si.ItemId,
									si.ItemName,
									si.Package.User.Username,
									FullName = si.Package.User.FirstName + " " + si.Package.User.LastName,
									si.Package.User.Email
								});

							search = srvcItems
								.Select(p => new TextSearchItem()
								{
									ItemId = p.ItemId,
									TextSearch = p.ItemName,
									ColumnType = "ItemName"
								})
								.Concat(srvcItems.Select(p => new TextSearchItem()
								{
									ItemId = p.ItemId,
									TextSearch = p.Username,
									ColumnType = "Username",
								}))
								.Concat(srvcItems.Select(p => new TextSearchItem()
								{
									ItemId = p.ItemId,
									TextSearch = p.FullName,
									ColumnType = "FullName"
								}))
								.Concat(srvcItems.Select(p => new TextSearchItem()
								{
									ItemId = p.ItemId,
									TextSearch = p.Email,
									ColumnType = "Email"
								}));

							break;

						case "Users":

							if (!Recursive)
							{
								userChildren.AddRange(Users
									.Where(u => u.OwnerId == UserID)
									.Select(u => u.UserId));
								SaveChanges();
							}

							var hasUserRights = CheckActorUserRights(ActorID, UserID);

							var users = Users
								.Join(userChildren, u => u.UserId, uc => uc, (u, uc) => u)
								.Where(u => hasUserRights && u.UserId != UserID && !u.IsPeer &&
									(StatusID == 0 || u.StatusId == StatusID) &&
									(RoleID == 0 || u.RoleId == RoleID))
								.Select(u => new
								{
									u.UserId,
									u.Username,
									FullName = u.FirstName + " " + u.LastName,
									u.Email,
									u.CompanyName
								});

							search = users
								.Select(u => new TextSearchItem()
								{
									ItemId = u.UserId,
									TextSearch = u.Username,
									ColumnType = "UserName"
								})
								.Concat(users.Select(u => new TextSearchItem()
								{
									ItemId = u.UserId,
									TextSearch = u.FullName,
									ColumnType = "FullName",
								}))
								.Concat(users.Select(u => new TextSearchItem()
								{
									ItemId = u.UserId,
									TextSearch = u.Email,
									ColumnType = "Email"
								}))
								.Concat(users.Select(u => new TextSearchItem()
								{
									ItemId = u.UserId,
									TextSearch = u.CompanyName,
									ColumnType = "CompanyName"
								}));
							break;

						case "VirtualMachines":

							if (!CheckActorPackageRights(ActorID, PackageID))
								/// <summary>TODO</summary>
								throw new AccessViolationException("You are not allowed to access this package");

							var packageIps = PackageIpAddresses
								.Where(p => p.IsPrimary == true)
								.Join(IpAddresses
									.Where(ip => ip.PoolId == 3), // external ip address
									p => p.AddressId, ip => ip.AddressId, (p, ip) => new
									{
										p.ItemId,
										ip.ExternalIp
									});
							var vms = Packages
								.Join(packagesTree, p => p.PackageId, pt => pt, (p, pt) => p)
								.Join(ServiceItems
									.Where(si => si.ItemTypeId == vpsTypeId),
									p => p.PackageId, si => si.PackageId, (p, si) => new
									{
										P = p,
										SI = si
									})
								.GroupJoin(packageIps, p => p.SI.ItemId, pip => pip.ItemId, (p, pips) => new
								{
									p.P,
									p.SI,
									PIPS = pips
								})
								.SelectMany(p => p.PIPS.DefaultIfEmpty(), (p, pkip) => new
								{
									p.P,
									p.SI,
									ExternalIp = pkip != null ? pkip.ExternalIp : null
								})
								.GroupJoin(PrivateIpAddresses
									.Where(ip => ip.IsPrimary),
									p => p.SI.ItemId, pip => pip.ItemId, (p, pips) => new
									{
										p.P,
										p.SI,
										p.ExternalIp,
										PIPS = pips
									})
								.SelectMany(p => p.PIPS.DefaultIfEmpty(), (p, privip) => new
								{
									p.SI.ItemId,
									p.SI.ItemName,
									p.SI.Package.User.Username,
									p.ExternalIp,
									IpAddress = privip != null ? privip.IpAddress : null
								});

							search = vms
								.Select(v => new TextSearchItem()
								{
									ItemId = v.ItemId,
									TextSearch = v.ItemName,
									ColumnType = "ItemName"
								})
								.Concat(vms.Select(v => new TextSearchItem()
								{
									ItemId = v.ItemId,
									TextSearch = v.ExternalIp,
									ColumnType = "ExternalIP",
								}))
								.Concat(vms.Select(v => new TextSearchItem()
								{
									ItemId = v.ItemId,
									TextSearch = v.Username,
									ColumnType = "Username"
								}))
								.Concat(vms.Select(v => new TextSearchItem()
								{
									ItemId = v.ItemId,
									TextSearch = v.IpAddress,
									ColumnType = "IPAddress"
								}));
							break;

						case "PackagePrivateIPAddresses":

							var ips = PrivateIpAddresses
								.Where(ip => ip.Item.PackageId == PackageID)
								.Select(ip => new
								{
									ip.PrivateAddressId,
									ip.IpAddress,
									ip.Item.ItemName
								});

							search = ips
								.Select(ip => new TextSearchItem()
								{
									ItemId = ip.PrivateAddressId,
									TextSearch = ip.IpAddress,
									ColumnType = "IpAddress"
								})
								.Concat(ips.Select(ip => new TextSearchItem()
								{
									ItemId = ip.PrivateAddressId,
									TextSearch = ip.ItemName,
									ColumnType = "ItemName",
								}));

							break;

						default:
							search = Enumerable.Empty<TextSearchItem>().AsQueryable();
							break;
					}

					search = search
						.Where(ds =>
#if NETFRAMEWORK
							DbFunctions.Like(ds.TextSearch, FilterValue)
#else
							EF.Functions.Like(ds.TextSearch, FilterValue)
#endif
						);

					if (!string.IsNullOrEmpty(FilterColumns))
					{
						var columns = FilterColumns
							.Split(',')
							.Select(s => s.Trim().Trim('\''))
							.Where(s => !string.IsNullOrEmpty(s));
						search = search
							.Where(BuildOrExpression<TextSearchItem, string>(x => x.ColumnType, columns));
					}

					var result = search
						.GroupBy(s => new { s.TextSearch, s.ColumnType })
						.Select(g => new
						{
							ItemId = g.Min(s => s.ItemId),
							g.Key.TextSearch,
							g.Key.ColumnType,
							Count = g.Count()
						})
						.OrderBy(x => x.TextSearch)
						.Take(MaximumRows);

					/// <summary>TODO</summary>
					return EntityDataSet(result);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetSearchTableByColumns",
					 /// <summary>TODO</summary>
					 new SqlParameter("@PagedStored", PagedStored),
					 /// <summary>TODO</summary>
					 new SqlParameter("@FilterValue", FilterValue),
					 /// <summary>TODO</summary>
					 new SqlParameter("@MaximumRows", MaximumRows),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Recursive", Recursive),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PoolID", PoolID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", ServerID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorID", ActorID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@StatusID", StatusID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PlanID", PlanID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@OrgID", OrgID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ItemTypeName", ItemTypeName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@GroupName", GroupName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PackageID", PackageID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@VPSType", VPSType),
					 /// <summary>TODO</summary>
					 new SqlParameter("@RoleID", RoleID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", UserID),
					 /// <summary>TODO</summary>
					 new SqlParameter("@FilterColumns", FilterColumns));
			}
		}

		//TODO END
		/// <summary>Auto-generated member.</summary>
		public DataSet GetUsersSummary(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var users = Users.Where(u => u.OwnerId == userId && !u.IsPeer);
				var nofUsers = new { UsersNumber = users.Count() };
				var usersByStatus = users
					.GroupBy(u => u.StatusId)
					.Select(g => new { StatusId = g.Key, UsersNumber = g.Count() })
					.OrderBy(g => g.StatusId);
				var usersByRole = users
					.GroupBy(u => u.RoleId)
					.Select(g => new { RoleId = g.Key, UsersNumber = g.Count() })
					.OrderByDescending(g => g.RoleId);

				var set = new DataSet();
				set.Tables.Add(EntityDataTable(new object[] { nofUsers }));
				set.Tables.Add(EntityDataTable(usersByStatus));
				set.Tables.Add(EntityDataTable(usersByRole));

				/// <summary>TODO</summary>
				return set;
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUsersSummary",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public TempIdSet UsersTree(int ownerId, bool recursive)
		{
			var tree = new TempIdSet(this);
			tree.Add(ownerId);
			SaveChanges();

			if (recursive)
			{
				int level = 0;
				var children = Users
					.Where(u => u.OwnerId == ownerId)
					.Select(u => u.UserId);
				while (tree.AddRange(children, ++level) > 0)
				{
					children = Users
						.Join(tree.OfLevel(level), u => u.OwnerId, ch => ch, (u, ch) => u.UserId);
				}
			}

			/// <summary>TODO</summary>
			return tree;
		}
		/// <summary>Auto-generated member.</summary>
		public DataSet GetUserDomainsPaged(int actorId, int userId, string filterColumn, string filterValue,
			 string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var hasRights = CheckActorUserRights(actorId, userId);

				using (var tree = UsersTree(userId, true))
				{
					var users = Users
						.Join(tree, u => u.UserId, ut => ut, (user, ut) => user)
                        .GroupJoin(Packages, u => u.UserId, p => p.UserId, (user, packages) => new { User = user, Packages = packages })
                        .SelectMany(up => up.Packages.DefaultIfEmpty(), (user, package) => new { User = user.User, PackageId = package != null ? package.PackageId : -1 })
                        .GroupJoin(Domains, up => up.PackageId, d => d.PackageId, (user, domains) => new { User = user.User, Domains = domains })
                        .SelectMany(ud => ud.Domains.DefaultIfEmpty(), (user, domain) => new
                        {
                            user.User.UserId,
							user.User.RoleId,
							user.User.StatusId,
							user.User.SubscriberNumber,
							user.User.LoginStatusId,
							user.User.FailedLogins,
							user.User.OwnerId,
							user.User.Created,
							user.User.Changed,
							user.User.IsDemo,
							user.User.Comments,
							user.User.IsPeer,
							user.User.Username,
							user.User.FirstName,
							user.User.LastName,
							user.User.Email,
                            DomainName = domain != null ? domain.DomainName : null
                        })
						.Where(u => u.UserId != userId && !u.IsPeer && hasRights);

					if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
					{
						users = users.Where(DynamicFunctions.ColumnLike(users, filterColumn, filterValue));
					}

					var count = users.Count();

					if (!string.IsNullOrEmpty(sortColumn))
					{
						users = users.OrderBy(ColumnName(sortColumn));
					}

					/// <summary>TODO</summary>
					return EntityDataSet(count, users);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserDomainsPaged",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@filterColumn", VerifyColumnName(filterColumn)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@filterValue", VerifyColumnValue(filterValue)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@sortColumn", VerifyColumnName(sortColumn)),
					 /// <summary>TODO</summary>
					 new SqlParameter("@startRow", startRow),
					 /// <summary>TODO</summary>
					 new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CanGetUserDetails(int actorId, int userId)
		{
			if (actorId == -1 || actorId == userId) return true;

			var actor = Users
				.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
				.FirstOrDefault(u => u.UserId == actorId);
			if (actor != null && actor.IsPeer && actor.OwnerId.HasValue)
			{
				actorId = actor.OwnerId.Value;
				actor = Users
					.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
					.FirstOrDefault(u => u.UserId == actorId);
			}

			if (actor != null && actor.OwnerId.HasValue && userId == actor.OwnerId.Value) return true;

			var id = userId;
			var user = Users
				.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
				.FirstOrDefault(u => u.UserId == id);
			while (user != null && user.OwnerId.HasValue && user.OwnerId != actorId)
			{
				id = user.OwnerId.Value;
				user = Users
					.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
					.FirstOrDefault(u => u.UserId == id);
			}

			return user != null && user.OwnerId.HasValue && user.OwnerId == actorId;
		}
		/// <summary>Auto-generated member.</summary>
		public DataSet GetUsers(int actorId, int ownerId, bool recursive)
		{
			if (UseEntityFramework)
			{
				var canGetDetails = CanGetUserDetails(actorId, ownerId);

				using (var childUsers = UserChildren(ownerId, recursive))
				{
					var users = UsersDetailed
						.Where(u => canGetDetails && u.UserId != ownerId && !u.IsPeer)
						.Join(childUsers, u => u.UserId, ch => ch, (u, ch) => new
						{
							u.UserId,
							u.RoleId,
							u.StatusId,
							u.SubscriberNumber,
							u.LoginStatusId,
							u.FailedLogins,
							u.OwnerId,
							u.Created,
							u.Changed,
							u.IsDemo,
							u.Comments,
							u.IsPeer,
							u.Username,
							u.FirstName,
							u.LastName,
							u.Email,
							u.FullName,
							u.OwnerUsername,
							u.OwnerFirstName,
							u.OwnerLastName,
							u.OwnerRoleId,
							u.OwnerFullName,
							u.PackagesNumber,
							u.CompanyName,
							u.EcommerceEnabled
						});

					/// <summary>TODO</summary>
					return EntityDataSet(users);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUsers",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@OwnerID", ownerId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Recursive", recursive));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetUserParents(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorUserRights(actorId, userId)) throw new AccessViolationException("You are not allowed to access this account");

				using (var parents = UserParents(actorId, userId).ToTempIdSet(this))
				{
					var tempIds = parents.TempIds();
					var users = Users
						.Join(tempIds, u => u.UserId, p => p.Id, (user, parent) => new { User = user, Order = parent.Key })
						.OrderByDescending(u => u.Order)
						.Select(u => new
						{
							u.User.UserId,
							u.User.RoleId,
							u.User.StatusId,
							u.User.SubscriberNumber,
							u.User.LoginStatusId,
							u.User.FailedLogins,
							u.User.OwnerId,
							u.User.Created,
							u.User.Changed,
							u.User.IsDemo,
							u.User.Comments,
							u.User.IsPeer,
							u.User.Username,
							u.User.FirstName,
							u.User.LastName,
							u.User.Email,
							u.User.CompanyName,
							u.User.EcommerceEnabled
						});
					/// <summary>TODO</summary>
					return EntityDataSet(users);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserParents",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetUserPeers(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				var canGetDetails = CanGetUserDetails(actorId, userId);

				var userPeers = Users
					.Where(u => canGetDetails && u.OwnerId == userId && u.IsPeer)
					.Select(u => new
					{
						u.UserId,
						u.RoleId,
						u.StatusId,
						u.LoginStatusId,
						u.FailedLogins,
						u.OwnerId,
						u.Created,
						u.Changed,
						u.IsDemo,
						u.Comments,
						u.IsPeer,
						u.Username,
						u.FirstName,
						u.LastName,
						u.Email,
						FullName = u.FirstName + " " + u.LastName,
						u.CompanyName,
						u.EcommerceEnabled
					});
				/// <summary>TODO</summary>
				return EntityDataSet(userPeers);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserPeers",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@userId", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetUserByExchangeOrganizationIdInternally(int itemId)
		{
			if (UseEntityFramework)
			{
				var users = Users
					.Join(Packages, u => u.UserId, p => p.UserId, (user, package) => new { User = user, package.PackageId })
					.Join(ServiceItems, u => u.PackageId, s => s.PackageId, (user, serviceItem) => new { user.User, serviceItem.ItemId })
					.Where(u => u.ItemId == itemId)
					.Select(u => new
					{
						u.User.UserId,
                        u.User.RoleId,
						u.User.StatusId,
						u.User.SubscriberNumber,
						u.User.LoginStatusId,
						u.User.FailedLogins,
						u.User.OwnerId,
						u.User.Created,
						u.User.Changed,
						u.User.IsDemo,
						u.User.Comments,
						u.User.IsPeer,
						u.User.Username,
						u.User.Password,
						u.User.FirstName,
						u.User.LastName,
						u.User.Email,
						u.User.SecondaryEmail,
						u.User.Address,
						u.User.City,
						u.User.State,
						u.User.Country,
						u.User.Zip,
						u.User.PrimaryPhone,
						u.User.SecondaryPhone,
						u.User.Fax,
						u.User.InstantMessenger,
						u.User.HtmlMail,
						u.User.CompanyName,
						u.User.EcommerceEnabled,
						u.User.AdditionalParams
					});
				/// <summary>TODO</summary>
				return EntityDataReader(users);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserByExchangeOrganizationIdInternally",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetUserByIdInternally(int userId)
		{
			if (UseEntityFramework)
			{
				var users = Users
					.Where(u => u.UserId == userId)
					.Select(u => new
					{
						u.UserId,
						u.RoleId,
						u.StatusId,
						u.SubscriberNumber,
						u.LoginStatusId,
						u.FailedLogins,
						u.OwnerId,
						u.Created,
						u.Changed,
						u.IsDemo,
						u.Comments,
						u.IsPeer,
						u.Username,
						u.Password,
						u.FirstName,
						u.LastName,
						u.Email,
						u.SecondaryEmail,
						u.Address,
						u.City,
						u.State,
						u.Country,
						u.Zip,
						u.PrimaryPhone,
						u.SecondaryPhone,
						u.Fax,
						u.InstantMessenger,
						u.HtmlMail,
						u.CompanyName,
						u.EcommerceEnabled,
						u.AdditionalParams,
						u.OneTimePasswordState,
						u.MfaMode,
						u.PinSecret
					});
				/// <summary>TODO</summary>
				return EntityDataReader(users);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserByIdInternally",
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetUserByUsernameInternally(string username)
		{
			if (UseEntityFramework)
			{
				var users = Users
					.Where(u => u.Username == username)
					.Select(u => new
					{
						u.UserId,
						u.RoleId,
						u.StatusId,
						u.SubscriberNumber,
						u.LoginStatusId,
						u.FailedLogins,
						u.OwnerId,
						u.Created,
						u.Changed,
						u.IsDemo,
						u.Comments,
						u.IsPeer,
						u.Username,
						u.Password,
						u.FirstName,
						u.LastName,
						u.Email,
						u.SecondaryEmail,
						u.Address,
						u.City,
						u.State,
						u.Country,
						u.Zip,
						u.PrimaryPhone,
						u.SecondaryPhone,
						u.Fax,
						u.InstantMessenger,
						u.HtmlMail,
						u.CompanyName,
						u.EcommerceEnabled,
						u.AdditionalParams,
						u.OneTimePasswordState,
						u.MfaMode,
						u.PinSecret
					});
				/// <summary>TODO</summary>
				return EntityDataReader(users);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserByUsernameInternally",
					 /// <summary>TODO</summary>
					 new SqlParameter("@Username", username));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CanGetUserPassword(int actorId, int userId)
		{
			if (actorId == -1 || actorId == userId) return true;

			var actor = Users
				.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
				.FirstOrDefault(u => u.UserId == actorId);
			if (actor != null && actor.IsPeer)
			{
				// peer can't get the password of his peers and his owner
				if (actor.OwnerId == userId ||
					Users.Any(u => u.IsPeer && u.OwnerId == actor.OwnerId && u.UserId == userId)) return false;

				// set actor to his owner
				actorId = actor.OwnerId ?? -1;
			}

			// get users owner
			var owner = Users
				.Select(u => new { u.UserId, u.OwnerId })
				.FirstOrDefault(u => u.UserId == actorId);
			if (owner != null && owner.OwnerId == userId) return false; // user can't get the password of his owner

			var id = userId;
			var user = Users
				.Select(u => new { u.UserId, u.OwnerId })
				.FirstOrDefault(u => u.UserId == id);
			while (user != null && user.OwnerId.HasValue && user.OwnerId != actorId)
			{
				id = user.OwnerId.Value;
				user = Users
					.Select(u => new { u.UserId, u.OwnerId })
					.FirstOrDefault(u => u.UserId == id);
			}
			return user != null && user.OwnerId == actorId; // actor is owner of user
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetUserById(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				var canGetUserDetails = CanGetUserDetails(actorId, userId);
				var canGetUserPassword = CanGetUserPassword(actorId, userId);
				var user = Users
					.Where(u => u.UserId == userId && canGetUserDetails)
					.Select(u => new UserInfoInternal()
					{
						UserId = u.UserId,
						RoleId = u.RoleId,
						StatusId = u.StatusId,
						SubscriberNumber = u.SubscriberNumber,
						LoginStatusId = u.LoginStatusId ?? 0,
						FailedLogins = u.FailedLogins ?? 0,
						OwnerId = u.OwnerId ?? 0,
						Created = u.Created ?? default(DateTime),
						Changed = u.Changed ?? default(DateTime),
						IsDemo = u.IsDemo,
						Comments = u.Comments,
						IsPeer = u.IsPeer,
						Username = u.Username,
						Password = canGetUserPassword ? u.Password : "",
						FirstName = u.FirstName,
						LastName = u.LastName,
						Email = u.Email,
						SecondaryEmail = u.SecondaryEmail,
						Address = u.Address,
						City = u.City,
						State = u.State,
						Country = u.Country,
						Zip = u.Zip,
						PrimaryPhone = u.PrimaryPhone,
						SecondaryPhone = u.SecondaryPhone,
						Fax = u.Fax,
						InstantMessenger = u.InstantMessenger,
						HtmlMail = u.HtmlMail ?? false,
						CompanyName = u.CompanyName,
						EcommerceEnabled = u.EcommerceEnabled ?? false,
						AdditionalParams = u.AdditionalParams,
						MfaMode = u.MfaMode,
						PinSecret = canGetUserPassword ? u.PinSecret : ""
					});

				/// <summary>TODO</summary>
				return EntityDataReader(user);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserById",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetUserByUsername(int actorId, string username)
		{
			if (UseEntityFramework)
			{
				var user = Users
					.Where(u => u.Username == username)
					.AsEnumerable()
					.Select(u =>
					{
						var canGetUserPassword = CanGetUserPassword(actorId, u.UserId);
						/// <summary>TODO</summary>
						return new UserInfoInternal()
						{
							UserId = u.UserId,
							RoleId = u.RoleId,
							StatusId = u.StatusId,
							SubscriberNumber = u.SubscriberNumber,
							LoginStatusId = u.LoginStatusId ?? 0,
							FailedLogins = u.FailedLogins ?? 0,
							OwnerId = u.OwnerId ?? 0,
							Created = u.Created ?? default(DateTime),
							Changed = u.Changed ?? default(DateTime),
							IsDemo = u.IsDemo,
							Comments = u.Comments,
							IsPeer = u.IsPeer,
							Username = u.Username,
							Password = canGetUserPassword ? u.Password : "",
							FirstName = u.FirstName,
							LastName = u.LastName,
							Email = u.Email,
							SecondaryEmail = u.SecondaryEmail,
							Address = u.Address,
							City = u.City,
							State = u.State,
							Country = u.Country,
							Zip = u.Zip,
							PrimaryPhone = u.PrimaryPhone,
							SecondaryPhone = u.SecondaryPhone,
							Fax = u.Fax,
							InstantMessenger = u.InstantMessenger,
							HtmlMail = u.HtmlMail ?? false,
							CompanyName = u.CompanyName,
							EcommerceEnabled = u.EcommerceEnabled ?? false,
							AdditionalParams = u.AdditionalParams,
							MfaMode = u.MfaMode,
							PinSecret = canGetUserPassword ? u.PinSecret : ""
						};
					})
					.Where(u => Clone.CanGetUserDetails(actorId, u.UserId));

				/// <summary>TODO</summary>
				return EntityDataReader(user);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserByUsername",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Username", username));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CanCreateUser(int actorId, int ownerId)
		{
			if (actorId == -1 || actorId == ownerId) return true;

			var actor = Users
				.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
				.FirstOrDefault(u => u.UserId == actorId);
			if (actor != null && actor.IsPeer && actor.OwnerId.HasValue) actorId = actor.OwnerId.Value;
			if (actorId == ownerId) return true;

			var id = ownerId;
			var user = Users
				.Select(u => new { u.UserId, u.OwnerId })
				.FirstOrDefault(u => u.UserId == id);
			while (user != null && user.OwnerId.HasValue && user.OwnerId != actorId)
			{
				id = user.OwnerId.Value;
				user = Users
					.Select(u => new { u.UserId, u.OwnerId })
					.FirstOrDefault(u => u.UserId == id);
			}
			return user != null && user.OwnerId == actorId;
		}
		/// <summary>Auto-generated member.</summary>

		public int AddUser(int actorId, int ownerId, int roleId, int statusId, string subscriberNumber, int loginStatusId, bool isDemo,
			 bool isPeer, string comments, string username, string password,
			 string firstName, string lastName, string email, string secondaryEmail,
			 string address, string city, string country, string state, string zip,
			 string primaryPhone, string secondaryPhone, string fax, string instantMessenger, bool htmlMail,
			 string companyName, bool ecommerceEnabled)
		{
			if (UseEntityFramework)
			{
				if (Users.Any(u => u.Username == username)) return -1;
				if (!CanCreateUser(actorId, ownerId)) return -2;
				var user = new Data.Entities.User()
				{
					OwnerId = ownerId,
					RoleId = roleId,
					StatusId = statusId,
					SubscriberNumber = subscriberNumber,
					LoginStatusId = loginStatusId,
					Created = DateTime.Now,
					Changed = DateTime.Now,
					IsDemo = isDemo,
					IsPeer = isPeer,
					Comments = comments,
					Username = username,
					Password = password,
					FirstName = firstName,
					LastName = lastName,
					Email = email,
					SecondaryEmail = secondaryEmail,
					Address = address,
					City = city,
					Country = country,
					State = state,
					Zip = zip,
					PrimaryPhone = primaryPhone,
					SecondaryPhone = secondaryPhone,
					Fax = fax,
					InstantMessenger = instantMessenger,
					HtmlMail = htmlMail,
					CompanyName = companyName,
					EcommerceEnabled = ecommerceEnabled
				};
				Users.Add(user);
				SaveChanges();

				return user.UserId;
			}
			else
			{
				SqlParameter prmUserId = new SqlParameter("@UserID", SqlDbType.Int);
				prmUserId.Direction = ParameterDirection.Output;

				// add user to FuseCP Users table
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "AddUser",
					 prmUserId,
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@OwnerID", ownerId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@RoleID", roleId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@StatusId", statusId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SubscriberNumber", subscriberNumber),
					 /// <summary>TODO</summary>
					 new SqlParameter("@LoginStatusId", loginStatusId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@IsDemo", isDemo),
					 /// <summary>TODO</summary>
					 new SqlParameter("@IsPeer", isPeer),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Comments", comments),
					 /// <summary>TODO</summary>
					 new SqlParameter("@username", username),
					 /// <summary>TODO</summary>
					 new SqlParameter("@password", password),
					 /// <summary>TODO</summary>
					 new SqlParameter("@firstName", firstName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@lastName", lastName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@email", email),
					 /// <summary>TODO</summary>
					 new SqlParameter("@secondaryEmail", secondaryEmail),
					 /// <summary>TODO</summary>
					 new SqlParameter("@address", address),
					 /// <summary>TODO</summary>
					 new SqlParameter("@city", city),
					 /// <summary>TODO</summary>
					 new SqlParameter("@country", country),
					 /// <summary>TODO</summary>
					 new SqlParameter("@state", state),
					 /// <summary>TODO</summary>
					 new SqlParameter("@zip", zip),
					 /// <summary>TODO</summary>
					 new SqlParameter("@primaryPhone", primaryPhone),
					 /// <summary>TODO</summary>
					 new SqlParameter("@secondaryPhone", secondaryPhone),
					 /// <summary>TODO</summary>
					 new SqlParameter("@fax", fax),
					 /// <summary>TODO</summary>
					 new SqlParameter("@instantMessenger", instantMessenger),
					 /// <summary>TODO</summary>
					 new SqlParameter("@htmlMail", htmlMail),
					 /// <summary>TODO</summary>
					 new SqlParameter("@CompanyName", companyName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@EcommerceEnabled", ecommerceEnabled));

				return Convert.ToInt32(prmUserId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CanUpdateUserDetails(int actorId, int userId)
		{
			if (actorId == -1 || actorId == userId) return true;

			var actor = Users
				.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
				.FirstOrDefault(u => u.UserId == actorId);
			if (actor != null && actor.IsPeer && actor.OwnerId.HasValue)
			{
				// check if the peer is trying to update his owner
				if (actor.OwnerId == userId) return false;

				// check if the peer is trying to update his peers
				if (Users.Any(u => u.IsPeer && u.OwnerId == actor.OwnerId && u.UserId == userId)) return false;

				actorId = actor.OwnerId.Value;
			}

			var id = userId;
			var user = Users
				.Select(u => new { u.UserId, u.OwnerId })
				.FirstOrDefault(u => u.UserId == id);
			while (user != null && user.OwnerId.HasValue && user.OwnerId != actorId)
			{
				id = user.OwnerId.Value;
				user = Users
					.Select(u => new { u.UserId, u.OwnerId })
					.FirstOrDefault(u => u.UserId == id);
			}
			return user != null && user.OwnerId == actorId;
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateUser(int actorId, int userId, int roleId, int statusId, string subscriberNumber, int loginStatusId, bool isDemo,
			 bool isPeer, string comments, string firstName, string lastName, string email, string secondaryEmail,
			 string address, string city, string country, string state, string zip,
			 string primaryPhone, string secondaryPhone, string fax, string instantMessenger, bool htmlMail,
			 string companyName, bool ecommerceEnabled, string additionalParams)
		{
			if (UseEntityFramework)
			{
				if (CanUpdateUserDetails(actorId, userId))
				{
					var user = Users.FirstOrDefault(u => u.UserId == userId);
					if (user != null)
					{
						if (loginStatusId == 0) user.FailedLogins = 0;
						user.RoleId = roleId;
						user.StatusId = statusId;
						user.SubscriberNumber = subscriberNumber;
						user.LoginStatusId = loginStatusId;
						user.Changed = DateTime.Now;
                        user.IsDemo = isDemo;
						user.IsPeer = isPeer;
						user.Comments = comments;
						user.FirstName = firstName;
						user.LastName = lastName;
						user.Email = email;
						user.SecondaryEmail = secondaryEmail;
						user.Address = address;
						user.City = city;
						user.State = state;
                        user.Country = country;
                        user.Zip = zip;
						user.PrimaryPhone = primaryPhone;
						user.SecondaryPhone = secondaryPhone;
						user.Fax = fax;
						user.InstantMessenger = instantMessenger;
						user.HtmlMail = htmlMail;
						user.CompanyName = companyName;
						user.EcommerceEnabled = ecommerceEnabled;
						user.AdditionalParams = additionalParams;

						SaveChanges();
					}
				}
			}
			else
			{
				// update user
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdateUser",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@RoleID", roleId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@StatusId", statusId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SubscriberNumber", subscriberNumber),
					 /// <summary>TODO</summary>
					 new SqlParameter("@LoginStatusId", loginStatusId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@IsDemo", isDemo),
					 /// <summary>TODO</summary>
					 new SqlParameter("@IsPeer", isPeer),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Comments", comments),
					 /// <summary>TODO</summary>
					 new SqlParameter("@firstName", firstName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@lastName", lastName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@email", email),
					 /// <summary>TODO</summary>
					 new SqlParameter("@secondaryEmail", secondaryEmail),
					 /// <summary>TODO</summary>
					 new SqlParameter("@address", address),
					 /// <summary>TODO</summary>
					 new SqlParameter("@city", city),
					 /// <summary>TODO</summary>
					 new SqlParameter("@country", country),
					 /// <summary>TODO</summary>
					 new SqlParameter("@state", state),
					 /// <summary>TODO</summary>
					 new SqlParameter("@zip", zip),
					 /// <summary>TODO</summary>
					 new SqlParameter("@primaryPhone", primaryPhone),
					 /// <summary>TODO</summary>
					 new SqlParameter("@secondaryPhone", secondaryPhone),
					 /// <summary>TODO</summary>
					 new SqlParameter("@fax", fax),
					 /// <summary>TODO</summary>
					 new SqlParameter("@instantMessenger", instantMessenger),
					 /// <summary>TODO</summary>
					 new SqlParameter("@htmlMail", htmlMail),
					 /// <summary>TODO</summary>
					 new SqlParameter("@CompanyName", companyName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@EcommerceEnabled", ecommerceEnabled),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdditionalParams", additionalParams));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateUserFailedLoginAttempt(int userId, int lockOut, bool reset)
		{
			if (UseEntityFramework)
			{
				var user = Users.FirstOrDefault(u => u.UserId == userId);
				if (user == null) return;

				if (reset) user.FailedLogins = 0;
                /// <summary>TODO</summary>
                else if (lockOut <= (user.FailedLogins ?? int.MinValue)) user.LoginStatusId = 2;
                else user.FailedLogins = (user.FailedLogins ?? 0) + 1;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdateUserFailedLoginAttempt",
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@LockOut", lockOut),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Reset", reset));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteUser(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				if (!CanUpdateUserDetails(actorId, userId)) return;

				using (var transaction = Database.BeginTransaction())
				{
					// delete user comments
					Comments.Where(c => c.ItemId == userId && c.ItemTypeId == "USER").ExecuteDelete();
					// delete reseller addon
					HostingPlans.Where(h => h.UserId == userId && h.IsAddon == true).ExecuteDelete();
					// delete user peers
					Users.Where(u => u.IsPeer && u.OwnerId == userId).ExecuteDelete();
					// delete user
					Users.Where(u => u.UserId == userId).ExecuteDelete();

					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "DeleteUser",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void ChangeUserPassword(int actorId, int userId, string password)
		{
			if (UseEntityFramework)
			{
				if (!CanUpdateUserDetails(actorId, userId)) return;

				var user = Users.FirstOrDefault(u => u.UserId == userId);
				if (user == null) return;

				user.Password = password;
				user.OneTimePasswordState = 0;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "ChangeUserPassword",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@password", password));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void SetUserOneTimePassword(int userId, string password, int auths)
		{
			if (UseEntityFramework)
			{
				var user = Users.FirstOrDefault(u => u.UserId == userId);
				if (user == null) return;

				user.Password = password;
				user.OneTimePasswordState = auths;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "SetUserOneTimePassword",
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Password", password),
					 /// <summary>TODO</summary>
					 new SqlParameter("@OneTimePasswordState", auths));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateUserPinSecret(int actorId, int userId, string pinSecret)
		{
			if (UseEntityFramework)
			{
				if (!CanUpdateUserDetails(actorId, userId)) return;

				var user = Users.FirstOrDefault(u => u.UserId == userId);
				if (user == null) return;

				user.PinSecret = pinSecret;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdateUserPinSecret",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PinSecret", pinSecret)
					 );
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateUserMfaMode(int actorId, int userId, int mfaMode)
		{
			if (UseEntityFramework)
			{
				if (!CanUpdateUserDetails(actorId, userId)) return;

				var user = Users.FirstOrDefault(u => u.UserId == userId);
				if (user == null) return;

				user.MfaMode = mfaMode;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdateUserMfaMode",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@MfaMode", mfaMode));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CanUserChangeMfa(int callerId, int changeUserId, bool canPeerChangeMfa)
		{
			if (UseEntityFramework)
			{
				var user = Users
					.Select(u => new { u.UserId, u.OwnerId, u.IsPeer })
					.FirstOrDefault(u => u.UserId == callerId);
				if (user == null) return false;

				if (user.OwnerId == null) return true; // serveradmin user

				// check if the user requests himself
				if (callerId == changeUserId && user.IsPeer && canPeerChangeMfa) return true;

				if (callerId == changeUserId && !user.IsPeer) return true;

				int generationNumber = 0, userId = user.UserId;
				if (user.IsPeer)
				{
					userId = user.OwnerId.Value;
					generationNumber = 1;
				}

				var generation = Users
					.Where(u => u.UserId == userId)
					.Select(u => new
					{
						u.UserId,
						u.Username,
						u.OwnerId,
						u.IsPeer,
						GenerationNumber = 0
					});
				var nextGeneration = Users.Join(generation, u => u.OwnerId, g => g.UserId, (usr, gen) => new
				{
					usr.UserId,
					usr.Username,
					usr.OwnerId,
					usr.IsPeer,
					GenerationNumber = gen.GenerationNumber + 1
				});
				while (nextGeneration.Any())
				{
					generation = generation.Concat(nextGeneration);
				}

				return generation.Join(Users, g => g.OwnerId, u => u.UserId, (gen, usr) => gen)
					.Any(g => (g.GenerationNumber > generationNumber || !g.IsPeer) &&
						g.UserId == changeUserId);
			}
			else
			{
				SqlParameter prmResult = new SqlParameter("@Result", SqlDbType.Bit);
				prmResult.Direction = ParameterDirection.Output;
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "CanChangeMfa",
					 /// <summary>TODO</summary>
					 new SqlParameter("@CallerID", callerId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ChangeUserID", changeUserId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@CanPeerChangeMfa", canPeerChangeMfa ? 1 : 0),
					 prmResult
					 );

				return Convert.ToBoolean(prmResult.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetUserPackagesServerUrls(int userId)
		{
			if (UseEntityFramework)
			{
				var servers = Packages
					.Where(package => package.UserId == userId)
					.Select(package => package.Server);
				var ids = servers
					.Where(server => !server.VirtualServer)
					.Select(server => server.ServerId)
					.Union(servers
					.Where(server => server.VirtualServer)
					.SelectMany(server => server.VirtualServices
						.Select(vs => vs.Service.Server.ServerId)));
				var urls = ids
					.Join(Servers, id => id, server => server.ServerId, (id, server) => new {
						server.ServerUrl
					});

				/// <summary>TODO</summary>
				return EntityDataReader(urls);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserPackagesServerUrls",
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserId", userId));
			}
		}

		#endregion

		#region User Settings
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetUserSettings(int actorId, int userId, string settingsName)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var id = userId;
				var setting = UserSettings.Where(s => s.SettingsName == settingsName && s.UserId == id);
				while (!setting.Any())
				{
					var user = Users
						.Select(u => new { u.UserId, u.OwnerId })
						.FirstOrDefault(u => u.UserId == id);
					if (user != null && user.OwnerId.HasValue) id = user.OwnerId.Value;
					/// <summary>TODO</summary>
					else break;
					setting = UserSettings.Where(s => s.SettingsName == settingsName && s.UserId == id);
				}

				/// <summary>TODO</summary>
				return EntityDataReader(setting);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetUserSettings",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SettingsName", settingsName));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void UpdateUserSettings(int actorId, int userId, string settingsName, string xml)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				UserSettings.RemoveRange(UserSettings
					.Where(s => s.UserId == userId && s.SettingsName == settingsName));

				var properties = XElement.Parse(xml);
                var settings = properties.Elements()
                    .Select(property => new Data.Entities.UserSetting()
                    {
                        UserId = userId,
                        SettingsName = settingsName,
                        PropertyName = (string)property.Attribute("name"),
                        PropertyValue = (string)property.Attribute("value")
                    });
                UserSettings.AddRange(settings);

                SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdateUserSettings",
					 /// <summary>TODO</summary>
					 new SqlParameter("@UserID", userId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ActorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SettingsName", settingsName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Xml", xml));
			}
		}
		#endregion

		#region Servers
		/// <summary>Auto-generated member.</summary>
		public bool CheckIsUserAdmin(int userId) =>
			userId == -1 || Users.Any(u => u.UserId == userId && u.RoleId == 1);
		/// <summary>Auto-generated member.</summary>
		public DataSet GetAllServers(int actorId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var servers = Servers
					.Where(s => isAdmin)
					.OrderBy(s => s.VirtualServer)
					.ThenBy(s => s.ServerName)
					.Select(s => new
					{
						s.ServerId,
						s.ServerName,
						s.ServerUrl,
						ServicesNumber = VirtualServices.Count(v => v.ServerId == s.ServerId),
						s.Comments
					});
				
				/// <summary>TODO</summary>
				return EntityDataSet(servers);
            }
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetAllServers",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public DataSet GetServers(int actorId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var servers = Servers
					.Where(s => isAdmin && !s.VirtualServer)
					.OrderBy(s => s.ServerName)
					.Select(s => new
					{
						s.ServerId,
						s.ServerName,
						s.ServerUrl,
						ServicesNumber = Services.Where(sc => sc.ServerId == s.ServerId).Count(),
						s.Comments,
						s.PrimaryGroupId,
						s.ADEnabled
					});

				var services = Services
					.Where(s => isAdmin)
					.Join(Providers, srvc => srvc.ProviderId, p => p.ProviderId, (srvc, prov) => new
					{
						Service = srvc,
						prov.GroupId
					})
					.Join(ResourceGroups, srvc => srvc.GroupId, rg => rg.GroupId, (srvc, rg) => new
					{
						srvc.Service,
						rg.GroupOrder
					})
					.OrderBy(s => s.GroupOrder)
					.Select(s => new
					{
						s.Service.ServiceId,
						s.Service.ServerId,
						s.Service.ProviderId,
						s.Service.ServiceName,
						s.Service.Comments
					});

				/// <summary>TODO</summary>
				return EntityDataSet(servers, services);
            }
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetServers",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetServer(int actorId, int serverId, bool forAutodiscover)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var server = Servers
					.Where(s => s.ServerId == serverId && (isAdmin || forAutodiscover))
					.Select(s => new
					{
						s.ServerId,
						s.ServerName,
						s.ServerUrl,
						s.Password,
						s.Comments,
						s.VirtualServer,
						s.InstantDomainAlias,
						s.PrimaryGroupId,
						s.ADEnabled,
						s.ADRootDomain,
						s.ADUsername,
						s.ADPassword,
						s.ADAuthenticationType,
						s.ADParentDomain,
						s.ADParentDomainController,
						s.OSPlatform,
						s.IsCore,
						s.PasswordIsSHA256
					});

				/// <summary>TODO</summary>
				return EntityDataReader(server);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetServer",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@forAutodiscover", forAutodiscover));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetServerShortDetails(int serverId)
		{
			if (UseEntityFramework)
			{
				var server = Servers
					.Where(s => s.ServerId == serverId)
					.Select(s => new
					{
						s.ServerId,
						s.ServerName,
						s.Comments,
						s.VirtualServer,
						s.InstantDomainAlias
					});

				/// <summary>TODO</summary>
				return EntityDataReader(server);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetServerShortDetails",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetServerByName(int actorId, string serverName)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var server = Servers
					.Where(s => isAdmin && s.ServerName == serverName)
					.Select(s => new
					{
						s.ServerId,
						s.ServerName,
						s.ServerUrl,
						s.Password,
						s.Comments,
						s.VirtualServer,
						s.InstantDomainAlias,
						s.PrimaryGroupId,
						s.ADRootDomain,
						s.ADUsername,
						s.ADPassword,
						s.ADAuthenticationType,
						s.ADParentDomain,
						s.ADParentDomainController,
						s.OSPlatform,
						s.IsCore,
						s.PasswordIsSHA256
					});

				/// <summary>TODO</summary>
				return EntityDataReader(server);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetServerByName",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerName", serverName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetServerInternal(int serverId)
		{
			if (UseEntityFramework)
			{
				var server = Servers
					.Where(s => s.ServerId == serverId)
					.Select(s => new
					{
						s.ServerId,
						s.ServerName,
						s.ServerUrl,
						s.Password,
						s.Comments,
						s.VirtualServer,
						s.InstantDomainAlias,
						s.PrimaryGroupId,
						s.ADEnabled,
						s.ADRootDomain,
						s.ADUsername,
						s.ADPassword,
						s.ADAuthenticationType,
						s.ADParentDomain,
						s.ADParentDomainController,
						s.OSPlatform,
						s.IsCore,
						s.PasswordIsSHA256
					});

				/// <summary>TODO</summary>
				return EntityDataReader(server);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetServerInternal",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddServer(string serverName, string serverUrl,
			 string password, string comments, bool virtualServer, string instantDomainAlias,
			 int primaryGroupId, bool adEnabled, string adRootDomain, string adUsername, string adPassword,
			 string adAuthenticationType, OSPlatform osPlatform, bool? isCore, bool passwordIsSHA256)
		{
			if (UseEntityFramework)
			{
				var server = new Data.Entities.Server()
				{
					ServerName = serverName,
					ServerUrl = serverUrl,
					Password = password,
					Comments = comments,
					VirtualServer = virtualServer,
					InstantDomainAlias = instantDomainAlias,
					PrimaryGroupId = primaryGroupId != 0 ? primaryGroupId : null,
					ADEnabled = adEnabled,
					ADRootDomain = adRootDomain,
					ADUsername = adUsername,
					ADPassword = adPassword,
					ADAuthenticationType = adAuthenticationType,
					OSPlatform = osPlatform,
					IsCore = isCore,
					PasswordIsSHA256 = passwordIsSHA256
				};
				Servers.Add(server);

				SaveChanges();

				return server.ServerId;
			}
			else
			{
				SqlParameter prmServerId = new SqlParameter("@ServerID", SqlDbType.Int);
				prmServerId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "AddServer",
					 prmServerId,
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerName", serverName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerUrl", serverUrl),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Password", password),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Comments", comments),
					 /// <summary>TODO</summary>
					 new SqlParameter("@VirtualServer", virtualServer),
					 /// <summary>TODO</summary>
					 new SqlParameter("@InstantDomainAlias", instantDomainAlias),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PrimaryGroupId", primaryGroupId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdEnabled", adEnabled),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdRootDomain", adRootDomain),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdUsername", adUsername),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdPassword", adPassword),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdAuthenticationType", adAuthenticationType),
					 /// <summary>TODO</summary>
					 new SqlParameter("@OSPlatform", osPlatform),
					 /// <summary>TODO</summary>
					 new SqlParameter("@IsCore", isCore),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PasswordIsSHA256", passwordIsSHA256));

				return Convert.ToInt32(prmServerId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateServer(int serverId, string serverName, string serverUrl,
			 string password, string comments, string instantDomainAlias,
			 int primaryGroupId, bool adEnabled, string adRootDomain, string adUsername, string adPassword,
			 string adAuthenticationType, string adParentDomain, String adParentDomainController,
			 OSPlatform osPlatform, bool? isCore, bool PasswordIsSHA256)
		{
			if (UseEntityFramework)
			{
				var server = Servers.FirstOrDefault(s => s.ServerId == serverId);
				if (server == null) return;

				server.ServerName = serverName;
				server.ServerUrl = serverUrl;
				server.Password = password;
				server.Comments = comments;
				server.InstantDomainAlias = instantDomainAlias;
				server.PrimaryGroupId = primaryGroupId != 0 ? primaryGroupId : null;
				server.ADEnabled = adEnabled;
				server.ADRootDomain = adRootDomain;
				server.ADUsername = adUsername;
				server.ADPassword = adPassword;
				server.ADAuthenticationType = adAuthenticationType;
				server.ADParentDomain = adParentDomain;
				server.ADParentDomainController = adParentDomainController;
				server.OSPlatform = osPlatform;
				server.IsCore = isCore;
				server.PasswordIsSHA256 = PasswordIsSHA256;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdateServer",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerName", serverName),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerUrl", serverUrl),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Password", password),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Comments", comments),
					 /// <summary>TODO</summary>
					 new SqlParameter("@InstantDomainAlias", instantDomainAlias),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PrimaryGroupId", primaryGroupId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdEnabled", adEnabled),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdRootDomain", adRootDomain),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdUsername", adUsername),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdPassword", adPassword),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdAuthenticationType", adAuthenticationType),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdParentDomain", adParentDomain),
					 /// <summary>TODO</summary>
					 new SqlParameter("@AdParentDomainController", adParentDomainController),
					 /// <summary>TODO</summary>
					 new SqlParameter("@OSPlatform", osPlatform),
					 /// <summary>TODO</summary>
					 new SqlParameter("@IsCore", isCore),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PasswordIsSHA256", PasswordIsSHA256));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteServer(int serverId)
		{
			if (UseEntityFramework)
			{
				// check related services
				if (Services.Any(svc => svc.ServerId == serverId)) return -1;

				// check related packages
				if (Packages.Any(p => p.ServerId == serverId)) return -2;

				// check related hosting plans
				if (HostingPlans.Any(p => p.ServerId == serverId)) return -3;

				using (var transaction = Database.BeginTransaction())
				{
					// delete IP addresses
					IpAddresses.Where(ip => ip.ServerId == serverId).ExecuteDelete();

					// delete global DNS records
					GlobalDnsRecords.Where(r => r.ServerId == serverId).ExecuteDelete();

					// delete server
					Servers.Where(s => s.ServerId == serverId).ExecuteDelete();

					// delete virtual services if any
					VirtualServices.Where(vs => vs.ServerId == serverId).ExecuteDelete();

					transaction.Commit();
				}
				return 0;
			}
			else
			{
				SqlParameter prmResult = new SqlParameter("@Result", SqlDbType.Int);
				prmResult.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "DeleteServer",
					 prmResult,
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId));

				return Convert.ToInt32(prmResult.Value);
			}
		}
		#endregion

		#region Virtual Servers
		/// <summary>Auto-generated member.</summary>
		public DataSet GetVirtualServers(int actorId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var servers = Servers
					.Where(s => isAdmin && s.VirtualServer)
					.OrderBy(s => s.ServerName)
					.Select(s => new
					{
						s.ServerId,
						s.ServerName,
						s.ServerUrl,
						ServicesNumber = VirtualServices.Where(v => v.ServerId == s.ServerId).Count(),
						s.Comments,
						s.PrimaryGroupId
					});

				/// <summary>TODO</summary>
				return EntityDataSet(servers);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetVirtualServers",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetAvailableVirtualServices(int actorId, int serverId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var servers = Servers
					.Where(s => isAdmin && !s.VirtualServer) // get only physical servers
					.Select(s => new
					{
						s.ServerId,
						s.ServerName,
						s.Comments
					});

				var virtualServiceIds = VirtualServices
					.Where(v => v.ServerId == serverId)
					.Select(v => v.ServiceId)
					.ToList();
				var services = Services
					.Where(s => isAdmin && !virtualServiceIds.Any(id => id == s.ServiceId))
					.Select(s => new
					{
						s.ServiceId,
						s.ServerId,
						s.ProviderId,
						s.ServiceName,
						s.Comments
					});

				var set = new DataSet();
				set.Tables.Add(EntityDataTable(servers));
				set.Tables.Add(EntityDataTable(services));

				/// <summary>TODO</summary>
				return set;
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetAvailableVirtualServices",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetVirtualServices(int actorId, int serverId, bool forAutodiscover)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				// virtual groups
				var virtGroups = ResourceGroups
					.Where(g => (isAdmin || forAutodiscover) && g.ShowGroup == true)
					.SelectMany(g => g.VirtualGroups
						.Where(vg => vg.ServerId == serverId)
						.DefaultIfEmpty(),
						(g, vg) => new
						{
							VirtualGroupId = vg != null ? (int?)vg.VirtualGroupId : null,
							g.GroupId,
							g.GroupName,
							g.GroupOrder,
							DistributionType = (vg != null ? vg.DistributionType : null) ?? 1,
							BindDistributionToPrimary = (vg != null ? vg.BindDistributionToPrimary : null) ?? true
						})
					.OrderBy(g => g.GroupOrder)
					.Select(g => new
					{
						g.VirtualGroupId,
						g.GroupId,
						g.GroupName,
						g.DistributionType,
						g.BindDistributionToPrimary
					});

				var services = VirtualServices
					.Where(vs => vs.ServerId == serverId && (isAdmin || forAutodiscover))
					.Select(s => new
					{
						s.ServiceId,
						s.Service.ServiceName,
						s.Service.Comments,
						s.Service.Provider.GroupId,
						s.Service.Provider.DisplayName,
						s.Server.ServerName
					});
				/// <summary>TODO</summary>
				return EntityDataSet(virtGroups, services);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetVirtualServices",
					 /// <summary>TODO</summary>
					 new SqlParameter("@actorId", actorId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@forAutodiscover", forAutodiscover));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddVirtualServices(int serverId, string xml)
		{
			if (UseEntityFramework)
			{
				/* XML Format:
				<services>
					<service id="16" />
				</services> */

				var services = XElement.Parse(xml);

				var existingServices = VirtualServices
					.Where(v => v.ServerId == serverId)
					.Select(v => v.ServiceId)
					.ToHashSet();

				bool addedAny = false;
				foreach (var service in services.Elements())
				{
					var serviceId = (int)service.Attribute("id");
					if (!existingServices.Contains(serviceId))
					{
						var virtualService = new Data.Entities.VirtualService()
						{
							ServerId = serverId,
							ServiceId = serviceId
						};
						VirtualServices.Add(virtualService);
						addedAny = true;
					}
				}

				if (addedAny) SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "AddVirtualServices",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@xml", xml));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteVirtualServices(int serverId, string xml)
		{
			if (UseEntityFramework)
			{
				/* XML Format:
				<services>
					<service id=""16"" />
				</services> */

				var services = XElement.Parse(xml);
				var serviceIds = services.Elements()
					.Select(service => (int)service.Attribute("id"))
					.ToList();
				var toDelete = VirtualServices
					.Where(vs => vs.ServerId == serverId)
					.AsEnumerable()
					.Join(serviceIds, vs => vs.ServiceId, sid => sid, (vs, sid) => vs);
				VirtualServices.RemoveRange(toDelete);

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "DeleteVirtualServices",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@xml", xml));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateVirtualGroups(int serverId, string xml)
		{
			if (UseEntityFramework)
			{
				/* XML Format:
				<groups>
					<group id="16" distributionType="1" bindDistributionToPrimary="1"/>
				</groups> */

				var groupsXml = XElement.Parse(xml);
				var groups = groupsXml
					.Elements()
					.Select(group => new Data.Entities.VirtualGroup
					{
						ServerId = serverId,
						GroupId = (int)group.Attribute("id"),
						DistributionType = (int?)group.Attribute("distributionType"),
						BindDistributionToPrimary = ((int?)group.Attribute("bindDistributionToPrimary") ?? 1) == 1
					});

				// delete existing groups
				VirtualGroups.RemoveRange(VirtualGroups.Where(vg => vg.ServerId == serverId));

				VirtualGroups.AddRange(groups);

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdateVirtualGroups",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@xml", xml));
			}
		}
		#endregion

		#region Providers

		// Providers methods
		/// <summary>Auto-generated member.</summary>

		public DataSet GetProviders()
		{
			if (UseEntityFramework)
			{
				// TODO ProviderName was duplicate in the Stored Procedure. Resolve correctly. I've changed
				// the duplicate ProviderName to ProviderGroupedName
				var providers = Providers
					.Join(ResourceGroups, p => p.GroupId, rg => rg.GroupId, (p, rg) => new
					{
						Provider = p,
						ResourceGroup = rg
					})
					.OrderBy(j => j.ResourceGroup.GroupOrder)
					.ThenBy(j => j.Provider.DisplayName)
					.Select(j => new
					{
						j.Provider.ProviderId,
						j.Provider.GroupId,
						j.Provider.ProviderName,
						j.Provider.EditorControl,
						j.Provider.DisplayName,
						j.Provider.ProviderType,
						ProviderGroupedName = j.ResourceGroup.GroupName + " - " + j.Provider.DisplayName,
						j.Provider.DisableAutoDiscovery
					});

				/// <summary>TODO</summary>
				return EntityDataSet(providers);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetProviders");
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetGroupProviders(int groupId)
		{
			if (UseEntityFramework)
			{
				// TODO ProviderName was duplicate in the Stored Procedure. Resolve correctly. I've changed
				// the duplicate ProviderName to ProviderGroupedName
				var providers = Providers
					.Where(p => p.GroupId == groupId)
					.Join(ResourceGroups, p => p.GroupId, rg => rg.GroupId, (p, rg) => new
					{
						Provider = p,
						ResourceGroup = rg
					})
					.OrderBy(j => j.ResourceGroup.GroupOrder)
					.ThenBy(j => j.Provider.DisplayName)
					.Select(j => new
					{
						j.Provider.ProviderId,
						j.Provider.GroupId,
						j.Provider.ProviderName,
						j.Provider.DisplayName,
						j.Provider.ProviderType,
						ProviderGroupedName = j.ResourceGroup.GroupName + " - " + j.Provider.DisplayName,
					});

				/// <summary>TODO</summary>
				return EntityDataSet(providers);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetGroupProviders",
					 /// <summary>TODO</summary>
					 new SqlParameter("@groupId", groupId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetProvider(int providerId)
		{
			if (UseEntityFramework)
			{
				var provider = Providers
					.Where(p => p.ProviderId == providerId)
					.Select(p => new
					{
						p.ProviderId,
						p.GroupId,
						p.ProviderName,
						p.EditorControl,
						p.DisplayName,
						p.ProviderType
					});

				/// <summary>TODO</summary>
				return EntityDataReader(provider);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetProvider",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ProviderID", providerId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetProviderByServiceID(int serviceId)
		{
			if (UseEntityFramework)
			{
				var provider = Services
					.Where(s => s.ServiceId == serviceId)
					.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => new
					{
						p.ProviderId,
						p.GroupId,
						p.DisplayName,
						p.EditorControl,
						p.ProviderType
					});

				/// <summary>TODO</summary>
				return EntityDataReader(provider);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetProviderByServiceID",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServiceID", serviceId));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public bool GetQuotaHidden(string quotaName, int groupID)
		{
			if (UseEntityFramework)
			{
				return Quotas
					.Where(quota => quota.QuotaName == quotaName && quota.GroupId == groupID)
					.Select(quota => quota.HideQuota)
					.FirstOrDefault() ?? false;
			}
			else
			{
				SqlParameter prmHideQuota = new SqlParameter("@HideQuota", SqlDbType.Bit);
				prmHideQuota.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetQuotaHidden",
					/// <summary>TODO</summary>
					new SqlParameter("@QuotaName", quotaName),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupID", groupID),
					prmHideQuota);

				return (prmHideQuota.Value as bool?) == true;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int UpdateQuotaHidden(string quotaName, int groupID, bool hideQuota)
		{
			if (UseEntityFramework)
			{
				return Quotas.Where(quota => quota.QuotaName == quotaName && quota.GroupId == groupID)
					.ExecuteUpdate(quota => new Data.Entities.Quota { HideQuota = hideQuota });
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateQuotaHidden",
					/// <summary>TODO</summary>
					new SqlParameter("@QuotaName", quotaName),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupID", groupID),
					/// <summary>TODO</summary>
					new SqlParameter("@HideQuota", hideQuota.ToString()));
			}
		}
		#endregion

		#region Private Network VLANs
		/// <summary>Auto-generated member.</summary>
		public int AddPrivateNetworkVLAN(int serverId, int vlan, string comments)
		{
			if (UseEntityFramework)
			{
				var vlanEntity = new Data.Entities.PrivateNetworkVlan()
				{
					ServerId = serverId != 0 ? serverId : null,
					Vlan = vlan,
					Comments = comments
				};
				PrivateNetworkVlans.Add(vlanEntity);

				SaveChanges();

				return vlanEntity.VlanId;
			}
			else
			{
				SqlParameter prmAddresId = new SqlParameter("@VlanID", SqlDbType.Int);
				prmAddresId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "AddPrivateNetworkVlan",
					 prmAddresId,
					 /// <summary>TODO</summary>
					 new SqlParameter("@Vlan", vlan),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Comments", comments));

				return Convert.ToInt32(prmAddresId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeletePrivateNetworkVLAN(int vlanId)
		{
			if (UseEntityFramework)
			{
				if (PackageVlans.Any(vlan => vlan.VlanId == vlanId)) return -2;

				PrivateNetworkVlans.Where(vlan => vlan.VlanId == vlanId).ExecuteDelete();

				return 0;
			}
			else
			{
				SqlParameter prmResult = new SqlParameter("@Result", SqlDbType.Int);
				prmResult.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "DeletePrivateNetworkVLAN",
					 prmResult,
					 /// <summary>TODO</summary>
					 new SqlParameter("@VlanID", vlanId));

				return Convert.ToInt32(prmResult.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPrivateNetworkVLANsPaged(int actorId, int serverId,
			 string filterColumn, string filterValue,
			 string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var vlans = PrivateNetworkVlans
					.Where(vlan => isAdmin && (serverId == 0 || serverId != 0 && serverId == vlan.ServerId))
					.Join(Servers, v => v.ServerId, s => s.ServerId, (v, s) => new
					{
						Vlan = v,
						Server = s
					})
					.Join(PackageVlans, j => j.Vlan.VlanId, pv => pv.VlanId, (j, pv) => new
					{
						j.Vlan,
						j.Server,
						PackageVlan = pv
					})
					.Join(Packages, j => j.PackageVlan.PackageId, p => p.PackageId, (j, p) => new
					{
						j.Vlan,
						j.Server,
						j.PackageVlan,
						Package = p
					})
					.Join(Users, j => j.Package.UserId, u => u.UserId, (j, u) => new
					{
						j.Vlan.VlanId,
						j.Vlan.Vlan,
						j.Vlan.Comments,
						j.Vlan.ServerId,
						j.Server.ServerName,
						j.PackageVlan.PackageId,
						j.Package.PackageName,
						j.Package.UserId,
						u.Username
					});

				if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
				{
					vlans = vlans.Where(DynamicFunctions.ColumnLike(vlans, filterColumn, filterValue));
				}
				else if (!string.IsNullOrEmpty(filterValue))
				{
						vlans = vlans.Where(v => v.Vlan.ToString() == filterValue || v.ServerName == filterValue ||
							v.Username == filterValue);
				}

				if (string.IsNullOrEmpty(sortColumn)) sortColumn = "Vlan";

				var count = vlans.Count();

				vlans = vlans.OrderBy(ColumnName(sortColumn));

				vlans = vlans.Skip(startRow).Take(maximumRows);

				/// <summary>TODO</summary>
				return EntityDataReader(count, vlans);
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
												 "GetPrivateNetworVLANsPaged",
													 /// <summary>TODO</summary>
													 new SqlParameter("@ActorId", actorId),
													 /// <summary>TODO</summary>
													 new SqlParameter("@ServerId", serverId),
													 /// <summary>TODO</summary>
													 new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
													 /// <summary>TODO</summary>
													 new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
													 /// <summary>TODO</summary>
													 new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
													 /// <summary>TODO</summary>
													 new SqlParameter("@startRow", startRow),
													 /// <summary>TODO</summary>
													 new SqlParameter("@maximumRows", maximumRows));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPrivateNetworVLAN(int vlanId)
		{
			if (UseEntityFramework)
			{
				var vlan = PrivateNetworkVlans
					.Where(v => v.VlanId == vlanId);

				/// <summary>TODO</summary>
				return EntityDataReader(vlan);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "GetPrivateNetworVLAN",
					 /// <summary>TODO</summary>
					 new SqlParameter("@VlanID", vlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdatePrivateNetworVLAN(int vlanId, int serverId, int vlan, string comments)
		{
			if (UseEntityFramework)
			{
				var vl = PrivateNetworkVlans.FirstOrDefault(v => v.VlanId == vlanId);
				if (vl == null) return;

				vl.ServerId = serverId != 0 ? serverId : null;
				vl.Vlan = vlan;
				vl.Comments = comments;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "UpdatePrivateNetworVLAN",
					 /// <summary>TODO</summary>
					 new SqlParameter("@VlanID", vlanId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Vlan", vlan),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Comments", comments));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckPackageParent(int parentPackageId, int packageId)
		{
			// check if the user requests hiself
			if (parentPackageId == packageId) return true;

			var id = packageId;
			var package = Packages
				.Select(p => new { p.PackageId, p.ParentPackageId })
				.FirstOrDefault(p => p.PackageId == id);
			while (package != null && package.ParentPackageId.HasValue && package.ParentPackageId != parentPackageId)
			{
				id = package.ParentPackageId.Value;
				package = Packages
					.Select(p => new { p.PackageId, p.ParentPackageId })
					.FirstOrDefault(p => p.PackageId == id);
			}
			return package != null && package.ParentPackageId == parentPackageId;
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackagePrivateNetworkVLANs(int packageId, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				using (var packages = PackagesTree(packageId, true))
				{
					var vlans = PackageVlans
						//.Where(pv => CheckPackageParent(packageId, pv.PackageId) && !pv.IsDmz)
						.Where(pv => !pv.IsDmz)
						.Join(packages, pv => pv.PackageId, p => p, (pv, p) => pv)
						.Join(PrivateNetworkVlans, p => p.VlanId, v => v.VlanId, (pv, vl) => new
						{
							PackageVlan = pv,
							Vlan = vl
						})
						.Join(Packages, j => j.PackageVlan.PackageId, p => p.PackageId, (j, p) => new
						{
							j.PackageVlan,
							j.Vlan,
							Package = p
						})
						.Join(Users, j => j.Package.UserId, u => u.UserId, (j, u) => new
						{
							j.PackageVlan.PackageVlanId,
							j.PackageVlan.VlanId,
							j.Vlan.Vlan,
							j.PackageVlan.PackageId,
							j.Package.PackageName,
							j.Package.UserId,
							u.Username
						});

					var count = vlans.Count();

					vlans = !string.IsNullOrEmpty(sortColumn) ? vlans.OrderBy(ColumnName(sortColumn)) : vlans.OrderBy(v => v.Vlan);


					vlans = vlans.Skip(startRow).Take(maximumRows);

					/// <summary>TODO</summary>
					return EntityDataReader(count, vlans);
				}
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
												 "GetPackagePrivateNetworkVLANs",
													 /// <summary>TODO</summary>
													 new SqlParameter("@PackageID", packageId),
													 /// <summary>TODO</summary>
													 new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
													 /// <summary>TODO</summary>
													 new SqlParameter("@startRow", startRow),
													 /// <summary>TODO</summary>
													 new SqlParameter("@maximumRows", maximumRows));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeallocatePackageVLAN(int id)
		{
			if (UseEntityFramework)
			{
				var packageVlan = PackageVlans
					.Where(pv => pv.PackageVlanId == id)
					.Include(pv => pv.Package)
					.Single();

				if (packageVlan.Package.ParentPackageId == 1) // System space
				{
					PackageVlans.Where(pv => pv.PackageVlanId == id).ExecuteDelete();
				}
				else // 2nd level space and below
				{
					packageVlan.PackageId = id;

					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure, "DeallocatePackageVLAN",
												  /// <summary>TODO</summary>
												  new SqlParameter("@PackageVlanID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetUnallottedVLANs(int packageId, int serviceId)
		{
			if (UseEntityFramework)
			{
				int parentPackageId = -1;
				int serverId = -1;

				if (packageId == -1)
				{ // No PackageID defined, use ServerID from ServiceID (VPS Import)
					serverId = Services
						.Where(service => service.ServiceId == serviceId)
						.Select(service => service.ServerId)
						.FirstOrDefault();
					parentPackageId = 1;
				}
				else
				{
					var package = Packages
						.Where(p => p.PackageId == packageId)
						.Select(p => new { p.ServerId, p.ParentPackageId })
						.FirstOrDefault();
					if (package != null)
					{
						parentPackageId = package.ParentPackageId ?? 1;
						serverId = package.ServerId ?? -1;
					}
				}

				if (parentPackageId == 1) // "System" space
				{
					// check if server is physical
					if (Servers.Any(s => s.ServerId == serverId && !s.VirtualServer))
					{
						// physical server
						var vlans = PrivateNetworkVlans
							.SelectMany(v => v.PackageVlans.DefaultIfEmpty(), (v, ps) => new
							{
								Vlan = v,
								HasPackage = ps != null,
							})
							.Where(v => !v.HasPackage && (v.Vlan.ServerId == serverId || v.Vlan.ServerId == null))
							.Select(v => new
							{
								v.Vlan.VlanId,
								v.Vlan.Vlan,
								v.Vlan.ServerId
							});
						/// <summary>TODO</summary>
						return EntityDataReader(vlans);
					}
					else
					{ // virtual server, get resource group by service
						var groupId = Services
							.Where(s => s.ServiceId == serviceId)
							.Select(s => s.Provider.GroupId)
							.FirstOrDefault();
						var vlans = PrivateNetworkVlans
							.SelectMany(v => v.Server.Services, (v, s) => new
							{
								Vlan = v,
								Service = s,
								s.Provider
							})
							.Where(v => (v.Service.ServiceId == serviceId && v.Provider.GroupId == groupId) ||
								v.Vlan.ServerId == null)
							.SelectMany(v => v.Vlan.PackageVlans.DefaultIfEmpty(), (v, ps) => new
							{
								v.Vlan,
								HasPackages = ps != null,
							})
							.Where(v => !v.HasPackages)
							.Select(v => new
							{
								v.Vlan.VlanId,
								v.Vlan.Vlan,
								v.Vlan.ServerId,
							})
							.OrderByDescending(v => v.ServerId)
							.ThenBy(v => v.Vlan);
						/// <summary>TODO</summary>
						return EntityDataReader(vlans);
					}
				}
				else
				{ // 2nd level space and below, get service location
					serverId = Services
						.Where(s => s.ServiceId == serviceId)
						.Select(s => s.ServerId)
						.Single();
					var vlans = PrivateNetworkVlans
						.SelectMany(v => v.PackageVlans, (v, pv) => new
						{
							Vlan = v,
							Package = pv,
						})
						.Where(g => g.Package.PackageId == packageId &&
							(g.Vlan.ServerId == serverId || g.Vlan.ServerId == null))
						.Select(g => new
						{
							g.Vlan.ServerId,
							g.Vlan.Vlan,
							g.Vlan.VlanId
						})
						.OrderByDescending(g => g.ServerId)
						.ThenBy(g => g.Vlan);
					/// <summary>TODO</summary>
					return EntityDataReader(vlans);
				}
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
												 "GetUnallottedVLANs",
													 /// <summary>TODO</summary>
													 new SqlParameter("@PackageId", packageId),
													 /// <summary>TODO</summary>
													 new SqlParameter("@ServiceId", serviceId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AllocatePackageVLANs(int packageId, bool isDmz, string xml)
		{
			if (UseEntityFramework)
			{
				var items = XElement.Parse(xml);
				var idsEnumerable = items
					.Elements()
					.Select(e => (int)e.Attribute("id"))
					.ToList();
				using (var ids = idsEnumerable
					.ToTempIdSet(this))
				{
					// delete
					var toDelete = PackageVlans
						.Join(ids, p => p.VlanId, id => id, (p, id) => p);
					PackageVlans.RemoveRange(toDelete);

					// insert
					PackageVlans.AddRange(
						idsEnumerable.Select(id => new Data.Entities.PackageVlan
						{
							PackageId = packageId,
							VlanId = id,
							IsDmz = isDmz
						}));

					SaveChanges();
				}
			}
			else
			{
				SqlParameter[] param = new[] {
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDmz", isDmz),
					/// <summary>TODO</summary>
					new SqlParameter("@xml", xml)
				};

				ExecuteLongNonQuery("AllocatePackageVLANs", param);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackageDmzNetworkVLANs(int packageId, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				using (var packages = PackagesTree(packageId, true))
				{
					var vlans = PackageVlans
						//.Where(pv => CheckPackageParent(packageId, pv.PackageId) && pv.IsDmz)
						.Where(pv => pv.IsDmz)
						.Join(packages, pv => pv.PackageId, p => p, (pv, p) => pv)
						.Join(PrivateNetworkVlans, p => p.VlanId, v => v.VlanId, (pv, vl) => new
						{
							PackageVlan = pv,
							Vlan = vl
						})
						.Join(Packages, j => j.PackageVlan.PackageId, p => p.PackageId, (j, p) => new
						{
							j.PackageVlan,
							j.Vlan,
							Package = p
						})
						.Join(Users, j => j.Package.UserId, u => u.UserId, (j, u) => new
						{
							j.PackageVlan.PackageVlanId,
							j.PackageVlan.VlanId,
							j.Vlan.Vlan,
							j.PackageVlan.PackageId,
							j.Package.PackageName,
							j.Package.UserId,
							u.Username
						});

					var count = vlans.Count();

					vlans = !string.IsNullOrEmpty(sortColumn) ? vlans.OrderBy(ColumnName(sortColumn)) : vlans.OrderBy(v => v.Vlan);


					vlans = vlans.Skip(startRow).Take(maximumRows);

					/// <summary>TODO</summary>
					return EntityDataReader(count, vlans);
				}
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
										 "GetPackageDmzNetworkVLANs",
											/// <summary>TODO</summary>
											new SqlParameter("@PackageID", packageId),
											/// <summary>TODO</summary>
											new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
											/// <summary>TODO</summary>
											new SqlParameter("@startRow", startRow),
											/// <summary>TODO</summary>
											new SqlParameter("@maximumRows", maximumRows));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		#endregion

		#region IPAddresses
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetIPAddress(int ipAddressId)
		{
			if (UseEntityFramework)
			{
				var address = IpAddresses
					.Where(ip => ip.AddressId == ipAddressId)
					.Select(ip => new
					{
						ip.AddressId,
						ip.ServerId,
						ip.ExternalIp,
						ip.InternalIp,
						ip.PoolId,
						ip.SubnetMask,
						ip.DefaultGateway,
						ip.Comments,
						ip.Vlan
					});
				/// <summary>TODO</summary>
				return EntityDataReader(address);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@AddressID", ipAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetIPAddresses(int actorId, int poolId, int serverId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

                var addresses = IpAddresses
                    .Where(ip => isAdmin &&
						(poolId == 0 || poolId == ip.PoolId) &&
                        (serverId == 0 || serverId == ip.ServerId))
                    .GroupJoin(Servers, ip => ip.ServerId, s => s.ServerId, (ip, ss) => new { Ip = ip, Servers = ss })
                    .SelectMany(ip => ip.Servers.DefaultIfEmpty(), (ip, ss) => new { ip.Ip, Server = ss })
                    .GroupJoin(PackageIpAddresses, g => g.Ip.AddressId, p => p.AddressId, (g, ps) => new
                    {
                        g.Ip,
                        g.Server,
                        PackageIps = ps
                    })
                    .SelectMany(ip => ip.PackageIps.DefaultIfEmpty(), (ip, p) => new
                    {
                        ip.Ip,
                        ip.Server,
                        PackageIp = p
                    })
                    .GroupJoin(ServiceItems, g => g.PackageIp != null ? g.PackageIp.ItemId : null, s => (int?)s.ItemId, (g, ss) => new
                    {
                        g.Ip,
                        g.Server,
                        g.PackageIp,
                        Items = ss
                    })
                    .SelectMany(ip => ip.Items.DefaultIfEmpty(), (ip, s) => new
                    {
                        ip.Ip,
                        ip.Server,
                        ip.PackageIp,
                        Item = s
                    })
                    .GroupJoin(Packages, g => g.PackageIp != null ? (int?)g.PackageIp.PackageId : null, p => (int?)p.PackageId, (g, ps) => new
                    {
                        g.Ip,
                        g.Server,
                        g.PackageIp,
                        g.Item,
                        Packages = ps
                    })
                    .SelectMany(ip => ip.Packages.DefaultIfEmpty(), (ip, p) => new
                    {
                        ip.Ip,
                        ip.Server,
                        ip.PackageIp,
                        ip.Item,
                        Package = p
                    })
                    .GroupJoin(Users, g => g.Package.UserId, u => u.UserId, (g, us) => new
                    {
                        g.Ip,
                        g.Server,
                        g.PackageIp,
                        g.Item,
                        g.Package,
                        Users = us
                    })
                    .SelectMany(g => g.Users.DefaultIfEmpty(), (g, u) => new
                    {
                        g.Ip.AddressId,
                        g.Ip.PoolId,
                        g.Ip.ExternalIp,
                        g.Ip.InternalIp,
                        g.Ip.SubnetMask,
                        g.Ip.DefaultGateway,
                        g.Ip.Comments,
                        g.Ip.ServerId,
                        ServerName = g.Server != null ? g.Server.ServerName : null,
                        ItemId = g.PackageIp != null ? g.PackageIp.ItemId : null,
                        ItemName = g.Item != null ? g.Item.ItemName : null,
                        PackageId = g.PackageIp != null ? (int?)g.PackageIp.PackageId : null,
                        PackageName = g.Package != null ? g.Package.PackageName : null,
                        UserId = g.Package != null ? (int?)g.Package.UserId : null,
                        Username = u != null ? u.Username : null
                    });

                /// <summary>TODO</summary>
                return EntityDataReader(addresses);
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PoolId", poolId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", serverId));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetIPAddressesPaged(int actorId, int poolId, int serverId,
			string filterColumn, string filterValue,
			string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

                var addresses = IpAddresses
                    .Where(ip => isAdmin &&
						(poolId == 0 || poolId == ip.PoolId) &&
                        (serverId == 0 || serverId == ip.ServerId))
                    .GroupJoin(Servers, ip => ip.ServerId, s => s.ServerId, (ip, ss) => new { Ip = ip, Servers = ss })
                    .SelectMany(ip => ip.Servers.DefaultIfEmpty(), (ip, ss) => new
                    {
                        ip.Ip,
                        Server = ss
                    })
                    .GroupJoin(PackageIpAddresses, g => g.Ip.AddressId, p => p.AddressId, (g, ps) => new
                    {
                        g.Ip,
                        g.Server,
                        PackageIps = ps
                    })
                    .SelectMany(ip => ip.PackageIps.DefaultIfEmpty(), (ip, ps) => new
                    {
                        ip.Ip,
                        ip.Server,
                        PackageIp = ps
                    })
                    .GroupJoin(ServiceItems, g => g.PackageIp != null ? g.PackageIp.ItemId : null, s => s.ItemId, (g, ss) => new
                    {
                        g.Ip,
                        g.Server,
                        g.PackageIp,
                        Items = ss
                    })
                    .SelectMany(ip => ip.Items.DefaultIfEmpty(), (ip, s) => new
                    {
                        ip.Ip,
                        ip.Server,
                        ip.PackageIp,
                        Item = s
                    })
                    .GroupJoin(Packages, g => g.PackageIp != null ? g.PackageIp.PackageId : -1, p => p.PackageId, (g, ps) => new
                    {
                        g.Ip,
                        g.Server,
                        g.PackageIp,
                        g.Item,
                        Packages = ps
                    })
                    .SelectMany(g => g.Packages.DefaultIfEmpty(), (g, ps) => new
                    {
                        g.Ip,
                        g.Server,
                        g.PackageIp,
                        g.Item,
                        Package = ps
                    })
                    .GroupJoin(Users, g => g.Package != null ? g.Package.UserId : -1, u => u.UserId, (g, u) => new
                    {
                        g.Ip,
                        g.Server,
                        g.PackageIp,
                        g.Item,
                        g.Package,
                        Users = u
                    })
                    .SelectMany(g => g.Users.DefaultIfEmpty(), (g, u) => new
                    {
                        g.Ip.AddressId,
                        g.Ip.PoolId,
                        g.Ip.ExternalIp,
                        g.Ip.InternalIp,
                        g.Ip.SubnetMask,
                        g.Ip.DefaultGateway,
                        g.Ip.Comments,
                        g.Ip.ServerId,
                        ServerName = g.Server != null ? g.Server.ServerName : null,
                        ItemId = g.PackageIp != null ? g.PackageIp.ItemId : null,
                        ItemName = g.Item != null ? g.Item.ItemName : null,
                        PackageId = g.PackageIp != null ? (int?)g.PackageIp.PackageId : null,
                        PackageName = g.Package != null ? g.Package.PackageName : null,
                        UserId = g.Package != null ? g.Package.UserId : 0,
                        Username = u != null ? u.Username : null
                    });

                if (!string.IsNullOrEmpty(filterValue))
				{
					if (!string.IsNullOrEmpty(filterColumn))
					{
						addresses = addresses.Where(DynamicFunctions.ColumnLike(addresses, filterColumn, filterValue));
					}
					else
					{
#if NETFRAMEWORK
						addresses = addresses.Where(a => DbFunctions.Like(a.ExternalIp, filterValue) ||
							DbFunctions.Like(a.InternalIp, filterValue) ||
							DbFunctions.Like(a.DefaultGateway, filterValue) ||
							DbFunctions.Like(a.ServerName, filterValue) ||
							DbFunctions.Like(a.ItemName, filterValue) ||
							DbFunctions.Like(a.Username, filterValue));
#else
						addresses = addresses.Where(a => EF.Functions.Like(a.ExternalIp, filterValue) ||
							EF.Functions.Like(a.InternalIp, filterValue) ||
							EF.Functions.Like(a.DefaultGateway, filterValue) ||
							EF.Functions.Like(a.ServerName, filterValue) ||
							EF.Functions.Like(a.ItemName, filterValue) ||
							EF.Functions.Like(a.Username, filterValue));
#endif
					}
				}

				var count = addresses.Count();

				addresses = string.IsNullOrEmpty(sortColumn) ? addresses.OrderBy(a => a.ExternalIp) : addresses.OrderBy(ColumnName(sortColumn));


				addresses = addresses.Skip(startRow).Take(maximumRows);

				/// <summary>TODO</summary>
				return EntityDataReader(count, addresses);
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetIPAddressesPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PoolId", poolId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddIPAddress(int poolId, int serverId, string externalIP, string internalIP,
			 string subnetMask, string defaultGateway, string comments, int VLAN)
		{
			if (UseEntityFramework)
			{
				var address = new Data.Entities.IpAddress()
				{
					PoolId = poolId,
					ServerId = serverId != 0 ? serverId : null,
					ExternalIp = externalIP,
					InternalIp = internalIP,
					SubnetMask = subnetMask,
					DefaultGateway = defaultGateway,
					Comments = comments,
					Vlan = VLAN
				};

				IpAddresses.Add(address);

				SaveChanges();

				return address.AddressId;
			}
			else
			{
				SqlParameter prmAddresId = new SqlParameter("@AddressID", SqlDbType.Int);
				prmAddresId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "AddIPAddress",
					 prmAddresId,
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@externalIP", externalIP),
					 /// <summary>TODO</summary>
					 new SqlParameter("@internalIP", internalIP),
					 /// <summary>TODO</summary>
					 new SqlParameter("@PoolId", poolId),
					 /// <summary>TODO</summary>
					 new SqlParameter("@SubnetMask", subnetMask),
					 /// <summary>TODO</summary>
					 new SqlParameter("@DefaultGateway", defaultGateway),
					 /// <summary>TODO</summary>
					 new SqlParameter("@Comments", comments),
					 /// <summary>TODO</summary>
					 new SqlParameter("@VLAN", VLAN));

				return Convert.ToInt32(prmAddresId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateIPAddress(int addressId, int poolId, int serverId,
			 string externalIP, string internalIP, string subnetMask, string defaultGateway, string comments, int VLAN)
		{
			if (UseEntityFramework)
			{
				var address = IpAddresses.FirstOrDefault(ip => ip.AddressId == addressId);

				if (address != null)
				{
					address.PoolId = poolId;
					address.ServerId = serverId > 0 ? serverId : null;
					address.ExternalIp = externalIP;
					address.InternalIp = internalIP;
					address.SubnetMask = subnetMask;
					address.DefaultGateway = defaultGateway;
					address.Comments = comments;
					address.Vlan = VLAN;

					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "UpdateIPAddress",
				 /// <summary>TODO</summary>
				 new SqlParameter("@AddressID", addressId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@externalIP", externalIP),
				 /// <summary>TODO</summary>
				 new SqlParameter("@internalIP", internalIP),
				 /// <summary>TODO</summary>
				 new SqlParameter("@ServerID", serverId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@PoolId", poolId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@SubnetMask", subnetMask),
				 /// <summary>TODO</summary>
				 new SqlParameter("@DefaultGateway", defaultGateway),
				 /// <summary>TODO</summary>
				 new SqlParameter("@Comments", comments),
				 /// <summary>TODO</summary>
				 new SqlParameter("@VLAN", VLAN));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateIPAddresses(string xmlIds, int poolId, int serverId,
			 string subnetMask, string defaultGateway, string comments, int VLAN)
		{
			if (UseEntityFramework)
			{
                var items = XElement.Parse(xmlIds);
                using (var addressIds = items
                    .Elements()
                    .Select(e => (int)e.Attribute("id"))
                    .ToTempIdSet(this))
                {
                    IpAddresses
                        .Join(addressIds, ip => ip.AddressId, id => id, (ip, id) => ip)
                        .ExecuteUpdate(ip => new Data.Entities.IpAddress()
                        {
                            PoolId = poolId,
                            ServerId = serverId,
                            SubnetMask = subnetMask,
                            DefaultGateway = defaultGateway,
                            Comments = comments,
                        });
                }
            }
            else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "UpdateIPAddresses",
				 /// <summary>TODO</summary>
				 new SqlParameter("@Xml", xmlIds),
				 /// <summary>TODO</summary>
				 new SqlParameter("@ServerID", serverId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@PoolId", poolId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@SubnetMask", subnetMask),
				 /// <summary>TODO</summary>
				 new SqlParameter("@DefaultGateway", defaultGateway),
				 /// <summary>TODO</summary>
				 new SqlParameter("@Comments", comments),
				 /// <summary>TODO</summary>
				 new SqlParameter("@VLAN", VLAN));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteIPAddress(int ipAddressId)
		{
			if (UseEntityFramework)
			{
				if (GlobalDnsRecords.Any(r => r.IpAddressId == ipAddressId)) return -1;
				if (PackageIpAddresses.Any(p => p.AddressId == ipAddressId && p.ItemId != null)) return -2;

				using (var transaction = Database.BeginTransaction())
				{
					// delete package-IP relation
					PackageIpAddresses.Where(p => p.AddressId == ipAddressId).ExecuteDelete();

					// delete IP address
					IpAddresses.Where(a => a.AddressId == ipAddressId).ExecuteDelete();

					transaction.Commit();
				}

				return 0;
			}
			else
			{
				SqlParameter prmResult = new SqlParameter("@Result", SqlDbType.Int);
				prmResult.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "DeleteIPAddress",
					 prmResult,
					 /// <summary>TODO</summary>
					 new SqlParameter("@AddressID", ipAddressId));

				return Convert.ToInt32(prmResult.Value);
			}
		}
		#endregion

		#region Clusters
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetClusters(int actorId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var clusters = Clusters.Where(c => isAdmin);

				/// <summary>TODO</summary>
				return EntityDataReader(clusters);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "GetClusters",
				 /// <summary>TODO</summary>
				 new SqlParameter("@actorId", actorId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddCluster(string clusterName)
		{
			if (UseEntityFramework)
			{
				var cluster = new Data.Entities.Cluster() { ClusterName = clusterName };

				Clusters.Add(cluster);
				SaveChanges();

				return cluster.ClusterId;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@ClusterID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					 ObjectQualifier + "AddCluster",
					 prmId,
					 /// <summary>TODO</summary>
					 new SqlParameter("@ClusterName", clusterName));

				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteCluster(int clusterId)
		{
			if (UseEntityFramework)
			{
				using (var transaction = Database.BeginTransaction())
				{
					// reset cluster in services
					Services
						.Where(s => s.ClusterId == clusterId)
						.ExecuteUpdate(s => new Data.Entities.Service { ClusterId = null });
					// delete cluster
					Clusters
						.Where(c => c.ClusterId == clusterId)
						.ExecuteDelete();

					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "DeleteCluster",
				 /// <summary>TODO</summary>
				 new SqlParameter("@ClusterId", clusterId));
			}
		}

		#endregion

		#region Global DNS records
		/// <summary>Auto-generated member.</summary>

		public string GetFullIPAddress(string externalIp, string internalIp)
		{
			var ip = string.Empty;

			if (!string.IsNullOrEmpty(externalIp)) ip = externalIp;

			if (!string.IsNullOrEmpty(internalIp))
			{
				ip = ip == string.Empty ? internalIp : $"{ip} ({internalIp})";
			}

			/// <summary>TODO</summary>
			return ip;
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetDnsRecordsByService(int actorId, int serviceId)
		{
			if (UseEntityFramework)
			{
				var records = GlobalDnsRecords
					.Where(r => r.ServiceId == serviceId)
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId,
						ExternalIp = r.IpAddress != null ? r.IpAddress.ExternalIp : null,
						InternalIp = r.IpAddress != null ? r.IpAddress.InternalIp : null
					})
					.AsEnumerable()
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						FullRecordData = r.RecordType == "A" && string.IsNullOrEmpty(r.RecordData) ?
							GetFullIPAddress(r.ExternalIp, r.InternalIp) :
							(r.RecordType == "MX" ?
								$"{r.MXPriority}, {r.RecordData}" :
								(r.RecordType == "SRV" ? $"{r.SrvPort}, {r.RecordData}" :
									r.RecordData)),
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId,
						IPAddress = GetFullIPAddress(r.ExternalIp, r.InternalIp),
						r.ExternalIp,
						r.InternalIp
					});
				/// <summary>TODO</summary>
				return EntityDataSet(records);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "GetDnsRecordsByService",
				 /// <summary>TODO</summary>
				 new SqlParameter("@actorId", actorId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@ServiceId", serviceId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetDnsRecordsByServer(int actorId, int serverId)
		{
			if (UseEntityFramework)
			{
				var records = GlobalDnsRecords
					.Where(r => r.ServerId == serverId)
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId,
						ExternalIp = r.IpAddress != null ? r.IpAddress.ExternalIp : null,
						InternalIp = r.IpAddress != null ? r.IpAddress.InternalIp : null
					})
					.AsEnumerable()
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						FullRecordData = r.RecordType == "A" && string.IsNullOrEmpty(r.RecordData) ?
							GetFullIPAddress(r.ExternalIp, r.InternalIp) :
							(r.RecordType == "MX" ?
								$"{r.MXPriority}, {r.RecordData}" :
								(r.RecordType == "SRV" ? $"{r.SrvPort}, {r.RecordData}" :
									r.RecordData)),
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId,
						IPAddress = GetFullIPAddress(r.ExternalIp, r.InternalIp),
						r.ExternalIp,
						r.InternalIp
					});
				/// <summary>TODO</summary>
				return EntityDataSet(records);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "GetDnsRecordsByServer",
				 /// <summary>TODO</summary>
				 new SqlParameter("@actorId", actorId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@ServerId", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckActorPackageRights(int actorId, int? packageId)
		{
			if (actorId == -1 || packageId == null) return true;

			// check if this is a 'system' package
			if (packageId >= 0 && packageId <= 1 && !CheckIsUserAdmin(actorId)) return false;

			// get package owner
			var ownerId = Packages
				.Where(p => p.PackageId == packageId)
				.Select(p => (int?)p.UserId)
				.FirstOrDefault();

			if (ownerId == null) return true; // unexisting package

			/// <summary>TODO</summary>
			return CheckActorUserRights(actorId, ownerId.Value);
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetDnsRecordsByPackage(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId)) throw new AccessViolationException("You are not allowed to access this package");

				var records = GlobalDnsRecords
					.Where(r => r.PackageId == packageId)
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId,
						ExternalIp = r.IpAddress != null ? r.IpAddress.ExternalIp : null,
						InternalIp = r.IpAddress != null ? r.IpAddress.InternalIp : null
					})
					.AsEnumerable()
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						FullRecordData = r.RecordType == "A" && string.IsNullOrEmpty(r.RecordData) ?
							GetFullIPAddress(r.ExternalIp, r.InternalIp) :
							(r.RecordType == "MX" ?
								$"{r.MXPriority}, {r.RecordData}" :
								(r.RecordType == "SRV" ? $"{r.SrvPort}, {r.RecordData}" :
									r.RecordData)),
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId,
						IPAddress = GetFullIPAddress(r.ExternalIp, r.InternalIp),
						r.ExternalIp,
						r.InternalIp
					});
				/// <summary>TODO</summary>
				return EntityDataSet(records);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "GetDnsRecordsByPackage",
				 /// <summary>TODO</summary>
				 new SqlParameter("@actorId", actorId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@PackageId", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetDnsRecordsByGroup(int groupId)
		{
			if (UseEntityFramework)
			{
				var records = ResourceGroupDnsRecords
					.Where(r => r.GroupId == groupId)
					.OrderBy(r => r.RecordOrder)
					.Select(r => new
					{
						r.RecordId,
						r.RecordOrder,
						r.GroupId,
						r.RecordType,
						r.RecordName,
						r.RecordData,
						r.MXPriority
					});

				/// <summary>TODO</summary>
				return EntityDataSet(records);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "GetDnsRecordsByGroup",
				 /// <summary>TODO</summary>
				 new SqlParameter("@GroupId", groupId));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public DataSet GetDnsRecordsTotal(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId)) throw new AccessViolationException("You are not allowed to access this package");

				// select PACKAGES DNS records
				var pid = (int?)packageId;

				IQueryable<Data.Entities.GlobalDnsRecord> records = GlobalDnsRecords
					.Where(r => r.PackageId == pid);
				pid = Packages
					.Where(p => p.PackageId == pid)
					.Select(p => p.ParentPackageId)
					.FirstOrDefault();
				while (pid != null)
				{
					records = records
						.Concat(GlobalDnsRecords
							.Where(r => r.PackageId == pid));
					pid = Packages
						.Where(p => p.PackageId == pid)
						.Select(p => p.ParentPackageId)
						.FirstOrDefault();
				}

				// select VIRTUAL SERVER DNS records
				var serverId = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => p.ServerId)
					.FirstOrDefault();

				records = records
					.Concat(GlobalDnsRecords
						.Where(r => r.ServerId == serverId));

				// select SERVER DNS records
				records = records
					.Concat(GlobalDnsRecords
						.Where(r => r.ServerId == serverId)
						.Join(Services, r => r.ServerId, s => s.ServerId, (r, s) => new
						{
							Record = r,
							Service = s
						})
						.Join(VirtualServices, r => r.Service.ServiceId, vs => vs.ServiceId, (r, vs) => new
						{
							r.Record,
							vs.ServerId
						})
						.Where(r => r.ServerId == serverId)
						.Select(r => r.Record));

				// select SERVICES DNS records

				// re-distribute package services
				DistributePackageServices(actorId, packageId);

				// TODO uncomment this?
				/* records = records
					.Concat(GlobalDnsRecords
					.Join(PackageServices.Where(p => p.PackageId == packageId),
						r => r.ServiceId, p => p.ServiceId, (r, p) => r)); */

#if NETCOREAPP
				records = records.DistinctBy(r => r.RecordType + r.RecordName);
#else
				records = records.GroupBy(r => new { r.RecordType, r.RecordName })
					.Select(g => g.FirstOrDefault());
#endif
				var recordsSelected = records
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId,
						ExternalIp = r.IpAddress != null ? r.IpAddress.ExternalIp : null,
						InternalIp = r.IpAddress != null ? r.IpAddress.InternalIp : null
					})
					.AsEnumerable()
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId,
						ExternalIp = r.ExternalIp ?? "",
						InternalIp = r.InternalIp ?? "",
						FullRecordData = r.RecordType == "A" && string.IsNullOrEmpty(r.RecordData) ?
							GetFullIPAddress(r.ExternalIp, r.InternalIp) :
							(r.RecordType == "MX" ?
								$"{r.MXPriority}, {r.RecordData}" :
								(r.RecordType == "SRV" ? $"{r.SrvPort}, {r.RecordData}" :
									r.RecordData)),
						IPAddress = GetFullIPAddress(r.ExternalIp, r.InternalIp)
					});
				/// <summary>TODO</summary>
				return EntityDataSet(recordsSelected);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetDnsRecordsTotal",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetDnsRecord(int actorId, int recordId)
		{
			if (UseEntityFramework)
			{
				// check rights
				var records = GlobalDnsRecords
					.Where(r => r.RecordId == recordId)
					.Select(r => new
					{
						r.RecordId,
						r.ServiceId,
						r.ServerId,
						r.PackageId,
						r.RecordType,
						r.RecordName,
						r.RecordData,
						r.MXPriority,
						r.SrvPriority,
						r.SrvWeight,
						r.SrvPort,
						r.IpAddressId
					})
					.ToList();
				var record = records
					.SingleOrDefault();

				if (record != null && (record.ServiceId > 0 || record.ServerId > 0) && !CheckIsUserAdmin(actorId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to perform this operation");

				if (record != null && record.PackageId > 0 && !CheckActorPackageRights(actorId, record.PackageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				/// <summary>TODO</summary>
				return EntityDataReader(records);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetDnsRecord",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordId", recordId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddDnsRecord(int actorId, int serviceId, int serverId, int packageId, string recordType,
			 string recordName, string recordData, int mxPriority, int SrvPriority, int SrvWeight, int SrvPort, int ipAddressId)
		{
			if (UseEntityFramework)
			{
				if ((serviceId > 0 || serverId > 0) && !CheckIsUserAdmin(actorId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You should have administrator role to perform such operation");

				if (packageId > 0 && !CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				int? serverIdOrNull = serverId != 0 ? serverId : null;
				int? serviceIdOrNull = serviceId != 0 ? serviceId : null;
				int? packageIdOrNull = packageId != 0 ? packageId : null;
				int? ipAddressIdOrNull = ipAddressId != 0 ? ipAddressId : null;

				var record = GlobalDnsRecords
					.FirstOrDefault(r => r.ServiceId == serviceIdOrNull &&
						r.ServerId == serverIdOrNull && r.PackageId == packageIdOrNull &&
						r.RecordName == recordName && r.RecordType == recordType);

				if (record == null)
				{
					record = new Data.Entities.GlobalDnsRecord()
					{
						RecordType = recordType,
						RecordName = recordName,
						ServiceId = serviceIdOrNull,
						ServerId = serverIdOrNull,
						PackageId = packageIdOrNull
					};
					GlobalDnsRecords.Add(record);
				}

				record.RecordData = recordData;
				record.MXPriority = mxPriority;
				record.SrvPriority = SrvPriority;
				record.SrvWeight = SrvWeight;
				record.SrvPort = SrvPort;
				record.IpAddressId = ipAddressIdOrNull;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddDnsRecord",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceId", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordType", recordType),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordName", recordName),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordData", recordData),
					/// <summary>TODO</summary>
					new SqlParameter("@MXPriority", mxPriority),
					/// <summary>TODO</summary>
					new SqlParameter("@SrvPriority", SrvPriority),
					/// <summary>TODO</summary>
					new SqlParameter("@SrvWeight", SrvWeight),
					/// <summary>TODO</summary>
					new SqlParameter("@SrvPort", SrvPort),
					/// <summary>TODO</summary>
					new SqlParameter("@IpAddressId", ipAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void UpdateDnsRecord(int actorId, int recordId, string recordType,
			 string recordName, string recordData, int mxPriority, int SrvPriority, int SrvWeight, int SrvPort, int ipAddressId)
		{
			if (UseEntityFramework)
			{
				int? ipAddressIdOrNull = ipAddressId != 0 ? ipAddressId : null;

				var record = GlobalDnsRecords
					.FirstOrDefault(r => r.RecordId == recordId);

				if (record != null)
				{
					// check rights
					if ((record.ServiceId > 0 || record.ServerId > 0) && !CheckIsUserAdmin(actorId))
						/// <summary>TODO</summary>
						throw new AccessViolationException("You are not allowed to perform this operation");

					if (record.PackageId > 0 && !CheckActorPackageRights(actorId, record.PackageId))
						/// <summary>TODO</summary>
						throw new AccessViolationException("You are not allowed to access this package");

					record.RecordType = recordType;
					record.RecordName = recordName;
					record.RecordData = recordData;
					record.MXPriority = mxPriority;
					record.SrvPriority = SrvPriority;
					record.SrvWeight = SrvWeight;
					record.SrvPort = SrvPort;
					record.IpAddressId = ipAddressIdOrNull;

					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateDnsRecord",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordId", recordId),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordType", recordType),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordName", recordName),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordData", recordData),
					/// <summary>TODO</summary>
					new SqlParameter("@MXPriority", mxPriority),
					/// <summary>TODO</summary>
					new SqlParameter("@SrvPriority", SrvPriority),
					/// <summary>TODO</summary>
					new SqlParameter("@SrvWeight", SrvWeight),
					/// <summary>TODO</summary>
					new SqlParameter("@SrvPort", SrvPort),
					/// <summary>TODO</summary>
					new SqlParameter("@IpAddressId", ipAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>


		public void DeleteDnsRecord(int actorId, int recordId)
		{
			if (UseEntityFramework)
			{
				var record = GlobalDnsRecords
					.FirstOrDefault(r => r.RecordId == recordId);

				if (record != null)
				{
					// check rights
					if ((record.ServerId > 0 || record.ServiceId > 0) && !CheckIsUserAdmin(actorId) ||
						record.PackageId > 0 && !CheckActorPackageRights(actorId, record.PackageId))
						return;

					GlobalDnsRecords.Remove(record);

					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteDnsRecord",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordId", recordId));
			}
		}
		#endregion

		#region Domains
		/// <summary>Auto-generated member.</summary>

		public TempIdSet PackagesTree(int packageId, bool recursive = false)
		{
			var tree = new TempIdSet(this);
			tree.Add(packageId);
			SaveChanges();

			if (recursive)
			{
				int level = 0;
				var children = Packages
					.Where(p => p.ParentPackageId == packageId)
					.Select(p => p.PackageId);
				while (tree.AddRange(children, ++level) > 0)
				{
					children = Packages
						.Join(tree.OfLevel(level), pkg => pkg.ParentPackageId, t => t, (pkg, t) => pkg.PackageId);
				}
			}

			/// <summary>TODO</summary>
			return tree;
		}
		/// <summary>Auto-generated member.</summary>
		public DataSet GetDomains(int actorId, int packageId, bool recursive = true)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var tree = PackagesTree(packageId, recursive))
				{
					var domains = Domains
						.Join(tree, d => d.PackageId, t => t, (d, t) => d)
						.Select(d => new
						{
							d.DomainId,
							d.PackageId,
							d.ZoneItemId,
							d.DomainItemId,
							d.DomainName,
							d.HostingAllowed,
							WebSiteId = d.WebSite != null ? d.WebSite.ItemId : 0,
							WebSiteName = d.WebSite != null ? d.WebSite.ItemName : null,
							MailDomainId = d.MailDomain != null ? d.MailDomain.ItemId : 0,
							MailDomainName = d.MailDomain != null ? d.MailDomain.ItemName : null,
							ZoneName = d.Zone != null ? d.Zone.ItemName : null,
							d.IsSubDomain,
							d.IsPreviewDomain,
							d.CreationDate,
							d.ExpirationDate,
							d.LastUpdateDate,
							d.IsDomainPointer,
							d.RegistrarName
						});
					/// <summary>TODO</summary>
					return EntityDataSet(domains);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
				 ObjectQualifier + "GetDomains",
				 /// <summary>TODO</summary>
				 new SqlParameter("@ActorId", actorId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@PackageId", packageId),
				 /// <summary>TODO</summary>
				 new SqlParameter("@Recursive", recursive));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetResellerDomains(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				// load parent package
				var parentPackageId = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => p.ParentPackageId)
					.FirstOrDefault();
				var domains = Domains
					.Where(d => d.HostingAllowed && d.PackageId == parentPackageId)
					.Select(d => new
					{
						d.DomainId,
						d.PackageId,
						d.ZoneItemId,
						d.DomainName,
						d.HostingAllowed,
						d.WebSiteId,
						WebSiteName = d.WebSite != null ? d.WebSite.ItemName : null,
						d.MailDomainId,
						MailDomainName = d.MailDomain != null ? d.MailDomain.ItemName : null
					});
				/// <summary>TODO</summary>
				return EntityDataSet(domains);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetResellerDomains",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetDomainsPaged(int actorId, int packageId, int serverId, bool recursive, string filterColumn, string filterValue,
			 string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				TempIdSet childPackages = null;
				try
				{
					/// <summary>TODO</summary>
					IQueryable<Data.Entities.Domain> domainsFiltered;
					if (!recursive) domainsFiltered = Domains.Where(d => d.PackageId == packageId);
					else
					{
						childPackages = PackagesTree(packageId, true);
						domainsFiltered = Domains
							.Join(childPackages, d => d.PackageId, ch => ch, (d, ch) => d);
					}

					var domains = domainsFiltered
						.Where(d => !d.IsPreviewDomain && !d.IsDomainPointer &&
							(serverId == 0 ||
								d.Zone != null && d.Zone.Service != null &&
								d.Zone.Service.ServerId == serverId))
						.Select(d => new
						{
							d.DomainId,
							d.PackageId,
							d.ZoneItemId,
							d.DomainItemId,
							d.DomainName,
							d.HostingAllowed,
							WebSiteId = d.WebSite != null ? d.WebSite.ItemId : 0,
							WebSiteName = d.WebSite != null ? d.WebSite.ItemName : null,
							MailDomainId = d.MailDomain != null ? d.MailDomain.ItemId : 0,
							MailDomainName = d.MailDomain != null ? d.MailDomain.ItemName : null,
							d.IsSubDomain,
							d.IsPreviewDomain,
							d.IsDomainPointer,
							d.ExpirationDate,
							d.LastUpdateDate,
							d.RegistrarName,
							d.Package.PackageName,
							ServerId = serverId != 0 ? d.Zone.Service.Server.ServerId : 0,
							ServerName = serverId != 0 ? d.Zone.Service.Server.ServerName : "",
							ServerComments = serverId != 0 ? d.Zone.Service.Server.Comments : "",
							VirtualServer = serverId != 0 && d.Zone.Service.Server.VirtualServer,
							d.Package.UserId,
							d.Package.User.Username,
							d.Package.User.FirstName,
							d.Package.User.LastName,
							FullName = d.Package.User.FirstName + " " + d.Package.User.LastName,
							d.Package.User.RoleId,
							d.Package.User.Email
						});

					if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
					{
						domains = domains.Where(DynamicFunctions.ColumnLike(domains, filterColumn, filterValue));
					}
					else if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
							domains = domains.Where(d => DbFunctions.Like(d.DomainName, filterValue) ||
								DbFunctions.Like(d.Username, filterValue) ||
								DbFunctions.Like(d.ServerName, filterValue) ||
								DbFunctions.Like(d.PackageName, filterValue));
#else
							domains = domains.Where(d => EF.Functions.Like(d.DomainName, filterValue) ||
								EF.Functions.Like(d.Username, filterValue) ||
								EF.Functions.Like(d.ServerName, filterValue) ||
								EF.Functions.Like(d.PackageName, filterValue));
#endif
					}

					var count = domains.Count();

					domains = !string.IsNullOrEmpty(sortColumn) ? domains.OrderBy(ColumnName(sortColumn)) : domains.OrderBy(d => d.DomainName);


					domains = domains.Skip(startRow).Take(maximumRows);

					/// <summary>TODO</summary>
					return EntityDataSet(count, domains);
				}
				finally
				{
					childPackages?.Dispose();
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetDomainsPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@serverId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@recursive", recursive),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@MaximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetDomain(int actorId, int domainId)
		{
			if (UseEntityFramework)
			{
				var domains = Domains
					.Where(d => d.DomainId == domainId)
					.Select(d => new
					{
						d.DomainId,
						d.PackageId,
						d.ZoneItemId,
						d.DomainItemId,
						d.DomainName,
						d.HostingAllowed,
						WebSiteId = d.WebSite != null ? d.WebSite.ItemId : 0,
						WebSiteName = d.WebSite != null ? d.WebSite.ItemName : null,
						MailDomainId = d.MailDomain != null ? d.MailDomain.ItemId : 0,
						MailDomainName = d.MailDomain != null ? d.MailDomain.ItemName : null,
						ZoneName = d.Zone != null ? d.Zone.ItemName : null,
						d.IsSubDomain,
						d.IsPreviewDomain,
						d.IsDomainPointer,
						ZoneServiceId = d.Zone != null ? d.Zone.ServiceId : null
					})
					.ToList()
					.Where(d => CheckActorPackageRights(actorId, d.PackageId));
				/// <summary>TODO</summary>
				return EntityDataReader(domains);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetDomain",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@domainId", domainId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetDomainByName(int actorId, string domainName, bool searchOnDomainPointer, bool isDomainPointer)
		{
			if (UseEntityFramework)
			{
				var domains = Domains
					.Where(d => d.DomainName == domainName &&
						(!searchOnDomainPointer || d.IsDomainPointer == isDomainPointer))
					.Select(d => new
					{
						d.DomainId,
						d.PackageId,
						d.ZoneItemId,
						d.DomainItemId,
						d.DomainName,
						d.HostingAllowed,
						WebSiteId = d.WebSite != null ? d.WebSite.ItemId : 0,
						WebSiteName = d.WebSite != null ? d.WebSite.ItemName : null,
						MailDomainId = d.MailDomain != null ? d.MailDomain.ItemId : 0,
						MailDomainName = d.MailDomain != null ? d.MailDomain.ItemName : null,
						ZoneName = d.Zone != null ? d.Zone.ItemName : null,
						d.IsSubDomain,
						d.IsPreviewDomain,
						d.IsDomainPointer
					})
					.AsEnumerable()
					.Where(d => Clone.CheckActorPackageRights(actorId, d.PackageId));
				/// <summary>TODO</summary>
				return EntityDataReader(domains);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetDomainByName",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@domainName", domainName),
					/// <summary>TODO</summary>
					new SqlParameter("@SearchOnDomainPointer", searchOnDomainPointer),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDomainPointer", isDomainPointer));
			}
		}
		/// <summary>Auto-generated member.</summary>


		public DataSet GetDomainsByZoneId(int actorId, int zoneId)
		{
			if (UseEntityFramework)
			{
				var domains = Domains
					.Where(d => d.ZoneItemId == zoneId)
					.Select(d => new
					{
						d.DomainId,
						d.PackageId,
						d.ZoneItemId,
						d.DomainItemId,
						d.DomainName,
						d.HostingAllowed,
						WebSiteId = d.WebSite != null ? d.WebSite.ItemId : 0,
						WebSiteName = d.WebSite != null ? d.WebSite.ItemName : null,
						MailDomainId = d.MailDomain != null ? d.MailDomain.ItemId : 0,
						MailDomainName = d.MailDomain != null ? d.MailDomain.ItemName : null,
						ZoneName = d.Zone != null ? d.Zone.ItemName : null,
						d.IsSubDomain,
						d.IsPreviewDomain,
						d.IsDomainPointer
					})
					.AsEnumerable()
					.Where(d => Clone.CheckActorPackageRights(actorId, d.PackageId));
				/// <summary>TODO</summary>
				return EntityDataSet(domains);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetDomainsByZoneID",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ZoneID", zoneId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetDomainsByDomainItemId(int actorId, int domainId)
		{
			if (UseEntityFramework)
			{
				var domains = Domains
					.Where(d => d.DomainItemId == domainId)
					.Select(d => new
					{
						d.DomainId,
						d.PackageId,
						d.ZoneItemId,
						d.DomainItemId,
						d.DomainName,
						d.HostingAllowed,
						WebSiteId = d.WebSite != null ? d.WebSite.ItemId : 0,
						WebSiteName = d.WebSite != null ? d.WebSite.ItemName : null,
						MailDomainId = d.MailDomain != null ? d.MailDomain.ItemId : 0,
						MailDomainName = d.MailDomain != null ? d.MailDomain.ItemName : null,
						ZoneName = d.Zone != null ? d.Zone.ItemName : null,
						d.IsSubDomain,
						d.IsPreviewDomain,
						d.IsDomainPointer
					})
					.AsEnumerable()
					.Where(d => Clone.CheckActorPackageRights(actorId, d.PackageId));
				/// <summary>TODO</summary>
				return EntityDataSet(domains);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetDomainsByDomainItemID",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainID", domainId));
			}
		}
		/// <summary>Auto-generated member.</summary>



		public int CheckDomain(int packageId, string domainName, bool isDomainPointer)
		{
			if (UseEntityFramework)
			{
				// check if the domain already exists
				if (Domains.Any(d => d.DomainName == domainName && d.IsDomainPointer == isDomainPointer)) return -1;

				// check if this is a sub-domain of other domain that is not allowed for 3rd level hosting
				var userId = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => p.UserId)
					.FirstOrDefault();

				// find sub-domains
				var subdomains = Domains
					.Where(d => domainName.IndexOf("." + d.DomainName) >= 0 &&
						domainName.IndexOf("." + d.DomainName) + d.DomainName.Length + 1 == domainName.Length + 1 &&
						!d.IsDomainPointer)
					.Join(Packages, d => d.PackageId, p => p.PackageId, (d, p) => new
					{
						p.UserId,
						d.HostingAllowed
					});

				if (subdomains.Any(d => d.UserId != userId && !d.HostingAllowed)) return -2;

				return 0;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@Result", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CheckDomain",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@domainName", domainName),
					/// <summary>TODO</summary>
					new SqlParameter("@isDomainPointer", isDomainPointer));

				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>



		public int CheckDomainUsedByHostedOrganization(string domainName)
		{
			if (UseEntityFramework)
			{
				var mailDomain = $"@{domainName}";

				return ExchangeAccounts.Any(a => a.UserPrincipalName.EndsWith(mailDomain) && a.AccountType != ExchangeAccountType.Contact) ||
					ExchangeAccountEmailAddresses.Any(e => e.EmailAddress.EndsWith(mailDomain)) ||
					LyncUsers.Any(u => u.SipAddress.EndsWith(mailDomain)) ||
					SfBUsers.Any(u => u.SipAddress.EndsWith(mailDomain))
					? 1
					: 0;

			}
			else
			{
				SqlParameter prmId = new SqlParameter("@Result", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CheckDomainUsedByHostedOrganization",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@domainName", domainName));

				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>


		public int AddDomain(int actorId, int packageId, int zoneItemId, string domainName,
			 bool hostingAllowed, int webSiteId, int mailDomainId, bool isSubDomain, bool isPreviewDomain, bool isDomainPointer)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var domain = new Data.Entities.Domain()
				{
					PackageId = packageId,
					ZoneItemId = zoneItemId != 0 ? zoneItemId : null,
					DomainName = domainName,
					HostingAllowed = hostingAllowed,
					WebSiteId = webSiteId != 0 ? webSiteId : null,
					MailDomainId = mailDomainId != 0 ? mailDomainId : null,
					IsSubDomain = isSubDomain,
					IsPreviewDomain = isPreviewDomain,
					IsDomainPointer = isDomainPointer
				};

				Domains.Add(domain);

				SaveChanges();

				return domain.DomainId;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@DomainID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddDomain",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@ZoneItemId", zoneItemId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainName", domainName),
					/// <summary>TODO</summary>
					new SqlParameter("@HostingAllowed", hostingAllowed),
					/// <summary>TODO</summary>
					new SqlParameter("@WebSiteId", webSiteId),
					/// <summary>TODO</summary>
					new SqlParameter("@MailDomainId", mailDomainId),
					/// <summary>TODO</summary>
					new SqlParameter("@IsSubDomain", isSubDomain),
					/// <summary>TODO</summary>
					new SqlParameter("@IsPreviewDomain", isPreviewDomain),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDomainPointer", isDomainPointer));

				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateDomain(int actorId, int domainId, int zoneItemId,
			 bool hostingAllowed, int webSiteId, int mailDomainId, int domainItemId)
		{
			if (UseEntityFramework)
			{
				var domain = Domains.FirstOrDefault(d => d.DomainId == domainId);
				if (domain == null) return;

				if (!CheckActorPackageRights(actorId, domain.PackageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				domain.ZoneItemId = zoneItemId != 0 ? zoneItemId : null;
				domain.HostingAllowed = hostingAllowed;
				domain.WebSiteId = webSiteId != 0 ? webSiteId : null;
				domain.MailDomainId = mailDomainId != 0 ? mailDomainId : null;
				domain.DomainItemId = domainItemId;

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateDomain",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainId", domainId),
					/// <summary>TODO</summary>
					new SqlParameter("@ZoneItemId", zoneItemId),
					/// <summary>TODO</summary>
					new SqlParameter("@HostingAllowed", hostingAllowed),
					/// <summary>TODO</summary>
					new SqlParameter("@WebSiteId", webSiteId),
					/// <summary>TODO</summary>
					new SqlParameter("@MailDomainId", mailDomainId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainItemId", domainItemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteDomain(int actorId, int domainId)
		{
			if (UseEntityFramework)
			{
				var domain = Domains.FirstOrDefault(d => d.DomainId == domainId);
				if (domain == null) return;

				if (!CheckActorPackageRights(actorId, domain.PackageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				Domains.Remove(domain);

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteDomain",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainId", domainId));
			}
		}
		#endregion

		#region Services
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetServicesByServerId(int actorId, int serverId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var services = Services
					.Where(s => isAdmin && s.ServerId == serverId)
					.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => new
					{
						Service = s,
						Provider = p
					})
					.Join(ResourceGroups, s => s.Provider.GroupId, r => r.GroupId, (s, r) => new
					{
						s.Service,
						s.Provider,
						ResourceGroup = r
					})
					.OrderBy(s => s.ResourceGroup.GroupOrder)
					.Select(s => new
					{
						s.Service.ServiceId,
						s.Service.ServerId,
						s.Service.ServiceName,
						s.Service.Comments,
						s.Service.ServiceQuotaValue,
						s.ResourceGroup.GroupName,
						s.Service.ProviderId,
						ProviderName = s.Provider.DisplayName
					});

				/// <summary>TODO</summary>
				return EntityDataReader(services);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServicesByServerID",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerID", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetServicesByServerIdGroupName(int actorId, int serverId, string groupName)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

                var services = Services
                    .Where(s => isAdmin && s.ServerId == serverId)
                    .Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => new
                    {
                        Service = s,
                        Provider = p
                    })
                    .Join(ResourceGroups, s => s.Provider.GroupId, r => r.GroupId, (s, r) => new
                    {
                        s.Service,
                        s.Provider,
                        r.GroupOrder,
                        r.GroupName,
                    })
                    .OrderBy(s => s.GroupOrder)
                    .Select(s => new
                    {
                        s.Service.ServiceId,
                        s.Service.ServerId,
                        s.Service.ServiceName,
                        s.Service.Comments,
                        s.Service.ServiceQuotaValue,
                        s.GroupName,
                        ProviderName = s.Provider.DisplayName
                    })
                    .Where(s => s.GroupName == groupName);


                /// <summary>TODO</summary>
                return EntityDataReader(services);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServicesByServerIDGroupName",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerID", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupName", groupName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetRawServicesByServerId(int actorId, int serverId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var groups = ResourceGroups
					.Where(g => isAdmin && g.ShowGroup == true)
					.OrderBy(g => g.GroupOrder)
					.Select(g => new { g.GroupId, g.GroupName });

				var services = Services
					.Where(s => isAdmin && s.ServerId == serverId)
					.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => new
					{
						Service = s,
						Provider = p
					})
					.Join(ResourceGroups, s => s.Provider.GroupId, r => r.GroupId, (s, r) => new
					{
						s.Service,
						s.Provider,
						r.GroupId,
						r.GroupOrder
					})
					.OrderBy(s => s.GroupOrder)
					.Select(s => new
					{
                        s.Service.ServiceId,
						s.Service.ServerId,
						s.Service.ServiceName,
						s.Service.Comments,
						s.GroupId,
						ProviderName = s.Provider.DisplayName
					});

				/// <summary>TODO</summary>
				return EntityDataSet(groups, services);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetRawServicesByServerID",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerID", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServicesByGroupId(int actorId, int groupId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var services = Services
					.Where(s => isAdmin)
					.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => new
					{
						Service = s,
						Provider = p
					})
					.Where(s => s.Provider.GroupId == groupId)
					.Join(Servers, s => s.Service.ServerId, s => s.ServerId, (s, t) => new
					{
						s.Service.ServiceId,
						s.Service.ServiceName,
						s.Service.ServerId,
						s.Service.ServiceQuotaValue,
						t.ServerName,
						s.Service.ProviderId,
						FullServiceName = s.Service.ServiceName + " on " + t.ServerName
					});

				/// <summary>TODO</summary>
				return EntityDataSet(services);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServicesByGroupID",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@groupId", groupId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServicesByGroupName(int actorId, string groupName, bool forAutodiscover)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				var services = Services
					.Where(s => isAdmin)
					.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => new
					{
						Service = s,
						Provider = p
					})
					.Join(Servers, s => s.Service.ServerId, t => t.ServerId, (s, t) => new
					{
						s.Service,
						s.Provider,
						Server = t
					})
					.Join(ResourceGroups, s => s.Provider.GroupId, r => r.GroupId, (s, r) => new
					{
						s.Service,
						s.Provider,
						s.Server,
						ResourceGroup = r
					})
					.Where(s => s.ResourceGroup.GroupName == groupName)
					.Select(s => new
					{
						s.Service.ServiceId,
						s.Service.ServiceName,
						s.Service.ServerId,
						s.Service.ServiceQuotaValue,
						s.Server.ServerName,
						s.Service.ProviderId,
						s.Provider.ProviderName,
						FullServiceName = s.Service.ServiceName + " on " + s.Server.ServerName
					});

				/// <summary>TODO</summary>
				return EntityDataSet(services);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServicesByGroupName",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupName", groupName),
					/// <summary>TODO</summary>
					new SqlParameter("@forAutodiscover", forAutodiscover));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetService(int actorId, int serviceId)
		{
			if (UseEntityFramework)
			{
				var services = Services
					.Where(s => s.ServiceId == serviceId)
					.Join(Servers, s => s.ServerId, t => t.ServerId, (s, t) => new
					{
						s.ServiceId,
						s.ServerId,
						s.ProviderId,
						s.ServiceName,
						s.ServiceQuotaValue,
						s.ClusterId,
						s.Comments,
						t.ServerName
					});

				/// <summary>TODO</summary>
				return EntityDataReader(services);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString,
					CommandType.StoredProcedure,
					ObjectQualifier + "GetService",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceID", serviceId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddService(int serverId, int providerId, string serviceName, int serviceQuotaValue,
			 int clusterId, string comments)
		{
			if (UseEntityFramework)
			{
				int serviceId = 0;

				using (var transaction = Database.BeginTransaction())
				{
					var service = new Data.Entities.Service()
					{
						ServerId = serverId,
						ProviderId = providerId,
						ServiceName = serviceName,
						ServiceQuotaValue = serviceQuotaValue,
						ClusterId = clusterId != 0 ? clusterId : null,
						Comments = comments
					};
					Services.Add(service);
					SaveChanges();

					serviceId = service.ServiceId;

					// copy default service settings
					var properties = ServiceDefaultProperties
						.Where(p => p.ProviderId == providerId)
						.AsEnumerable()
						.Select(p => new Data.Entities.ServiceProperty
						{
							ServiceId = serviceId,
							PropertyName = p.PropertyName,
							PropertyValue = p.PropertyValue
						});
					ServiceProperties.AddRange(properties);

					// copy all default DNS records for the given service
					var groupId = Providers
						.Where(p => p.ProviderId == providerId)
						.Select(p => p.GroupId)
						.FirstOrDefault();

					// default IP address for added records
					var addressId = IpAddresses
						.Where(ip => ip.ServerId == serverId)
						.Select(ip => ip.AddressId)
						.FirstOrDefault();

					var dnsRecords = ResourceGroupDnsRecords
						.Where(r => r.GroupId == groupId)
						.OrderBy(r => r.RecordOrder)
						.AsEnumerable()
						.Select(r => new Data.Entities.GlobalDnsRecord
						{
							RecordType = r.RecordType,
							RecordName = r.RecordName,
							RecordData = r.RecordData == "[ip]" ? "" : r.RecordData,
							MXPriority = r.MXPriority != null ? r.MXPriority.Value : 0,
							IpAddressId = r.RecordData == "[ip]" ? addressId : null,
							ServiceId = serviceId,
							ServerId = null,
							PackageId = null
						});
					GlobalDnsRecords.AddRange(dnsRecords);
					SaveChanges();

					transaction.Commit();
				}
				/// <summary>TODO</summary>
				return serviceId;
			}
			else
			{
				SqlParameter prmServiceId = new SqlParameter("@ServiceID", SqlDbType.Int);
				prmServiceId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddService",
					prmServiceId,
					/// <summary>TODO</summary>
					new SqlParameter("@ServerID", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@ProviderID", providerId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceName", serviceName),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceQuotaValue", serviceQuotaValue),
					/// <summary>TODO</summary>
					new SqlParameter("@ClusterId", clusterId),
					/// <summary>TODO</summary>
					new SqlParameter("@comments", comments));

				UpdateServerPackageServices(serverId);

				return Convert.ToInt32(prmServiceId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateServiceFully(int serviceId, int providerId, string serviceName, int serviceQuotaValue,
			 int clusterId, string comments)
		{
			if (UseEntityFramework)
			{
				var service = Services
					.FirstOrDefault(s => s.ServiceId == serviceId);

				if (service != null)
				{
					service.ProviderId = providerId;
					service.ServiceName = serviceName;
					service.ServiceQuotaValue = serviceQuotaValue;
					service.Comments = comments;
					service.ClusterId = clusterId != 0 ? clusterId : null;

					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateServiceFully",
					/// <summary>TODO</summary>
					new SqlParameter("@ProviderID", providerId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceName", serviceName),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceID", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceQuotaValue", serviceQuotaValue),
					/// <summary>TODO</summary>
					new SqlParameter("@ClusterId", clusterId),
					/// <summary>TODO</summary>
					new SqlParameter("@Comments", comments));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateService(int serviceId, string serviceName, int serviceQuotaValue,
			 int clusterId, string comments)
		{
			if (UseEntityFramework)
			{
				var service = Services
					.FirstOrDefault(s => s.ServiceId == serviceId);

				if (service != null)
				{
					service.ServiceName = serviceName;
					service.ServiceQuotaValue = serviceQuotaValue;
					service.Comments = comments;
					service.ClusterId = clusterId != 0 ? clusterId : null;

					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateService",
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceName", serviceName),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceID", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceQuotaValue", serviceQuotaValue),
					/// <summary>TODO</summary>
					new SqlParameter("@ClusterId", clusterId),
					/// <summary>TODO</summary>
					new SqlParameter("@Comments", comments));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteService(int serviceId)
		{
			if (UseEntityFramework)
			{
				// check related service items
				if (ServiceItems.Any(s => s.ServiceId == serviceId)) return -1;
				if (VirtualServices.Any(s => s.ServiceId == serviceId)) return -2;

				using (var transaction = Database.BeginTransaction())
				{
					GlobalDnsRecords.Where(r => r.ServiceId == serviceId).ExecuteDelete();
					Services.Where(s => s.ServiceId == serviceId).ExecuteDelete();
					PackageServices.Where(s => s.ServiceId == serviceId).ExecuteDelete();

					transaction.Commit();
					return 0;
				}
			}
			else
			{
				SqlParameter prmResult = new SqlParameter("@Result", SqlDbType.Int);
				prmResult.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteService",
					prmResult,
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceID", serviceId));

				return Convert.ToInt32(prmResult.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetServiceProperties(int actorId, int serviceId)
		{
			if (UseEntityFramework)
			{
				var properties = ServiceProperties
					.Where(s => s.ServiceId == serviceId);

				/// <summary>TODO</summary>
				return EntityDataReader(properties);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString,
					CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceProperties",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceID", serviceId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateServiceProperties(int serviceId, string xml)
		{
			{ // always use EF since StoredProcedure is very slow
				var properties = XElement.Parse(xml)
					.Elements()
					.Select(e => new Data.Entities.ServiceProperty
					{
						ServiceId = serviceId,
						PropertyName = (string)e.Attribute("name"),
						PropertyValue = (string)e.Attribute("value")
					})
					.ToList();

				// delete old properties (case insensitive, as strings are case insensitve in SQL Server)
				var serviceProperties = ServiceProperties
					.Where(s => s.ServiceId == serviceId)
					.AsEnumerable()
					.Join(properties, sp => sp.PropertyName, p => p.PropertyName, (sp, p) => sp, StringComparer.InvariantCultureIgnoreCase);
				ServiceProperties.RemoveRange(serviceProperties);

				ServiceProperties.AddRange(properties);

				SaveChanges();
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetResourceGroup(int groupId)
		{
			if (UseEntityFramework)
			{
				var groups = ResourceGroups
					.Where(g => g.GroupId == groupId)
					.Select(g => new
					{
						g.GroupId,
						g.GroupOrder,
						g.GroupName,
						g.GroupController
					});

				/// <summary>TODO</summary>
				return EntityDataReader(groups);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString,
					CommandType.StoredProcedure,
					ObjectQualifier + "GetResourceGroup",
					/// <summary>TODO</summary>
					new SqlParameter("@groupId", groupId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetResourceGroups()
		{
			if (UseEntityFramework)
			{
				var groups = ResourceGroups
					.OrderBy(g => g.GroupOrder)
					.Select(g => new
					{
						g.GroupId,
						g.GroupName,
						g.GroupController
					});

				/// <summary>TODO</summary>
				return EntityDataSet(groups);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString,
					CommandType.StoredProcedure,
					ObjectQualifier + "GetResourceGroups");
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetResourceGroupByName(string groupName)
		{
			if (UseEntityFramework)
			{
				var group = ResourceGroups
					.Where(g => g.GroupName == groupName)
					.Select(g => new
					{
						g.GroupId,
						g.GroupOrder,
						g.GroupName,
						g.GroupController
					});
				/// <summary>TODO</summary>
				return EntityDataReader(group);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString,
					CommandType.StoredProcedure,
					ObjectQualifier + "GetResourceGroupByName",
					/// <summary>TODO</summary>
					new SqlParameter("@groupName", groupName));
			}
		}

		#endregion

		#region Service Items
		/// <summary>Auto-generated member.</summary>
		public DataSet GetServiceItems(int actorId, int packageId, string groupName, string itemTypeName, bool recursive)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

                using (var tree = PackagesTree(packageId, recursive))
                using (var items = ServiceItems
                        .Join(tree, s => s.PackageId, p => p, (s, p) => s)
                        .Join(ServiceItemTypes, s => s.ItemTypeId, t => t.ItemTypeId, (s, t) => new
                        {
                            Item = s,
                            Type = t
                        })
                        .Join(ResourceGroups, s => s.Type.GroupId, r => r.GroupId, (s, r) => new
                        {
                            s.Item,
                            s.Type,
                            ResourceGroup = r
                        })
                        .Where(s => s.Type.TypeName == itemTypeName &&
                            (groupName == null || groupName == s.ResourceGroup.GroupName))
                        .Select(s => s.Item.ItemId)
                        .ToTempIdSet(this))
                {

                    // select service items
                    var serviceItems = items
						.Join(ServiceItems, i => i, s => s.ItemId, (i, s) => s)
						.Join(ServiceItemTypes, s => s.ItemTypeId, t => t.ItemTypeId, (s, t) => new
						{
							Item = s,
							Type = t
						})
						.Join(Packages, s => s.Item.PackageId, p => p.PackageId, (s, p) => new
						{
							s.Item,
							s.Type,
							Package = p
						})
						.Join(Services, s => s.Item.ServiceId, s => s.ServiceId, (s, t) => new
						{
							s.Item,
							s.Type,
							s.Package,
							Service = t
						})
						.Join(Servers, s => s.Service.ServerId, t => t.ServerId, (s, t) => new
						{
							s.Item,
							s.Type,
							s.Package,
							s.Service,
							Server = t
						})
						.Join(ResourceGroups, s => s.Type.GroupId, r => r.GroupId, (s, r) => new
						{
							s.Item,
							s.Type,
							s.Package,
							s.Service,
							s.Server,
							ResourceGroup = r
						})
						.Join(Users, s => s.Package.UserId, u => u.UserId, (s, u) => new
						{
							s.Item.ItemId,
							s.Item.ItemName,
							s.Item.ItemTypeId,
							s.Type.TypeName,
							s.Item.ServiceId,
							s.Item.PackageId,
							s.Package.PackageName,
							s.Service.ServiceName,
							s.Server.ServerId,
							s.Server.ServerName,
							s.ResourceGroup.GroupName,
							u.UserId,
							u.Username,
							UserFullName = u.FirstName + " " + u.LastName,
							s.Item.CreatedDate
						});

					// select item properties
					// get corresponding item properties
					var itemProperties = ServiceItemProperties
						.Join(items, p => p.ItemId, i => i, (p, i) => new
						{
							p.ItemId,
							p.PropertyName,
							p.PropertyValue
						});

					/// <summary>TODO</summary>
					return EntityDataSet(serviceItems, itemProperties);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItems",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupName", groupName),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemTypeName", itemTypeName),
					/// <summary>TODO</summary>
					new SqlParameter("@Recursive", recursive));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServiceItemsPaged(int actorId, int packageId, string groupName, string itemTypeName,
			int serverId, bool recursive, string filterColumn, string filterValue,
			string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var groupId = ResourceGroups
					.Where(r => r.GroupName == groupName)
					.Select(r => (int?)r.GroupId)
					.FirstOrDefault();

				var itemTypeId = ServiceItemTypes
					.Where(t => t.TypeName == itemTypeName &&
						(groupId == null || t.GroupId == groupId))
					.Select(t => t.ItemTypeId)
					.FirstOrDefault();

				/// <summary>TODO</summary>
				IQueryable<Data.Entities.ServiceItem> serviceItems;
				TempIdSet childPackages = null;
				try
				{
					if (!recursive) serviceItems = ServiceItems.Where(si => si.PackageId == packageId);
					else
					{
						childPackages = PackagesTree(packageId, true);
						serviceItems = ServiceItems.Join(childPackages, si => si.PackageId, ch => ch, (p, ch) => p);
					}
					var items = serviceItems
						.Where(s => s.ItemTypeId == itemTypeId)
						// &&
						// (!recursive && s.PackageId == packageId ||
						// recursive && CheckPackageParent(packageId, s.PackageId ?? 0)))
						.Join(Packages, i => i.PackageId, p => p.PackageId, (si, p) => new
						{
							Item = si,
							Package = p
						})
						.Join(ServiceItemTypes, i => i.Item.ItemTypeId, t => t.ItemTypeId, (i, t) => new
						{
							i.Item,
							i.Package,
							Type = t
						})
						.Where(i => groupId == null || i.Type.GroupId == groupId)
						.Join(UsersDetailed, i => i.Package.UserId, u => u.UserId, (i, u) => new
						{
							i.Item,
							i.Package,
							i.Type,
							User = u
						})
						.Join(Services, i => i.Item.ServiceId, s => s.ServiceId, (i, s) => new
						{
							i.Item,
							i.Package,
							i.Type,
							i.User,
							Service = s
						})
						.Where(i => serverId == 0 || serverId > 0 && i.Service.ServerId == serverId)
						.Join(Servers, i => i.Service.ServerId, s => s.ServerId, (i, s) => new
						{
							i.Item,
							i.Package,
							i.Type,
							i.User,
							i.Service,
							Server = s
						})
						.Join(ResourceGroups, i => i.Type.GroupId, r => r.GroupId, (i, r) => new
						{
							i.Item.ItemId,
							i.Item.ItemName,
							i.Item.ItemTypeId,
							i.Type.TypeName,
							i.Item.ServiceId,
							i.Item.PackageId,
							i.Item.CreatedDate,
							r.GroupName,
							i.Package.PackageName,
							i.Server.ServerId,
							i.Server.ServerName,
							ServerComments = i.Server.Comments,
							i.Server.VirtualServer,
							i.Package.UserId,
							i.User.Username,
							i.User.FirstName,
							i.User.LastName,
							i.User.FullName,
							i.User.RoleId,
							i.User.Email
						});

					if (!string.IsNullOrEmpty(filterValue))
					{
						if (!string.IsNullOrEmpty(filterColumn))
						{
							items = items.Where(DynamicFunctions.ColumnLike(items, filterColumn, filterValue));
						}
						else
						{
							items = items
#if NETFRAMEWORK
								.Where(i => DbFunctions.Like(i.ItemName, filterValue) ||
									DbFunctions.Like(i.Username, filterValue) ||
									DbFunctions.Like(i.FullName, filterValue) ||
									DbFunctions.Like(i.Email, filterValue));
#else
							.Where(i => EF.Functions.Like(i.ItemName, filterValue) ||
								EF.Functions.Like(i.Username, filterValue) ||
								EF.Functions.Like(i.FullName, filterValue) ||
								EF.Functions.Like(i.Email, filterValue));
#endif
						}
					}

					var count = items.Count();

					items = !string.IsNullOrEmpty(sortColumn) ? items.OrderBy(ColumnName(sortColumn)) : items.OrderBy(i => i.ItemName);


					items = items.Skip(startRow).Take(maximumRows);

					var properties = ServiceItemProperties
						.Join(items, s => s.ItemId, i => i.ItemId, (s, i) => new
						{
							s.ItemId,
							s.PropertyName,
							s.PropertyValue
						});

					/// <summary>TODO</summary>
					return EntityDataSet(count, items, properties);
				}
				finally
				{
					childPackages?.Dispose();
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItemsPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@groupName", groupName),
					/// <summary>TODO</summary>
					new SqlParameter("@serverId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemTypeName", itemTypeName),
					/// <summary>TODO</summary>
					new SqlParameter("@recursive", recursive),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetSearchableServiceItemTypes()
		{
			if (UseEntityFramework)
			{
				var types = ServiceItemTypes
					.Where(t => t.Searchable == true)
					.OrderBy(t => t.TypeOrder)
					.Select(t => new
					{
						t.ItemTypeId,
						t.DisplayName
					});

				/// <summary>TODO</summary>
				return EntityDataSet(types);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetSearchableServiceItemTypes");
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServiceItemsByService(int actorId, int serviceId)
		{
			if (UseEntityFramework)
			{
				var isAdmin = CheckIsUserAdmin(actorId);

				// select service items
				var items = ServiceItems
					.Where(s => isAdmin && s.ServiceId == serviceId)
					.Join(Packages, i => i.PackageId, p => p.PackageId, (si, p) => new
					{
						Item = si,
						Package = p
					})
					.Join(ServiceItemTypes, i => i.Item.ItemTypeId, t => t.ItemTypeId, (i, t) => new
					{
						i.Item,
						i.Package,
						Type = t
					})
					.Join(UsersDetailed, i => i.Package.UserId, u => u.UserId, (i, u) => new
					{
						i.Item,
						i.Package,
						i.Type,
						User = u
					})
					.Join(Services, i => i.Item.ServiceId, s => s.ServiceId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						Service = s
					})
					.Join(Servers, i => i.Service.ServerId, s => s.ServerId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						i.Service,
						Server = s
					})
					.Join(ResourceGroups, i => i.Type.GroupId, r => r.GroupId, (i, r) => new
					{
						i.Item.ItemId,
						i.Item.ItemName,
						i.Item.ItemTypeId,
						i.Type.TypeName,
						i.Item.ServiceId,
						i.Item.PackageId,
						i.Package.PackageName,
						i.Service.ServiceName,
						i.Server.ServerId,
						i.Server.ServerName,
						r.GroupName,
						i.User.UserId,
						i.User.Username,
						UserFullName = i.User.FirstName + " " + i.User.LastName,
						i.Item.CreatedDate
					});

				// select item properties, get corresponding item properties
				var properties = ServiceItemProperties
					.Join(items, p => p.ItemId, i => i.ItemId, (p, i) => new
					{
						p.ItemId,
						p.PropertyName,
						p.PropertyValue
					});

				/// <summary>TODO</summary>
				return EntityDataSet(items, properties);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItemsByService",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceID", serviceId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetServiceItemsCount(string typeName, string groupName, int serviceId)
		{
			if (UseEntityFramework)
			{
				return ServiceItems
					.Where(s => serviceId == 0 || serviceId > 0 && s.ServiceId == serviceId)
					.Join(ServiceItemTypes, s => s.ItemTypeId, t => t.ItemTypeId, (s, t) => new
					{
						Item = s,
						Type = t
					})
					.Where(s => s.Type.TypeName == typeName)
					.Join(ResourceGroups, s => s.Type.GroupId, r => r.GroupId, (s, r) => r)
					.Where(r => groupName == null || r.GroupName == groupName)
					.Count();
			}
			else
			{
				SqlParameter prmTotalNumber = new SqlParameter("@TotalNumber", SqlDbType.Int);
				prmTotalNumber.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItemsCount",
					prmTotalNumber,
					/// <summary>TODO</summary>
					new SqlParameter("@itemTypeName", typeName),
					/// <summary>TODO</summary>
					new SqlParameter("@groupName", groupName),
					/// <summary>TODO</summary>
					new SqlParameter("@serviceId", serviceId));

				// read identity
				return Convert.ToInt32(prmTotalNumber.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServiceItemsForStatistics(int actorId, int serviceId, int packageId,
			bool calculateDiskspace, bool calculateBandwidth, bool suspendable, bool disposable)
		{
			if (UseEntityFramework)
			{
				var items = ServiceItems
					.Where(s => (serviceId == 0 || serviceId > 0 && s.ServiceId == serviceId) &&
						(packageId == 0 || packageId != 0 && s.PackageId == packageId))
					.Join(ServiceItemTypes, s => s.ItemTypeId, t => t.ItemTypeId, (s, t) => new
					{
						Item = s,
						Type = t
					})
					.Where(s => (!calculateDiskspace || s.Type.CalculateDiskspace == true) &&
						(!calculateBandwidth || s.Type.CalculateBandwidth == true) &&
						(!suspendable || s.Type.Suspendable == true) &&
						(!disposable || s.Type.Disposable == true));

				var serviceItems = items
					.Join(ResourceGroups, s => s.Type.GroupId, r => r.GroupId, (s, r) => new
					{
						s.Item,
						s.Type,
						ResourceGroup = r
					})
					.OrderBy(s => s.ResourceGroup.GroupOrder)
					.Select(s => new
					{
						s.Item.ItemId,
						s.Item.ItemName,
						s.Item.ItemTypeId,
						s.ResourceGroup.GroupName,
						s.Type.TypeName,
						s.Item.ServiceId,
						s.Item.PackageId,
						s.Item.CreatedDate
					});

				var properties = ServiceItemProperties
					.Join(items, p => p.ItemId, i => i.Item.ItemId, (p, i) => new
					{
						p.ItemId,
						p.PropertyName,
						p.PropertyValue
					});

				/// <summary>TODO</summary>
				return EntityDataSet(serviceItems, properties);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItemsForStatistics",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceID", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@calculateDiskspace", calculateDiskspace),
					/// <summary>TODO</summary>
					new SqlParameter("@calculateBandwidth", calculateBandwidth),
					/// <summary>TODO</summary>
					new SqlParameter("@suspendable", suspendable),
					/// <summary>TODO</summary>
					new SqlParameter("@disposable", disposable));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServiceItemsByPackage(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				// select service items
				var items = ServiceItems
					.Where(s => s.PackageId == packageId)
					.Join(Packages, i => i.PackageId, p => p.PackageId, (si, p) => new
					{
						Item = si,
						Package = p
					})
					.Join(ServiceItemTypes, i => i.Item.ItemTypeId, t => t.ItemTypeId, (i, t) => new
					{
						i.Item,
						i.Package,
						Type = t
					})
					.Join(Users, i => i.Package.UserId, u => u.UserId, (i, u) => new
					{
						i.Item,
						i.Package,
						i.Type,
						User = u
					})
					.Join(Services, i => i.Item.ServiceId, s => s.ServiceId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						Service = s
					})
					.Join(Servers, i => i.Service.ServerId, s => s.ServerId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						i.Service,
						Server = s
					})
					.Join(ResourceGroups, i => i.Type.GroupId, r => r.GroupId, (i, r) => new
					{
						i.Item.ItemId,
						i.Item.ItemName,
						i.Item.ItemTypeId,
						i.Type.TypeName,
                        i.Type.DisplayName,
						i.Item.ServiceId,
						i.Item.PackageId,
						i.Package.PackageName,
						i.Service.ServiceName,
						i.Server.ServerId,
						i.Server.ServerName,
						r.GroupName,
						i.User.UserId,
						i.User.Username,
						UserFullName = i.User.FirstName + " " + i.User.LastName,
						i.Item.CreatedDate
					});

				// select item properties, get corresponding item properties
				var properties = ServiceItemProperties
					.Join(items, p => p.ItemId, i => i.ItemId, (p, i) => new
					{
						p.ItemId,
						p.PropertyName,
						p.PropertyValue
					});

				/// <summary>TODO</summary>
				return EntityDataSet(items, properties);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItemsByPackage",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServiceItem(int actorId, int itemId)
		{
			if (UseEntityFramework)
			{
				// select service items
				var items = ServiceItems
					.Where(s => s.ItemId == itemId)
					.Join(Packages, i => i.PackageId, p => p.PackageId, (si, p) => new
					{
						Item = si,
						Package = p
					})
					.Join(ServiceItemTypes, i => i.Item.ItemTypeId, t => t.ItemTypeId, (i, t) => new
					{
						i.Item,
						i.Package,
						Type = t
					})
					.Join(Users, i => i.Package.UserId, u => u.UserId, (i, u) => new
					{
						i.Item,
						i.Package,
						i.Type,
						User = u
					})
					.Join(Services, i => i.Item.ServiceId, s => s.ServiceId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						Service = s
					})
					.Join(Servers, i => i.Service.ServerId, s => s.ServerId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						i.Service,
						Server = s
					})
					.Join(ResourceGroups, i => i.Type.GroupId, r => r.GroupId, (i, r) => new
					{
						i.Item.ItemId,
						i.Item.ItemName,
						i.Item.ItemTypeId,
						i.Type.TypeName,
						i.Item.ServiceId,
						i.Item.PackageId,
						i.Package.PackageName,
						i.Service.ServiceName,
						i.Server.ServerId,
						i.Server.ServerName,
						r.GroupName,
						i.User.UserId,
						i.User.Username,
						UserFullName = i.User.FirstName + " " + i.User.LastName,
						i.Item.CreatedDate
					})
					.AsEnumerable()
					.Where(s => Clone.CheckActorPackageRights(actorId, s.PackageId));

				// select item properties, get corresponding item properties
				var properties = ServiceItemProperties
					.Where(sip => sip.ItemId == itemId)
					.AsEnumerable()
					.Join(items, p => p.ItemId, i => i.ItemId, (p, i) => new
					{
						p.ItemId,
						p.PropertyName,
						p.PropertyValue
					});

				/// <summary>TODO</summary>
				return EntityDataSet(items, properties);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItem",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckServiceItemExists(int serviceId, string itemName, string itemTypeName)
		{
			if (UseEntityFramework)
			{
				var itemTypeId = ServiceItemTypes
					.Where(t => t.TypeName == itemTypeName)
					.Select(t => t.ItemTypeId)
					.FirstOrDefault();

				return ServiceItems.Any(s => s.ServiceId == serviceId && s.ItemName == itemName &&
					s.ItemTypeId == itemTypeId);
			}
			else
			{
				SqlParameter prmExists = new SqlParameter("@Exists", SqlDbType.Bit);
				prmExists.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CheckServiceItemExistsInService",
					prmExists,
					/// <summary>TODO</summary>
					new SqlParameter("@serviceId", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@itemTypeName", itemTypeName));

				return Convert.ToBoolean(prmExists.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckServiceItemExists(string itemName, string groupName, string itemTypeName)
		{
			if (UseEntityFramework)
			{
				var itemTypeId = ServiceItemTypes
					.Where(t => t.TypeName == itemTypeName)
					.Select(t => t.ItemTypeId)
					.FirstOrDefault();

				return ServiceItems
					.Where(s => s.ItemName == itemName && s.ItemTypeId == itemTypeId)
					.Join(Services, i => i.ServiceId, s => s.ServiceId, (i, s) => new
					{
						Item = i,
						Service = s,
					})
					.Join(Providers, i => i.Service.ProviderId, p => p.ProviderId, (i, p) => new
					{
						i.Item,
						i.Service,
						Provider = p
					})
					.Join(ResourceGroups, i => i.Provider.GroupId, r => r.GroupId, (i, r) => new
					{
						i.Item,
						i.Service,
						i.Provider,
						ResourceGroup = r
					})
					.Where(s => groupName == null || s.ResourceGroup.GroupName == groupName)
					.Any();
			}
			else
			{
				SqlParameter prmExists = new SqlParameter("@Exists", SqlDbType.Bit);
				prmExists.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CheckServiceItemExists",
					prmExists,
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@groupName", groupName),
					/// <summary>TODO</summary>
					new SqlParameter("@itemTypeName", itemTypeName));

				return Convert.ToBoolean(prmExists.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServiceItemByName(int actorId, int packageId, string groupName,
			string itemName, string itemTypeName)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				// select service items
				var items = ServiceItems
					.Where(s => s.PackageId == packageId && s.ItemName == itemName)
					.Join(Packages, i => i.PackageId, p => p.PackageId, (si, p) => new
					{
						Item = si,
						Package = p
					})
					.Join(ServiceItemTypes, i => i.Item.ItemTypeId, t => t.ItemTypeId, (i, t) => new
					{
						i.Item,
						i.Package,
						Type = t
					})
					.Where(s => s.Type.TypeName == itemTypeName)
					.Join(Users, i => i.Package.UserId, u => u.UserId, (i, u) => new
					{
						i.Item,
						i.Package,
						i.Type,
						User = u
					})
					.Join(Services, i => i.Item.ServiceId, s => s.ServiceId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						Service = s
					})
					.Join(Servers, i => i.Service.ServerId, s => s.ServerId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						i.Service,
						Server = s
					})
					.Join(ResourceGroups, i => i.Type.GroupId, r => r.GroupId, (i, r) => new
					{
						i.Item.ItemId,
						i.Item.ItemName,
						i.Item.ItemTypeId,
						i.Type.TypeName,
						i.Item.ServiceId,
						i.Item.PackageId,
						i.Package.PackageName,
						i.Service.ServiceName,
						i.Server.ServerId,
						i.Server.ServerName,
						r.GroupName,
						i.User.UserId,
						i.User.Username,
						UserFullName = i.User.FirstName + " " + i.User.LastName,
						i.Item.CreatedDate
					})
					.Where(i => groupName == null || i.GroupName == groupName);

				// select item properties, get corresponding item properties
				var properties = ServiceItemProperties
					.Join(items, p => p.ItemId, i => i.ItemId, (p, i) => new
					{
						p.ItemId,
						p.PropertyName,
						p.PropertyValue
					});

				/// <summary>TODO</summary>
				return EntityDataSet(items, properties);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItemByName",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@itemTypeName", itemTypeName),
					/// <summary>TODO</summary>
					new SqlParameter("@groupName", groupName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetServiceItemsByName(int actorId, int packageId, string itemName)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				// select service items
				var items = ServiceItems
#if NETFRAMEWORK
                    .Where(s => s.PackageId == packageId && DbFunctions.Like(s.ItemName, itemName))
#else
                    .Where(s => s.PackageId == packageId && EF.Functions.Like(s.ItemName, itemName))
#endif
                    .Join(Packages, i => i.PackageId, p => p.PackageId, (si, p) => new
					{
						Item = si,
						Package = p
					})
					.Join(ServiceItemTypes, i => i.Item.ItemTypeId, t => t.ItemTypeId, (i, t) => new
					{
						i.Item,
						i.Package,
						Type = t
					})
					.Join(Users, i => i.Package.UserId, u => u.UserId, (i, u) => new
					{
						i.Item,
						i.Package,
						i.Type,
						User = u
					})
					.Join(Services, i => i.Item.ServiceId, s => s.ServiceId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						Service = s
					})
					.Join(Servers, i => i.Service.ServerId, s => s.ServerId, (i, s) => new
					{
						i.Item,
						i.Package,
						i.Type,
						i.User,
						i.Service,
						Server = s
					})
					.Join(ResourceGroups, i => i.Type.GroupId, r => r.GroupId, (i, r) => new
					{
						i.Item.ItemId,
						i.Item.ItemName,
						i.Item.ItemTypeId,
						i.Type.TypeName,
						i.Item.ServiceId,
						i.Item.PackageId,
						i.Package.PackageName,
						i.Service.ServiceName,
						i.Server.ServerId,
						i.Server.ServerName,
						r.GroupName,
						i.User.UserId,
						i.User.Username,
						UserFullName = i.User.FirstName + " " + i.User.LastName,
						i.Item.CreatedDate
					});

				// select item properties, get corresponding item properties
				var properties = ServiceItemProperties
					.Join(items, p => p.ItemId, i => i.ItemId, (p, i) => new
					{
						p.ItemId,
						p.PropertyName,
						p.PropertyValue
					});

				/// <summary>TODO</summary>
				return EntityDataSet(items, properties);

			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItemsByName",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetServiceItemsCountByNameAndServiceId(int actorId, int serviceId, string groupName,
			string itemName, string itemTypeName)
		{
			if (UseEntityFramework)
			{
				// select service items
				return ServiceItems
					.Where(s => s.ServiceId == serviceId && s.ItemName == itemName)
					.Join(ServiceItemTypes, i => i.ItemTypeId, t => t.ItemTypeId, (i, t) => new
					{
						Item = i,
						Type = t
					})
					.Where(s => s.Type.TypeName == itemTypeName)
					.Join(ResourceGroups, i => i.Type.GroupId, r => r.GroupId, (i, r) => r)
					.Where(r => groupName == null || r.GroupName == groupName)
					.Count();
			}
			else
			{
				int res = 0;

				object obj = SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetServiceItemsCountByNameAndServiceId",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceId", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupName", groupName),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemTypeName", itemTypeName));

				if (!int.TryParse(obj.ToString(), out res)) return -1;

				/// <summary>TODO</summary>
				return res;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddServiceItem(int actorId, int serviceId, int packageId, string itemName,
			string itemTypeName, string xmlProperties)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var groupId = Services
					.Where(s => s.ServiceId == serviceId)
					.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => (int?)p.GroupId)
					.FirstOrDefault();

				var itemTypeId = ServiceItemTypes
					.Where(t => t.TypeName == itemTypeName &&
						(groupId == null || t.GroupId == groupId))
					.Select(t => (int?)t.ItemTypeId)
					.FirstOrDefault();

				using (var transaction = Database.BeginTransaction())
				{
					// Fix to allow plans assigned to serveradmin
					if (itemTypeName == "FuseCP.Providers.HostedSolution.Organization, FuseCP.Providers.Base" && !ServiceItems.Any(s => s.PackageId == 1))
					{
						var serviceItem = new Data.Entities.ServiceItem()
						{
							PackageId = 1,
							ItemTypeId = itemTypeId,
							ServiceId = serviceId,
							ItemName = "System",
							CreatedDate = DateTime.Now
						};
						ServiceItems.Add(serviceItem);
						ExchangeOrganizations.Add(new Data.Entities.ExchangeOrganization()
						{
							Item = serviceItem,
							OrganizationId = "System"
						});
					}

					// add item
					var item = new Data.Entities.ServiceItem()
					{
						PackageId = packageId,
						ItemTypeId = itemTypeId,
						ServiceId = serviceId,
						ItemName = itemName,
						CreatedDate = DateTime.Now
					};
					ServiceItems.Add(item);
					SaveChanges();

					ServiceItemProperties.RemoveRange(
						ServiceItemProperties.Where(p => p.ItemId == item.ItemId));

					var properties = XElement.Parse(xmlProperties)
						.Elements()
						.Select(e => new Data.Entities.ServiceItemProperty
						{
							ItemId = item.ItemId,
							PropertyName = (string)e.Attribute("name"),
							PropertyValue = (string)e.Attribute("value")
						});

					ServiceItemProperties.AddRange(properties);
					SaveChanges();

					transaction.Commit();

					return item.ItemId;
				}
			}
			else
			{
				// add item
				SqlParameter prmItemId = new SqlParameter("@ItemID", SqlDbType.Int);
				prmItemId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddServiceItem",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceID", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemTypeName", itemTypeName),
					/// <summary>TODO</summary>
					new SqlParameter("@xmlProperties", xmlProperties),
					/// <summary>TODO</summary>
					new SqlParameter("@CreatedDate", DateTime.Now),
					prmItemId);

				return Convert.ToInt32(prmItemId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateServiceItem(int actorId, int itemId, string itemName, string xmlProperties)
		{
			if (UseEntityFramework)
			{
				var item = ServiceItems
					.FirstOrDefault(s => s.ItemId == itemId);
				if (item == null)
					throw new InvalidOperationException($"Service item with id '{itemId}' was not found");

				var packageId = item.PackageId;

				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					// update item
					item.ItemName = itemName;

					ServiceItemProperties
						.RemoveRange(ServiceItemProperties
							.Where(p => p.ItemId == itemId));

					var properties = XElement.Parse(xmlProperties)
						.Elements()
						.Select(e => new Data.Entities.ServiceItemProperty
						{
							ItemId = itemId,
							PropertyName = (string)e.Attribute("name"),
							PropertyValue = (string)e.Attribute("value")
						});
					ServiceItemProperties.AddRange(properties);

					SaveChanges();

					transaction.Commit();
				}
			}
			else
			{
				// update item
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateServiceItem",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@XmlProperties", xmlProperties));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteServiceItem(int actorId, int itemId)
		{
			if (UseEntityFramework)
			{
				var packageId = ServiceItems
					.Where(s => s.ItemId == itemId)
					.Select(s => s.PackageId)
					.FirstOrDefault();

				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					Domains
						.Where(d => d.WebSiteId == itemId && d.IsDomainPointer)
						.ExecuteDelete();
					Domains
						.Where(d => d.ZoneItemId == itemId)
						.ExecuteUpdate(d => new Data.Entities.Domain { ZoneItemId = null });
					Domains
						.Where(d => d.WebSiteId == itemId)
						.ExecuteUpdate(d => new Data.Entities.Domain { WebSiteId = null });
					Domains
						.Where(d => d.MailDomainId == itemId)
						.ExecuteUpdate(d => new Data.Entities.Domain { MailDomainId = null });

					// delete item comments
					Comments
						.Where(c => c.ItemId == itemId && c.ItemTypeId == "SERVICE_ITEM")
						.ExecuteDelete();

					// delete item properties
					ServiceItemProperties
						.Where(p => p.ItemId == itemId)
						.ExecuteDelete();

					// delete external IP addresses
					DeleteItemIPAddresses(actorId, itemId);

					// delete item
					ServiceItems
						.Where(s => s.ItemId == itemId)
						.ExecuteDelete();

					SaveChanges();

					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteServiceItem",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void MoveServiceItem(int actorId, int itemId, int destinationServiceId, bool forAutodiscover)
		{
			if (UseEntityFramework)
			{
				var packageId = ServiceItems
					.Where(s => s.ItemId == itemId)
					.Select(s => s.PackageId)
					.FirstOrDefault();

				if (!forAutodiscover && !CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					ServiceItems.Where(s => s.ItemId == itemId)
						.ExecuteUpdate(s => new Data.Entities.ServiceItem()
						{
							ServiceId = destinationServiceId
						});

					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "MoveServiceItem",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@DestinationServiceID", destinationServiceId),
					/// <summary>TODO</summary>
					new SqlParameter("@forAutodiscover", forAutodiscover));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool GetPackageAllocatedResource(int? packageId, int groupId, int? serverId)
		{
			if (packageId == null) return true;

			if (serverId == null || serverId == 0)
			{
				serverId = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => p.ServerId)
					.FirstOrDefault();
			}

			var id = packageId;
			bool groupEnabled = true;

			while (id != null && groupEnabled)
			{
				var package = Packages
					.Where(p => p.PackageId == id)
					.Select(p => new { p.ParentPackageId, p.OverrideQuotas })
					.FirstOrDefault();

				// check if this is a root 'System' package
				if (package?.ParentPackageId == null)
				{
					if (serverId == -1 || serverId == null) return true;

					if (Servers.Any(s => s.ServerId == serverId && s.VirtualServer))
					{
						if (!VirtualServices
							.Where(v => v.ServerId == serverId)
							.Join(Services, v => v.ServiceId, s => s.ServiceId, (v, s) => s)
							.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => p)
							.Any(p => p.GroupId == groupId))
						{
							groupEnabled = false;
						}
					}
					else
					{
						if (!Services
							.Where(s => s.ServerId == serverId)
							.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => p)
							.Any(p => p.GroupId == groupId))
						{
							groupEnabled = false;
						}
					}

					/// <summary>TODO</summary>
					return groupEnabled;
				}
				else // parentPackageId != null
				{
					// check the current package
					if (package.OverrideQuotas)
					{
						if (!PackageResources.Any(r => r.GroupId == groupId && r.PackageId == id))
						{
							groupEnabled = false;
						}
					}
					else
					{
						if (!Packages
							.Where(p => p.PackageId == id)
							.Join(HostingPlanResources, p => p.PlanId, r => r.PlanId, (p, r) => r)
							.Any(r => r.GroupId == groupId))
						{
							groupEnabled = false;
						}
					}

					// check addons
					if (PackageAddons
						.Where(p => p.PackageId == id && p.StatusId == 1)
						.Join(HostingPlanResources, p => p.PlanId, r => r.PlanId, (p, r) => r)
						.Any(r => r.GroupId == groupId))
					{
						groupEnabled = true;
					}
				}

				id = package?.ParentPackageId;
			}

			/// <summary>TODO</summary>
			return false;
		}
		/// <summary>Auto-generated member.</summary>

		public int GetPackageServiceId(int actorId, int packageId, string groupName, bool updatePackage)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				int serviceId = 0;
				//optimized run when we don't need any changes
				if (!updatePackage)
				{
					serviceId = PackageServices
						.Where(ps => ps.PackageId == packageId)
						.Join(Services, ps => ps.ServiceId, s => s.ServiceId, (ps, s) => s)
						.Join(ResourceGroups.Where(rg => rg.GroupName == groupName),
							s => s.Provider.GroupId, rg => rg.GroupId, (s, rg) => s.ServiceId)
						.FirstOrDefault();
					/// <summary>TODO</summary>
					return serviceId;
				}

				var groupId = ResourceGroups
					.Where(g => g.GroupName == groupName)
					.Select(g => g.GroupId)
					.FirstOrDefault();

				// check if user has this resource enabled
				if (!GetPackageAllocatedResource(packageId, groupId, null))
				{
					// remove all resource services from the space
					PackageServices
						.Where(ps => ps.PackageId == packageId)
						.Join(Services, ps => ps.ServiceId, s => s.ServiceId, (ps, s) => new
						{
							PackageService = ps,
							Service = s
						})
						.Join(Providers.Where(p => p.GroupId == groupId),
							ps => ps.Service.ProviderId, p => p.ProviderId, (ps, p) => ps.PackageService)
						.ExecuteDelete();
				}

				// check if the service is already distributed
				var serviceIdQuery = PackageServices
					.Where(ps => ps.PackageId == packageId)
					.Join(Services, ps => ps.ServiceId, s => s.ServiceId, (ps, s) => s)
					.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => new
					{
						Service = s,
						Provider = p
					})
					.Where(g => g.Provider.GroupId == groupId)
					.Select(g => g.Service.ServiceId);

				serviceId = serviceIdQuery
					.FirstOrDefault();

				if (serviceId != 0) return serviceId;

				// distribute services
				DistributePackageServices(actorId, packageId);

				// get distributed service again
				serviceId = serviceIdQuery
					.FirstOrDefault();

				/// <summary>TODO</summary>
				return serviceId;
			}
			else
			{
				SqlParameter prmServiceId = new SqlParameter("@ServiceID", SqlDbType.Int);
				prmServiceId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackageServiceID",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@groupName", groupName),
					/// <summary>TODO</summary>
					new SqlParameter("@UpdatePackage", updatePackage),
					prmServiceId);

				return Convert.ToInt32(prmServiceId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public string GetMailFilterURL(int actorId, int packageId, string groupName)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var groupId = ResourceGroups
					.Where(g => g.GroupName == groupName)
					.Select(g => g.GroupId)
					.FirstOrDefault();
				var package = Packages
					.Where(p => p.PackageId == packageId);
				var serviceId = package
					.Join(PackageServices, p => p.PackageId, ps => ps.PackageId, (p, ps) => ps.ServiceId)
					.Join(Services, ps => ps, s => s.ServiceId, (ps, s) => s)
					.Join(Providers, s => s.ProviderId, p => p.ProviderId, (s, p) => new
					{
						s.ServiceId,
						p.GroupId
					})
					.Where(s => s.GroupId == groupId)
					.Select(s => s.ServiceId)
					.FirstOrDefault();
				var filterUrl = ServiceProperties
					.Where(p => p.ServiceId == serviceId && p.PropertyName == "apiurl")
					.Select(p => p.PropertyValue)
					.FirstOrDefault();
				/// <summary>TODO</summary>
				return filterUrl;
			}
			else
			{
				SqlParameter prmFilterUrl = new SqlParameter("@FilterUrl", SqlDbType.NVarChar, 200);
				prmFilterUrl.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetFilterURL",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupName", groupName),
					prmFilterUrl);

				return Convert.ToString(prmFilterUrl.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public string GetMailFilterUrlByHostingPlan(int actorId, int PlanID, string groupName)
		{
			if (UseEntityFramework)
			{
				// load ServerID info
				var serverId = HostingPlans
					.Where(p => p.PlanId == PlanID)
					.Select(p => p.ServerId)
					.FirstOrDefault();
				var isVirtualServer = Servers
					.Where(s => s.ServerId == serverId)
					.Select(s => s.VirtualServer)
					.FirstOrDefault();
				var providerId = ResourceGroups
					.Where(g => g.GroupName == groupName)
					.Join(Providers, r => r.GroupId, p => p.GroupId, (r, p) => p)
					.Where(p => p.ProviderName == "MailCleaner")
					.Select(p => p.ProviderId)
					.FirstOrDefault();
				/// <summary>TODO</summary>
				var serviceId = isVirtualServer
					? Services
						.Join(VirtualServices, s => s.ServiceId, v => v.ServiceId, (s, v) => new
						{
							Service = s,
							VirtualService = v
						})
						.Where(s => s.VirtualService.ServerId == serverId && s.Service.ProviderId == providerId)
						.Select(s => s.Service.ServiceId)
						.FirstOrDefault()
					: Services
						.Where(s => s.ServerId == serverId && s.ProviderId == providerId)
						.Select(s => s.ServiceId)
						.FirstOrDefault();
				var filterUrl = ServiceProperties
					.Where(p => p.ServiceId == serviceId && p.PropertyName == "apiurl")
					.Select(p => p.PropertyValue)
					.FirstOrDefault();
				/// <summary>TODO</summary>
				return filterUrl;
			}
			else
			{
				SqlParameter prmFilterUrl = new SqlParameter("@FilterUrl", SqlDbType.NVarChar, 200);
				prmFilterUrl.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetFilterURLByHostingPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PlanID", PlanID),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupName", groupName),
					prmFilterUrl);

				return Convert.ToString(prmFilterUrl.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>


		public void UpdatePackageDiskSpace(int packageId, string xml)
		{
			if (UseEntityFramework)
			{
				// remove current diskspace
				PackagesDiskspaces.Where(d => d.PackageId == packageId).ExecuteDelete();

				var items = XElement.Parse(xml)
					.Elements()
					.Select(e => new
					{
						ItemId = (int)e.Attribute("id"),
						Bytes = (long)e.Attribute("bytes")
					})
					.ToList();
				using (var tempIds = items.Select(item => item.ItemId).ToTempIdSet(this))
				{
					var diskspace = tempIds
						.Join(ServiceItems, id => id, s => s.ItemId, (it, s) => s)
						.Join(ServiceItemTypes, s => s.ItemTypeId, t => t.ItemTypeId, (s, t) => new
						{
							s.ItemId,
							t.GroupId
						})
						.Where(s => s.GroupId != null)
						.AsEnumerable()
						.Join(items, s => s.ItemId, item => item.ItemId, (s, item) => new
						{
							GroupId = s.GroupId.Value,
							item.Bytes
						})
						.GroupBy(s => s.GroupId)
						.Select(g => new Data.Entities.PackagesDiskspace
						{
							PackageId = packageId,
							GroupId = g.Key,
							DiskSpace = g.Sum(s => s.Bytes)
						});
					PackagesDiskspaces.AddRange(diskspace);
					SaveChanges();
				}
			}
			else
			{
				ExecuteLongNonQuery(
					ObjectQualifier + "UpdatePackageDiskSpace",
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@xml", xml));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdatePackageBandwidth(int packageId, string xml)
		{
			if (UseEntityFramework)
			{
				var items = XElement.Parse(xml)
					.Elements()
					.Select(e => new
					{
						ItemId = (int)e.Attribute("id"),
						LogDate = DateTime.Parse((string)e.Attribute("date")),
						BytesSent = (long)e.Attribute("sent"),
						BytesReceived = (long)e.Attribute("received")
					})
					.ToList();

				using (var itemIds = items.Select(item => item.ItemId).ToTempIdSet(this))
				{
					// delete current statistics
					var groupedItems = itemIds
						.Join(ServiceItems, i => i, s => s.ItemId, (i, s) => s)
						.Join(ServiceItemTypes, i => i.ItemTypeId, t => t.ItemTypeId, (i, t) => new
						{
							ItemId = i.ItemId,
							GroupId = t.GroupId
						})
						.Where(s => s.GroupId != null)
						.AsEnumerable()
						.Join(items, s => s.ItemId, i => i.ItemId, (s, i) => new
						{
							GroupId = s.GroupId.Value,
							i.LogDate,
							i.BytesSent,
							i.BytesReceived
						})
						.GroupBy(i => new DatedId { Date = i.LogDate, Id = i.GroupId });

					using (var tempKeys = groupedItems.Select(g => g.Key).ToTempDatedIdSet(this))
					{
						var bandwidthsToDelete = PackagesBandwidths
							.Where(pb => pb.PackageId == packageId)
							.Join(tempKeys,
								pb => new DatedId { Date = pb.LogDate, Id = pb.GroupId }, k => k, (pb, k) => pb);

						using (var transaction = Database.BeginTransaction())
						{
							bandwidthsToDelete.ExecuteDelete();

							// insert new statistics
							var newBandwidths = groupedItems
								.AsEnumerable()
								.Select(item => new Data.Entities.PackagesBandwidth
								{
									PackageId = packageId,
									GroupId = item.Key.Id,
									LogDate = item.Key.Date,
									BytesSent = item.Sum(x => (long?)x.BytesSent) ?? 0,
									BytesReceived = item.Sum(x => (long?)x.BytesReceived) ?? 0
								});
							PackagesBandwidths.AddRange(newBandwidths);

							SaveChanges();

							transaction.Commit();
						}
					}
				}
			}
			else
			{
				ExecuteLongNonQuery(
					ObjectQualifier + "UpdatePackageBandwidth",
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@xml", xml));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DateTime GetPackageBandwidthUpdate(int packageId)
		{
			if (UseEntityFramework)
			{
				return Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => p.BandwidthUpdated)
					.FirstOrDefault() ??
					default(DateTime);
			}
			else
			{
				SqlParameter prmUpdateDate = new SqlParameter("@UpdateDate", SqlDbType.DateTime);
				prmUpdateDate.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackageBandwidthUpdate",
					prmUpdateDate,
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId));

				return (prmUpdateDate.Value != DBNull.Value) ? Convert.ToDateTime(prmUpdateDate.Value) : DateTime.MinValue;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdatePackageBandwidthUpdate(int packageId, DateTime updateDate)
		{
			if (UseEntityFramework)
			{
                Packages.Where(p => p.PackageId == packageId).ExecuteUpdate(p => new Data.Entities.Package() { BandwidthUpdated = updateDate });
            }
            else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdatePackageBandwidthUpdate",
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@updateDate", updateDate));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetServiceItemType(int itemTypeId)
		{
			if (UseEntityFramework)
			{
				var type = ServiceItemTypes
					.Where(t => t.ItemTypeId == itemTypeId);
				/// <summary>TODO</summary>
				return EntityDataReader(type);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetServiceItemType",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemTypeID", itemTypeId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetServiceItemTypes()
		{
			if (UseEntityFramework)
			{
				var types = ServiceItemTypes
					.OrderBy(t => t.TypeOrder);
				/// <summary>TODO</summary>
				return EntityDataReader(types);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetServiceItemTypes");
			}
		}
		#endregion

		#region Plans
		// Plans methods
		/// <summary>Auto-generated member.</summary>
		public DataSet GetHostingPlans(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var plans = HostingPlans
					.Where(pl => pl.UserId == userId && pl.IsAddon == false)
					// we have to do a GroupJoin here since hp.Packages is not related to hp.PackageId
					.GroupJoin(Packages, hp => hp.PackageId, p => p.PackageId, (hp, ps) => new
					{
						Plan = hp,
						Packages = ps
					})
					.SelectMany(pl => pl.Packages.DefaultIfEmpty(), (pl, p) => new
					{
						pl.Plan.PlanId,
						pl.Plan.UserId,
						pl.Plan.PackageId,
						pl.Plan.PlanName,
						pl.Plan.PlanDescription,
						pl.Plan.Available,
						pl.Plan.SetupPrice,
						pl.Plan.RecurringPrice,
						pl.Plan.RecurrenceLength,
						pl.Plan.RecurrenceUnit,
						pl.Plan.IsAddon,
						PackagesNumber = pl.Plan.Packages.Count(),
						// server
						ServerId = pl.Plan.ServerId != null ? pl.Plan.ServerId : 0,
						ServerName = pl.Plan.Server != null ? pl.Plan.Server.ServerName : "None",
						ServerComments = pl.Plan.Server != null ? pl.Plan.Server.Comments : "",
						VirtualServer = pl.Plan.Server == null || pl.Plan.Server.VirtualServer,
						// package
						PackageName = p != null ? p.PackageName : "None"
					})
					.OrderBy(pl => pl.PlanName);
				/// <summary>TODO</summary>
				return EntityDataSet(plans);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
				ObjectQualifier + "GetHostingPlans",
				/// <summary>TODO</summary>
				new SqlParameter("@actorId", actorId),
				/// <summary>TODO</summary>
				new SqlParameter("@userId", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetHostingAddons(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var plans = HostingPlans
					.Where(pl => pl.UserId == userId && pl.IsAddon == true)
					.OrderBy(pl => pl.PlanName)
					.Select(pl => new
					{
						pl.PlanId,
						pl.UserId,
						pl.PackageId,
						pl.PlanName,
						pl.PlanDescription,
						pl.Available,
						pl.SetupPrice,
						pl.RecurringPrice,
						pl.RecurrenceLength,
						pl.RecurrenceUnit,
						pl.IsAddon,
						PackagesNumber = pl.Packages.Count(),
					});
				/// <summary>TODO</summary>
				return EntityDataSet(plans);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetHostingAddons",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetUserAvailableHostingPlans(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var ownerId = Users
					.Where(u => u.UserId == userId)
					.Select(u => u.OwnerId)
					.FirstOrDefault();

				var plans = HostingPlans
					.Where(pl => pl.UserId == ownerId && pl.IsAddon == false)
					.OrderBy(pl => pl.PlanName)
					.Select(pl => new
					{
						pl.PlanId,
						pl.PackageId,
						pl.PlanName,
						pl.PlanDescription,
						pl.Available,
						pl.ServerId,
						pl.SetupPrice,
						pl.RecurringPrice,
						pl.RecurrenceLength,
						pl.RecurrenceUnit,
						pl.IsAddon
					});
				/// <summary>TODO</summary>
				return EntityDataSet(plans);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetUserAvailableHostingPlans",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetUserAvailableHostingAddons(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var ownerId = Users
					.Where(u => u.UserId == userId)
					.Select(u => u.OwnerId)
					.FirstOrDefault();

				var plans = HostingPlans
					.Where(pl => pl.UserId == ownerId && pl.IsAddon == true)
					.OrderBy(pl => pl.PlanName)
					.Select(pl => new
					{
						pl.PlanId,
						pl.PackageId,
						pl.PlanName,
						pl.PlanDescription,
						pl.Available,
						pl.ServerId,
						pl.SetupPrice,
						pl.RecurringPrice,
						pl.RecurrenceLength,
						pl.RecurrenceUnit,
						pl.IsAddon
					});
				/// <summary>TODO</summary>
				return EntityDataSet(plans);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetUserAvailableHostingAddons",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetHostingPlan(int actorId, int planId)
		{
			if (UseEntityFramework)
			{
				var plans = HostingPlans
					.Where(p => p.PlanId == planId)
					.Select(p => new
					{
						p.PlanId,
						p.UserId,
						p.PackageId,
						p.ServerId,
						p.PlanName,
						p.PlanDescription,
						p.Available,
						p.SetupPrice,
						p.RecurringPrice,
						p.RecurrenceLength,
						p.RecurrenceUnit,
						p.IsAddon
					});
				/// <summary>TODO</summary>
				return EntityDataReader(plans);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetHostingPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PlanId", planId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckActorParentPackageRights(int actorId, int? packageId)
		{
			if (actorId == -1 || packageId == null) return true;

			// get package owner
			var userId = Packages
				.Where(p => p.PackageId == packageId)
				.Select(p => (int?)p.UserId)
				.FirstOrDefault();
			if (userId == null) return true;

			// check user
			/// <summary>TODO</summary>
			return CanGetUserDetails(actorId, userId ?? -1);
		}
		/// <summary>Auto-generated member.</summary>

		public bool GetPackageServiceLevelResource(int? packageId, int groupId, int? serverId)
		{
			if (!ResourceGroups.Any(g => g.GroupId == groupId && g.GroupName == "Service Levels")) return false;

			if (packageId == null) return true;

			int? pid = packageId;
			var package = Packages
				.Where(p => p.PackageId == pid)
				.Select(p => new
				{
					p.ServerId,
					p.ParentPackageId,
					p.OverrideQuotas
				})
				.FirstOrDefault();

			if (serverId == null || serverId == 0) serverId = package?.ServerId;

			bool groupEnabled = true;
			while (groupEnabled)
			{
				// check if this is a root "System" package
				if (package?.ParentPackageId == null)
				{
					return serverId != 0 && (pid == -1 || serverId == null || serverId > 0);
				}
				else
				{
					// check the current package
					if (package.OverrideQuotas)
					{
						if (!PackageResources.Any(r => r.GroupId == groupId && r.PackageId == pid))
						{
							groupEnabled = false;
						}
					}
					else
					{
						if (!Packages
							.Where(p => p.PackageId == pid)
							.Join(HostingPlanResources, p => p.PlanId, hr => hr.PlanId, (p, hr) => hr)
							.Any(hr => hr.GroupId == groupId))
						{
							groupEnabled = false;
						}
					}

					// check addons
					if (PackageAddons
						.Where(a => a.PackageId == pid && a.StatusId == 1)
						.Join(HostingPlanResources, a => a.PlanId, hr => hr.PlanId, (a, hr) => hr)
						.Any(hr => hr.GroupId == groupId))
					{
						groupEnabled = true;
					}
				}

				pid = package?.ParentPackageId;
				package = Packages
					.Where(p => p.PackageId == pid)
					.Select(p => new
					{
						p.ServerId,
						p.ParentPackageId,
						p.OverrideQuotas
					})
					.FirstOrDefault();
			}

			/// <summary>TODO</summary>
			return false;
		}
		/// <summary>Auto-generated member.</summary>

		public int GetPackageAllocatedQuota(int? packageId, int quotaId)
		{
			/// <summary>TODO</summary>
			int result;

			var quotaTypeId = Quotas
				.Where(q => q.QuotaId == quotaId)
				.Select(q => q.QuotaTypeId)
				.FirstOrDefault();

			result = quotaTypeId == 1 ? 1 : -1; // enabled/unlimited

			int? pid = packageId;

			while (pid != null)
			{
				var package = Packages
					.Where(p => p.PackageId == pid)
					.Select(p => new
					{
						p.ParentPackageId,
						p.OverrideQuotas
					})
					.FirstOrDefault();

				int? quotaValue = null;

				// check if this is a root 'System' package
				if (package?.ParentPackageId == null)
				{
					if (quotaTypeId == 1) // boolean
						quotaValue = 1; // enabled
					/// <summary>TODO</summary>
					else if (quotaTypeId > 1) // numeric
						quotaValue = -1; // unlimited
				}
				else
				{
					// check the current package
					quotaValue = package.OverrideQuotas
						? PackageQuotas
							.Where(q => q.QuotaId == quotaId && q.PackageId == pid)
							.Select(q => q.QuotaValue)
							.FirstOrDefault()
						: Packages
							.Where(p => p.PackageId == pid)
							.Join(HostingPlanQuotas, p => p.PlanId, hq => hq.PlanId, (p, hq) => hq)
							.Where(q => q.QuotaId == quotaId)
							.Select(q => q.QuotaValue)
							.FirstOrDefault();

					if (quotaValue == null) quotaValue = 0;

					// check package addons
					int? quotaAddonValue = null;
					quotaAddonValue = PackageAddons
						.Where(p => p.PackageId == pid && p.StatusId == 1 /* active */)
						.Join(HostingPlanQuotas, a => a.PlanId, hq => hq.PlanId, (a, hq) => new
						{
							Addon = a,
							Quota = hq
						})
						.Where(q => q.Quota.QuotaId == quotaId)
						.Sum(q => (int?)(q.Quota.QuotaValue * q.Addon.Quantity ?? 0));

					// process bool quota
					if (quotaAddonValue != null)
					{
						if (quotaTypeId == 1)
						{
							if (quotaAddonValue > 0 && quotaValue == 0 /* enabled */) quotaValue = 1;
						}
						else // numeric quota
						{
							if (quotaAddonValue < 0) // unlimited
							{
								quotaValue = -1;
							}
							else
							{
								quotaValue += quotaAddonValue;
							}
						}
					}
				}

				// process bool quota
				if (quotaTypeId == 1)
				{
					if (quotaValue == 0 || quotaValue == null) return 0; // disabled
				}
				else // numeric quota
				{
					if (quotaValue == 0 || quotaValue == null) return 0; // zero quantity

					if (quotaValue != -1 && (result == -1 || quotaValue < result)) result = quotaValue.Value;
				}

				pid = package?.ParentPackageId;
			}

			/// <summary>TODO</summary>
			return result;
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetHostingPlanQuotas(int actorId, int packageId, int planId, int serverId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorParentPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				if (serverId == 0)
				{
					serverId = Packages
						.Where(p => p.PackageId == packageId)
						.Select(p => p.ServerId)
						.FirstOrDefault() ?? 0;
				}

				// get resource groups
				var groups = ResourceGroups
					.Where(r => r.ShowGroup == true)
					.GroupJoin(HostingPlanResources,
						g => new { g.GroupId, PlanId = planId },
						hr => new { hr.GroupId, hr.PlanId }, (g, hr) => new
						{
							Group = g,
							HostingPlans = hr
						})
					.SelectMany(g => g.HostingPlans.DefaultIfEmpty(), (g, plan) => new
					{
						g.Group.GroupId,
						g.Group.GroupName,
						g.Group.GroupOrder,
						Enabled = plan != null,
						CalculateDiskSpace = plan != null ? plan.CalculateDiskSpace : true,
						CalculateBandwidth = plan != null ? plan.CalculateBandwidth : true
					})
					.OrderBy(g => g.GroupOrder)
					.ThenBy(g => g.GroupName)
					.ToList()
					.Select(g => new
					{
						g.GroupId,
						g.GroupName,
						g.Enabled,
						ParentEnabled = g.GroupName == "Service Levels" ?
							GetPackageServiceLevelResource(packageId, g.GroupId, serverId) :
							GetPackageAllocatedResource(packageId, g.GroupId, serverId),
						g.CalculateDiskSpace,
						g.CalculateBandwidth
					});

				// get quotas by groups
				var quotas = Quotas
					.Where(q => q.HideQuota != true)
					.GroupJoin(HostingPlanQuotas,
						q => new { q.QuotaId, PlanId = planId },
						hq => new { hq.QuotaId, hq.PlanId }, (q, hq) => new
						{
							Quota = q,
							HostingPlanQuotas = hq
						})
					.SelectMany(q => q.HostingPlanQuotas.DefaultIfEmpty(), (q, hq) => new
					{
						q.Quota.QuotaId,
						q.Quota.GroupId,
						q.Quota.QuotaName,
						q.Quota.QuotaDescription,
						q.Quota.QuotaTypeId,
						q.Quota.QuotaOrder,
						QuotaValue = hq != null ? hq.QuotaValue : 0
					})
					.OrderBy(q => q.QuotaOrder)
					.ToList()
					.Select(q => new
					{
						q.QuotaId,
						q.GroupId,
						q.QuotaName,
						q.QuotaDescription,
						q.QuotaTypeId,
						q.QuotaValue,
						ParentQuotaValue = GetPackageAllocatedQuota(packageId, q.QuotaId),
					});

				/// <summary>TODO</summary>
				return EntityDataSet(groups, quotas);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetHostingPlanQuotas",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@planId", planId),
					/// <summary>TODO</summary>
					new SqlParameter("@serverId", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateHostingPlanQuotas(int actorId, int planId, string quotasXml)
		{
			var userId = HostingPlans
				.Where(p => p.PlanId == planId)
				.Select(p => p.UserId)
				.FirstOrDefault();

			// check rights
			if (!CheckActorUserRights(actorId, userId))
				/// <summary>TODO</summary>
				throw new AccessViolationException("You are not allowed to access this account");

			var xml = XElement.Parse(quotasXml);
			var groups = xml.Element("groups")
				.Elements()
				.Select(e => new Data.Entities.HostingPlanResource
				{
					PlanId = planId,
					GroupId = (int)e.Attribute("id"),
					CalculateDiskSpace = (int)e.Attribute("calculateDiskSpace") == 1,
					CalculateBandwidth = (int)e.Attribute("calculateBandwidth") == 1
				});
			var quotas = xml.Element("quotas")
				.Elements()
				.Select(e => new Data.Entities.HostingPlanQuota
				{
					PlanId = planId,
					QuotaId = (int)e.Attribute("id"),
					QuotaValue = (int)e.Attribute("value")
				});

			// delete old HP resources
			HostingPlanResources.Where(r => r.PlanId == planId).ExecuteDelete();

			// delete old HP quotas
			HostingPlanQuotas.Where(q => q.PlanId == planId).ExecuteDelete();

			HostingPlanResources.AddRange(groups);
			HostingPlanQuotas.AddRange(quotas);

			SaveChanges();
		}
		/// <summary>Auto-generated member.</summary>
		public int AddHostingPlan(int actorId, int userId, int packageId, string planName,
			string planDescription, bool available, int serverId, decimal setupPrice, decimal recurringPrice,
			int recurrenceUnit, int recurrenceLength, bool isAddon, string quotasXml)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				if (serverId == 0)
				{
					serverId = Packages
						.Where(p => p.PackageId == packageId)
						.Select(p => p.ServerId)
						.FirstOrDefault() ?? 0;
				}

				using (var transaction = Database.BeginTransaction())
				{
					var plan = new Data.Entities.HostingPlan()
					{
						UserId = userId,
						PackageId = packageId != 0 ? packageId : null,
						PlanName = planName,
						PlanDescription = planDescription,
						Available = available,
						ServerId = serverId != 0 && !isAddon ? serverId : null,
						SetupPrice = setupPrice,
						RecurringPrice = recurringPrice,
						RecurrenceLength = recurrenceLength,
						RecurrenceUnit = recurrenceUnit,
						IsAddon = isAddon
					};
					HostingPlans.Add(plan);
					SaveChanges();

					// save quotas
					UpdateHostingPlanQuotas(actorId, plan.PlanId, quotasXml);

					transaction.Commit();

					return plan.PlanId;
				}
			}
			else
			{
				SqlParameter prmPlanId = new SqlParameter("@PlanID", SqlDbType.Int);
				prmPlanId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddHostingPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@planName", planName),
					/// <summary>TODO</summary>
					new SqlParameter("@planDescription", planDescription),
					/// <summary>TODO</summary>
					new SqlParameter("@available", available),
					/// <summary>TODO</summary>
					new SqlParameter("@serverId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@setupPrice", setupPrice),
					/// <summary>TODO</summary>
					new SqlParameter("@recurringPrice", recurringPrice),
					/// <summary>TODO</summary>
					new SqlParameter("@recurrenceUnit", recurrenceUnit),
					/// <summary>TODO</summary>
					new SqlParameter("@recurrenceLength", recurrenceLength),
					/// <summary>TODO</summary>
					new SqlParameter("@isAddon", isAddon),
					/// <summary>TODO</summary>
					new SqlParameter("@quotasXml", quotasXml),
					prmPlanId);

				// read identity
				return Convert.ToInt32(prmPlanId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int CheckExceedingQuota(int? packageId, int quotaId, int quotaTypeId)
		{
			if (packageId == null) return 0;

			var packageQuotaValue = GetPackageAllocatedQuota(packageId, quotaId);

			// check boolean quota
			if (quotaTypeId == 1) return 0; // && packageQuotaValue > 0 // enabled, can exceed

			// check numeric quota
			if (quotaTypeId == 2 && packageQuotaValue == -1) return 0; // unlimited, can exceed

			/// <summary>TODO</summary>
			int usedQuantity, usedPlans, usedOverrides, usedAddons;

			// limited by hosting plans
			usedPlans = Packages
				.Where(p => p.ParentPackageId == packageId && !p.OverrideQuotas)
				.Join(HostingPlanQuotas, p => p.PlanId, hq => hq.PlanId, (p, hq) => hq)
				.Where(hq => hq.QuotaId == quotaId)
				.Sum(hq => (int?)hq.QuotaValue) ?? 0;

			// overrides
			usedOverrides = Packages
				.Where(p => p.ParentPackageId == packageId && p.OverrideQuotas)
				.Join(PackageQuotas.Where(pq => pq.QuotaId == quotaId), p => p.PackageId, pq => pq.PackageId, (p, pq) => pq)
				.Sum(pq => (int?)pq.QuotaValue) ?? 0;

			// addons
			usedAddons = Packages
				.Where(p => p.ParentPackageId == packageId)
				.Join(PackageAddons, p => p.PackageId, pa => pa.PackageId, (p, pa) => new
				{
					Package = p,
					Addon = pa
				})
				.Join(HostingPlanQuotas, p => p.Addon.PlanId, hq => hq.PlanId, (p, hq) => new
				{
					hq.QuotaId,
					p.Addon.StatusId,
					hq.QuotaValue,
					p.Addon.Quantity
				})
				.Where(p => p.QuotaId == quotaId && p.StatusId == 1 /* active */)
				.Sum(p => (p.QuotaValue * p.Quantity)) ?? 0;

			/*
			usedQuantity = Packages
				.Where(p => p.ParentPackageId == packageId)
				.Sum(p => (int?)GetPackageAllocatedQuota(p.PackageId, quotaId)) ?? 0;
			*/

			usedQuantity = usedPlans + usedOverrides + usedAddons;

			if (usedQuantity == 0) return 0; // can exceed

			return usedQuantity - packageQuotaValue;
		}
		/// <summary>Auto-generated member.</summary>

		public class ExceedingQuota
		{
			/// <summary>Auto-generated member.</summary>
			public int QuotaId { get; set; }
			/// <summary>Auto-generated member.</summary>
			public string QuotaName { get; set; }
			/// <summary>Auto-generated member.</summary>
			public int QuotaValue { get; set; }
		}
		/// <summary>Auto-generated member.</summary>

		public IEnumerable<ExceedingQuota> GetPackageExceedingQuotas(int? packageId)
		{
			var package = Packages
				.Where(p => p.PackageId == packageId)
				.Select(p => new { p.PlanId, p.ParentPackageId, p.OverrideQuotas })
				.FirstOrDefault();
			if (package?.ParentPackageId != null) // not root package
			{
				if (!package.OverrideQuotas) // hosting plan quotas
				{
					var quotas = HostingPlanQuotas
						.Where(q => q.PlanId == package.PlanId)
						.Join(Quotas.Where(q => q.QuotaTypeId != 3), hq => hq.QuotaId, q => q.QuotaId, (hq, q) => q)
						.Select(q => new
						{
							q.QuotaId,
							q.QuotaName,
							q.QuotaTypeId
						})
						.ToList();
					return quotas
						.Select(q => new ExceedingQuota()
						{
							QuotaId = q.QuotaId,
							QuotaName = q.QuotaName,
							QuotaValue = CheckExceedingQuota(packageId, q.QuotaId, q.QuotaTypeId)
						});
				}
				else // overriden quotas
				{
					var quotas = PackageQuotas
						.Where(q => q.PackageId == packageId)
						.Join(Quotas.Where(q => q.QuotaTypeId != 3), hq => hq.QuotaId, q => q.QuotaId, (hq, q) => q)
						.Select(q => new
						{
							q.QuotaId,
							q.QuotaName,
							q.QuotaTypeId
						})
						.ToList();
					return quotas
						.Select(q => new ExceedingQuota
						{
							QuotaId = q.QuotaId,
							QuotaName = q.QuotaName,
							QuotaValue = CheckExceedingQuota(packageId, q.QuotaId, q.QuotaTypeId)
						});
				}
			}
			return Enumerable.Empty<ExceedingQuota>();
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet UpdateHostingPlan(int actorId, int planId, int packageId, int serverId, string planName,
			string planDescription, bool available, decimal setupPrice, decimal recurringPrice,
			int recurrenceUnit, int recurrenceLength, string quotasXml)
		{
			if (UseEntityFramework)
			{
				// check rights
				var userId = HostingPlans
					.Where(p => p.PlanId == planId)
					.Select(p => p.UserId)
					.FirstOrDefault();

				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				if (serverId == 0)
				{
					serverId = Packages
						.Where(p => p.PackageId == packageId)
						.Select(p => p.ServerId)
						.FirstOrDefault() ?? 0;
				}

				// update record
				var plan = HostingPlans
					.FirstOrDefault(p => p.PlanId == planId);
                if (plan != null)
                {
                    plan.PackageId = packageId;
                    plan.ServerId = serverId;
                    plan.PlanName = planName;
                    plan.PlanDescription = planDescription;
                    plan.Available = available;
                    plan.SetupPrice = setupPrice;
                    plan.RecurringPrice = recurringPrice;
                    plan.RecurrenceLength = recurrenceLength;
                    plan.RecurrenceUnit = recurrenceUnit;
                    SaveChanges();
                }

                using (var transaction = Database.BeginTransaction())
				{
					// update quotas
					UpdateHostingPlanQuotas(actorId, planId, quotasXml);

					var exceedingQuotas = GetPackageExceedingQuotas(packageId)
						.Where(q => q.QuotaValue > 0)
						.ToList();

					if (exceedingQuotas.Any()) transaction.Rollback();
					else transaction.Commit();

					/// <summary>TODO</summary>
					return EntityDataSet(exceedingQuotas);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateHostingPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@planId", planId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@serverId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@planName", planName),
					/// <summary>TODO</summary>
					new SqlParameter("@planDescription", planDescription),
					/// <summary>TODO</summary>
					new SqlParameter("@available", available),
					/// <summary>TODO</summary>
					new SqlParameter("@setupPrice", setupPrice),
					/// <summary>TODO</summary>
					new SqlParameter("@recurringPrice", recurringPrice),
					/// <summary>TODO</summary>
					new SqlParameter("@recurrenceUnit", recurrenceUnit),
					/// <summary>TODO</summary>
					new SqlParameter("@recurrenceLength", recurrenceLength),
					/// <summary>TODO</summary>
					new SqlParameter("@quotasXml", quotasXml));
			}
		}

		/*
		// TODO: This method is missing from the stored procedures
		/// <summary>Auto-generated member.</summary>
		public int CopyHostingPlan(int planId, int userId, int packageId)
		{
			if (UseEntityFramework)
			{
			}
			else
			{
				SqlParameter prmPlanId = new SqlParameter("@DestinationPlanID", SqlDbType.Int);
				prmPlanId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CopyHostingPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@SourcePlanID", planId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					prmPlanId);

				return Convert.ToInt32(prmPlanId.Value);
			}
		} */
		/// <summary>Auto-generated member.</summary>

		public int DeleteHostingPlan(int actorId, int planId)
		{
			if (UseEntityFramework)
			{
				// check rights
				var packageId = HostingPlans
					.Where(p => p.PlanId == planId)
					.Select(p => p.PackageId)
					.FirstOrDefault();
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				// check if some packages uses this plan
				if (Packages.Any(p => p.PlanId == planId)) return -1;

				// check if some package addons uses this plan
				if (PackageAddons.Any(a => a.PlanId == planId)) return -2;

				// delete hosting plan
				HostingPlans.Where(p => p.PlanId == planId).ExecuteDelete();

				return 0;
			}
			else
			{
				SqlParameter prmResult = new SqlParameter("@Result", SqlDbType.Int);
				prmResult.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteHostingPlan",
					prmResult,
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PlanId", planId));

				return Convert.ToInt32(prmResult.Value);
			}
		}
		#endregion

		#region Packages

		// Packages
		/// <summary>Auto-generated member.</summary>
		public DataSet GetMyPackages(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var packages = Packages
					.Where(p => p.UserId == userId)
					.Select(p => new
					{
						p.PackageId,
						p.ParentPackageId,
						p.PackageName,
						p.StatusId,
						p.PlanId,
						p.PurchaseDate,
						p.StatusIdChangeDate,
						// server
						ServerId = p.ServerId ?? 0,
						ServerName = p.Server != null ? p.Server.ServerName : "None",
						ServerComments = p.Server != null ? p.Server.Comments : "",
						VirtualServer = p.Server == null || p.Server.VirtualServer,
						// hosting plan
						p.HostingPlan.PlanName,
						// user
						p.UserId,
						p.User.Username,
						p.User.FirstName,
						p.User.LastName,
						FullName = p.User.FirstName + " " + p.User.LastName,
						p.User.RoleId,
						p.User.Email,
						p.DefaultTopPackage
					})
					.AsEnumerable()
					.Select(p => new
					{
						p.PackageId,
						p.ParentPackageId,
						p.PackageName,
						p.StatusId,
						p.PlanId,
						p.PurchaseDate,
						p.StatusIdChangeDate,
						Comments = Clone.GetItemComments(p.PackageId, "PACKAGE", actorId),
						// server
						p.ServerId,
						p.ServerName,
						p.ServerComments,
						p.VirtualServer,
						// hosting plan
						p.PlanName,
						// user
						p.UserId,
						p.Username,
						p.FirstName,
						p.LastName,
						p.FullName,
						p.RoleId,
						p.Email,
						p.DefaultTopPackage
					});
				/// <summary>TODO</summary>
				return EntityDataSet(packages);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetMyPackages",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetPackages(int actorId, int userId)
		{
			if (UseEntityFramework)
			{
				var packages = Packages
					.Where(p => p.UserId == userId)
					.Select(p => new
					{
						p.PackageId,
						p.ParentPackageId,
						p.PackageName,
						p.StatusId,
						p.PurchaseDate,
						p.StatusIdChangeDate,
						// server
						ServerId = p.ServerId != null ? p.ServerId : 0,
						ServerName = p.Server != null ? p.Server.ServerName : "None",
						ServerComments = p.Server != null ? p.Server.Comments : "",
						VirtualServer = p.Server == null || p.Server.VirtualServer,
						// hosting plan
						p.PlanId,
						p.HostingPlan.PlanName,
						// user
						p.UserId,
						p.User.Username,
						p.User.FirstName,
						p.User.LastName,
						p.User.RoleId,
						p.User.Email,
						p.DefaultTopPackage
					});
				/// <summary>TODO</summary>
				return EntityDataSet(packages);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackages",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetNestedPackagesSummary(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var nofPackages = new[] { new { PackagesNumber = Packages
					.Where(p => p.ParentPackageId == packageId)
					.Count() } };

				// by status spaces
				var spaces = Packages
					.Where(p => p.ParentPackageId == packageId && p.StatusId > 0)
					.GroupBy(p => p.StatusId)
					.OrderBy(p => p.Key)
					.Select(p => new
					{
						StatusId = p.Key,
						PackagesNumber = p.Count()
					});

				/// <summary>TODO</summary>
				return EntityDataSet(nofPackages, spaces);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetNestedPackagesSummary",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet SearchServiceItemsPaged(int actorId, int userId, int itemTypeId, string filterValue,
			string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				if (itemTypeId != 13)
				{
					using (var childUsers = UserChildren(userId))
					{
						var items = ServiceItems
							.Where(s => s.ItemTypeId == itemTypeId)
							.Join(Packages, s => s.PackageId, p => p.PackageId, (s, p) => new
							{
								Item = s,
								Package = p
							})
							.Join(UsersDetailed, s => s.Package.UserId, u => u.UserId, (s, u) => new
							{
								s.Item,
								s.Package,
								User = u
							})
							.Join(childUsers, s => s.User.UserId, u => u, (s, u) => s)
							//.Where(s => CheckUserParent(userId, s.User.UserId))
							.Select(s => new
							{
								s.Item.ItemId,
								s.Item.ItemName,
								s.Package.PackageId,
								s.Package.PackageName,
								s.Package.StatusId,
								s.Package.PurchaseDate,
								// user
								s.Package.UserId,
								s.User.Username,
								s.User.FirstName,
								s.User.LastName,
								s.User.FullName,
								s.User.RoleId,
								s.User.Email
							});

                        if (!string.IsNullOrEmpty(filterValue))
                        {
#if NETFRAMEWORK
                            items = items.Where(it => DbFunctions.Like(it.ItemName, filterValue));
#else
                            items = items.Where(it => EF.Functions.Like(it.ItemName, filterValue));
#endif
                        }

						var count = items.Count();

						items = string.IsNullOrEmpty(sortColumn) ? items.OrderBy(it => it.ItemName) : items.OrderBy(ColumnName(sortColumn));


						items = items.Skip(startRow).Take(maximumRows);

						/// <summary>TODO</summary>
						return EntityDataSet(count, items);
					}
				}
				else
				{
					//sortColumn = sortColumn.Replace("ItemName", "DomainName");

					using (var childUsers = UserChildren(userId))
					{
						var domains = Domains
							.Join(Packages, d => d.PackageId, p => p.PackageId, (d, p) => new
							{
								Domain = d,
								Package = p
							})
							.Join(UsersDetailed, d => d.Package.UserId, u => u.UserId, (d, u) => new
							{
								d.Domain,
								d.Package,
								User = u
							})
							.Join(childUsers, d => d.User.UserId, u => u, (d, u) => d)
							//.Where(d => CheckUserParent(userId, d.User.UserId))
							.Select(d => new
							{
								ItemId = d.Domain.DomainId,
								ItemName = d.Domain.DomainName,
								d.Package.PackageId,
								d.Package.PackageName,
								d.Package.StatusId,
								d.Package.PurchaseDate,
								// user
								d.Package.UserId,
								d.User.Username,
								d.User.FirstName,
								d.User.LastName,
								d.User.FullName,
								d.User.RoleId,
								d.User.Email
							});

                        if (!string.IsNullOrEmpty(filterValue))
                        {
#if NETFRAMEWORK
                           domains = domains.Where(it => DbFunctions.Like(it.ItemName, filterValue));
#else
                            domains = domains.Where(it => EF.Functions.Like(it.ItemName, filterValue));
#endif
                        }

                        var count = domains.Count();

						domains = string.IsNullOrEmpty(sortColumn) ? domains.OrderBy(it => it.ItemName) : domains.OrderBy(ColumnName(sortColumn));


						domains = domains.Skip(startRow).Take(maximumRows);

						/// <summary>TODO</summary>
						return EntityDataSet(count, domains);
					}
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "SearchServiceItemsPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemTypeId", itemTypeId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetPackagesPaged(int actorId, int userId, string filterColumn, string filterValue,
			string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var hasUserRights = CheckActorUserRights(actorId, userId);

				using (var userChildren = UsersTree(userId, true))
				{
					var packages = Packages
						.Where(p => hasUserRights && p.UserId != userId)
						.Join(userChildren, p => p.UserId, u => u, (p, u) => p)
						.Select(p => new
						{
							p.PackageId,
							p.PackageName,
							p.StatusId,
							p.PurchaseDate,
							// server
							p.ServerId,
							ServerName = p.Server.ServerName != null ? p.Server.ServerName : "None",
							ServerComments = p.Server.Comments != null ? p.Server.Comments : "",
							p.Server.VirtualServer,
							// hosting plan
							p.PlanId,
							p.HostingPlan.PlanName,
							// user
							p.UserId,
							p.User.Username,
							p.User.FirstName,
							p.User.LastName,
							FullName = p.User.FirstName + " " + p.User.LastName,
							p.User.RoleId,
							p.User.Email
						});

					if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
					{
						packages = packages.Where(DynamicFunctions.ColumnLike(packages, filterColumn, filterValue));
					}

					var count = packages.Count();

					if (!string.IsNullOrEmpty(sortColumn)) packages = packages.OrderBy(ColumnName(sortColumn));

					packages = packages.Skip(startRow).Take(maximumRows);

					var packagesSelected = packages
						.AsEnumerable()
						.Select(p => new
						{
							p.PackageId,
							p.PackageName,
							p.StatusId,
							p.PurchaseDate,
							Comments = Clone.GetItemComments(p.PackageId, "PACKAGE", actorId),
							// server
							p.ServerId,
							p.ServerName,
							p.ServerComments,
							p.VirtualServer,
							// hosting plan
							p.PlanId,
							p.PlanName,
							// user
							p.UserId,
							p.Username,
							p.FirstName,
							p.LastName,
							p.FullName,
							p.RoleId,
							p.Email
						});

					/// <summary>TODO</summary>
					return EntityDataSet(count, packagesSelected);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackagesPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetNestedPackagesPaged(int actorId, int packageId, string filterColumn, string filterValue,
			int statusId, int planId, int serverId, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var packages = Packages
					.Where(p => p.ParentPackageId == packageId &&
						(statusId == 0 || statusId > 0 && p.StatusId == statusId) &&
						(planId == 0 || planId > 0 && p.PlanId == planId) &&
						(serverId == 0 || serverId > 0 && p.ServerId == serverId))
					.Select(p => new
					{
						p.PackageId,
						p.PackageName,
						p.StatusId,
						p.PurchaseDate,
						p.StatusIdChangeDate,
						// server
						p.ServerId,
						ServerName = p.Server.ServerName != null ? p.Server.ServerName : "None",
						ServerComments = p.Server.Comments != null ? p.Server.Comments : "",
						p.Server.VirtualServer,
						// hosting plan
						p.PlanId,
						p.HostingPlan.PlanName,
						// user
						p.UserId,
						p.User.Username,
						p.User.FirstName,
						p.User.LastName,
						FullName = p.User.FirstName + " " + p.User.LastName,
						p.User.RoleId,
						p.User.Email
					});

				if (!string.IsNullOrEmpty(filterValue))
				{
					if (!string.IsNullOrEmpty(filterColumn))
					{
						packages = packages.Where(DynamicFunctions.ColumnLike(packages, filterColumn, filterValue));
					}
					else
					{
#if NETFRAMEWORK
						packages = packages.Where(p => DbFunctions.Like(p.Username, filterValue) ||
							DbFunctions.Like(p.FullName, filterValue) ||
							DbFunctions.Like(p.Email, filterValue));
#else
						packages = packages.Where(p => EF.Functions.Like(p.Username, filterValue) ||
							EF.Functions.Like(p.FullName, filterValue) ||
							EF.Functions.Like(p.Email, filterValue));
#endif
					}
				}

				var count = packages.Count();

				packages = !string.IsNullOrEmpty(sortColumn) ? packages.OrderBy(ColumnName(sortColumn)) : packages.OrderBy(p => p.PackageName);


				packages = packages.Skip(startRow).Take(maximumRows);

				var packagesSelected = packages
					.AsEnumerable()
					.Select(p => new
					{
						p.PackageId,
						p.PackageName,
						p.StatusId,
						p.PurchaseDate,
						p.StatusIdChangeDate,
						Comments = Clone.GetItemComments(p.PackageId, "PACKAGE", actorId),
						// server
						p.ServerId,
						p.ServerName,
						p.ServerComments,
						p.VirtualServer,
						// hosting plan
						p.PlanId,
						p.PlanName,
						// user
						p.UserId,
						p.Username,
						p.FirstName,
						p.LastName,
						p.FullName,
						p.RoleId,
						p.Email
					});

				/// <summary>TODO</summary>
				return EntityDataSet(count, packagesSelected);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetNestedPackagesPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@statusId", statusId),
					/// <summary>TODO</summary>
					new SqlParameter("@planId", planId),
					/// <summary>TODO</summary>
					new SqlParameter("@serverId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetPackagePackages(int actorId, int packageId, bool recursive)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				/// <summary>TODO</summary>
				IQueryable<Data.Entities.Package> packagesFiltered;
				TempIdSet childPackages = null;
				try
				{
					if (!recursive) packagesFiltered = Packages.Where(p => p.ParentPackageId == packageId);
					else
					{
						childPackages = PackagesTree(packageId, true);
						packagesFiltered = Packages.Join(childPackages, p => p.PackageId, ch => ch, (p, ch) => p);
					}

					var packages = packagesFiltered
						.Where(p => p.PackageId != packageId)
						// &&
						// (recursive && CheckPackageParent(packageId, p.PackageId) ||
						// !recursive && p.ParentPackageId == packageId))
						.Join(Users, p => p.UserId, u => u.UserId, (p, u) => new
						{
							Package = p,
							User = u
						})
						.Join(Servers, p => p.Package.ServerId, s => s.ServerId, (p, s) => new
						{
							p.Package,
							p.User,
							Server = s
						})
						.Join(HostingPlans, p => p.Package.PlanId, hp => hp.PlanId, (p, hp) => new
						{
							p.Package.PackageId,
							p.Package.ParentPackageId,
							p.Package.PackageName,
							p.Package.StatusId,
							p.Package.PurchaseDate,
							// server
							p.Package.ServerId,
							ServerName = p.Server.ServerName != null ? p.Server.ServerName : "None",
							ServerComments = p.Server.Comments != null ? p.Server.Comments : "",
							p.Server.VirtualServer,
							// hosting plan
							p.Package.PlanId,
							hp.PlanName,
							// user
							p.Package.UserId,
							p.User.Username,
							p.User.FirstName,
							p.User.LastName,
							p.User.RoleId,
							p.User.Email
						});
					/// <summary>TODO</summary>
					return EntityDataSet(packages);
				}
				finally
				{
					childPackages?.Dispose();
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackagePackages",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@recursive", recursive));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackage(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				// TODO Note: actorId is not verified
				// check both requested and parent package

				var packages = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => new
					{
						p.PackageId,
						p.ParentPackageId,
						p.UserId,
						p.PackageName,
						p.PackageComments,
						p.ServerId,
						p.StatusId,
						p.PlanId,
						p.PurchaseDate,
						p.StatusIdChangeDate,
						p.OverrideQuotas,
						p.DefaultTopPackage
					});
				/// <summary>TODO</summary>
				return EntityDataReader(packages);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackage",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int CalculatePackageDiskspace(int packageId)
		{
			int diskspace = (int)(((PackagesTreeCaches
				.Where(t => t.ParentPackageId == packageId)
				.Join(Packages, t => t.PackageId, p => p.PackageId, (t, p) => p)
				.Join(PackagesDiskspaces, p => p.PackageId, pd => pd.PackageId, (p, pd) => new
				{
					Package = p,
					Diskspace = pd
				})
				.Join(HostingPlanResources.Where(hr => hr.CalculateDiskSpace == true),
					p => new { p.Diskspace.GroupId, p.Package.PlanId }, hr => new { hr.GroupId, PlanId = (int?)hr.PlanId },
					(p, hr) => p.Diskspace.DiskSpace)
				.Sum(space => (long?)space) ?? 0) + MB / 2) / MB);
			/// <summary>TODO</summary>
			return diskspace;
		}
		/// <summary>Auto-generated member.</summary>

		public int CalculatePackageBandwidth(int packageId)
		{
			var today = DateTime.Now.Date;
			var startDate = today.AddDays(-today.Day + 1);
			var endDate = startDate.AddMonths(1);

			int bandwidth = (int)(((PackagesTreeCaches
				.Where(t => t.ParentPackageId == packageId)
				.Join(Packages, t => t.PackageId, p => p.PackageId, (t, p) => p)
				.Join(PackagesBandwidths, p => p.PackageId, pb => pb.PackageId, (p, pb) => new
				{
					Package = p,
					Bandwidth = pb
				})
				.Where(p => startDate <= p.Bandwidth.LogDate && p.Bandwidth.LogDate < endDate)
				.Join(HostingPlanResources.Where(hr => hr.CalculateBandwidth == true),
					p => new { p.Bandwidth.GroupId, p.Package.PlanId }, hr => new { hr.GroupId, PlanId = (int?)hr.PlanId },
					(p, hr) => p.Bandwidth.BytesSent + p.Bandwidth.BytesReceived)
				.Sum(space => (long?)space) ?? 0) + MB / 2) / MB);
			/// <summary>TODO</summary>
			return bandwidth;
		}
		/// <summary>Auto-generated member.</summary>

		public int CalculateQuotaUsage(int packageId, int quotaId)
		{
			var quota = Quotas
				.Where(q => q.QuotaId == quotaId)
				.Select(q => new { q.QuotaTypeId, q.QuotaName })
				.FirstOrDefault();

			if (quota?.QuotaTypeId != 2) return 0;

			int result = 0;
			bool isCore = IsCore;

			switch (quotaId)
			{
				case 52: // diskspace
					result = CalculatePackageDiskspace(packageId);
					break;
				case 51: // bandwidth
					result = CalculatePackageBandwidth(packageId);
					break;
				case 53: // domains
					result = PackagesTreeCaches
						.Where(p => p.ParentPackageId == packageId)
						.Join(Domains, p => p.PackageId, d => d.PackageId, (p, d) => d)
						.Where(d => !d.IsSubDomain && !d.IsPreviewDomain && !d.IsDomainPointer)
						.Count();
					break;
				case 54: // sub-domains
					result = PackagesTreeCaches
						.Where(p => p.ParentPackageId == packageId)
						.Join(Domains, p => p.PackageId, d => d.PackageId, (p, d) => d)
						.Where(d => d.IsSubDomain && !d.IsPreviewDomain && !d.IsDomainPointer)
						.Count();
					break;
				case 220: // domain-pointers
					result = PackagesTreeCaches
						.Where(p => p.ParentPackageId == packageId)
						.Join(Domains, p => p.PackageId, d => d.PackageId, (p, d) => d)
						.Where(d => d.IsDomainPointer)
						.Count();
					break;
				case 71: // scheduled tasks
					result = PackagesTreeCaches
						.Where(p => p.ParentPackageId == packageId)
						.Join(Schedules, p => p.PackageId, s => s.PackageId, (p, s) => s)
						.Count();
					break;
				case 305: // RAM of VPS
					var ps = ServiceItemProperties
						.Join(ServiceItems, p => p.ItemId, s => s.ItemId, (p, s) => new
						{
							Property = p,
							Item = s
						})
						.Join(PackagesTreeCaches, p => p.Item.PackageId, t => t.PackageId, (p, t) => new
						{
							p.Property.PropertyName,
							t.ParentPackageId,
							p.Property.PropertyValue
						})
						.Where(p => p.PropertyName == "RamSize" && p.ParentPackageId == packageId);
					result = isCore
						? ps.Sum(p => (int?)Convert.ToInt32(p.PropertyValue)) ?? 0
						: ps
							.Select(p => p.PropertyValue)
							.Cast<int?>()
							.Sum() ?? 0;
					break;
				case 302: // CpuNumber of VPS
				case 555: // CpuNumber of VPS2012
                case 347: // CpuNumber of VPSforPc 
                //case 675: // CpuNumber of Proxmox
                    ps = ServiceItemProperties
						.Join(ServiceItems, p => p.ItemId, s => s.ItemId, (p, s) => new
						{
							Property = p,
							Item = s
						})
						.Join(PackagesTreeCaches, p => p.Item.PackageId, t => t.PackageId, (p, t) => new
						{
							p.Property.PropertyName,
							t.ParentPackageId,
							p.Property.PropertyValue
						})
						.Where(p => p.PropertyName == "CpuCores" && p.ParentPackageId == packageId);
					result = isCore
						? ps.Sum(p => (int?)Convert.ToInt32(p.PropertyValue)) ?? 0
						: ps
							.Select(p => p.PropertyValue)
							.Cast<int?>()
							.Sum() ?? 0;
					break;
				case 306: // HDD of VPS
				case 559: // HDD of VPS2012
				case 351: // HDD of VPSforPc
				//case 679: // HDD of Proxmox
					result = ServiceItemProperties
						.Join(ServiceItems, p => p.ItemId, s => s.ItemId, (p, s) => new
						{
							Property = p,
							Item = s
						})
						.Join(PackagesTreeCaches, p => p.Item.PackageId, t => t.PackageId, (p, t) => new
						{
							p.Property.PropertyName,
							t.ParentPackageId,
							p.Property.PropertyValue
						})
						.Where(p => p.PropertyName == "HddSize" && p.ParentPackageId == packageId)
						.AsEnumerable()
						.SelectMany(p => p.PropertyValue.Split(';'))
						.Sum(p => (int?)int.Parse(p)) ?? 0;
					break;
				case 309: // External IP addresses of VPS
				case 562: // External IP addresses of VPS2012
				case 354: // External IP addresses of VPSforPc
				//case 682: // External IP addresses of Proxmox
                    result = PackageIpAddresses
						.Join(IpAddresses, p => p.AddressId, ip => ip.AddressId, (p, ip) => new
						{
							Package = p,
							Ip = ip
						})
						.Join(PackagesTreeCaches, p => p.Package.PackageId, t => t.PackageId, (p, t) => new
						{
							t.ParentPackageId,
							p.Ip.PoolId
						})
						.Where(p => p.ParentPackageId == packageId && p.PoolId == 3)
						.Count();
					break;
				case 558: // RAM of VPS2012
					/// <summary>TODO</summary>
					int fixedMem, dynamicMem;
					ps = ServiceItemProperties
						.Join(ServiceItems, p => p.ItemId, s => s.ItemId, (p, s) => new
						{
							Property = p,
							Item = s
						})
						.Join(PackagesTreeCaches, p => p.Item.PackageId, t => t.PackageId, (p, t) => new
						{
							p.Property.PropertyName,
							t.ParentPackageId,
							p.Property.PropertyValue
						})
						.Where(p => p.PropertyName == "RamSize" && p.ParentPackageId == packageId);
					fixedMem = isCore
						? ps.Sum(p => (int?)Convert.ToInt32(p.PropertyValue)) ?? 0
						: ps
							.Select(p => p.PropertyValue)
							.Cast<int?>()
							.Sum() ?? 0;

					ps = ServiceItemProperties
						.Join(ServiceItems, p => p.ItemId, s => s.ItemId, (p, s) => new
						{
							Property = p,
							Item = s
						})
						.Join(ServiceItemProperties.Where(p => p.PropertyName == "DynamicMemory.Enabled" && p.PropertyValue == "True"),
							p => p.Item.ItemId, sp => sp.ItemId, (p, sp) => p)
						.Join(PackagesTreeCaches, p => p.Item.PackageId, t => t.PackageId, (p, t) => new
						{
							p.Property.PropertyName,
							t.ParentPackageId,
							p.Property.PropertyValue
						})
						.Where(p => p.PropertyName == "DynamicMemory.Maximum" && p.ParentPackageId == packageId);
					dynamicMem = isCore
						? ps.Sum(p => (int?)Convert.ToInt32(p.PropertyValue)) ?? 0
						: ps
							.Select(p => p.PropertyValue)
							.Cast<int?>()
							.Sum() ?? 0;

					result = Math.Max(fixedMem, dynamicMem);
					break;
				case 728: // Private Network VLANs of VPS2012
					result = PackageVlans
						.Join(PrivateNetworkVlans, v => v.VlanId, pv => pv.VlanId, (v, pv) => v)
						.Join(PackagesTreeCaches, v => v.PackageId, t => t.PackageId, (v, t) => new
						{
							Vlan = v,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId && !t.Vlan.IsDmz)
						.Count();
					break;
				case 752: // DMZ Network VLANs of VPS2012
					result = PackageVlans
						.Join(PrivateNetworkVlans, v => v.VlanId, pv => pv.VlanId, (v, pv) => v)
						.Join(PackagesTreeCaches, v => v.PackageId, t => t.PackageId, (v, t) => new
						{
							Vlan = v,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId && t.Vlan.IsDmz)
						.Count();
					break;
				case 100: // Dedicated Web IP addresses
					result = PackageIpAddresses
						.Join(IpAddresses, p => p.AddressId, ip => ip.AddressId, (p, ip) => new
						{
							Package = p,
							Ip = ip
						})
						.Join(PackagesTreeCaches, p => p.Package.PackageId, t => t.PackageId, (p, t) => new
						{
							t.ParentPackageId,
							p.Ip.PoolId
						})
						.Where(p => p.ParentPackageId == packageId && p.PoolId == 2)
						.Count();
					break;
				case 350: // RAM of VPSforPc
					ps = ServiceItemProperties
						.Join(ServiceItems, p => p.ItemId, s => s.ItemId, (p, s) => new
						{
							Property = p,
							Item = s
						})
						.Join(PackagesTreeCaches, p => p.Item.PackageId, t => t.PackageId, (p, t) => new
						{
							p.Property.PropertyName,
							t.ParentPackageId,
							p.Property.PropertyValue
						})
						.Where(p => p.PropertyName == "Memory" && p.ParentPackageId == packageId);
					result = isCore
						? ps.Sum(p => (int?)Convert.ToInt32(p.PropertyValue)) ?? 0
						: ps
							.Select(p => p.PropertyValue)
							.Cast<int?>()
							.Sum() ?? 0;
					break;
				case 319: // BB Users
					result = ExchangeAccounts
						.Join(BlackBerryUsers, ea => ea.AccountId, bb => bb.AccountId, (ea, bb) => ea)
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => si)
						.Join(PackagesTreeCaches, si => si.PackageId, t => t.PackageId, (si, t) => t)
						.Where(t => t.ParentPackageId == packageId)
						.Count();
					break;
				case 320: // OCS Users
					result = ExchangeAccounts
						.Join(OcsUsers, ea => ea.AccountId, ocs => ocs.AccountId, (ea, ocs) => ea)
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => si)
						.Join(PackagesTreeCaches, si => si.PackageId, t => t.PackageId, (si, t) => t)
						.Where(t => t.ParentPackageId == packageId)
						.Count();
					break;
				case 206: // HostedSolution.Users
					var accountTypes = new[] { ExchangeAccountType.Mailbox, ExchangeAccountType.Room,
						ExchangeAccountType.Equipment, ExchangeAccountType.User };
					result = ExchangeAccounts
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => new
						{
							Exchange = ea,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							ea.Item,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId && accountTypes.Any(type => t.Exchange.AccountType == type))
						.Count();
					break;
				case 78: // Exchange2007.Mailboxes
					result = ExchangeAccounts
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => new
						{
							Exchange = ea,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							ea.Item,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId && t.Exchange.MailboxPlanId != null &&
							t.Exchange.AccountType == ExchangeAccountType.Mailbox)
						.Count();
					break;
				case 731: // Exchange2013.JournalingMailboxes
					result = ExchangeAccounts
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => new
						{
							Exchange = ea,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							ea.Item,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId && t.Exchange.MailboxPlanId != null &&
							t.Exchange.AccountType == ExchangeAccountType.JournalingMailbox)
						.Count();
					break;
				case 77: // Exchange2007.DiskSpace
					accountTypes = new[] { ExchangeAccountType.Mailbox, ExchangeAccountType.Room,
						ExchangeAccountType.Equipment, ExchangeAccountType.SharedMailbox,
						ExchangeAccountType.JournalingMailbox };
					result = ExchangeAccounts
						.Join(ExchangeMailboxPlans, ea => ea.MailboxPlanId, ep => ep.MailboxPlanId, (ea, ep) => new
						{
							Exchange = ea,
							MailboxPlan = ep
						})
						.Join(ServiceItems, ea => ea.Exchange.ItemId, si => si.ItemId, (ea, si) => new
						{
							ea.Exchange,
							ea.MailboxPlan,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							ea.MailboxPlan,
							ea.Item,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId &&
							accountTypes.Any(type => t.Exchange.AccountType == type))
						.Sum(t => (int?)t.MailboxPlan.MailboxSizeMb) ?? 0;
					break;
				case 370: // Lync.Users
					result = ExchangeAccounts
						.Join(LyncUsers, ea => ea.AccountId, lu => lu.AccountId, (ea, lu) => ea)
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => new
						{
							Exchange = ea,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							ea.Item,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId)
						.Count();
					break;
				case 376: // Lync.EVUsers
					result = ExchangeAccounts
						.Join(LyncUsers, ea => ea.AccountId, lu => lu.AccountId, (ea, lu) => new
						{
							Exchange = ea,
							LyncUser = lu,
						})
						.Join(LyncUserPlans, ea => ea.LyncUser.LyncUserPlanId, lp => lp.LyncUserPlanId, (ea, lp) => new
						{
							ea.Exchange,
							ea.LyncUser,
							LyncUserPlan = lp,
						})
						.Join(ServiceItems, ea => ea.Exchange.ItemId, si => si.ItemId, (ea, si) => new
						{
							ea.Exchange,
							ea.LyncUser,
							ea.LyncUserPlan,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							ea.LyncUser,
							ea.LyncUserPlan,
							ea.Item,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId && t.LyncUserPlan.EnterpriseVoice)
						.Count();
					break;
				case 381: // Dedicated Lync Phone Numbers
					result = PackageIpAddresses
						.Join(IpAddresses, p => p.AddressId, ip => ip.AddressId, (p, ip) => new
						{
							Package = p,
							Ip = ip
						})
						.Join(PackagesTreeCaches, p => p.Package.PackageId, t => t.PackageId, (p, t) => new
						{
							t.ParentPackageId,
							p.Ip.PoolId
						})
						.Where(p => p.ParentPackageId == packageId && p.PoolId == 5)
						.Count();
					break;
				case 430: // Enterprise Storage
					result = EnterpriseFolders
						.Join(ServiceItems, ef => ef.ItemId, si => si.ItemId, (ef, si) => new
						{
							Folder = ef,
							Item = si
						})
						.Join(PackagesTreeCaches, ef => ef.Item.PackageId, t => t.PackageId, (ef, t) => new
						{
							ef.Folder,
							ef.Item,
							Tree = t
						})
						.Where(ef => ef.Tree.ParentPackageId == packageId)
						.Sum(ef => (int?)ef.Folder.FolderQuota) ?? 0;
					break;
				case 431: // Enterprise Storage Folders
					result = EnterpriseFolders
						.Join(ServiceItems, ef => ef.ItemId, si => si.ItemId, (ef, si) => new
						{
							Folder = ef,
							Item = si
						})
						.Join(PackagesTreeCaches, ef => ef.Item.PackageId, t => t.PackageId, (ef, t) => new
						{
							ef.Folder,
							ef.Item,
							Tree = t
						})
						.Where(ef => ef.Tree.ParentPackageId == packageId)
						.Count();
					break;
				case 423: // HostedSolution.SecurityGroups
					accountTypes = new[] { ExchangeAccountType.SecurityGroup,
						ExchangeAccountType.DefaultSecurityGroup };
					result = ExchangeAccounts
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => new
						{
							Exchange = ea,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							ea.Item,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId &&
							accountTypes.Any(type => t.Exchange.AccountType == type))
						.Count();
					break;
				case 495: // HostedSolution.DeletedUsers
					result = ExchangeAccounts
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => new
						{
							Exchange = ea,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							ea.Item,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId &&
							t.Exchange.AccountType == ExchangeAccountType.DeletedUser)
						.Count();
					break;
				case 450: // RDSCollectionUsers
					result = RdsCollectionUsers
						.Join(ExchangeAccounts, rds => rds.AccountId, ea => ea.AccountId, (rds, ea) => ea)
						.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => new
						{
							Exchange = ea,
							Item = si
						})
						.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
						{
							ea.Exchange,
							Tree = t
						})
						.Where(t => t.Tree.ParentPackageId == packageId &&
							t.Exchange.AccountType == ExchangeAccountType.DeletedUser)
						.GroupBy(ea => ea.Exchange.AccountId)
						.Count();
					break;
				case 451: // RDSServers
					result = RdsServers
						.Join(ServiceItems, s => s.ItemId, si => si.ItemId, (s, si) => si)
						.Join(PackagesTreeCaches, si => si.PackageId, t => t.PackageId, (s, t) => t)
						.Where(t => t.ParentPackageId == packageId)
						.Count();
					break;
				case 491: // RDSCollections
					result = RdsCollections
						.Join(ServiceItems, rc => rc.ItemId, si => si.ItemId, (s, si) => si)
						.Join(PackagesTreeCaches, si => si.PackageId, t => t.PackageId, (s, t) => t)
						.Where(t => t.ParentPackageId == packageId)
						.Count();
					break;
				default:
					if (quota.QuotaName.StartsWith("ServiceLevel.")) // Support Service Level Quota
					{
						var levelName = quota.QuotaName.Substring("ServiceLevel.".Length);
						var levelId = SupportServiceLevels
							.Where(l => l.LevelName == levelName)
							.Select(l => (int?)l.LevelId)
							.FirstOrDefault();
						result = levelId != null
							? ExchangeAccounts
								.Join(ServiceItems, ea => ea.ItemId, si => si.ItemId, (ea, si) => new
								{
									Exchange = ea,
									Item = si
								})
								.Join(PackagesTreeCaches, ea => ea.Item.PackageId, t => t.PackageId, (ea, t) => new
								{
									ea.Exchange,
									ea.Item,
									Tree = t
								})
								.Where(t => t.Tree.ParentPackageId == packageId && t.Exchange.LevelId == levelId)
								.Count()
							: 0;
					}
					else
					{
						result = Quotas
							.Where(q => q.QuotaId == quotaId)
							.Join(ServiceItems, q => q.ItemTypeId, si => si.ItemTypeId, (q, si) => si)
							.Join(PackagesTreeCaches, si => si.PackageId, t => t.PackageId, (si, t) => t)
							.Where(t => t.ParentPackageId == packageId)
							.Count();
					}
					break;
			}
			/// <summary>TODO</summary>
			return result;
		}
		/// <summary>Auto-generated member.</summary>

		public int GetMailAccountsCountForDomain(int packageId, string domain)
		{
			domain = "@" + domain;
			return ServiceItems
				.Where(si => si.ItemTypeId == 15 && si.ItemName.EndsWith(domain)) // Mail Account
				.Join(PackagesTreeCaches, si => si.PackageId, t => t.PackageId, (si, t) => t)
				.Where(t => t.ParentPackageId == packageId)
				.Count();
		}
		/// <summary>Auto-generated member.</summary>
		public DataSet GetPackageQuotas(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var package = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => new { p.PlanId, p.ParentPackageId })
					.FirstOrDefault();
				var packagePlanId = package?.PlanId;

				// get resource groups
				var groups = ResourceGroups
					.GroupJoin(HostingPlanResources,
						r => new { r.GroupId, PlanId = packagePlanId },
						hr => new { hr.GroupId, PlanId = (int?)hr.PlanId },
						(r, hr) => new
						{
							Group = r,
							HostingPlans = hr
						})
					.SelectMany(g => g.HostingPlans.DefaultIfEmpty(), (g, hr) => new
					{
						g.Group.GroupId,
						g.Group.GroupName,
						g.Group.GroupOrder,
						CalculateDiskSpace = hr != null ? hr.CalculateDiskSpace : false,
						CalculateBandwidth = hr != null ? hr.CalculateBandwidth : false,
					})
					.AsEnumerable()
					.Where(r => r.GroupName != "Service Levels" && Clone.GetPackageAllocatedResource(packageId, r.GroupId, 0) ||
						r.GroupName == "Service Levels" && Clone.GetPackageServiceLevelResource(packageId, r.GroupId, 0))
					.OrderBy(r => r.GroupOrder)
					.Select(g => new
					{
						g.GroupId,
						g.GroupName,
						g.CalculateDiskSpace,
						g.CalculateBandwidth,
						ParentEnabled = g.GroupName == "Service Levels" ?
							Clone.GetPackageServiceLevelResource(package?.ParentPackageId, g.GroupId, 0) :
							Clone.GetPackageAllocatedResource(package?.ParentPackageId, g.GroupId, 0)
					});

				// return quotas
				var nofOrgs = GetPackageAllocatedQuota(packageId, 205); // 205 - HostedSolution.Organizations
				if (nofOrgs < 1) nofOrgs = 1;

				var quotas = Quotas
					.Where(q => q.HideQuota != true)
					.OrderBy(q => q.QuotaOrder)
					.AsEnumerable()
					.Select(q => new
					{
						Quota = q,
						AllocatedQuota = Clone.GetPackageAllocatedQuota(packageId, q.QuotaId)
					})
					.Select(q => new
					{
						q.Quota.QuotaId,
						q.Quota.GroupId,
						q.Quota.QuotaName,
						q.Quota.QuotaDescription,
						q.Quota.QuotaTypeId,
						QuotaValue = q.Quota.PerOrganization == 1 && q.AllocatedQuota != -1 ?
							q.AllocatedQuota * nofOrgs :
							q.AllocatedQuota,
						QuotaValuePerOrganization = q.AllocatedQuota,
						ParentQuotaValue = Clone.GetPackageAllocatedQuota(package?.ParentPackageId, q.Quota.QuotaId),
						QuotaUsedValue = Clone.CalculateQuotaUsage(packageId, q.Quota.QuotaId),
						q.Quota.PerOrganization
					});
				/// <summary>TODO</summary>
				return EntityDataSet(groups, quotas);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackageQuotas",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetParentPackageQuotas(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorParentPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				// TODO is this implementation really just the same as GetPackageQuotas?
				var package = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => new { p.PlanId, p.ParentPackageId })
					.FirstOrDefault();
				// get resource groups
				var groups = ResourceGroups
					.GroupJoin(HostingPlanResources.Where(r => r.PlanId == package.PlanId),
						r => r.GroupId, hr => hr.GroupId, (r, hr) => new
						{
							Group = r,
							Resources = hr
						})
					.SelectMany(g => g.Resources.DefaultIfEmpty(), (g, hr) => new
					{
						g.Group.GroupId,
						g.Group.GroupName,
						g.Group.GroupOrder,
						CalculateDiskSpace = hr != null ? hr.CalculateDiskSpace : false,
						CalculateBandwidth = hr != null ? hr.CalculateBandwidth : false,
					})
					.AsEnumerable()
					.Where(r => r.GroupName != "Service Levels" && Clone.GetPackageAllocatedResource(packageId, r.GroupId, 0) ||
						r.GroupName == "Service Levels" && Clone.GetPackageServiceLevelResource(packageId, r.GroupId, 0))
					.OrderBy(r => r.GroupOrder)
					.Select(g => new
					{
						g.GroupId,
						g.GroupName,
						ParentEnabled = g.GroupName == "Service Levels" ?
							Clone.GetPackageServiceLevelResource(package.ParentPackageId, g.GroupId, 0) :
							Clone.GetPackageAllocatedResource(package.ParentPackageId, g.GroupId, 0)
					});
				// return quotas
				var nofOrgs = GetPackageAllocatedQuota(packageId, 205); // 205 - HostedSolution.Organizations
				if (nofOrgs < 1) nofOrgs = 1;

				var quotas = Quotas
					.Where(q => q.HideQuota != true)
					.OrderBy(q => q.QuotaOrder)
					.AsEnumerable()
					.Select(q => new
					{
						Quota = q,
						AllocatedQuota = Clone.GetPackageAllocatedQuota(packageId, q.QuotaId)
					})
					.Select(q => new
					{
						q.Quota.QuotaId,
						q.Quota.GroupId,
						q.Quota.QuotaName,
						q.Quota.QuotaDescription,
						q.Quota.QuotaTypeId,
						QuotaValue = q.Quota.PerOrganization == 1 && q.AllocatedQuota != -1 ?
							q.AllocatedQuota * nofOrgs :
							q.AllocatedQuota,
						QuotaValuePerOrganization = q.AllocatedQuota,
						ParentQuotaValue = Clone.GetPackageAllocatedQuota(package.ParentPackageId, q.Quota.QuotaId),
						QuotaUsedValue = Clone.CalculateQuotaUsage(packageId, q.Quota.QuotaId),
						q.Quota.PerOrganization
					});
				/// <summary>TODO</summary>
				return EntityDataSet(groups, quotas);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetParentPackageQuotas",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetPackageQuotasForEdit(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var package = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => new { p.ServerId, p.PlanId, p.ParentPackageId })
					.FirstOrDefault();
				// get resource groups
				var groups = ResourceGroups
					.GroupJoin(HostingPlanResources.Where(r => r.PlanId == package.PlanId), r => r.GroupId, hr => hr.GroupId, (r, hr) => new
					{
						Group = r,
						Resources = hr
						//HostingPlan = hr.SingleOrDefault()
					})
					.SelectMany(g => g.Resources.DefaultIfEmpty(), (g, hr) => new
					{
						g.Group.GroupId,
						g.Group.GroupName,
						g.Group.GroupOrder,
						CalculateDiskSpace = hr != null ? hr.CalculateDiskSpace : false,
						CalculateBandwidth = hr != null ? hr.CalculateBandwidth : false,
					})
					.AsEnumerable()
					.OrderBy(r => r.GroupOrder)
					.Select(g => new
					{
                        g.GroupId,
                        g.GroupName,
                        g.CalculateDiskSpace,
                        g.CalculateBandwidth,
                        Enabled = g.GroupName == "Service Levels" ?
							Clone.GetPackageServiceLevelResource(packageId, g.GroupId, package.ServerId) :
							Clone.GetPackageAllocatedResource(packageId, g.GroupId, package.ServerId),
						ParentEnabled = g.GroupName == "Service Levels" ?
							Clone.GetPackageServiceLevelResource(package.ParentPackageId, g.GroupId, package.ServerId) :
							Clone.GetPackageAllocatedResource(package.ParentPackageId, g.GroupId, package.ServerId)
					});
				// return quotas
				var nofOrgs = GetPackageAllocatedQuota(packageId, 205); // 205 - HostedSolution.Organizations
				if (nofOrgs < 1) nofOrgs = 1;

				var quotas = Quotas
					.Where(q => q.HideQuota != true)
					.OrderBy(q => q.QuotaOrder)
					.AsEnumerable()
					.Select(q => new
					{
						Quota = q,
						AllocatedQuota = Clone.GetPackageAllocatedQuota(packageId, q.QuotaId)
					})
					.Select(q => new
					{
						q.Quota.QuotaId,
						q.Quota.GroupId,
						q.Quota.QuotaName,
						q.Quota.QuotaDescription,
						q.Quota.QuotaTypeId,
						QuotaValue = q.Quota.PerOrganization == 1 && q.AllocatedQuota != -1 ?
							q.AllocatedQuota * nofOrgs :
							q.AllocatedQuota,
						QuotaValuePerOrganization = q.AllocatedQuota,
						ParentQuotaValue = Clone.GetPackageAllocatedQuota(package.ParentPackageId, q.Quota.QuotaId),
						QuotaUsedValue = Clone.CalculateQuotaUsage(packageId, q.Quota.QuotaId),
						q.Quota.PerOrganization
					});
				/// <summary>TODO</summary>
				return EntityDataSet(groups, quotas);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackageQuotasForEdit",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IEnumerable<int> PackageParents(int packageId)
		{
			int? pid = packageId;
			var parentPackageId = Packages
				.Where(p => p.PackageId == pid)
				.Select(p => p.ParentPackageId);
			pid = parentPackageId.FirstOrDefault();
			while (pid != null)
			{
				yield return pid.Value;
				pid = parentPackageId.FirstOrDefault();
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet AddPackage(int actorId, out int packageId, int userId, int planId, string packageName,
			string packageComments, int statusId, DateTime purchaseDate)
		{
			if (UseEntityFramework)
			{
				var plan = HostingPlans
					.Where(hp => hp.PlanId == planId)
					.Select(hp => new { hp.PackageId, hp.ServerId })
					.FirstOrDefault();

				var parentPackageId = plan?.PackageId;

				if (parentPackageId == null || parentPackageId == 0)
				{
					parentPackageId = Packages
						.Where(p => p.ParentPackageId == null)
						.Select(p => p.PackageId)
						.FirstOrDefault();
				}

				// check rights
				if (!CheckActorPackageRights(actorId, parentPackageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var today = DateTime.Now.Date;
				var dateLastYear = today.AddYears(-1);

				using (var transaction = Database.BeginTransaction())
				{
					var package = new Data.Entities.Package()
					{
						ParentPackageId = parentPackageId,
						UserId = userId,
						PackageName = packageName,
						PackageComments = packageComments,
						ServerId = plan?.ServerId,
						StatusId = statusId,
						PlanId = planId,
						PurchaseDate = purchaseDate,
						BandwidthUpdated = dateLastYear,
						StatusIdChangeDate = DateTime.Now
					};
					Packages.Add(package);
					SaveChanges();

					var pid = packageId = package.PackageId;

					// add package to packages cache
					var parents = PackageParents(pid)
						.Select(parentId => new Data.Entities.PackagesTreeCache
						{
							PackageId = pid,
							ParentPackageId = parentId
						});
					PackagesTreeCaches.AddRange(parents);
					SaveChanges();

					var exceedingQuotas = GetPackageExceedingQuotas(parentPackageId)
						.Where(q => q.QuotaValue > 0)
						.ToList();

					if (exceedingQuotas.Any()) transaction.Rollback();
					else transaction.Commit();

					var result = EntityDataSet(exceedingQuotas);

					//DistributePackageServices(actorId, packageId);

					/// <summary>TODO</summary>
					return result;
				}
			}
			else
			{
				SqlParameter prmPackageId = new SqlParameter("@PackageID", SqlDbType.Int);
				prmPackageId.Direction = ParameterDirection.Output;

				DataSet ds = SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddPackage",
					prmPackageId,
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageName", packageName),
					/// <summary>TODO</summary>
					new SqlParameter("@packageComments", packageComments),
					/// <summary>TODO</summary>
					new SqlParameter("@statusId", statusId),
					/// <summary>TODO</summary>
					new SqlParameter("@planId", planId),
					/// <summary>TODO</summary>
					new SqlParameter("@purchaseDate", purchaseDate));

				// read identity
				packageId = Convert.ToInt32(prmPackageId.Value);

				DistributePackageServices(actorId, packageId);

				/// <summary>TODO</summary>
				return ds;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdatePackageQuotas(int actorId, int packageId, string xml)
		{
			// check rights
			if (!CheckActorPackageRights(actorId, packageId))
				/// <summary>TODO</summary>
				throw new AccessViolationException("You are not allowed to access this package");

			// delete old Package resources
			PackageResources.Where(r => r.PackageId == packageId).ExecuteDelete();

			// delete old Package quotas
			PackageQuotas.Where(q => q.PackageId == packageId).ExecuteDelete();

			var overrideQuotas = Packages
				.Where(p => p.PackageId == packageId)
				.Select(p => p.OverrideQuotas)
				.FirstOrDefault();
			if (overrideQuotas)
			{
				var plan = XElement.Parse(xml);

				var resources = plan
					.Element("groups")
					.Elements()
					.Select(e => new Data.Entities.PackageResource
					{
						PackageId = packageId,
						GroupId = (int)e.Attribute("id"),
						CalculateDiskspace = (int)e.Attribute("calculateDiskSpace") == 1,
						CalculateBandwidth = (int)e.Attribute("calculateBandwidth") == 1
					});
				PackageResources.AddRange(resources);

				var quotas = plan
					.Element("quotas")
					.Elements()
					.Select(e => new Data.Entities.PackageQuota
					{
						PackageId = packageId,
						QuotaId = (int)e.Attribute("id"),
						QuotaValue = (int)e.Attribute("value")
					});
				PackageQuotas.AddRange(quotas);
			}

			SaveChanges();
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet UpdatePackage(int actorId, int packageId, int planId, string packageName,
			string packageComments, int statusId, DateTime purchaseDate,
			bool overrideQuotas, string quotasXml, bool defaultTopPackage)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var package = Packages
					.Where(p => p.PackageId == packageId)
					.FirstOrDefault();
				if (package != null)
				{
					using (var transaction = Database.BeginTransaction())
					{
						var oldPlanId = package.PlanId;
                        var parentPackageId = package.ParentPackageId;
                        package.PackageName = packageName;
						package.PackageComments = packageComments;
						package.StatusId = statusId;
						package.PlanId = planId;
						package.PurchaseDate = purchaseDate;
						package.OverrideQuotas = overrideQuotas;
						package.DefaultTopPackage = defaultTopPackage;
						SaveChanges();

						// update quotas (if required)
						UpdatePackageQuotas(actorId, packageId, quotasXml);

						// check exceeding quotas if plan has been changed
						/// <summary>TODO</summary>
						ExceedingQuota[] exceedingQuotas;

						exceedingQuotas = oldPlanId != planId || overrideQuotas
							? GetPackageExceedingQuotas(parentPackageId)
								.Where(q => q.QuotaValue > 0)
								.ToArray()
							: Array.Empty<ExceedingQuota>();

						if (exceedingQuotas.Any()) transaction.Rollback();
						else transaction.Commit();

						/// <summary>TODO</summary>
						return EntityDataSet(exceedingQuotas);
					}
				}
				/// <summary>TODO</summary>
				else return EntityDataSet(Array.Empty<ExceedingQuota>());
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdatePackage",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageName", packageName),
					/// <summary>TODO</summary>
					new SqlParameter("@packageComments", packageComments),
					/// <summary>TODO</summary>
					new SqlParameter("@statusId", statusId),
					/// <summary>TODO</summary>
					new SqlParameter("@planId", planId),
					/// <summary>TODO</summary>
					new SqlParameter("@purchaseDate", purchaseDate),
					/// <summary>TODO</summary>
					new SqlParameter("@overrideQuotas", overrideQuotas),
					/// <summary>TODO</summary>
					new SqlParameter("@quotasXml", quotasXml),
					/// <summary>TODO</summary>
					new SqlParameter("@defaultTopPackage", defaultTopPackage));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void ChangePackageUser(int actorId, int packageId, int userId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

                Packages
					.Where(p => p.PackageId == packageId)
					.ExecuteUpdate(p => new Data.Entities.Package { UserId = userId });
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "ChangePackageUser",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserId", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdatePackageName(int actorId, int packageId, string packageName,
			string packageComments)
		{
			if (UseEntityFramework)
			{
				var userId = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => p.UserId)
					.FirstOrDefault();

				if (!CheckActorPackageRights(actorId, packageId) || userId == actorId ||
					Users.Any(u => u.OwnerId == userId && u.IsPeer))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

                // update package
                Packages
                    .Where(p => p.PackageId == packageId)
                    .ExecuteUpdate(p => new Data.Entities.Package
                    {
                        PackageName = packageName,
                        PackageComments = packageComments
                    });
            }
            else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdatePackageName",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageName", packageName),
					/// <summary>TODO</summary>
					new SqlParameter("@packageComments", packageComments));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeletePackage(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					// remove package from cache
					PackagesTreeCaches.Where(p => p.ParentPackageId == packageId || p.PackageId == packageId)
						.ExecuteDelete();

					// delete package comments
					Comments.Where(c => c.ItemId == packageId && c.ItemTypeId == "PACKAGE")
						.ExecuteDelete();

					// delete diskspace
					PackagesDiskspaces.Where(d => d.PackageId == packageId).ExecuteDelete();

					// delete bandwidth
					PackagesBandwidths.Where(b => b.PackageId == packageId).ExecuteDelete();

					// delete settings
					PackageSettings.Where(s => s.PackageId == packageId).ExecuteDelete();

					// delete domains
					Domains.Where(d => d.PackageId == packageId).ExecuteDelete();

					// delete package IP addresses
					PackageIpAddresses.Where(ip => ip.PackageId == packageId).ExecuteDelete();

					// delete service items
					ServiceItems.Where(s => s.PackageId == packageId).ExecuteDelete();

					// delete global DNS records
					GlobalDnsRecords.Where(r => r.PackageId == packageId).ExecuteDelete();

					// delete package services
					PackageServices.Where(s => s.PackageId == packageId).ExecuteDelete();

					// delete package quotas
					PackageQuotas.Where(q => q.PackageId == packageId).ExecuteDelete();

					// delete package resources
					PackageResources.Where(r => r.PackageId == packageId).ExecuteDelete();

					// delete package
					Packages.Where(p => p.PackageId == packageId).ExecuteDelete();

					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeletePackage",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}

		// Package Add-ons
		/// <summary>Auto-generated member.</summary>
		public DataSet GetPackageAddons(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var addons = PackageAddons
					.Where(a => a.PackageId == packageId)
					.Join(HostingPlans, a => a.PlanId, hp => hp.PlanId, (a, hp) => new
					{
						a.PackageAddonId,
						a.PackageId,
						a.PlanId,
						a.Quantity,
						a.PurchaseDate,
						a.StatusId,
						a.Comments,
						hp.PlanName,
						hp.PlanDescription
					});
				/// <summary>TODO</summary>
				return EntityDataSet(addons);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackageAddons",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackageAddon(int actorId, int packageAddonId)
		{
			if (UseEntityFramework)
			{
				var packageId = PackageAddons
					.Where(p => p.PackageAddonId == packageAddonId)
					.Select(p => p.PackageId)
					.FirstOrDefault();
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var addon = PackageAddons
					.Where(a => a.PackageAddonId == packageAddonId)
					.Select(a => new
					{
						a.PackageAddonId,
						a.PackageId,
						a.PlanId,
						a.PurchaseDate,
						a.Quantity,
						a.StatusId,
						a.Comments
					});
				/// <summary>TODO</summary>
				return EntityDataReader(addon);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackageAddon",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageAddonID", packageAddonId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet AddPackageAddon(int actorId, out int addonId, int packageId, int planId, int quantity,
			int statusId, DateTime purchaseDate, string comments)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var parentPackageId = Packages
					.Where(p => p.PackageId == packageId)
					.Select(p => p.ParentPackageId)
					.FirstOrDefault();
				using (var transaction = Database.BeginTransaction())
				{
					var addon = new Data.Entities.PackageAddon()
					{
						PackageId = packageId,
						PlanId = planId,
						PurchaseDate = purchaseDate,
						Quantity = quantity,
						StatusId = statusId,
						Comments = comments
					};
					PackageAddons.Add(addon);
					SaveChanges();

					addonId = addon.PackageAddonId;

					var exceedingQuotas = GetPackageExceedingQuotas(parentPackageId)
						.Where(q => q.QuotaValue > 0)
						.ToList();

					if (exceedingQuotas.Any()) transaction.Rollback();
					else transaction.Commit();

					/// <summary>TODO</summary>
					return EntityDataSet(exceedingQuotas);
				}
			}
			else
			{
				SqlParameter prmPackageAddonId = new SqlParameter("@PackageAddonID", SqlDbType.Int);
				prmPackageAddonId.Direction = ParameterDirection.Output;

				DataSet ds = SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddPackageAddon",
					prmPackageAddonId,
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@planId", planId),
					/// <summary>TODO</summary>
					new SqlParameter("@Quantity", quantity),
					/// <summary>TODO</summary>
					new SqlParameter("@statusId", statusId),
					/// <summary>TODO</summary>
					new SqlParameter("@PurchaseDate", purchaseDate),
					/// <summary>TODO</summary>
					new SqlParameter("@Comments", comments));

				// read identity
				addonId = Convert.ToInt32(prmPackageAddonId.Value);

				/// <summary>TODO</summary>
				return ds;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet UpdatePackageAddon(int actorId, int packageAddonId, int planId, int quantity,
			int statusId, DateTime purchaseDate, string comments)
		{
			if (UseEntityFramework)
			{
				var package = PackageAddons
					.FirstOrDefault(a => a.PackageAddonId == packageAddonId);
				if (package == null)
				{
					/// <summary>TODO</summary>
					return EntityDataSet(Array.Empty<ExceedingQuota>());
				}

				if (!CheckActorPackageRights(actorId, package.PackageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					var parentPackageId = Packages
						.Where(p => p.PackageId == package.PackageId)
						.Select(p => p.ParentPackageId)
						.FirstOrDefault();

					package.PlanId = planId;
					package.Quantity = quantity;
					package.PurchaseDate = purchaseDate;
					package.StatusId = statusId;
					package.Comments = comments;
					SaveChanges();

					var exceedingQuotas = GetPackageExceedingQuotas(parentPackageId)
						.Where(q => q.QuotaValue > 0)
						.ToList();

					if (exceedingQuotas.Any())
					{
						transaction.Rollback();
					}
					else
					{
						transaction.Commit();
					}

					/// <summary>TODO</summary>
					return EntityDataSet(exceedingQuotas);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdatePackageAddon",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageAddonID", packageAddonId),
					/// <summary>TODO</summary>
					new SqlParameter("@planId", planId),
					/// <summary>TODO</summary>
					new SqlParameter("@Quantity", quantity),
					/// <summary>TODO</summary>
					new SqlParameter("@statusId", statusId),
					/// <summary>TODO</summary>
					new SqlParameter("@PurchaseDate", purchaseDate),
					/// <summary>TODO</summary>
					new SqlParameter("@Comments", comments));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeletePackageAddon(int actorId, int packageAddonId)
		{
			if (UseEntityFramework)
			{
				var package = PackageAddons
					.FirstOrDefault(a => a.PackageAddonId == packageAddonId);

				if (package != null)
				{
					if (!CheckActorPackageRights(actorId, package.PackageId))
						/// <summary>TODO</summary>
						throw new AccessViolationException("You are not allowed to access this package");

					PackageAddons.Remove(package);

					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeletePackageAddon",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageAddonID", packageAddonId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateServerPackageServices(int serverId)
		{
			// FIXME
			const int defaultActorID = 1; // serveradmin

			if (UseEntityFramework)
			{
				// get server packages
				var packages = Packages
					.Where(p => p.ServerId == serverId);
				// call DistributePackageServices for all packages on this server
				foreach (var package in packages)
					DistributePackageServices(defaultActorID, package.PackageId);
			}
			else
			{
				// get server packages
				IDataReader packagesReader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.Text,
					 @"SELECT PackageID FROM Packages WHERE ServerID = @ServerID",
					 /// <summary>TODO</summary>
					 new SqlParameter("@ServerID", serverId));

				// call DistributePackageServices for all packages on this server
				while (packagesReader.Read())
				{
					int packageId = (int)packagesReader["PackageID"];
					DistributePackageServices(defaultActorID, packageId);
				}
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DistributePackageServices(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				if (packageId <= 1) return;

				var random = new Random();

				var packages = Packages
					.Include(p => p.Server)
					.Where(p => p.PackageId == packageId)
					.Select(p => new
					{
						Package = p,
						Server = p.Server
					});
				var package = packages.FirstOrDefault();
				var services = PackageServices
					.Where(ps => ps.PackageId == packageId)
					.Join(Services, ps => ps.ServiceId, s => s.ServiceId, (ps, s) => new
					{
						s.ServerId,
						s.ServiceId,
						s.Provider.GroupId
					})
					.ToList();

				// get the list of available groups from hosting plan
				var packageGroups = services
					.Select(s => s.GroupId)
					.ToList();
				var groups = ResourceGroups
					.Where(r => !packageGroups.Contains(r.GroupId))
                    .Select(r => new { r.GroupId, PrimaryGroup = r.GroupId == package.Server.PrimaryGroupId })
                    .AsEnumerable()
                    .Where(r => Clone.GetPackageAllocatedResource(packageId, r.GroupId, null));

                if (!package.Server.VirtualServer)
				{   // Physical Server
					// just return the list of services based on the plan
					using (var groupIds = new TempIdSet(this, groups.Select(g => g.GroupId)))
					{
						var servicesSet = services.Select(s => s.ServiceId).ToHashSet();
                        foreach (var serviceId in Services
                            .Where(s => s.ServerId == package.Package.ServerId)
                            .Join(groupIds, s => s.Provider.GroupId, g => g, (s, g) => s.ServiceId))
                        {
                            if (!servicesSet.Contains(serviceId))
                            {
                                PackageServices.Add(new Data.Entities.PackageService()
                                {
                                    PackageId = packageId,
                                    ServiceId = serviceId
                                });
                            }
                        }
					}
                }
				else
				{   // Virtual Server
					var primaryGroupId = package.Server.PrimaryGroupId;

					foreach (var group in groups.OrderByDescending(g => g.PrimaryGroup))
					{
						// read group information
						var virtualGroup = VirtualGroups
							.Where(v => v.ServerId == package.Package.ServerId && v.GroupId == group.GroupId)
							.Select(v => new { v.DistributionType, v.BindDistributionToPrimary })
							.FirstOrDefault();
						var virtualServices = VirtualServices
							.Include(v => v.Service)
							.Where(v => v.ServerId == package.Package.ServerId &&
								v.Service.Provider.GroupId == group.GroupId);

						// bind distribution to primary
						if (virtualGroup.BindDistributionToPrimary == true && !group.PrimaryGroup &&
							primaryGroupId != 0)
						{
							// if only one service found just use it and do not distribute
							if (virtualServices.Count() == 1)
							{
								PackageServices.Add(virtualServices
									.Select(v => v.ServiceId)
									.Take(1)
									.AsEnumerable()
									.Select(serviceId => new Data.Entities.PackageService()
									{
										PackageId = packageId,
										ServiceId = serviceId
									})
									.First());
							}
							else
							{
								// try to get primary distribution server
								var primaryServerId = services
									.Where(s => s.GroupId == primaryGroupId)
									.Select(s => s.ServerId)
									.FirstOrDefault();
								foreach (var virtualServiceId in virtualServices
									.Where(v => v.Service.ServerId == primaryServerId)
									.Select(vs => vs.ServiceId))
								{
									PackageServices.Add(new Data.Entities.PackageService()
									{
										PackageId = packageId,
										ServiceId = virtualServiceId
									});
								}
							}
						}
						else // Distribution
						{
							var vservices = virtualServices
								.Select(v => new
								{
									v.ServiceId,
									ItemsNumber = v.Service.ServiceItems.Count(),
									//RandomNumber = random.Next(),
									v.Service
								});

							if (virtualGroup.DistributionType == 1) // Balanced distribution
							{
								// get the less allocated service
								var service = vservices
									.OrderBy(s => s.ItemsNumber)
									.FirstOrDefault();
								if (service != null)
								{
									PackageServices.Add(new Data.Entities.PackageService()
									{
										PackageId = packageId,
										ServiceId = service.ServiceId
									});
								}
							}
							else // Randomized distribution
							{
                                var serviceId = -1;
                                var n = vservices.Count();
                                if (n > 0)
                                {
                                    if (n == 1) serviceId = vservices.First().ServiceId;
                                    else
                                    {
                                        var i = random.Next(0, n);
                                        serviceId = vservices
                                            .Select(vs => vs.ServiceId)
                                            .Skip(i)
                                            .First();
                                    }
                                }
                                if (serviceId > 0)
                                {
                                    PackageServices.Add(new Data.Entities.PackageService()
                                    {
                                        PackageId = packageId,
                                        ServiceId = serviceId
                                    });
                                }
                            }
                        }

						if (group.PrimaryGroup) primaryGroupId = group.GroupId;
					}
				}

				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DistributePackageServices",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
			}
		}

		#endregion

		#region Packages Settings
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetPackageSettings(int actorId, int packageId, string settingsName)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				if (packageId <= 1) // system package
				{
					/// <summary>TODO</summary>
					return EntityDataReader(new Data.Entities.PackageSetting[]
					{
						new Data.Entities.PackageSetting() { PackageId = packageId, PropertyName = "Dump", PropertyValue = "" }
					});
				}
				else // user package
				{
					int? pid = packageId;
					Data.Entities.PackageSetting settings = null;
					while (pid != null)
					{
						settings = PackageSettings
							.FirstOrDefault(s => s.SettingsName == settingsName && s.PackageId == pid);
						if (settings != null) break;

						// get owner
						pid = Packages
							.Where(p => p.PackageId == pid)
							.Select(p => p.ParentPackageId)
							.FirstOrDefault();
					}

					return settings != null ? EntityDataReader(new Data.Entities.PackageSetting[] { settings }) : EntityDataReader(Array.Empty<Data.Entities.PackageSetting>());









				}
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackageSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@SettingsName", settingsName));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void UpdatePackageSettings(int actorId, int packageId, string settingsName, string xml)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					// delete old properties
					PackageSettings
						.Where(s => s.PackageId == packageId && s.SettingsName == settingsName)
						.ExecuteDelete();

					var settings = XElement.Parse(xml)
						.Elements()
						.Select(e => new Data.Entities.PackageSetting
						{
							PackageId = packageId,
							SettingsName = settingsName,
							PropertyName = (string)e.Attribute("name"),
							PropertyValue = (string)e.Attribute("value")
						});
					PackageSettings.AddRange(settings);
					SaveChanges();

					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdatePackageSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@SettingsName", settingsName),
					/// <summary>TODO</summary>
					new SqlParameter("@Xml", xml));
			}
		}
		#endregion

		#region Quotas
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetProviderServiceQuota(int providerId)
		{
			if (UseEntityFramework)
			{
				var quota = Providers
					.Where(p => p.ProviderId == providerId)
					.Join(Quotas, p => p.GroupId, q => q.GroupId, (p, q) => q)
					.Where(q => q.ServiceQuota == true)
					.Take(1);
				/// <summary>TODO</summary>
				return EntityDataReader(quota);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetProviderServiceQuota",
					/// <summary>TODO</summary>
					new SqlParameter("@providerId", providerId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackageQuota(int actorId, int packageId, string quotaName)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var orgsCount = GetPackageAllocatedQuota(packageId, 205); // 205 - HostedSolution.Organizations
				if (orgsCount < 1) orgsCount = 1;

				var quotas = Quotas
					.Where(q => q.QuotaName == quotaName)
					.AsEnumerable()
					.Select(q => new
					{
						Quota = q,
						AllocatedQuota = Clone.GetPackageAllocatedQuota(packageId, q.QuotaId)
					})
					.Select(q => new
					{
						q.Quota.QuotaId,
						q.Quota.QuotaName,
						q.Quota.QuotaDescription,
						q.Quota.QuotaTypeId,
						QuotaAllocatedValue = q.Quota.PerOrganization == 1 && q.AllocatedQuota != -1 ?
							q.AllocatedQuota * orgsCount : q.AllocatedQuota,
						QuotaAllocatedValuePerOrganization = q.AllocatedQuota,
						QuotaUsedValue = Clone.CalculateQuotaUsage(packageId, q.Quota.QuotaId)
					});
				/// <summary>TODO</summary>
				return EntityDataReader(quotas);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPackageQuota",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@QuotaName", quotaName));
			}
		}
		#endregion

		#region Log
		/// <summary>Auto-generated member.</summary>
		public void AddAuditLogRecord(string recordId, int severityId,
			int userId, string username, int packageId, int itemId, string itemName, DateTime startDate, DateTime finishDate, string sourceName,
			string taskName, string executionLog)
		{
			if (UseEntityFramework)
			{
				var log = new Data.Entities.AuditLog()
				{
					RecordId = recordId,
					SeverityId = severityId,
					UserId = userId != 0 && userId != -1 ? userId : null,
					PackageId = packageId,
					Username = username,
					ItemId = itemId != 0 ? itemId : null,
					SourceName = sourceName,
					StartDate = startDate,
					FinishDate = finishDate,
					TaskName = taskName,
					ItemName = itemName,
					ExecutionLog = executionLog
				};
				AuditLogs.Add(log);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddAuditLogRecord",
					/// <summary>TODO</summary>
					new SqlParameter("@recordId", recordId),
					/// <summary>TODO</summary>
					new SqlParameter("@severityId", severityId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@username", username),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@startDate", startDate),
					/// <summary>TODO</summary>
					new SqlParameter("@finishDate", finishDate),
					/// <summary>TODO</summary>
					new SqlParameter("@sourceName", sourceName),
					/// <summary>TODO</summary>
					new SqlParameter("@taskName", taskName),
					/// <summary>TODO</summary>
					new SqlParameter("@executionLog", executionLog));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetAuditLogRecordsPaged(int actorId, int userId, int packageId, int itemId, string itemName, DateTime startDate, DateTime endDate,
			int severityId, string sourceName, string taskName, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				if (sourceName == null) sourceName = "";
				if (taskName == null) taskName = "";
				if (itemName == null) itemName = "";

				var isAdmin = Users.Any(u => u.UserId == actorId && u.RoleId == 1);

				TempIdSet childUsers = null;
				try
				{
					IQueryable<Data.Entities.AuditLog> logs = AuditLogs;

					if (packageId == 0)
					{
						childUsers = UserChildren(userId);
						logs = logs
							.Join(childUsers, l => l.UserId, u => u, (l, u) => l);
						if (isAdmin) logs = logs.Concat(AuditLogs.Where(l => l.UserId == null));
					}
					else
					{
						logs = logs.Where(l => l.UserId == null && isAdmin || l.PackageId == packageId);
					}

					logs = logs
						.Where(l => startDate <= l.StartDate && l.StartDate < endDate &&
							(sourceName == "" || l.SourceName == sourceName) &&
							(taskName == "" || l.TaskName == taskName) &&
							(itemId == 0 || itemId > 0 && l.ItemId == itemId) &&
#if NETFRAMEWORK
							(itemName == "" || DbFunctions.Like(l.ItemName, itemName)) &&
#else
							(itemName == "" || EF.Functions.Like(l.ItemName, itemName)) &&
#endif
							(severityId == -1 || severityId > -1 && l.SeverityId == severityId));

					var count = logs.Count();

					logs = !string.IsNullOrEmpty(sortColumn) ? logs.OrderBy(ColumnName(sortColumn)) : logs.OrderByDescending(l => l.StartDate);


					logs = logs.Skip(startRow).Take(maximumRows);

					var logsWithUser = logs
						.GroupJoin(UsersDetailed, l => l.UserId, u => u.UserId, (l, u) => new
						{
							l.RecordId,
							l.SeverityId,
							l.StartDate,
							l.FinishDate,
							l.ItemId,
							l.SourceName,
							l.TaskName,
							l.ItemName,
							l.ExecutionLog,
                            UserId = l.UserId != null ? l.UserId : 0,
                            l.Username,
							Users = u
						})
						.SelectMany(l => l.Users.DefaultIfEmpty(), (l, u) => new
						{
							l.RecordId,
							l.SeverityId,
							l.StartDate,
							l.FinishDate,
							l.ItemId,
							l.SourceName,
							l.TaskName,
							l.ItemName,
							l.ExecutionLog,
							l.UserId,
							l.Username,
							FirstName = u != null ? u.FirstName : null,
							LastName = u != null ? u.LastName : null,
							FullName = u != null ? u.FullName : null,
							RoleId = u != null ? u.RoleId : 0,
							Email = u != null ? u.Email : null,
							EffectiveUserId = u != null ?
								(u.IsPeer ? u.OwnerId : u.UserId) : null
						});
					/// <summary>TODO</summary>
					return EntityDataSet(count, logsWithUser);
				}
				finally
				{
					childUsers?.Dispose();
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetAuditLogRecordsPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@StartDate", startDate),
					/// <summary>TODO</summary>
					new SqlParameter("@EndDate", endDate),
					/// <summary>TODO</summary>
					new SqlParameter("@severityId", severityId),
					/// <summary>TODO</summary>
					new SqlParameter("@sourceName", sourceName),
					/// <summary>TODO</summary>
					new SqlParameter("@taskName", taskName),
					/// <summary>TODO</summary>
					new SqlParameter("@sortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetAuditLogSources()
		{
			if (UseEntityFramework)
			{
				var sources = AuditLogSources.Select(l => new { l.SourceName });
				/// <summary>TODO</summary>
				return EntityDataSet(sources);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetAuditLogSources");
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetAuditLogTasks(string sourceName)
		{
			if (UseEntityFramework)
			{
				if (sourceName == "") sourceName = null;
				var tasks = AuditLogTasks
					.Where(t => sourceName == null || t.SourceName == sourceName)
					.Select(t => new { t.SourceName, t.TaskName });
				/// <summary>TODO</summary>
				return EntityDataSet(tasks);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetAuditLogTasks",
					/// <summary>TODO</summary>
					new SqlParameter("@sourceName", sourceName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetAuditLogRecord(string recordId)
		{
			if (UseEntityFramework)
			{
				var logsWithUser = AuditLogs
					.Where(l => l.RecordId == recordId)
					.GroupJoin(UsersDetailed, l => l.UserId, u => u.UserId, (l, u) => new
					{
						l.RecordId,
						l.SeverityId,
						l.StartDate,
						l.FinishDate,
						l.ItemId,
						l.SourceName,
						l.TaskName,
						l.ItemName,
						l.ExecutionLog,
						UserId = l.UserId ?? 0,
						l.Username,
						Users = u
					})
					.SelectMany(l => l.Users.DefaultIfEmpty(), (l, u) => new
					{
						l.RecordId,
						l.SeverityId,
						l.StartDate,
						l.FinishDate,
						l.ItemId,
						l.SourceName,
						l.TaskName,
						l.ItemName,
						l.ExecutionLog,
						l.UserId,
						l.Username,
						FirstName = u != null ? u.FirstName : null,
						LastName = u != null ? u.LastName : null,
						FullName = u != null ? u.FullName : null,
						RoleId = u != null ? u.RoleId : 0,
						Email = u != null ? u.Email : null,
					});
				/// <summary>TODO</summary>
				return EntityDataReader(logsWithUser);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetAuditLogRecord",
					/// <summary>TODO</summary>
					new SqlParameter("@recordId", recordId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteAuditLogRecords(int actorId, int userId, int itemId, string itemName, DateTime startDate, DateTime endDate,
			int severityId, string sourceName, string taskName)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

				var isAdmin = Users.Any(u => u.UserId == actorId && u.RoleId == 1);

				using (var childUsers = UserChildren(userId))
				{
					var logs = AuditLogs
						.Where(l => startDate <= l.StartDate && l.StartDate < endDate &&
							(string.IsNullOrEmpty(sourceName) || l.SourceName == sourceName) &&
							(string.IsNullOrEmpty(taskName) || l.TaskName == taskName) &&
							(itemId <= 0 || itemId == l.ItemId) &&
#if NETFRAMEWORK
							(string.IsNullOrEmpty(itemName) || DbFunctions.Like(l.ItemName, itemName)));
#else
							(string.IsNullOrEmpty(itemName) || EF.Functions.Like(l.ItemName, itemName)));
#endif
					logs = logs.Where(l => l.UserId == null && isAdmin)
						.Concat(logs
							.Join(childUsers, l => l.UserId, u => u, (l, u) => l));
					logs.ExecuteDelete();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteAuditLogRecords",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@startDate", startDate),
					/// <summary>TODO</summary>
					new SqlParameter("@endDate", endDate),
					/// <summary>TODO</summary>
					new SqlParameter("@severityId", severityId),
					/// <summary>TODO</summary>
					new SqlParameter("@sourceName", sourceName),
					/// <summary>TODO</summary>
					new SqlParameter("@taskName", taskName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteAuditLogRecordsComplete()
		{
			if (UseEntityFramework)
			{
				if (!IsSqlite)
				{
#if NETCOREAPP
					Database.ExecuteSqlRaw("TRUNCATE TABLE AuditLog");
#else
					Database.ExecuteSqlCommand("TRUNCATE TABLE AuditLog");
#endif
				}
				else
				{
#if NETCOREAPP
					Database.ExecuteSqlRaw("DELETE FROM AuditLog");
#else
					Database.ExecuteSqlCommand("DELETE FROM AuditLog");
#endif
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteAuditLogRecordsComplete");
			}
		}

		#endregion

		#region Reports
		/// <summary>Auto-generated member.</summary>
		public DataSet GetPackagesBandwidthPaged(int actorId, int userId, int packageId,
			DateTime startDate, DateTime endDate, string sortColumn,
			int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var packagesGrouped = Packages
					.Join(PackagesTreeCaches, p => p.PackageId, t => t.ParentPackageId, (p, t) => new { P = p, T = t })
					.Join(Packages, pp => pp.T.PackageId, pc => pc.PackageId, (pp, pc) => new { pp.P, pp.T, PC = pc })
					.Join(PackagesBandwidths, pp => pp.PC.PackageId, pb => pb.PackageId, (pp, pb) => new { pp.P, pp.PC, PB = pb })
					.Join(HostingPlanResources, pp => new { pp.PC.PlanId, pp.PB.GroupId }, hpr => new { PlanId = (int?)hpr.PlanId, hpr.GroupId },
						(pp, hpr) => new { pp.P, pp.PC, pp.PB, HPR = hpr })
					.Where(p => startDate <= p.PB.LogDate && p.PB.LogDate < endDate && p.HPR.CalculateBandwidth == true)
					.GroupBy(p => p.P.PackageId)
					.Select(p => new
					{
						PackageId = p.Key,
						// QuotaValue = GetPackageAllocatedQuota(p.Key, 51),
						Bandwidth = p.Sum(s => (long?)(s.PB.BytesSent + s.PB.BytesReceived)) ?? 0
					});
				var packages = Packages
					.Where(p => packageId == -1 && p.UserId == userId ||
						packageId != -1 && p.ParentPackageId == packageId)
					.GroupJoin(packagesGrouped, p => p.PackageId, pg => pg.PackageId, (p, pg) => new
					{
						Package = p,
						PackageGroup = pg
					})
					.SelectMany(p => p.PackageGroup.DefaultIfEmpty(), (p, pg) => new
					{
						p.Package.PackageId,
						p.Package.PackageName,
						p.Package.StatusId,
						p.Package.UserId,
						Bandwidth = pg != null ? (long?)pg.Bandwidth : null,
						GroupedPackageId = pg != null ? (int?)pg.PackageId : null
					})
					.Join(UsersDetailed, p => p.UserId, u => u.UserId, (p, u) => new
					{
						p.PackageId,
						Bandwidth = p.Bandwidth ?? 0,
						p.GroupedPackageId,
						//UsagePercentage = p.PG != null ? (p.PG.QuotaValue > 0 ? p.PG.Bandwidth * 100 / p.PG.QuotaValue : 0) : 0,
						PackagesNumber = Packages.Count(np => np.ParentPackageId == p.PackageId),
						p.PackageName,
						p.StatusId,
						p.UserId,
						u.Username,
						u.FirstName,
						u.LastName,
						u.FullName,
						u.RoleId,
						u.Email,
						//UserComments = GetItemComments(u.UserId, "USER", actorId)
					});

				var count = packages.Count();

				if (!string.IsNullOrEmpty(sortColumn) && !sortColumn.StartsWith("PackagesNumber") &&
					!sortColumn.StartsWith("QuotaValue"))
				{
					packages = packages.OrderBy(ColumnName(sortColumn));
					packages = packages.Skip(startRow).Take(maximumRows);
				}

				var packagesSelected = packages
					.AsEnumerable()
					.Select(p => new
					{
						Package = p,
						QuotaValue = p.GroupedPackageId != null ?
							Clone.GetPackageAllocatedQuota(p.GroupedPackageId, 51) : 0 // 51 - Bandwidth
                    })
					.Select(p => new
					{
						p.Package.PackageId,
						p.Package.Bandwidth,
						p.Package.PackageName,
						p.QuotaValue,
						UsagePercentage = (int)(p.QuotaValue > 0 ? p.Package.Bandwidth * 100 / p.QuotaValue : 0),
						//PackagesNumber = Local.Packages.Count(np => np.ParentPackageId == p.Package.PackageId),
						p.Package.PackagesNumber,
						p.Package.StatusId,
						p.Package.UserId,
						p.Package.Username,
						p.Package.FirstName,
						p.Package.LastName,
						p.Package.FullName,
						p.Package.RoleId,
						p.Package.Email,
						UserComments = Clone.GetItemComments(p.Package.UserId, "USER", actorId)
					});

				if (string.IsNullOrEmpty(sortColumn))
				{
					packagesSelected = packagesSelected
						.OrderByDescending(p => p.UsagePercentage)
						.Skip(startRow).Take(maximumRows);
				}
				/// <summary>TODO</summary>
				else if (sortColumn.StartsWith("PackagesNumber"))
				{
					packagesSelected = sortColumn.EndsWith(" desc", StringComparison.OrdinalIgnoreCase) ? packagesSelected.OrderByDescending(p => p.PackagesNumber) : packagesSelected.OrderBy(p => p.PackagesNumber);







					packagesSelected = packagesSelected.Skip(startRow).Take(maximumRows);
				}
				/// <summary>TODO</summary>
				else if (sortColumn.StartsWith("QuotaValue"))
				{
					packagesSelected = sortColumn.EndsWith(" desc", StringComparison.OrdinalIgnoreCase) ? packagesSelected.OrderByDescending(p => p.QuotaValue) : packagesSelected.OrderBy(p => p.QuotaValue);







					packagesSelected = packagesSelected.Skip(startRow).Take(maximumRows);
				}

				/// <summary>TODO</summary>
				return EntityDataSet(count, packagesSelected);
			}
			else
			{
				/// <summary>TODO</summary>
				return ExecuteLongDataSet(
					ObjectQualifier + "GetPackagesBandwidthPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@StartDate", startDate),
					/// <summary>TODO</summary>
					new SqlParameter("@EndDate", endDate),
					/// <summary>TODO</summary>
					new SqlParameter("@sortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetPackagesDiskspacePaged(int actorId, int userId, int packageId, string sortColumn,
			int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var packagesGrouped = Packages
					.Join(PackagesTreeCaches, p => p.PackageId, t => t.ParentPackageId, (p, t) => new { P = p, T = t })
					.Join(Packages, pp => pp.T.PackageId, pc => pc.PackageId, (pp, pc) => new { pp.P, pp.T, PC = pc })
					.Join(PackagesDiskspaces, pp => pp.PC.PackageId, pd => pd.PackageId, (pp, pd) => new { pp.P, pp.PC, PD = pd })
					.Join(HostingPlanResources, pp => new { pp.PC.PlanId, pp.PD.GroupId }, hpr => new { PlanId = (int?)hpr.PlanId, hpr.GroupId },
						(pp, hpr) => new { pp.P, pp.PC, pp.PD, HPR = hpr })
					.Where(p => p.HPR.CalculateDiskSpace == true)
					.GroupBy(p => p.P.PackageId)
					.Select(p => new
					{
						PackageId = p.Key,
						// QuotaValue = GetPackageAllocatedQuota(p.Key, 51),
						Diskspace = (p.Sum(s => (long?)s.PD.DiskSpace) ?? 0) / 1024 / 1024
					});
				var packages = Packages
					.Where(p => packageId == -1 && p.UserId == userId ||
						packageId != -1 && p.ParentPackageId == packageId)
					.GroupJoin(packagesGrouped, p => p.PackageId, pg => pg.PackageId, (p, pg) => new
					{
						Package = p,
						PackageGroup = pg
					})
					.SelectMany(p => p.PackageGroup.DefaultIfEmpty(), (p, pg) => new
					{
						p.Package.PackageId,
						p.Package.PackageName,
						p.Package.StatusId,
						p.Package.UserId,
						GroupedPackageId = pg != null ? (int?)pg.PackageId : null,
						Diskspace = pg != null ? (long?)pg.Diskspace : null
					})
					.Join(UsersDetailed, p => p.UserId, u => u.UserId, (p, u) => new
					{
						p.PackageId,
						p.GroupedPackageId,
						//QuotaValue = p.PG != null ? p.PG.QuotaValue : 0,
						p.Diskspace,
						//UsagePercentage = p.PG != null ? (p.PG.QuotaValue > 0 ? p.PG.Diskspace * 100 / p.PG.QuotaValue : 0) : 0,
						PackagesNumber = Packages.Count(np => np.ParentPackageId == p.PackageId),
						p.PackageName,
						p.StatusId,
						p.UserId,
						u.Username,
						u.FirstName,
						u.LastName,
						u.FullName,
						u.RoleId,
						u.Email,
						//UserComments = GetItemComments(u.UserId, "USER", actorId)
					});

				var count = packages.Count();

				if (!string.IsNullOrEmpty(sortColumn) && !sortColumn.StartsWith("PackagesNumber") &&
					!sortColumn.StartsWith("QuotaValue"))
				{
					packages = packages.OrderBy(ColumnName(sortColumn));
					packages = packages.Skip(startRow).Take(maximumRows);
				}

				var packagesSelected = packages
					.AsEnumerable()
					.Select(p => new
					{
						Package = p,
						QuotaValue = p.GroupedPackageId != null ?
							Clone.GetPackageAllocatedQuota(p.GroupedPackageId, 52) : 0, // 52 diskspace
                    })
					.Select(p => new
					{
						p.Package.PackageId,
						p.QuotaValue,
						Diskspace = p.Package.Diskspace ?? 0,
						UsagePercentage = (int)(p.QuotaValue > 0 ? (p.Package.Diskspace ?? 0) * 100 / p.QuotaValue : 0),
						//PackagesNumber = Local.Packages.Count(np => np.ParentPackageId == p.Package.PackageId),
						p.Package.PackagesNumber,
						p.Package.PackageName,
						p.Package.StatusId,
						p.Package.UserId,
						p.Package.Username,
						p.Package.FirstName,
						p.Package.LastName,
						p.Package.FullName,
						p.Package.RoleId,
						p.Package.Email,
						UserComments = Clone.GetItemComments(p.Package.UserId, "USER", actorId)
					});

				if (string.IsNullOrEmpty(sortColumn))
				{
					packagesSelected = packagesSelected
						.OrderByDescending(p => p.UsagePercentage);
					packagesSelected = packagesSelected.Skip(startRow).Take(maximumRows);
				}
				/// <summary>TODO</summary>
				else if (sortColumn.StartsWith("PackagesNumber"))
				{
					packagesSelected = sortColumn.EndsWith(" desc", StringComparison.OrdinalIgnoreCase)
						? packagesSelected.OrderByDescending(p => p.PackagesNumber)
						: packagesSelected.OrderBy(p => p.PackagesNumber);
					packagesSelected = packagesSelected.Skip(startRow).Take(maximumRows);
				}
				/// <summary>TODO</summary>
				else if (sortColumn.StartsWith("QuotaValue"))
				{
					packagesSelected = sortColumn.EndsWith(" desc", StringComparison.OrdinalIgnoreCase)
						? packagesSelected.OrderByDescending(p => p.QuotaValue)
						: packagesSelected.OrderBy(p => p.QuotaValue);
					packagesSelected = packagesSelected.Skip(startRow).Take(maximumRows);
				}

				/// <summary>TODO</summary>
				return EntityDataSet(count, packagesSelected);
			}
			else
			{
				/// <summary>TODO</summary>
				return ExecuteLongDataSet(
					ObjectQualifier + "GetPackagesDiskspacePaged",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@sortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetPackageBandwidth(int actorId, int packageId, DateTime startDate, DateTime endDate)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var packagesGrouped = PackagesTreeCaches
					.Where(p => p.ParentPackageId == packageId)
					.Join(PackagesBandwidths.Where(pb => startDate <= pb.LogDate && pb.LogDate < endDate),
						pt => pt.PackageId, pb => pb.PackageId, (pt, pb) => new { PT = pt, PB = pb })
					.Join(Packages, pb => pb.PB.PackageId, p => p.PackageId, (pb, p) => new { pb.PT, pb.PB, P = p })
					.Join(HostingPlanResources.Where(hp => hp.CalculateBandwidth == true), p => new { p.PB.GroupId, p.P.PlanId },
						hpr => new { hpr.GroupId, PlanId = (int?)hpr.PlanId }, (p, hpr) => new { p.P, p.PB, p.PT, HPR = hpr })
					.GroupBy(p => p.PB.GroupId)
					.Select(p => new
					{
						GroupId = p.Key,
						BytesSent = p.Sum(pb => (long?)pb.PB.BytesSent) ?? 0,
						BytesReceived = p.Sum(pb => (long?)pb.PB.BytesReceived) ?? 0
					});
				var packages = ResourceGroups
					.GroupJoin(packagesGrouped, r => r.GroupId, g => g.GroupId, (rg, pg) => new
					{
						rg.GroupId,
						rg.GroupName,
						rg.GroupOrder,
						PackageGroup = pg
					})
					.SelectMany(g => g.PackageGroup.DefaultIfEmpty(), (g, pg) => new
					{
						g.GroupId,
						g.GroupName,
						g.GroupOrder,
						MegaBytesSent = pg != null ? (long?)((pg.BytesSent + MB / 2) / MB) : null,
						MegaBytesReceived = pg != null ? (long?)((pg.BytesReceived + MB / 2) / MB) : null,
						MegaBytesTotal = pg != null ? (long?)((pg.BytesSent + pg.BytesReceived + MB / 2) / MB) : null,
						BytesSent = pg != null ? (long?)pg.BytesSent : null,
						BytesReceived = pg != null ? (long?)pg.BytesReceived : null,
						BytesTotal = pg != null ? (long?)(pg.BytesSent + pg.BytesReceived) : null
					})
					.Where(g => g.BytesTotal != null && g.BytesTotal.Value > 0)
					.OrderBy(g => g.GroupOrder)
					.Select(g => new
					{
						g.GroupId,
						g.GroupName,
                        MegaBytesSent = g.MegaBytesSent != null ? g.MegaBytesSent : 0,
                        MegaBytesReceived = g.MegaBytesReceived != null ? g.MegaBytesReceived : 0,
                        MegaBytesTotal = g.MegaBytesTotal != null ? g.MegaBytesTotal : 0,
                        BytesSent = g.BytesSent != null ? g.BytesSent : 0,
                        BytesReceived = g.BytesReceived != null ? g.BytesReceived : 0,
                        BytesTotal = g.BytesTotal != null ? g.BytesTotal : 0
                    });
				/// <summary>TODO</summary>
				return EntityDataSet(packages);
			}
			else
			{
				/// <summary>TODO</summary>
				return ExecuteLongDataSet(
					ObjectQualifier + "GetPackageBandwidth",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@StartDate", startDate),
					/// <summary>TODO</summary>
					new SqlParameter("@EndDate", endDate));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetPackageDiskspace(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var packagesGrouped = PackagesTreeCaches
				.Where(p => p.ParentPackageId == packageId)
					.Join(PackagesDiskspaces, pt => pt.PackageId, pd => pd.PackageId, (pt, pd) => new { PT = pt, PD = pd })
					.Join(Packages, pb => pb.PD.PackageId, p => p.PackageId, (pb, p) => new { pb.PT, pb.PD, P = p })
					.Join(HostingPlanResources.Where(hp => hp.CalculateDiskSpace == true), p => new { p.PD.GroupId, p.P.PlanId },
						hpr => new { hpr.GroupId, PlanId = (int?)hpr.PlanId }, (p, hpr) => new { p.P, p.PD, p.PT, HPR = hpr })
					.GroupBy(p => p.PD.GroupId)
					.Select(p => new
					{
						GroupId = p.Key,
						Diskspace = p.Sum(pb => (long?)pb.PD.DiskSpace) ?? 0
					});
				var packages = ResourceGroups
					.GroupJoin(packagesGrouped, r => r.GroupId, g => g.GroupId, (rg, pg) => new
					{
						rg.GroupId,
						rg.GroupName,
						rg.GroupOrder,
						PackageGroup = pg
					})
					.SelectMany(g => g.PackageGroup.DefaultIfEmpty(), (g, pg) => new
					{
						g.GroupId,
						g.GroupName,
						g.GroupOrder,
						Diskspace = pg != null ? (long?)((pg.Diskspace + MB / 2) / MB) : null,
						DiskspaceBytes = pg != null ? (long?)pg.Diskspace : null
					})
					.Where(g => g.DiskspaceBytes != null && g.DiskspaceBytes.Value > 0)
					.OrderBy(g => g.GroupOrder)
					.Select(g => new
					{
						g.GroupId,
						g.GroupName,
                        Diskspace = g.Diskspace.HasValue ? g.Diskspace.Value : 0,
                        DiskspaceBytes = g.DiskspaceBytes.HasValue ? g.DiskspaceBytes.Value : 0
                    });

				/// <summary>TODO</summary>
				return EntityDataSet(packages);
			}
			else
			{
				/// <summary>TODO</summary>
				return ExecuteLongDataSet(
					ObjectQualifier + "GetPackageDiskspace",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId));
			}
		}

		#endregion

		#region Scheduler
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetBackgroundTask(string taskId)
		{
			if (UseEntityFramework)
			{
				var tasks = BackgroundTasks
					.Where(t => t.TaskId == taskId)
					.Join(BackgroundTaskStacks, t => t.Id, ts => ts.TaskId, (t, ts) => new
					{
						t.Id,
						t.Guid,
						t.TaskId,
						t.ScheduleId,
						t.PackageId,
						t.UserId,
						t.EffectiveUserId,
						t.TaskName,
						t.ItemId,
						t.ItemName,
						t.StartDate,
						t.FinishDate,
						t.IndicatorCurrent,
						t.IndicatorMaximum,
						t.MaximumExecutionTime,
						t.Source,
						t.Severity,
						t.Completed,
						t.NotifyOnComplete,
						t.Status
					})
					.Take(1);

				/// <summary>TODO</summary>
				return EntityDataReader(tasks);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetBackgroundTask",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetScheduleBackgroundTasks(int scheduleId)
		{
			if (UseEntityFramework)
			{
				var tasksGuids = BackgroundTasks
					.Where(t => t.ScheduleId == scheduleId && t.Completed == false &&
						(t.Status == BackgroundTaskStatus.Run || t.Status == BackgroundTaskStatus.Starting))
					.Select(t => t.Guid);
				var tasks = BackgroundTasks
					.Join(tasksGuids, t => t.Guid, tg => tg, (t, tg) => new
					{
						t.Id,
						t.Guid,
						t.TaskId,
						t.ScheduleId,
						t.PackageId,
						t.UserId,
						t.EffectiveUserId,
						t.TaskName,
						t.ItemId,
						t.ItemName,
						t.StartDate,
						t.FinishDate,
						t.IndicatorCurrent,
						t.IndicatorMaximum,
						t.MaximumExecutionTime,
						t.Source,
						t.Severity,
						t.Completed,
						t.NotifyOnComplete,
						t.Status
					});
				/// <summary>TODO</summary>
				return EntityDataReader(tasks);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetScheduleBackgroundTasks",
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IEnumerable<int> GetChildUsersId(int userId)
		{
			if (Users.Any(u => u.UserId == userId)) yield return userId;

			var descendants = Users
				.Where(u => u.OwnerId == userId)
				.Select(u => u.UserId)
				.ToList();
			while (descendants.Any())
			{
				foreach (var child in descendants) yield return child;

				descendants = Users
					.Join(descendants, u => u.OwnerId, d => d, (u, d) => u.UserId)
					.ToList();
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetBackgroundTasks(int actorId)
		{
			if (UseEntityFramework)
			{
				var children = GetChildUsersId(actorId)
					.ToList();
				var tasksGrouped = BackgroundTasks
					.Join(children, t => t.UserId, uid => uid, (t, uid) => t)
					.Join(BackgroundTaskStacks, t => t.Id, ts => ts.TaskId, (t, ts) => t)
					.GroupBy(t => t.Guid)
					.Select(ts => new { Guid = ts.Key, Date = ts.Min(t => t.StartDate) });

				var tasks = BackgroundTasks
					.Join(tasksGrouped, t => new { t.Guid, t.StartDate }, tg => new { tg.Guid, StartDate = tg.Date }, (t, tg) => new
					{
						t.Id,
						t.Guid,
						t.TaskId,
						t.ScheduleId,
						t.PackageId,
						t.UserId,
						t.EffectiveUserId,
						t.TaskName,
						t.ItemId,
						t.ItemName,
						t.StartDate,
						t.FinishDate,
						t.IndicatorCurrent,
						t.IndicatorMaximum,
						t.MaximumExecutionTime,
						t.Source,
						t.Severity,
						t.Completed,
						t.NotifyOnComplete,
						t.Status
					});
				/// <summary>TODO</summary>
				return EntityDataReader(tasks);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetBackgroundTasks",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetBackgroundTasks(Guid guid)
		{
			if (UseEntityFramework)
			{
				var tasks = BackgroundTasks
					.Where(t => t.Guid == guid)
					.Join(BackgroundTaskStacks, t => t.Id, ts => ts.TaskId, (t, ts) => new
					{
						t.Id,
						t.Guid,
						t.TaskId,
						t.ScheduleId,
						t.PackageId,
						t.UserId,
						t.EffectiveUserId,
						t.TaskName,
						t.ItemId,
						t.ItemName,
						t.StartDate,
						t.FinishDate,
						t.IndicatorCurrent,
						t.IndicatorMaximum,
						t.MaximumExecutionTime,
						t.Source,
						t.Severity,
						t.Completed,
						t.NotifyOnComplete,
						t.Status
					});
				/// <summary>TODO</summary>
				return EntityDataReader(tasks);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetThreadBackgroundTasks",
					/// <summary>TODO</summary>
					new SqlParameter("@guid", guid));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetProcessBackgroundTasks(BackgroundTaskStatus status)
		{
			if (UseEntityFramework)
			{
				var tasks = BackgroundTasks
					.Where(t => t.Completed == false && t.Status == status)
					.Select(t => new
					{
						t.Id,
						t.TaskId,
						t.ScheduleId,
						t.PackageId,
						t.UserId,
						t.EffectiveUserId,
						t.TaskName,
						t.ItemId,
						t.ItemName,
						t.StartDate,
						t.FinishDate,
						t.IndicatorCurrent,
						t.IndicatorMaximum,
						t.MaximumExecutionTime,
						t.Source,
						t.Severity,
						t.Completed,
						t.NotifyOnComplete,
						t.Status
					});
				/// <summary>TODO</summary>
				return EntityDataReader(tasks);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetProcessBackgroundTasks",
					/// <summary>TODO</summary>
					new SqlParameter("@status", (int)status));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetBackgroundTopTask(Guid guid)
		{
			if (UseEntityFramework)
			{
				var tasks = BackgroundTasks
					.Where(t => t.Guid == guid)
					.Join(BackgroundTaskStacks, t => t.Id, ts => ts.TaskId, (t, ts) => t)
					.OrderBy(t => t.StartDate)
					.Take(1)
					.Select(t => new
					{
						t.Id,
						t.Guid,
						t.TaskId,
						t.ScheduleId,
						t.PackageId,
						t.UserId,
						t.EffectiveUserId,
						t.TaskName,
						t.ItemId,
						t.ItemName,
						t.StartDate,
						t.FinishDate,
						t.IndicatorCurrent,
						t.IndicatorMaximum,
						t.MaximumExecutionTime,
						t.Source,
						t.Severity,
						t.Completed,
						t.NotifyOnComplete,
						t.Status
					});
				/// <summary>TODO</summary>
				return EntityDataReader(tasks);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetBackgroundTopTask",
					/// <summary>TODO</summary>
					new SqlParameter("@guid", guid));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddBackgroundTask(Guid guid, string taskId, int scheduleId, int packageId, int userId,
			int effectiveUserId, string taskName, int itemId, string itemName, DateTime startDate,
			int indicatorCurrent, int indicatorMaximum, int maximumExecutionTime, string source,
			int severity, bool completed, bool notifyOnComplete, BackgroundTaskStatus status)
		{
			if (UseEntityFramework)
			{
				var task = new Data.Entities.BackgroundTask()
				{
					Guid = guid,
					TaskId = taskId,
					ScheduleId = scheduleId,
					PackageId = packageId,
					UserId = userId,
					EffectiveUserId = effectiveUserId,
					TaskName = taskName,
					ItemId = itemId,
					ItemName = itemName,
					StartDate = startDate,
					IndicatorCurrent = indicatorCurrent,
					IndicatorMaximum = indicatorMaximum,
					MaximumExecutionTime = maximumExecutionTime,
					Source = source,
					Severity = severity,
					Completed = completed,
					NotifyOnComplete = notifyOnComplete,
					Status = status
				};
				BackgroundTasks.Add(task);
				SaveChanges();
				return task.Id;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@BackgroundTaskID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddBackgroundTask",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@guid", guid),
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@effectiveUserId", effectiveUserId),
					/// <summary>TODO</summary>
					new SqlParameter("@taskName", taskName),
					/// <summary>TODO</summary>
					new SqlParameter("@itemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@startDate", startDate),
					/// <summary>TODO</summary>
					new SqlParameter("@indicatorCurrent", indicatorCurrent),
					/// <summary>TODO</summary>
					new SqlParameter("@indicatorMaximum", indicatorMaximum),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumExecutionTime", maximumExecutionTime),
					/// <summary>TODO</summary>
					new SqlParameter("@source", source),
					/// <summary>TODO</summary>
					new SqlParameter("@severity", severity),
					/// <summary>TODO</summary>
					new SqlParameter("@completed", completed),
					/// <summary>TODO</summary>
					new SqlParameter("@notifyOnComplete", notifyOnComplete),
					/// <summary>TODO</summary>
					new SqlParameter("@status", status));

				// read identity
				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddBackgroundTaskLog(int taskId, DateTime date, string exceptionStackTrace,
			bool innerTaskStart, int severity, string text, int textIdent, string xmlParameters)
		{
			if (UseEntityFramework)
			{
				var log = new Data.Entities.BackgroundTaskLog()
				{
					TaskId = taskId,
					Date = date,
					ExceptionStackTrace = exceptionStackTrace,
					InnerTaskStart = innerTaskStart ? 1 : 0,
					Severity = severity,
					Text = text,
					TextIdent = textIdent,
					XmlParameters = xmlParameters
				};
				BackgroundTaskLogs.Add(log);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddBackgroundTaskLog",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId),
					/// <summary>TODO</summary>
					new SqlParameter("@date", date),
					/// <summary>TODO</summary>
					new SqlParameter("@exceptionStackTrace", exceptionStackTrace),
					/// <summary>TODO</summary>
					new SqlParameter("@innerTaskStart", innerTaskStart),
					/// <summary>TODO</summary>
					new SqlParameter("@severity", severity),
					/// <summary>TODO</summary>
					new SqlParameter("@text", text),
					/// <summary>TODO</summary>
					new SqlParameter("@textIdent", textIdent),
					/// <summary>TODO</summary>
					new SqlParameter("@xmlParameters", xmlParameters));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetBackgroundTaskLogs(int taskId, DateTime startLogTime)
		{
			if (UseEntityFramework)
			{
				var logs = BackgroundTaskLogs
					.Where(l => l.TaskId == taskId && l.Date >= startLogTime)
					.OrderBy(l => l.Date)
					.Select(l => new
					{
						l.LogId,
						l.TaskId,
						l.Date,
						l.ExceptionStackTrace,
						InnerTaskStart = l.InnerTaskStart != null && l.InnerTaskStart != 0,
						l.Severity,
						l.Text,
						l.XmlParameters
					});
				/// <summary>TODO</summary>
				return EntityDataReader(logs);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetBackgroundTaskLogs",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId),
					/// <summary>TODO</summary>
					new SqlParameter("@startLogTime", startLogTime));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateBackgroundTask(Guid guid, int taskId, int scheduleId, int packageId, string taskName, int itemId,
			string itemName, DateTime finishDate, int indicatorCurrent, int indicatorMaximum, int maximumExecutionTime,
			string source, int severity, bool completed, bool notifyOnComplete, BackgroundTaskStatus status)
		{
			if (UseEntityFramework)
			{
				var task = BackgroundTasks
					.FirstOrDefault(t => t.Id == taskId);
				if (task != null)
				{
					task.Guid = guid;
					task.ScheduleId = scheduleId;
					task.PackageId = packageId;
					task.TaskName = taskName;
					task.ItemId = itemId;
					task.ItemName = itemName;
					task.FinishDate = finishDate == DateTime.MinValue ? null : finishDate;
					task.IndicatorCurrent = indicatorCurrent;
					task.IndicatorMaximum = indicatorMaximum;
					task.MaximumExecutionTime = maximumExecutionTime;
					task.Source = source;
					task.Severity = severity;
					task.Completed = completed;
					task.NotifyOnComplete = notifyOnComplete;
					task.Status = status;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateBackgroundTask",
					/// <summary>TODO</summary>
					new SqlParameter("@Guid", guid),
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@taskName", taskName),
					/// <summary>TODO</summary>
					new SqlParameter("@itemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemName", itemName),
					/// <summary>TODO</summary>
					new SqlParameter("@finishDate", finishDate == DateTime.MinValue ? DBNull.Value : (object)finishDate),
					/// <summary>TODO</summary>
					new SqlParameter("@indicatorCurrent", indicatorCurrent),
					/// <summary>TODO</summary>
					new SqlParameter("@indicatorMaximum", indicatorMaximum),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumExecutionTime", maximumExecutionTime),
					/// <summary>TODO</summary>
					new SqlParameter("@source", source),
					/// <summary>TODO</summary>
					new SqlParameter("@severity", severity),
					/// <summary>TODO</summary>
					new SqlParameter("@completed", completed),
					/// <summary>TODO</summary>
					new SqlParameter("@notifyOnComplete", notifyOnComplete),
					/// <summary>TODO</summary>
					new SqlParameter("@status", (int)status));

			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetBackgroundTaskParams(int taskId)
		{
			if (UseEntityFramework)
			{
				var parameters = BackgroundTaskParameters
					.Where(p => p.TaskId == taskId)
					.Select(p => new { p.ParameterId, p.TaskId, p.Name, p.SerializerValue, p.TypeName });
				/// <summary>TODO</summary>
				return EntityDataReader(parameters);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetBackgroundTaskParams",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddBackgroundTaskParam(int taskId, string name, string value, string typeName)
		{
			if (UseEntityFramework)
			{
				var parameter = new Data.Entities.BackgroundTaskParameter()
				{
					TaskId = taskId,
					Name = name,
					SerializerValue = value,
					TypeName = typeName
				};
				BackgroundTaskParameters.Add(parameter);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddBackgroundTaskParam",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId),
					/// <summary>TODO</summary>
					new SqlParameter("@name", name),
					/// <summary>TODO</summary>
					new SqlParameter("@value", value),
					/// <summary>TODO</summary>
					new SqlParameter("@typeName", typeName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteBackgroundTaskParams(int taskId)
		{
			if (UseEntityFramework)
			{
				BackgroundTaskParameters.Where(p => p.TaskId == taskId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteBackgroundTaskParams",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddBackgroundTaskStack(int taskId)
		{
			if (UseEntityFramework)
			{
				var stack = new Data.Entities.BackgroundTaskStack()
				{
					TaskId = taskId
				};
				BackgroundTaskStacks.Add(stack);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddBackgroundTaskStack",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteBackgroundTasks(Guid guid)
		{
			if (UseEntityFramework)
			{
				var tasks = BackgroundTasks
					.Where(t => t.Guid == guid);

				BackgroundTaskStacks
					.Join(tasks, ts => ts.TaskId, t => t.Id, (ts, t) => ts)
					.ExecuteDelete();

				BackgroundTaskLogs
					.Join(tasks, tl => tl.TaskId, t => t.Id, (tl, t) => tl)
					.ExecuteDelete();

				BackgroundTaskParameters
					.Join(tasks, tp => tp.TaskId, t => t.Id, (tp, t) => tp)
					.ExecuteDelete();

				tasks.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteBackgroundTasks",
					/// <summary>TODO</summary>
					new SqlParameter("@guid", guid));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteBackgroundTask(int taskId)
		{
			if (UseEntityFramework)
			{
				BackgroundTaskStacks.Where(ts => ts.TaskId == taskId).ExecuteDelete();

				BackgroundTaskLogs.Where(tl => tl.TaskId == taskId).ExecuteDelete();

				BackgroundTaskParameters.Where(tp => tp.TaskId == taskId).ExecuteDelete();

				BackgroundTasks.Where(t => t.Id == taskId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteBackgroundTask",
					/// <summary>TODO</summary>
					new SqlParameter("@id", taskId));
			}
		}

		// TODO bug, in GetScheuldeTask(int actorId) the WHERE clause is roleId <= t.RoleId and in
		// GetScheduleTask(int actorId, string taskId) the WHERE clause is roleId >= t.RoleId. Looks like a
		// bug, but I don't know if >= or <= is correct. I think roleId <= t.RoleID is correct, since a
		// lower roleId is more privileged.
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetScheduleTasks(int actorId)
		{
			if (UseEntityFramework)
			{
				var roleId = Users
					.Where(u => u.UserId == actorId)
					.Select(u => u.RoleId)
					.FirstOrDefault();
				var tasks = ScheduleTasks
					.Where(t => roleId <= t.RoleId)
					.Select(t => new { t.TaskId, t.TaskType, t.RoleId });
				/// <summary>TODO</summary>
				return EntityDataReader(tasks);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetScheduleTasks",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId));
			}
		}

		// TODO bug, in GetScheuldeTask(int actorId) the WHERE clause is roleId <= t.RoleId and in
		// GetScheduleTask(int actorId, string taskId) the WHERE clause is roleId >= t.RoleId. Looks like a
		// bug, but I don't know if >= or <= is correct. I think roleId <= t.RoleID is correct, since a
		// lower roleId is more privileged. So I corrected the WHERE clause to roleId <= r.RoleId.
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetScheduleTask(int actorId, string taskId)
		{
			if (UseEntityFramework)
			{
				var roleId = Users
					.Where(u => u.UserId == actorId)
					.Select(u => u.RoleId)
					.FirstOrDefault();
				var tasks = ScheduleTasks
					.Where(t => t.TaskId == taskId && roleId <= /* was >= */  t.RoleId)
					.Select(t => new { t.TaskId, t.TaskType, t.RoleId });
				/// <summary>TODO</summary>
				return EntityDataReader(tasks);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetScheduleTask",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetSchedules(int actorId, int packageId)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

                using (var tree = PackagesTree(actorId, true))
                using (var scheduleIds = Schedules
                    .Join(tree, s => s.PackageId, pt => pt, (s, pt) => new { s.ScheduleId, s.Enabled, s.NextRun })
                    .OrderByDescending(s => s.Enabled)
                    .ThenBy(s => s.NextRun)
                    .Select(s => s.ScheduleId)
                    .ToTempIdSet(this))
                {
                    var schedules = scheduleIds
						.Join(Schedules, sid => sid, s => s.ScheduleId, (sid, s) => s)
						.Select(s => new
						{
							s.ScheduleId,
							s.TaskId,
							s.Task.TaskType,
							s.Task.RoleId,
							s.PackageId,
							s.ScheduleName,
							s.ScheduleTypeId,
							s.Interval,
							s.FromTime,
							s.ToTime,
							s.StartTime,
							s.LastRun,
							s.NextRun,
							s.Enabled,
							StatusId = ScheduleStatus.Idle,
							s.PriorityId,
							s.MaxExecutionTime,
							s.WeekMonthDay,
							LastResult = AuditLogs
								.Where(l => l.ItemId == s.ScheduleId && l.SourceName == "SCHEDULER")
								.OrderByDescending(l => l.StartDate)
								.Select(l => l.SeverityId)
								.FirstOrDefault(),
							s.Package.User.Username,
							s.Package.User.FirstName,
							s.Package.User.LastName,
							FullName = s.Package.User.FirstName + " " + s.Package.User.LastName,
							UserRoleId = s.Package.User.RoleId,
							s.Package.User.Email
						});
					var parameters = scheduleIds
						.Join(Schedules, sid => sid, s => s.ScheduleId, (sid, s) => s)
						.Join(ScheduleTaskParameters, s => s.TaskId, sp => sp.TaskId, (s, sp) => new { Schedule = s, TaskParameter = sp })
						.GroupJoin(ScheduleParameters, s => new { s.TaskParameter.ParameterId, s.Schedule.ScheduleId }, sp => new { sp.ParameterId, sp.ScheduleId }, (s, sp) => new
						{
							s.Schedule.ScheduleId,
							s.TaskParameter.ParameterId,
							s.TaskParameter.DataTypeId,
							s.TaskParameter.DefaultValue,
							Parameters = sp
						})
						.SelectMany(s => s.Parameters.DefaultIfEmpty(), (s, sp) => new
						{
							s.ScheduleId,
							s.ParameterId,
							s.DataTypeId,
							ParameterValue = sp != null ? sp.ParameterValue : s.DefaultValue
						});
					/// <summary>TODO</summary>
					return EntityDataSet(schedules, parameters);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetSchedules",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@recursive", true));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetSchedulesPaged(int actorId, int packageId, bool recursive,
			string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var tree = PackagesTree(packageId, recursive))
				{
					var schedules = Schedules
						.Join(Packages, s => s.PackageId, p => p.PackageId, (s, p) => new { S = s, P = p })
						.Join(tree, s => s.S.PackageId, pt => pt, (s, pt) => s)
						.Join(UsersDetailed, s => s.P.UserId, u => u.UserId, (s, u) => new
						{
							s.S.ScheduleId,
							s.S.TaskId,
							s.S.Task.TaskType,
							s.S.Task.RoleId,
							s.S.ScheduleName,
							s.S.ScheduleTypeId,
							s.S.Interval,
							s.S.FromTime,
							s.S.ToTime,
							s.S.StartTime,
							s.S.LastRun,
							s.S.NextRun,
							s.S.Enabled,
							StatusId = ScheduleStatus.Idle,
							s.S.PriorityId,
							s.S.MaxExecutionTime,
							s.S.WeekMonthDay,
							LastResult = AuditLogs
								.Where(l => l.ItemId == s.S.ScheduleId && l.SourceName == "SCHEDULER")
								.OrderByDescending(l => l.StartDate)
								.Select(l => l.SeverityId)
								.FirstOrDefault(),
							s.P.PackageId,
							s.P.PackageName,
							s.P.UserId,
							u.Username,
							u.FirstName,
							u.LastName,
							u.FullName,
							UserRoleId = u.RoleId,
							u.Email
						});

					if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
					{
						schedules = schedules.Where(DynamicFunctions.ColumnLike(schedules, filterColumn, filterValue));
					}
					else if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
							schedules = schedules.Where(s => DbFunctions.Like(s.ScheduleName, filterValue) ||
								DbFunctions.Like(s.Username, filterValue) ||
								DbFunctions.Like(s.FullName, filterValue) ||
								DbFunctions.Like(s.Email, filterValue));
#else
							schedules = schedules.Where(s => EF.Functions.Like(s.ScheduleName, filterValue) ||
								EF.Functions.Like(s.Username, filterValue) ||
								EF.Functions.Like(s.FullName, filterValue) ||
								EF.Functions.Like(s.Email, filterValue));
#endif
					}

					var count = schedules.Count();

					schedules = !string.IsNullOrEmpty(sortColumn) ? schedules.OrderBy(ColumnName(sortColumn)) : schedules.OrderBy(s => s.ScheduleName);








					schedules = schedules.Skip(startRow).Take(maximumRows);

					/// <summary>TODO</summary>
					return EntityDataSet(count, schedules);
				}
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetSchedulesPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@recursive", recursive),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetSchedule(int actorId, int scheduleId)
		{
			if (UseEntityFramework)
			{
				var schedule = Schedules
					.Where(s => s.ScheduleId == scheduleId)
					.AsEnumerable()
					.Where(s => Clone.CheckActorPackageRights(actorId, s.PackageId))
					.Take(1)
					.Select(s => new
					{
						s.ScheduleId,
						s.TaskId,
						s.PackageId,
						s.ScheduleName,
						s.ScheduleTypeId,
						s.Interval,
						s.FromTime,
						s.ToTime,
						s.StartTime,
						s.LastRun,
						s.NextRun,
						s.Enabled,
						s.HistoriesNumber,
						s.PriorityId,
						s.MaxExecutionTime,
						s.WeekMonthDay,
                        StatusId = ScheduleStatus.Idle
                    });

				var task = schedule
					.Join(ScheduleTasks, s => s.TaskId, st => st.TaskId, (s, st) => new
					{
						st.TaskId,
						st.TaskType,
						st.RoleId
					});

				var parameter = schedule
					.Join(ScheduleTaskParameters, s => s.TaskId, stp => stp.TaskId, (s, stp) => new
					{
						Schedule = s,
						TaskParameter = stp
					})
					.GroupJoin(ScheduleParameters, s => new { s.TaskParameter.ParameterId, s.Schedule.ScheduleId },
						sp => new { sp.ParameterId, sp.ScheduleId }, (s, sp) => new
						{
							s.Schedule.ScheduleId,
							s.TaskParameter.ParameterId,
							s.TaskParameter.DataTypeId,
							s.TaskParameter.DefaultValue,
							Parameters = sp
						})
					.SelectMany(s => s.Parameters.DefaultIfEmpty(), (s, sp) => new
					{
						s.ScheduleId,
						s.ParameterId,
						s.DataTypeId,
						ParameterValue = sp != null ? sp.ParameterValue : s.DefaultValue
					});

				/// <summary>TODO</summary>
				return EntityDataSet(schedule, task, parameter);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetSchedule",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetScheduleInternal(int scheduleId)
		{
			if (UseEntityFramework)
			{
				var schedule = Schedules
					.Where(s => s.ScheduleId == scheduleId)
					.Select(s => new
					{
						s.ScheduleId,
						s.TaskId,
						s.Task.TaskType,
						s.Task.RoleId,
						s.PackageId,
						s.ScheduleName,
						s.ScheduleTypeId,
						s.Interval,
						s.FromTime,
						s.ToTime,
						s.StartTime,
						s.LastRun,
						s.NextRun,
						s.Enabled,
                        StatusId = ScheduleStatus.Idle,
						s.PriorityId,
						s.HistoriesNumber,
						s.MaxExecutionTime,
						s.WeekMonthDay
					});

				/// <summary>TODO</summary>
				return EntityDataReader(schedule);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetScheduleInternal",
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public DataSet GetNextSchedule()
		{
			if (UseEntityFramework)
			{
				var next = Schedules
					.Where(s => s.Enabled)
					.OrderBy(s => s.NextRun)
					.Select(s => new
					{
						s.ScheduleId,
						s.TaskId
					})
					.FirstOrDefault();
				var nextScheduleId = next?.ScheduleId;

				var schedule = Schedules
					.Where(s => s.ScheduleId == nextScheduleId)
					.Take(1)
					.Select(s => new
					{
						s.ScheduleId,
						s.TaskId,
						s.PackageId,
						s.ScheduleName,
						s.ScheduleTypeId,
						s.Interval,
						s.FromTime,
						s.ToTime,
						s.StartTime,
						s.LastRun,
						s.NextRun,
						s.Enabled,
						s.HistoriesNumber,
						s.PriorityId,
						s.MaxExecutionTime,
						s.WeekMonthDay,
                        StatusId = ScheduleStatus.Idle
                    });

				var task = ScheduleTasks
					.Where(st => st.TaskId == next.TaskId)
					.Select(st => new
					{
						st.TaskId,
						st.TaskType,
						st.RoleId
					});

                var parameter = Schedules
                    .Where(s => s.ScheduleId == nextScheduleId)
                    .Join(ScheduleTaskParameters, s => s.TaskId, stp => stp.TaskId, (s, stp) => new
                    {
                        Schedule = s,
                        TaskParameter = stp
                    })
                    .GroupJoin(ScheduleParameters, s => new { s.TaskParameter.ParameterId, s.Schedule.ScheduleId },
                        sp => new { sp.ParameterId, sp.ScheduleId }, (stp, sp) => new
                        {
                            stp.Schedule.ScheduleId,
                            stp.TaskParameter.ParameterId,
                            stp.TaskParameter.DataTypeId,
                            stp.TaskParameter.DefaultValue,
                            Parameters = sp
                        })
                    .SelectMany(p => p.Parameters.DefaultIfEmpty(), (p, sp) => new
                    {
                        p.ScheduleId,
                        p.ParameterId,
                        p.DataTypeId,
                        ParameterValue = sp != null ? sp.ParameterValue : p.DefaultValue
                    });

                /// <summary>TODO</summary>
                return EntityDataSet(schedule, task, parameter);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetNextSchedule");
			}
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetScheduleParameters(int actorId, string taskId, int scheduleId)
		{
			if (UseEntityFramework)
			{
				// check rights
				var packageId = Schedules
					.Where(s => s.ScheduleId == scheduleId)
					.Select(s => s.PackageId)
					.FirstOrDefault();
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var parameters = ScheduleTaskParameters
					.Where(stp => stp.TaskId == taskId)
					.GroupJoin(ScheduleParameters, stp => new { stp.ParameterId, ScheduleId = scheduleId },
						sp => new { sp.ParameterId, sp.ScheduleId }, (stp, sp) => new
						{
							ScheduleId = scheduleId,
							stp.ParameterId,
							stp.DataTypeId,
							stp.DefaultValue,
							stp.ParameterOrder,
							Parameters = sp
						})
					.SelectMany(s => s.Parameters.DefaultIfEmpty(), (s, sp) => new
					{
						s.ScheduleId,
						s.ParameterId,
						s.DataTypeId,
						s.DefaultValue,
						s.ParameterOrder,
						ParameterValue = sp != null ? sp.ParameterValue : null,
					})
					.OrderBy(s => s.ParameterOrder)
					.Select(s => new
					{
						s.ScheduleId,
						s.ParameterId,
						s.DataTypeId,
						s.DefaultValue,
						s.ParameterValue
					});

				/// <summary>TODO</summary>
				return EntityDataReader(parameters);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetScheduleParameters",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId));
			}
		}

		/// <summary>
		/// Loads view configuration for the task with specified id.
		/// </summary>
		/// <param name="taskId">Task id which points to task for which view configuration will be loaded.</param>
		/// <returns>View configuration for the task with supplied id.</returns>
		/// <summary>TODO</summary>
		public IDataReader GetScheduleTaskViewConfigurations(string taskId)
		{
			if (UseEntityFramework)
			{
				var conf = ScheduleTaskViewConfigurations
					.Where(c => c.TaskId == taskId)
					.Select(c => new
					{
						TaskId = taskId,
						c.ConfigurationId,
						c.Environment,
						c.Description
					});

				/// <summary>TODO</summary>
				return EntityDataReader(conf);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetScheduleTaskViewConfigurations",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddSchedule(int actorId, string taskId, int packageId,
			string scheduleName, string scheduleTypeId, int interval,
			DateTime fromTime, DateTime toTime, DateTime startTime,
			DateTime nextRun, bool enabled, string priorityId, int historiesNumber,
			int maxExecutionTime, int weekMonthDay, string xmlParameters)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					var schedule = new Data.Entities.Schedule()
					{
						TaskId = taskId,
						PackageId = packageId,
						ScheduleName = scheduleName,
						ScheduleTypeId = scheduleTypeId,
						Interval = interval,
						FromTime = fromTime,
						ToTime = toTime,
						StartTime = startTime,
						NextRun = nextRun,
						Enabled = enabled,
						PriorityId = priorityId,
						HistoriesNumber = historiesNumber,
						MaxExecutionTime = maxExecutionTime,
						WeekMonthDay = weekMonthDay
					};
					Schedules.Add(schedule);
					SaveChanges();

					ScheduleParameters
						.Where(p => p.ScheduleId == schedule.ScheduleId)
						.ExecuteDelete();

					var parameters = XElement.Parse(xmlParameters)
						.Elements()
						.Select(e => new Data.Entities.ScheduleParameter
						{
							ScheduleId = schedule.ScheduleId,
							ParameterId = (string)e.Attribute("id"),
							ParameterValue = (string)e.Attribute("value")
						});
					ScheduleParameters.AddRange(parameters);
					SaveChanges();

					transaction.Commit();

					return schedule.ScheduleId;
				}
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@ScheduleID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddSchedule",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId),
					/// <summary>TODO</summary>
					new SqlParameter("@packageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleName", scheduleName),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleTypeId", scheduleTypeId),
					/// <summary>TODO</summary>
					new SqlParameter("@interval", interval),
					/// <summary>TODO</summary>
					new SqlParameter("@fromTime", fromTime),
					/// <summary>TODO</summary>
					new SqlParameter("@toTime", toTime),
					/// <summary>TODO</summary>
					new SqlParameter("@startTime", startTime),
					/// <summary>TODO</summary>
					new SqlParameter("@nextRun", nextRun),
					/// <summary>TODO</summary>
					new SqlParameter("@enabled", enabled),
					/// <summary>TODO</summary>
					new SqlParameter("@priorityId", priorityId),
					/// <summary>TODO</summary>
					new SqlParameter("@historiesNumber", historiesNumber),
					/// <summary>TODO</summary>
					new SqlParameter("@maxExecutionTime", maxExecutionTime),
					/// <summary>TODO</summary>
					new SqlParameter("@weekMonthDay", weekMonthDay),
					/// <summary>TODO</summary>
					new SqlParameter("@xmlParameters", xmlParameters));

				// read identity
				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateSchedule(int actorId, int scheduleId, string taskId,
			string scheduleName, string scheduleTypeId, int interval,
			DateTime fromTime, DateTime toTime, DateTime startTime,
			DateTime lastRun, DateTime nextRun, bool enabled, string priorityId,
			int historiesNumber, int maxExecutionTime, int weekMonthDay, string xmlParameters)
		{
			if (UseEntityFramework)
			{
				// check rights
				var packageId = Schedules
					.Where(s => s.ScheduleId == scheduleId)
					.Select(s => s.PackageId)
					.FirstOrDefault();
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					var schedule = Schedules.FirstOrDefault(s => s.ScheduleId == scheduleId);
					if (schedule != null)
					{
						schedule.TaskId = taskId;
						schedule.ScheduleName = scheduleName;
						schedule.ScheduleTypeId = scheduleTypeId;
						schedule.Interval = interval;
						schedule.FromTime = fromTime;
						schedule.ToTime = toTime;
						schedule.StartTime = startTime;
						schedule.LastRun = lastRun == DateTime.MinValue ? null : lastRun;
						schedule.NextRun = nextRun;
						schedule.Enabled = enabled;
						schedule.PriorityId = priorityId;
						schedule.HistoriesNumber = historiesNumber;
						schedule.MaxExecutionTime = maxExecutionTime;
						schedule.WeekMonthDay = weekMonthDay;
						SaveChanges();

						ScheduleParameters.Where(p => p.ScheduleId == schedule.ScheduleId).ExecuteDelete();

						var parameters = XElement.Parse(xmlParameters)
							.Elements()
							.Select(e => new Data.Entities.ScheduleParameter
							{
								ScheduleId = schedule.ScheduleId,
								ParameterId = (string)e.Attribute("id"),
								ParameterValue = (string)e.Attribute("value")
							});
						ScheduleParameters.AddRange(parameters);
						SaveChanges();

						transaction.Commit();
					}
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateSchedule",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId),
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleName", scheduleName),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleTypeId", scheduleTypeId),
					/// <summary>TODO</summary>
					new SqlParameter("@interval", interval),
					/// <summary>TODO</summary>
					new SqlParameter("@fromTime", fromTime),
					/// <summary>TODO</summary>
					new SqlParameter("@toTime", toTime),
					/// <summary>TODO</summary>
					new SqlParameter("@startTime", startTime),
					/// <summary>TODO</summary>
					new SqlParameter("@lastRun", (lastRun == DateTime.MinValue) ? DBNull.Value : (object)lastRun),
					/// <summary>TODO</summary>
					new SqlParameter("@nextRun", nextRun),
					/// <summary>TODO</summary>
					new SqlParameter("@enabled", enabled),
					/// <summary>TODO</summary>
					new SqlParameter("@priorityId", priorityId),
					/// <summary>TODO</summary>
					new SqlParameter("@historiesNumber", historiesNumber),
					/// <summary>TODO</summary>
					new SqlParameter("@maxExecutionTime", maxExecutionTime),
					/// <summary>TODO</summary>
					new SqlParameter("@weekMonthDay", weekMonthDay),
					/// <summary>TODO</summary>
					new SqlParameter("@xmlParameters", xmlParameters));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteSchedule(int actorId, int scheduleId)
		{
			if (UseEntityFramework)
			{
				// check rights
				var packageId = Schedules
					.Where(s => s.ScheduleId == scheduleId)
					.Select(s => s.PackageId)
					.FirstOrDefault();
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				using (var transaction = Database.BeginTransaction())
				{
					ScheduleParameters.Where(sp => sp.ScheduleId == scheduleId).ExecuteDelete();
					Schedules.Where(s => s.ScheduleId == scheduleId).ExecuteDelete();

					transaction.Commit();
				}

			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteSchedule",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId));
			}
		}

		/*
		// TODO This rotuine is not present in the Stored Procedures
		/// <summary>Auto-generated member.</summary>
		public DataSet GetScheduleHistories(int actorId, int scheduleId)
		{
			/// <summary>TODO</summary>
			throw new NotImplementedException();

			if (UseEntityFramework)
			{
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetScheduleHistories",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId));
			}
		}

		// TODO This rotuine is not present in the Stored Procedures
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetScheduleHistory(int actorId, int scheduleHistoryId)
		{
			/// <summary>TODO</summary>
			throw new NotImplementedException();

			if (UseEntityFramework)
			{
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetScheduleHistory",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleHistoryId", scheduleHistoryId));
			}
		}

		// TODO This rotuine is not present in the Stored Procedures
		/// <summary>Auto-generated member.</summary>
		public int AddScheduleHistory(int actorId, int scheduleId,
			DateTime startTime, DateTime finishTime, string statusId, string executionLog)
		{
			/// <summary>TODO</summary>
			throw new NotImplementedException();

			if (UseEntityFramework)
			{
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@ScheduleHistoryID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddScheduleHistory",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId),
					/// <summary>TODO</summary>
					new SqlParameter("@startTime", (startTime == DateTime.MinValue) ? DBNull.Value : (object)startTime),
					/// <summary>TODO</summary>
					new SqlParameter("@finishTime", (finishTime == DateTime.MinValue) ? DBNull.Value : (object)finishTime),
					/// <summary>TODO</summary>
					new SqlParameter("@statusId", statusId),
					/// <summary>TODO</summary>
					new SqlParameter("@executionLog", executionLog));

				// read identity
				return Convert.ToInt32(prmId.Value);
			}
		}

		// TODO This rotuine is not present in the Stored Procedures
		/// <summary>Auto-generated member.</summary>
		public void UpdateScheduleHistory(int actorId, int scheduleHistoryId,
			DateTime startTime, DateTime finishTime, string statusId, string executionLog)
		{
			/// <summary>TODO</summary>
			throw new NotImplementedException();

			if (UseEntityFramework)
			{
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateScheduleHistory",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleHistoryId", scheduleHistoryId),
					/// <summary>TODO</summary>
					new SqlParameter("@startTime", (startTime == DateTime.MinValue) ? DBNull.Value : (object)startTime),
					/// <summary>TODO</summary>
					new SqlParameter("@finishTime", (finishTime == DateTime.MinValue) ? DBNull.Value : (object)finishTime),
					/// <summary>TODO</summary>
					new SqlParameter("@statusId", statusId),
					/// <summary>TODO</summary>
					new SqlParameter("@executionLog", executionLog));
			}
		}

		// TODO This rotuine is not present in the Stored Procedures
		/// <summary>Auto-generated member.</summary>
		public void DeleteScheduleHistories(int actorId, int scheduleId)
		{
			/// <summary>TODO</summary>
			throw new NotImplementedException();

			if (UseEntityFramework)
			{
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteScheduleHistories",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@scheduleId", scheduleId));
			}
		}*/
		#endregion

		#region Comments
		/// <summary>Auto-generated member.</summary>
		public DataSet GetComments(int actorId, int userId, string itemTypeId, int itemId)
		{
			if (UseEntityFramework)
			{
				//check rights
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this account");

                var comments = Comments
                    .Where(c => c.ItemTypeId == itemTypeId && c.ItemId == itemId)
                    .OrderBy(c => c.CreatedDate)
                    .AsEnumerable()
                    .Where(c => Clone.CheckUserParent(userId, c.UserId))
                    .Select(c => new
                    {
                        c.CommentId,
                        c.ItemTypeId,
                        c.ItemId,
                        c.UserId,
                        c.CreatedDate,
                        c.CommentText,
                        c.SeverityId,
                        //user
                        c.User.Username,
                        c.User.FirstName,
                        c.User.LastName,
                        FullName = c.User.FirstName + " " + c.User.LastName,
                        c.User.RoleId,
                        c.User.Email
                    });

                /// <summary>TODO</summary>
                return EntityDataSet(comments);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetComments",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@userId", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemTypeId", itemTypeId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemId", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddComment(int actorId, string itemTypeId, int itemId,
			 string commentText, int severityId)
		{
			if (UseEntityFramework)
			{
				var comment = new Data.Entities.Comment()
				{
					ItemTypeId = itemTypeId,
					ItemId = itemId,
					UserId = actorId,
					CreatedDate = DateTime.Now,
					CommentText = commentText,
					SeverityId = severityId
				};
				Comments.Add(comment);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddComment",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemTypeId", itemTypeId),
					/// <summary>TODO</summary>
					new SqlParameter("@itemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@commentText", commentText),
					/// <summary>TODO</summary>
					new SqlParameter("@severityId", severityId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteComment(int actorId, int commentId)
		{
			if (UseEntityFramework)
			{
				// check rights
				var userId = Comments
					.Where(c => c.CommentId == commentId)
					.Select(c => (int?)c.UserId)
					.FirstOrDefault();
				if (!CheckActorUserRights(actorId, userId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to perform this operation");

				Comments.Where(c => c.CommentId == commentId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteComment",
					/// <summary>TODO</summary>
					new SqlParameter("@actorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@commentId", commentId));
			}
		}
		#endregion

		#region Helper Methods
		/// <summary>Auto-generated member.</summary>
		private string VerifyColumnName(string str)
		{
			if (str == null)
				str = "";
			return Regex.Replace(str, @"[^\w\. ]", "");
		}
		/// <summary>Auto-generated member.</summary>

		private string VerifyColumnValue(string str)
		{
			return String.IsNullOrEmpty(str) ? str : str.Replace("'", "''");
		}
		/// <summary>Auto-generated member.</summary>

		private DataSet ExecuteLongDataSet(string spName, params SqlParameter[] parameters)
		{
			/// <summary>TODO</summary>
			return ExecuteLongDataSet(spName, CommandType.StoredProcedure, parameters);
		}
		/// <summary>Auto-generated member.</summary>

		private DataSet ExecuteLongQueryDataSet(string spName, params SqlParameter[] parameters)
		{
			/// <summary>TODO</summary>
			return ExecuteLongDataSet(spName, CommandType.Text, parameters);
		}
		/// <summary>Auto-generated member.</summary>

		private DataSet ExecuteLongDataSet(string commandText, CommandType commandType, params SqlParameter[] parameters)
		{
			using SqlConnection conn = new SqlConnection(NativeConnectionString);
			using SqlCommand cmd = new SqlCommand(commandText, conn);
			cmd.CommandType = commandType;
			cmd.CommandTimeout = 300;

			if (parameters != null)
			{
				foreach (SqlParameter prm in parameters)
				{
					cmd.Parameters.Add(prm);
				}
			}

			DataSet ds = new DataSet();
			try
			{
				using SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
			}
			finally
			{
				if (conn.State == ConnectionState.Open)
					conn.Close();
			}

			/// <summary>TODO</summary>
			return ds;
		}
		/// <summary>Auto-generated member.</summary>

		private void ExecuteLongNonQuery(string spName, params SqlParameter[] parameters)
		{
			using SqlConnection conn = new SqlConnection(NativeConnectionString);
			using SqlCommand cmd = new SqlCommand(spName, conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandTimeout = 300;

			if (parameters != null)
			{
				foreach (SqlParameter prm in parameters)
				{
					cmd.Parameters.Add(prm);
				}
			}

			try
			{
				conn.Open();
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if (conn.State == ConnectionState.Open)
					conn.Close();
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataTable EntityDataTable<TEntity>(IEnumerable<TEntity> set) where TEntity : class => new EntityDataTable<TEntity>(set);
		/// <summary>Auto-generated member.</summary>
		public DataSet EntityDataSet(params DataTable[] tables)
		{
			var dataSet = new DataSet();
			foreach (var table in tables) dataSet.Tables.Add(table);
			/// <summary>TODO</summary>
			return dataSet;
		}
		/// <summary>Auto-generated member.</summary>
		public DataSet EntityDataSet<TEntity>(IEnumerable<TEntity> set) where TEntity : class
			=> EntityDataSet(EntityDataTable(set));
		/// <summary>Auto-generated member.</summary>
		public DataSet EntityDataSet<TEntity>(int count, IEnumerable<TEntity> set) where TEntity : class
			=> EntityDataSet(CountDataTable(count), EntityDataTable(set));
		/// <summary>Auto-generated member.</summary>
		public DataSet EntityDataSet<TEntity1, TEntity2>(int count, IEnumerable<TEntity1> set1, IEnumerable<TEntity2> set2)
			/// <summary>TODO</summary>
			where TEntity1 : class where TEntity2 : class
			=> EntityDataSet(CountDataTable(count), EntityDataTable(set1), EntityDataTable(set2));
		/// <summary>Auto-generated member.</summary>
		public DataSet EntityDataSet<TEntity1, TEntity2>(IEnumerable<TEntity1> set1, IEnumerable<TEntity2> set2)
			/// <summary>TODO</summary>
			where TEntity1 : class where TEntity2 : class
			=> EntityDataSet(EntityDataTable(set1), EntityDataTable(set2));
		/// <summary>Auto-generated member.</summary>
		public DataSet EntityDataSet<TEntity1, TEntity2, TEntity3>(IEnumerable<TEntity1> set1, IEnumerable<TEntity2> set2, IEnumerable<TEntity3> set3)
			/// <summary>TODO</summary>
			where TEntity1 : class where TEntity2 : class where TEntity3 : class
			=> EntityDataSet(EntityDataTable(set1), EntityDataTable(set2), EntityDataTable(set3));
		/// <summary>Auto-generated member.</summary>
		public DataTable CountDataTable(int count)
		{
			var table = new DataTable();
			table.Columns.Add(new DataColumn("Column1", typeof(int)));
			table.Rows.Add(count);
			/// <summary>TODO</summary>
			return table;
		}
		/// <summary>Auto-generated member.</summary>
		public CountDataReader<TEntity> EntityDataReader<TEntity>(int count, IEnumerable<TEntity> set) where TEntity : class => new CountDataReader<TEntity>(set, count);
		/// <summary>Auto-generated member.</summary>
		public EntityDataReader<TEntity> EntityDataReader<TEntity>(IEnumerable<TEntity> set) where TEntity : class => new EntityDataReader<TEntity>(set);
		#endregion

		#region Exchange Server
		/// <summary>Auto-generated member.</summary>

		public int AddExchangeAccount(int itemId, ExchangeAccountType accountType, string accountName,
			string displayName, string primaryEmailAddress, bool mailEnabledPublicFolder,
			string mailboxManagerActions, string samAccountName, int mailboxPlanId, string subscriberNumber)
		{
			if (UseEntityFramework)
			{
				var account = new Data.Entities.ExchangeAccount()
				{
					ItemId = itemId,
					AccountType = accountType,
					AccountName = accountName,
					DisplayName = displayName,
					PrimaryEmailAddress = primaryEmailAddress,
					MailEnabledPublicFolder = mailEnabledPublicFolder,
					MailboxManagerActions = mailboxManagerActions,
					SamAccountName = samAccountName,
					MailboxPlanId = mailboxPlanId == 0 ? null : mailboxPlanId,
					SubscriberNumber = string.IsNullOrEmpty(subscriberNumber) ? null : subscriberNumber,
					UserPrincipalName = primaryEmailAddress,
					CreatedDate = DateTime.Now
				};
				ExchangeAccounts.Add(account);
				SaveChanges();
				return account.AccountId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@AccountID", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddExchangeAccount",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountType", accountType),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountName", accountName),
					/// <summary>TODO</summary>
					new SqlParameter("@DisplayName", displayName),
					/// <summary>TODO</summary>
					new SqlParameter("@PrimaryEmailAddress", primaryEmailAddress),
					/// <summary>TODO</summary>
					new SqlParameter("@MailEnabledPublicFolder", mailEnabledPublicFolder),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxManagerActions", mailboxManagerActions),
					/// <summary>TODO</summary>
					new SqlParameter("@SamAccountName", samAccountName),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", (mailboxPlanId == 0) ? (object)DBNull.Value : (object)mailboxPlanId),
					/// <summary>TODO</summary>
					new SqlParameter("@SubscriberNumber", (string.IsNullOrEmpty(subscriberNumber) ? (object)DBNull.Value : (object)subscriberNumber)));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddExchangeAccountEmailAddress(int accountId, string emailAddress)
		{
			if (UseEntityFramework)
			{
				var address = new Data.Entities.ExchangeAccountEmailAddress()
				{
					AccountId = accountId,
					EmailAddress = emailAddress
				};
				ExchangeAccountEmailAddresses.Add(address);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddExchangeAccountEmailAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@EmailAddress", emailAddress));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddExchangeOrganization(int itemId, string organizationId)
		{
			if (UseEntityFramework)
			{
				if (!ExchangeOrganizations.Any(eo => eo.OrganizationId == organizationId))
				{
					var organization = new Data.Entities.ExchangeOrganization()
					{
						ItemId = itemId,
						OrganizationId = organizationId
					};
					ExchangeOrganizations.Add(organization);
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddExchangeOrganization",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@OrganizationID", organizationId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddExchangeOrganizationDomain(int itemId, int domainId, bool isHost)
		{
			if (UseEntityFramework)
			{
				var domain = new Data.Entities.ExchangeOrganizationDomain()
				{
					ItemId = itemId,
					DomainId = domainId,
					IsHost = isHost
				};
				ExchangeOrganizationDomains.Add(domain);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddExchangeOrganizationDomain",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainID", domainId),
					/// <summary>TODO</summary>
					new SqlParameter("@IsHost", isHost));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void ChangeExchangeAcceptedDomainType(int itemId, int domainId, int domainTypeId)
		{
			if (UseEntityFramework)
			{
                ExchangeOrganizationDomains
                    .Where(d => d.ItemId == itemId && d.DomainId == domainId)
                    .ExecuteUpdate(d => new Data.Entities.ExchangeOrganizationDomain() { DomainTypeId = domainTypeId });
            }
            else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"ChangeExchangeAcceptedDomainType",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainID", domainId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainTypeID", domainTypeId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeOrganizationStatistics(int itemId)
		{
			if (UseEntityFramework)
			{
				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId);
				var archiveSizeUnlimited = accounts
					.Any(a => a.MailboxPlan.ArchiveSizeMb == -1);
				var mailboxSizeUnlimited = accounts
					.Any(a => a.MailboxPlan.MailboxSizeMb == -1);
				var accountTypes = new ExchangeAccountType[] { ExchangeAccountType.Mailbox, ExchangeAccountType.Room,
					ExchangeAccountType.Equipment, ExchangeAccountType.SharedMailbox, ExchangeAccountType.JournalingMailbox };
				var specialAccounts = accounts
					.Where(a => accountTypes.Any(t => t == a.AccountType));
				int archiveSize = archiveSizeUnlimited ? -1 : specialAccounts
					.Where(a => a.MailboxPlan.EnableArchiving == true)
					.Sum(a => a.MailboxPlan.ArchiveSizeMb) ?? 0;
				var tmp = new
				{
					CreatedMailboxes = accounts.Count(a => a.AccountType == ExchangeAccountType.Mailbox),
					CreatedSharedMailboxes = accounts.Count(a => a.AccountType == ExchangeAccountType.SharedMailbox),
					CreatedResourceMailboxes = accounts.Count(a =>
						a.AccountType == ExchangeAccountType.Room ||
						a.AccountType == ExchangeAccountType.Equipment),
					CreatedContacts = accounts.Count(a => a.AccountType == ExchangeAccountType.Contact),
					CreatedDistributionLists = accounts.Count(a => a.AccountType == ExchangeAccountType.DistributionList),
					CreatedPublicFolders = accounts.Count(a => a.AccountType == ExchangeAccountType.PublicFolder),
					CreatedJournalingMailboxes = accounts.Count(a => a.AccountType == ExchangeAccountType.JournalingMailbox),
					CreatedDomains = ExchangeOrganizationDomains.Count(d => d.ItemId == itemId),
					UsedDiskSpaces = specialAccounts
							.Select(a => (int?)a.MailboxPlan.MailboxSizeMb),
					UsedLitigationHoldSpaces = specialAccounts
							.Where(a => a.MailboxPlan.AllowLitigationHold == true)
							.Select(a => a.MailboxPlan.RecoverableItemsSpace),
					//UsedArchingStorage = archiveSize
				};
				if (mailboxSizeUnlimited)
				{
					var sizes = new
					{
						tmp.CreatedMailboxes,
						tmp.CreatedSharedMailboxes,
						tmp.CreatedResourceMailboxes,
						tmp.CreatedContacts,
						tmp.CreatedDistributionLists,
						tmp.CreatedPublicFolders,
						tmp.CreatedJournalingMailboxes,
						tmp.CreatedDomains,
						UsedDiskSpace = tmp.UsedDiskSpaces.Min() ?? 0,
						UsedLitigationHoldSpace = tmp.UsedLitigationHoldSpaces.Min() ?? 0,
						UsedArchingStorage = archiveSize
					};
					/// <summary>TODO</summary>
					return EntityDataReader(new[] { sizes });
				}
				else
				{
					var sizes = new
					{
						tmp.CreatedMailboxes,
						tmp.CreatedSharedMailboxes,
						tmp.CreatedResourceMailboxes,
						tmp.CreatedContacts,
						tmp.CreatedDistributionLists,
						tmp.CreatedPublicFolders,
						tmp.CreatedJournalingMailboxes,
						tmp.CreatedDomains,
						UsedDiskSpace = tmp.UsedDiskSpaces.Sum() ?? 0,
						UsedLitigationHoldSpace = tmp.UsedLitigationHoldSpaces.Sum() ?? 0,
						UsedArchingStorage = archiveSize
					};
					/// <summary>TODO</summary>
					return EntityDataReader(new[] { sizes });
				}
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeOrganizationStatistics",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteUserEmailAddresses(int accountId, string primaryAddress)
		{
			if (UseEntityFramework)
			{
				ExchangeAccountEmailAddresses
					.Where(a => a.AccountId == accountId && a.EmailAddress.ToLower() != primaryAddress.ToLower())
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteUserEmailAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@PrimaryEmailAddress", primaryAddress));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExchangeAccount(int itemId, int accountId)
		{
			if (UseEntityFramework)
			{
				ExchangeAccountEmailAddresses.Where(a => a.AccountId == accountId).ExecuteDelete();

				ExchangeAccounts.Where(a => a.ItemId == itemId && a.AccountId == accountId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExchangeAccount",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExchangeAccountEmailAddress(int accountId, string emailAddress)
		{
			if (UseEntityFramework)
			{
				ExchangeAccountEmailAddresses.Where(a => a.AccountId == accountId && a.EmailAddress == emailAddress)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExchangeAccountEmailAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@EmailAddress", emailAddress));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExchangeOrganization(int itemId)
		{
			if (UseEntityFramework)
			{
				using (var transaction = Database.BeginTransaction())
				{
					ExchangeMailboxPlans.Where(p => p.ItemId == itemId).ExecuteDelete();
					ExchangeOrganizations.Where(o => o.ItemId == itemId).ExecuteDelete();
					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExchangeOrganization",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExchangeOrganizationDomain(int itemId, int domainId)
		{
			if (UseEntityFramework)
			{
				ExchangeOrganizationDomains.Where(d => d.DomainId == domainId && d.ItemId == itemId)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExchangeOrganizationDomain",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainID", domainId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool ExchangeAccountEmailAddressExists(string emailAddress, bool checkContacts)
		{
			if (UseEntityFramework)
			{
				return ExchangeAccountEmailAddresses.Any(a => a.EmailAddress == emailAddress) ||
					ExchangeAccounts.Any(a => a.PrimaryEmailAddress == emailAddress && (checkContacts || a.AccountType != ExchangeAccountType.Contact));
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@Exists", SqlDbType.Bit);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"ExchangeAccountEmailAddressExists",
					/// <summary>TODO</summary>
					new SqlParameter("@EmailAddress", emailAddress),
					/// <summary>TODO</summary>
					new SqlParameter("@checkContacts", checkContacts),
					outParam);

				return Convert.ToBoolean(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool ExchangeOrganizationDomainExists(int domainId)
		{
			if (UseEntityFramework)
			{
				return ExchangeOrganizationDomains.Any(d => d.DomainId == domainId);
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@Exists", SqlDbType.Bit);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"ExchangeOrganizationDomainExists",
					/// <summary>TODO</summary>
					new SqlParameter("@DomainID", domainId),
					outParam);

				return Convert.ToBoolean(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>
		public bool ExchangeOrganizationExists(string organizationId)
		{
			if (UseEntityFramework)
			{
				return ExchangeOrganizations.Any(o => o.OrganizationId == organizationId);
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@Exists", SqlDbType.Bit);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"ExchangeOrganizationExists",
					/// <summary>TODO</summary>
					new SqlParameter("@OrganizationID", organizationId),
					outParam);

				return Convert.ToBoolean(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>
		public bool ExchangeAccountExists(string accountName)
		{
			if (UseEntityFramework)
			{
				var path = $"\\{accountName}";

				return ExchangeAccounts.Any(a => a.SamAccountName.EndsWith(path));
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@Exists", SqlDbType.Bit);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"ExchangeAccountExists",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountName", accountName),
					outParam);

				return Convert.ToBoolean(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void UpdateExchangeAccount(int accountId, string accountName, ExchangeAccountType accountType,
			string displayName, string primaryEmailAddress, bool mailEnabledPublicFolder,
			string mailboxManagerActions, string samAccountName, int mailboxPlanId, int archivePlanId, string subscriberNumber,
			bool EnableArchiving)
		{
			if (UseEntityFramework)
			{
				var account = ExchangeAccounts.FirstOrDefault(a => a.AccountId == accountId);
				if (account != null)
				{
					account.AccountName = accountName;
					account.DisplayName = displayName;
					account.PrimaryEmailAddress = primaryEmailAddress;
					account.MailEnabledPublicFolder = mailEnabledPublicFolder;
					account.MailboxManagerActions = mailboxManagerActions;
					account.AccountType = accountType;
					account.SamAccountName = samAccountName;
					account.MailboxPlanId = (mailboxPlanId == -1 || mailboxPlanId == 0) ? null : mailboxPlanId;
					account.SubscriberNumber = string.IsNullOrEmpty(subscriberNumber) ? null : subscriberNumber;
					account.ArchivingMailboxPlanId = archivePlanId < 1 ? null : archivePlanId;
					account.EnableArchiving = EnableArchiving;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateExchangeAccount",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountName", accountName),
					/// <summary>TODO</summary>
					new SqlParameter("@DisplayName", displayName),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountType", (int)accountType),
					/// <summary>TODO</summary>
					new SqlParameter("@PrimaryEmailAddress", primaryEmailAddress),
					/// <summary>TODO</summary>
					new SqlParameter("@MailEnabledPublicFolder", mailEnabledPublicFolder),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxManagerActions", mailboxManagerActions),
					/// <summary>TODO</summary>
					new SqlParameter("@SamAccountName", samAccountName),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", (mailboxPlanId == 0) ? (object)DBNull.Value : (object)mailboxPlanId),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchivingMailboxPlanId", (archivePlanId < 1) ? (object)DBNull.Value : (object)archivePlanId),
					/// <summary>TODO</summary>
					new SqlParameter("@SubscriberNumber", (string.IsNullOrEmpty(subscriberNumber) ? (object)DBNull.Value : (object)subscriberNumber)),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableArchiving", EnableArchiving));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void UpdateExchangeAccountServiceLevelSettings(int accountId, int levelId, bool isVIP)
		{
			if (UseEntityFramework)
			{
				var account = ExchangeAccounts.FirstOrDefault(a => a.AccountId == accountId);
				if (account != null)
				{
					account.LevelId = (levelId != -1 && levelId != 0) ? levelId : null;
					account.IsVip = isVIP;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateExchangeAccountSLSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@LevelID", (levelId == 0) ? (object)DBNull.Value : (object)levelId),
					/// <summary>TODO</summary>
					new SqlParameter("@IsVIP", isVIP));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateExchangeAccountUserPrincipalName(int accountId, string userPrincipalName)
		{
			if (UseEntityFramework)
			{
				var account = ExchangeAccounts.FirstOrDefault(a => a.AccountId == accountId);
				if (account != null)
				{
					account.UserPrincipalName = userPrincipalName;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateExchangeAccountUserPrincipalName",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserPrincipalName", userPrincipalName));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetExchangeAccount(int itemId, int accountId)
		{
			if (UseEntityFramework)
			{
				var account = ExchangeAccounts
					.Where(a => a.ItemId == itemId && a.AccountId == accountId)
					.GroupJoin(ExchangeMailboxPlans, a => a.ArchivingMailboxPlanId, p => p.MailboxPlanId, (a, p) => new
					{
						Account = a,
						ArchivingMailboxPlans = p
					})
					.SelectMany(a => a.ArchivingMailboxPlans.DefaultIfEmpty(), (a, p) => new
					{
						a.Account.AccountId,
						a.Account.ItemId,
						a.Account.AccountType,
						a.Account.AccountName,
						a.Account.DisplayName,
						a.Account.PrimaryEmailAddress,
						a.Account.MailEnabledPublicFolder,
						a.Account.MailboxManagerActions,
						a.Account.SamAccountName,
						a.Account.MailboxPlanId,
						a.Account.MailboxPlan.MailboxPlan,
						a.Account.SubscriberNumber,
						a.Account.UserPrincipalName,
						a.Account.ArchivingMailboxPlanId,
						ArchivingMailboxPlan = p != null ? p.MailboxPlan : null,
						a.Account.EnableArchiving,
						a.Account.LevelId,
						a.Account.IsVip
					});

				/// <summary>TODO</summary>
				return EntityDataReader(account);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeAccount",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetExchangeAccountByAccountName(int itemId, string accountName)
		{
			if (UseEntityFramework)
			{
				var account = ExchangeAccounts
					.Where(a => a.ItemId == itemId && a.AccountName == accountName)
					.GroupJoin(ExchangeMailboxPlans, a => a.ArchivingMailboxPlanId, p => p.MailboxPlanId, (a, p) => new
					{
						Account = a,
						ArchvingMailboxPlans = p
					})
					.SelectMany(a => a.ArchvingMailboxPlans.DefaultIfEmpty(), (a, p) => new
					{
						a.Account.AccountId,
						a.Account.ItemId,
						a.Account.AccountType,
						a.Account.AccountName,
						a.Account.DisplayName,
						a.Account.PrimaryEmailAddress,
						a.Account.MailEnabledPublicFolder,
						a.Account.MailboxManagerActions,
						a.Account.SamAccountName,
						a.Account.MailboxPlanId,
						a.Account.MailboxPlan.MailboxPlan,
						a.Account.SubscriberNumber,
						a.Account.UserPrincipalName,
						a.Account.ArchivingMailboxPlanId,
						ArchivingMailboxPlan = p != null ? p.MailboxPlan : null,
						a.Account.EnableArchiving
					});

				/// <summary>TODO</summary>
				return EntityDataReader(account);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeAccountByAccountName",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountName", accountName));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetExchangeAccountByMailboxPlanId(int itemId, int MailboxPlanId)
		{
			if (UseEntityFramework)
			{
				var accountTypes = new ExchangeAccountType[] { ExchangeAccountType.Mailbox, ExchangeAccountType.Room,
					ExchangeAccountType.Equipment, ExchangeAccountType.SharedMailbox, ExchangeAccountType.JournalingMailbox };

				IQueryable<Data.Entities.ExchangeAccount> accounts = ExchangeAccounts;

				if (MailboxPlanId < 0)
				{
					accounts = accounts
						.Where(a => a.ItemId == itemId && a.MailboxPlanId == null && accountTypes.Any(t => t == a.AccountType));
				}
				/// <summary>TODO</summary>
				else if (itemId == 0)
				{
					accounts = accounts
						.Where(a => a.MailboxPlanId == MailboxPlanId && accountTypes.Any(t => t == a.AccountType));
				}
				else
				{
					accounts = accounts
						.Where(a => a.ItemId == itemId && a.MailboxPlanId == MailboxPlanId &&
							accountTypes.Any(t => t == a.AccountType));
				}

				var account = accounts
					.GroupJoin(ExchangeMailboxPlans, a => a.ArchivingMailboxPlanId, p => p.MailboxPlanId, (a, p) => new
					{
						Account = a,
						ArchivingMailboxPlans = p
					})
					.SelectMany(a => a.ArchivingMailboxPlans.DefaultIfEmpty(), (a, p) => new
					{
						a.Account,
						ArchivingMailboxPlan = p != null ? p.MailboxPlan : null
					})
					.Select(a => new
					{
						a.Account.AccountId,
						a.Account.ItemId,
						a.Account.AccountType,
						a.Account.AccountName,
						a.Account.DisplayName,
						a.Account.PrimaryEmailAddress,
						a.Account.MailEnabledPublicFolder,
						a.Account.MailboxManagerActions,
						a.Account.SamAccountName,
						a.Account.MailboxPlanId,
						a.Account.MailboxPlan.MailboxPlan,
						a.Account.SubscriberNumber,
						a.Account.UserPrincipalName,
						a.Account.ArchivingMailboxPlanId,
						a.ArchivingMailboxPlan,
						a.Account.EnableArchiving
					});

				/// <summary>TODO</summary>
				return EntityDataReader(account);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeAccountByMailboxPlanId",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", MailboxPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeAccountEmailAddresses(int accountId)
		{
			if (UseEntityFramework)
			{
				var addresses = ExchangeAccountEmailAddresses
					.Where(a => a.AccountId == accountId)
					.Select(a => new { a.AddressId, a.AccountId, a.EmailAddress });
				/// <summary>TODO</summary>
				return EntityDataReader(addresses);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeAccountEmailAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetExchangeOrganizationDomains(int itemId)
		{
			if (UseEntityFramework)
			{
				var domains = ExchangeOrganizationDomains
					.Where(d => d.ItemId == itemId)
					.Join(Domains, ed => ed.DomainId, d => d.DomainId, (ed, d) => new
					{
						ed.DomainId,
						d.DomainName,
						ed.IsHost,
						ed.DomainTypeId
					});
				/// <summary>TODO</summary>
				return EntityDataReader(domains);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeOrganizationDomains",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeAccounts(int itemId, ExchangeAccountType accountType)
		{
			if (UseEntityFramework)
			{
				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId && (accountType == ExchangeAccountType.Undefined || a.AccountType == accountType))
					.OrderBy(a => a.DisplayName)
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountType,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.MailEnabledPublicFolder,
						a.MailboxPlanId,
                        MailboxPlan = a.MailboxPlan != null ? a.MailboxPlan.MailboxPlan : null,
                        a.SubscriberNumber,
						a.UserPrincipalName
					});

				/// <summary>TODO</summary>
				return EntityDataReader(accounts);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeAccounts",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountType", accountType));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetExchangeAccountByAccountNameWithoutItemId(string userPrincipalName)
		{
			if (UseEntityFramework)
			{
				var account = ExchangeAccounts
					.Where(a => a.UserPrincipalName == userPrincipalName)
					.GroupJoin(ExchangeMailboxPlans, a => a.ArchivingMailboxPlanId, p => p.MailboxPlanId, (a, p) => new
					{
						Account = a,
						ArchivingMailboxPlans = p
					})
					.SelectMany(a => a.ArchivingMailboxPlans.DefaultIfEmpty(), (a, p) => new
					{
						a.Account.AccountId,
						a.Account.ItemId,
						a.Account.AccountType,
						a.Account.AccountName,
						a.Account.DisplayName,
						a.Account.PrimaryEmailAddress,
						a.Account.MailEnabledPublicFolder,
						a.Account.MailboxManagerActions,
						a.Account.SamAccountName,
						a.Account.MailboxPlanId,
						a.Account.MailboxPlan.MailboxPlan,
						a.Account.SubscriberNumber,
						a.Account.UserPrincipalName,
						a.Account.ArchivingMailboxPlanId,
						ArchivingMailboxPlan = p != null ? p.MailboxPlan : null,
						a.Account.EnableArchiving
					});

				/// <summary>TODO</summary>
				return EntityDataReader(account);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeAccountByAccountNameWithoutItemId",
					/// <summary>TODO</summary>
					new SqlParameter("@UserPrincipalName", userPrincipalName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeMailboxes(int itemId)
		{
			if (UseEntityFramework)
			{
				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId && (a.AccountType == ExchangeAccountType.Mailbox ||
						a.AccountType == ExchangeAccountType.Room || a.AccountType == ExchangeAccountType.Equipment))
					.OrderBy(a => a.AccountId)
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountType,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.MailEnabledPublicFolder,
						a.SubscriberNumber,
						a.UserPrincipalName
					});

				/// <summary>TODO</summary>
				return EntityDataReader(accounts);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeMailboxes",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader SearchExchangeAccountsByTypes(int actorId, int itemId, ExchangeAccountType[] accountTypes,
			string filterColumn, string filterValue, string sortColumn)
		{
			if (UseEntityFramework)
			{
				var packageId = ServiceItems
					.Where(i => i.ItemId == itemId)
					.Select(i => i.PackageId)
					.FirstOrDefault();
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId && accountTypes.Any(t => t == a.AccountType));

				if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
				{
					if (filterColumn == "PrimaryEmailAddress" &&
						(accountTypes.Length != 1 || accountTypes[0] != ExchangeAccountType.Contact))
					{
#if NETFRAMEWORK
						accounts = accounts.Where(a => a.ExchangeAccountEmailAddresses.Any(e => DbFunctions.Like(e.EmailAddress, filterValue)));
#else
						accounts = accounts.Where(a => a.ExchangeAccountEmailAddresses.Any(e => EF.Functions.Like(e.EmailAddress, filterValue)));
#endif
					}
					else
					{
						accounts = accounts.Where(DynamicFunctions.ColumnLike(accounts, filterColumn, filterValue));
					}
				}

				accounts = !string.IsNullOrEmpty(sortColumn) ? accounts.OrderBy(ColumnName(sortColumn)) : accounts.OrderBy(a => a.DisplayName);


				var accountsSelected = accounts
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountType,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.MailEnabledPublicFolder,
						a.MailboxPlanId,
						a.MailboxPlan.MailboxPlan,
						a.SubscriberNumber,
						a.UserPrincipalName
					});
				/// <summary>TODO</summary>
				return EntityDataReader(accountsSelected);
			}
			else
			{
				var accountTypesAsString = string.Join(",", accountTypes.Select(t => (int)t));

				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SearchExchangeAccountsByTypes",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountTypes", accountTypesAsString),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetExchangeAccountsPaged(int actorId, int itemId, string accountTypesAsString,
			string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows, bool archiving)
		{
			// check input parameters
			if (!Regex.IsMatch(accountTypesAsString, @"^\s*([0-9]+|[a-zA-Z_][a-zA-Z0-9_]*)(\s*,\s*([0-9]+|[a-zA-Z_][a-zA-Z0-9_]*))*\s*$", RegexOptions.Singleline))
			{
				/// <summary>TODO</summary>
				throw new ArgumentException("Wrong patameter", "accountTypesAsString");
			}

			accountTypesAsString = Regex.Replace(accountTypesAsString, @"[ \t]", ""); // remove whitespace

			var accountTypes = accountTypesAsString.Split(',')
				.Select(t =>
				{
					/// <summary>TODO</summary>
					int type;
					/// <summary>TODO</summary>
					ExchangeAccountType etype;
					if (int.TryParse(t, out type)) return (ExchangeAccountType)type;
					/// <summary>TODO</summary>
					else if (Enum.TryParse<ExchangeAccountType>(t, out etype)) return etype;
					/// <summary>TODO</summary>
					else throw new NotSupportedException($"Value {t} is not a valid ExchangeAccountType.");
				})
				.ToList();

			if (UseEntityFramework)
			{
				var packageId = ServiceItems
					.Where(i => i.ItemId == itemId)
					.Select(i => i.PackageId)
					.FirstOrDefault();
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId && accountTypes.Any(t => t == a.AccountType));

				if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
				{
					if (filterColumn == "PrimaryEmailAddress" &&
						(accountTypes.Count != 1 || accountTypes[0] != ExchangeAccountType.Contact))
					{
#if NETFRAMEWORK
						accounts = accounts.Where(a => a.ExchangeAccountEmailAddresses.Any(e => DbFunctions.Like(e.EmailAddress, filterValue)));
#else
						accounts = accounts.Where(a => a.ExchangeAccountEmailAddresses.Any(e => EF.Functions.Like(e.EmailAddress, filterValue)));
#endif
					}
					else
					{
						accounts = accounts.Where(DynamicFunctions.ColumnLike(accounts, filterColumn, filterValue));
					}
				}

				if (archiving)
				{
					accounts = accounts.Where(a => a.ArchivingMailboxPlanId > 0);
				}

				var count = accounts.Count();

				accounts = !string.IsNullOrEmpty(sortColumn) ? accounts.OrderBy(ColumnName(sortColumn)) : accounts.OrderBy(a => a.DisplayName);


				accounts = accounts.Skip(startRow).Take(maximumRows);

				var accountsSelected = accounts
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountType,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.MailEnabledPublicFolder,
						a.MailboxPlanId,
                        MaliboxPlan = a.MailboxPlan != null ? a.MailboxPlan.MailboxPlan : null,
                        a.SubscriberNumber,
						a.UserPrincipalName,
						a.LevelId,
						a.IsVip
					});

				/// <summary>TODO</summary>
				return EntityDataSet(count, accountsSelected);
			}
			else
			{
				accountTypesAsString = string.Join(",", accountTypes.Select(t => ((int)t).ToString()));

				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeAccountsPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountTypes", accountTypesAsString),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@MaximumRows", maximumRows),
					/// <summary>TODO</summary>
					new SqlParameter("@Archiving", archiving));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public IDataReader SearchExchangeAccounts(int actorId, int itemId, bool includeMailboxes,
			bool includeContacts, bool includeDistributionLists, bool includeRooms, bool includeEquipment, bool includeSharedMailbox,
			bool includeSecurityGroups, string filterColumn, string filterValue, string sortColumn)
		{
			if (UseEntityFramework)
			{
				var packageId = ServiceItems
					.Where(i => i.ItemId == itemId)
					.Select(i => i.PackageId)
					.FirstOrDefault();
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId && (
						(includeMailboxes && a.AccountType == ExchangeAccountType.Mailbox) ||
						(includeContacts && a.AccountType == ExchangeAccountType.Contact) ||
						(includeDistributionLists && a.AccountType == ExchangeAccountType.DistributionList) ||
						(includeRooms && a.AccountType == ExchangeAccountType.Room) ||
						(includeEquipment && a.AccountType == ExchangeAccountType.Equipment) ||
						(includeSecurityGroups && a.AccountType == ExchangeAccountType.SecurityGroup) ||
						(includeSharedMailbox && a.AccountType == ExchangeAccountType.SharedMailbox)));

				if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
				{
					accounts = accounts.Where(DynamicFunctions.ColumnLike(accounts, filterColumn, filterValue));
				}

				accounts = !string.IsNullOrEmpty(sortColumn) ? accounts.OrderBy(ColumnName(sortColumn)) : accounts.OrderBy(a => a.DisplayName);


				var accountsSelected = accounts
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountType,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.MailEnabledPublicFolder,
						a.SubscriberNumber,
						a.UserPrincipalName
					});

				/// <summary>TODO</summary>
				return EntityDataReader(accountsSelected);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SearchExchangeAccounts",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@IncludeMailboxes", includeMailboxes),
					/// <summary>TODO</summary>
					new SqlParameter("@IncludeContacts", includeContacts),
					/// <summary>TODO</summary>
					new SqlParameter("@IncludeDistributionLists", includeDistributionLists),
					/// <summary>TODO</summary>
					new SqlParameter("@IncludeRooms", includeRooms),
					/// <summary>TODO</summary>
					new SqlParameter("@IncludeEquipment", includeEquipment),
					/// <summary>TODO</summary>
					new SqlParameter("@IncludeSharedMailbox", includeSharedMailbox),
					/// <summary>TODO</summary>
					new SqlParameter("@IncludeSecurityGroups", includeSecurityGroups),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader SearchExchangeAccount(int actorId, ExchangeAccountType accountType, string primaryEmailAddress)
		{
			if (UseEntityFramework)
			{
				var account = ExchangeAccounts
					.Where(a => a.PrimaryEmailAddress == primaryEmailAddress && a.AccountType == accountType)
					.Select(a => new { a.AccountId, a.Item.PackageId })
					.FirstOrDefault();

				// check rights
				if (!CheckActorPackageRights(actorId, account.PackageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var accounts = ExchangeAccounts
					.Where(a => a.AccountId == account.AccountId)
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						PackageId = account.PackageId,
						a.AccountType,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.MailEnabledPublicFolder,
						a.MailboxManagerActions,
						a.SamAccountName,
						a.SubscriberNumber,
					});

				/// <summary>TODO</summary>
				return EntityDataReader(accounts);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SearchExchangeAccount",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountType", (int)accountType),
					/// <summary>TODO</summary>
					new SqlParameter("@PrimaryEmailAddress", primaryEmailAddress));
			}
		}
		#endregion

		#region Exchange Mailbox Plans
		/// <summary>Auto-generated member.</summary>
		public int AddExchangeMailboxPlan(int itemId, string mailboxPlan, bool enableActiveSync, bool enableIMAP, bool enableMAPI, bool enableOWA, bool enablePOP, bool enableAutoReply,
			bool isDefault, int issueWarningPct, int keepDeletedItemsDays, int mailboxSizeMB, int maxReceiveMessageSizeKB, int maxRecipients,
			int maxSendMessageSizeKB, int prohibitSendPct, int prohibitSendReceivePct, bool hideFromAddressBook, int mailboxPlanType,
			bool enabledLitigationHold, int recoverableItemsSpace, int recoverableItemsWarning, string litigationHoldUrl, string litigationHoldMsg,
			bool archiving, bool EnableArchiving, int ArchiveSizeMB, int ArchiveWarningPct, bool enableForceArchiveDeletion, bool isForJournaling)
		{
			if (UseEntityFramework)
			{
				if (!ExchangeMailboxPlans.Any(p => p.ItemId == itemId) && mailboxPlanType == 0)
				{
					isDefault = true;
				}
				/// <summary>TODO</summary>
				else if (isDefault && mailboxPlanType == 0)
				{
#if NETFRAMEWORK
					foreach (var plan0 in ExchangeMailboxPlans.Where(p => p.ItemId == itemId)) plan0.IsDefault = false;
					SaveChanges();
#else
					ExchangeMailboxPlans.Where(p => p.ItemId == itemId)
						.ExecuteUpdate(set => set.SetProperty(p => p.IsDefault, false));
#endif
				}

				var plan = new Data.Entities.ExchangeMailboxPlan()
				{
					ItemId = itemId,
					MailboxPlan = mailboxPlan,
					EnableActiveSync = enableActiveSync,
					EnableImap = enableIMAP,
					EnableMapi = enableMAPI,
					EnableOwa = enableOWA,
					EnablePop = enablePOP,
					EnableAutoReply = enableAutoReply,
					IsDefault = isDefault,
					IssueWarningPct = issueWarningPct,
					KeepDeletedItemsDays = keepDeletedItemsDays,
					MailboxSizeMb = mailboxSizeMB,
					MaxReceiveMessageSizeKb = maxReceiveMessageSizeKB,
					MaxRecipients = maxRecipients,
					MaxSendMessageSizeKb = maxSendMessageSizeKB,
					ProhibitSendPct = prohibitSendPct,
					ProhibitSendReceivePct = prohibitSendReceivePct,
					HideFromAddressBook = hideFromAddressBook,
					MailboxPlanType = mailboxPlanType,
					AllowLitigationHold = enabledLitigationHold,
					RecoverableItemsWarningPct = recoverableItemsWarning,
					RecoverableItemsSpace = recoverableItemsSpace,
					LitigationHoldUrl = litigationHoldUrl,
					LitigationHoldMsg = litigationHoldMsg,
					Archiving = archiving,
					EnableArchiving = EnableArchiving,
					ArchiveSizeMb = ArchiveSizeMB,
					ArchiveWarningPct = ArchiveWarningPct,
					EnableForceArchiveDeletion = enableForceArchiveDeletion,
					IsForJournaling = isForJournaling
				};
				ExchangeMailboxPlans.Add(plan);
				SaveChanges();

				return plan.MailboxPlanId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@MailboxPlanId", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddExchangeMailboxPlan",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlan", mailboxPlan),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableActiveSync", enableActiveSync),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableIMAP", enableIMAP),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableMAPI", enableMAPI),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableOWA", enableOWA),
					/// <summary>TODO</summary>
					new SqlParameter("@EnablePOP", enablePOP),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableAutoReply", enableAutoReply),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDefault", isDefault),
					/// <summary>TODO</summary>
					new SqlParameter("@IssueWarningPct", issueWarningPct),
					/// <summary>TODO</summary>
					new SqlParameter("@KeepDeletedItemsDays", keepDeletedItemsDays),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxSizeMB", mailboxSizeMB),
					/// <summary>TODO</summary>
					new SqlParameter("@MaxReceiveMessageSizeKB", maxReceiveMessageSizeKB),
					/// <summary>TODO</summary>
					new SqlParameter("@MaxRecipients", maxRecipients),
					/// <summary>TODO</summary>
					new SqlParameter("@MaxSendMessageSizeKB", maxSendMessageSizeKB),
					/// <summary>TODO</summary>
					new SqlParameter("@ProhibitSendPct", prohibitSendPct),
					/// <summary>TODO</summary>
					new SqlParameter("@ProhibitSendReceivePct", prohibitSendReceivePct),
					/// <summary>TODO</summary>
					new SqlParameter("@HideFromAddressBook", hideFromAddressBook),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanType", mailboxPlanType),
					/// <summary>TODO</summary>
					new SqlParameter("@AllowLitigationHold", enabledLitigationHold),
					/// <summary>TODO</summary>
					new SqlParameter("@RecoverableItemsWarningPct", recoverableItemsWarning),
					/// <summary>TODO</summary>
					new SqlParameter("@RecoverableItemsSpace", recoverableItemsSpace),
					/// <summary>TODO</summary>
					new SqlParameter("@LitigationHoldUrl", litigationHoldUrl),
					/// <summary>TODO</summary>
					new SqlParameter("@LitigationHoldMsg", litigationHoldMsg),
					/// <summary>TODO</summary>
					new SqlParameter("@Archiving", archiving),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableArchiving", EnableArchiving),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchiveSizeMB", ArchiveSizeMB),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchiveWarningPct", ArchiveWarningPct),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableForceArchiveDeletion", enableForceArchiveDeletion),
					/// <summary>TODO</summary>
					new SqlParameter("@IsForJournaling", isForJournaling));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateExchangeMailboxPlan(int mailboxPlanID, string mailboxPlan, bool enableActiveSync, bool enableIMAP, bool enableMAPI, bool enableOWA, bool enablePOP, bool enableAutoReply,
			bool isDefault, int issueWarningPct, int keepDeletedItemsDays, int mailboxSizeMB, int maxReceiveMessageSizeKB, int maxRecipients,
			int maxSendMessageSizeKB, int prohibitSendPct, int prohibitSendReceivePct, bool hideFromAddressBook, int mailboxPlanType,
			bool enabledLitigationHold, int recoverableItemsSpace, int recoverableItemsWarning, string litigationHoldUrl, string litigationHoldMsg,
			bool Archiving, bool EnableArchiving, int ArchiveSizeMB, int ArchiveWarningPct, bool enableForceArchiveDeletion, bool isForJournaling)
		{
			if (UseEntityFramework)
			{
				var plan = ExchangeMailboxPlans
					.FirstOrDefault(p => p.MailboxPlanId == mailboxPlanID);
				if (plan != null)
				{
					plan.MailboxPlan = mailboxPlan;
					plan.EnableActiveSync = enableActiveSync;
					plan.EnableImap = enableIMAP;
					plan.EnableMapi = enableMAPI;
					plan.EnableOwa = enableOWA;
					plan.EnablePop = enablePOP;
					plan.EnableAutoReply = enableAutoReply;
					plan.IsDefault = isDefault;
					plan.IssueWarningPct = issueWarningPct;
					plan.KeepDeletedItemsDays = keepDeletedItemsDays;
					plan.MailboxSizeMb = mailboxSizeMB;
					plan.MaxReceiveMessageSizeKb = maxReceiveMessageSizeKB;
					plan.MaxRecipients = maxRecipients;
					plan.MaxSendMessageSizeKb = maxSendMessageSizeKB;
					plan.ProhibitSendPct = prohibitSendPct;
					plan.ProhibitSendReceivePct = prohibitSendReceivePct;
					plan.HideFromAddressBook = hideFromAddressBook;
					plan.MailboxPlanType = mailboxPlanType;
					plan.AllowLitigationHold = enabledLitigationHold;
					plan.RecoverableItemsWarningPct = recoverableItemsWarning;
					plan.RecoverableItemsSpace = recoverableItemsSpace;
					plan.LitigationHoldUrl = litigationHoldUrl;
					plan.LitigationHoldMsg = litigationHoldMsg;
					plan.Archiving = Archiving;
					plan.EnableArchiving = EnableArchiving;
					plan.ArchiveSizeMb = ArchiveSizeMB;
					plan.ArchiveWarningPct = ArchiveWarningPct;
					plan.EnableForceArchiveDeletion = enableForceArchiveDeletion;
					plan.IsForJournaling = isForJournaling;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateExchangeMailboxPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanID", mailboxPlanID),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlan", mailboxPlan),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableActiveSync", enableActiveSync),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableIMAP", enableIMAP),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableMAPI", enableMAPI),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableOWA", enableOWA),
					/// <summary>TODO</summary>
					new SqlParameter("@EnablePOP", enablePOP),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableAutoReply", enableAutoReply),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDefault", isDefault),
					/// <summary>TODO</summary>
					new SqlParameter("@IssueWarningPct", issueWarningPct),
					/// <summary>TODO</summary>
					new SqlParameter("@KeepDeletedItemsDays", keepDeletedItemsDays),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxSizeMB", mailboxSizeMB),
					/// <summary>TODO</summary>
					new SqlParameter("@MaxReceiveMessageSizeKB", maxReceiveMessageSizeKB),
					/// <summary>TODO</summary>
					new SqlParameter("@MaxRecipients", maxRecipients),
					/// <summary>TODO</summary>
					new SqlParameter("@MaxSendMessageSizeKB", maxSendMessageSizeKB),
					/// <summary>TODO</summary>
					new SqlParameter("@ProhibitSendPct", prohibitSendPct),
					/// <summary>TODO</summary>
					new SqlParameter("@ProhibitSendReceivePct", prohibitSendReceivePct),
					/// <summary>TODO</summary>
					new SqlParameter("@HideFromAddressBook", hideFromAddressBook),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanType", mailboxPlanType),
					/// <summary>TODO</summary>
					new SqlParameter("@AllowLitigationHold", enabledLitigationHold),
					/// <summary>TODO</summary>
					new SqlParameter("@RecoverableItemsWarningPct", recoverableItemsWarning),
					/// <summary>TODO</summary>
					new SqlParameter("@RecoverableItemsSpace", recoverableItemsSpace),
					/// <summary>TODO</summary>
					new SqlParameter("@LitigationHoldUrl", litigationHoldUrl),
					/// <summary>TODO</summary>
					new SqlParameter("@LitigationHoldMsg", litigationHoldMsg),
					/// <summary>TODO</summary>
					new SqlParameter("@Archiving", Archiving),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableArchiving", EnableArchiving),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchiveSizeMB", ArchiveSizeMB),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchiveWarningPct", ArchiveWarningPct),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableForceArchiveDeletion", enableForceArchiveDeletion),
					/// <summary>TODO</summary>
					new SqlParameter("@IsForJournaling", isForJournaling));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExchangeMailboxPlan(int mailboxPlanId)
		{
			if (UseEntityFramework)
			{
				ExchangeMailboxPlans.Where(p => p.MailboxPlanId == mailboxPlanId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExchangeMailboxPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", mailboxPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeMailboxPlan(int mailboxPlanId)
		{
			if (UseEntityFramework)
			{
				var plan = ExchangeMailboxPlans
					.Where(p => p.MailboxPlanId == mailboxPlanId)
					.Select(p => new
					{
						p.MailboxPlanId,
						p.ItemId,
						p.MailboxPlan,
						p.EnableActiveSync,
						p.EnableImap,
						p.EnableMapi,
						p.EnableOwa,
						p.EnablePop,
						p.EnableAutoReply,
						p.IsDefault,
						p.IssueWarningPct,
						p.KeepDeletedItemsDays,
						p.MailboxSizeMb,
						p.MaxReceiveMessageSizeKb,
						p.MaxRecipients,
						p.MaxSendMessageSizeKb,
						p.ProhibitSendPct,
						p.ProhibitSendReceivePct,
						p.HideFromAddressBook,
						p.MailboxPlanType,
						p.AllowLitigationHold,
						p.RecoverableItemsWarningPct,
						p.RecoverableItemsSpace,
						p.LitigationHoldUrl,
						p.LitigationHoldMsg,
						p.Archiving,
						p.EnableArchiving,
						p.ArchiveSizeMb,
						p.ArchiveWarningPct,
						p.EnableForceArchiveDeletion,
						p.IsForJournaling
					});
				/// <summary>TODO</summary>
				return EntityDataReader(plan);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeMailboxPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", mailboxPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeMailboxPlans(int itemId, bool archiving)
		{
			if (UseEntityFramework)
			{
				var plans = ExchangeMailboxPlans
					.Where(p => p.ItemId == itemId &&
						(p.Archiving == archiving || (!(archiving) && p.Archiving == null)))
					.OrderBy(p => p.MailboxPlan)
					.Select(p => new
					{
						p.MailboxPlanId,
						p.ItemId,
						p.MailboxPlan,
						p.EnableActiveSync,
						p.EnableImap,
						p.EnableMapi,
						p.EnableOwa,
						p.EnablePop,
						p.EnableAutoReply,
						p.IsDefault,
						p.IssueWarningPct,
						p.KeepDeletedItemsDays,
						p.MailboxSizeMb,
						p.MaxReceiveMessageSizeKb,
						p.MaxRecipients,
						p.MaxSendMessageSizeKb,
						p.ProhibitSendPct,
						p.ProhibitSendReceivePct,
						p.HideFromAddressBook,
						p.MailboxPlanType,
						p.Archiving,
						p.EnableArchiving,
						p.ArchiveSizeMb,
						p.ArchiveWarningPct,
						p.EnableForceArchiveDeletion,
						p.IsForJournaling
					});
				/// <summary>TODO</summary>
				return EntityDataReader(plans);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeMailboxPlans",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@Archiving", archiving));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeOrganization(int itemId)
		{
			if (UseEntityFramework)
			{
				var orgs = ExchangeOrganizations
					.Where(o => o.ItemId == itemId)
					.Select(o => new
					{
						o.ItemId,
						o.ExchangeMailboxPlanId,
						o.LyncUserPlanId,
						o.SfBuserPlanId
					});
				/// <summary>TODO</summary>
				return EntityDataReader(orgs);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeOrganization",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void SetOrganizationDefaultExchangeMailboxPlan(int itemId, int mailboxPlanId)
		{
			if (UseEntityFramework)
			{
                ExchangeOrganizations.Where(o => o.ItemId == itemId)
                    .ExecuteUpdate(eo => new Data.Entities.ExchangeOrganization() { ExchangeMailboxPlanId = mailboxPlanId });
            }
            else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SetOrganizationDefaultExchangeMailboxPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", mailboxPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void SetExchangeAccountMailboxPlan(int accountId, int mailboxPlanId, int archivePlanId, bool EnableArchiving)
		{
			if (UseEntityFramework)
			{
                ExchangeAccounts.Where(a => a.AccountId == accountId)
                    .ExecuteUpdate(ea => new Data.Entities.ExchangeAccount()
                    {
                        MailboxPlanId = mailboxPlanId,
                        ArchivingMailboxPlanId = archivePlanId,
                        EnableArchiving = EnableArchiving
                    });
            }
            else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SetExchangeAccountMailboxplan",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", (mailboxPlanId == 0) ? (object)DBNull.Value : (object)mailboxPlanId),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchivingMailboxPlanId", (archivePlanId < 1) ? (object)DBNull.Value : (object)archivePlanId),
					/// <summary>TODO</summary>
					new SqlParameter("@EnableArchiving", EnableArchiving));
			}
		}
		#endregion

		#region Exchange Retention Policy Tags
		/// <summary>Auto-generated member.</summary>
		public int AddExchangeRetentionPolicyTag(int ItemID, string TagName, int TagType, int AgeLimitForRetention, int RetentionAction)
		{
			if (UseEntityFramework)
			{
				var tag = new Data.Entities.ExchangeRetentionPolicyTag()
				{
					ItemId = ItemID,
					TagName = TagName,
					TagType = TagType,
					AgeLimitForRetention = AgeLimitForRetention,
					RetentionAction = RetentionAction
				};
				ExchangeRetentionPolicyTags.Add(tag);
				SaveChanges();
				return tag.TagId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@TagID", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddExchangeRetentionPolicyTag",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", ItemID),
					/// <summary>TODO</summary>
					new SqlParameter("@TagName", TagName),
					/// <summary>TODO</summary>
					new SqlParameter("@TagType", TagType),
					/// <summary>TODO</summary>
					new SqlParameter("@AgeLimitForRetention", AgeLimitForRetention),
					/// <summary>TODO</summary>
					new SqlParameter("@RetentionAction", RetentionAction));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateExchangeRetentionPolicyTag(int TagID, int ItemID, string TagName, int TagType, int AgeLimitForRetention, int RetentionAction)
		{
			if (UseEntityFramework)
			{
				var tag = ExchangeRetentionPolicyTags
					.FirstOrDefault(t => t.TagId == TagID);
				if (tag != null)
				{
					tag.ItemId = ItemID;
					tag.TagName = TagName;
					tag.TagType = TagType;
					tag.AgeLimitForRetention = AgeLimitForRetention;
					tag.RetentionAction = RetentionAction;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateExchangeRetentionPolicyTag",
					/// <summary>TODO</summary>
					new SqlParameter("@TagID", TagID),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", ItemID),
					/// <summary>TODO</summary>
					new SqlParameter("@TagName", TagName),
					/// <summary>TODO</summary>
					new SqlParameter("@TagType", TagType),
					/// <summary>TODO</summary>
					new SqlParameter("@AgeLimitForRetention", AgeLimitForRetention),
					/// <summary>TODO</summary>
					new SqlParameter("@RetentionAction", RetentionAction));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExchangeRetentionPolicyTag(int TagID)
		{
			if (UseEntityFramework)
			{
				ExchangeRetentionPolicyTags
					.Where(t => t.TagId == TagID)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExchangeRetentionPolicyTag",
					/// <summary>TODO</summary>
					new SqlParameter("@TagID", TagID));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeRetentionPolicyTag(int TagID)
		{
			if (UseEntityFramework)
			{
				var tag = ExchangeRetentionPolicyTags
					.Where(t => t.TagId == TagID)
					.Select(t => new
					{
						t.TagId,
						t.ItemId,
						t.TagName,
						t.TagType,
						t.AgeLimitForRetention,
						t.RetentionAction
					});
				/// <summary>TODO</summary>
				return EntityDataReader(tag);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeRetentionPolicyTag",
					/// <summary>TODO</summary>
					new SqlParameter("@TagID", TagID));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeRetentionPolicyTags(int itemId)
		{
			if (UseEntityFramework)
			{
				var tags = ExchangeRetentionPolicyTags
					.Where(t => t.ItemId == itemId)
					.OrderBy(t => t.TagName)
					.Select(t => new
					{
						t.TagId,
						t.ItemId,
						t.TagName,
						t.TagType,
						t.AgeLimitForRetention,
						t.RetentionAction
					});
				/// <summary>TODO</summary>
				return EntityDataReader(tags);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeRetentionPolicyTags",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddExchangeMailboxPlanRetentionPolicyTag(int TagID, int MailboxPlanId)
		{
			if (UseEntityFramework)
			{
				var tag = new Data.Entities.ExchangeMailboxPlanRetentionPolicyTag()
				{
					TagId = TagID,
					MailboxPlanId = MailboxPlanId
				};
				ExchangeMailboxPlanRetentionPolicyTags.Add(tag);
				SaveChanges();
				return tag.PlanTagId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@PlanTagID", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddExchangeMailboxPlanRetentionPolicyTag",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@TagID", TagID),
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", MailboxPlanId));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void DeleteExchangeMailboxPlanRetentionPolicyTag(int PlanTagID)
		{
			if (UseEntityFramework)
			{
				ExchangeMailboxPlanRetentionPolicyTags.Where(t => t.PlanTagId == PlanTagID)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExchangeMailboxPlanRetentionPolicyTag",
					/// <summary>TODO</summary>
					new SqlParameter("@PlanTagID", PlanTagID));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeMailboxPlanRetentionPolicyTags(int MailboxPlanId)
		{
			if (UseEntityFramework)
			{
				var tags = ExchangeMailboxPlanRetentionPolicyTags
					.Where(t => t.MailboxPlanId == MailboxPlanId)
					.GroupJoin(ExchangeMailboxPlans, t => t.MailboxPlanId, p => p.MailboxPlanId, (t, p) => new
					{
						t.PlanTagId,
						t.TagId,
						t.MailboxPlanId,
						MailboxPlans = p
					})
					.SelectMany(t => t.MailboxPlans.DefaultIfEmpty(), (t, p) => new
					{
						t.PlanTagId,
						t.TagId,
						t.MailboxPlanId,
						MailboxPlan = p != null ? p.MailboxPlan : null
					})
					.GroupJoin(ExchangeRetentionPolicyTags, d => d.TagId, t => t.TagId, (d, t) => new
					{
						d.PlanTagId,
						d.TagId,
						d.MailboxPlanId,
						d.MailboxPlan,
						Tags = t
					})
					.SelectMany(d => d.Tags.DefaultIfEmpty(), (d, t) => new
					{
						d.PlanTagId,
						d.TagId,
						d.MailboxPlanId,
						d.MailboxPlan,
						TagName = t != null ? t.TagName : null
					});
				/// <summary>TODO</summary>
				return EntityDataReader(tags);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeMailboxPlanRetentionPolicyTags",
					/// <summary>TODO</summary>
					new SqlParameter("@MailboxPlanId", MailboxPlanId));
			}
		}
		#endregion

		#region Exchange Disclaimers
		/// <summary>Auto-generated member.</summary>
		public int AddExchangeDisclaimer(int itemID, ExchangeDisclaimer disclaimer)
		{
			if (UseEntityFramework)
			{
				var d = new Data.Entities.ExchangeDisclaimer()
				{
					ItemId = itemID,
					DisclaimerName = disclaimer.DisclaimerName,
					DisclaimerText = disclaimer.DisclaimerText
				};
				ExchangeDisclaimers.Add(d);
				SaveChanges();
				return d.ExchangeDisclaimerId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@ExchangeDisclaimerId", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddExchangeDisclaimer",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemID),
					/// <summary>TODO</summary>
					new SqlParameter("@DisclaimerName", disclaimer.DisclaimerName),
					/// <summary>TODO</summary>
					new SqlParameter("@DisclaimerText", disclaimer.DisclaimerText));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateExchangeDisclaimer(int itemID, ExchangeDisclaimer disclaimer)
		{
			if (UseEntityFramework)
			{
#if NETFRAMEWORK
				var d = ExchangeDisclaimers
					.FirstOrDefault(e => e.ExchangeDisclaimerId == disclaimer.ExchangeDisclaimerId);
				if (d != null)
				{
					d.DisclaimerName = disclaimer.DisclaimerName;
					d.DisclaimerText = disclaimer.DisclaimerText;
					SaveChanges();
				}
#else
				ExchangeDisclaimers
					.Where(e => e.ExchangeDisclaimerId == disclaimer.ExchangeDisclaimerId)
					.ExecuteUpdate(set => set
						.SetProperty(d => d.DisclaimerName, disclaimer.DisclaimerName)
						.SetProperty(d => d.DisclaimerText, disclaimer.DisclaimerText));
#endif
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateExchangeDisclaimer",
					/// <summary>TODO</summary>
					new SqlParameter("@ExchangeDisclaimerId", disclaimer.ExchangeDisclaimerId),
					/// <summary>TODO</summary>
					new SqlParameter("@DisclaimerName", disclaimer.DisclaimerName),
					/// <summary>TODO</summary>
					new SqlParameter("@DisclaimerText", disclaimer.DisclaimerText));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExchangeDisclaimer(int exchangeDisclaimerId)
		{
			if (UseEntityFramework)
			{
				ExchangeDisclaimers.Where(d => d.ExchangeDisclaimerId == exchangeDisclaimerId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExchangeDisclaimer",
					/// <summary>TODO</summary>
					new SqlParameter("@ExchangeDisclaimerId", exchangeDisclaimerId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeDisclaimer(int exchangeDisclaimerId)
		{
			if (UseEntityFramework)
			{
				var disclaimers = ExchangeDisclaimers
					.Where(d => d.ExchangeDisclaimerId == exchangeDisclaimerId)
					.Select(d => new
					{
						d.ExchangeDisclaimerId,
						d.ItemId,
						d.DisclaimerName,
						d.DisclaimerText
					});
				/// <summary>TODO</summary>
				return EntityDataReader(disclaimers);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeDisclaimer",
					/// <summary>TODO</summary>
					new SqlParameter("@ExchangeDisclaimerId", exchangeDisclaimerId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetExchangeDisclaimers(int itemId)
		{
			if (UseEntityFramework)
			{
				var disclaimers = ExchangeDisclaimers
					.Where(d => d.ItemId == itemId)
					.Select(d => new
					{
						d.ExchangeDisclaimerId,
						d.ItemId,
						d.DisclaimerName,
						d.DisclaimerText
					})
					.OrderBy(d => d.DisclaimerName);
				/// <summary>TODO</summary>
				return EntityDataReader(disclaimers);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeDisclaimers",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void SetExchangeAccountDisclaimerId(int AccountID, int ExchangeDisclaimerId)
		{
			if (UseEntityFramework)
			{
                ExchangeAccounts
                    .Where(a => a.AccountId == AccountID)
                    .ExecuteUpdate(ea => new Data.Entities.ExchangeAccount()
                    {
                        ExchangeDisclaimerId = ExchangeDisclaimerId
                    });
            }
            else
			{
				object id = null;
				if (ExchangeDisclaimerId != -1) id = ExchangeDisclaimerId;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SetExchangeAccountDisclaimerId",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", AccountID),
					/// <summary>TODO</summary>
					new SqlParameter("@ExchangeDisclaimerId", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetExchangeAccountDisclaimerId(int AccountID)
		{
			if (UseEntityFramework)
			{
				return ExchangeAccounts
					.Where(a => a.AccountId == AccountID)
					.Select(a => a.ExchangeDisclaimerId)
					.FirstOrDefault() ?? -1;
			}
			else
			{
				object objReturn = SqlHelper.ExecuteScalar(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetExchangeAccountDisclaimerId",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", AccountID));

				/// <summary>TODO</summary>
				int ret;
				if (!int.TryParse(objReturn.ToString(), out ret)) return -1;
				/// <summary>TODO</summary>
				return ret;
			}
		}
		#endregion

		#region Organizations
		/// <summary>Auto-generated member.</summary>

		public int AddAccessToken(AccessToken token)
		{
			/// <summary>TODO</summary>
			return AddAccessToken(token.AccessTokenGuid, token.AccountId, token.ItemId, token.ExpirationDate, token.TokenType);
		}
		/// <summary>Auto-generated member.</summary>

		public int AddAccessToken(Guid accessToken, int accountId, int itemId, DateTime expirationDate, AccessTokenTypes type)
		{
			if (UseEntityFramework)
			{
				var token = new Data.Entities.AccessToken()
				{
					AccessTokenGuid = accessToken,
					ExpirationDate = expirationDate,
					AccountId = accountId,
					ItemId = itemId,
					TokenType = type
				};
				AccessTokens.Add(token);
				SaveChanges();
				return token.Id;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@TokenID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddAccessToken",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@AccessToken", accessToken),
					/// <summary>TODO</summary>
					new SqlParameter("@ExpirationDate", expirationDate),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@TokenType", (int)type));

				// read identity
				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void SetAccessTokenResponseMessage(Guid accessToken, string response)
		{
			if (UseEntityFramework)
			{
                AccessTokens
                    .Where(t => t.AccessTokenGuid == accessToken)
                    .ExecuteUpdate(at => new Data.Entities.AccessToken() { SmsResponse = response });
            }
            else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SetAccessTokenSmsResponse",
					/// <summary>TODO</summary>
					new SqlParameter("@AccessToken", accessToken),
					/// <summary>TODO</summary>
					new SqlParameter("@SmsResponse", response));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExpiredAccessTokens()
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				AccessTokens.Where(t => t.ExpirationDate < now).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExpiredAccessTokenTokens");
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetAccessTokenByAccessToken(Guid accessToken, AccessTokenTypes type)
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				var token = AccessTokens
					.Where(t => t.AccessTokenGuid == accessToken && t.ExpirationDate > now && t.TokenType == type)
					.Select(t => new
					{
						t.Id,
						t.AccessTokenGuid,
						t.ExpirationDate,
						t.AccountId,
						t.ItemId,
						t.TokenType,
						t.SmsResponse
					});
				/// <summary>TODO</summary>
				return EntityDataReader(token);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetAccessTokenByAccessToken",
					/// <summary>TODO</summary>
					new SqlParameter("@AccessToken", accessToken),
					/// <summary>TODO</summary>
					new SqlParameter("@TokenType", type));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteAccessToken(Guid accessToken, AccessTokenTypes type)
		{
			if (UseEntityFramework)
			{
				AccessTokens
					.Where(a => a.AccessTokenGuid == accessToken && a.TokenType == type)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteAccessToken",
					/// <summary>TODO</summary>
					new SqlParameter("@AccessToken", accessToken),
					/// <summary>TODO</summary>
					new SqlParameter("@TokenType", type));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateOrganizationSettings(int itemId, string settingsName, string xml)
		{
			if (UseEntityFramework)
			{
				var setting = ExchangeOrganizationSettings
					.FirstOrDefault(s => s.ItemId == itemId && s.SettingsName == settingsName);
				if (setting != null)
				{
					setting.Xml = xml;
				}
				else
				{
					setting = new Data.Entities.ExchangeOrganizationSetting()
					{
						ItemId = itemId,
						SettingsName = settingsName,
						Xml = xml
					};
					ExchangeOrganizationSettings.Add(setting);
				}
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateExchangeOrganizationSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SettingsName", settingsName),
					/// <summary>TODO</summary>
					new SqlParameter("@Xml", xml));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetOrganizationSettings(int itemId, string settingsName)
		{
			if (UseEntityFramework)
			{
				var settings = ExchangeOrganizationSettings
					.Where(s => s.ItemId == itemId && s.SettingsName == settingsName)
					.Select(s => new
					{
						s.ItemId,
						s.SettingsName,
						s.Xml
					});
				/// <summary>TODO</summary>
				return EntityDataReader(settings);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetExchangeOrganizationSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SettingsName", settingsName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddOrganizationDeletedUser(int accountId, int originAT, string storagePath, string folderName, string fileName, DateTime expirationDate)
		{
			if (UseEntityFramework)
			{
				var account = new Data.Entities.ExchangeDeletedAccount()
				{
					AccountId = accountId,
					OriginAt = originAT,
					StoragePath = storagePath,
					FolderName = folderName,
					FileName = fileName,
					ExpirationDate = expirationDate
				};
				ExchangeDeletedAccounts.Add(account);
				SaveChanges();
				return account.Id;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@ID", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddOrganizationDeletedUser",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@OriginAT", originAT),
					/// <summary>TODO</summary>
					new SqlParameter("@StoragePath", storagePath),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderName", folderName),
					/// <summary>TODO</summary>
					new SqlParameter("@FileName", fileName),
					/// <summary>TODO</summary>
					new SqlParameter("@ExpirationDate", expirationDate));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteOrganizationDeletedUser(int id)
		{
			if (UseEntityFramework)
			{
				ExchangeDeletedAccounts
					.Where(a => a.AccountId == id)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteOrganizationDeletedUser",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetOrganizationDeletedUser(int accountId)
		{
			if (UseEntityFramework)
			{
				var account = ExchangeDeletedAccounts
					.Where(a => a.AccountId == accountId)
					.Select(a => new
					{
						a.AccountId,
						a.OriginAt,
						a.StoragePath,
						a.FolderName,
						a.FileName,
						a.ExpirationDate
					});
				/// <summary>TODO</summary>
				return EntityDataReader(account);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetOrganizationDeletedUser",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetAdditionalGroups(int userId)
		{
			if (UseEntityFramework)
			{
				var groups = AdditionalGroups
					.Where(g => g.UserId == userId);
				/// <summary>TODO</summary>
				return EntityDataReader(groups);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetAdditionalGroups",
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddAdditionalGroup(int userId, string groupName)
		{
			if (UseEntityFramework)
			{
				var group = new Data.Entities.AdditionalGroup()
				{
					UserId = userId,
					GroupName = groupName
				};
				AdditionalGroups.Add(group);
				SaveChanges();
				return group.Id;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@GroupID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddAdditionalGroup",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userId),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupName", groupName));

				// read identity
				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteAdditionalGroup(int groupId)
		{
			if (UseEntityFramework)
			{
				AdditionalGroups
					.Where(a => a.Id == groupId)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteAdditionalGroup",
					/// <summary>TODO</summary>
					new SqlParameter("@GroupID", groupId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateAdditionalGroup(int groupId, string groupName)
		{
			if (UseEntityFramework)
			{
#if NETCOREAPP
				AdditionalGroups
					.Where(a => a.Id == groupId)
					.ExecuteUpdate(e => e
						.SetProperty(p => p.GroupName, groupName));
#else
				var group = AdditionalGroups.FirstOrDefault(a => a.Id == groupId);
				if (group != null)
				{
					group.GroupName = groupName;
					SaveChanges();
				}
#endif
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateAdditionalGroup",
					/// <summary>TODO</summary>
					new SqlParameter("@GroupID", groupId),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupName", groupName));
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void DeleteOrganizationUser(int itemId)
		{
			if (UseEntityFramework)
			{
				ExchangeAccounts
					.Where(a => a.ItemId == itemId)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure, "DeleteOrganizationUsers",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetItemIdByOrganizationId(string id)
		{
			if (UseEntityFramework)
			{
				return ExchangeOrganizations
					.Where(a => a.OrganizationId == id)
					.Select(a => a.ItemId)
					.FirstOrDefault();
			}
			else
			{
				object obj = SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "GetItemIdByOrganizationId",
					/// <summary>TODO</summary>
					new SqlParameter("@OrganizationId", id));

				return (obj == null || DBNull.Value == obj) ? 0 : (int)obj;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetOrganizationStatistics(int itemId)
		{
			if (UseEntityFramework)
			{
				var accounts = ExchangeAccounts.Where(a => a.ItemId == itemId);
				var stats = new
				{
					CreatedUsers = accounts
						.Where(a => a.AccountType == ExchangeAccountType.User ||
							a.AccountType == ExchangeAccountType.Mailbox ||
							a.AccountType == ExchangeAccountType.Equipment ||
							a.AccountType == ExchangeAccountType.Room)
						.Count(),
					CreatedDomains = ExchangeOrganizationDomains
						.Where(d => d.ItemId == itemId)
						.Count(),
					CreatedGroups = accounts
						.Where(a => a.AccountType == ExchangeAccountType.SecurityGroup ||
							a.AccountType == ExchangeAccountType.DefaultSecurityGroup)
						.Count(),
					DeletedUsers = accounts
						.Where(a => a.AccountType == ExchangeAccountType.DeletedUser)
						.Count()
				};
				/// <summary>TODO</summary>
				return EntityDataReader(new[] { stats });
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetOrganizationStatistics",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetOrganizationGroupsByDisplayName(int itemId, string displayName)
		{
			if (UseEntityFramework)
			{
				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId && a.DisplayName == displayName &&
						(a.AccountType == ExchangeAccountType.SecurityGroup ||
						a.AccountType == ExchangeAccountType.DefaultSecurityGroup))
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountType,
						a.AccountName,
						a.DisplayName,
						a.UserPrincipalName
					});
				/// <summary>TODO</summary>
				return EntityDataReader(accounts);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetOrganizationGroupsByDisplayName",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@DisplayName", displayName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader SearchOrganizationAccounts(int actorId, int itemId,
			string filterColumn, string filterValue, string sortColumn, bool includeMailboxes)
		{
			if (UseEntityFramework)
			{
				var packageId = ServiceItems
					.Where(i => i.ItemId == itemId)
					.Select(i => i.PackageId)
					.FirstOrDefault();
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId &&
						(a.AccountType == ExchangeAccountType.User ||
						a.AccountType == ExchangeAccountType.Mailbox && includeMailboxes))
					.GroupJoin(LyncUsers, a => a.AccountId, lu => lu.AccountId, (a, lus) => new { A = a, LyncUsers = lus })
					.SelectMany(a => a.LyncUsers.DefaultIfEmpty(), (a, lu) => new { A = a.A, IsLyncUser = lu != null })
					.GroupJoin(SfBUsers, a => a.A.AccountId, su => su.AccountId, (a, sus) => new
					{
						a.A,
						a.IsLyncUser,
						SfBUsers = sus
					})
					.SelectMany(a => a.SfBUsers.DefaultIfEmpty(), (a, su) => new
					{
						a.A.AccountId,
						a.A.ItemId,
						a.A.AccountType,
						a.A.AccountName,
						a.A.DisplayName,
						a.A.PrimaryEmailAddress,
						a.A.SubscriberNumber,
						a.A.UserPrincipalName,
						a.A.LevelId,
						a.A.IsVip,
						a.IsLyncUser,
						IsSfBUser = su != null
					});

				if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
				{
					accounts = accounts.Where(DynamicFunctions.ColumnLike(accounts, filterColumn, filterValue));
				}

				accounts = !string.IsNullOrEmpty(sortColumn) ? accounts.OrderBy(ColumnName(sortColumn)) : accounts.OrderBy(a => a.DisplayName);








				/// <summary>TODO</summary>
				return EntityDataReader(accounts);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SearchOrganizationAccounts",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@IncludeMailboxes", includeMailboxes));
			}
		}

		class OrganizationObject
		{
			/// <summary>Auto-generated member.</summary>
			public string ObjectName { get; set; }
			/// <summary>Auto-generated member.</summary>
			public int ObjectId { get; set; }
			/// <summary>Auto-generated member.</summary>
			public ExchangeAccountType ObjectType { get; set; }
			/// <summary>Auto-generated member.</summary>
			public string DisplayName { get; set; }
			/// <summary>Auto-generated member.</summary>
			public int OwnerId { get; set; }
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetOrganizationObjectsByDomain(int itemId, string domainName)
		{
			if (UseEntityFramework)
			{
				var objects = ExchangeAccounts
					.Where(a => a.AccountType != ExchangeAccountType.Contact &&
#if NETFRAMEWORK
						DbFunctions.Like(a.UserPrincipalName, "%@" + domainName))
#else
						EF.Functions.Like(a.UserPrincipalName, "%@" + domainName))
#endif
					.Select(a => new OrganizationObject()
					{
						ObjectName = "ExchangeAccounts",
						ObjectId = a.AccountId,
						ObjectType = a.AccountType,
						DisplayName = a.DisplayName,
						OwnerId = 0
					})
					.Union(ExchangeAccountEmailAddresses
						.Where(a =>
#if NETFRAMEWORK
							DbFunctions.Like(a.EmailAddress, "%@" + domainName) &&
#else
							EF.Functions.Like(a.EmailAddress, "%@" + domainName) &&
#endif
							a.EmailAddress != a.Account.PrimaryEmailAddress &&
							a.EmailAddress != a.Account.UserPrincipalName)
						.Select(a => new OrganizationObject()
						{
							ObjectName = "ExchangeAccountEmailAddresses",
							ObjectId = a.AddressId,
							ObjectType = a.Account.AccountType,
							DisplayName = a.EmailAddress,
							OwnerId = a.AccountId
						}))
					.Union(ExchangeAccounts
						.Join(SfBUsers, a => a.AccountId, u => u.AccountId, (a, u) => new { Account = a, User = u })
						.Where(a =>
#if NETFRAMEWORK
							DbFunctions.Like(a.User.SipAddress, "%@" + domainName))
#else
							EF.Functions.Like(a.User.SipAddress, "%@" + domainName))
#endif
						.Select(a => new OrganizationObject()
						{
							ObjectName = "SfBUsers",
							ObjectId = a.Account.AccountId,
							ObjectType = a.Account.AccountType,
							DisplayName = a.Account.DisplayName,
							OwnerId = 0
						}))
					.Union(ExchangeAccounts
						.Join(LyncUsers, a => a.AccountId, u => u.AccountId, (a, u) => new { Account = a, User = u })
						.Where(a =>
#if NETFRAMEWORK
							DbFunctions.Like(a.User.SipAddress, "%@" + domainName))
#else
							EF.Functions.Like(a.User.SipAddress, "%@" + domainName))
#endif
						.Select(a => new OrganizationObject()
						{
							ObjectName = "LyncUsers",
							ObjectId = a.Account.AccountId,
							ObjectType = a.Account.AccountType,
							DisplayName = a.Account.DisplayName,
							OwnerId = 0
						}))
					.OrderBy(o => o.DisplayName);
				/// <summary>TODO</summary>
				return EntityDataSet(objects);
			}
			else
			{
				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetOrganizationObjectsByDomain",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainName", domainName));
			}
		}
		#endregion

		#region CRM
		/// <summary>Auto-generated member.</summary>

		public int GetCRMUsersCount(int itemId, string name, string email, int CALType)
		{
			if (UseEntityFramework)
			{
				if (string.IsNullOrEmpty(name)) name = "%";
				if (string.IsNullOrEmpty(email)) email = "%";

				return ExchangeAccounts
					.Where(a => a.ItemId == itemId &&
#if NETFRAMEWORK
						DbFunctions.Like(a.DisplayName, name) && DbFunctions.Like(a.PrimaryEmailAddress, email))
#else
						EF.Functions.Like(a.DisplayName, name) && EF.Functions.Like(a.PrimaryEmailAddress, email))
#endif
					.Join(CrmUsers, a => a.AccountId, u => u.AccountId, (a, u) => u.CalType)
					.Count(cal => cal == CALType || CALType == -1);
			}
			else
			{
				SqlParameter[] sqlParams = new SqlParameter[]
				{
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					GetFilterSqlParam("@Name", name),
					GetFilterSqlParam("@Email", email),
					/// <summary>TODO</summary>
					new SqlParameter("@CALType", CALType)
				};

				return (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "GetCRMUsersCount", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>

		private SqlParameter GetFilterSqlParam(string paramName, string value)
		{
			if (string.IsNullOrEmpty(value))
				/// <summary>TODO</summary>
				return new SqlParameter(paramName, DBNull.Value);

			/// <summary>TODO</summary>
			return new SqlParameter(paramName, value);
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetCrmUsers(int itemId, string sortColumn, string sortDirection, string name, string email, int startRow, int count)
		{
			if (UseEntityFramework)
			{
				if (string.IsNullOrEmpty(name)) name = "%";
				if (string.IsNullOrEmpty(email)) email = "%";

				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId &&
#if NETFRAMEWORK
						DbFunctions.Like(a.DisplayName, name) && DbFunctions.Like(a.PrimaryEmailAddress, email))
#else
						EF.Functions.Like(a.DisplayName, name) && EF.Functions.Like(a.PrimaryEmailAddress, email))
#endif
					.Join(CrmUsers, a => a.AccountId, u => u.AccountId, (a, u) => a)
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.SamAccountName
					});

				accounts = string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase)
					? (sortColumn == "DisplayName" ? accounts.OrderBy(a => a.DisplayName) : accounts.OrderBy(a => a.PrimaryEmailAddress))
					: (sortColumn == "DisplayName" ? accounts.OrderByDescending(a => a.DisplayName) : accounts.OrderByDescending(a => a.PrimaryEmailAddress));
				accounts = accounts.Skip(startRow).Take(count);
				/// <summary>TODO</summary>
				return EntityDataReader(accounts);
			}
			else
			{
				SqlParameter[] sqlParams = new SqlParameter[] {
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", sortColumn),
					/// <summary>TODO</summary>
					new SqlParameter("@SortDirection", sortDirection),
					GetFilterSqlParam("@Name", name),
					GetFilterSqlParam("@Email", email),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("Count", count)
				};

				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetCRMUsers", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetCRMOrganizationUsers(int itemId)
		{
			if (UseEntityFramework)
			{
				var accounts = ExchangeAccounts
					.Where(a => a.ItemId == itemId)
					.Join(CrmUsers, a => a.AccountId, u => u.AccountId, (a, u) => a)
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.SamAccountName
					});
				/// <summary>TODO</summary>
				return EntityDataReader(accounts);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure, "GetCRMOrganizationUsers",
					new SqlParameter[] { new SqlParameter("@ItemID", itemId) });
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void CreateCRMUser(int itemId, Guid crmId, Guid businessUnitId, int CALType)
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				var user = new Data.Entities.CrmUser()
				{
					AccountId = itemId,
					CrmUserGuid = crmId,
					BusinessUnitId = businessUnitId,
					CalType = CALType,
					ChangedDate = now,
					CreatedDate = now
				};
				CrmUsers.Add(user);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure, "InsertCRMUser",
					new SqlParameter[] {
						/// <summary>TODO</summary>
						new SqlParameter("@ItemID", itemId),
						/// <summary>TODO</summary>
						new SqlParameter("@CrmUserID", crmId),
						/// <summary>TODO</summary>
						new SqlParameter("@BusinessUnitId", businessUnitId),
						/// <summary>TODO</summary>
						new SqlParameter("@CALType", CALType)
					});

			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateCRMUser(int itemId, int CALType)
		{
			if (UseEntityFramework)
			{
                CrmUsers.Where(u => u.AccountId == itemId)
                    .ExecuteUpdate(cu => new Data.Entities.CrmUser() { CalType = CALType });
            }
            else
			{
				SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure, "UpdateCRMUser",
					new SqlParameter[] {
						/// <summary>TODO</summary>
						new SqlParameter("@ItemID", itemId),
						/// <summary>TODO</summary>
						new SqlParameter("@CALType", CALType)
					});
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetCrmUser(int accountId)
		{
			if (UseEntityFramework)
			{
				var user = CrmUsers
					.Where(u => u.AccountId == accountId)
					.Select(u => new
					{
						CrmUserId = u.CrmUserGuid,
						u.BusinessUnitId
					});
				/// <summary>TODO</summary>
				return EntityDataReader(user);
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure, "GetCRMUser",
					new SqlParameter[] { new SqlParameter("@AccountID", accountId) });
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetCrmUserCount(int itemId)
		{
			if (UseEntityFramework)
			{
				return ExchangeAccounts
					.Where(a => a.ItemId == itemId)
					.SelectMany(a => a.CrmUsers)
					.Count();
			}
			else
			{
				return (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "GetOrganizationCRMUserCount",
					new SqlParameter[] { new SqlParameter("@ItemID", itemId) });
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteCrmOrganization(int organizationId)
		{
			if (UseEntityFramework)
			{
				CrmUsers.Where(u => u.Account.ItemId == organizationId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "DeleteCRMOrganization",
					new SqlParameter[] { new SqlParameter("@ItemID", organizationId) });
			}
		}
		#endregion

		#region VPS - Virtual Private Servers
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetVirtualMachinesPaged(int actorId, int packageId, string filterColumn, string filterValue,
			string sortColumn, int startRow, int maximumRows, bool recursive)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var externalIpAddresses = PackageIpAddresses
					.Where(pip => pip.IsPrimary == true)
					.Join(IpAddresses.Where(ip => ip.PoolId == 3), pip => pip.AddressId, ip => ip.AddressId, (pip, ip) => new
					{
						pip.ItemId,
						ip.ExternalIp
					});
				/// <summary>TODO</summary>
				IQueryable<Data.Entities.Package> packages;
				TempIdSet childPackages = null;
				try
				{
					if (!recursive) packages = Packages.Where(p => p.PackageId == packageId);
					else
					{
						childPackages = PackagesTree(packageId, true);
						packages = Packages.Join(childPackages, p => p.PackageId, ch => ch, (p, ch) => p);
					}

					var items = packages
						.Join(ServiceItems.Where(si => si.ItemTypeId == 33 /* VPS */),
							p => p.PackageId, i => i.PackageId, (p, i) => new { Package = p, Item = i })
						.GroupJoin(PrivateIpAddresses.Where(ip => ip.IsPrimary), p => p.Item.ItemId, pip => pip.ItemId, (p, pip) => new
						{
							p.Package,
							p.Item,
							IpAddresses = pip
							//pip.Any() ? pip.Single().IpAddress : null
						})
						.SelectMany(p => p.IpAddresses.DefaultIfEmpty(), (p, pip) => new
						{
							p.Package,
							p.Item,
							IpAddress = pip != null ? pip.IpAddress : null
						})
						.GroupJoin(externalIpAddresses, p => p.Item.ItemId, eip => eip.ItemId, (p, eip) => new
						{
							p.Package,
							p.Item,
							p.IpAddress,
							ExternalIps = eip
						})
						.SelectMany(p => p.ExternalIps.DefaultIfEmpty(), (p, eip) => new
						{
							p.Item.ItemId,
							p.Item.ItemName,
							p.Item.PackageId,
							p.Package.PackageName,
							p.Package.UserId,
							p.Package.User.Username,
							p.IpAddress,
							ExternalIp = eip != null ? eip.ExternalIp : null
						});

					if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
					{
						items = items.Where(DynamicFunctions.ColumnLike(items, filterColumn, filterValue));
					}
					else if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
							items = items.Where(i => DbFunctions.Like(i.ItemName, filterValue) ||
								DbFunctions.Like(i.Username, filterValue) || DbFunctions.Like(i.ExternalIp, filterValue) ||
								DbFunctions.Like(i.IpAddress, filterValue));
#else
							items = items.Where(i => EF.Functions.Like(i.ItemName, filterValue) ||
								EF.Functions.Like(i.Username, filterValue) || EF.Functions.Like(i.ExternalIp, filterValue) ||
								EF.Functions.Like(i.IpAddress, filterValue));
#endif
					}

					var count = items.Count();

					items = !string.IsNullOrEmpty(sortColumn) ? items.OrderBy(ColumnName(sortColumn)) : items.OrderBy(i => i.ItemName);


					items = items.Skip(startRow).Take(maximumRows);

					/// <summary>TODO</summary>
					return EntityDataReader(count, items);
				}
				finally
				{
					childPackages?.Dispose();
				}
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetVirtualMachinesPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@MaximumRows", maximumRows),
					/// <summary>TODO</summary>
					new SqlParameter("@Recursive", recursive));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetVirtualMachinesPaged2012(int actorId, int packageId, string filterColumn, string filterValue,
			string sortColumn, int startRow, int maximumRows, bool recursive)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var externalIpAddresses = PackageIpAddresses
					.Where(pip => pip.IsPrimary == true)
					.Join(IpAddresses.Where(ip => ip.PoolId == 3), pip => pip.AddressId, ip => ip.AddressId, (pip, ip) => new
					{
						pip.ItemId,
						ip.ExternalIp
					});
				/// <summary>TODO</summary>
				IQueryable<Data.Entities.Package> packages;
				TempIdSet childPackages = null;
				try
				{
					if (!recursive) packages = Packages.Where(p => p.PackageId == packageId);
					else
					{
						childPackages = PackagesTree(packageId, true);
						packages = Packages.Join(childPackages, p => p.PackageId, ch => ch, (p, ch) => p);
					}
					var items = packages
						.Join(ServiceItems.Where(si => si.ItemTypeId == 41 /* VPS2012 */),
							p => p.PackageId, i => i.PackageId, (p, i) => new { Package = p, Item = i })
						.GroupJoin(PrivateIpAddresses.Where(ip => ip.IsPrimary), p => p.Item.ItemId, pip => pip.ItemId, (p, pip) => new
						{
							p.Package,
							p.Item,
							IpAddresses = pip
						})
						.SelectMany(p => p.IpAddresses.DefaultIfEmpty(), (p, pip) => new
						{
							p.Package,
							p.Item,
							IpAddress = pip != null ? pip.IpAddress : null
						})
						.GroupJoin(DmzIpAddresses.Where(ip => ip.IsPrimary), p => p.Item.ItemId, dip => dip.ItemId, (p, dip) => new
						{
							p.Package,
							p.Item,
							p.IpAddress,
							DmzIps = dip
						})
						.SelectMany(p => p.DmzIps.DefaultIfEmpty(), (p, dip) => new
						{
							p.Package,
							p.Item,
							p.IpAddress,
							DmzIp = dip
						})
						.GroupJoin(externalIpAddresses, p => p.Item.ItemId, eip => eip.ItemId, (p, eip) => new
						{
							p.Package,
							p.Item,
							p.IpAddress,
							p.DmzIp,
							ExternalIps = eip
						})
						.SelectMany(p => p.ExternalIps.DefaultIfEmpty(), (p, eip) => new
						{
							p.Item.ItemId,
							p.Item.ItemName,
							p.Item.PackageId,
							p.Package.PackageName,
							p.Package.UserId,
							p.Package.User.Username,
							ExternalIp = eip != null ? eip.ExternalIp : null,
							p.IpAddress,
							p.DmzIp
						});

					if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
					{
						items = items.Where(DynamicFunctions.ColumnLike(items, filterColumn, filterValue));
					}
					else if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
							items = items.Where(i => DbFunctions.Like(i.ItemName, filterValue) ||
								DbFunctions.Like(i.Username, filterValue) || DbFunctions.Like(i.ExternalIp, filterValue) ||
								DbFunctions.Like(i.IpAddress, filterValue));
#else
							items = items.Where(i => EF.Functions.Like(i.ItemName, filterValue) ||
								EF.Functions.Like(i.Username, filterValue) || EF.Functions.Like(i.ExternalIp, filterValue) ||
								EF.Functions.Like(i.IpAddress, filterValue));
#endif
					}

					var count = items.Count();

					items = !string.IsNullOrEmpty(sortColumn) ? items.OrderBy(ColumnName(sortColumn)) : items.OrderBy(i => i.ItemName);


					items = items.Skip(startRow).Take(maximumRows);

					/// <summary>TODO</summary>
					return EntityDataReader(count, items);
				}
				finally
				{
					childPackages?.Dispose();
				}
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetVirtualMachinesPaged2012",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@MaximumRows", maximumRows),
					/// <summary>TODO</summary>
					new SqlParameter("@Recursive", recursive));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetVirtualMachinesPagedProxmox(int actorId, int packageId, string filterColumn, string filterValue,
			 string sortColumn, int startRow, int maximumRows, bool recursive)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var externalIpAddresses = PackageIpAddresses
					.Where(pip => pip.IsPrimary == true)
					.Join(IpAddresses.Where(ip => ip.PoolId == 3), pip => pip.AddressId, ip => ip.AddressId, (pip, ip) => new
					{
						pip.ItemId,
						ip.ExternalIp
					});
				/// <summary>TODO</summary>
				IQueryable<Data.Entities.Package> packages;
				TempIdSet childPackages = null;
				try
				{
					if (!recursive) packages = Packages.Where(p => p.PackageId == packageId);
					else
					{
						childPackages = PackagesTree(packageId, true);
						packages = Packages.Join(childPackages, p => p.PackageId, ch => ch, (p, ch) => p);
					}
					var items = packages
						.Join(ServiceItems.Where(si => si.ItemTypeId == 143 /* Proxmox */),
							p => p.PackageId, i => i.PackageId, (p, i) => new { Package = p, Item = i })
						.GroupJoin(PrivateIpAddresses.Where(ip => ip.IsPrimary), p => p.Item.ItemId, pip => pip.ItemId, (p, pip) => new
						{
							p.Package,
							p.Item,
							IpAdresses = pip
						})
						.SelectMany(p => p.IpAdresses.DefaultIfEmpty(), (p, pip) => new
						{
							p.Package,
							p.Item,
							IpAddress = pip != null ? pip.IpAddress : null
						})
						.GroupJoin(externalIpAddresses, p => p.Item.ItemId, eip => eip.ItemId, (p, eip) => new
						{
							p.Package,
							p.Item,
							p.IpAddress,
							ExternalIps = eip
						})
						.SelectMany(p => p.ExternalIps.DefaultIfEmpty(), (p, eip) => new
						{
							p.Item.ItemId,
							p.Item.ItemName,
							p.Item.PackageId,
							p.Package.PackageName,
							p.Package.UserId,
							p.Package.User.Username,
							ExternalIp = eip != null ? eip.ExternalIp : null,
							p.IpAddress
						});

					if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
					{
						items = items.Where(DynamicFunctions.ColumnLike(items, filterColumn, filterValue));
					}
					else if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
							items = items.Where(i => DbFunctions.Like(i.ItemName, filterValue) ||
								DbFunctions.Like(i.Username, filterValue) || DbFunctions.Like(i.ExternalIp, filterValue) ||
								DbFunctions.Like(i.IpAddress, filterValue));
#else
							items = items.Where(i => EF.Functions.Like(i.ItemName, filterValue) ||
								EF.Functions.Like(i.Username, filterValue) || EF.Functions.Like(i.ExternalIp, filterValue) ||
								EF.Functions.Like(i.IpAddress, filterValue));
#endif
					}

					var count = items.Count();

					items = !string.IsNullOrEmpty(sortColumn) ? items.OrderBy(ColumnName(sortColumn)) : items.OrderBy(i => i.ItemName);


					items = items.Skip(startRow).Take(maximumRows);

					/// <summary>TODO</summary>
					return EntityDataReader(count, items);
				}
				finally
				{
					childPackages?.Dispose();
				}
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetVirtualMachinesPagedProxmox",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@MaximumRows", maximumRows),
					/// <summary>TODO</summary>
					new SqlParameter("@Recursive", recursive));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetVirtualMachinesForPCPaged(int actorId, int packageId, string filterColumn, string filterValue,
			 string sortColumn, int startRow, int maximumRows, bool recursive)
		{
			if (UseEntityFramework)
			{
				// check rights
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var externalIpAddresses = PackageIpAddresses
					.Where(pip => pip.IsPrimary == true)
					.Join(IpAddresses.Where(ip => ip.PoolId == 3), pip => pip.AddressId, ip => ip.AddressId, (pip, ip) => new
					{
						pip.ItemId,
						ip.ExternalIp
					});
				/// <summary>TODO</summary>
				IQueryable<Data.Entities.Package> packages;
				TempIdSet childPackages = null;
				try
				{
					if (!recursive) packages = Packages.Where(p => p.PackageId == packageId);
					else
					{
						childPackages = PackagesTree(packageId, true);
						packages = Packages.Join(childPackages, p => p.PackageId, ch => ch, (p, ch) => p);
					}

					var items = packages
						.Join(ServiceItems.Where(si => si.ItemTypeId == 35 /* VPS for PC */),
							p => p.PackageId, i => i.PackageId, (p, i) => new { Package = p, Item = i })
						.GroupJoin(PrivateIpAddresses.Where(ip => ip.IsPrimary), p => p.Item.ItemId, pip => pip.ItemId, (p, pip) => new
						{
							p.Package,
							p.Item,
							IpAddresses = pip
							//IpAddress = pip.Any() ? pip.Single().IpAddress : null
						})
						.SelectMany(p => p.IpAddresses.DefaultIfEmpty(), (p, pip) => new
						{
							p.Package,
							p.Item,
							IpAddress = pip != null ? pip.IpAddress : null
						})
						.GroupJoin(externalIpAddresses, p => p.Item.ItemId, eip => eip.ItemId, (p, eip) => new
						{
							p.Package,
							p.Item,
							p.IpAddress,
							ExternalIps = eip
						})
						.SelectMany(p => p.ExternalIps.DefaultIfEmpty(), (p, eip) => new
						{
							p.Item.ItemId,
							p.Item.ItemName,
							p.Item.PackageId,
							p.Package.PackageName,
							p.Package.UserId,
							p.Package.User.Username,
							ExternalIp = eip != null ? eip.ExternalIp : null,
							p.IpAddress
						});

					if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
					{
						items = items.Where(DynamicFunctions.ColumnLike(items, filterColumn, filterValue));
					}
					else if (!string.IsNullOrEmpty(filterValue))
					{
#if NETFRAMEWORK
							items = items.Where(i => DbFunctions.Like(i.ItemName, filterValue) ||
								DbFunctions.Like(i.Username, filterValue) || DbFunctions.Like(i.ExternalIp, filterValue) ||
								DbFunctions.Like(i.IpAddress, filterValue));
#else
							items = items.Where(i => EF.Functions.Like(i.ItemName, filterValue) ||
								EF.Functions.Like(i.Username, filterValue) || EF.Functions.Like(i.ExternalIp, filterValue) ||
								EF.Functions.Like(i.IpAddress, filterValue));
#endif
					}

					var count = items.Count();

					items = !string.IsNullOrEmpty(sortColumn) ? items.OrderBy(ColumnName(sortColumn)) : items.OrderBy(i => i.ItemName);


					items = items.Skip(startRow).Take(maximumRows);

					/// <summary>TODO</summary>
					return EntityDataReader(count, items);
				}
				finally
				{
					childPackages?.Dispose();
				}
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetVirtualMachinesPagedForPC",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@MaximumRows", maximumRows),
					/// <summary>TODO</summary>
					new SqlParameter("@Recursive", recursive));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		#endregion

		#region VPS - External Network
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetUnallottedIPAddresses(int packageId, int serviceId, int poolId)
		{
			if (UseEntityFramework)
			{
				/// <summary>TODO</summary>
				int? serverId, parentPackageId;

				if (packageId == -1) // NO PackageID defined, use ServerID from ServiceID (VPS Import)
				{
					serverId = Services
						.Where(s => s.ServiceId == serviceId)
						.Select(s => s.ServerId)
						.FirstOrDefault();
					parentPackageId = 1;
				}
				else
				{
					var package = Packages
						.Where(p => p.PackageId == packageId)
						.Select(p => new { p.ParentPackageId, p.ServerId })
						.FirstOrDefault();
					serverId = package.ServerId;
					parentPackageId = package.ParentPackageId;
				}

				if (parentPackageId == 1 || poolId == 4 /* management network */) // "System" space
				{
					// check if server is physical
					if (Servers.Any(s => s.ServerId == serverId && !s.VirtualServer))
					{
						// physical server
						var packageIps = PackageIpAddresses
							.Select(pip => pip.AddressId);
						var addresses = IpAddresses
							.Where(ip => (ip.ServerId == serverId || ip.ServerId == null) &&
								(poolId == 0 || ip.PoolId == poolId) &&
								!packageIps.Any(pip => pip == ip.AddressId))
							.OrderByDescending(ip => ip.ServerId)
							.ThenBy(ip => ip.DefaultGateway)
							.ThenBy(ip => ip.ExternalIp)
							.Select(ip => new
							{
								ip.AddressId,
								ip.ExternalIp,
								ip.InternalIp,
								ip.ServerId,
								ip.PoolId,
								ip.SubnetMask,
								ip.DefaultGateway,
								ip.Vlan
							});
						/// <summary>TODO</summary>
						return EntityDataReader(addresses);
					}
					else
					{
						// virtual server
						// get resource group by service
                        var groupId = Services
                            .Where(s => s.ServiceId == serviceId)
                            .Select(s => s.Provider.GroupId)
                            .FirstOrDefault();
                        var serverIds = Services
                            .Where(s => s.ServiceId == serviceId && s.Provider.GroupId == groupId)
                            .Select(s => s.ServerId);
                        var packageIps = PackageIpAddresses
                            .Select(pip => pip.AddressId);
                        var addresses = IpAddresses
                            .Where(ip => (ip.ServerId == null || serverIds.Any(si => ip.ServerId == si)) &&
                                (poolId == 0 || ip.PoolId == poolId) &&
                                !packageIps.Any(pip => pip == ip.AddressId))
                            .OrderByDescending(ip => ip.ServerId)
                            .ThenBy(ip => ip.DefaultGateway)
                            .ThenBy(ip => ip.ExternalIp)
                            .Select(ip => new
							{
								ip.AddressId,
								ip.ExternalIp,
								ip.InternalIp,
								ip.ServerId,
								ip.PoolId,
								ip.SubnetMask,
								ip.DefaultGateway,
								ip.Vlan
							});
						/// <summary>TODO</summary>
						return EntityDataReader(addresses);
					}
				}
				else
				{
					// 2rd level space and below
					// get service location
					serverId = Services
						.Where(s => s.ServiceId == serviceId)
						.Select(s => s.ServerId)
						.FirstOrDefault();
					var addresses = PackageIpAddresses
						.Where(pip => pip.PackageId == parentPackageId && pip.ItemId == null)
						.Join(IpAddresses, pip => pip.AddressId, ip => ip.AddressId, (pip, ip) => ip)
						.Where(ip => (poolId == 0 || ip.PoolId == poolId) &&
							(ip.ServerId == serverId || ip.ServerId == null))
						.OrderByDescending(ip => ip.ServerId)
						.ThenBy(ip => ip.DefaultGateway)
						.ThenBy(ip => ip.ExternalIp)
						.Select(ip => new
						{
							ip.AddressId,
							ip.ExternalIp,
							ip.InternalIp,
							ip.ServerId,
							ip.PoolId,
							ip.SubnetMask,
							ip.DefaultGateway,
							ip.Vlan
						});
					/// <summary>TODO</summary>
					return EntityDataReader(addresses);
				}
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetUnallottedIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceId", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@PoolId", poolId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AllocatePackageIPAddresses(int packageId, int orgId, string xml)
		{
			if (UseEntityFramework)
			{
				var addressIds = XElement.Parse(xml)
					.Elements()
					.Select(e => (int)e.Attribute("id"))
					.ToList();
				using (var addressIdsIdSet = addressIds.ToTempIdSet(this))
				{
					PackageIpAddresses
						.Join(addressIdsIdSet, ip => ip.AddressId, id => id, (ip, id) => ip)
						.ExecuteDelete();
				}
				var newIps = addressIds
					.Select(id => new Data.Entities.PackageIpAddress
					{
						PackageId = packageId,
						OrgId = orgId,
						AddressId = id
					});
				PackageIpAddresses.AddRange(newIps);
				SaveChanges();
			}
			else
			{
				SqlParameter[] param = new[] {
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@OrgID", orgId),
					/// <summary>TODO</summary>
					new SqlParameter("@xml", xml)
				};

				ExecuteLongNonQuery("AllocatePackageIPAddresses", param);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackageIPAddresses(int packageId, int orgId, int poolId, string filterColumn, string filterValue,
			 string sortColumn, int startRow, int maximumRows, bool recursive)
		{
			if (UseEntityFramework)
			{
				/// <summary>TODO</summary>
				IQueryable<Data.Entities.PackageIpAddress> addressesFiltered;
				TempIdSet childPackages = null;
				try
				{
					if (!recursive) addressesFiltered = PackageIpAddresses.Where(pa => pa.PackageId == packageId);
					else
					{
						childPackages = PackagesTree(packageId, true);
						addressesFiltered = PackageIpAddresses.Join(childPackages, pa => pa.PackageId, ch => ch, (pa, ch) => pa);
					}

					var addresses = addressesFiltered
						.Where(pa => (orgId == 0 || pa.OrgId == orgId) &&
							(poolId == 0 || pa.Address.PoolId == poolId))
						.Select(pa => new
						{
							pa.PackageAddressId,
							pa.AddressId,
							pa.Address.ExternalIp,
							pa.Address.InternalIp,
							pa.Address.SubnetMask,
							pa.Address.DefaultGateway,
							pa.Address.Vlan,
							pa.ItemId,
                            ItemName = pa.Item != null ? pa.Item.ItemName : null,
                            pa.PackageId,
							pa.Package.PackageName,
							pa.Package.UserId,
							pa.Package.User.Username,
							pa.IsPrimary
						});
					if (!string.IsNullOrEmpty(filterValue))
					{
						if (!string.IsNullOrEmpty(filterColumn))
						{
							addresses = addresses.Where(DynamicFunctions.ColumnLike(addresses, filterColumn, filterValue));
						}
						else
						{
							addresses = addresses
#if NETFRAMEWORK
								.Where(a => DbFunctions.Like(a.ExternalIp, filterValue) ||
									DbFunctions.Like(a.InternalIp, filterValue) ||
									DbFunctions.Like(a.DefaultGateway, filterValue) ||
									DbFunctions.Like(a.ItemName, filterValue) ||
									DbFunctions.Like(a.Username, filterValue));
#else
								.Where(a => EF.Functions.Like(a.ExternalIp, filterValue) ||
									EF.Functions.Like(a.InternalIp, filterValue) ||
									EF.Functions.Like(a.DefaultGateway, filterValue) ||
									EF.Functions.Like(a.ItemName, filterValue) ||
									EF.Functions.Like(a.Username, filterValue));
#endif
						}
					}

					var count = addresses.Count();

					addresses = string.IsNullOrEmpty(sortColumn) ? addresses.OrderBy(a => a.ExternalIp) : addresses.OrderBy(ColumnName(sortColumn));


					addresses = addresses.Skip(startRow).Take(maximumRows);
					/// <summary>TODO</summary>
					return EntityDataReader(count, addresses);
				}
				finally
				{
					childPackages?.Dispose();
				}
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetPackageIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@OrgID", orgId),
					/// <summary>TODO</summary>
					new SqlParameter("@PoolId", poolId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows),
					/// <summary>TODO</summary>
					new SqlParameter("@Recursive", recursive));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetPackageIPAddressesCount(int packageId, int orgId, int poolId)
		{
			if (UseEntityFramework)
			{
                return PackageIpAddresses
                    .Join(IpAddresses, pip => pip.AddressId, ip => ip.AddressId, (pip, ip) => new
                    {
                        pip.OrgId,
                        pip.PackageId,
                        ip.PoolId
                    })
                    .Join(Packages, pip => pip.PackageId, p => p.PackageId, (pip, p) => new
                    {
                        PIP = pip,
                        UserId = p.UserId
                    })
                    .Join(Users, pip => pip.UserId, u => u.UserId, (pip, u) => new
                    {
                        pip.PIP.OrgId,
                        pip.PIP.PoolId
                    })
                    .Count(ip => (poolId == 0 || poolId == ip.PoolId) &&
                        (orgId == 0 || orgId == ip.OrgId));
            }
            else
			{
				object obj = SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure,
					"GetPackageIPAddressesCount",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@OrgID", orgId),
					/// <summary>TODO</summary>
					new SqlParameter("@PoolId", poolId));
				int res = 0;
				int.TryParse(obj.ToString(), out res);
				/// <summary>TODO</summary>
				return res;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeallocatePackageIPAddress(int id)
		{
			if (UseEntityFramework)
			{
				var parentPackageId = PackageIpAddresses
					.Where(pa => pa.PackageAddressId == id)
					.Select(pa => pa.Package.ParentPackageId)
					.FirstOrDefault();
				if (!parentPackageId.HasValue)
					return;

				if (parentPackageId == 1) // System space
				{
					PackageIpAddresses
						.Where(pa => pa.PackageAddressId == id)
						.ExecuteDelete();
				}
				else // 2rd level space and below
				{
                    PackageIpAddresses
                        .Where(pa => pa.PackageAddressId == id)
                        .ExecuteUpdate(pip => new Data.Entities.PackageIpAddress()
                        {
                            PackageId = parentPackageId.Value
                        });
                }
            }
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure, "DeallocatePackageIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageAddressID", id));
			}
		}
		#endregion

		#region VPS - Private Network
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackagePrivateIPAddressesPaged(int packageId, string filterColumn, string filterValue,
			string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var addresses = PrivateIpAddresses
					.Where(a => a.Item.PackageId == packageId)
					.Select(a => new
					{
						a.PrivateAddressId,
						a.IpAddress,
						a.ItemId,
						a.Item.ItemName,
						a.IsPrimary
					});

				if (!string.IsNullOrEmpty(filterValue))
				{
					if (!string.IsNullOrEmpty(filterColumn))
					{
						addresses = addresses.Where(DynamicFunctions.ColumnLike(addresses, filterColumn, filterValue));
					}
					else
					{
						addresses = addresses
#if NETFRAMEWORK
							.Where(a => DbFunctions.Like(a.IpAddress, filterValue) ||
								DbFunctions.Like(a.ItemName, filterValue));
#else
							.Where(a => EF.Functions.Like(a.IpAddress, filterValue) ||
								EF.Functions.Like(a.ItemName, filterValue));
#endif
					}
				}

				var count = addresses.Count();

				addresses = string.IsNullOrEmpty(sortColumn) ? addresses.OrderBy(pa => pa.IpAddress) : addresses.OrderBy(ColumnName(sortColumn));


				addresses = addresses.Skip(startRow).Take(maximumRows);
				/// <summary>TODO</summary>
				return EntityDataReader(count, addresses);
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetPackagePrivateIPAddressesPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackagePrivateIPAddresses(int packageId)
		{
			if (UseEntityFramework)
			{
				var addresses = PrivateIpAddresses
					.Where(a => a.Item.PackageId == packageId)
					.Select(a => new
					{
						a.PrivateAddressId,
						a.IpAddress,
						a.ItemId,
						a.Item.ItemName,
						a.IsPrimary
					});
				/// <summary>TODO</summary>
				return EntityDataReader(addresses);
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetPackagePrivateIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		#endregion

		#region VPS - DMZ Network
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackageDmzIPAddressesPaged(int packageId, string filterColumn, string filterValue,
			string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var addresses = DmzIpAddresses
					.Where(a => a.Item.PackageId == packageId)
					.Select(a => new
					{
						a.DmzAddressId,
						a.IpAddress,
						a.ItemId,
						a.Item.ItemName,
						a.IsPrimary
					});

				if (!string.IsNullOrEmpty(filterValue))
				{
					if (!string.IsNullOrEmpty(filterColumn))
					{
						addresses = addresses.Where(DynamicFunctions.ColumnLike(addresses, filterColumn, filterValue));
					}
					else
					{
						addresses = addresses
#if NETFRAMEWORK
							.Where(a => DbFunctions.Like(a.IpAddress, filterValue) ||
								DbFunctions.Like(a.ItemName, filterValue));
#else
							.Where(a => EF.Functions.Like(a.IpAddress, filterValue) ||
								EF.Functions.Like(a.ItemName, filterValue));
#endif
					}
				}

				var count = addresses.Count();

				addresses = string.IsNullOrEmpty(sortColumn) ? addresses.OrderBy(pa => pa.IpAddress) : addresses.OrderBy(ColumnName(sortColumn));


				addresses = addresses.Skip(startRow).Take(maximumRows);
				/// <summary>TODO</summary>
				return EntityDataReader(count, addresses);
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
										 "GetPackageDmzIPAddressesPaged",
											/// <summary>TODO</summary>
											new SqlParameter("@PackageID", packageId),
											/// <summary>TODO</summary>
											new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
											/// <summary>TODO</summary>
											new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
											/// <summary>TODO</summary>
											new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
											/// <summary>TODO</summary>
											new SqlParameter("@startRow", startRow),
											/// <summary>TODO</summary>
											new SqlParameter("@maximumRows", maximumRows));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackageDmzIPAddresses(int packageId)
		{
			if (UseEntityFramework)
			{
				var addresses = DmzIpAddresses
					.Where(a => a.Item.PackageId == packageId)
					.Select(a => new
					{
						a.DmzAddressId,
						a.IpAddress,
						a.ItemId,
						a.Item.ItemName,
						a.IsPrimary
					});
				/// <summary>TODO</summary>
				return EntityDataReader(addresses);
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
									 "GetPackageDmzIPAddresses",
										/// <summary>TODO</summary>
										new SqlParameter("@PackageID", packageId));
				/// <summary>TODO</summary>
				return reader;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddItemDmzIPAddress(int actorId, int itemId, string ipAddress)
		{
			if (UseEntityFramework)
			{
				if (ServiceItems
					//TODO .Where(i => i.ItemId == itemId) or not?
					.Where(i => i.ItemId == itemId) // bugfix added by Simon Egli, 27.6.2024 
					.Select(i => i.PackageId)
					.ToList()
					.Any(packageId => CheckActorPackageRights(actorId, packageId)))
				{
					var ip = new Data.Entities.DmzIpAddress()
					{
						ItemId = itemId,
						IpAddress = ipAddress,
						IsPrimary = false
					};
					DmzIpAddresses.Add(ip);
					/// <summary>TODO</summary>
					return SaveChanges();
				}
				return 0;
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
								"AddItemDmzIPAddress",
								/// <summary>TODO</summary>
								new SqlParameter("@ActorID", actorId),
								/// <summary>TODO</summary>
								new SqlParameter("@ItemID", itemId),
								/// <summary>TODO</summary>
								new SqlParameter("@IPAddress", ipAddress));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int SetItemDmzPrimaryIPAddress(int actorId, int itemId, int dmzAddressId)
		{
			if (UseEntityFramework)
			{
				var addresses = DmzIpAddresses
					.Where(ip => ip.ItemId == itemId)
					.Include(ip => ip.Item)
					.AsEnumerable()
					.Where(ip => Clone.CheckActorPackageRights(actorId, ip.Item.PackageId));

				foreach (var ip in addresses) ip.IsPrimary = ip.DmzAddressId == dmzAddressId;

				/// <summary>TODO</summary>
				return SaveChanges();
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
								"SetItemDmzPrimaryIPAddress",
								/// <summary>TODO</summary>
								new SqlParameter("@ActorID", actorId),
								/// <summary>TODO</summary>
								new SqlParameter("@ItemID", itemId),
								/// <summary>TODO</summary>
								new SqlParameter("@DmzAddressID", dmzAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteItemDmzIPAddress(int actorId, int itemId, int dmzAddressId)
		{
			if (UseEntityFramework)
			{
				var addresses = DmzIpAddresses
					.Where(ip => ip.DmzAddressId == dmzAddressId)
					.Include(ip => ip.Item)
					.AsEnumerable()
					.Where(ip => Clone.CheckActorPackageRights(actorId, ip.Item.PackageId));

				DmzIpAddresses.RemoveRange(addresses);

				/// <summary>TODO</summary>
				return SaveChanges();

			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
									"DeleteItemDmzIPAddress",
									/// <summary>TODO</summary>
									new SqlParameter("@ActorID", actorId),
									/// <summary>TODO</summary>
									new SqlParameter("@ItemID", itemId),
									/// <summary>TODO</summary>
									new SqlParameter("@DmzAddressID", dmzAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetItemDmzIPAddresses(int actorId, int itemId)
		{
			if (UseEntityFramework)
			{
				var addresses = DmzIpAddresses
					.Where(ip => ip.ItemId == itemId)
					.Select(ip => new
					{
						AddressId = ip.DmzAddressId,
						ip.IpAddress,
						ip.IsPrimary,
						ip.Item.PackageId
					})
					.OrderBy(ip => ip.IsPrimary)
					.AsEnumerable()
					.Where(ip => Clone.CheckActorPackageRights(actorId, ip.PackageId))
					.Select(ip => new
					{
						ip.AddressId,
						ip.IpAddress,
						ip.IsPrimary
					});

				/// <summary>TODO</summary>
				return EntityDataReader(addresses);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
									"GetItemDmzIPAddresses",
									/// <summary>TODO</summary>
									new SqlParameter("@ActorID", actorId),
									/// <summary>TODO</summary>
									new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteItemDmzIPAddresses(int actorId, int itemId)
		{
			if (UseEntityFramework)
			{
				var addresses = DmzIpAddresses
					.Where(ip => ip.ItemId == itemId)
					.Include(ip => ip.Item)
					.AsEnumerable()
					.Where(ip => Clone.CheckActorPackageRights(actorId, ip.Item.PackageId));

				DmzIpAddresses.RemoveRange(addresses);

				/// <summary>TODO</summary>
				return SaveChanges();

			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
								"DeleteItemDmzIPAddresses",
								/// <summary>TODO</summary>
								new SqlParameter("@ActorID", actorId),
								/// <summary>TODO</summary>
								new SqlParameter("@ItemID", itemId));
			}
		}
		#endregion

		#region VPS - External Network Adapter
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetPackageUnassignedIPAddresses(int actorId, int packageId, int orgId, int poolId)
		{
			if (UseEntityFramework)
			{
				var addresses = PackageIpAddresses
					.Include(a => a.Address)
					.Where(a => a.ItemId == null && a.PackageId == packageId &&
						(orgId == 0 || a.OrgId == orgId) &&
						(poolId == 0 || a.Address.PoolId == poolId))
					.OrderBy(a => a.Address.DefaultGateway)
					.ThenBy(a => a.Address.ExternalIp)
					.AsEnumerable()
					.Where(a => Clone.CheckActorPackageRights(actorId, a.PackageId))
					.Select(a => new
					{
						a.PackageAddressId,
						a.Address.AddressId,
						a.Address.ExternalIp,
						a.Address.InternalIp,
						a.Address.ServerId,
						a.Address.PoolId,
						a.IsPrimary,
						a.Address.SubnetMask,
						a.Address.DefaultGateway,
						a.Address.Vlan
					});
				/// <summary>TODO</summary>
				return EntityDataReader(addresses);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetPackageUnassignedIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@OrgID", orgId),
					/// <summary>TODO</summary>
					new SqlParameter("@PoolId", poolId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetPackageIPAddress(int packageAddressId)
		{
			if (UseEntityFramework)
			{
				var address = PackageIpAddresses
					.Where(a => a.PackageAddressId == packageAddressId)
					.Select(a => new
					{
						a.PackageAddressId,
						a.AddressId,
						a.Address.ExternalIp,
						a.Address.InternalIp,
						a.Address.SubnetMask,
						a.Address.DefaultGateway,
						a.Address.Vlan,
						a.ItemId,
                        ItemName = a.Item != null ? a.Item.ItemName : null,
                        a.PackageId,
						a.Package.PackageName,
						a.Package.UserId,
						a.Package.User.Username,
						a.IsPrimary
					});
				/// <summary>TODO</summary>
				return EntityDataReader(address);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetPackageIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@PackageAddressId", packageAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetItemIPAddresses(int actorId, int itemId, int poolId)
		{
			if (UseEntityFramework)
			{
				var addresses = PackageIpAddresses
					.Where(a => a.ItemId == itemId && (poolId == 0 || a.Address.PoolId == poolId))
					.Select(a => new
					{
						AddressId = a.PackageAddressId,
						IpAddress = a.Address.ExternalIp,
						NatAddress = a.Address.InternalIp,
						a.Address.SubnetMask,
						a.Address.DefaultGateway,
						a.IsPrimary,
                        a.Item.PackageId
                    })
					.OrderByDescending(a => a.IsPrimary)
					.AsEnumerable()
					.Where(a => Clone.CheckActorPackageRights(actorId, a.PackageId));
				/// <summary>TODO</summary>
				return EntityDataReader(addresses);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetItemIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@PoolID", poolId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddItemIPAddress(int actorId, int itemId, int packageAddressId)
		{
			if (UseEntityFramework)
			{
				var address = PackageIpAddresses
					.Where(a => a.PackageAddressId == packageAddressId)
					.AsEnumerable()
					.Where(a => Clone.CheckActorPackageRights(actorId, a.PackageId))
					.FirstOrDefault();
				if (address != null)
				{
					address.ItemId = itemId;
					address.IsPrimary = false;
					/// <summary>TODO</summary>
					return SaveChanges();
				}
				return 0;
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					"AddItemIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageAddressID", packageAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int SetItemPrimaryIPAddress(int actorId, int itemId, int packageAddressId)
		{
			if (UseEntityFramework)
			{
				// read item pool
				var poolId = PackageIpAddresses
					.Where(a => a.PackageAddressId == packageAddressId)
					.Select(a => a.Address.PoolId)
					.FirstOrDefault();
				// update all IP addresses of the specified pool
				foreach (var ip in PackageIpAddresses
					.Where(a => a.ItemId == itemId && a.Address.PoolId == poolId))
				{
					if (CheckActorPackageRights(actorId, ip.PackageId))
					{
						ip.IsPrimary = ip.PackageAddressId == packageAddressId;
					}
				}
				/// <summary>TODO</summary>
				return SaveChanges();
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					"SetItemPrimaryIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageAddressID", packageAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteItemIPAddress(int actorId, int itemId, int packageAddressId)
		{
			if (UseEntityFramework)
			{
				foreach (var ip in PackageIpAddresses
					.Where(a => a.PackageAddressId == packageAddressId))
				{
					if (CheckActorPackageRights(actorId, ip.PackageId))
					{
						ip.ItemId = null;
						ip.IsPrimary = false;
					}
				}
				/// <summary>TODO</summary>
				return SaveChanges();
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					"DeleteItemIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageAddressID", packageAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteItemIPAddresses(int actorId, int itemId)
		{
			if (UseEntityFramework)
			{
				foreach (var ip in PackageIpAddresses
					.Where(a => a.ItemId == itemId)
					.ToList())
				{
					if (CheckActorPackageRights(actorId, ip.PackageId))
					{
						ip.ItemId = null;
						ip.IsPrimary = false;
					}
				}
				/// <summary>TODO</summary>
				return SaveChanges();
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					"DeleteItemIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		#endregion

		#region VPS - Private Network Adapter
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetItemPrivateIPAddresses(int actorId, int itemId)
		{
			if (UseEntityFramework)
			{
				var addresses = PrivateIpAddresses
					.Include(a => a.Item)
					.Where(a => a.ItemId == itemId)
					.OrderByDescending(a => a.IsPrimary)
					.AsEnumerable()
					.Where(a => Clone.CheckActorPackageRights(actorId, a.Item.PackageId))
					.Select(a => new
					{
						AddressId = a.PrivateAddressId,
						a.IpAddress,
						a.IsPrimary
					});
				/// <summary>TODO</summary>
				return EntityDataReader(addresses);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					"GetItemPrivateIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddItemPrivateIPAddress(int actorId, int itemId, string ipAddress)
		{
			if (UseEntityFramework)
			{
				var packages = ServiceItems
					//TODO .Where(i => i.ItemId == itemId) or not?
					.Where(i => i.ItemId == itemId) // bugfix added by Simon Egli, 27.6.2024 
					.Select(i => i.PackageId)
					.ToList();
				if (packages
					.Any(packageId => CheckActorPackageRights(actorId, packageId)))
				{
					var ip = new Data.Entities.PrivateIpAddress()
					{
						ItemId = itemId,
						IpAddress = ipAddress,
						IsPrimary = false
					};
					PrivateIpAddresses.Add(ip);
					/// <summary>TODO</summary>
					return SaveChanges();
				}
				return 0;
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					"AddItemPrivateIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@IPAddress", ipAddress));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int SetItemPrivatePrimaryIPAddress(int actorId, int itemId, int privateAddressId)
		{
			if (UseEntityFramework)
			{
				var addresses = PrivateIpAddresses
					.Where(ip => ip.ItemId == itemId)
					.Include(ip => ip.Item)
					.ToList()
					.Where(ip => CheckActorPackageRights(actorId, ip.Item.PackageId));

				foreach (var ip in addresses) ip.IsPrimary = ip.PrivateAddressId == privateAddressId;

				/// <summary>TODO</summary>
				return SaveChanges();

			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					"SetItemPrivatePrimaryIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@PrivateAddressID", privateAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteItemPrivateIPAddress(int actorId, int itemId, int privateAddressId)
		{
			if (UseEntityFramework)
			{
				PrivateIpAddresses
					.RemoveRange(PrivateIpAddresses
						.Include(a => a.Item)
						.Where(pa => pa.PrivateAddressId == privateAddressId)
						.AsEnumerable()
						.Where(pa => Clone.CheckActorPackageRights(actorId, pa.Item.PackageId)));
				/// <summary>TODO</summary>
				return SaveChanges();
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					"DeleteItemPrivateIPAddress",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@PrivateAddressID", privateAddressId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int DeleteItemPrivateIPAddresses(int actorId, int itemId)
		{
			if (UseEntityFramework)
			{
				PrivateIpAddresses
					.RemoveRange(PrivateIpAddresses
						.Include(a => a.Item)
						.Where(pa => pa.ItemId == itemId)
						.AsEnumerable()
						.Where(pa => Clone.CheckActorPackageRights(actorId, pa.Item.PackageId)));
				/// <summary>TODO</summary>
				return SaveChanges();
			}
			else
			{
				return SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					"DeleteItemPrivateIPAddresses",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		#endregion

		#region BlackBerry
		/// <summary>Auto-generated member.</summary>

		public void AddBlackBerryUser(int accountId)
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				var user = new Data.Entities.BlackBerryUser()
				{
					AccountId = accountId,
					CreatedDate = now,
					ModifiedDate = now
				};
				BlackBerryUsers.Add(user);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"AddBlackBerryUser",
					new[] { new SqlParameter("@AccountID", accountId) });
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckBlackBerryUserExists(int accountId)
		{
			if (UseEntityFramework)
			{
				return BlackBerryUsers.Any(u => u.AccountId == accountId);
			}
			else
			{
				int res = (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "CheckBlackBerryUserExists",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
				return res > 0;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetBlackBerryUsers(int itemId, string sortColumn, string sortDirection, string name, string email, int startRow, int count)
		{
			if (UseEntityFramework)
			{
				if (string.IsNullOrEmpty(name)) name = "%";
				if (string.IsNullOrEmpty(email)) email = "%";
				var users = ExchangeAccounts
					.Where(ea => ea.ItemId == itemId &&
#if NETFRAMEWORK
						DbFunctions.Like(ea.DisplayName, name) &&
						DbFunctions.Like(ea.PrimaryEmailAddress, email))
#else
						EF.Functions.Like(ea.DisplayName, name) &&
						EF.Functions.Like(ea.PrimaryEmailAddress, email))
#endif
					.Join(BlackBerryUsers, ea => ea.AccountId, bb => bb.AccountId, (ea, bb) => new
					{
						ea.AccountId,
						ea.ItemId,
						ea.AccountName,
						ea.DisplayName,
						ea.PrimaryEmailAddress,
						ea.SamAccountName
					});

				users.Count();

				if (sortColumn == "DisplayName")
				{
					users = string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase) ? users.OrderBy(ea => ea.DisplayName) : users.OrderByDescending(ea => ea.DisplayName);




				}
				else
				{
					users = string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase) ? users.OrderBy(ea => ea.PrimaryEmailAddress) : users.OrderByDescending(ea => ea.PrimaryEmailAddress);




				}

				users = users.Skip(startRow).Take(count);

				// TODO bug not returning usersCount?
				//return EntityDataReader(usersCount, users);
				/// <summary>TODO</summary>
				return EntityDataReader(users);
			}
			else
			{
				SqlParameter[] sqlParams = new SqlParameter[]
				{
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", sortColumn),
					/// <summary>TODO</summary>
					new SqlParameter("@SortDirection", sortDirection),
					GetFilterSqlParam("@Name", name),
					GetFilterSqlParam("@Email", email),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("Count", count)
				};


				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetBlackBerryUsers", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetBlackBerryUsersCount(int itemId, string name, string email)
		{
			if (UseEntityFramework)
			{
				if (string.IsNullOrEmpty(name)) name = "%";
				if (string.IsNullOrEmpty(email)) email = "%";
				var users = ExchangeAccounts
					.Where(ea => ea.ItemId == itemId &&
#if NETFRAMEWORK
						DbFunctions.Like(ea.DisplayName, name) &&
						DbFunctions.Like(ea.PrimaryEmailAddress, email))
#else
						EF.Functions.Like(ea.DisplayName, name) &&
						EF.Functions.Like(ea.PrimaryEmailAddress, email))
#endif
					.Join(BlackBerryUsers, ea => ea.AccountId, bb => bb.AccountId, (ea, bb) => ea);

				return users.Count();
			}
			else
			{
				SqlParameter[] sqlParams = new SqlParameter[]
				{
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					GetFilterSqlParam("@Name", name),
					GetFilterSqlParam("@Email", email),
				};

				return (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "GetBlackBerryUsersCount", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>
		public void DeleteBlackBerryUser(int accountId)
		{
			if (UseEntityFramework)
			{
				BlackBerryUsers.Where(u => u.AccountId == accountId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteBlackBerryUser",
					new[] { new SqlParameter("@AccountID", accountId) });
			}
		}
		#endregion

		#region OCS
		/// <summary>Auto-generated member.</summary>

		public void AddOCSUser(int accountId, string instanceId)
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				var user = new Data.Entities.OcsUser()
				{
					AccountId = accountId,
					InstanceId = instanceId,
					CreatedDate = now,
					ModifiedDate = now
				};
				OcsUsers.Add(user);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"AddOCSUser",
					new[] {
						/// <summary>TODO</summary>
						new SqlParameter("@AccountID", accountId),
						/// <summary>TODO</summary>
						new SqlParameter("@InstanceID", instanceId)
					});
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckOCSUserExists(int accountId)
		{
			if (UseEntityFramework)
			{
				return OcsUsers.Any(u => u.AccountId == accountId);
			}
			else
			{
				int res = (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "CheckOCSUserExists",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
				return res > 0;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetOCSUsers(int itemId, string sortColumn, string sortDirection, string name, string email, int startRow, int count)
		{
			if (UseEntityFramework)
			{
				if (string.IsNullOrEmpty(name)) name = "%";
				if (string.IsNullOrEmpty(email)) email = "%";
				var users = ExchangeAccounts
					.Where(ea => ea.ItemId == itemId &&
#if NETFRAMEWORK
						DbFunctions.Like(ea.DisplayName, name) &&
						DbFunctions.Like(ea.PrimaryEmailAddress, email))
#else
						EF.Functions.Like(ea.DisplayName, name) &&
						EF.Functions.Like(ea.PrimaryEmailAddress, email))
#endif
					.Join(OcsUsers, ea => ea.AccountId, ou => ou.AccountId, (ea, ou) => new
					{
						ea.AccountId,
						ea.ItemId,
						ea.AccountName,
						ea.DisplayName,
						ou.InstanceId,
						ea.PrimaryEmailAddress,
						ea.SamAccountName
					});

				users.Count();

				if (sortColumn == "DisplayName")
				{
					users = string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase) ? users.OrderBy(ea => ea.DisplayName) : users.OrderByDescending(ea => ea.DisplayName);




				}
				else
				{
					users = string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase) ? users.OrderBy(ea => ea.PrimaryEmailAddress) : users.OrderByDescending(ea => ea.PrimaryEmailAddress);




				}

				users = users.Skip(startRow).Take(count);

                // TODO bug not returning usersCount?
                // return EntityDataReader(usersCount, users);
                /// <summary>TODO</summary>
                return EntityDataReader(users);
            }
            else
			{
				SqlParameter[] sqlParams = new SqlParameter[]
				{
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", sortColumn),
					/// <summary>TODO</summary>
					new SqlParameter("@SortDirection", sortDirection),
					GetFilterSqlParam("@Name", name),
					GetFilterSqlParam("@Email", email),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("Count", count)
				};

				return SqlHelper.ExecuteReader(
					 NativeConnectionString,
					 CommandType.StoredProcedure,
					 "GetOCSUsers", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>
		public int GetOCSUsersCount(int itemId, string name, string email)
		{
			if (UseEntityFramework)
			{
				if (string.IsNullOrEmpty(name)) name = "%";
				if (string.IsNullOrEmpty(email)) email = "%";
				var users = ExchangeAccounts
					.Where(ea => ea.ItemId == itemId &&
#if NETFRAMEWORK
						DbFunctions.Like(ea.DisplayName, name) &&
						DbFunctions.Like(ea.PrimaryEmailAddress, email))
#else
						EF.Functions.Like(ea.DisplayName, name) &&
						EF.Functions.Like(ea.PrimaryEmailAddress, email))
#endif
					.Join(OcsUsers, ea => ea.AccountId, bb => bb.AccountId, (ea, bb) => ea);

				return users.Count();
			}
			else
			{
				SqlParameter[] sqlParams = new SqlParameter[]
				{
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					GetFilterSqlParam("@Name", name),
					GetFilterSqlParam("@Email", email),
				};

				return (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "GetOCSUsersCount", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteOCSUser(string instanceId)
		{
			if (UseEntityFramework)
			{
				OcsUsers.Where(u => u.InstanceId == instanceId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteOCSUser",
					new[] { new SqlParameter("@InstanceId", instanceId) });

			}
		}
		/// <summary>Auto-generated member.</summary>

		public string GetOCSUserInstanceID(int accountId)
		{
			if (UseEntityFramework)
			{
				var instanceId = OcsUsers
					.Where(u => u.AccountId == accountId)
					.Select(u => u.InstanceId)
					.FirstOrDefault();
				/// <summary>TODO</summary>
				return instanceId;
			}
			else
			{
				return (string)SqlHelper.ExecuteScalar(NativeConnectionString,
					CommandType.StoredProcedure,
					"GetInstanceID",
					new[] { new SqlParameter("@AccountID", accountId) });
			}
		}

		#endregion

		#region SSL
		/// <summary>Auto-generated member.</summary>
		public int AddSSLRequest(int actorId, int packageId, int siteID, int userID, string friendlyname, string hostname, string csr, int csrLength, string distinguishedName, bool isRenewal, int previousID)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");
				var cert = new Data.Entities.SslCertificate()
				{
					UserId = userID,
					SiteId = siteID,
					FriendlyName = friendlyname,
					Hostname = hostname,
					DistinguishedName = distinguishedName,
					Csr = csr,
					CsrLength = csrLength,
					IsRenewal = isRenewal,
					PreviousId = previousID
				};
				SslCertificates.Add(cert);
				SaveChanges();
				return cert.Id;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@SSLID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddSSLRequest", prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userID),
					/// <summary>TODO</summary>
					new SqlParameter("@WebSiteID", siteID),
					/// <summary>TODO</summary>
					new SqlParameter("@FriendlyName", friendlyname),
					/// <summary>TODO</summary>
					new SqlParameter("@HostName", hostname),
					/// <summary>TODO</summary>
					new SqlParameter("@CSR", csr),
					/// <summary>TODO</summary>
					new SqlParameter("@CSRLength", csrLength),
					/// <summary>TODO</summary>
					new SqlParameter("@DistinguishedName", distinguishedName),
					/// <summary>TODO</summary>
					new SqlParameter("@IsRenewal", isRenewal),
					/// <summary>TODO</summary>
					new SqlParameter("@PreviousId", previousID));

				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void CompleteSSLRequest(int actorId, int packageId, int id, string certificate, string distinguishedName, string serialNumber, byte[] hash, DateTime validFrom, DateTime expiryDate)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");

				var cert = SslCertificates
					.FirstOrDefault(c => c.Id == id);
				if (cert != null)
				{
					cert.Certificate = certificate;
					cert.Installed = true;
					cert.SerialNumber = serialNumber;
					cert.DistinguishedName = distinguishedName;
					cert.Hash = Convert.ToBase64String(hash);
					cert.ValidFrom = validFrom;
					cert.ExpiryDate = expiryDate;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CompleteSSLRequest",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id),
					/// <summary>TODO</summary>
					new SqlParameter("@DistinguishedName", distinguishedName),
					/// <summary>TODO</summary>
					new SqlParameter("@Certificate", certificate),
					/// <summary>TODO</summary>
					new SqlParameter("@SerialNumber", serialNumber),
					/// <summary>TODO</summary>
					new SqlParameter("@Hash", Convert.ToBase64String(hash)),
					/// <summary>TODO</summary>
					new SqlParameter("@ValidFrom", validFrom),
					/// <summary>TODO</summary>
					new SqlParameter("@ExpiryDate", expiryDate));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddPFX(int actorId, int packageId, int siteID, int userID, string hostname, string friendlyName, string distinguishedName, int csrLength, string serialNumber, DateTime validFrom, DateTime expiryDate)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");
				var cert = new Data.Entities.SslCertificate()
				{
					UserId = userID,
					SiteId = siteID,
					FriendlyName = friendlyName,
					Hostname = hostname,
					DistinguishedName = distinguishedName,
					CsrLength = csrLength,
					SerialNumber = serialNumber,
					ValidFrom = validFrom,
					ExpiryDate = expiryDate,
					Installed = true
				};
				SslCertificates.Add(cert);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "AddPFX",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@UserID", userID),
					/// <summary>TODO</summary>
					new SqlParameter("@WebSiteID", siteID),
					/// <summary>TODO</summary>
					new SqlParameter("@FriendlyName", friendlyName),
					/// <summary>TODO</summary>
					new SqlParameter("@HostName", hostname),
					/// <summary>TODO</summary>
					new SqlParameter("@CSRLength", csrLength),
					/// <summary>TODO</summary>
					new SqlParameter("@DistinguishedName", distinguishedName),
					/// <summary>TODO</summary>
					new SqlParameter("@SerialNumber", serialNumber),
					/// <summary>TODO</summary>
					new SqlParameter("@ValidFrom", validFrom),
					/// <summary>TODO</summary>
					new SqlParameter("@ExpiryDate", expiryDate));
			}
		}

		// Does not exist in stored procedures
		/*public DataSet GetSSL(int actorId, int packageId, int id)
		{
			if (UseEntityFramework)
			{
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetSSL",
					/// <summary>TODO</summary>
					new SqlParameter("@SSLID", id));
			}
		}*/
		/// <summary>Auto-generated member.</summary>

		public DataSet GetCertificatesForSite(int actorId, int packageId, int siteId)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");
				var cert = SslCertificates
					.Where(c => c.SiteId == siteId)
					.Select(c => new
					{
						c.Id,
						c.UserId,
						c.SiteId,
						c.FriendlyName,
						c.Hostname,
						c.DistinguishedName,
						c.Csr,
						c.CsrLength,
						c.ValidFrom,
						c.ExpiryDate,
						c.Installed,
						c.IsRenewal,
						c.PreviousId,
						c.SerialNumber
					});
				/// <summary>TODO</summary>
				return EntityDataSet(cert);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetCertificatesForSite",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@websiteid", siteId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetPendingCertificates(int actorId, int packageId, int id, bool recursive)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");
				var certs = SslCertificates
					.Where(c => id == c.SiteId && c.Installed == false && c.IsRenewal == false)
					.Select(c => new
					{
						c.Id,
						c.UserId,
						c.SiteId,
						c.Hostname,
						c.Csr,
						c.Certificate,
						c.Hash,
						c.Installed
					});
				/// <summary>TODO</summary>
				return EntityDataSet(certs);
			}
			else
			{
				return SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetPendingSSLForWebsite",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageId", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@websiteid", id),
					/// <summary>TODO</summary>
					new SqlParameter("@Recursive", recursive));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetSSLCertificateByID(int actorId, int id)
		{
			if (UseEntityFramework)
			{
				var cert = SslCertificates
					.Where(c => c.Id == id)
					.Join(ServiceItems, c => c.SiteId, i => i.ItemId, (c, i) => new
					{
						c.Id,
						c.UserId,
						c.SiteId,
						c.Hostname,
						c.FriendlyName,
						c.Csr,
						c.Certificate,
						c.Hash,
						c.Installed,
						c.IsRenewal,
						c.PreviousId,
						i.PackageId
					})
					.AsEnumerable()
					.Where(c => Clone.CheckActorPackageRights(actorId, c.PackageId));
				/// <summary>TODO</summary>
				return EntityDataReader(cert);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetSSLCertificateByID",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorId", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int CheckSSL(int siteID, bool renewal)
		{
			if (UseEntityFramework)
			{
				//TODO add renewal stuff
				return SslCertificates.Any(c => c.SiteId == siteID) ? -1 : 0;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@Result", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CheckSSL",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@siteID", siteID),
					/// <summary>TODO</summary>
					new SqlParameter("@Renewal", renewal));

				return Convert.ToInt32(prmId.Value);
			}
		}

		// TODO this is a duplicate of GetSSLCertificateByID
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetSiteCert(int actorId, int siteID)
		{
			return UseEntityFramework
				? GetSSLCertificateByID(actorId, siteID)
				: SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetSSLCertificateByID",
					new SqlParameter("@ActorId", actorId),
					new SqlParameter("@ID", siteID));
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteCertificate(int actorId, int packageId, int id)
		{
			if (UseEntityFramework)
			{
				if (!CheckActorPackageRights(actorId, packageId))
					/// <summary>TODO</summary>
					throw new AccessViolationException("You are not allowed to access this package");
				SslCertificates.Where(c => c.Id == id).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "DeleteCertificate",
					/// <summary>TODO</summary>
					new SqlParameter("@ActorID", actorId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId),
					/// <summary>TODO</summary>
					new SqlParameter("@id", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckSSLExistsForWebsite(int siteId)
		{
			if (UseEntityFramework)
			{
				return SslCertificates.Any(c => c.SiteId == siteId);
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@Result", SqlDbType.Bit);
				prmId.Direction = ParameterDirection.Output;
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CheckSSLExistsForWebsite", prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@siteID", siteId),
					/// <summary>TODO</summary>
					new SqlParameter("@SerialNumber", ""));
				return Convert.ToBoolean(prmId.Value);
			}
		}
		#endregion

		#region Lync
		/// <summary>Auto-generated member.</summary>

		public void AddLyncUser(int accountId, int lyncUserPlanId, string sipAddress)
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				var user = new Data.Entities.LyncUser()
				{
					AccountId = accountId,
					LyncUserPlanId = lyncUserPlanId,
					CreatedDate = now,
					ModifiedDate = now,
					SipAddress = sipAddress
				};
				LyncUsers.Add(user);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"AddLyncUser",
					new[] {
						/// <summary>TODO</summary>
						new SqlParameter("@AccountID", accountId),
						/// <summary>TODO</summary>
						new SqlParameter("@LyncUserPlanID", lyncUserPlanId),
						/// <summary>TODO</summary>
						new SqlParameter("@SipAddress", sipAddress)
					});
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateLyncUser(int accountId, string sipAddress)
		{
			if (UseEntityFramework)
			{
                LyncUsers
                    .Where(u => u.AccountId == accountId)
                    .ExecuteUpdate(lu => new Data.Entities.LyncUser() { SipAddress = sipAddress });
            }
            else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateLyncUser",
					new[] {
						/// <summary>TODO</summary>
						new SqlParameter("@AccountID", accountId),
						/// <summary>TODO</summary>
						new SqlParameter("@SipAddress", sipAddress)
					});
			}
		}
		/// <summary>Auto-generated member.</summary>


		public bool CheckLyncUserExists(int accountId)
		{
			if (UseEntityFramework)
			{
				return LyncUsers.Any(u => u.AccountId == accountId);
			}
			else
			{
				int res = (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "CheckLyncUserExists",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
				return res > 0;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool LyncUserExists(int accountId, string sipAddress)
		{
			if (UseEntityFramework)
			{
				return ExchangeAccountEmailAddresses.Any(a => a.EmailAddress == sipAddress && a.AccountId != accountId) ||
					ExchangeAccounts.Any(a => a.AccountId != accountId &&
						(a.PrimaryEmailAddress == sipAddress ||
						a.UserPrincipalName == sipAddress ||
						a.AccountName == sipAddress)) ||
					LyncUsers.Any((u => u.SipAddress == sipAddress));
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@Exists", SqlDbType.Bit);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"LyncUserExists",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@SipAddress", sipAddress),
					outParam);

				return Convert.ToBoolean(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetLyncUsers(int itemId, string sortColumn, string sortDirection, int startRow, int count)
		{
			if (UseEntityFramework)
			{
				var users = ExchangeAccounts
					.Where(ea => ea.ItemId == itemId)
					.Join(LyncUsers, ea => ea.AccountId, lu => lu.AccountId, (ea, lu) => new
					{
						ea.AccountId,
						ea.ItemId,
						ea.AccountName,
						ea.DisplayName,
						ea.UserPrincipalName,
						lu.SipAddress,
						ea.SamAccountName,
						lu.LyncUserPlanId,
						lu.LyncUserPlan.LyncUserPlanName
					});

				var countUsers = users.Count();

				users = string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase) ? users.OrderBy(ColumnName(sortColumn)) : users.OrderBy($"{sortColumn} desc");








				users = users.Skip(startRow).Take(count);

				/// <summary>TODO</summary>
				return EntityDataReader(countUsers, users);
			}
			else
			{
				SqlParameter[] sqlParams = new SqlParameter[] {
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", sortColumn),
					/// <summary>TODO</summary>
					new SqlParameter("@SortDirection", sortDirection),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("Count", count)
				};


				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetLyncUsers", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetLyncUsersByPlanId(int itemId, int planId)
		{
			if (UseEntityFramework)
			{
				var users = ExchangeAccounts
					.Where(ea => ea.ItemId == itemId)
					.Join(LyncUsers, ea => ea.AccountId, lu => lu.AccountId, (ea, lu) => new
					{
						ea.AccountId,
						ea.ItemId,
						ea.AccountName,
						ea.DisplayName,
						ea.UserPrincipalName,
						ea.SamAccountName,
						lu.LyncUserPlanId,
						lu.LyncUserPlan.LyncUserPlanName
					})
					.Where(u => u.LyncUserPlanId == planId);
				/// <summary>TODO</summary>
				return EntityDataReader(users);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetLyncUsersByPlanId",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@PlanId", planId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetLyncUsersCount(int itemId)
		{
			if (UseEntityFramework)
			{
				return ExchangeAccounts
					.Where(ea => ea.ItemId == itemId)
					.Join(LyncUsers, ea => ea.AccountId, l => l.AccountId, (ea, l) => null as object)
					.Count();
			}
			else
			{
				SqlParameter[] sqlParams = new[] { new SqlParameter("@ItemID", itemId) };

				return (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "GetLyncUsersCount", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteLyncUser(int accountId)
		{
			if (UseEntityFramework)
			{
				LyncUsers.Where(u => u.AccountId == accountId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteLyncUser",
					new[] { new SqlParameter("@AccountId", accountId) });

			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddLyncUserPlan(int itemID, LyncUserPlan lyncUserPlan)
		{
			if (UseEntityFramework)
			{
				var isDefault = lyncUserPlan.IsDefault;
				var plans = LyncUserPlans.Where(p => p.ItemId == itemID);
				if (!plans.Any() && lyncUserPlan.LyncUserPlanType == 0)
				{
					// First plan of type 0: no reset of defaults needed
				}
				else
				{
					if (isDefault && lyncUserPlan.LyncUserPlanType == 0)
					{
						foreach (var pl in plans) pl.IsDefault = false;
					}
				}

				var plan = new Data.Entities.LyncUserPlan()
				{
					ItemId = itemID,
					LyncUserPlanName = lyncUserPlan.LyncUserPlanName,
					LyncUserPlanType = lyncUserPlan.LyncUserPlanType,
					IM = lyncUserPlan.IM,
					Mobility = lyncUserPlan.Mobility,
					MobilityEnableOutsideVoice = lyncUserPlan.MobilityEnableOutsideVoice,
					Federation = lyncUserPlan.Federation,
					Conferencing = lyncUserPlan.Conferencing,
					EnterpriseVoice = lyncUserPlan.EnterpriseVoice,
					VoicePolicy = lyncUserPlan.VoicePolicy,
					IsDefault = lyncUserPlan.IsDefault,
					RemoteUserAccess = lyncUserPlan.RemoteUserAccess,
					PublicIMConnectivity = lyncUserPlan.PublicIMConnectivity,
					AllowOrganizeMeetingsWithExternalAnonymous = lyncUserPlan.AllowOrganizeMeetingsWithExternalAnonymous,
					Telephony = lyncUserPlan.Telephony,
					ServerUri = lyncUserPlan.ServerURI,
					ArchivePolicy = lyncUserPlan.ArchivePolicy,
					TelephonyDialPlanPolicy = lyncUserPlan.TelephonyDialPlanPolicy,
					TelephonyVoicePolicy = lyncUserPlan.TelephonyVoicePolicy
				};
				LyncUserPlans.Add(plan);
				SaveChanges();
				return plan.LyncUserPlanId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@LyncUserPlanId", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddLyncUserPlan",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemID),
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanName", lyncUserPlan.LyncUserPlanName),
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanType", lyncUserPlan.LyncUserPlanType),
					/// <summary>TODO</summary>
					new SqlParameter("@IM", lyncUserPlan.IM),
					/// <summary>TODO</summary>
					new SqlParameter("@Mobility", lyncUserPlan.Mobility),
					/// <summary>TODO</summary>
					new SqlParameter("@MobilityEnableOutsideVoice", lyncUserPlan.MobilityEnableOutsideVoice),
					/// <summary>TODO</summary>
					new SqlParameter("@Federation", lyncUserPlan.Federation),
					/// <summary>TODO</summary>
					new SqlParameter("@Conferencing", lyncUserPlan.Conferencing),
					/// <summary>TODO</summary>
					new SqlParameter("@EnterpriseVoice", lyncUserPlan.EnterpriseVoice),
					/// <summary>TODO</summary>
					new SqlParameter("@VoicePolicy", lyncUserPlan.VoicePolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDefault", lyncUserPlan.IsDefault),
					/// <summary>TODO</summary>
					new SqlParameter("@RemoteUserAccess", lyncUserPlan.RemoteUserAccess),
					/// <summary>TODO</summary>
					new SqlParameter("@PublicIMConnectivity", lyncUserPlan.PublicIMConnectivity),
					/// <summary>TODO</summary>
					new SqlParameter("@AllowOrganizeMeetingsWithExternalAnonymous", lyncUserPlan.AllowOrganizeMeetingsWithExternalAnonymous),
					/// <summary>TODO</summary>
					new SqlParameter("@Telephony", lyncUserPlan.Telephony),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerURI", lyncUserPlan.ServerURI),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchivePolicy", lyncUserPlan.ArchivePolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@TelephonyDialPlanPolicy", lyncUserPlan.TelephonyDialPlanPolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@TelephonyVoicePolicy", lyncUserPlan.TelephonyVoicePolicy));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateLyncUserPlan(int itemID, LyncUserPlan lyncUserPlan)
		{
			if (UseEntityFramework)
			{
				var plan = LyncUserPlans
					.FirstOrDefault(p => p.LyncUserPlanId == lyncUserPlan.LyncUserPlanId);
				if (plan != null)
				{
					plan.LyncUserPlanName = lyncUserPlan.LyncUserPlanName;
					plan.LyncUserPlanType = lyncUserPlan.LyncUserPlanType;
					plan.IM = lyncUserPlan.IM;
					plan.Mobility = lyncUserPlan.Mobility;
					plan.MobilityEnableOutsideVoice = lyncUserPlan.MobilityEnableOutsideVoice;
					plan.Federation = lyncUserPlan.Federation;
					plan.Conferencing = lyncUserPlan.Conferencing;
					plan.EnterpriseVoice = lyncUserPlan.EnterpriseVoice;
					plan.VoicePolicy = lyncUserPlan.VoicePolicy;
					plan.IsDefault = lyncUserPlan.IsDefault;
					plan.RemoteUserAccess = lyncUserPlan.RemoteUserAccess;
					plan.PublicIMConnectivity = lyncUserPlan.PublicIMConnectivity;
					plan.AllowOrganizeMeetingsWithExternalAnonymous = lyncUserPlan.AllowOrganizeMeetingsWithExternalAnonymous;
					plan.Telephony = lyncUserPlan.Telephony;
					plan.ServerUri = lyncUserPlan.ServerURI;
					plan.ArchivePolicy = lyncUserPlan.ArchivePolicy;
					plan.TelephonyDialPlanPolicy = lyncUserPlan.TelephonyDialPlanPolicy;
					plan.TelephonyVoicePolicy = lyncUserPlan.TelephonyVoicePolicy;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateLyncUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanId", lyncUserPlan.LyncUserPlanId),
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanName", lyncUserPlan.LyncUserPlanName),
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanType", lyncUserPlan.LyncUserPlanType),
					/// <summary>TODO</summary>
					new SqlParameter("@IM", lyncUserPlan.IM),
					/// <summary>TODO</summary>
					new SqlParameter("@Mobility", lyncUserPlan.Mobility),
					/// <summary>TODO</summary>
					new SqlParameter("@MobilityEnableOutsideVoice", lyncUserPlan.MobilityEnableOutsideVoice),
					/// <summary>TODO</summary>
					new SqlParameter("@Federation", lyncUserPlan.Federation),
					/// <summary>TODO</summary>
					new SqlParameter("@Conferencing", lyncUserPlan.Conferencing),
					/// <summary>TODO</summary>
					new SqlParameter("@EnterpriseVoice", lyncUserPlan.EnterpriseVoice),
					/// <summary>TODO</summary>
					new SqlParameter("@VoicePolicy", lyncUserPlan.VoicePolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDefault", lyncUserPlan.IsDefault),
					/// <summary>TODO</summary>
					new SqlParameter("@RemoteUserAccess", lyncUserPlan.RemoteUserAccess),
					/// <summary>TODO</summary>
					new SqlParameter("@PublicIMConnectivity", lyncUserPlan.PublicIMConnectivity),
					/// <summary>TODO</summary>
					new SqlParameter("@AllowOrganizeMeetingsWithExternalAnonymous", lyncUserPlan.AllowOrganizeMeetingsWithExternalAnonymous),
					/// <summary>TODO</summary>
					new SqlParameter("@Telephony", lyncUserPlan.Telephony),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerURI", lyncUserPlan.ServerURI),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchivePolicy", lyncUserPlan.ArchivePolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@TelephonyDialPlanPolicy", lyncUserPlan.TelephonyDialPlanPolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@TelephonyVoicePolicy", lyncUserPlan.TelephonyVoicePolicy));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteLyncUserPlan(int lyncUserPlanId)
		{
			if (UseEntityFramework)
			{
				LyncUserPlans.Where(p => p.LyncUserPlanId == lyncUserPlanId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteLyncUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanId", lyncUserPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetLyncUserPlan(int lyncUserPlanId)
		{
			if (UseEntityFramework)
			{
				var plan = LyncUserPlans
					.Where(p => p.LyncUserPlanId == lyncUserPlanId)
					.Select(p => new
					{
						p.LyncUserPlanId,
						p.ItemId,
						p.LyncUserPlanName,
						p.LyncUserPlanType,
						p.IM,
						p.Mobility,
						p.MobilityEnableOutsideVoice,
						p.Federation,
						p.Conferencing,
						p.EnterpriseVoice,
						p.VoicePolicy,
						p.IsDefault,
						p.RemoteUserAccess,
						p.PublicIMConnectivity,
						p.AllowOrganizeMeetingsWithExternalAnonymous,
						p.Telephony,
						p.ServerUri,
						p.ArchivePolicy,
						p.TelephonyDialPlanPolicy,
						p.TelephonyVoicePolicy
					});
				/// <summary>TODO</summary>
				return EntityDataReader(plan);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetLyncUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanId", lyncUserPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>


		public IDataReader GetLyncUserPlans(int itemId)
		{
			if (UseEntityFramework)
			{
				var plans = LyncUserPlans
					.Where(p => p.ItemId == itemId)
					.OrderBy(p => p.LyncUserPlanName)
					.Select(p => new
					{
						p.LyncUserPlanId,
						p.ItemId,
						p.LyncUserPlanName,
						p.LyncUserPlanType,
						p.IM,
						p.Mobility,
						p.MobilityEnableOutsideVoice,
						p.Federation,
						p.Conferencing,
						p.EnterpriseVoice,
						p.VoicePolicy,
						p.IsDefault
					});
				/// <summary>TODO</summary>
				return EntityDataReader(plans);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetLyncUserPlans",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void SetOrganizationDefaultLyncUserPlan(int itemId, int lyncUserPlanId)
		{
			if (UseEntityFramework)
			{
#if NETFRAMEWORK
				var plan = ExchangeOrganizations
					.FirstOrDefault(o => o.ItemId == itemId);
				if (plan != null)
				{
					plan.LyncUserPlanId = lyncUserPlanId;
					SaveChanges();
				}
#else
				ExchangeOrganizations
					.Where(o => o.ItemId == itemId)
					.ExecuteUpdate(set => set
						.SetProperty(p => p.LyncUserPlanId, lyncUserPlanId));
#endif
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SetOrganizationDefaultLyncUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanId", lyncUserPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetLyncUserPlanByAccountId(int AccountId)
		{
			if (UseEntityFramework)
			{
				var plan = LyncUserPlans
					.Join(LyncUsers
						.Where(u => u.AccountId == AccountId),
						p => p.LyncUserPlanId, u => u.LyncUserPlanId, (p, u) => new
						{
							p.LyncUserPlanId,
							p.ItemId,
							p.LyncUserPlanName,
							p.LyncUserPlanType,
							p.IM,
							p.Mobility,
							p.MobilityEnableOutsideVoice,
							p.Federation,
							p.Conferencing,
							p.EnterpriseVoice,
							p.VoicePolicy,
							p.IsDefault
						});
				/// <summary>TODO</summary>
				return EntityDataReader(plan);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetLyncUserPlanByAccountId",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", AccountId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void SetLyncUserLyncUserplan(int accountId, int lyncUserPlanId)
		{
			if (UseEntityFramework)
			{
                LyncUsers
                    .Where(u => u.AccountId == accountId)
                    .ExecuteUpdate(lu => new Data.Entities.LyncUser() { LyncUserPlanId = lyncUserPlanId });
            }
            else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SetLyncUserLyncUserplan",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					// TODO: Bug, LyncUserPlanId is not nullable in the table
					/// <summary>TODO</summary>
					new SqlParameter("@LyncUserPlanId", (lyncUserPlanId == 0) ? (object)DBNull.Value : (object)lyncUserPlanId));
			}
		}
		#endregion

		#region SfB
		/// <summary>Auto-generated member.</summary>

		public void AddSfBUser(int accountId, int sfbUserPlanId, string sipAddress)
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				var user = new Data.Entities.SfBUser()
				{
					AccountId = accountId,
					SfBUserPlanId = sfbUserPlanId,
					SipAddress = sipAddress,
					CreatedDate = now,
					ModifiedDate = now
				};
				SfBUsers.Add(user);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"AddSfBUser",
					new[] {
						/// <summary>TODO</summary>
						new SqlParameter("@AccountID", accountId),
						/// <summary>TODO</summary>
						new SqlParameter("@SfBUserPlanID", sfbUserPlanId),
						/// <summary>TODO</summary>
						new SqlParameter("@SipAddress", sipAddress)
					});
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateSfBUser(int accountId, string sipAddress)
		{
			if (UseEntityFramework)
			{
#if NETFRAMEWORK
				var user = SfBUsers.FirstOrDefault(u => u.AccountId == accountId);
				if (user != null)
				{
					user.SipAddress = sipAddress;
					SaveChanges();
				}
#else
				SfBUsers
					.Where(u => u.AccountId == accountId)
					.ExecuteUpdate(set => set
						.SetProperty(u => u.SipAddress, sipAddress));
#endif
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateSfBUser",
					new[] {
						/// <summary>TODO</summary>
						new SqlParameter("@AccountID", accountId),
						/// <summary>TODO</summary>
						new SqlParameter("@SipAddress", sipAddress)
					});
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckSfBUserExists(int accountId)
		{
			if (UseEntityFramework)
			{
				return SfBUsers.Any(u => u.AccountId == accountId);
			}
			else
			{
				int res = (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "CheckSfBUserExists",
				/// <summary>TODO</summary>
				new SqlParameter("@AccountID", accountId));
				return res > 0;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public bool SfBUserExists(int accountId, string sipAddress)
		{
			if (UseEntityFramework)
			{
				return ExchangeAccountEmailAddresses.Any(a => a.EmailAddress == sipAddress && a.AccountId != accountId) ||
					ExchangeAccounts.Any(a => (a.PrimaryEmailAddress == sipAddress ||
						a.UserPrincipalName == sipAddress || a.AccountName == sipAddress) && a.AccountId != accountId) ||
					SfBUsers.Any(u => u.SipAddress == sipAddress);
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@Exists", SqlDbType.Bit);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SfBUserExists",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@SipAddress", sipAddress),
					outParam);

				return Convert.ToBoolean(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetSfBUsers(int itemId, string sortColumn, string sortDirection, int startRow, int count)
		{
			if (UseEntityFramework)
			{
				var users = ExchangeAccounts
					.Where(ea => ea.ItemId == itemId)
					.Join(SfBUsers, ea => ea.AccountId, u => u.AccountId, (ea, u) => new
					{
						Account = ea,
						User = u
					})
					.Join(SfBUserPlans, u => u.User.SfBUserPlanId, p => p.SfBUserPlanId, (u, p) => new
					{
						u.Account.AccountId,
						u.Account.ItemId,
						u.Account.AccountName,
						u.Account.DisplayName,
						u.Account.UserPrincipalName,
						u.User.SipAddress,
						u.Account.SamAccountName,
						u.User.SfBUserPlanId,
						p.SfBUserPlanName
					});

				var countUsers = users.Count();

				users = string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase) ? users.OrderBy(ColumnName(sortColumn)) : users.OrderBy($"{sortColumn} desc");








				users = users.Skip(startRow).Take(count);

				/// <summary>TODO</summary>
				return EntityDataReader(countUsers, users);
			}
			else
			{
				SqlParameter[] sqlParams = new[]
				{
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", sortColumn),
					/// <summary>TODO</summary>
					new SqlParameter("@SortDirection", sortDirection),
					/// <summary>TODO</summary>
					new SqlParameter("@StartRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("Count", count)
				};

				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetSfBUsers", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetSfBUsersByPlanId(int itemId, int planId)
		{
			if (UseEntityFramework)
			{
				var users = ExchangeAccounts
					.Where(ea => ea.ItemId == itemId)
					.Join(SfBUsers, ea => ea.AccountId, u => u.AccountId, (ea, u) => new
					{
						Account = ea,
						User = u
					})
					.Where(u => u.User.SfBUserPlanId == planId)
					.Join(SfBUserPlans, u => u.User.SfBUserPlanId, p => p.SfBUserPlanId, (u, p) => new
					{
						u.Account.AccountId,
						u.Account.ItemId,
						u.Account.AccountName,
						u.Account.DisplayName,
						u.Account.UserPrincipalName,
						u.User.SipAddress,
						u.Account.SamAccountName,
						u.User.SfBUserPlanId,
						p.SfBUserPlanName
					});
				/// <summary>TODO</summary>
				return EntityDataReader(users);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetSfBUsersByPlanId",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@PlanId", planId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetSfBUsersCount(int itemId)
		{
			if (UseEntityFramework)
			{
				return ExchangeAccounts
					.Where(ea => ea.ItemId == itemId)
					.Join(SfBUsers, ea => ea.AccountId, u => u.AccountId, (ea, u) => ea)
					.Count();
			}
			else
			{
				SqlParameter[] sqlParams = new[] { new SqlParameter("@ItemID", itemId) };

				return (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "GetSfBUsersCount", sqlParams);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteSfBUser(int accountId)
		{
			if (UseEntityFramework)
			{
				LyncUsers.Where(u => u.AccountId == accountId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteSfBUser",
					new[] { new SqlParameter("@AccountId", accountId) });
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddSfBUserPlan(int itemID, SfBUserPlan sfbUserPlan)
		{
			if (UseEntityFramework)
			{
				var isDefault = sfbUserPlan.IsDefault;
				var plans = SfBUserPlans.Where(p => p.ItemId == itemID);
				if (!plans.Any() && sfbUserPlan.SfBUserPlanType == 0)
				{
					// First plan of type 0: no reset of defaults needed
				}
				else
				{
					if (isDefault && sfbUserPlan.SfBUserPlanType == 0)
					{
						foreach (var pl in plans) pl.IsDefault = false;
					}
				}

				var plan = new Data.Entities.SfBUserPlan()
				{
					ItemId = itemID,
					SfBUserPlanName = sfbUserPlan.SfBUserPlanName,
					SfBUserPlanType = sfbUserPlan.SfBUserPlanType,
					IM = sfbUserPlan.IM,
					Mobility = sfbUserPlan.Mobility,
					MobilityEnableOutsideVoice = sfbUserPlan.MobilityEnableOutsideVoice,
					Federation = sfbUserPlan.Federation,
					Conferencing = sfbUserPlan.Conferencing,
					EnterpriseVoice = sfbUserPlan.EnterpriseVoice,
					VoicePolicy = sfbUserPlan.VoicePolicy,
					IsDefault = sfbUserPlan.IsDefault,
					RemoteUserAccess = sfbUserPlan.RemoteUserAccess,
					PublicIMConnectivity = sfbUserPlan.PublicIMConnectivity,
					AllowOrganizeMeetingsWithExternalAnonymous = sfbUserPlan.AllowOrganizeMeetingsWithExternalAnonymous,
					Telephony = sfbUserPlan.Telephony,
					ServerUri = sfbUserPlan.ServerURI,
					ArchivePolicy = sfbUserPlan.ArchivePolicy,
					TelephonyDialPlanPolicy = sfbUserPlan.TelephonyDialPlanPolicy,
					TelephonyVoicePolicy = sfbUserPlan.TelephonyVoicePolicy
				};
				SfBUserPlans.Add(plan);
				SaveChanges();
				return plan.SfBUserPlanId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@SfBUserPlanId", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddSfBUserPlan",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemID),
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanName", sfbUserPlan.SfBUserPlanName),
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanType", sfbUserPlan.SfBUserPlanType),
					/// <summary>TODO</summary>
					new SqlParameter("@IM", sfbUserPlan.IM),
					/// <summary>TODO</summary>
					new SqlParameter("@Mobility", sfbUserPlan.Mobility),
					/// <summary>TODO</summary>
					new SqlParameter("@MobilityEnableOutsideVoice", sfbUserPlan.MobilityEnableOutsideVoice),
					/// <summary>TODO</summary>
					new SqlParameter("@Federation", sfbUserPlan.Federation),
					/// <summary>TODO</summary>
					new SqlParameter("@Conferencing", sfbUserPlan.Conferencing),
					/// <summary>TODO</summary>
					new SqlParameter("@EnterpriseVoice", sfbUserPlan.EnterpriseVoice),
					/// <summary>TODO</summary>
					new SqlParameter("@VoicePolicy", sfbUserPlan.VoicePolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDefault", sfbUserPlan.IsDefault),
					/// <summary>TODO</summary>
					new SqlParameter("@RemoteUserAccess", sfbUserPlan.RemoteUserAccess),
					/// <summary>TODO</summary>
					new SqlParameter("@PublicIMConnectivity", sfbUserPlan.PublicIMConnectivity),
					/// <summary>TODO</summary>
					new SqlParameter("@AllowOrganizeMeetingsWithExternalAnonymous", sfbUserPlan.AllowOrganizeMeetingsWithExternalAnonymous),
					/// <summary>TODO</summary>
					new SqlParameter("@Telephony", sfbUserPlan.Telephony),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerURI", sfbUserPlan.ServerURI),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchivePolicy", sfbUserPlan.ArchivePolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@TelephonyDialPlanPolicy", sfbUserPlan.TelephonyDialPlanPolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@TelephonyVoicePolicy", sfbUserPlan.TelephonyVoicePolicy));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>


		public void UpdateSfBUserPlan(int itemID, SfBUserPlan sfbUserPlan)
		{
			if (UseEntityFramework)
			{
				var plan = SfBUserPlans.FirstOrDefault(u => u.SfBUserPlanId == sfbUserPlan.SfBUserPlanId);
				if (plan != null)
				{
					plan.SfBUserPlanName = sfbUserPlan.SfBUserPlanName;
					plan.SfBUserPlanType = sfbUserPlan.SfBUserPlanType;
					plan.IM = sfbUserPlan.IM;
					plan.Mobility = sfbUserPlan.Mobility;
					plan.MobilityEnableOutsideVoice = sfbUserPlan.MobilityEnableOutsideVoice;
					plan.Federation = sfbUserPlan.Federation;
					plan.Conferencing = sfbUserPlan.Conferencing;
					plan.EnterpriseVoice = sfbUserPlan.EnterpriseVoice;
					plan.VoicePolicy = sfbUserPlan.VoicePolicy;
					plan.IsDefault = sfbUserPlan.IsDefault;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateSfBUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanId", sfbUserPlan.SfBUserPlanId),
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanName", sfbUserPlan.SfBUserPlanName),
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanType", sfbUserPlan.SfBUserPlanType),
					/// <summary>TODO</summary>
					new SqlParameter("@IM", sfbUserPlan.IM),
					/// <summary>TODO</summary>
					new SqlParameter("@Mobility", sfbUserPlan.Mobility),
					/// <summary>TODO</summary>
					new SqlParameter("@MobilityEnableOutsideVoice", sfbUserPlan.MobilityEnableOutsideVoice),
					/// <summary>TODO</summary>
					new SqlParameter("@Federation", sfbUserPlan.Federation),
					/// <summary>TODO</summary>
					new SqlParameter("@Conferencing", sfbUserPlan.Conferencing),
					/// <summary>TODO</summary>
					new SqlParameter("@EnterpriseVoice", sfbUserPlan.EnterpriseVoice),
					/// <summary>TODO</summary>
					new SqlParameter("@VoicePolicy", sfbUserPlan.VoicePolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDefault", sfbUserPlan.IsDefault),
					/// <summary>TODO</summary>
					new SqlParameter("@RemoteUserAccess", sfbUserPlan.RemoteUserAccess),
					/// <summary>TODO</summary>
					new SqlParameter("@PublicIMConnectivity", sfbUserPlan.PublicIMConnectivity),
					/// <summary>TODO</summary>
					new SqlParameter("@AllowOrganizeMeetingsWithExternalAnonymous", sfbUserPlan.AllowOrganizeMeetingsWithExternalAnonymous),
					/// <summary>TODO</summary>
					new SqlParameter("@Telephony", sfbUserPlan.Telephony),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerURI", sfbUserPlan.ServerURI),
					/// <summary>TODO</summary>
					new SqlParameter("@ArchivePolicy", sfbUserPlan.ArchivePolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@TelephonyDialPlanPolicy", sfbUserPlan.TelephonyDialPlanPolicy),
					/// <summary>TODO</summary>
					new SqlParameter("@TelephonyVoicePolicy", sfbUserPlan.TelephonyVoicePolicy));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteSfBUserPlan(int sfbUserPlanId)
		{
			if (UseEntityFramework)
			{
				SfBUserPlans.Where(p => p.SfBUserPlanId == sfbUserPlanId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteSfBUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanId", sfbUserPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetSfBUserPlan(int sfbUserPlanId)
		{
			if (UseEntityFramework)
			{
				var plan = SfBUserPlans
					.Where(p => p.SfBUserPlanId == sfbUserPlanId)
					.Select(p => new
					{
						p.SfBUserPlanId,
						p.ItemId,
						p.SfBUserPlanName,
						p.SfBUserPlanType,
						p.IM,
						p.Mobility,
						p.MobilityEnableOutsideVoice,
						p.Federation,
						p.Conferencing,
						p.EnterpriseVoice,
						p.VoicePolicy,
						p.IsDefault,
						p.RemoteUserAccess,
						p.PublicIMConnectivity,
						p.AllowOrganizeMeetingsWithExternalAnonymous,
						p.Telephony,
						p.ServerUri,
						p.ArchivePolicy,
						p.TelephonyDialPlanPolicy,
						p.TelephonyVoicePolicy
					});
				/// <summary>TODO</summary>
				return EntityDataReader(plan);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetSfBUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanId", sfbUserPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>


		public IDataReader GetSfBUserPlans(int itemId)
		{
			if (UseEntityFramework)
			{
				var plans = SfBUserPlans
					.Where(p => p.ItemId == itemId)
					.OrderBy(p => p.SfBUserPlanName)
					.Select(p => new
					{
						p.SfBUserPlanId,
						p.ItemId,
						p.SfBUserPlanName,
						p.SfBUserPlanType,
						p.IM,
						p.Mobility,
						p.MobilityEnableOutsideVoice,
						p.Federation,
						p.Conferencing,
						p.EnterpriseVoice,
						p.VoicePolicy,
						p.IsDefault
					});
				/// <summary>TODO</summary>
				return EntityDataReader(plans);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetSfBUserPlans",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>


		public void SetOrganizationDefaultSfBUserPlan(int itemId, int sfbUserPlanId)
		{
			if (UseEntityFramework)
			{
#if NETFRAMEWORK
				var org = ExchangeOrganizations.FirstOrDefault(o => o.ItemId == itemId);
				if (org != null)
				{
					org.SfBuserPlanId = sfbUserPlanId;
					SaveChanges();
				}
#else
				ExchangeOrganizations
					.Where(o => o.ItemId == itemId)
					.ExecuteUpdate(set => set
						.SetProperty(o => o.SfBuserPlanId, sfbUserPlanId));
#endif
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SetOrganizationDefaultSfBUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanId", sfbUserPlanId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetSfBUserPlanByAccountId(int AccountId)
		{
			if (UseEntityFramework)
			{
				var plans = SfBUserPlans
					.Join(SfBUsers
						.Where(u => u.AccountId == AccountId),
						p => p.SfBUserPlanId, u => u.SfBUserPlanId, (p, u) => p)
					.OrderBy(p => p.SfBUserPlanName)
					.Select(p => new
					{
						p.SfBUserPlanId,
						p.ItemId,
						p.SfBUserPlanName,
						p.SfBUserPlanType,
						p.IM,
						p.Mobility,
						p.MobilityEnableOutsideVoice,
						p.Federation,
						p.Conferencing,
						p.EnterpriseVoice,
						p.VoicePolicy,
						p.IsDefault
					});
				/// <summary>TODO</summary>
				return EntityDataReader(plans);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetSfBUserPlanByAccountId",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", AccountId));
			}
		}
		/// <summary>Auto-generated member.</summary>


		public void SetSfBUserSfBUserPlan(int accountId, int sfbUserPlanId)
		{
			if (UseEntityFramework)
			{
#if NETFRAMEWORK
				var user = SfBUsers.FirstOrDefault(u => u.AccountId == accountId);
				if (user != null)
				{
					user.SfBUserPlanId = sfbUserPlanId;
					SaveChanges();
				}
#else
				SfBUsers
					.Where(u => u.AccountId == accountId)
					.ExecuteUpdate(set => set
						.SetProperty(u => u.SfBUserPlanId, sfbUserPlanId));
#endif
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"SetSfBUserSfBUserPlan",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId),
					// TODO: Bug SfBUserPlanId is not nullable in the table
					/// <summary>TODO</summary>
					new SqlParameter("@SfBUserPlanId", (sfbUserPlanId == 0) ? (object)DBNull.Value : (object)sfbUserPlanId));
			}
		}
        #endregion

        #region Diverse Methods
        /*public int GetPackageIdByName(string Name)
		{
			const bool UseEntityFrameworkForGetPackageIdByName = true;

			int packageId = -1;

			if (UseEntityFrameworkForGetPackageIdByName || UseEntityFramework)
			{
				packageId = Packages
					.Where(p => Name == p.PackageName)
					.Select(p => (int?)p.PackageId)
					.FirstOrDefault() ?? -1;
				/// <summary>TODO</summary>
				return packageId;
			}
			else
			{   // TODO is this a bug? Querying Providers instead of Packages? But this routine has no
				// references, so the bug does not show up
				List<ProviderInfo> providers = ServerController.GetProviders();
				foreach (ProviderInfo providerInfo in providers)
				{
					if (string.Equals(Name, providerInfo.ProviderName, StringComparison.OrdinalIgnoreCase))
					{
						packageId = providerInfo.ProviderId;
						break;
					}
				}

				//if (-1 == packageId)
				//{
				//    throw new Exception("Provider not found");
				//}

				/// <summary>TODO</summary>
				return packageId;
			}
		}*/
        /// <summary>Auto-generated member.</summary>
        public IEnumerable<int> GetProviderIdsByServerId(int serverId) =>
			Services
			   .Where(s => s.ServerId == serverId)
			   .Select(s => s.ProviderId);
        /// <summary>Auto-generated member.</summary>

        public int GetServiceIdByProviderForServer(int providerId, int packageId)
		{
			if (UseEntityFramework)
			{
				return PackageServices
					.Where(p => p.PackageId == packageId)
					.Join(Services.Where(s => s.ProviderId == providerId),
						p => p.ServiceId, s => s.ServiceId, (p, s) => (int?)p.ServiceId)
					.FirstOrDefault() ?? -1;
			}
			else
			{
				IDataReader reader = SqlHelper.ExecuteReader(NativeConnectionString, CommandType.Text,
@"SELECT TOP 1
	PackageServices.ServiceID
FROM PackageServices
LEFT JOIN Services ON Services.ServiceID = PackageServices.ServiceID
WHERE PackageServices.PackageID = @PackageID AND Services.ProviderID = @ProviderID",
					/// <summary>TODO</summary>
					new SqlParameter("@ProviderID", providerId),
					/// <summary>TODO</summary>
					new SqlParameter("@PackageID", packageId));

				if (reader.Read())
				{
					return (int)reader["ServiceID"];
				}

				return -1;
			}
		}
		#endregion

		#region Enterprise Storage
		/// <summary>Auto-generated member.</summary>

		public int AddWebDavAccessToken(Base.HostedSolution.WebDavAccessToken accessToken)
		{
			if (UseEntityFramework)
			{
				var token = new Data.Entities.WebDavAccessToken()
				{
					FilePath = accessToken.FilePath,
					AccessToken = accessToken.AccessToken,
					AuthData = accessToken.AuthData,
					ExpirationDate = accessToken.ExpirationDate,
					AccountId = accessToken.AccountId,
					ItemId = accessToken.ItemId
				};
				WebDavAccessTokens.Add(token);
				SaveChanges();
				return token.Id;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@TokenID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddWebDavAccessToken",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@AccessToken", accessToken.AccessToken),
					/// <summary>TODO</summary>
					new SqlParameter("@FilePath", accessToken.FilePath),
					/// <summary>TODO</summary>
					new SqlParameter("@AuthData", accessToken.AuthData),
					/// <summary>TODO</summary>
					new SqlParameter("@ExpirationDate", accessToken.ExpirationDate),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accessToken.AccountId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", accessToken.ItemId));

				// read identity
				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteExpiredWebDavAccessTokens()
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				WebDavAccessTokens.Where(p => p.ExpirationDate < now).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteExpiredWebDavAccessTokens");
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetWebDavAccessTokenById(int id)
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				var tokens = WebDavAccessTokens
					.Where(t => t.Id == id && t.ExpirationDate > now);
				/// <summary>TODO</summary>
				return EntityDataReader(tokens);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetWebDavAccessTokenById",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetWebDavAccessTokenByAccessToken(Guid accessToken)
		{
			if (UseEntityFramework)
			{
				var now = DateTime.Now;
				var tokens = WebDavAccessTokens
					.Where(t => t.AccessToken == accessToken && t.ExpirationDate > now);
				/// <summary>TODO</summary>
				return EntityDataReader(tokens);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetWebDavAccessTokenByAccessToken",
					/// <summary>TODO</summary>
					new SqlParameter("@AccessToken", accessToken));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddEnterpriseFolder(int itemId, string folderName, int folderQuota, string locationDrive, string homeFolder, string domain, int? storageSpaceFolderId)
		{
			if (UseEntityFramework)
			{
				var folder = new Data.Entities.EnterpriseFolder()
				{
					ItemId = itemId,
					FolderName = folderName,
					FolderQuota = folderQuota,
					LocationDrive = locationDrive,
					HomeFolder = homeFolder,
					Domain = domain,
					StorageSpaceFolderId = storageSpaceFolderId
				};
				EnterpriseFolders.Add(folder);
				SaveChanges();
				return folder.EnterpriseFolderId;
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@FolderID", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddEnterpriseFolder",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderName", folderName),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderQuota", folderQuota),
					/// <summary>TODO</summary>
					new SqlParameter("@LocationDrive", locationDrive),
					/// <summary>TODO</summary>
					new SqlParameter("@HomeFolder", homeFolder),
					/// <summary>TODO</summary>
					new SqlParameter("@Domain", domain),
					/// <summary>TODO</summary>
					new SqlParameter("@StorageSpaceFolderId", storageSpaceFolderId));

				// read identity
				return Convert.ToInt32(prmId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateEnterpriseFolderStorageSpaceFolder(int itemId, string folderName, int? storageSpaceFolderId)
		{
			if (UseEntityFramework)
			{
				EnterpriseFolders
					.Where(f => f.ItemId == itemId && f.FolderName == folderName)
					.ExecuteUpdate(e => new Data.Entities.EnterpriseFolder
					{
						StorageSpaceFolderId = storageSpaceFolderId
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateEntepriseFolderStorageSpaceFolder",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderName", folderName),
					/// <summary>TODO</summary>
					new SqlParameter("@StorageSpaceFolderId", storageSpaceFolderId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteEnterpriseFolder(int itemId, string folderName)
		{
			if (UseEntityFramework)
			{
				EnterpriseFolders.Where(f => f.ItemId == itemId && f.FolderName == folderName)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteEnterpriseFolder",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderName", folderName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateEnterpriseFolder(int itemId, string folderID, string folderName, int folderQuota)
		{
			if (UseEntityFramework)
			{
				EnterpriseFolders
					.Where(f => f.ItemId == itemId && f.FolderName == folderID)
					.ExecuteUpdate(e => new Data.Entities.EnterpriseFolder
					{
						FolderName = folderName,
						FolderQuota = folderQuota
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateEnterpriseFolder",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderID", folderID),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderName", folderName),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderQuota", folderQuota));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetEnterpriseFolders(int itemId)
		{
			if (UseEntityFramework)
			{
				var folders = EnterpriseFolders
					.Where(f => f.ItemId == itemId)
					.Select(f => new { f.LocationDrive, f.HomeFolder, f.Domain })
					.Distinct();
				/// <summary>TODO</summary>
				return EntityDataReader(folders);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetEnterpriseFolders",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetEnterpriseFoldersPaged(int itemId, string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var folders = EnterpriseFolders
					.Where(f => f.ItemId == itemId)
					.Select(f => new
					{
						f.EnterpriseFolderId,
						f.ItemId,
						f.FolderName,
						f.FolderQuota,
						f.LocationDrive,
						f.HomeFolder,
						f.Domain,
						f.StorageSpaceFolderId,
						Name = f.StorageSpaceFolder != null ? f.StorageSpaceFolder.Name : null,
						StorageSpaceId = f.StorageSpaceFolder != null ? (int?)f.StorageSpaceFolder.StorageSpaceId : null,
						Path = f.StorageSpaceFolder != null ? f.StorageSpaceFolder.Path : null,
						UncPath = f.StorageSpaceFolder != null ? f.StorageSpaceFolder.UncPath : null,
						IsShared = f.StorageSpaceFolder != null ? (bool?)f.StorageSpaceFolder.IsShared : null,
						FsrmQuotaType = f.StorageSpaceFolder != null ? (QuotaType?)f.StorageSpaceFolder.FsrmQuotaType : null,
						FsrmQuotaSizeBytes = f.StorageSpaceFolder != null ? (long?)f.StorageSpaceFolder.FsrmQuotaSizeBytes : null
					});

				if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
				{
					folders = folders.Where(DynamicFunctions.ColumnLike(folders, filterColumn, filterValue));
				}

				var count = folders.Count();

				if (!string.IsNullOrEmpty(sortColumn))
				{
					folders = folders.OrderBy(ColumnName(sortColumn));
				}

				folders = folders.Skip(startRow).Take(maximumRows);

				/// <summary>TODO</summary>
				return EntityDataSet(count, folders);
			}
			else
			{
				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetEnterpriseFoldersPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetEnterpriseFolder(int itemId, string folderName)
		{
			if (UseEntityFramework)
			{
				var folder = EnterpriseFolders
					.Where(f => f.ItemId == itemId && f.FolderName == folderName)
					.Select(f => new
					{
						f.EnterpriseFolderId,
						f.ItemId,
						f.FolderName,
						f.FolderQuota,
						f.LocationDrive,
						f.HomeFolder,
						f.Domain,
						f.StorageSpaceFolderId,
						Name = f.StorageSpaceFolder != null ? f.StorageSpaceFolder.Name : null,
						StorageSpaceId = f.StorageSpaceFolder != null ? (int?)f.StorageSpaceFolder.StorageSpaceId : null,
						Path = f.StorageSpaceFolder != null ? f.StorageSpaceFolder.Path : null,
						UncPath = f.StorageSpaceFolder != null ? f.StorageSpaceFolder.UncPath : null,
						IsShared = f.StorageSpaceFolder != null ? (bool?)f.StorageSpaceFolder.IsShared : null,
						FsrmQuotaType = f.StorageSpaceFolder != null ? (QuotaType?)f.StorageSpaceFolder.FsrmQuotaType : null,
						FsrmQuotaSizeBytes = f.StorageSpaceFolder != null ? (long?)f.StorageSpaceFolder.FsrmQuotaSizeBytes : null
					})
					.Take(1);

				/// <summary>TODO</summary>
				return EntityDataReader(folder);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetEnterpriseFolder",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderName", folderName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetWebDavPortalUserSettingsByAccountId(int accountId)
		{
			if (UseEntityFramework)
			{
				var setting = WebDavPortalUsersSettings
					.Where(s => s.AccountId == accountId)
					.Take(1);
				/// <summary>TODO</summary>
				return EntityDataReader(setting);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetWebDavPortalUsersSettingsByAccountId",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountId", accountId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddWebDavPortalUsersSettings(int accountId, string settings)
		{
			if (UseEntityFramework)
			{
				var setting = new Data.Entities.WebDavPortalUsersSetting()
				{
					AccountId = accountId,
					Settings = settings
				};
				WebDavPortalUsersSettings.Add(setting);
				SaveChanges();
				return setting.Id;
			}
			else
			{
				SqlParameter settingsId = new SqlParameter("@WebDavPortalUsersSettingsId", SqlDbType.Int);
				settingsId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddWebDavPortalUsersSettings",
					settingsId,
					/// <summary>TODO</summary>
					new SqlParameter("@AccountId", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@Settings", settings));

				// read identity
				return Convert.ToInt32(settingsId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateWebDavPortalUsersSettings(int accountId, string settings)
		{
			if (UseEntityFramework)
			{
				WebDavPortalUsersSettings
					.Where(s => s.AccountId == accountId)
					.ExecuteUpdate(e => new Data.Entities.WebDavPortalUsersSetting
					{
						Settings = settings
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateWebDavPortalUsersSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@AccountId", accountId),
					/// <summary>TODO</summary>
					new SqlParameter("@Settings", settings));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteAllEnterpriseFolderOwaUsers(int itemId, int folderId)
		{
			if (UseEntityFramework)
			{
				EnterpriseFoldersOwaPermissions.Where(p => p.ItemId == itemId && p.FolderId == folderId)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteAllEnterpriseFolderOwaUsers",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderID", folderId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddEnterpriseFolderOwaUser(int itemId, int folderId, int accountId)
		{
			if (UseEntityFramework)
			{
				var permission = new Data.Entities.EnterpriseFoldersOwaPermission()
				{
					ItemId = itemId,
					FolderId = folderId,
					AccountId = accountId
				};
				EnterpriseFoldersOwaPermissions.Add(permission);
				SaveChanges();
				return permission.Id;
			}
			else
			{
				SqlParameter id = new SqlParameter("@ESOwsaUserId", SqlDbType.Int);
				id.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddEnterpriseFolderOwaUser",
					id,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderID", folderId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountId", accountId));

				// read identity
				return Convert.ToInt32(id.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetEnterpriseFolderOwaUsers(int itemId, int folderId)
		{
			if (UseEntityFramework)
			{
				var users = EnterpriseFoldersOwaPermissions
					.Where(p => p.ItemId == itemId && p.FolderId == folderId)
					.Select(p => new
					{
						p.Account.AccountId,
						p.Account.ItemId,
						p.Account.AccountType,
						p.Account.AccountName,
						p.Account.DisplayName,
						p.Account.PrimaryEmailAddress,
						p.Account.MailEnabledPublicFolder,
						p.Account.MailboxPlanId,
						p.Account.SubscriberNumber,
						p.Account.UserPrincipalName
					});
				/// <summary>TODO</summary>
				return EntityDataReader(users);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetEnterpriseFolderOwaUsers",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderID", folderId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetEnterpriseFolderId(int itemId, string folderName)
		{
			if (UseEntityFramework)
			{
				var folders = EnterpriseFolders
					.Where(f => f.ItemId == itemId && f.FolderName == folderName)
					.Select(f => new { f.EnterpriseFolderId })
					.Take(1);
				/// <summary>TODO</summary>
				return EntityDataReader(folders);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetEnterpriseFolderId",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@FolderName", folderName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetUserEnterpriseFolderWithOwaEditPermission(int itemId, int accountId)
		{
			if (UseEntityFramework)
			{
				var folder = EnterpriseFoldersOwaPermissions
					.Where(p => p.ItemId == itemId && p.AccountId == accountId)
					.Select(p => new { p.Folder.FolderName });
				/// <summary>TODO</summary>
				return EntityDataReader(folder);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetUserEnterpriseFolderWithOwaEditPermission",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
			}
		}
		#endregion

		#region Support Service Levels
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetSupportServiceLevels()
		{
			return UseEntityFramework
				? EntityDataReader(SupportServiceLevels)
				: SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetSupportServiceLevels");
		}
		/// <summary>Auto-generated member.</summary>

		public int AddSupportServiceLevel(string levelName, string levelDescription)
		{
			if (UseEntityFramework)
			{
				if (SupportServiceLevels.Any(l => l.LevelName == levelName)) return -1;

				var level = new Data.Entities.SupportServiceLevel()
				{
					LevelName = levelName,
					LevelDescription = levelDescription
				};
				SupportServiceLevels.Add(level);
				SaveChanges();
				var levelId = level.LevelId;

				var resourceGroupId = ResourceGroups
					.Where(g => g.GroupName == "Service Levels")
					.Select(g => (int?)g.GroupId)
					.FirstOrDefault();
				if (resourceGroupId != null)
				{
					var quotaLastId = Quotas
						.Max(q => q.QuotaId);
					var quotaOrderInGroup = Quotas
						.Where(q => q.GroupId == resourceGroupId)
						.Max(q => (int?)q.QuotaOrder) ?? 0;
					var curQuotaName = $"ServiceLevel.{levelName}";
					var curQuotaDescription = $"{levelName}, users";

					if (!Quotas.Any(q => q.QuotaName == curQuotaName))
					{
						var quota = new Data.Entities.Quota()
						{
							QuotaId = quotaLastId + 1,
							GroupId = resourceGroupId.Value,
							QuotaOrder = quotaOrderInGroup + 1,
							QuotaName = curQuotaName,
							QuotaDescription = curQuotaDescription,
							QuotaTypeId = 2,
							ServiceQuota = false,
							ItemTypeId = null
						};
						Quotas.Add(quota);
						SaveChanges();
					}
				}

				/// <summary>TODO</summary>
				return levelId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@LevelID", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddSupportServiceLevel",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@LevelName", levelName),
					/// <summary>TODO</summary>
					new SqlParameter("@LevelDescription", levelDescription));

				return Convert.ToInt32(outParam.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateSupportServiceLevel(int levelId, string levelName, string levelDescription)
		{
			if (UseEntityFramework)
			{
				var level = SupportServiceLevels
					.Where(l => l.LevelId == levelId)
					.FirstOrDefault();
				if (level != null)
				{
					var prevLevelName = level.LevelName;
					var prevQuotaName = $"ServiceLevel.{prevLevelName}";
					level.LevelName = levelName;
					level.LevelDescription = levelDescription;
					SaveChanges();

					var quota = Quotas
						.Where(q => q.QuotaName == prevQuotaName)
						.FirstOrDefault();
					if (quota != null)
					{
						quota.QuotaName = $"ServiceLevel.{levelName}";
						quota.QuotaDescription = $"{levelName}, users";
						SaveChanges();
					}
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateSupportServiceLevel",
					/// <summary>TODO</summary>
					new SqlParameter("@LevelID", levelId),
					/// <summary>TODO</summary>
					new SqlParameter("@LevelName", levelName),
					/// <summary>TODO</summary>
					new SqlParameter("@LevelDescription", levelDescription));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteSupportServiceLevel(int levelId)
		{
			if (UseEntityFramework)
			{
				var levelName = SupportServiceLevels
					.Where(l => l.LevelId == levelId)
					.Select(l => l.LevelName)
					.FirstOrDefault();
				if (levelName != null)
				{
					var quotaName = $"ServiceLevel.{levelName}";

					var quotaId = Quotas
						.Where(q => q.QuotaName == quotaName)
						.Select(q => (int?)q.QuotaId)
						.FirstOrDefault();
					if (quotaId != null)
					{
						HostingPlanQuotas.Where(q => q.QuotaId == quotaId).ExecuteDelete();
						PackageQuotas.Where(q => q.QuotaId == quotaId).ExecuteDelete();
						Quotas.Where(q => q.QuotaId == quotaId).ExecuteDelete();
					}

					var accounts = ExchangeAccounts
						.Where(a => a.LevelId == levelId);
					if (accounts.Any())
					{
						foreach (var account in accounts)
						{
							account.LevelId = null;
						}
						SaveChanges();
					}

					SupportServiceLevels.Where(l => l.LevelId == levelId).ExecuteDelete();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteSupportServiceLevel",
					/// <summary>TODO</summary>
					new SqlParameter("@LevelID", levelId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetSupportServiceLevel(int levelID)
		{
			return UseEntityFramework
				? EntityDataReader(SupportServiceLevels.Where(l => l.LevelId == levelID))
				: SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetSupportServiceLevel",
					new SqlParameter("@LevelID", levelID));
		}
		/// <summary>Auto-generated member.</summary>

		public bool CheckServiceLevelUsage(int levelID)
		{
			if (UseEntityFramework)
			{
				return SupportServiceLevels
					.Join(ExchangeAccounts, l => l.LevelId, ea => ea.LevelId, (l, ea) => ea)
					.Any(ea => ea.LevelId == levelID);
			}
			else
			{
				int res = (int)SqlHelper.ExecuteScalar(NativeConnectionString, CommandType.StoredProcedure, "CheckServiceLevelUsage",
					/// <summary>TODO</summary>
					new SqlParameter("@LevelID", levelID));
				return res > 0;
			}
		}

		#endregion

		#region Storage Spaces 
		/// <summary>Auto-generated member.</summary>

		public DataSet GetStorageSpaceLevelsPaged(string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var levels = StorageSpaceLevels
					.Select(l => new
					{
						l.Id,
						l.Name,
						l.Description
					});

				if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
				{
					levels = levels.Where(DynamicFunctions.ColumnLike(levels, filterColumn, filterValue));
				}

				var count = levels.Count();

				if (!string.IsNullOrEmpty(sortColumn))
				{
					levels = levels.OrderBy(ColumnName(sortColumn));
				}

				levels = levels.Skip(startRow).Take(maximumRows);

				/// <summary>TODO</summary>
				return EntityDataSet(count, levels);
			}
			else
			{
				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpaceLevelsPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetStorageSpaceLevelById(int id)
		{
			if (UseEntityFramework)
			{
				var levels = StorageSpaceLevels
					.Where(sl => sl.Id == id)
					.Select(sl => new { sl.Id, sl.Name, sl.Description })
					.Take(1);
				/// <summary>TODO</summary>
				return EntityDataReader(levels);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpaceLevelById",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int UpdateStorageSpaceLevel(StorageSpaceLevel level)
		{
			if (UseEntityFramework)
			{
				return StorageSpaceLevels
					.Where(l => l.Id == level.Id)
					.ExecuteUpdate(l => new Data.Entities.StorageSpaceLevel
					{
						Name = level.Name,
						Description = level.Description
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateStorageSpaceLevel",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", level.Id),
					/// <summary>TODO</summary>
					new SqlParameter("@Name", level.Name),
					/// <summary>TODO</summary>
					new SqlParameter("@Description", level.Description));

				return level.Id;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int InsertStorageSpaceLevel(StorageSpaceLevel level)
		{
			if (UseEntityFramework)
			{
				var lev = new Data.Entities.StorageSpaceLevel()
				{
					Name = level.Name,
					Description = level.Description
				};
				StorageSpaceLevels.Add(lev);
				SaveChanges();
				return lev.Id;
			}
			else
			{
				SqlParameter id = new SqlParameter("@ID", SqlDbType.Int);
				id.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"InsertStorageSpaceLevel",
					id,
					/// <summary>TODO</summary>
					new SqlParameter("@Name", level.Name),
					/// <summary>TODO</summary>
					new SqlParameter("@Description", level.Description));

				// read identity
				return Convert.ToInt32(id.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void RemoveStorageSpaceLevel(int id)
		{
			if (UseEntityFramework)
			{
				StorageSpaceLevels
					.Where(sl => sl.Id == id)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"RemoveStorageSpaceLevel",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetStorageSpaceLevelResourceGroups(int levelId)
		{
			if (UseEntityFramework)
			{
				var groups = StorageSpaceLevelResourceGroups
					.Where(g => g.LevelId == levelId)
					.Select(g => new
					{
						g.Group.GroupId,
						g.Group.GroupName,
						g.Group.GroupOrder,
						g.Group.GroupController,
						g.Group.ShowGroup
					});
				/// <summary>TODO</summary>
				return EntityDataReader(groups);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetLevelResourceGroups",
					/// <summary>TODO</summary>
					new SqlParameter("@LevelId", levelId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void RemoveStorageSpaceLevelResourceGroups(int levelId)
		{
			if (UseEntityFramework)
			{
				StorageSpaceLevelResourceGroups.Where(g => g.LevelId == levelId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteLevelResourceGroups",
					/// <summary>TODO</summary>
					new SqlParameter("@LevelId", levelId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddStorageSpaceLevelResourceGroup(int levelId, int groupId)
		{
			if (UseEntityFramework)
			{
				var group = new Data.Entities.StorageSpaceLevelResourceGroup()
				{
					LevelId = levelId,
					GroupId = groupId
				};
				StorageSpaceLevelResourceGroups.Add(group);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddLevelResourceGroups",
					/// <summary>TODO</summary>
					new SqlParameter("@LevelId", levelId),
					/// <summary>TODO</summary>
					new SqlParameter("@GroupId", groupId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetStorageSpacesPaged(string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var spaces = StorageSpaces
					.Select(s => new
					{
						s.Id,
						s.Name,
						s.ServiceId,
						s.ServerId,
						s.LevelId,
						s.Path,
						s.FsrmQuotaType,
						s.FsrmQuotaSizeBytes,
						s.IsShared,
						s.IsDisabled,
						s.UncPath,
						UsedSizeBytes = StorageSpaceFolders
							.Where(f => f.StorageSpaceId == s.Id)
							.Sum(f => (long?)f.FsrmQuotaSizeBytes) ?? 0
					});

				if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
				{
					spaces = spaces.Where(DynamicFunctions.ColumnLike(spaces, filterColumn, filterValue));
				}

				var count = spaces.Count();

				if (!string.IsNullOrEmpty(sortColumn))
				{
					spaces = spaces.OrderBy(ColumnName(sortColumn));
				}

				spaces = spaces.Skip(startRow).Take(maximumRows);

				/// <summary>TODO</summary>
				return EntityDataSet(count, spaces);
			}
			else
			{
				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpacesPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetStorageSpaceById(int id)
		{
			if (UseEntityFramework)
			{
				var spaces = StorageSpaces
					.Where(s => s.Id == id)
					.Take(1)
					.Select(s => new
					{
						s.Id,
						s.Name,
						s.ServiceId,
						s.ServerId,
						s.LevelId,
						s.Path,
						s.FsrmQuotaType,
						s.FsrmQuotaSizeBytes,
						s.IsShared,
						s.IsDisabled,
						s.UncPath,
						UsedSizeBytes = StorageSpaceFolders
							.Where(f => f.StorageSpaceId == s.Id)
							.Sum(f => (long?)f.FsrmQuotaSizeBytes) ?? 0
					});
				/// <summary>TODO</summary>
				return EntityDataReader(spaces);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpaceById",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetStorageSpaceByServiceAndPath(int serverId, string path)
		{
			if (UseEntityFramework)
			{
				var spaces = StorageSpaces
					.Where(s => s.ServerId == serverId && s.Path == path)
					.Take(1)
					.Select(s => new
					{
						s.Id,
						s.Name,
						s.ServiceId,
						s.ServerId,
						s.LevelId,
						s.Path,
						s.FsrmQuotaType,
						s.FsrmQuotaSizeBytes,
						s.IsShared,
						s.IsDisabled,
						s.UncPath,
						UsedSizeBytes = StorageSpaceFolders
							.Where(f => f.StorageSpaceId == s.Id)
							.Sum(f => (long?)f.FsrmQuotaSizeBytes) ?? 0
					});
				/// <summary>TODO</summary>
				return EntityDataReader(spaces);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpaceByServiceAndPath",
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@Path", path));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int UpdateStorageSpace(StorageSpace space)
		{
			if (UseEntityFramework)
			{
				var sp = StorageSpaces
					.FirstOrDefault(s => s.Id == space.Id);
				if (sp != null)
				{
					sp.Name = space.Name;
					sp.ServiceId = space.ServiceId;
					sp.ServerId = space.ServerId;
					sp.LevelId = space.LevelId;
					sp.Path = space.Path;
					sp.FsrmQuotaType = space.FsrmQuotaType;
					sp.FsrmQuotaSizeBytes = space.FsrmQuotaSizeBytes;
					sp.IsShared = space.IsShared;
					sp.UncPath = space.UncPath;
					sp.IsDisabled = space.IsDisabled;
					SaveChanges();
				}
				return space.Id;
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateStorageSpace",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", space.Id),
					/// <summary>TODO</summary>
					new SqlParameter("@Name", space.Name),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceId", space.ServiceId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", space.ServerId),
					/// <summary>TODO</summary>
					new SqlParameter("@LevelId", space.LevelId),
					/// <summary>TODO</summary>
					new SqlParameter("@Path", space.Path),
					/// <summary>TODO</summary>
					new SqlParameter("@FsrmQuotaType", space.FsrmQuotaType),
					/// <summary>TODO</summary>
					new SqlParameter("@FsrmQuotaSizeBytes", space.FsrmQuotaSizeBytes),
					/// <summary>TODO</summary>
					new SqlParameter("@IsShared", space.IsShared),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDisabled", space.IsDisabled),
					/// <summary>TODO</summary>
					new SqlParameter("@UncPath", space.UncPath));

				return space.Id;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int InsertStorageSpace(StorageSpace space)
		{
			if (UseEntityFramework)
			{
				var sp = new Data.Entities.StorageSpace()
				{
					Name = space.Name,
					ServiceId = space.ServiceId,
					ServerId = space.ServerId,
					LevelId = space.LevelId,
					Path = space.Path,
					FsrmQuotaType = space.FsrmQuotaType,
					FsrmQuotaSizeBytes = space.FsrmQuotaSizeBytes,
					IsShared = space.IsShared,
					UncPath = space.UncPath,
					IsDisabled = space.IsDisabled
				};
				StorageSpaces.Add(sp);
				SaveChanges();
				return sp.Id;
			}
			else
			{
				SqlParameter id = new SqlParameter("@ID", SqlDbType.Int);
				id.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"InsertStorageSpace",
					id,
					/// <summary>TODO</summary>
					new SqlParameter("@Name", space.Name),
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceId", space.ServiceId),
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", space.ServerId),
					/// <summary>TODO</summary>
					new SqlParameter("@LevelId", space.LevelId),
					/// <summary>TODO</summary>
					new SqlParameter("@Path", space.Path),
					/// <summary>TODO</summary>
					new SqlParameter("@FsrmQuotaType", space.FsrmQuotaType),
					/// <summary>TODO</summary>
					new SqlParameter("@FsrmQuotaSizeBytes", space.FsrmQuotaSizeBytes),
					/// <summary>TODO</summary>
					new SqlParameter("@IsShared", space.IsShared),
					/// <summary>TODO</summary>
					new SqlParameter("@IsDisabled", space.IsDisabled),
					/// <summary>TODO</summary>
					new SqlParameter("@UncPath", space.UncPath));

				// read identity
				return Convert.ToInt32(id.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void RemoveStorageSpace(int id)
		{
			if (UseEntityFramework)
			{
				StorageSpaces.Where(s => s.Id == id).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"RemoveStorageSpace",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetStorageSpacesByLevelId(int levelId)
		{
			if (UseEntityFramework)
			{
				var spaces = StorageSpaces
					.Where(s => s.LevelId == levelId)
					.Join(StorageSpaceLevels, s => s.LevelId, l => l.Id, (s, l) => s)
					.Select(s => new
					{
						s.Id,
						s.Name,
						s.ServiceId,
						s.ServerId,
						s.LevelId,
						s.Path,
						s.FsrmQuotaType,
						s.FsrmQuotaSizeBytes,
						s.IsShared,
						s.IsDisabled,
						s.UncPath,
						UsedSizeBytes = StorageSpaceFolders
							.Where(f => f.StorageSpaceId == s.Id)
							.Sum(f => (long?)f.FsrmQuotaSizeBytes) ?? 0
					});
				/// <summary>TODO</summary>
				return EntityDataSet(spaces);
			}
			else
			{
				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpacesByLevelId",
					/// <summary>TODO</summary>
					new SqlParameter("@LevelId", levelId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetStorageSpacesByResourceGroupName(string groupName)
		{
			if (UseEntityFramework)
			{
				var spaces = StorageSpaces
					.Join(StorageSpaceLevelResourceGroups
						.Where(g => g.Group.GroupName == groupName), s => s.LevelId, g => g.LevelId, (s, g) => s)
					.Select(s => new
					{
						s.Id,
						s.Name,
						s.ServiceId,
						s.ServerId,
						s.LevelId,
						s.Path,
						s.FsrmQuotaType,
						s.FsrmQuotaSizeBytes,
						s.IsShared,
						s.IsDisabled,
						s.UncPath,
						UsedSizeBytes = StorageSpaceFolders
							.Where(f => f.StorageSpaceId == s.Id)
							.Sum(f => (long?)f.FsrmQuotaSizeBytes) ?? 0
					});
				/// <summary>TODO</summary>
				return EntityDataReader(spaces);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpacesByResourceGroupName",
					/// <summary>TODO</summary>
					new SqlParameter("@ResourceGroupName", groupName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int CreateStorageSpaceFolder(StorageSpaceFolder folder)
		{
			folder.Id = CreateStorageSpaceFolder(folder.Name, folder.StorageSpaceId, folder.Path, folder.UncPath, folder.IsShared, folder.FsrmQuotaType, folder.FsrmQuotaSizeBytes);

			return folder.Id;
		}
		/// <summary>Auto-generated member.</summary>

		public int CreateStorageSpaceFolder(string name, int storageSpaceId, string path, string uncPath, bool isShared, QuotaType quotaType, long fsrmQuotaSizeBytes)
		{
			if (UseEntityFramework)
			{
				var folder = new Data.Entities.StorageSpaceFolder()
				{
					Name = name,
					StorageSpaceId = storageSpaceId,
					Path = path,
					UncPath = uncPath,
					IsShared = isShared,
					FsrmQuotaType = quotaType,
					FsrmQuotaSizeBytes = fsrmQuotaSizeBytes
				};
				StorageSpaceFolders.Add(folder);
				SaveChanges();
				return folder.Id;
			}
			else
			{
				SqlParameter id = new SqlParameter("@ID", SqlDbType.Int);
				id.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"CreateStorageSpaceFolder",
					id,
					/// <summary>TODO</summary>
					new SqlParameter("@Name", name),
					/// <summary>TODO</summary>
					new SqlParameter("@StorageSpaceId", storageSpaceId),
					/// <summary>TODO</summary>
					new SqlParameter("@Path", path),
					/// <summary>TODO</summary>
					new SqlParameter("@UncPath", uncPath),
					/// <summary>TODO</summary>
					new SqlParameter("@IsShared", isShared),
					/// <summary>TODO</summary>
					new SqlParameter("@FsrmQuotaType", quotaType),
					/// <summary>TODO</summary>
					new SqlParameter("@FsrmQuotaSizeBytes", fsrmQuotaSizeBytes));

				// read identity
				return Convert.ToInt32(id.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int UpdateStorageSpaceFolder(StorageSpaceFolder folder)
		{
			/// <summary>TODO</summary>
			return UpdateStorageSpaceFolder(folder.Id, folder.Name, folder.StorageSpaceId, folder.Path, folder.UncPath,
				folder.IsShared, folder.FsrmQuotaType, folder.FsrmQuotaSizeBytes);
		}
		/// <summary>Auto-generated member.</summary>

		public int UpdateStorageSpaceFolder(int id, string folderName, int storageSpaceId, string path, string uncPath, bool isShared, QuotaType type, long fsrmQuotaSizeBytes)
		{
			if (UseEntityFramework)
			{
				var folder = StorageSpaceFolders
					.FirstOrDefault(f => f.Id == id);
				if (folder != null)
				{
					folder.Name = folderName;
					folder.StorageSpaceId = storageSpaceId;
					folder.Path = path;
					folder.UncPath = uncPath;
					folder.IsShared = isShared;
					folder.FsrmQuotaType = type;
					folder.FsrmQuotaSizeBytes = fsrmQuotaSizeBytes;
					SaveChanges();
				}
				/// <summary>TODO</summary>
				return id;
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateStorageSpaceFolder",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id),
					/// <summary>TODO</summary>
					new SqlParameter("@Name", folderName),
					/// <summary>TODO</summary>
					new SqlParameter("@StorageSpaceId", storageSpaceId),
					/// <summary>TODO</summary>
					new SqlParameter("@Path", path),
					/// <summary>TODO</summary>
					new SqlParameter("@UncPath", uncPath),
					/// <summary>TODO</summary>
					new SqlParameter("@IsShared", isShared),
					/// <summary>TODO</summary>
					new SqlParameter("@FsrmQuotaType", type),
					/// <summary>TODO</summary>
					new SqlParameter("@FsrmQuotaSizeBytes", fsrmQuotaSizeBytes));

				/// <summary>TODO</summary>
				return id;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetStorageSpaceFoldersByStorageSpaceId(int id)
		{
			if (UseEntityFramework)
			{
				var folders = StorageSpaceFolders
					.Where(f => f.StorageSpaceId == id)
					.Select(f => new
					{
						f.Id,
						f.Name,
						f.StorageSpaceId,
						f.Path,
						f.UncPath,
						f.IsShared,
						f.FsrmQuotaType,
						f.FsrmQuotaSizeBytes
					});
				/// <summary>TODO</summary>
				return EntityDataReader(folders);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpaceFoldersByStorageSpaceId",
					/// <summary>TODO</summary>
					new SqlParameter("@StorageSpaceId", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetStorageSpaceFolderById(int id)
		{
			if (UseEntityFramework)
			{
				var folder = StorageSpaceFolders
					.Where(f => f.Id == id)
					.Take(1)
					.Select(f => new
					{
						f.Id,
						f.Name,
						f.StorageSpaceId,
						f.Path,
						f.UncPath,
						f.IsShared,
						f.FsrmQuotaType,
						f.FsrmQuotaSizeBytes
					});
				/// <summary>TODO</summary>
				return EntityDataReader(folder);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetStorageSpaceFolderById",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void RemoveStorageSpaceFolder(int id)
		{
			if (UseEntityFramework)
			{
				StorageSpaceFolders.Where(f => f.Id == id).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"RemoveStorageSpaceFolder",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		#endregion

		#region RDS
		/// <summary>Auto-generated member.</summary>

		public bool CheckRDSServerExists(string ServerFQDN)
		{
			if (UseEntityFramework)
			{
				return RdsServers.Any(s => s.FqdName == ServerFQDN);
			}
			else
			{
				SqlParameter prmId = new SqlParameter("@Result", SqlDbType.Int);
				prmId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "CheckRDSServer",
					prmId,
					/// <summary>TODO</summary>
					new SqlParameter("@ServerFQDN", ServerFQDN));

				return Convert.ToInt32(prmId.Value) != 0;
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRdsServerSettings(int serverId, string settingsName)
		{
			if (UseEntityFramework)
			{
				var settings = RdsServerSettings
					.Where(s => s.RdsServerId == serverId && s.SettingsName == settingsName)
					.Select(s => new
					{
						s.RdsServerId,
						s.PropertyName,
						s.PropertyValue,
						s.ApplyUsers,
						s.ApplyAdministrators
					});
				/// <summary>TODO</summary>
				return EntityDataReader(settings);
			}
			else
			{
				return SqlHelper.ExecuteReader(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetRDSServerSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@SettingsName", settingsName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateRdsServerSettings(int serverId, string settingsName, string xml)
		{
			if (UseEntityFramework)
			{
				using (var transaction = Database.BeginTransaction())
				{
					RdsServerSettings
						.Where(s => s.RdsServerId == serverId && s.SettingsName == settingsName)
						.ExecuteDelete();

					var settings = XElement.Parse(xml)
						.Elements()
						.Select(e => new Data.Entities.RdsServerSetting
						{
							RdsServerId = serverId,
							SettingsName = settingsName,
							ApplyUsers = (int)e.Attribute("applyUsers") != 0,
							ApplyAdministrators = (int)e.Attribute("applyAdministrators") != 0,
							PropertyName = (string)e.Attribute("name"),
							PropertyValue = (string)e.Attribute("value")
						});
					RdsServerSettings.AddRange(settings);
					SaveChanges();
					transaction.Commit();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "UpdateRDSServerSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@SettingsName", settingsName),
					/// <summary>TODO</summary>
					new SqlParameter("@Xml", xml));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddRdsCertificate(int serviceId, string content, byte[] hash, string fileName, DateTime? validFrom, DateTime? expiryDate)
		{
			if (UseEntityFramework)
			{
				var cert = new Data.Entities.RdsCertificate()
				{
					ServiceId = serviceId,
					Content = content,
					Hash = Convert.ToBase64String(hash),
					FileName = fileName,
					ValidFrom = validFrom,
					ExpiryDate = expiryDate
				};
				RdsCertificates.Add(cert);
				SaveChanges();
				return cert.Id;
			}
			else
			{
				SqlParameter rdsCertificateId = new SqlParameter("@RDSCertificateID", SqlDbType.Int);
				rdsCertificateId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddRDSCertificate",
					rdsCertificateId,
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceId", serviceId),
					/// <summary>TODO</summary>
					new SqlParameter("@Content", content),
					/// <summary>TODO</summary>
					new SqlParameter("@Hash", Convert.ToBase64String(hash)),
					/// <summary>TODO</summary>
					new SqlParameter("@FileName", fileName),
					/// <summary>TODO</summary>
					new SqlParameter("@ValidFrom", validFrom),
					/// <summary>TODO</summary>
					new SqlParameter("@ExpiryDate", expiryDate));

				return Convert.ToInt32(rdsCertificateId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRdsCertificateByServiceId(int serviceId)
		{
			if (UseEntityFramework)
			{
				var cert = RdsCertificates
					.Where(c => c.ServiceId == serviceId)
					.OrderByDescending(c => c.Id)
					.Take(1)
					.Select(c => new
					{
						c.Id,
						c.ServiceId,
						c.Content,
						c.Hash,
						c.FileName,
						c.ValidFrom,
						c.ExpiryDate
					});
				/// <summary>TODO</summary>
				return EntityDataReader(cert);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSCertificateByServiceId",
					/// <summary>TODO</summary>
					new SqlParameter("@ServiceId", serviceId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRdsCollectionSettingsByCollectionId(int collectionId)
		{
			if (UseEntityFramework)
			{
				var setting = RdsCollectionSettings
					.Where(s => s.RdsCollectionId == collectionId)
					.Take(1)
					.Select(s => new
					{
						s.Id,
						s.RdsCollectionId,
						s.DisconnectedSessionLimitMin,
						s.ActiveSessionLimitMin,
						s.IdleSessionLimitMin,
						s.BrokenConnectionAction,
						s.AutomaticReconnectionEnabled,
						s.TemporaryFoldersDeletedOnExit,
						s.TemporaryFoldersPerSession,
						s.ClientDeviceRedirectionOptions,
						s.ClientPrinterRedirected,
						s.ClientPrinterAsDefault,
						s.RdEasyPrintDriverEnabled,
						s.MaxRedirectedMonitors,
						s.SecurityLayer,
						s.EncryptionLevel,
						s.AuthenticateUsingNla
					});
				/// <summary>TODO</summary>
				return EntityDataReader(setting);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSCollectionSettingsByCollectionId",
					/// <summary>TODO</summary>
					new SqlParameter("@RDSCollectionID", collectionId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddRdsCollectionSettings(RdsCollectionSettings settings)
		{
			/// <summary>TODO</summary>
			return AddRdsCollectionSettings(settings.RdsCollectionId, settings.DisconnectedSessionLimitMin, settings.ActiveSessionLimitMin, settings.IdleSessionLimitMin, settings.BrokenConnectionAction,
				settings.AutomaticReconnectionEnabled, settings.TemporaryFoldersDeletedOnExit, settings.TemporaryFoldersPerSession, settings.ClientDeviceRedirectionOptions, settings.ClientPrinterRedirected,
				settings.ClientPrinterAsDefault, settings.RDEasyPrintDriverEnabled, settings.MaxRedirectedMonitors, settings.SecurityLayer, settings.EncryptionLevel, settings.AuthenticateUsingNLA);
		}
		/// <summary>Auto-generated member.</summary>

		private int AddRdsCollectionSettings(int rdsCollectionId, int disconnectedSessionLimitMin, int activeSessionLimitMin, int idleSessionLimitMin, string brokenConnectionAction,
			 bool automaticReconnectionEnabled, bool temporaryFoldersDeletedOnExit, bool temporaryFoldersPerSession, string clientDeviceRedirectionOptions, bool ClientPrinterRedirected,
			 bool clientPrinterAsDefault, bool rdEasyPrintDriverEnabled, int maxRedirectedMonitors, string SecurityLayer, string EncryptionLevel, bool AuthenticateUsingNLA)
		{
			if (UseEntityFramework)
			{
				var setting = new Data.Entities.RdsCollectionSetting()
				{
					RdsCollectionId = rdsCollectionId,
					DisconnectedSessionLimitMin = disconnectedSessionLimitMin,
					ActiveSessionLimitMin = activeSessionLimitMin,
					IdleSessionLimitMin = idleSessionLimitMin,
					BrokenConnectionAction = brokenConnectionAction,
					AutomaticReconnectionEnabled = automaticReconnectionEnabled,
					TemporaryFoldersDeletedOnExit = temporaryFoldersDeletedOnExit,
					TemporaryFoldersPerSession = temporaryFoldersPerSession,
					ClientDeviceRedirectionOptions = clientDeviceRedirectionOptions,
					ClientPrinterRedirected = ClientPrinterRedirected,
					ClientPrinterAsDefault = clientPrinterAsDefault,
					RdEasyPrintDriverEnabled = rdEasyPrintDriverEnabled,
					MaxRedirectedMonitors = maxRedirectedMonitors,
					SecurityLayer = SecurityLayer,
					EncryptionLevel = EncryptionLevel,
					AuthenticateUsingNla = AuthenticateUsingNLA
				};
				RdsCollectionSettings.Add(setting);
				SaveChanges();
				return setting.Id;
			}
			else
			{
				SqlParameter rdsCollectionSettingsId = new SqlParameter("@RDSCollectionSettingsID", SqlDbType.Int);
				rdsCollectionSettingsId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddRDSCollectionSettings",
					rdsCollectionSettingsId,
					/// <summary>TODO</summary>
					new SqlParameter("@RdsCollectionId", rdsCollectionId),
					/// <summary>TODO</summary>
					new SqlParameter("@DisconnectedSessionLimitMin", disconnectedSessionLimitMin),
					/// <summary>TODO</summary>
					new SqlParameter("@ActiveSessionLimitMin", activeSessionLimitMin),
					/// <summary>TODO</summary>
					new SqlParameter("@IdleSessionLimitMin", idleSessionLimitMin),
					/// <summary>TODO</summary>
					new SqlParameter("@BrokenConnectionAction", brokenConnectionAction),
					/// <summary>TODO</summary>
					new SqlParameter("@AutomaticReconnectionEnabled", automaticReconnectionEnabled),
					/// <summary>TODO</summary>
					new SqlParameter("@TemporaryFoldersDeletedOnExit", temporaryFoldersDeletedOnExit),
					/// <summary>TODO</summary>
					new SqlParameter("@TemporaryFoldersPerSession", temporaryFoldersPerSession),
					/// <summary>TODO</summary>
					new SqlParameter("@ClientDeviceRedirectionOptions", clientDeviceRedirectionOptions),
					/// <summary>TODO</summary>
					new SqlParameter("@ClientPrinterRedirected", ClientPrinterRedirected),
					/// <summary>TODO</summary>
					new SqlParameter("@ClientPrinterAsDefault", clientPrinterAsDefault),
					/// <summary>TODO</summary>
					new SqlParameter("@RDEasyPrintDriverEnabled", rdEasyPrintDriverEnabled),
					/// <summary>TODO</summary>
					new SqlParameter("@MaxRedirectedMonitors", maxRedirectedMonitors),
					/// <summary>TODO</summary>
					new SqlParameter("@SecurityLayer", SecurityLayer),
					/// <summary>TODO</summary>
					new SqlParameter("@EncryptionLevel", EncryptionLevel),
					/// <summary>TODO</summary>
					new SqlParameter("@AuthenticateUsingNLA", AuthenticateUsingNLA));

				return Convert.ToInt32(rdsCollectionSettingsId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateRDSCollectionSettings(RdsCollectionSettings settings)
		{
			UpdateRDSCollectionSettings(settings.Id, settings.RdsCollectionId, settings.DisconnectedSessionLimitMin, settings.ActiveSessionLimitMin, settings.IdleSessionLimitMin, settings.BrokenConnectionAction,
				settings.AutomaticReconnectionEnabled, settings.TemporaryFoldersDeletedOnExit, settings.TemporaryFoldersPerSession, settings.ClientDeviceRedirectionOptions, settings.ClientPrinterRedirected,
				settings.ClientPrinterAsDefault, settings.RDEasyPrintDriverEnabled, settings.MaxRedirectedMonitors, settings.SecurityLayer, settings.EncryptionLevel, settings.AuthenticateUsingNLA);
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateRDSCollectionSettings(int id, int rdsCollectionId, int disconnectedSessionLimitMin, int activeSessionLimitMin, int idleSessionLimitMin, string brokenConnectionAction,
			bool automaticReconnectionEnabled, bool temporaryFoldersDeletedOnExit, bool temporaryFoldersPerSession, string clientDeviceRedirectionOptions, bool ClientPrinterRedirected,
			bool clientPrinterAsDefault, bool rdEasyPrintDriverEnabled, int maxRedirectedMonitors, string SecurityLayer, string EncryptionLevel, bool AuthenticateUsingNLA)
		{
			if (UseEntityFramework)
			{
				var setting = RdsCollectionSettings
					.FirstOrDefault(s => s.Id == id);
				if (setting != null)
				{
					setting.RdsCollectionId = rdsCollectionId;
					setting.DisconnectedSessionLimitMin = disconnectedSessionLimitMin;
					setting.ActiveSessionLimitMin = activeSessionLimitMin;
					setting.IdleSessionLimitMin = idleSessionLimitMin;
					setting.BrokenConnectionAction = brokenConnectionAction;
					setting.AutomaticReconnectionEnabled = automaticReconnectionEnabled;
					setting.TemporaryFoldersDeletedOnExit = temporaryFoldersDeletedOnExit;
					setting.TemporaryFoldersPerSession = temporaryFoldersPerSession;
					setting.ClientDeviceRedirectionOptions = clientDeviceRedirectionOptions;
					setting.ClientPrinterRedirected = ClientPrinterRedirected;
					setting.ClientPrinterAsDefault = clientPrinterAsDefault;
					setting.RdEasyPrintDriverEnabled = rdEasyPrintDriverEnabled;
					setting.MaxRedirectedMonitors = maxRedirectedMonitors;
					setting.SecurityLayer = SecurityLayer;
					setting.EncryptionLevel = EncryptionLevel;
					setting.AuthenticateUsingNla = AuthenticateUsingNLA;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateRDSCollectionSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", id),
					/// <summary>TODO</summary>
					new SqlParameter("@RdsCollectionId", rdsCollectionId),
					/// <summary>TODO</summary>
					new SqlParameter("@DisconnectedSessionLimitMin", disconnectedSessionLimitMin),
					/// <summary>TODO</summary>
					new SqlParameter("@ActiveSessionLimitMin", activeSessionLimitMin),
					/// <summary>TODO</summary>
					new SqlParameter("@IdleSessionLimitMin", idleSessionLimitMin),
					/// <summary>TODO</summary>
					new SqlParameter("@BrokenConnectionAction", brokenConnectionAction),
					/// <summary>TODO</summary>
					new SqlParameter("@AutomaticReconnectionEnabled", automaticReconnectionEnabled),
					/// <summary>TODO</summary>
					new SqlParameter("@TemporaryFoldersDeletedOnExit", temporaryFoldersDeletedOnExit),
					/// <summary>TODO</summary>
					new SqlParameter("@TemporaryFoldersPerSession", temporaryFoldersPerSession),
					/// <summary>TODO</summary>
					new SqlParameter("@ClientDeviceRedirectionOptions", clientDeviceRedirectionOptions),
					/// <summary>TODO</summary>
					new SqlParameter("@ClientPrinterRedirected", ClientPrinterRedirected),
					/// <summary>TODO</summary>
					new SqlParameter("@ClientPrinterAsDefault", clientPrinterAsDefault),
					/// <summary>TODO</summary>
					new SqlParameter("@RDEasyPrintDriverEnabled", rdEasyPrintDriverEnabled),
					/// <summary>TODO</summary>
					new SqlParameter("@MaxRedirectedMonitors", maxRedirectedMonitors),
					/// <summary>TODO</summary>
					new SqlParameter("@SecurityLayer", SecurityLayer),
					/// <summary>TODO</summary>
					new SqlParameter("@EncryptionLevel", EncryptionLevel),
					/// <summary>TODO</summary>
					new SqlParameter("@AuthenticateUsingNLA", AuthenticateUsingNLA));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteRDSCollectionSettings(int id)
		{
			if (UseEntityFramework)
			{
				RdsCollectionSettings.Where(s => s.Id == id).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteRDSCollectionSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRDSCollectionsByItemId(int itemId)
		{
			if (UseEntityFramework)
			{
				var items = RdsCollections
					.Where(c => c.ItemId == itemId);
				/// <summary>TODO</summary>
				return EntityDataReader(items);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSCollectionsByItemId",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRDSCollectionByName(string name)
		{
			if (UseEntityFramework)
			{
				var items = RdsCollections
					.Where(c => c.DisplayName == name)
					.Take(1);
				/// <summary>TODO</summary>
				return EntityDataReader(items);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSCollectionByName",
					/// <summary>TODO</summary>
					new SqlParameter("@Name", name));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRDSCollectionById(int id)
		{
			if (UseEntityFramework)
			{
				var collection = RdsCollections
					.Where(c => c.Id == id)
					.Take(1);
				/// <summary>TODO</summary>
				return EntityDataReader(collection);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSCollectionById",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetRDSCollectionsPaged(int itemId, string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
		{
			if (UseEntityFramework)
			{
				var collections = RdsCollections
					.Where(c => c.ItemId == itemId)
					.Select(c => new
					{
						c.Id,
						c.ItemId,
						c.Name,
						c.Description,
						c.DisplayName
					});

				if (!string.IsNullOrEmpty(filterValue) && !string.IsNullOrEmpty(filterColumn))
				{
					collections = collections.Where(DynamicFunctions.ColumnLike(collections, filterColumn, filterValue));
				}

				var count = collections.Count();

				if (!string.IsNullOrEmpty(sortColumn)) collections = collections.OrderBy(ColumnName(sortColumn));

				collections = collections.Skip(startRow).Take(maximumRows);

				/// <summary>TODO</summary>
				return EntityDataSet(count, collections);
			}
			else
			{
				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSCollectionsPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@itemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddRDSCollection(int itemId, string name, string description, string displayName)
		{
			if (UseEntityFramework)
			{
				var collection = new Data.Entities.RdsCollection()
				{
					ItemId = itemId,
					Name = name,
					Description = description,
					DisplayName = displayName
				};
				RdsCollections.Add(collection);
				SaveChanges();
				return collection.Id;
			}
			else
			{
				SqlParameter rdsCollectionId = new SqlParameter("@RDSCollectionID", SqlDbType.Int);
				rdsCollectionId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddRDSCollection",
					rdsCollectionId,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@Name", name),
					/// <summary>TODO</summary>
					new SqlParameter("@Description", description),
					/// <summary>TODO</summary>
					new SqlParameter("@DisplayName", displayName));

				// read identity
				return Convert.ToInt32(rdsCollectionId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetOrganizationRdsUsersCount(int itemId)
		{
			if (UseEntityFramework)
			{
				return RdsCollectionUsers
					.Where(u => u.RdsCollection.ItemId == itemId)
					.Count();
			}
			else
			{
				SqlParameter count = new SqlParameter("@TotalNumber", SqlDbType.Int);
				count.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetOrganizationRdsUsersCount",
					count,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId));

				// read identity
				return Convert.ToInt32(count.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetOrganizationRdsCollectionsCount(int itemId)
		{
			if (UseEntityFramework)
			{
				return RdsCollections.Count(c => c.ItemId == itemId);
			}
			else
			{
				SqlParameter count = new SqlParameter("@TotalNumber", SqlDbType.Int);
				count.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetOrganizationRdsCollectionsCount",
					count,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId));

				// read identity
				return Convert.ToInt32(count.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetOrganizationRdsServersCount(int itemId)
		{
			if (UseEntityFramework)
			{
				return RdsServers.Count(s => s.ItemId == itemId);
			}
			else
			{
				SqlParameter count = new SqlParameter("@TotalNumber", SqlDbType.Int);
				count.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteDataset(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetOrganizationRdsServersCount",
					count,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId));

				// read identity
				return Convert.ToInt32(count.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateRDSCollection(RdsCollection collection)
		{
			UpdateRDSCollection(collection.Id, collection.ItemId, collection.Name, collection.Description, collection.DisplayName);
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateRDSCollection(int id, int itemId, string name, string description, string displayName)
		{
			if (UseEntityFramework)
			{
				var collection = RdsCollections
					.FirstOrDefault(c => c.Id == id);
				if (collection != null)
				{
					collection.ItemId = itemId;
					collection.Name = name;
					collection.Description = description;
					collection.DisplayName = displayName;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateRDSCollection",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", id),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@Name", name),
					/// <summary>TODO</summary>
					new SqlParameter("@Description", description),
					/// <summary>TODO</summary>
					new SqlParameter("@DisplayName", displayName));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteRDSServerSettings(int serverId)
		{
			if (UseEntityFramework)
			{
				RdsServerSettings.Where(s => s.RdsServerId == serverId).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteRDSServerSettings",
					/// <summary>TODO</summary>
					new SqlParameter("@ServerId", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteRDSCollection(int id)
		{
			if (UseEntityFramework)
			{
				RdsServers
					.Where(s => s.RdsCollectionId == id)
					.ExecuteUpdate(rc => new Data.Entities.RdsServer
					{
						RdsCollectionId = null
					});

				RdsCollections
					.Where(c => c.Id == id)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteRDSCollection",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddRDSServer(string name, string fqdName, string description, string controller)
		{
			if (UseEntityFramework)
			{
				var server = new Data.Entities.RdsServer()
				{
					Name = name,
					FqdName = fqdName,
					Description = description,
					Controller = int.Parse(controller)
				};
				RdsServers.Add(server);
				SaveChanges();
				return server.Id;
			}
			else
			{
				SqlParameter rdsServerId = new SqlParameter("@RDSServerID", SqlDbType.Int);
				rdsServerId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddRDSServer",
					rdsServerId,
					/// <summary>TODO</summary>
					new SqlParameter("@FqdName", fqdName),
					/// <summary>TODO</summary>
					new SqlParameter("@Name", name),
					/// <summary>TODO</summary>
					new SqlParameter("@Description", description),
					/// <summary>TODO</summary>
					new SqlParameter("@Controller", controller));

				// read identity
				return Convert.ToInt32(rdsServerId.Value);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRDSServersByItemId(int itemId)
		{
			if (UseEntityFramework)
			{
				var servers = RdsServers
					.Where(s => s.ItemId == itemId)
					.GroupJoin(ServiceItems, s => s.ItemId, i => i.ItemId, (s, i) => new
					{
						Server = s,
						Items = i
					})
					.SelectMany(s => s.Items.DefaultIfEmpty(), (s, i) => new
					{
						s.Server.Id,
						s.Server.ItemId,
						s.Server.Name,
						s.Server.FqdName,
						s.Server.Description,
						s.Server.RdsCollectionId,
						ItemName = i != null ? i.ItemName : null
					});
				/// <summary>TODO</summary>
				return EntityDataReader(servers);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSServersByItemId",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public DataSet GetRDSServersPaged(int? itemId, int? collectionId, string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows, string controller, bool ignoreItemId = false, bool ignoreRdsCollectionId = false)
		{
			if (UseEntityFramework)
			{
				var servers = RdsServers
					.Where(s => (ignoreItemId || s.ItemId == itemId) &&
						(ignoreRdsCollectionId || s.RdsCollectionId == collectionId))
					.GroupJoin(ServiceItems, s => s.ItemId, si => si.ItemId, (s, si) => new
					{
						Server = s,
						ServiceItems = si
					})
					.SelectMany(s => s.ServiceItems.DefaultIfEmpty(), (s, si) => new
					{
						s.Server,
						ItemName = si != null ? si.ItemName : null,
					})
					.GroupJoin(Services, s => s.Server.Controller, svc => svc.ServiceId, (s, svc) => new
					{
						s.Server,
						s.ItemName,
						Services = svc
					})
					.SelectMany(s => s.Services.DefaultIfEmpty(), (s, svc) => new
					{
						s.Server.Id,
						s.Server.ItemId,
						s.Server.Name,
						s.Server.FqdName,
						s.Server.Description,
						s.Server.RdsCollectionId,
						s.Server.ConnectionEnabled,
						s.Server.Controller,
						s.ItemName,
						ControllerName = svc != null ? svc.ServiceName : null,
						CollectionName = s.Server.RdsCollection != null ? s.Server.RdsCollection.Name : null
					});

				if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
				{
					servers = servers.Where(DynamicFunctions.ColumnLike(servers, filterColumn, filterValue));
				}

				var count = servers.Count();

				if (!string.IsNullOrEmpty(sortColumn))
				{
                    sortColumn = Regex.Replace(sortColumn, "^[A-Z]+\\.", "");
                    servers = servers.OrderBy(ColumnName(sortColumn));
				}

				servers = servers.Skip(startRow).Take(maximumRows);

				/// <summary>TODO</summary>
				return EntityDataSet(count, servers);
			}
			else
			{
				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSServersPaged",
					/// <summary>TODO</summary>
					new SqlParameter("@FilterColumn", VerifyColumnName(filterColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@FilterValue", VerifyColumnValue(filterValue)),
					/// <summary>TODO</summary>
					new SqlParameter("@SortColumn", VerifyColumnName(sortColumn)),
					/// <summary>TODO</summary>
					new SqlParameter("@startRow", startRow),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@RdsCollectionId", collectionId),
					/// <summary>TODO</summary>
					new SqlParameter("@IgnoreItemId", ignoreItemId),
					/// <summary>TODO</summary>
					new SqlParameter("@IgnoreRdsCollectionId", ignoreRdsCollectionId),
					/// <summary>TODO</summary>
					new SqlParameter("@maximumRows", maximumRows),
					/// <summary>TODO</summary>
					new SqlParameter("@Controller", controller));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRDSServerById(int id)
		{
			if (UseEntityFramework)
			{
				var server = RdsServers
					.Where(s => s.Id == id)
					.GroupJoin(ServiceItems, s => s.ItemId, si => si.ItemId, (s, si) => new
					{
						Server = s,
						Items = si,
						CollectionName = s.RdsCollection != null ? s.RdsCollection.Name : null
					})
					.SelectMany(s => s.Items.DefaultIfEmpty(), (s, si) => new
					{
						s.Server.Id,
						s.Server.ItemId,
						s.Server.Name,
						s.Server.FqdName,
						s.Server.Description,
						s.Server.RdsCollectionId,
						s.Server.ConnectionEnabled,
						ItemName = si != null ? si.ItemName : null,
						s.CollectionName
					})
					.Take(1);
				/// <summary>TODO</summary>
				return EntityDataReader(server);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSServerById",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRDSServersByCollectionId(int collectionId)
		{
			if (UseEntityFramework)
			{
				var servers = RdsServers
					.Where(s => s.RdsCollectionId == collectionId)
					.GroupJoin(ServiceItems, s => s.ItemId, si => si.ItemId, (s, si) => new
					{
						Server = s,
						Items = si
					})
					.SelectMany(s => s.Items.DefaultIfEmpty(), (s, si) => new
					{
						s.Server.Id,
						s.Server.ItemId,
						s.Server.Name,
						s.Server.FqdName,
						s.Server.Description,
						s.Server.RdsCollectionId,
						ItemName = si != null ? si.ItemName : null,
					});
				/// <summary>TODO</summary>
				return EntityDataReader(servers);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSServersByCollectionId",
					/// <summary>TODO</summary>
					new SqlParameter("@RdsCollectionId", collectionId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteRDSServer(int id)
		{
			if (UseEntityFramework)
			{
				RdsServers.Where(s => s.Id == id).ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteRDSServer",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateRDSServer(RdsServer server)
		{
			UpdateRDSServer(server.Id, server.ItemId, server.Name, server.FqdName, server.Description,
				 server.RdsCollectionId, server.ConnectionEnabled);
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateRDSServer(int id, int? itemId, string name, string fqdName, string description, int? rdsCollectionId, string connectionEnabled)
		{
			byte connEnabled = !String.IsNullOrEmpty(connectionEnabled) &&
				(connectionEnabled.Equals("false") || connectionEnabled.Equals("no") || connectionEnabled.Equals("0"))
				? (byte)0
				: (byte)1;

			if (UseEntityFramework)
			{
				var server = RdsServers
					.FirstOrDefault(s => s.Id == id);
				if (server != null)
				{
					server.ItemId = itemId;
					server.Name = name;
					server.FqdName = fqdName;
					server.Description = description;
					server.RdsCollectionId = rdsCollectionId;
					server.ConnectionEnabled = connEnabled == 1;
					SaveChanges();
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateRDSServer",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", id),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@Name", name),
					/// <summary>TODO</summary>
					new SqlParameter("@FqdName", fqdName),
					/// <summary>TODO</summary>
					new SqlParameter("@Description", description),
					/// <summary>TODO</summary>
					new SqlParameter("@RDSCollectionId", rdsCollectionId),
					/// <summary>TODO</summary>
					new SqlParameter("@ConnectionEnabled", connEnabled));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddRDSServerToCollection(int serverId, int rdsCollectionId)
		{
			if (UseEntityFramework)
			{
				RdsServers
					.Where(s => s.Id == serverId)
					.ExecuteUpdate(s => new Data.Entities.RdsServer
					{
						RdsCollectionId = rdsCollectionId
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddRDSServerToCollection",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@RDSCollectionId", rdsCollectionId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddRDSServerToOrganization(int itemId, int serverId)
		{
			if (UseEntityFramework)
			{
				RdsServers
					.Where(s => s.Id == serverId)
					.ExecuteUpdate(s => new Data.Entities.RdsServer
					{
						ItemId = itemId
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddRDSServerToOrganization",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", serverId),
					/// <summary>TODO</summary>
					new SqlParameter("@ItemID", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void RemoveRDSServerFromOrganization(int serverId)
		{
			if (UseEntityFramework)
			{
				RdsServers
					.Where(s => s.Id == serverId)
					.ExecuteUpdate(s => new Data.Entities.RdsServer
					{
						ItemId = null
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"RemoveRDSServerFromOrganization",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void RemoveRDSServerFromCollection(int serverId)
		{
			if (UseEntityFramework)
			{
				RdsServers
					.Where(s => s.Id == serverId)
					.ExecuteUpdate(s => new Data.Entities.RdsServer
					{
						RdsCollectionId = null
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"RemoveRDSServerFromCollection",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", serverId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetRDSCollectionUsersByRDSCollectionId(int id)
		{
			if (UseEntityFramework)
			{
				var users = ExchangeAccounts
					.Where(a => a.RdsCollectionUsers.Any(u => u.RdsCollectionId == id))
					.Select(a => new
					{
						a.AccountId,
						a.ItemId,
						a.AccountType,
						a.AccountName,
						a.DisplayName,
						a.PrimaryEmailAddress,
						a.MailEnabledPublicFolder,
						a.MailboxManagerActions,
						a.SamAccountName,
						a.CreatedDate,
						a.MailboxPlanId,
						a.SubscriberNumber,
						a.UserPrincipalName,
						a.ExchangeDisclaimerId,
						a.ArchivingMailboxPlanId,
						a.EnableArchiving,
						a.LevelId,
						a.IsVip
					});
				/// <summary>TODO</summary>
				return EntityDataReader(users);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSCollectionUsersByRDSCollectionId",
					/// <summary>TODO</summary>
					new SqlParameter("@id", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddRDSUserToRDSCollection(int rdsCollectionId, int accountId)
		{
			if (UseEntityFramework)
			{
				var user = new Data.Entities.RdsCollectionUser()
				{
					RdsCollectionId = rdsCollectionId,
					AccountId = accountId
				};
				RdsCollectionUsers.Add(user);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddUserToRDSCollection",
					/// <summary>TODO</summary>
					new SqlParameter("@RDSCollectionId", rdsCollectionId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void RemoveRDSUserFromRDSCollection(int rdsCollectionId, int accountId)
		{
			if (UseEntityFramework)
			{
				RdsCollectionUsers
					.Where(u => u.AccountId == accountId && u.RdsCollectionId == rdsCollectionId)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"RemoveRDSUserFromRDSCollection",
					/// <summary>TODO</summary>
					new SqlParameter("@RDSCollectionId", rdsCollectionId),
					/// <summary>TODO</summary>
					new SqlParameter("@AccountID", accountId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int GetRDSControllerServiceIDbyFQDN(string fqdnName)
		{
			if (UseEntityFramework)
			{
				return RdsServers
					.Where(s => s.FqdName == fqdnName)
					.Select(s => s.Controller)
					.FirstOrDefault() ?? 0;
			}
			else
			{
				SqlParameter prmController = new SqlParameter("@Controller", SqlDbType.Int);
				prmController.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(NativeConnectionString, CommandType.StoredProcedure,
					ObjectQualifier + "GetRDSControllerServiceIDbyFQDN",
					/// <summary>TODO</summary>
					new SqlParameter("@RdsfqdnName", fqdnName),
					prmController);

				return Convert.ToInt32(prmController.Value);
			}
		}

		#endregion

		#region MX|NX Services
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetAllPackages()
		{
			if (UseEntityFramework)
			{
				var packages = Packages
					.Select(p => new
					{
						p.PackageId,
						p.ParentPackageId,
						p.UserId,
						p.PackageName,
						p.PackageComments,
						p.ServerId,
						p.StatusId,
						p.PlanId,
						p.PurchaseDate,
						p.OverrideQuotas,
						p.BandwidthUpdated
					});
				/// <summary>TODO</summary>
				return EntityDataReader(packages);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetAllPackages");
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetDomainDnsRecords(int domainId, DnsRecordType recordType)
		{
			if (UseEntityFramework)
			{
				var records = DomainDnsRecords
					.Where(r => r.DomainId == domainId && r.RecordType == recordType)
					.Select(r => new
					{
						r.Id,
						r.DomainId,
						r.DnsServer,
						r.RecordType,
						r.Value,
						r.Date
					});
				/// <summary>TODO</summary>
				return EntityDataReader(records);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetDomainDnsRecords",
					/// <summary>TODO</summary>
					new SqlParameter("@DomainId", domainId),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordType", recordType));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetDomainAllDnsRecords(int domainId)
		{
			if (UseEntityFramework)
			{
				var records = DomainDnsRecords
					.Where(r => r.DomainId == domainId)
					.Select(r => new
					{
						r.Id,
						r.DomainId,
						r.DnsServer,
						r.RecordType,
						r.Value,
						r.Date
					});
				/// <summary>TODO</summary>
				return EntityDataReader(records);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetDomainAllDnsRecords",
					/// <summary>TODO</summary>
					new SqlParameter("@DomainId", domainId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void AddDomainDnsRecord(DnsRecordInfo domainDnsRecord)
		{
			if (UseEntityFramework)
			{
				var record = new Data.Entities.DomainDnsRecord()
				{
					DomainId = domainDnsRecord.Id,
					DnsServer = domainDnsRecord.DnsServer,
					RecordType = domainDnsRecord.RecordType,
					Value = domainDnsRecord.Value,
					Date = domainDnsRecord.Date
				};
				DomainDnsRecords.Add(record);
				SaveChanges();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddDomainDnsRecord",
					/// <summary>TODO</summary>
					new SqlParameter("@DomainId", domainDnsRecord.DomainId),
					/// <summary>TODO</summary>
					new SqlParameter("@RecordType", domainDnsRecord.RecordType),
					/// <summary>TODO</summary>
					new SqlParameter("@DnsServer", domainDnsRecord.DnsServer),
					/// <summary>TODO</summary>
					new SqlParameter("@Value", domainDnsRecord.Value),
					/// <summary>TODO</summary>
					new SqlParameter("@Date", domainDnsRecord.Date));
			}
		}

		/* Table ScheduleTasksEmailTemplates does not exist.
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetScheduleTaskEmailTemplate(string taskId)
		{
			if (UseEntityFramework)
			{
				var templates = ScheduleTasksEmailTemplates.
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetScheduleTaskEmailTemplate",
					/// <summary>TODO</summary>
					new SqlParameter("@taskId", taskId));
			}
		} */
		/// <summary>Auto-generated member.</summary>

		public void DeleteDomainDnsRecord(int id)
		{
			if (UseEntityFramework)
			{
				DomainDnsRecords
					.Where(r => r.Id == id)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteDomainDnsRecord",
					/// <summary>TODO</summary>
					new SqlParameter("@Id", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateDomainCreationDate(int domainId, DateTime date)
		{
			if (UseEntityFramework)
			{
				Domains
					.Where(d => d.DomainId == domainId)
					.ExecuteUpdate(d => new Data.Entities.Domain
					{
						CreationDate = date
					});
			}
			else
			{
				UpdateDomainDate(domainId, "UpdateDomainCreationDate", date);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateDomainExpirationDate(int domainId, DateTime date)
		{
			if (UseEntityFramework)
			{
				Domains
					.Where(d => d.DomainId == domainId)
					.ExecuteUpdate(d => new Data.Entities.Domain
					{
						ExpirationDate = date
					});
			}
			else
			{
				UpdateDomainDate(domainId, "UpdateDomainExpirationDate", date);
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateDomainLastUpdateDate(int domainId, DateTime date)
		{
			if (UseEntityFramework)
			{
				Domains
					.Where(d => d.DomainId == domainId)
					.ExecuteUpdate(d => new Data.Entities.Domain
					{
						LastUpdateDate = date
					});
			}
			else
			{
				UpdateDomainDate(domainId, "UpdateDomainLastUpdateDate", date);
			}
		}
		/// <summary>Auto-generated member.</summary>

		private void UpdateDomainDate(int domainId, string storedProcedure, DateTime date)
		{
			if (UseEntityFramework)
			{
				switch (storedProcedure)
				{
					case "UpdateDomainCreationDate":
						UpdateDomainCreationDate(domainId, date);
						break;
					case "UpdateDomainExpirationDate":
						UpdateDomainExpirationDate(domainId, date);
						break;
					case "UpdateDomainLastUpdateDate":
						UpdateDomainLastUpdateDate(domainId, date);
						break;
				}
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
				NativeConnectionString,
				CommandType.StoredProcedure,
				storedProcedure,
				/// <summary>TODO</summary>
				new SqlParameter("@DomainId", domainId),
				/// <summary>TODO</summary>
				new SqlParameter("@Date", date));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateDomainDates(int domainId, DateTime? domainCreationDate, DateTime? domainExpirationDate, DateTime? domainLastUpdateDate)
		{
			if (UseEntityFramework)
			{
				Domains
					.Where(d => d.DomainId == domainId)
					.ExecuteUpdate(d => new Data.Entities.Domain
					{
						CreationDate = domainCreationDate,
						ExpirationDate = domainExpirationDate,
						LastUpdateDate = domainLastUpdateDate
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateDomainDates",
					/// <summary>TODO</summary>
					new SqlParameter("@DomainId", domainId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainCreationDate", domainCreationDate),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainExpirationDate", domainExpirationDate),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainLastUpdateDate", domainLastUpdateDate));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void UpdateWhoisDomainInfo(int domainId, DateTime? domainCreationDate, DateTime? domainExpirationDate, DateTime? domainLastUpdateDate, string registrarName)
		{
			if (UseEntityFramework)
			{
				Domains
					.Where(d => d.DomainId == domainId)
					.ExecuteUpdate(d => new Data.Entities.Domain
					{
						CreationDate = domainCreationDate,
						ExpirationDate = domainExpirationDate,
						LastUpdateDate = domainLastUpdateDate,
						RegistrarName = registrarName
					});
			}
			else
			{
				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"UpdateWhoisDomainInfo",
					/// <summary>TODO</summary>
					new SqlParameter("@DomainId", domainId),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainCreationDate", domainCreationDate),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainExpirationDate", domainExpirationDate),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainLastUpdateDate", domainLastUpdateDate),
					/// <summary>TODO</summary>
					new SqlParameter("@DomainRegistrarName", registrarName));
			}
		}
		#endregion

		#region Organization Storage Space Folders
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetOrganizationStoragSpaceFolders(int itemId)
		{
			if (UseEntityFramework)
			{
				var folders = ExchangeOrganizationSsFolders
					.Where(e => e.ItemId == itemId)
					.Select(e => new
					{
						e.StorageSpaceFolder.Id,
						e.StorageSpaceFolder.Name,
						e.StorageSpaceFolder.StorageSpaceId,
						e.StorageSpaceFolder.Path,
						e.StorageSpaceFolder.UncPath,
						e.StorageSpaceFolder.IsShared,
						e.StorageSpaceFolder.FsrmQuotaType,
						e.StorageSpaceFolder.FsrmQuotaSizeBytes
					});
				/// <summary>TODO</summary>
				return EntityDataReader(folders);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetOrganizationStoragSpaceFolders",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public IDataReader GetOrganizationStoragSpacesFolderByType(int itemId, string type)
		{
			if (UseEntityFramework)
			{
				var folders = ExchangeOrganizationSsFolders
					.Where(e => e.ItemId == itemId && e.Type == type)
					.Select(e => new
					{
						e.StorageSpaceFolder.Id,
						e.StorageSpaceFolder.Name,
						e.StorageSpaceFolder.StorageSpaceId,
						e.StorageSpaceFolder.Path,
						e.StorageSpaceFolder.UncPath,
						e.StorageSpaceFolder.IsShared,
						e.StorageSpaceFolder.FsrmQuotaType,
						e.StorageSpaceFolder.FsrmQuotaSizeBytes
					});
				/// <summary>TODO</summary>
				return EntityDataReader(folders);
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetOrganizationStoragSpacesFolderByType",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@Type", type));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public void DeleteOrganizationStoragSpacesFolder(int id)
		{
			if (UseEntityFramework)
			{
				ExchangeOrganizationSsFolders
					.Where(f => f.StorageSpaceFolderId == id)
					.ExecuteDelete();
			}
			else
			{
				SqlHelper.ExecuteNonQuery(NativeConnectionString,
					CommandType.StoredProcedure,
					"DeleteOrganizationStoragSpacesFolder",
					/// <summary>TODO</summary>
					new SqlParameter("@ID", id));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddOrganizationStoragSpacesFolder(int itemId, string type, int storageSpaceFolderId)
		{
			if (UseEntityFramework)
			{
				var folder = new Data.Entities.ExchangeOrganizationSsFolder()
				{
					ItemId = itemId,
					Type = type,
					StorageSpaceFolderId = storageSpaceFolderId
				};
				ExchangeOrganizationSsFolders.Add(folder);
				SaveChanges();
				/// <summary>TODO</summary>
				return storageSpaceFolderId;
			}
			else
			{
				SqlParameter outParam = new SqlParameter("@ID", SqlDbType.Int);
				outParam.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddOrganizationStoragSpacesFolder",
					outParam,
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@Type", type),
					/// <summary>TODO</summary>
					new SqlParameter("@StorageSpaceFolderId", storageSpaceFolderId));

				return Convert.ToInt32(outParam.Value);
			}
		}

		/* Stored procedure is not in Database
		/// <summary>Auto-generated member.</summary>
		public IDataReader GetOrganizationStorageSpacesFolderById(int itemId, int folderId)
		{
			if (UseEntityFramework)
			{
			}
			else
			{
				return SqlHelper.ExecuteReader(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetOrganizationStorageSpacesFolderById",
					/// <summary>TODO</summary>
					new SqlParameter("@ItemId", itemId),
					/// <summary>TODO</summary>
					new SqlParameter("@ID", folderId));
			}
		}*/
		#endregion

		#region RDS Messages
		/// <summary>Auto-generated member.</summary>

		public DataSet GetRDSMessagesByCollectionId(int rdsCollectionId)
		{
			if (UseEntityFramework)
			{
				var messages = RdsMessages
					.Where(m => m.RdsCollectionId == rdsCollectionId)
					.Select(m => new { m.Id, m.RdsCollectionId, m.MessageText, m.UserName, m.Date });
				/// <summary>TODO</summary>
				return EntityDataSet(messages);
			}
			else
			{
				return SqlHelper.ExecuteDataset(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"GetRDSMessages",
					/// <summary>TODO</summary>
					new SqlParameter("@RDSCollectionId", rdsCollectionId));
			}
		}
		/// <summary>Auto-generated member.</summary>

		public int AddRDSMessage(int rdsCollectionId, string messageText, string userName)
		{
			if (UseEntityFramework)
			{
				var message = new Data.Entities.RdsMessage()
				{
					RdsCollectionId = rdsCollectionId,
					MessageText = messageText,
					UserName = userName,
					Date = DateTime.Now
				};
				RdsMessages.Add(message);
				SaveChanges();
				return message.Id;
			}
			else
			{
				SqlParameter rdsMessageId = new SqlParameter("@RDSMessageID", SqlDbType.Int);
				rdsMessageId.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(
					NativeConnectionString,
					CommandType.StoredProcedure,
					"AddRDSMessage",
					rdsMessageId,
					/// <summary>TODO</summary>
					new SqlParameter("@RDSCollectionId", rdsCollectionId),
					/// <summary>TODO</summary>
					new SqlParameter("@MessageText", messageText),
					/// <summary>TODO</summary>
					new SqlParameter("@UserName", userName),
					/// <summary>TODO</summary>
					new SqlParameter("@Date", DateTime.Now));

				return Convert.ToInt32(rdsMessageId.Value);
			}
		}

		#endregion

	}
}
