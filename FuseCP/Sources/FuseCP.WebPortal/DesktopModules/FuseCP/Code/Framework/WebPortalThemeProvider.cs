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
using System.Data;
using System.Collections.Generic;
using System.Text;

using FuseCP.WebPortal;
using System.Web;
using FuseCP.Portal;

namespace FuseCP.Portal
{
	public class WebPortalThemeProvider : PortalThemeProvider
	{
		public override string GetTheme()
		{
			string theme = PortalUtils.CurrentTheme;

			if(theme == "" | theme == null)
			{
				DataSet themedata = ES.Services.Authentication.GetLoginThemes();
                DataTable themeTable = (themedata != null && themedata.Tables.Count > 0) ? themedata.Tables[0] : null;
                if (themeTable == null || themeTable.Rows.Count == 0)
                    return theme;

				if (HttpContext.Current.Response.Cookies["UserRTL"] != null && HttpContext.Current.Response.Cookies["UserRTL"].Value == "1")
				{
					theme = themeTable.Rows[0]["RTLName"].ToString();
	
					HttpCookie cookieTheme = new HttpCookie("UserTheme", theme);
					cookieTheme.Expires = DateTime.Now.AddMonths(2);
					cookieTheme.HttpOnly = true;
					cookieTheme.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
					HttpContext.Current.Response.Cookies.Add(cookieTheme);
				}
				else
                {
					theme = themeTable.Rows[0]["LTRName"].ToString();
				}
			}

			return theme;
		}
	}
}
