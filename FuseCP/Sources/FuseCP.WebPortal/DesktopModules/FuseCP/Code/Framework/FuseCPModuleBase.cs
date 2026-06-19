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
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public class FuseCPModuleBase : FuseCPControlBase
    {
        private IMessageBoxControl messageBox;
        private readonly object _msgLock = new object();

                public FuseCPModuleBase()
        {
        }

        /// <summary>
        /// Temporarily unlocks the Controls collection for modification during locked
        /// lifecycle phases (Init, Load, PreRender, etc.). The WebFormsForCore framework
        /// marks the collection read-only during these phases. This uses the same technique
        /// the framework itself uses internally (e.g., WebPartManager, Wizard).
        /// </summary>
        private static IDisposable UnlockControlCollection(ControlCollection collection)
        {
            if (collection == null || !collection.IsReadOnly)
                return null;

            var method = typeof(ControlCollection).GetMethod("SetCollectionReadOnly",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
                return null;

            string oldMsg = (string)method.Invoke(collection, new object[] { null });
            return new CollectionLockRestorer(collection, method, oldMsg);
        }

        private sealed class CollectionLockRestorer : IDisposable
        {
            private readonly ControlCollection _collection;
            private readonly MethodInfo _method;
            private readonly string _originalMsg;
            private bool _disposed;

            public CollectionLockRestorer(ControlCollection collection, MethodInfo method, string originalMsg)
            {
                _collection = collection;
                _method = method;
                _originalMsg = originalMsg;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _method.Invoke(_collection, new object[] { _originalMsg });
                }
            }
        }

        protected override void CreateChildControls()
        {
            // The WebFormsForCore framework locks Controls during Init/Load/PreRender/etc.
            // Temporarily unlock to add the MessageBox control, then restore the lock.
            using (UnlockControlCollection(this.Controls))
            {
                messageBox = (IMessageBoxControl)this.LoadControl(
                    PanelGlobals.FuseCPRootPath + "UserControls/MessageBox.ascx");
                this.Controls.AddAt(0, (Control)messageBox);
                ((Control)messageBox).Visible = false;
            }

            base.CreateChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            //Page.MaintainScrollPositionOnPostBack = true;

            // call base handler
            base.OnLoad(e);
        }

        public void SwitchUser(object arg)
        {
            //PanelSecurity.SelectedUserId = Utils.ParseInt(arg.ToString(), PanelSecurity.EffectiveUserId);
            RedirectToBrowsePage();
        }

        public void SwitchPackage(object arg)
        {
            //PanelSecurity.SelectedUserId = Utils.ParseInt(args[0], PanelSecurity.EffectiveUserId);
            //PanelSecurity.PackageId = Utils.ParseInt(args[1], 0);
            RedirectToBrowsePage();
        }

        public void LoadProviderControl(int packageId, string groupName, PlaceHolder container, string controlName)
        {
            string ctrlPath = null;
            //
            ProviderInfo provider = ES.Services.Servers.GetPackageServiceProvider(packageId, groupName);

            // try to locate suitable control
            string currPath = this.AppRelativeVirtualPath;
            currPath = currPath.Substring(0, currPath.LastIndexOf("/"));

            ctrlPath = currPath + "/ProviderControls/" + provider.EditorControl + "_" + controlName;

            Control ctrl = Page.LoadControl(ctrlPath);

            // add control to the placeholder
            container.Controls.Add(ctrl);
        }

        public void HideServiceColumns(GridView gv)
        {
            try
            {
                gv.Columns[gv.Columns.Count - 1].Visible =
                    (PanelSecurity.EffectiveUser.Role == UserRole.Administrator);
            }
            catch (System.Exception swallowedEx) when (!(swallowedEx is System.OutOfMemoryException) && !(swallowedEx is System.StackOverflowException) && !(swallowedEx is System.AccessViolationException))
            {
                System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
            }
        }

        #region Error Messages Processing
        public void ProcessException(Exception ex)
        {
            string authError = "The security token could not be authenticated or authorized";
            if (ex.Message.Contains(authError) ||
                (ex.InnerException != null &&
                ex.InnerException.Message.Contains(authError)))
            {
                ShowWarningMessage("ES_CONNECT");
            }
            else
            {
                ShowErrorMessage("MODULE_LOAD", ex);
            }

        }

        public virtual void ShowResultMessage(int resultCode)
        {
            ShowResultMessage(Utils.ModuleName, resultCode, false);
        }

        public virtual void ShowResultMessageWithContactForm(int resultCode)
        {
            ShowResultMessage(Utils.ModuleName, resultCode, true);
        }

        public void ShowResultMessage(string moduleName, int resultCode, params object[] formatArgs)
        {
            ShowResultMessage(moduleName, resultCode, false, formatArgs);
        }

        public void ShowResultMessage(string moduleName, int resultCode, bool showcf, params object[] formatArgs)
                {
                    EnsureChildControls();
                    lock (_msgLock)
                    {
                        MessageBoxType messageType = MessageBoxType.Warning;

                // try to get warning
                string sCode = Convert.ToString(resultCode * -1);
                string localizedMessage = GetSharedLocalizedString(moduleName, "Warning." + sCode);
                string localizedDescription = GetSharedLocalizedString(moduleName, "WarningDescription." + sCode);

                if (localizedMessage == null)
                {
                    messageType = MessageBoxType.Error;

                    // try to get error
                    localizedMessage = GetSharedLocalizedString(moduleName, "Error." + sCode);
                    localizedDescription = GetSharedLocalizedString(moduleName, "ErrorDescription." + sCode);

                    if (localizedMessage == null)
                    {
                        localizedMessage = GetSharedLocalizedString(moduleName, "Message.Generic") + " " + resultCode;
                    }
                    else
                    {
                        if (formatArgs != null && formatArgs.Length > 0)
                            localizedMessage = String.Format(localizedMessage, formatArgs);
                    }
                }

                // check if this is a "demo" message and it is overriden
                if (resultCode == BusinessErrorCodes.ERROR_USER_ACCOUNT_DEMO)
                {
                    UserSettings fcpSettings = UsersHelper.GetCachedUserSettings(
                        PanelSecurity.EffectiveUserId, UserSettings.FuseCP_POLICY);
                    if (!String.IsNullOrEmpty(fcpSettings["DemoMessage"]))
                    {
                        localizedDescription = fcpSettings["DemoMessage"];
                    }
                }

                // render message
                Exception fake_ex = null;
                // Contact form is requested to be shown
                if (showcf)
                    fake_ex = new Exception();
                //
                messageBox.RenderMessage(messageType, localizedMessage, localizedDescription, fake_ex);
            }
        }

        public virtual void ShowSuccessMessage(string messageKey)
        {
            ShowSuccessMessage(Utils.ModuleName, messageKey, null);
        }

        public void ShowSuccessMessage(string moduleName, string messageKey)
        {
            ShowSuccessMessage(moduleName, messageKey, null);
        }

        public virtual void ShowSuccessMessage(string moduleName, string messageKey, params string[] formatArgs)
        {
            lock (_msgLock)
            {
                string localizedMessage = GetSharedLocalizedString(moduleName, "Success." + messageKey);
                string localizedDescription = GetSharedLocalizedString(moduleName, "SuccessDescription." + messageKey);
                if (localizedMessage == null)
                {
                    localizedMessage = messageKey;
                }
                else
                {
                    //Format message string with args
                    if (formatArgs != null && formatArgs.Length > 0)
                    {
                        localizedMessage = String.Format(localizedMessage, formatArgs);
                    }
                }
                // render message
                messageBox.RenderMessage(MessageBoxType.Information, localizedMessage, localizedDescription, null);
            }
        }

        public virtual void ShowWarningMessage(string messageKey)
        {
            ShowWarningMessage(Utils.ModuleName, messageKey);
        }

        public void ShowWarningMessage(string moduleName, string messageKey)
        {
            lock (_msgLock)
            {
                string localizedMessage = GetSharedLocalizedString(moduleName, "Warning." + messageKey);
                string localizedDescription = GetSharedLocalizedString(moduleName, "WarningDescription." + messageKey);
                if (localizedMessage == null)
                    localizedMessage = messageKey;

                // render message
                messageBox.RenderMessage(MessageBoxType.Warning, localizedMessage, localizedDescription, null);
            }
        }

        public void ShowErrorMessage(string messageKey, params string[] additionalParameters)
        {
            ShowErrorMessage(messageKey, null, additionalParameters);
        }

        public virtual void ShowErrorMessage(string messageKey, Exception ex, params string[] additionalParameters)
        {
            ShowErrorMessage(Utils.ModuleName, messageKey, ex, additionalParameters);
        }

        public void ShowErrorMessage(string moduleName, string messageKey, Exception ex, params string[] additionalParameters)
        {
            lock (_msgLock)
            {
                string exceptionKey = null;
                string[] messageParts = null;
                //
                if (ex != null && !String.IsNullOrEmpty(ex.Message) && ex.Message.Contains("FuseCP_ERROR"))
                {
                    messageParts = ex.Message.Split(':');
                    if (messageParts.Length > 1)
                        exceptionKey = messageParts[1].TrimStart(new char[] { ' ' });
                }
                string localizedMessage = GetSharedLocalizedString(moduleName, "Error." + exceptionKey);
                string localizedDescription = GetSharedLocalizedString(moduleName, "ErrorDescription." + exceptionKey);

                if (localizedMessage == null)
                {
                    localizedMessage = GetSharedLocalizedString(moduleName, "Error." + messageKey);
                    localizedDescription = GetSharedLocalizedString(moduleName, messageKey);
                    if (localizedMessage == null)
                        localizedMessage = messageKey;
                }
                else
                {
                    // Preserve exception details for diagnostics while keeping localized text.
                    messageBox.RenderMessage(MessageBoxType.Error, localizedMessage, localizedDescription, ex);
                    return;
                }

                // render message
                messageBox.RenderMessage(MessageBoxType.Error, localizedMessage, localizedDescription, ex);
            }
        }

        #endregion
    }
}
