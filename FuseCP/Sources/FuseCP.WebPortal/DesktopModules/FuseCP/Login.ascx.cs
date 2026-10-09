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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FuseCP.EnterpriseServer;
using FuseCP.Portal.WebAuthn;
using Microsoft.Extensions.DependencyInjection;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Models.Protocol.Json.AuthenticationCeremony.VerifyAssertion;
using WebAuthn.Net.Services.AuthenticationCeremony;
using WebAuthn.Net.Services.AuthenticationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.AuthenticationCeremony.Models.VerifyAssertion;
using FCP = FuseCP.EnterpriseServer;



namespace FuseCP.Portal
{
	public partial class Login : FuseCPModuleBase
	{
		private const string ThemeModeCookieName = "UserThemeMode";
		private const string ThemeModeAuto = "auto";
		private const string ThemeStyleLight = "light-theme";
		private const string ThemeStyleDark = "dark-theme";

		string ipAddress;
		//private IMessageBoxControl messageBox;     --- compile warning - never used

		private bool IsLocalUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false;
			}

			if (url.StartsWith("//", StringComparison.Ordinal) || url.StartsWith("\\\\", StringComparison.Ordinal) || url.StartsWith("/\\", StringComparison.Ordinal))
			{
				return false;
			}

			Uri absoluteUri;
			if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri))
			{
				return String.Equals(this.Request.Url.Host, absoluteUri.Host, StringComparison.OrdinalIgnoreCase)
					&& String.Equals(this.Request.Url.Scheme, absoluteUri.Scheme, StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				bool isLocal = url.StartsWith("/", StringComparison.Ordinal)
					&& !url.StartsWith("//", StringComparison.Ordinal)
					&& !url.StartsWith("/\\", StringComparison.Ordinal)
					&& Uri.IsWellFormedUriString(url, UriKind.Relative);
				return isLocal;
			}
		}

		private string RedirectUrl
		{
			get
			{
				return PortalUtils.LoginRedirectUrl;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			ApplyInputMetadata();

			if (!IsPostBack)
			{
				Page.RegisterAsyncTask(new PageAsyncTask(() => Task.WhenAll(
					EnsureFCPAAsync())));

				BindControls();
			}

			// capture Enter key
			//DotNetNuke.UI.Utilities.ClientAPI.RegisterKeyCapture(this.Parent, btnLogin, 13);

			// get user IP
			if (Request.UserHostAddress != null)
				ipAddress = Request.UserHostAddress;

			// update password control
			txtPassword.Attributes["value"] = txtPassword.Text;

			// autologin
			string usr = Request.QueryString["u"];
			if (String.IsNullOrEmpty(usr))
				usr = Request.QueryString["user"];

			string psw = Request.QueryString["p"];
			if (String.IsNullOrEmpty(psw))
				psw = Request.QueryString["pwd"];
			if (String.IsNullOrEmpty(psw))
				psw = Request.QueryString["password"];

			if (!String.IsNullOrEmpty(usr) && !String.IsNullOrEmpty(psw))
			{
				// perform login
				LoginUser(usr, psw, chkRemember.Checked, String.Empty, String.Empty);
			}
		}

		private void ApplyInputMetadata()
		{
			// Make login fields explicit for browser/extension autofill parsers.
			txtUsername.Attributes["autocomplete"] = "username";
			txtUsername.Attributes["aria-label"] = "Username";

			txtPassword.Attributes["autocomplete"] = "current-password";
			txtPassword.Attributes["aria-label"] = "Password";

			txtPin.Attributes["autocomplete"] = "one-time-code";
			txtPin.Attributes["aria-label"] = "One-time verification code";
		}

		private async Task EnsureFCPAAsync()
		{
			var enabledScpa = await ES.Services.Authentication.GetSystemSetupModeAsync().ConfigureAwait(false);
			//
			if (!(enabledScpa))
			{
				return;
			}
			//
			Response.Redirect(EditUrl("fcpa"), true);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (passkeyDiv != null && passkeyDiv.Visible)
			{
				RegisterPasskeyEventValidation();
			}

			base.Render(writer);
		}
		private void BindControls()
		{
			// load languages
			PortalUtils.LoadCultureDropDownList(ddlLanguage);
			BindThemeModes();

			string selectedThemeMode = ThemeModeAuto;
			HttpCookie modeCookie = Request.Cookies[ThemeModeCookieName];
			if (modeCookie != null)
			{
				string requestedMode = (modeCookie.Value ?? String.Empty).Trim().ToLowerInvariant();
				if (requestedMode == ThemeStyleDark || requestedMode == ThemeStyleLight)
				{
					selectedThemeMode = requestedMode;
				}
			}
			else
			{
				string currentThemeStyle = PortalUtils.CurrentThemeStyle;
				if (String.Equals(currentThemeStyle, ThemeStyleDark, StringComparison.OrdinalIgnoreCase))
				{
					selectedThemeMode = ThemeStyleDark;
				}
				else if (String.Equals(currentThemeStyle, ThemeStyleLight, StringComparison.OrdinalIgnoreCase))
				{
					selectedThemeMode = ThemeStyleLight;
				}
			}

			Utils.SelectListItem(ddlTheme, selectedThemeMode);

			// try to get the last login name from cookie
			HttpCookie cookie = Request.Cookies["FuseCPLogin"];
			if (cookie != null)
			{
				txtUsername.Text = cookie.Value;
				txtPassword.Focus();
			}
			else
			{
				// set focus on username field
				txtUsername.Focus();
			}

			if (PortalUtils.GetHideThemeAndLocale())
			{
				ddlLanguage.Visible = false;
				lblLanguage.Visible = false;
				ddlTheme.Visible = false;
				lblTheme.Visible = false;
			}
		}

		private void BindThemeModes()
		{
			ddlTheme.Items.Clear();
			ddlTheme.Items.Add(new ListItem("System", ThemeModeAuto));
			ddlTheme.Items.Add(new ListItem("Light", ThemeStyleLight));
			ddlTheme.Items.Add(new ListItem("Dark", ThemeStyleDark));
		}

		protected void cmdForgotPassword_Click(object sender, EventArgs e)
		{
			Response.Redirect(EditUrl("forgot_password"), true);
		}

		protected void btnLogin_Click(object sender, EventArgs e)
		{
			Page.Validate("LoginForm");

			// validate input
			if (!Page.IsValid)
				return;

			string username = txtUsername.Text.Trim();
			string password = txtPassword.Text;
			if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			// perform login
			LoginUser(username, password, chkRemember.Checked,
				 ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
		}

		protected void btnVerifyPin_Click(object sender, EventArgs e)
		{
			Page.Validate("PinForm");

			// validate input
			if (!Page.IsValid)
				return;

			if (String.IsNullOrWhiteSpace(txtPin.Text))
			{
				ShowErrorMessage("WrongPin");
				return;
			}

			// The user/password fields are not rendered during the PIN step
			// (their container is hidden), so txtUsername/txtPassword are empty
			// on this postback. The verified credentials are recovered from the
			// machine-key-protected ticket the login postback stored on the PIN
			// panel, and the PIN is validated against that username.
			var encryptedTicket = hdnPinTicket.Value;
			System.Web.Security.FormsAuthenticationTicket ticket = GetTicket(encryptedTicket);
			if (ticket == null)
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			string username = ticket.Name.Trim();
			string password = GetTicketPassword(ticket);

			var isValid = PortalUtils.ValidatePin(username, txtPin.Text);
			if (!isValid)
			{
				ShowErrorMessage("WrongPin");
				return;
			}

			// When the user reached the PIN step by falling back from the
			// passkey step (hdnPasskeyFallback set), the verification code is
			// the accepted second factor: complete the login without re-entering
			// the passkey challenge.
			if (RequiresPasskeyChallenge(username, password) && hdnPasskeyFallback.Value != "1")
			{
				ShowPasskeyStep(encryptedTicket);
				return;
			}

			PortalUtils.SetTicketAndCompleteLogin(encryptedTicket, username, chkRemember.Checked, ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
			hdnPinTicket.Value = String.Empty;
			hdnPasskeyFallback.Value = String.Empty;
			CompleteLogin(0);
		}


		private void LoginUser(string username, string password, bool rememberLogin,
			 string preferredLocale, string theme)
		{
			// status
			int loginStatus = PortalUtils.AuthenticateUser(username, password, ipAddress);

			if (loginStatus < 0)
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			string encryptedTicket = PortalUtils.CreateEncryptedAuthenticationTicket(username, password, ipAddress, rememberLogin, preferredLocale, theme);

			if (loginStatus == BusinessSuccessCodes.SUCCESS_USER_MFA_ACTIVE)
			{
				int mfaMode = PortalUtils.GetUserMfaMode(username, password, ipAddress);
				btnResendPin.Visible = mfaMode != 2;
				userPwdDiv.Visible = false;
				tokenDiv.Visible = true;
				hdnPinTicket.Value = encryptedTicket;
				txtPin.Focus();
				return;
			}

			if (RequiresPasskeyChallenge(username, password))
			{
				// The user/password fields are removed from the DOM for the
				// passkey step, so the verified credentials are carried by the
				// machine-key-protected ticket, the same way the MFA PIN step
				// carries its ticket.
				ShowPasskeyStep(encryptedTicket);
				return;
			}

			PortalUtils.SetTicketAndCompleteLogin(encryptedTicket, txtUsername.Text.Trim(), chkRemember.Checked, ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
			CompleteLogin(loginStatus);
		}

		private void CompleteLogin(int loginStatus)
		{
			// Access IP Settings
			FCP.SystemSettings settings = ES.Services.System.GetSystemSettings(FCP.SystemSettings.ACCESS_IP_SETTINGS);
			String AccessIps = String.Empty;
			String[] arAccessIps = null;
			if (settings != null)
			{
				AccessIps = settings.GetValueOrDefault(FCP.SystemSettings.ACCESS_IPs, string.Empty);
				arAccessIps = AccessIps.Split(new[] { ',', ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			}

			if (!String.IsNullOrEmpty(AccessIps))
			{
				String RequestIP = Request.ServerVariables["REMOTE_ADDR"];
				// String l_stSubnet = Knom.Helpers.Net.SubnetMask.ReturnSubnetmask(AccessIps);
				Boolean l_Mach = false;

				try
				{
					foreach (String l_AccessIP in arAccessIps!)
					{
						l_Mach = Knom.Helpers.Net.SubnetMask.IsInRange(RequestIP, l_AccessIP.Trim());
						if (l_Mach)
							break; // Once it passed then don't need to check for other access;
					}
				}
				catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
				{
				    _ = ex;
				}
				{ }
				if (!l_Mach)
				{
					PortalUtils.UserSignOutOnly();
					// messageBox.RenderMessage(MessageBoxType.Warning, "Unauthorized IP", "Unauthorized IP", null);
					ShowWarningMessage("IPAccessProhibited");
					return;
				}

			}

			if (loginStatus == BusinessSuccessCodes.SUCCESS_USER_ONETIMEPASSWORD)
			{
				// One time password should be changed after login
				Response.Redirect("Default.aspx?mid=1&ctl=change_onetimepassword&onetimepassword=true&UserID=" + PanelSecurity.LoggedUserId);
			}
			else
			{
				//Make Theme Cookies
				DataSet UserThemeSettingsData = ES.Services.Users.GetUserThemeSettings(PanelSecurity.LoggedUserId);
				if (UserThemeSettingsData.Tables.Count > 0)
				{
					foreach (DataRow row in UserThemeSettingsData.Tables[0].Rows)
					{
						string RowPropertyName = row.Field<String>("PropertyName");
						string RowPropertyValue = row.Field<String>("PropertyValue");

						if (RowPropertyName == "Style")
						{
							string UserThemeStyle = RowPropertyValue;

							HttpCookie UserThemeStyleCrumb = new HttpCookie("UserThemeStyle", UserThemeStyle);
							UserThemeStyleCrumb.HttpOnly = true;
							UserThemeStyleCrumb.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
							HttpContext.Current.Response.Cookies.Add(UserThemeStyleCrumb);

						}

						if (RowPropertyName == "palette-Light")
						{
							HttpCookie paletteLightCookie = new HttpCookie("UserThemePaletteLight", RowPropertyValue ?? string.Empty);
							paletteLightCookie.HttpOnly = true;
							paletteLightCookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
							HttpContext.Current.Response.Cookies.Add(paletteLightCookie);
						}

						if (RowPropertyName == "palette-Dark")
						{
							HttpCookie paletteDarkCookie = new HttpCookie("UserThemePaletteDark", RowPropertyValue ?? string.Empty);
							paletteDarkCookie.HttpOnly = true;
							paletteDarkCookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
							HttpContext.Current.Response.Cookies.Add(paletteDarkCookie);
						}

						if (RowPropertyName == "buttons-Light")
						{
							HttpCookie buttonsLightCookie = new HttpCookie("UserThemeButtonsLight", RowPropertyValue ?? string.Empty);
							buttonsLightCookie.HttpOnly = true;
							buttonsLightCookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
							HttpContext.Current.Response.Cookies.Add(buttonsLightCookie);
						}

						if (RowPropertyName == "buttons-Dark")
						{
							HttpCookie buttonsDarkCookie = new HttpCookie("UserThemeButtonsDark", RowPropertyValue ?? string.Empty);
							buttonsDarkCookie.HttpOnly = true;
							buttonsDarkCookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
							HttpContext.Current.Response.Cookies.Add(buttonsDarkCookie);
						}
					}
				}

				// redirect by shortcut
				ShortcutRedirect();

				// standard redirect
				string safeRedirectUrl = RedirectUrl;
				if (!IsLocalUrl(safeRedirectUrl))
					safeRedirectUrl = PortalUtils.LoginRedirectUrl;
				Response.Redirect(safeRedirectUrl, true);
			}
		}

		private void ShortcutRedirect()
		{
			if (PanelSecurity.EffectiveUser.Role == UserRole.Administrator)
				return; // not for administrators

			string shortcut = Request.QueryString["shortcut"];
			if ("vps".Equals(shortcut, StringComparison.InvariantCultureIgnoreCase))
			{
				// load hosting spaces
				PackageInfo[] packages = ES.Services.Packages.GetMyPackages(PanelSecurity.EffectiveUserId);
				if (packages.Length == 0)
					return; // no spaces - exit

				// check if some package has VPS resource enabled
				foreach (int packageId in packages.Select(package => package.PackageId))
				{
					PackageContext cntx = PackagesHelper.GetCachedPackageContext(packageId);
					if (cntx != null && cntx.Groups != null && cntx.Groups.ContainsKey(ResourceGroups.VPS))
					{
						// VPS resource found
						// check created VPS
						VirtualMachineMetaItemsPaged vms = ES.Services.VPS.GetVirtualMachines(packageId, "", "", "", 0, Int32.MaxValue, false);
						if (vms != null && vms.Items != null && vms.Items.Length == 1)
						{
							// one VPS - redirect to its properties screen
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPS", "SpaceID", packageId.ToString(),
								 "ItemID=" + vms.Items[0].ItemID, "ctl=vps_general", "moduleDefId=VPS"));
						}
						else
						{
							// several VPS - redirect to VPS list page
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPS", "SpaceID", packageId.ToString(),
								 "ctl=", "moduleDefId=VPS"));
						}
					}


					if (cntx != null && cntx.Groups != null && cntx.Groups.ContainsKey(ResourceGroups.VPS2012))
					{
						// VPS resource found
						// check created VPS
						VirtualMachineMetaItemsPaged vms = ES.Services.VPS2012.GetVirtualMachines(packageId, "", "", "", 0, Int32.MaxValue, false);
						if (vms != null && vms.Items != null && vms.Items.Length == 1)
						{
							// one VPS - redirect to its properties screen
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPS2012", "SpaceID", packageId.ToString(),
								 "ItemID=" + vms.Items[0].ItemID, "ctl=vps_general", "moduleDefId=VPS2012"));
						}
						else
						{
							// several VPS - redirect to VPS list page
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPS2012", "SpaceID", packageId.ToString(),
								 "ctl=", "moduleDefId=VPS2012"));
						}
					}


					if (cntx != null && cntx.Groups != null && cntx.Groups.ContainsKey(ResourceGroups.VPSForPC))
					{
						// VPS resource found
						// check created VPS
						VirtualMachineMetaItemsPaged vms = ES.Services.VPSPC.GetVirtualMachines(packageId, "", "", "", 0, Int32.MaxValue, false);
						if (vms != null && vms.Items != null && vms.Items.Length == 1)
						{
							// one VPS - redirect to its properties screen
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPSForPC", "SpaceID", packageId.ToString(),
								 "ItemID=" + vms.Items[0].ItemID, "ctl=vps_general", "moduleDefId=VPSForPC"));
						}
						else
						{
							// several VPS - redirect to VPS list page
							Response.Redirect(PortalUtils.NavigatePageURL("SpaceVPSForPC", "SpaceID", packageId.ToString(),
								 "ctl=", "moduleDefId=VPSForPC"));
						}
					}

				}

				// no VPS resources found
				// redirect to space home
				if (packages.Length == 1)
					Response.Redirect(PortalUtils.GetSpaceHomePageUrl(packages[0].PackageId));
			}
		}

		private void SetCurrentLanguage()
		{
			PortalUtils.SetCurrentLanguage(ddlLanguage.SelectedValue);
		}

		private void SetCurrentTheme()
		{
			string selectedMode = (ddlTheme.SelectedValue ?? String.Empty).Trim().ToLowerInvariant();
			if (selectedMode == ThemeModeAuto)
			{
				HttpCookie resetModeCookie = new HttpCookie(ThemeModeCookieName, String.Empty);
				resetModeCookie.Expires = DateTime.Now.AddMonths(-1);
				resetModeCookie.HttpOnly = true;
				resetModeCookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
				HttpContext.Current.Response.Cookies.Add(resetModeCookie);
				return;
			}

			if (!String.Equals(selectedMode, ThemeStyleDark, StringComparison.OrdinalIgnoreCase))
			{
				selectedMode = ThemeStyleLight;
			}

			HttpCookie modeCookie = new HttpCookie(ThemeModeCookieName, selectedMode);
			modeCookie.HttpOnly = true;
			modeCookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
			HttpContext.Current.Response.Cookies.Add(modeCookie);
		}

		protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetCurrentLanguage();
			SetCurrentTheme();

			string rawUrl = Request.RawUrl;
			if (IsLocalUrl(rawUrl))
				Response.Redirect(rawUrl);
		}

		protected void ddlTheme_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetCurrentTheme();
			string rawUrl = Request.RawUrl;
			if (IsLocalUrl(rawUrl))
				Response.Redirect(rawUrl);
		}

		protected void btnResendPin_Click(object sender, EventArgs e)
		{
			int sendPinResult = PortalUtils.SendPin(txtUsername.Text.Trim());

			if (sendPinResult == 0)
				ShowSuccessMessage("PinSend");
			else
				ShowErrorMessage("PinSendError");
		}

		//
		// Passkey sign-in: users with a registered passkey complete login with a
		// WebAuthn assertion on top of the already-verified password. The ceremony
		// services authenticate their EnterpriseServer calls with a ticket built
		// from the verified credentials, which is kept server-side only.
		//
		private bool RequiresPasskeyChallenge(string username, string password)
		{
			WebAuthnCredentialInfo[] credentials = PortalUtils.GetWebAuthnCredentials(username, password, ipAddress);
			return credentials != null && credentials.Length > 0;
		}

		private void ShowPasskeyStep(string encryptedTicket)
		{
			userPwdDiv.Visible = false;
			tokenDiv.Visible = false;
			hdnPasskeyOptions.Value = String.Empty;
			hdnPasskeyCeremonyId.Value = String.Empty;
			hdnPasskeyResponse.Value = String.Empty;
			hdnPasskeyTicket.Value = encryptedTicket;
			hdnPasskeyFallback.Value = String.Empty;
			passkeyDiv.Visible = true;

			// The passkey is the requested second factor. Offer the
			// verification-code method as a fallback only when the user has one
			// configured, so an unavailable passkey device does not lock the
			// user out.
			System.Web.Security.FormsAuthenticationTicket ticket = GetTicket(encryptedTicket);
			if (ticket == null)
			{
				passkeyFallbackDiv.Visible = false;
				return;
			}

			string username = ticket.Name.Trim();
			string password = GetTicketPassword(ticket);
			int mfaMode = PortalUtils.GetUserMfaMode(username, password, ipAddress);
			passkeyFallbackDiv.Visible = mfaMode > 0;
		}

		// The WebAuthn assertion is delivered to the server through a raw
		// __doPostBack from script. Unlike a native asp:Button, an
		// asp:LinkButton is not auto-registered for event validation, so the
		// postback is rejected unless the control is registered explicitly.
		// The compat layer only accepts the registration once the page has
		// reached the render phase (ControlState >= PreRendered), so it is
		// called from the Render override for every response that renders
		// the passkey step (the step's postback and the Begin response page).
		private void RegisterPasskeyEventValidation()
		{
			Page.ClientScript.RegisterForEventValidation(btnPasskey.UniqueID);
			if (btnPasskeyFallback != null)
			{
				Page.ClientScript.RegisterForEventValidation(btnPasskeyFallback.UniqueID);
			}
		}

		// The ticket's UserData carries "password + NewLine + role" (see
		// PortalUtils.CreateAuthTicket); only the password part is needed
		// here, and only server-side.
		private string GetTicketPassword(System.Web.Security.FormsAuthenticationTicket ticket)
		{
			string userData = ticket.UserData ?? String.Empty;
			return userData.Substring(0, userData.IndexOf(Environment.NewLine));
		}

		private System.Web.Security.FormsAuthenticationTicket GetTicket(string encryptedTicket)
		{
			try
			{
				return System.Web.Security.FormsAuthentication.Decrypt(encryptedTicket);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private System.Web.Security.FormsAuthenticationTicket GetPasskeyTicket()
		{
			return GetTicket(hdnPasskeyTicket.Value);
		}

		protected void btnPasskey_Click(object sender, EventArgs e)
		{
			string responseJson = hdnPasskeyResponse.Value;
			if (!String.IsNullOrEmpty(responseJson))
			{
				CompletePasskeySignin(responseJson);
				return;
			}

			// The user/password fields are not rendered on the passkey step, so
			// the verified credentials are recovered from the machine-key-
			// protected ticket stored by the step's postback.
			System.Web.Security.FormsAuthenticationTicket ticket = GetPasskeyTicket();
			if (ticket == null)
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			string username = ticket.Name.Trim();
			string password = GetTicketPassword(ticket);
			if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			if (PortalUtils.AuthenticateUser(username, password, ipAddress) < 0)
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			WebAuthnCredentialInfo[] credentials = PortalUtils.GetWebAuthnCredentials(username, password, ipAddress);
			if (credentials == null || credentials.Length == 0)
			{
				// No passkey registered: fall back to the standard login path.
				string encryptedTicket = PortalUtils.CreateEncryptedAuthenticationTicket(username, password, ipAddress, chkRemember.Checked, ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
				PortalUtils.SetTicketAndCompleteLogin(encryptedTicket, username, chkRemember.Checked, ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
				CompleteLogin(0);
				return;
			}

			BeginPasskeySignin(credentials, username, password);
		}

		// The user chose to sign in with a verification code instead of the
		// passkey. Return to the PIN step, carrying the machine-key-protected
		// ticket the passkey step holds, and set the fallback flag so that
		// verifying the code completes the login without re-entering the
		// passkey challenge (which would loop back to the passkey step).
		protected void btnPasskeyFallback_Click(object sender, EventArgs e)
		{
			string encryptedTicket = hdnPasskeyTicket.Value;
			if (String.IsNullOrEmpty(encryptedTicket))
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			// Re-showing the PIN panel after it was hidden for the passkey step
			// does not restore the child controls' visibility from the login
			// postback, so re-apply the resend-button rule that LoginUser used.
			System.Web.Security.FormsAuthenticationTicket ticket = GetTicket(encryptedTicket);
			if (ticket == null)
			{
				ShowWarningMessage("WrongLogin");
				return;
			}

			string username = ticket.Name.Trim();
			string password = GetTicketPassword(ticket);
			int mfaMode = PortalUtils.GetUserMfaMode(username, password, ipAddress);
			btnResendPin.Visible = mfaMode != 2;

			hdnPasskeyFallback.Value = "1";
			hdnPinTicket.Value = encryptedTicket;
			userPwdDiv.Visible = false;
			passkeyDiv.Visible = false;
			tokenDiv.Visible = true;
			txtPin.Focus();
		}

		private Microsoft.AspNetCore.Http.HttpContext GetCoreHttpContext()
		{
			try
			{
				return WebAuthnServices.Provider.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>().HttpContext;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void BeginPasskeySignin(WebAuthnCredentialInfo[] credentials, string username, string password)
		{
			Microsoft.AspNetCore.Http.HttpContext httpContext = GetCoreHttpContext();
			if (httpContext == null)
			{
				ShowPasskeyError("could not obtain the ASP.NET Core HTTP context (hosting not initialized) at Begin.");
				return;
			}

			string encryptedTicket = PortalUtils.CreateEncryptedAuthenticationTicket(username, password, ipAddress, false, String.Empty, String.Empty);
			httpContext.Items[FuseCPWebAuthnContext.TicketItemKey] = System.Web.Security.FormsAuthentication.Decrypt(encryptedTicket);

			// The portal may sit behind a TLS-terminating proxy, where Request.Scheme
			// reports the internal scheme (http) while the browser origin is the public
			// scheme (https). Allow both schemes for the same host so the ceremony is
			// not rejected by origin validation.
			Uri[] origins = new Uri[] { new Uri("https://" + httpContext.Request.Host.Value), new Uri("http://" + httpContext.Request.Host.Value) };
			byte[] userHandle = BitConverter.GetBytes(credentials[0].UserId);

			AuthenticationCeremonyPublicKeyCredentialDescriptor[] descriptors = new AuthenticationCeremonyPublicKeyCredentialDescriptor[credentials.Length];
			for (int i = 0; i < credentials.Length; i++)
			{
				descriptors[i] = new AuthenticationCeremonyPublicKeyCredentialDescriptor(PublicKeyCredentialType.PublicKey, credentials[i].CredentialId);
			}

			BeginAuthenticationCeremonyRequest request = new BeginAuthenticationCeremonyRequest(
				new AuthenticationCeremonyOriginParameters(origins),
				null,
				userHandle,
				32,
				60000,
				AuthenticationCeremonyIncludeCredentials.ManuallySpecified(descriptors),
				UserVerificationRequirement.Preferred,
				null,
				null);

			try
			{
				IAuthenticationCeremonyService ceremony = WebAuthnServices.Provider.GetRequiredService<IAuthenticationCeremonyService>();
				BeginAuthenticationCeremonyResult result = ceremony.BeginCeremonyAsync(httpContext, request, CancellationToken.None).GetAwaiter().GetResult();
				hdnPasskeyOptions.Value = JsonSerializer.Serialize(result.Options);
				hdnPasskeyCeremonyId.Value = result.AuthenticationCeremonyId;
				// Keep the step (and btnPasskey) rendered on the Begin response
				// page so the control is registered for event validation when the
				// script issues the Complete __doPostBack, and so the user can
				// retry by clicking the button if the assertion is cancelled.
				passkeyDiv.Visible = true;
				RegisterPasskeyScript();
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				System.Diagnostics.Trace.TraceError("WebAuthn sign-in (begin) threw an exception:\r\n" + ex
					+ "\r\n  " + GetOriginScheme(httpContext)
					+ "\r\n  " + DescribeCredentials("password-based (username/pwd)", SafePwdCreds(username, password)));
				ShowErrorMessage("WebAuthnLogin");
			}
		}

		private void CompletePasskeySignin(string responseJson)
		{
			string ceremonyId = hdnPasskeyCeremonyId.Value;
			hdnPasskeyOptions.Value = String.Empty;
			hdnPasskeyCeremonyId.Value = String.Empty;
			hdnPasskeyResponse.Value = String.Empty;

			if (String.IsNullOrEmpty(ceremonyId) || String.IsNullOrEmpty(responseJson))
			{
				ShowPasskeyError("the passkey ceremony state is missing (ceremonyId empty: "
					+ (String.IsNullOrEmpty(ceremonyId) ? "yes" : "no")
					+ "; browser response empty: "
					+ (String.IsNullOrEmpty(responseJson) ? "yes" : "no") + ").");
				return;
			}

			// The user/password fields are not rendered on the passkey step, so
			// the verified credentials are recovered from the machine-key-
			// protected ticket stored by the step's postback.
			System.Web.Security.FormsAuthenticationTicket ticket = GetPasskeyTicket();
			if (ticket == null)
			{
				ShowPasskeyError("the stored login ticket could not be read or decrypted (hdnPasskeyTicket empty or invalid).");
				return;
			}

			string username = ticket.Name.Trim();
			string password = GetTicketPassword(ticket);

			Microsoft.AspNetCore.Http.HttpContext httpContext = GetCoreHttpContext();
			if (httpContext == null)
			{
				ShowPasskeyError("could not obtain the ASP.NET Core HTTP context (hosting not initialized) at Complete for user " + username + ".");
				return;
			}

			string encryptedTicket = PortalUtils.CreateEncryptedAuthenticationTicket(username, password, ipAddress, false, String.Empty, String.Empty);
			httpContext.Items[FuseCPWebAuthnContext.TicketItemKey] = System.Web.Security.FormsAuthentication.Decrypt(encryptedTicket);

			try
			{
				AuthenticationResponseJSON response = JsonSerializer.Deserialize<AuthenticationResponseJSON>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				CompleteAuthenticationCeremonyRequest request = new CompleteAuthenticationCeremonyRequest(ceremonyId, response);

				IAuthenticationCeremonyService ceremony = WebAuthnServices.Provider.GetRequiredService<IAuthenticationCeremonyService>();
				var result = ceremony.CompleteCeremonyAsync(httpContext, request, CancellationToken.None).GetAwaiter().GetResult();

			if (result.HasError)
			{
				// WebAuthn.Net's Result<T> carries no failure reason. Log the deciding
				// context (credential state, origin/scheme, ceremony id) to the
				// server-side trace so the failing step is identifiable.
				string diagnostic = BuildPasskeyDiagnostic(ticket, username, password, httpContext, ceremonyId, response);
				System.Diagnostics.Trace.TraceError("WebAuthn sign-in failed (ceremony HasError):\n" + diagnostic);
				ShowErrorMessage("WebAuthnLogin");
				return;
			}

				// The assertion is verified against the stored public key; complete
				// the login with the ticket built from the verified credentials.
				PortalUtils.SetTicketAndCompleteLogin(encryptedTicket, username, chkRemember.Checked, ddlLanguage.SelectedValue, PortalUtils.CurrentTheme);
				CompleteLogin(0);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				System.Diagnostics.Trace.TraceError("WebAuthn login complete failed: " + ex);
				ShowErrorMessage("WebAuthnLogin");
			}
		}

		// Server-side diagnostic for a failed passkey sign-in: summarises the
		// credential state and the ceremony context so the failing step (empty
		// lookup, credentialId/rpId/userHandle mismatch, origin/rpId mismatch,
		// sign-count abort, or storage update failure) can be identified from the
		// application trace.
		private string BuildPasskeyDiagnostic(System.Web.Security.FormsAuthenticationTicket ticket, string username, string password,
			Microsoft.AspNetCore.Http.HttpContext httpContext, string ceremonyId, AuthenticationResponseJSON response)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Passkey sign-in failed. Diagnostic:\r\n");
			sb.Append("  user=" + username + "\r\n");
			sb.Append("  " + GetOriginScheme(httpContext) + "\r\n");
			sb.Append("  ceremonyId=" + (ceremonyId ?? String.Empty) + "\r\n");

			byte[] browserCredId = null;
			try
			{
				if (response != null && !String.IsNullOrEmpty(response.RawId))
					browserCredId = Base64UrlDecode(response.RawId);
			}
			catch (Exception) { /* ignore decode issues for diagnostics */ }
			sb.Append("  browser credentialId (rawId)=" + (browserCredId == null ? "(none)" : Convert.ToBase64String(browserCredId)) + "\r\n");

			sb.Append("  " + DescribeCredentials("ticket-based (SecurityContext)", SafeTicketCreds(ticket)) + "\r\n");
			sb.Append("  " + DescribeCredentials("password-based (username/pwd)", SafePwdCreds(username, password)) + "\r\n");

			return sb.ToString();
		}

		private WebAuthnCredentialInfo[] SafeTicketCreds(System.Web.Security.FormsAuthenticationTicket ticket)
		{
			try { return PortalUtils.GetMyWebAuthnCredentials(ticket); }
			catch (Exception ex) { return new WebAuthnCredentialInfo[] { new WebAuthnCredentialInfo { DisplayName = "LOOKUP THREW: " + ex.Message } }; }
		}

		private WebAuthnCredentialInfo[] SafePwdCreds(string username, string password)
		{
			try { return PortalUtils.GetWebAuthnCredentials(username, password, ipAddress); }
			catch (Exception ex) { return new WebAuthnCredentialInfo[] { new WebAuthnCredentialInfo { DisplayName = "LOOKUP THREW: " + ex.Message } }; }
		}

		private static string DescribeCredentials(string label, WebAuthnCredentialInfo[] creds)
		{
			if (creds == null)
				return label + " = null";
			if (creds.Length == 0)
				return label + " = EMPTY (0)";
			string items = String.Join("; ", creds
				.Where(c => c != null)
				.Select(c => (c.DisplayName != null && c.DisplayName.StartsWith("LOOKUP THREW", StringComparison.Ordinal))
					? c.DisplayName
					: "credId=" + (c.CredentialId == null ? "(null)" : Convert.ToBase64String(c.CredentialId))
						+ " rpId=" + (c.RpId ?? "(null)")
						+ " userHandle=" + (c.UserHandle == null ? "(null)" : Convert.ToBase64String(c.UserHandle))
						+ " signCount=" + c.SignCount.ToString()));
			return label + " = " + creds.Length.ToString() + " [" + items + "]";
		}

		private static byte[] Base64UrlDecode(string value)
		{
			string s = value.Replace('-', '+').Replace('_', '/');
			switch (s.Length % 4)
			{
				case 2: s += "=="; break;
				case 3: s += "="; break;
			}
			return Convert.FromBase64String(s);
		}

		// Host is a HostString struct, so the nullable-conditional chain
		// httpContext?.Request?.Host?.Value does not compile; guard explicitly.
		private static string GetOriginScheme(Microsoft.AspNetCore.Http.HttpContext httpContext)
		{
			if (httpContext == null || httpContext.Request == null)
				return "(no httpContext)";
			return "origin=" + (httpContext.Request.Host.Value ?? "(none)")
				+ "  scheme=" + (httpContext.Request.Scheme ?? "(none)");
		}

		private void ShowPasskeyError(string reason)
		{
			System.Diagnostics.Trace.TraceError("WebAuthn sign-in: " + reason);
			ShowErrorMessage("WebAuthnLogin");
		}

		private void RegisterPasskeyScript()
		{
			string js = "<script type=\"text/javascript\">(function(){"
				+ "function b64uToBuf(b){b=b.replace(/-/g,'+').replace(/_/g,'/');var p=(4-b.length%4)%4;var bin=atob(b+'===='.substring(0,p));var o=new Uint8Array(bin.length);for(var i=0;i<bin.length;i++)o[i]=bin.charCodeAt(i);return o.buffer;}"
				+ "function bufToB64u(b){var y=new Uint8Array(b);var s='';for(var i=0;i<y.length;i++)s+=String.fromCharCode(y[i]);return btoa(s).replace(/\\+/g,'-').replace(/\\//g,'_').replace(/=+$/,'');}"
				+ "var btn=document.getElementById('" + btnPasskey.ClientID + "');"
				+ "var opts=JSON.parse(document.getElementById('" + hdnPasskeyOptions.ClientID + "').value);"
				+ "if(!navigator.credentials||!navigator.credentials.get){if(btn){btn.disabled=false;}alert('WebAuthn is not supported in this browser.');return;}"
				+ "opts.challenge=b64uToBuf(opts.challenge);"
				+ "if(opts.allowCredentials){for(var j=0;j<opts.allowCredentials.length;j++)opts.allowCredentials[j].id=b64uToBuf(opts.allowCredentials[j].id);}"
				+ "function start(){navigator.credentials.get({publicKey:opts}).then(function(c){"
				+ "document.getElementById('" + hdnPasskeyResponse.ClientID + "').value=JSON.stringify({"
				+ "id:c.id,rawId:bufToB64u(c.rawId),type:c.type,authenticatorAttachment:c.authenticatorAttachment,clientExtensionResults:c.clientExtensionResults,"
				+ "response:{clientDataJSON:bufToB64u(c.response.clientDataJSON),authenticatorData:bufToB64u(c.response.authenticatorData),signature:bufToB64u(c.response.signature),userHandle:c.response.userHandle?bufToB64u(c.response.userHandle):null}"
				+ "});__doPostBack('" + btnPasskey.UniqueID + "','');"
				+ "}).catch(function(err){if(btn){btn.disabled=false;}alert('Passkey sign-in failed: '+(err&&err.message?err.message:err));});}"
				+ "if(document.hasFocus()){start();}"
				+ "else{var t=0;var iv=setInterval(function(){t++;if(document.hasFocus()||t>=50){clearInterval(iv);start();}},100);}"
				+ "})();</script>";
			Controls.Add(new System.Web.UI.LiteralControl(js));
		}
	}
}
