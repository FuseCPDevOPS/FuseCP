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

namespace FuseCP.Providers.FTP.CerberusFTP6Proxy
{
    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "CerberusFTPServiceSoapBinding", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AuthenticatedRequest))]
    /// <summary>TODO</summary>
    public partial class CerberusFTPService : Microsoft.Web.Services3.WebServicesClientProtocol
    {
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetBackupServersOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SaveBackupServersOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SharePublicFileOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback AddIpOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback DeleteIpOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback TestAndVerifyDatabaseOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback CreateStatisticsDatabaseOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback DropStatisticsDatabaseOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetMimeMappingsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SaveMimeMappingsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback ServerSummaryStatusOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback ServerInformationOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback CurrentStatusOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback StartServerOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback StopServerOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback ServerStartedOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback InitializeServerOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback ShutdownServerOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetEventRulesOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SetEventRulesOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback DeleteRequestedAccountsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetRequestedAccountsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SetRequestedAccountsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetAuthenticationListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SetAuthenticationListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetHostnameOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SetWANIPOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback AddUserOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback AddGroupOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback DeleteUserOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback DeleteGroupOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback AddRootOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback DeleteRootOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetUserListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetGroupListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetUserInformationOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetConnectedUserListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback ChangePasswordOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback RenameUserOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback TerminateConnectionOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetProfilesOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetGroupsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetConfigurationOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetInterfacesOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetIPBlockListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetAutoBlockListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetAppPathsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetLicenseInfoOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback VerifyLicenseOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetCurrentConnectionCountOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetAllCurrentConnectionCountOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetInterfaceByIDOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetInterfaceListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback InitializeInterfaceOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback ShutdownInterfaceOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetStatisticsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetCurrentBandwidthOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetFeaturesOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SaveProfilesOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SaveConfigurationOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback CommitSettingsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback SaveBlockListOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback ModifyInterfaceOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback ShutdownConnectionsOnInterfaceOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetFileTransfersOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GetLogMessagesOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback BlockAddressOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback GenerateStatisticsOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback BackupServerConfigurationOperationCompleted;
        /// <summary>Auto-generated member.</summary>

        private System.Threading.SendOrPostCallback RestoreServerConfigurationOperationCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public CerberusFTPService()
        {
            this.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            this.Url = "http://localhost:10001/service/cerberusftpservice";
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetBackupServersCompletedEventHandler GetBackupServersCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SaveBackupServersCompletedEventHandler SaveBackupServersCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SharePublicFileCompletedEventHandler SharePublicFileCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event AddIpCompletedEventHandler AddIpCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event DeleteIpCompletedEventHandler DeleteIpCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event TestAndVerifyDatabaseCompletedEventHandler TestAndVerifyDatabaseCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event CreateStatisticsDatabaseCompletedEventHandler CreateStatisticsDatabaseCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event DropStatisticsDatabaseCompletedEventHandler DropStatisticsDatabaseCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetMimeMappingsCompletedEventHandler GetMimeMappingsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SaveMimeMappingsCompletedEventHandler SaveMimeMappingsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event ServerSummaryStatusCompletedEventHandler ServerSummaryStatusCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event ServerInformationCompletedEventHandler ServerInformationCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event CurrentStatusCompletedEventHandler CurrentStatusCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event StartServerCompletedEventHandler StartServerCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event StopServerCompletedEventHandler StopServerCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event ServerStartedCompletedEventHandler ServerStartedCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event InitializeServerCompletedEventHandler InitializeServerCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event ShutdownServerCompletedEventHandler ShutdownServerCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetEventRulesCompletedEventHandler GetEventRulesCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SetEventRulesCompletedEventHandler SetEventRulesCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event DeleteRequestedAccountsCompletedEventHandler DeleteRequestedAccountsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetRequestedAccountsCompletedEventHandler GetRequestedAccountsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SetRequestedAccountsCompletedEventHandler SetRequestedAccountsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetAuthenticationListCompletedEventHandler GetAuthenticationListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SetAuthenticationListCompletedEventHandler SetAuthenticationListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetHostnameCompletedEventHandler GetHostnameCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SetWANIPCompletedEventHandler SetWANIPCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event AddUserCompletedEventHandler AddUserCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event AddGroupCompletedEventHandler AddGroupCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event DeleteUserCompletedEventHandler DeleteUserCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event DeleteGroupCompletedEventHandler DeleteGroupCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event AddRootCompletedEventHandler AddRootCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event DeleteRootCompletedEventHandler DeleteRootCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetUserListCompletedEventHandler GetUserListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetGroupListCompletedEventHandler GetGroupListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetUserInformationCompletedEventHandler GetUserInformationCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetConnectedUserListCompletedEventHandler GetConnectedUserListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event ChangePasswordCompletedEventHandler ChangePasswordCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event RenameUserCompletedEventHandler RenameUserCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event TerminateConnectionCompletedEventHandler TerminateConnectionCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetProfilesCompletedEventHandler GetProfilesCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetGroupsCompletedEventHandler GetGroupsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetConfigurationCompletedEventHandler GetConfigurationCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetInterfacesCompletedEventHandler GetInterfacesCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetIPBlockListCompletedEventHandler GetIPBlockListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetAutoBlockListCompletedEventHandler GetAutoBlockListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetAppPathsCompletedEventHandler GetAppPathsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetLicenseInfoCompletedEventHandler GetLicenseInfoCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event VerifyLicenseCompletedEventHandler VerifyLicenseCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetCurrentConnectionCountCompletedEventHandler GetCurrentConnectionCountCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetAllCurrentConnectionCountCompletedEventHandler GetAllCurrentConnectionCountCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetInterfaceByIDCompletedEventHandler GetInterfaceByIDCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetInterfaceListCompletedEventHandler GetInterfaceListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event InitializeInterfaceCompletedEventHandler InitializeInterfaceCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event ShutdownInterfaceCompletedEventHandler ShutdownInterfaceCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetStatisticsCompletedEventHandler GetStatisticsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetCurrentBandwidthCompletedEventHandler GetCurrentBandwidthCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetFeaturesCompletedEventHandler GetFeaturesCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SaveProfilesCompletedEventHandler SaveProfilesCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SaveConfigurationCompletedEventHandler SaveConfigurationCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event CommitSettingsCompletedEventHandler CommitSettingsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event SaveBlockListCompletedEventHandler SaveBlockListCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event ModifyInterfaceCompletedEventHandler ModifyInterfaceCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event ShutdownConnectionsOnInterfaceCompletedEventHandler ShutdownConnectionsOnInterfaceCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetFileTransfersCompletedEventHandler GetFileTransfersCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GetLogMessagesCompletedEventHandler GetLogMessagesCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event BlockAddressCompletedEventHandler BlockAddressCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event GenerateStatisticsCompletedEventHandler GenerateStatisticsCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event BackupServerConfigurationCompletedEventHandler BackupServerConfigurationCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public event RestoreServerConfigurationCompletedEventHandler RestoreServerConfigurationCompleted;

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetBackupServers", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetBackupServersResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetBackupServersResponse GetBackupServers([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetBackupServersRequest GetBackupServersRequest)
        {
            object[] results = this.Invoke("GetBackupServers", new object[] {
                        GetBackupServersRequest});
            return ((GetBackupServersResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetBackupServers(GetBackupServersRequest GetBackupServersRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetBackupServers", new object[] {
                        GetBackupServersRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetBackupServersResponse EndGetBackupServers(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetBackupServersResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetBackupServersAsync(GetBackupServersRequest GetBackupServersRequest)
        {
            this.GetBackupServersAsync(GetBackupServersRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetBackupServersAsync(GetBackupServersRequest GetBackupServersRequest, object userState)
        {
            if ((this.GetBackupServersOperationCompleted == null))
            {
                this.GetBackupServersOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetBackupServersOperationCompleted);
            }
            this.InvokeAsync("GetBackupServers", new object[] {
                        GetBackupServersRequest}, this.GetBackupServersOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetBackupServersOperationCompleted(object arg)
        {
            if ((this.GetBackupServersCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetBackupServersCompleted(this, new GetBackupServersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SaveBackupServers", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SaveBackupServersResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SaveBackupServersResponse SaveBackupServers([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SaveBackupServersRequest SaveBackupServersRequest)
        {
            object[] results = this.Invoke("SaveBackupServers", new object[] {
                        SaveBackupServersRequest});
            return ((SaveBackupServersResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSaveBackupServers(SaveBackupServersRequest SaveBackupServersRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SaveBackupServers", new object[] {
                        SaveBackupServersRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SaveBackupServersResponse EndSaveBackupServers(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SaveBackupServersResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveBackupServersAsync(SaveBackupServersRequest SaveBackupServersRequest)
        {
            this.SaveBackupServersAsync(SaveBackupServersRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveBackupServersAsync(SaveBackupServersRequest SaveBackupServersRequest, object userState)
        {
            if ((this.SaveBackupServersOperationCompleted == null))
            {
                this.SaveBackupServersOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSaveBackupServersOperationCompleted);
            }
            this.InvokeAsync("SaveBackupServers", new object[] {
                        SaveBackupServersRequest}, this.SaveBackupServersOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSaveBackupServersOperationCompleted(object arg)
        {
            if ((this.SaveBackupServersCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SaveBackupServersCompleted(this, new SaveBackupServersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SharePublicFile", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SharePublicFileResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SharePublicFileResponse SharePublicFile([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SharePublicFileRequest SharePublicFileRequest)
        {
            object[] results = this.Invoke("SharePublicFile", new object[] {
                        SharePublicFileRequest});
            return ((SharePublicFileResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSharePublicFile(SharePublicFileRequest SharePublicFileRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SharePublicFile", new object[] {
                        SharePublicFileRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SharePublicFileResponse EndSharePublicFile(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SharePublicFileResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SharePublicFileAsync(SharePublicFileRequest SharePublicFileRequest)
        {
            this.SharePublicFileAsync(SharePublicFileRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SharePublicFileAsync(SharePublicFileRequest SharePublicFileRequest, object userState)
        {
            if ((this.SharePublicFileOperationCompleted == null))
            {
                this.SharePublicFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSharePublicFileOperationCompleted);
            }
            this.InvokeAsync("SharePublicFile", new object[] {
                        SharePublicFileRequest}, this.SharePublicFileOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSharePublicFileOperationCompleted(object arg)
        {
            if ((this.SharePublicFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SharePublicFileCompleted(this, new SharePublicFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/AddIp", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("AddIpResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public AddIpResponse AddIp([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] AddIpRequest AddIpRequest)
        {
            object[] results = this.Invoke("AddIp", new object[] {
                        AddIpRequest});
            return ((AddIpResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginAddIp(AddIpRequest AddIpRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddIp", new object[] {
                        AddIpRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public AddIpResponse EndAddIp(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((AddIpResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void AddIpAsync(AddIpRequest AddIpRequest)
        {
            this.AddIpAsync(AddIpRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void AddIpAsync(AddIpRequest AddIpRequest, object userState)
        {
            if ((this.AddIpOperationCompleted == null))
            {
                this.AddIpOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddIpOperationCompleted);
            }
            this.InvokeAsync("AddIp", new object[] {
                        AddIpRequest}, this.AddIpOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnAddIpOperationCompleted(object arg)
        {
            if ((this.AddIpCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddIpCompleted(this, new AddIpCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/DeleteIp", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("DeleteIpResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public DeleteIpResponse DeleteIp([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] DeleteIpRequest DeleteIpRequest)
        {
            object[] results = this.Invoke("DeleteIp", new object[] {
                        DeleteIpRequest});
            return ((DeleteIpResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginDeleteIp(DeleteIpRequest DeleteIpRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteIp", new object[] {
                        DeleteIpRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public DeleteIpResponse EndDeleteIp(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeleteIpResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteIpAsync(DeleteIpRequest DeleteIpRequest)
        {
            this.DeleteIpAsync(DeleteIpRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteIpAsync(DeleteIpRequest DeleteIpRequest, object userState)
        {
            if ((this.DeleteIpOperationCompleted == null))
            {
                this.DeleteIpOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteIpOperationCompleted);
            }
            this.InvokeAsync("DeleteIp", new object[] {
                        DeleteIpRequest}, this.DeleteIpOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnDeleteIpOperationCompleted(object arg)
        {
            if ((this.DeleteIpCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteIpCompleted(this, new DeleteIpCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/TestAndVerifyDatabase", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("TestAndVerifyDatabaseResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public TestAndVerifyDatabaseResponse TestAndVerifyDatabase([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] TestAndVerifyDatabaseRequest TestAndVerifyDatabaseRequest)
        {
            object[] results = this.Invoke("TestAndVerifyDatabase", new object[] {
                        TestAndVerifyDatabaseRequest});
            return ((TestAndVerifyDatabaseResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginTestAndVerifyDatabase(TestAndVerifyDatabaseRequest TestAndVerifyDatabaseRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TestAndVerifyDatabase", new object[] {
                        TestAndVerifyDatabaseRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public TestAndVerifyDatabaseResponse EndTestAndVerifyDatabase(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((TestAndVerifyDatabaseResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void TestAndVerifyDatabaseAsync(TestAndVerifyDatabaseRequest TestAndVerifyDatabaseRequest)
        {
            this.TestAndVerifyDatabaseAsync(TestAndVerifyDatabaseRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void TestAndVerifyDatabaseAsync(TestAndVerifyDatabaseRequest TestAndVerifyDatabaseRequest, object userState)
        {
            if ((this.TestAndVerifyDatabaseOperationCompleted == null))
            {
                this.TestAndVerifyDatabaseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestAndVerifyDatabaseOperationCompleted);
            }
            this.InvokeAsync("TestAndVerifyDatabase", new object[] {
                        TestAndVerifyDatabaseRequest}, this.TestAndVerifyDatabaseOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnTestAndVerifyDatabaseOperationCompleted(object arg)
        {
            if ((this.TestAndVerifyDatabaseCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestAndVerifyDatabaseCompleted(this, new TestAndVerifyDatabaseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/CreateStatisticsDatabase", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("CreateStatisticsDatabaseResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public CreateStatisticsDatabaseResponse CreateStatisticsDatabase([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] CreateStatisticsDatabaseRequest CreateStatisticsDatabaseRequest)
        {
            object[] results = this.Invoke("CreateStatisticsDatabase", new object[] {
                        CreateStatisticsDatabaseRequest});
            return ((CreateStatisticsDatabaseResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginCreateStatisticsDatabase(CreateStatisticsDatabaseRequest CreateStatisticsDatabaseRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CreateStatisticsDatabase", new object[] {
                        CreateStatisticsDatabaseRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public CreateStatisticsDatabaseResponse EndCreateStatisticsDatabase(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((CreateStatisticsDatabaseResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void CreateStatisticsDatabaseAsync(CreateStatisticsDatabaseRequest CreateStatisticsDatabaseRequest)
        {
            this.CreateStatisticsDatabaseAsync(CreateStatisticsDatabaseRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void CreateStatisticsDatabaseAsync(CreateStatisticsDatabaseRequest CreateStatisticsDatabaseRequest, object userState)
        {
            if ((this.CreateStatisticsDatabaseOperationCompleted == null))
            {
                this.CreateStatisticsDatabaseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreateStatisticsDatabaseOperationCompleted);
            }
            this.InvokeAsync("CreateStatisticsDatabase", new object[] {
                        CreateStatisticsDatabaseRequest}, this.CreateStatisticsDatabaseOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnCreateStatisticsDatabaseOperationCompleted(object arg)
        {
            if ((this.CreateStatisticsDatabaseCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreateStatisticsDatabaseCompleted(this, new CreateStatisticsDatabaseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/DropStatisticsDatabase", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("DropStatisticsDatabaseResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public DropStatisticsDatabaseResponse DropStatisticsDatabase([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] DropStatisticsDatabaseRequest DropStatisticsDatabaseRequest)
        {
            object[] results = this.Invoke("DropStatisticsDatabase", new object[] {
                        DropStatisticsDatabaseRequest});
            return ((DropStatisticsDatabaseResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginDropStatisticsDatabase(DropStatisticsDatabaseRequest DropStatisticsDatabaseRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DropStatisticsDatabase", new object[] {
                        DropStatisticsDatabaseRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public DropStatisticsDatabaseResponse EndDropStatisticsDatabase(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DropStatisticsDatabaseResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DropStatisticsDatabaseAsync(DropStatisticsDatabaseRequest DropStatisticsDatabaseRequest)
        {
            this.DropStatisticsDatabaseAsync(DropStatisticsDatabaseRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DropStatisticsDatabaseAsync(DropStatisticsDatabaseRequest DropStatisticsDatabaseRequest, object userState)
        {
            if ((this.DropStatisticsDatabaseOperationCompleted == null))
            {
                this.DropStatisticsDatabaseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDropStatisticsDatabaseOperationCompleted);
            }
            this.InvokeAsync("DropStatisticsDatabase", new object[] {
                        DropStatisticsDatabaseRequest}, this.DropStatisticsDatabaseOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnDropStatisticsDatabaseOperationCompleted(object arg)
        {
            if ((this.DropStatisticsDatabaseCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DropStatisticsDatabaseCompleted(this, new DropStatisticsDatabaseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetMimeMappings", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetMimeMappingsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetMimeMappingsResponse GetMimeMappings([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetMimeMappingsRequest GetMimeMappingsRequest)
        {
            object[] results = this.Invoke("GetMimeMappings", new object[] {
                        GetMimeMappingsRequest});
            return ((GetMimeMappingsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetMimeMappings(GetMimeMappingsRequest GetMimeMappingsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetMimeMappings", new object[] {
                        GetMimeMappingsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetMimeMappingsResponse EndGetMimeMappings(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetMimeMappingsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetMimeMappingsAsync(GetMimeMappingsRequest GetMimeMappingsRequest)
        {
            this.GetMimeMappingsAsync(GetMimeMappingsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetMimeMappingsAsync(GetMimeMappingsRequest GetMimeMappingsRequest, object userState)
        {
            if ((this.GetMimeMappingsOperationCompleted == null))
            {
                this.GetMimeMappingsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetMimeMappingsOperationCompleted);
            }
            this.InvokeAsync("GetMimeMappings", new object[] {
                        GetMimeMappingsRequest}, this.GetMimeMappingsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetMimeMappingsOperationCompleted(object arg)
        {
            if ((this.GetMimeMappingsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetMimeMappingsCompleted(this, new GetMimeMappingsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SaveMimeMappings", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SaveMimeMappingsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SaveMimeMappingsResponse SaveMimeMappings([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SaveMimeMappingsRequest SaveMimeMappingsRequest)
        {
            object[] results = this.Invoke("SaveMimeMappings", new object[] {
                        SaveMimeMappingsRequest});
            return ((SaveMimeMappingsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSaveMimeMappings(SaveMimeMappingsRequest SaveMimeMappingsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SaveMimeMappings", new object[] {
                        SaveMimeMappingsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SaveMimeMappingsResponse EndSaveMimeMappings(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SaveMimeMappingsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveMimeMappingsAsync(SaveMimeMappingsRequest SaveMimeMappingsRequest)
        {
            this.SaveMimeMappingsAsync(SaveMimeMappingsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveMimeMappingsAsync(SaveMimeMappingsRequest SaveMimeMappingsRequest, object userState)
        {
            if ((this.SaveMimeMappingsOperationCompleted == null))
            {
                this.SaveMimeMappingsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSaveMimeMappingsOperationCompleted);
            }
            this.InvokeAsync("SaveMimeMappings", new object[] {
                        SaveMimeMappingsRequest}, this.SaveMimeMappingsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSaveMimeMappingsOperationCompleted(object arg)
        {
            if ((this.SaveMimeMappingsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SaveMimeMappingsCompleted(this, new SaveMimeMappingsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/ServerSummaryStatus", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ServerSummaryStatusResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public ServerSummaryStatusResponse ServerSummaryStatus([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] ServerSummaryStatusRequest ServerSummaryStatusRequest)
        {
            object[] results = this.Invoke("ServerSummaryStatus", new object[] {
                        ServerSummaryStatusRequest});
            return ((ServerSummaryStatusResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginServerSummaryStatus(ServerSummaryStatusRequest ServerSummaryStatusRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ServerSummaryStatus", new object[] {
                        ServerSummaryStatusRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public ServerSummaryStatusResponse EndServerSummaryStatus(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ServerSummaryStatusResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ServerSummaryStatusAsync(ServerSummaryStatusRequest ServerSummaryStatusRequest)
        {
            this.ServerSummaryStatusAsync(ServerSummaryStatusRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ServerSummaryStatusAsync(ServerSummaryStatusRequest ServerSummaryStatusRequest, object userState)
        {
            if ((this.ServerSummaryStatusOperationCompleted == null))
            {
                this.ServerSummaryStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnServerSummaryStatusOperationCompleted);
            }
            this.InvokeAsync("ServerSummaryStatus", new object[] {
                        ServerSummaryStatusRequest}, this.ServerSummaryStatusOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnServerSummaryStatusOperationCompleted(object arg)
        {
            if ((this.ServerSummaryStatusCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ServerSummaryStatusCompleted(this, new ServerSummaryStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/ServerInformation", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ServerInformationResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public ServerInformationResponse ServerInformation([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] ServerInformationRequest ServerInformationRequest)
        {
            object[] results = this.Invoke("ServerInformation", new object[] {
                        ServerInformationRequest});
            return ((ServerInformationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginServerInformation(ServerInformationRequest ServerInformationRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ServerInformation", new object[] {
                        ServerInformationRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public ServerInformationResponse EndServerInformation(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ServerInformationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ServerInformationAsync(ServerInformationRequest ServerInformationRequest)
        {
            this.ServerInformationAsync(ServerInformationRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ServerInformationAsync(ServerInformationRequest ServerInformationRequest, object userState)
        {
            if ((this.ServerInformationOperationCompleted == null))
            {
                this.ServerInformationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnServerInformationOperationCompleted);
            }
            this.InvokeAsync("ServerInformation", new object[] {
                        ServerInformationRequest}, this.ServerInformationOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnServerInformationOperationCompleted(object arg)
        {
            if ((this.ServerInformationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ServerInformationCompleted(this, new ServerInformationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/CurrentStatus", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("CurrentStatusResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public CurrentStatusResponse CurrentStatus([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] CurrentStatusRequest CurrentStatusRequest)
        {
            object[] results = this.Invoke("CurrentStatus", new object[] {
                        CurrentStatusRequest});
            return ((CurrentStatusResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginCurrentStatus(CurrentStatusRequest CurrentStatusRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CurrentStatus", new object[] {
                        CurrentStatusRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public CurrentStatusResponse EndCurrentStatus(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((CurrentStatusResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void CurrentStatusAsync(CurrentStatusRequest CurrentStatusRequest)
        {
            this.CurrentStatusAsync(CurrentStatusRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void CurrentStatusAsync(CurrentStatusRequest CurrentStatusRequest, object userState)
        {
            if ((this.CurrentStatusOperationCompleted == null))
            {
                this.CurrentStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCurrentStatusOperationCompleted);
            }
            this.InvokeAsync("CurrentStatus", new object[] {
                        CurrentStatusRequest}, this.CurrentStatusOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnCurrentStatusOperationCompleted(object arg)
        {
            if ((this.CurrentStatusCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CurrentStatusCompleted(this, new CurrentStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/StartServer", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("StartServerResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public StartServerResponse StartServer([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] StartServerRequest StartServerRequest)
        {
            object[] results = this.Invoke("StartServer", new object[] {
                        StartServerRequest});
            return ((StartServerResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginStartServer(StartServerRequest StartServerRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("StartServer", new object[] {
                        StartServerRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public StartServerResponse EndStartServer(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((StartServerResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void StartServerAsync(StartServerRequest StartServerRequest)
        {
            this.StartServerAsync(StartServerRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void StartServerAsync(StartServerRequest StartServerRequest, object userState)
        {
            if ((this.StartServerOperationCompleted == null))
            {
                this.StartServerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnStartServerOperationCompleted);
            }
            this.InvokeAsync("StartServer", new object[] {
                        StartServerRequest}, this.StartServerOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnStartServerOperationCompleted(object arg)
        {
            if ((this.StartServerCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.StartServerCompleted(this, new StartServerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/StopServer", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("StopServerResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public StopServerResponse StopServer([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] StopServerRequest StopServerRequest)
        {
            object[] results = this.Invoke("StopServer", new object[] {
                        StopServerRequest});
            return ((StopServerResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginStopServer(StopServerRequest StopServerRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("StopServer", new object[] {
                        StopServerRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public StopServerResponse EndStopServer(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((StopServerResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void StopServerAsync(StopServerRequest StopServerRequest)
        {
            this.StopServerAsync(StopServerRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void StopServerAsync(StopServerRequest StopServerRequest, object userState)
        {
            if ((this.StopServerOperationCompleted == null))
            {
                this.StopServerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnStopServerOperationCompleted);
            }
            this.InvokeAsync("StopServer", new object[] {
                        StopServerRequest}, this.StopServerOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnStopServerOperationCompleted(object arg)
        {
            if ((this.StopServerCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.StopServerCompleted(this, new StopServerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/ServerStarted", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ServerStartedResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public ServerStartedResponse ServerStarted([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] ServerStartedRequest ServerStartedRequest)
        {
            object[] results = this.Invoke("ServerStarted", new object[] {
                        ServerStartedRequest});
            return ((ServerStartedResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginServerStarted(ServerStartedRequest ServerStartedRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ServerStarted", new object[] {
                        ServerStartedRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public ServerStartedResponse EndServerStarted(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ServerStartedResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ServerStartedAsync(ServerStartedRequest ServerStartedRequest)
        {
            this.ServerStartedAsync(ServerStartedRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ServerStartedAsync(ServerStartedRequest ServerStartedRequest, object userState)
        {
            if ((this.ServerStartedOperationCompleted == null))
            {
                this.ServerStartedOperationCompleted = new System.Threading.SendOrPostCallback(this.OnServerStartedOperationCompleted);
            }
            this.InvokeAsync("ServerStarted", new object[] {
                        ServerStartedRequest}, this.ServerStartedOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnServerStartedOperationCompleted(object arg)
        {
            if ((this.ServerStartedCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ServerStartedCompleted(this, new ServerStartedCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/InitializeServer", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("InitializeServerResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public InitializeServerResponse InitializeServer([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] InitializeServerRequest InitializeServerRequest)
        {
            object[] results = this.Invoke("InitializeServer", new object[] {
                        InitializeServerRequest});
            return ((InitializeServerResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginInitializeServer(InitializeServerRequest InitializeServerRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("InitializeServer", new object[] {
                        InitializeServerRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public InitializeServerResponse EndInitializeServer(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((InitializeServerResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void InitializeServerAsync(InitializeServerRequest InitializeServerRequest)
        {
            this.InitializeServerAsync(InitializeServerRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void InitializeServerAsync(InitializeServerRequest InitializeServerRequest, object userState)
        {
            if ((this.InitializeServerOperationCompleted == null))
            {
                this.InitializeServerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnInitializeServerOperationCompleted);
            }
            this.InvokeAsync("InitializeServer", new object[] {
                        InitializeServerRequest}, this.InitializeServerOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnInitializeServerOperationCompleted(object arg)
        {
            if ((this.InitializeServerCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.InitializeServerCompleted(this, new InitializeServerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/ShutdownServer", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ShutdownServerResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public ShutdownServerResponse ShutdownServer([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] ShutdownServerRequest ShutdownServerRequest)
        {
            object[] results = this.Invoke("ShutdownServer", new object[] {
                        ShutdownServerRequest});
            return ((ShutdownServerResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginShutdownServer(ShutdownServerRequest ShutdownServerRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ShutdownServer", new object[] {
                        ShutdownServerRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public ShutdownServerResponse EndShutdownServer(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ShutdownServerResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ShutdownServerAsync(ShutdownServerRequest ShutdownServerRequest)
        {
            this.ShutdownServerAsync(ShutdownServerRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ShutdownServerAsync(ShutdownServerRequest ShutdownServerRequest, object userState)
        {
            if ((this.ShutdownServerOperationCompleted == null))
            {
                this.ShutdownServerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnShutdownServerOperationCompleted);
            }
            this.InvokeAsync("ShutdownServer", new object[] {
                        ShutdownServerRequest}, this.ShutdownServerOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnShutdownServerOperationCompleted(object arg)
        {
            if ((this.ShutdownServerCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ShutdownServerCompleted(this, new ShutdownServerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetEventRules", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetEventRulesResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetEventRulesResponse GetEventRules([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetEventRulesRequest GetEventRulesRequest)
        {
            object[] results = this.Invoke("GetEventRules", new object[] {
                        GetEventRulesRequest});
            return ((GetEventRulesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetEventRules(GetEventRulesRequest GetEventRulesRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetEventRules", new object[] {
                        GetEventRulesRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetEventRulesResponse EndGetEventRules(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetEventRulesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetEventRulesAsync(GetEventRulesRequest GetEventRulesRequest)
        {
            this.GetEventRulesAsync(GetEventRulesRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetEventRulesAsync(GetEventRulesRequest GetEventRulesRequest, object userState)
        {
            if ((this.GetEventRulesOperationCompleted == null))
            {
                this.GetEventRulesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetEventRulesOperationCompleted);
            }
            this.InvokeAsync("GetEventRules", new object[] {
                        GetEventRulesRequest}, this.GetEventRulesOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetEventRulesOperationCompleted(object arg)
        {
            if ((this.GetEventRulesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetEventRulesCompleted(this, new GetEventRulesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SetEventRules", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SetEventRulesResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SetEventRulesResponse SetEventRules([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SetEventRulesRequest SetEventRulesRequest)
        {
            object[] results = this.Invoke("SetEventRules", new object[] {
                        SetEventRulesRequest});
            return ((SetEventRulesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSetEventRules(SetEventRulesRequest SetEventRulesRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SetEventRules", new object[] {
                        SetEventRulesRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SetEventRulesResponse EndSetEventRules(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SetEventRulesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SetEventRulesAsync(SetEventRulesRequest SetEventRulesRequest)
        {
            this.SetEventRulesAsync(SetEventRulesRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SetEventRulesAsync(SetEventRulesRequest SetEventRulesRequest, object userState)
        {
            if ((this.SetEventRulesOperationCompleted == null))
            {
                this.SetEventRulesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetEventRulesOperationCompleted);
            }
            this.InvokeAsync("SetEventRules", new object[] {
                        SetEventRulesRequest}, this.SetEventRulesOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSetEventRulesOperationCompleted(object arg)
        {
            if ((this.SetEventRulesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetEventRulesCompleted(this, new SetEventRulesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/DeleteRequestedAccounts", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("DeleteRequestedAccountsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public DeleteRequestedAccountsResponse DeleteRequestedAccounts([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] DeleteRequestedAccountsRequest DeleteRequestedAccountsRequest)
        {
            object[] results = this.Invoke("DeleteRequestedAccounts", new object[] {
                        DeleteRequestedAccountsRequest});
            return ((DeleteRequestedAccountsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginDeleteRequestedAccounts(DeleteRequestedAccountsRequest DeleteRequestedAccountsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteRequestedAccounts", new object[] {
                        DeleteRequestedAccountsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public DeleteRequestedAccountsResponse EndDeleteRequestedAccounts(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeleteRequestedAccountsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteRequestedAccountsAsync(DeleteRequestedAccountsRequest DeleteRequestedAccountsRequest)
        {
            this.DeleteRequestedAccountsAsync(DeleteRequestedAccountsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteRequestedAccountsAsync(DeleteRequestedAccountsRequest DeleteRequestedAccountsRequest, object userState)
        {
            if ((this.DeleteRequestedAccountsOperationCompleted == null))
            {
                this.DeleteRequestedAccountsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteRequestedAccountsOperationCompleted);
            }
            this.InvokeAsync("DeleteRequestedAccounts", new object[] {
                        DeleteRequestedAccountsRequest}, this.DeleteRequestedAccountsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnDeleteRequestedAccountsOperationCompleted(object arg)
        {
            if ((this.DeleteRequestedAccountsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteRequestedAccountsCompleted(this, new DeleteRequestedAccountsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetRequestedAccounts", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetRequestedAccountsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetRequestedAccountsResponse GetRequestedAccounts([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetRequestedAccountsRequest GetRequestedAccountsRequest)
        {
            object[] results = this.Invoke("GetRequestedAccounts", new object[] {
                        GetRequestedAccountsRequest});
            return ((GetRequestedAccountsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetRequestedAccounts(GetRequestedAccountsRequest GetRequestedAccountsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetRequestedAccounts", new object[] {
                        GetRequestedAccountsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetRequestedAccountsResponse EndGetRequestedAccounts(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetRequestedAccountsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetRequestedAccountsAsync(GetRequestedAccountsRequest GetRequestedAccountsRequest)
        {
            this.GetRequestedAccountsAsync(GetRequestedAccountsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetRequestedAccountsAsync(GetRequestedAccountsRequest GetRequestedAccountsRequest, object userState)
        {
            if ((this.GetRequestedAccountsOperationCompleted == null))
            {
                this.GetRequestedAccountsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetRequestedAccountsOperationCompleted);
            }
            this.InvokeAsync("GetRequestedAccounts", new object[] {
                        GetRequestedAccountsRequest}, this.GetRequestedAccountsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetRequestedAccountsOperationCompleted(object arg)
        {
            if ((this.GetRequestedAccountsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetRequestedAccountsCompleted(this, new GetRequestedAccountsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SetRequestedAccounts", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SetRequestedAccountsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SetRequestedAccountsResponse SetRequestedAccounts([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SetRequestedAccountsRequest SetRequestedAccountsRequest)
        {
            object[] results = this.Invoke("SetRequestedAccounts", new object[] {
                        SetRequestedAccountsRequest});
            return ((SetRequestedAccountsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSetRequestedAccounts(SetRequestedAccountsRequest SetRequestedAccountsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SetRequestedAccounts", new object[] {
                        SetRequestedAccountsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SetRequestedAccountsResponse EndSetRequestedAccounts(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SetRequestedAccountsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SetRequestedAccountsAsync(SetRequestedAccountsRequest SetRequestedAccountsRequest)
        {
            this.SetRequestedAccountsAsync(SetRequestedAccountsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SetRequestedAccountsAsync(SetRequestedAccountsRequest SetRequestedAccountsRequest, object userState)
        {
            if ((this.SetRequestedAccountsOperationCompleted == null))
            {
                this.SetRequestedAccountsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetRequestedAccountsOperationCompleted);
            }
            this.InvokeAsync("SetRequestedAccounts", new object[] {
                        SetRequestedAccountsRequest}, this.SetRequestedAccountsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSetRequestedAccountsOperationCompleted(object arg)
        {
            if ((this.SetRequestedAccountsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetRequestedAccountsCompleted(this, new SetRequestedAccountsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetAuthenticationList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetAuthenticationListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetAuthenticationListResponse GetAuthenticationList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetAuthenticationListRequest GetAuthenticationListRequest)
        {
            object[] results = this.Invoke("GetAuthenticationList", new object[] {
                        GetAuthenticationListRequest});
            return ((GetAuthenticationListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetAuthenticationList(GetAuthenticationListRequest GetAuthenticationListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetAuthenticationList", new object[] {
                        GetAuthenticationListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetAuthenticationListResponse EndGetAuthenticationList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetAuthenticationListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetAuthenticationListAsync(GetAuthenticationListRequest GetAuthenticationListRequest)
        {
            this.GetAuthenticationListAsync(GetAuthenticationListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetAuthenticationListAsync(GetAuthenticationListRequest GetAuthenticationListRequest, object userState)
        {
            if ((this.GetAuthenticationListOperationCompleted == null))
            {
                this.GetAuthenticationListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetAuthenticationListOperationCompleted);
            }
            this.InvokeAsync("GetAuthenticationList", new object[] {
                        GetAuthenticationListRequest}, this.GetAuthenticationListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetAuthenticationListOperationCompleted(object arg)
        {
            if ((this.GetAuthenticationListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetAuthenticationListCompleted(this, new GetAuthenticationListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SetAuthenticationList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SetAuthenticationListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SetAuthenticationListResponse SetAuthenticationList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SetAuthenticationListRequest SetAuthenticationListRequest)
        {
            object[] results = this.Invoke("SetAuthenticationList", new object[] {
                        SetAuthenticationListRequest});
            return ((SetAuthenticationListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSetAuthenticationList(SetAuthenticationListRequest SetAuthenticationListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SetAuthenticationList", new object[] {
                        SetAuthenticationListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SetAuthenticationListResponse EndSetAuthenticationList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SetAuthenticationListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SetAuthenticationListAsync(SetAuthenticationListRequest SetAuthenticationListRequest)
        {
            this.SetAuthenticationListAsync(SetAuthenticationListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SetAuthenticationListAsync(SetAuthenticationListRequest SetAuthenticationListRequest, object userState)
        {
            if ((this.SetAuthenticationListOperationCompleted == null))
            {
                this.SetAuthenticationListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetAuthenticationListOperationCompleted);
            }
            this.InvokeAsync("SetAuthenticationList", new object[] {
                        SetAuthenticationListRequest}, this.SetAuthenticationListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSetAuthenticationListOperationCompleted(object arg)
        {
            if ((this.SetAuthenticationListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetAuthenticationListCompleted(this, new SetAuthenticationListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetHostname", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetHostnameResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetHostnameResponse GetHostname([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetHostnameRequest GetHostnameRequest)
        {
            object[] results = this.Invoke("GetHostname", new object[] {
                        GetHostnameRequest});
            return ((GetHostnameResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetHostname(GetHostnameRequest GetHostnameRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetHostname", new object[] {
                        GetHostnameRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetHostnameResponse EndGetHostname(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetHostnameResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetHostnameAsync(GetHostnameRequest GetHostnameRequest)
        {
            this.GetHostnameAsync(GetHostnameRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetHostnameAsync(GetHostnameRequest GetHostnameRequest, object userState)
        {
            if ((this.GetHostnameOperationCompleted == null))
            {
                this.GetHostnameOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetHostnameOperationCompleted);
            }
            this.InvokeAsync("GetHostname", new object[] {
                        GetHostnameRequest}, this.GetHostnameOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetHostnameOperationCompleted(object arg)
        {
            if ((this.GetHostnameCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetHostnameCompleted(this, new GetHostnameCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SetWANIP", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SetWANIPResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SetWANIPResponse SetWANIP([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SetWANIPRequest SetWANIPRequest)
        {
            object[] results = this.Invoke("SetWANIP", new object[] {
                        SetWANIPRequest});
            return ((SetWANIPResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSetWANIP(SetWANIPRequest SetWANIPRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SetWANIP", new object[] {
                        SetWANIPRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SetWANIPResponse EndSetWANIP(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SetWANIPResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SetWANIPAsync(SetWANIPRequest SetWANIPRequest)
        {
            this.SetWANIPAsync(SetWANIPRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SetWANIPAsync(SetWANIPRequest SetWANIPRequest, object userState)
        {
            if ((this.SetWANIPOperationCompleted == null))
            {
                this.SetWANIPOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetWANIPOperationCompleted);
            }
            this.InvokeAsync("SetWANIP", new object[] {
                        SetWANIPRequest}, this.SetWANIPOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSetWANIPOperationCompleted(object arg)
        {
            if ((this.SetWANIPCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetWANIPCompleted(this, new SetWANIPCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/AddUser", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("AddUserResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public AddUserResponse AddUser([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] AddUserRequest AddUserRequest)
        {
            object[] results = this.Invoke("AddUser", new object[] {
                        AddUserRequest});
            return ((AddUserResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginAddUser(AddUserRequest AddUserRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddUser", new object[] {
                        AddUserRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public AddUserResponse EndAddUser(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((AddUserResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void AddUserAsync(AddUserRequest AddUserRequest)
        {
            this.AddUserAsync(AddUserRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void AddUserAsync(AddUserRequest AddUserRequest, object userState)
        {
            if ((this.AddUserOperationCompleted == null))
            {
                this.AddUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddUserOperationCompleted);
            }
            this.InvokeAsync("AddUser", new object[] {
                        AddUserRequest}, this.AddUserOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnAddUserOperationCompleted(object arg)
        {
            if ((this.AddUserCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddUserCompleted(this, new AddUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/AddGroup", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("AddGroupResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public AddGroupResponse AddGroup([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] AddGroupRequest AddGroupRequest)
        {
            object[] results = this.Invoke("AddGroup", new object[] {
                        AddGroupRequest});
            return ((AddGroupResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginAddGroup(AddGroupRequest AddGroupRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddGroup", new object[] {
                        AddGroupRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public AddGroupResponse EndAddGroup(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((AddGroupResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void AddGroupAsync(AddGroupRequest AddGroupRequest)
        {
            this.AddGroupAsync(AddGroupRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void AddGroupAsync(AddGroupRequest AddGroupRequest, object userState)
        {
            if ((this.AddGroupOperationCompleted == null))
            {
                this.AddGroupOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddGroupOperationCompleted);
            }
            this.InvokeAsync("AddGroup", new object[] {
                        AddGroupRequest}, this.AddGroupOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnAddGroupOperationCompleted(object arg)
        {
            if ((this.AddGroupCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddGroupCompleted(this, new AddGroupCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/DeleteUser", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("DeleteUserResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public DeleteUserResponse DeleteUser([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] DeleteUserRequest DeleteUserRequest)
        {
            object[] results = this.Invoke("DeleteUser", new object[] {
                        DeleteUserRequest});
            return ((DeleteUserResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginDeleteUser(DeleteUserRequest DeleteUserRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteUser", new object[] {
                        DeleteUserRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public DeleteUserResponse EndDeleteUser(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeleteUserResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteUserAsync(DeleteUserRequest DeleteUserRequest)
        {
            this.DeleteUserAsync(DeleteUserRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteUserAsync(DeleteUserRequest DeleteUserRequest, object userState)
        {
            if ((this.DeleteUserOperationCompleted == null))
            {
                this.DeleteUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteUserOperationCompleted);
            }
            this.InvokeAsync("DeleteUser", new object[] {
                        DeleteUserRequest}, this.DeleteUserOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnDeleteUserOperationCompleted(object arg)
        {
            if ((this.DeleteUserCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteUserCompleted(this, new DeleteUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/DeleteGroup", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("DeleteGroupResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public DeleteGroupResponse DeleteGroup([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] DeleteGroupRequest DeleteGroupRequest)
        {
            object[] results = this.Invoke("DeleteGroup", new object[] {
                        DeleteGroupRequest});
            return ((DeleteGroupResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginDeleteGroup(DeleteGroupRequest DeleteGroupRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteGroup", new object[] {
                        DeleteGroupRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public DeleteGroupResponse EndDeleteGroup(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeleteGroupResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteGroupAsync(DeleteGroupRequest DeleteGroupRequest)
        {
            this.DeleteGroupAsync(DeleteGroupRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteGroupAsync(DeleteGroupRequest DeleteGroupRequest, object userState)
        {
            if ((this.DeleteGroupOperationCompleted == null))
            {
                this.DeleteGroupOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteGroupOperationCompleted);
            }
            this.InvokeAsync("DeleteGroup", new object[] {
                        DeleteGroupRequest}, this.DeleteGroupOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnDeleteGroupOperationCompleted(object arg)
        {
            if ((this.DeleteGroupCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteGroupCompleted(this, new DeleteGroupCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/AddRoot", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("AddRootResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public AddRootResponse AddRoot([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] AddRootRequest AddRootRequest)
        {
            object[] results = this.Invoke("AddRoot", new object[] {
                        AddRootRequest});
            return ((AddRootResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginAddRoot(AddRootRequest AddRootRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddRoot", new object[] {
                        AddRootRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public AddRootResponse EndAddRoot(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((AddRootResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void AddRootAsync(AddRootRequest AddRootRequest)
        {
            this.AddRootAsync(AddRootRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void AddRootAsync(AddRootRequest AddRootRequest, object userState)
        {
            if ((this.AddRootOperationCompleted == null))
            {
                this.AddRootOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddRootOperationCompleted);
            }
            this.InvokeAsync("AddRoot", new object[] {
                        AddRootRequest}, this.AddRootOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnAddRootOperationCompleted(object arg)
        {
            if ((this.AddRootCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddRootCompleted(this, new AddRootCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/DeleteRoot", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("DeleteRootResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public DeleteRootResponse DeleteRoot([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] DeleteRootRequest DeleteRootRequest)
        {
            object[] results = this.Invoke("DeleteRoot", new object[] {
                        DeleteRootRequest});
            return ((DeleteRootResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginDeleteRoot(DeleteRootRequest DeleteRootRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteRoot", new object[] {
                        DeleteRootRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public DeleteRootResponse EndDeleteRoot(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeleteRootResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteRootAsync(DeleteRootRequest DeleteRootRequest)
        {
            this.DeleteRootAsync(DeleteRootRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void DeleteRootAsync(DeleteRootRequest DeleteRootRequest, object userState)
        {
            if ((this.DeleteRootOperationCompleted == null))
            {
                this.DeleteRootOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteRootOperationCompleted);
            }
            this.InvokeAsync("DeleteRoot", new object[] {
                        DeleteRootRequest}, this.DeleteRootOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnDeleteRootOperationCompleted(object arg)
        {
            if ((this.DeleteRootCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteRootCompleted(this, new DeleteRootCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetUserList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetUserListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetUserListResponse GetUserList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetUserListRequest GetUserListRequest)
        {
            object[] results = this.Invoke("GetUserList", new object[] {
                        GetUserListRequest});
            return ((GetUserListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetUserList(GetUserListRequest GetUserListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetUserList", new object[] {
                        GetUserListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetUserListResponse EndGetUserList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetUserListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetUserListAsync(GetUserListRequest GetUserListRequest)
        {
            this.GetUserListAsync(GetUserListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetUserListAsync(GetUserListRequest GetUserListRequest, object userState)
        {
            if ((this.GetUserListOperationCompleted == null))
            {
                this.GetUserListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetUserListOperationCompleted);
            }
            this.InvokeAsync("GetUserList", new object[] {
                        GetUserListRequest}, this.GetUserListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetUserListOperationCompleted(object arg)
        {
            if ((this.GetUserListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetUserListCompleted(this, new GetUserListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetGroupList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetGroupListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetGroupListResponse GetGroupList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetGroupListRequest GetGroupListRequest)
        {
            object[] results = this.Invoke("GetGroupList", new object[] {
                        GetGroupListRequest});
            return ((GetGroupListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetGroupList(GetGroupListRequest GetGroupListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetGroupList", new object[] {
                        GetGroupListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetGroupListResponse EndGetGroupList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetGroupListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetGroupListAsync(GetGroupListRequest GetGroupListRequest)
        {
            this.GetGroupListAsync(GetGroupListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetGroupListAsync(GetGroupListRequest GetGroupListRequest, object userState)
        {
            if ((this.GetGroupListOperationCompleted == null))
            {
                this.GetGroupListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetGroupListOperationCompleted);
            }
            this.InvokeAsync("GetGroupList", new object[] {
                        GetGroupListRequest}, this.GetGroupListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetGroupListOperationCompleted(object arg)
        {
            if ((this.GetGroupListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetGroupListCompleted(this, new GetGroupListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetUserInformation", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetUserInformationResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetUserInformationResponse GetUserInformation([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetUserInformationRequest GetUserInformationRequest)
        {
            object[] results = this.Invoke("GetUserInformation", new object[] {
                        GetUserInformationRequest});
            return ((GetUserInformationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetUserInformation(GetUserInformationRequest GetUserInformationRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetUserInformation", new object[] {
                        GetUserInformationRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetUserInformationResponse EndGetUserInformation(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetUserInformationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetUserInformationAsync(GetUserInformationRequest GetUserInformationRequest)
        {
            this.GetUserInformationAsync(GetUserInformationRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetUserInformationAsync(GetUserInformationRequest GetUserInformationRequest, object userState)
        {
            if ((this.GetUserInformationOperationCompleted == null))
            {
                this.GetUserInformationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetUserInformationOperationCompleted);
            }
            this.InvokeAsync("GetUserInformation", new object[] {
                        GetUserInformationRequest}, this.GetUserInformationOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetUserInformationOperationCompleted(object arg)
        {
            if ((this.GetUserInformationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetUserInformationCompleted(this, new GetUserInformationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetConnectedUserList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetConnectedUserListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetConnectedUserListResponse GetConnectedUserList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetConnectedUserListRequest GetConnectedUserListRequest)
        {
            object[] results = this.Invoke("GetConnectedUserList", new object[] {
                        GetConnectedUserListRequest});
            return ((GetConnectedUserListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetConnectedUserList(GetConnectedUserListRequest GetConnectedUserListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetConnectedUserList", new object[] {
                        GetConnectedUserListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetConnectedUserListResponse EndGetConnectedUserList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetConnectedUserListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetConnectedUserListAsync(GetConnectedUserListRequest GetConnectedUserListRequest)
        {
            this.GetConnectedUserListAsync(GetConnectedUserListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetConnectedUserListAsync(GetConnectedUserListRequest GetConnectedUserListRequest, object userState)
        {
            if ((this.GetConnectedUserListOperationCompleted == null))
            {
                this.GetConnectedUserListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetConnectedUserListOperationCompleted);
            }
            this.InvokeAsync("GetConnectedUserList", new object[] {
                        GetConnectedUserListRequest}, this.GetConnectedUserListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetConnectedUserListOperationCompleted(object arg)
        {
            if ((this.GetConnectedUserListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetConnectedUserListCompleted(this, new GetConnectedUserListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/ChangePassword", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ChangePasswordResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public ChangePasswordResponse ChangePassword([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] ChangePasswordRequest ChangePasswordRequest)
        {
            object[] results = this.Invoke("ChangePassword", new object[] {
                        ChangePasswordRequest});
            return ((ChangePasswordResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginChangePassword(ChangePasswordRequest ChangePasswordRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ChangePassword", new object[] {
                        ChangePasswordRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public ChangePasswordResponse EndChangePassword(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ChangePasswordResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest)
        {
            this.ChangePasswordAsync(ChangePasswordRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest, object userState)
        {
            if ((this.ChangePasswordOperationCompleted == null))
            {
                this.ChangePasswordOperationCompleted = new System.Threading.SendOrPostCallback(this.OnChangePasswordOperationCompleted);
            }
            this.InvokeAsync("ChangePassword", new object[] {
                        ChangePasswordRequest}, this.ChangePasswordOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnChangePasswordOperationCompleted(object arg)
        {
            if ((this.ChangePasswordCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ChangePasswordCompleted(this, new ChangePasswordCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/RenameUser", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("RenameUserResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public RenameUserResponse RenameUser([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] RenameUserRequest RenameUserRequest)
        {
            object[] results = this.Invoke("RenameUser", new object[] {
                        RenameUserRequest});
            return ((RenameUserResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginRenameUser(RenameUserRequest RenameUserRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RenameUser", new object[] {
                        RenameUserRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public RenameUserResponse EndRenameUser(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((RenameUserResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void RenameUserAsync(RenameUserRequest RenameUserRequest)
        {
            this.RenameUserAsync(RenameUserRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void RenameUserAsync(RenameUserRequest RenameUserRequest, object userState)
        {
            if ((this.RenameUserOperationCompleted == null))
            {
                this.RenameUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRenameUserOperationCompleted);
            }
            this.InvokeAsync("RenameUser", new object[] {
                        RenameUserRequest}, this.RenameUserOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnRenameUserOperationCompleted(object arg)
        {
            if ((this.RenameUserCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RenameUserCompleted(this, new RenameUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/TerminateConnection", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("TerminateConnectionResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public TerminateConnectionResponse TerminateConnection([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] TerminateConnectionRequest TerminateConnectionRequest)
        {
            object[] results = this.Invoke("TerminateConnection", new object[] {
                        TerminateConnectionRequest});
            return ((TerminateConnectionResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginTerminateConnection(TerminateConnectionRequest TerminateConnectionRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TerminateConnection", new object[] {
                        TerminateConnectionRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public TerminateConnectionResponse EndTerminateConnection(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((TerminateConnectionResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void TerminateConnectionAsync(TerminateConnectionRequest TerminateConnectionRequest)
        {
            this.TerminateConnectionAsync(TerminateConnectionRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void TerminateConnectionAsync(TerminateConnectionRequest TerminateConnectionRequest, object userState)
        {
            if ((this.TerminateConnectionOperationCompleted == null))
            {
                this.TerminateConnectionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTerminateConnectionOperationCompleted);
            }
            this.InvokeAsync("TerminateConnection", new object[] {
                        TerminateConnectionRequest}, this.TerminateConnectionOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnTerminateConnectionOperationCompleted(object arg)
        {
            if ((this.TerminateConnectionCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TerminateConnectionCompleted(this, new TerminateConnectionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetProfiles", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetProfilesResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetProfilesResponse GetProfiles([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetProfilesRequest GetProfilesRequest)
        {
            object[] results = this.Invoke("GetProfiles", new object[] {
                        GetProfilesRequest});
            return ((GetProfilesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetProfiles(GetProfilesRequest GetProfilesRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetProfiles", new object[] {
                        GetProfilesRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetProfilesResponse EndGetProfiles(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetProfilesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetProfilesAsync(GetProfilesRequest GetProfilesRequest)
        {
            this.GetProfilesAsync(GetProfilesRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetProfilesAsync(GetProfilesRequest GetProfilesRequest, object userState)
        {
            if ((this.GetProfilesOperationCompleted == null))
            {
                this.GetProfilesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetProfilesOperationCompleted);
            }
            this.InvokeAsync("GetProfiles", new object[] {
                        GetProfilesRequest}, this.GetProfilesOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetProfilesOperationCompleted(object arg)
        {
            if ((this.GetProfilesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetProfilesCompleted(this, new GetProfilesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetGroups", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetGroupsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetGroupsResponse GetGroups([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetGroupsRequest GetGroupsRequest)
        {
            object[] results = this.Invoke("GetGroups", new object[] {
                        GetGroupsRequest});
            return ((GetGroupsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetGroups(GetGroupsRequest GetGroupsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetGroups", new object[] {
                        GetGroupsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetGroupsResponse EndGetGroups(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetGroupsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetGroupsAsync(GetGroupsRequest GetGroupsRequest)
        {
            this.GetGroupsAsync(GetGroupsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetGroupsAsync(GetGroupsRequest GetGroupsRequest, object userState)
        {
            if ((this.GetGroupsOperationCompleted == null))
            {
                this.GetGroupsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetGroupsOperationCompleted);
            }
            this.InvokeAsync("GetGroups", new object[] {
                        GetGroupsRequest}, this.GetGroupsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetGroupsOperationCompleted(object arg)
        {
            if ((this.GetGroupsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetGroupsCompleted(this, new GetGroupsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetConfiguration", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetConfigurationResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetConfigurationResponse GetConfiguration([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetConfigurationRequest GetConfigurationRequest)
        {
            object[] results = this.Invoke("GetConfiguration", new object[] {
                        GetConfigurationRequest});
            return ((GetConfigurationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetConfiguration(GetConfigurationRequest GetConfigurationRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetConfiguration", new object[] {
                        GetConfigurationRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetConfigurationResponse EndGetConfiguration(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetConfigurationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetConfigurationAsync(GetConfigurationRequest GetConfigurationRequest)
        {
            this.GetConfigurationAsync(GetConfigurationRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetConfigurationAsync(GetConfigurationRequest GetConfigurationRequest, object userState)
        {
            if ((this.GetConfigurationOperationCompleted == null))
            {
                this.GetConfigurationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetConfigurationOperationCompleted);
            }
            this.InvokeAsync("GetConfiguration", new object[] {
                        GetConfigurationRequest}, this.GetConfigurationOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetConfigurationOperationCompleted(object arg)
        {
            if ((this.GetConfigurationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetConfigurationCompleted(this, new GetConfigurationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetInterfaces", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetInterfacesResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetInterfacesResponse GetInterfaces([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetInterfacesRequest GetInterfacesRequest)
        {
            object[] results = this.Invoke("GetInterfaces", new object[] {
                        GetInterfacesRequest});
            return ((GetInterfacesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetInterfaces(GetInterfacesRequest GetInterfacesRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetInterfaces", new object[] {
                        GetInterfacesRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetInterfacesResponse EndGetInterfaces(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetInterfacesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetInterfacesAsync(GetInterfacesRequest GetInterfacesRequest)
        {
            this.GetInterfacesAsync(GetInterfacesRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetInterfacesAsync(GetInterfacesRequest GetInterfacesRequest, object userState)
        {
            if ((this.GetInterfacesOperationCompleted == null))
            {
                this.GetInterfacesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetInterfacesOperationCompleted);
            }
            this.InvokeAsync("GetInterfaces", new object[] {
                        GetInterfacesRequest}, this.GetInterfacesOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetInterfacesOperationCompleted(object arg)
        {
            if ((this.GetInterfacesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetInterfacesCompleted(this, new GetInterfacesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetIPBlockList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetIPBlockListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetIPBlockListResponse GetIPBlockList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetIPBlockListRequest GetIPBlockListRequest)
        {
            object[] results = this.Invoke("GetIPBlockList", new object[] {
                        GetIPBlockListRequest});
            return ((GetIPBlockListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetIPBlockList(GetIPBlockListRequest GetIPBlockListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetIPBlockList", new object[] {
                        GetIPBlockListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetIPBlockListResponse EndGetIPBlockList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetIPBlockListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetIPBlockListAsync(GetIPBlockListRequest GetIPBlockListRequest)
        {
            this.GetIPBlockListAsync(GetIPBlockListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetIPBlockListAsync(GetIPBlockListRequest GetIPBlockListRequest, object userState)
        {
            if ((this.GetIPBlockListOperationCompleted == null))
            {
                this.GetIPBlockListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetIPBlockListOperationCompleted);
            }
            this.InvokeAsync("GetIPBlockList", new object[] {
                        GetIPBlockListRequest}, this.GetIPBlockListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetIPBlockListOperationCompleted(object arg)
        {
            if ((this.GetIPBlockListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetIPBlockListCompleted(this, new GetIPBlockListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetAutoBlockList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetAutoBlockListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetAutoBlockListResponse GetAutoBlockList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetAutoBlockListRequest GetAutoBlockListRequest)
        {
            object[] results = this.Invoke("GetAutoBlockList", new object[] {
                        GetAutoBlockListRequest});
            return ((GetAutoBlockListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetAutoBlockList(GetAutoBlockListRequest GetAutoBlockListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetAutoBlockList", new object[] {
                        GetAutoBlockListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetAutoBlockListResponse EndGetAutoBlockList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetAutoBlockListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetAutoBlockListAsync(GetAutoBlockListRequest GetAutoBlockListRequest)
        {
            this.GetAutoBlockListAsync(GetAutoBlockListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetAutoBlockListAsync(GetAutoBlockListRequest GetAutoBlockListRequest, object userState)
        {
            if ((this.GetAutoBlockListOperationCompleted == null))
            {
                this.GetAutoBlockListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetAutoBlockListOperationCompleted);
            }
            this.InvokeAsync("GetAutoBlockList", new object[] {
                        GetAutoBlockListRequest}, this.GetAutoBlockListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetAutoBlockListOperationCompleted(object arg)
        {
            if ((this.GetAutoBlockListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetAutoBlockListCompleted(this, new GetAutoBlockListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetAppPaths", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetAppPathsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetAppPathsResponse GetAppPaths([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetAppPathsRequest GetAppPathsRequest)
        {
            object[] results = this.Invoke("GetAppPaths", new object[] {
                        GetAppPathsRequest});
            return ((GetAppPathsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetAppPaths(GetAppPathsRequest GetAppPathsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetAppPaths", new object[] {
                        GetAppPathsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetAppPathsResponse EndGetAppPaths(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetAppPathsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetAppPathsAsync(GetAppPathsRequest GetAppPathsRequest)
        {
            this.GetAppPathsAsync(GetAppPathsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetAppPathsAsync(GetAppPathsRequest GetAppPathsRequest, object userState)
        {
            if ((this.GetAppPathsOperationCompleted == null))
            {
                this.GetAppPathsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetAppPathsOperationCompleted);
            }
            this.InvokeAsync("GetAppPaths", new object[] {
                        GetAppPathsRequest}, this.GetAppPathsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetAppPathsOperationCompleted(object arg)
        {
            if ((this.GetAppPathsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetAppPathsCompleted(this, new GetAppPathsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetLicenseInfo", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetLicenseInfoResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetLicenseInfoResponse GetLicenseInfo([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetLicenseInfoRequest GetLicenseInfoRequest)
        {
            object[] results = this.Invoke("GetLicenseInfo", new object[] {
                        GetLicenseInfoRequest});
            return ((GetLicenseInfoResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetLicenseInfo(GetLicenseInfoRequest GetLicenseInfoRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetLicenseInfo", new object[] {
                        GetLicenseInfoRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetLicenseInfoResponse EndGetLicenseInfo(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetLicenseInfoResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetLicenseInfoAsync(GetLicenseInfoRequest GetLicenseInfoRequest)
        {
            this.GetLicenseInfoAsync(GetLicenseInfoRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetLicenseInfoAsync(GetLicenseInfoRequest GetLicenseInfoRequest, object userState)
        {
            if ((this.GetLicenseInfoOperationCompleted == null))
            {
                this.GetLicenseInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetLicenseInfoOperationCompleted);
            }
            this.InvokeAsync("GetLicenseInfo", new object[] {
                        GetLicenseInfoRequest}, this.GetLicenseInfoOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetLicenseInfoOperationCompleted(object arg)
        {
            if ((this.GetLicenseInfoCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetLicenseInfoCompleted(this, new GetLicenseInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/VerifyLicense", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("VerifyLicenseResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public VerifyLicenseResponse VerifyLicense([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] VerifyLicenseRequest VerifyLicenseRequest)
        {
            object[] results = this.Invoke("VerifyLicense", new object[] {
                        VerifyLicenseRequest});
            return ((VerifyLicenseResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginVerifyLicense(VerifyLicenseRequest VerifyLicenseRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("VerifyLicense", new object[] {
                        VerifyLicenseRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public VerifyLicenseResponse EndVerifyLicense(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((VerifyLicenseResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void VerifyLicenseAsync(VerifyLicenseRequest VerifyLicenseRequest)
        {
            this.VerifyLicenseAsync(VerifyLicenseRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void VerifyLicenseAsync(VerifyLicenseRequest VerifyLicenseRequest, object userState)
        {
            if ((this.VerifyLicenseOperationCompleted == null))
            {
                this.VerifyLicenseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnVerifyLicenseOperationCompleted);
            }
            this.InvokeAsync("VerifyLicense", new object[] {
                        VerifyLicenseRequest}, this.VerifyLicenseOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnVerifyLicenseOperationCompleted(object arg)
        {
            if ((this.VerifyLicenseCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.VerifyLicenseCompleted(this, new VerifyLicenseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetCurrentConnectionCount", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetCurrentConnectionCountResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetCurrentConnectionCountResponse GetCurrentConnectionCount([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetCurrentConnectionCountRequest GetCurrentConnectionCountRequest)
        {
            object[] results = this.Invoke("GetCurrentConnectionCount", new object[] {
                        GetCurrentConnectionCountRequest});
            return ((GetCurrentConnectionCountResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetCurrentConnectionCount(GetCurrentConnectionCountRequest GetCurrentConnectionCountRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetCurrentConnectionCount", new object[] {
                        GetCurrentConnectionCountRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetCurrentConnectionCountResponse EndGetCurrentConnectionCount(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetCurrentConnectionCountResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetCurrentConnectionCountAsync(GetCurrentConnectionCountRequest GetCurrentConnectionCountRequest)
        {
            this.GetCurrentConnectionCountAsync(GetCurrentConnectionCountRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetCurrentConnectionCountAsync(GetCurrentConnectionCountRequest GetCurrentConnectionCountRequest, object userState)
        {
            if ((this.GetCurrentConnectionCountOperationCompleted == null))
            {
                this.GetCurrentConnectionCountOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCurrentConnectionCountOperationCompleted);
            }
            this.InvokeAsync("GetCurrentConnectionCount", new object[] {
                        GetCurrentConnectionCountRequest}, this.GetCurrentConnectionCountOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetCurrentConnectionCountOperationCompleted(object arg)
        {
            if ((this.GetCurrentConnectionCountCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCurrentConnectionCountCompleted(this, new GetCurrentConnectionCountCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetAllCurrentConnectionCount", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetAllCurrentConnectionCountResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetAllCurrentConnectionCountResponse GetAllCurrentConnectionCount([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetAllCurrentConnectionCountRequest GetAllCurrentConnectionCountRequest)
        {
            object[] results = this.Invoke("GetAllCurrentConnectionCount", new object[] {
                        GetAllCurrentConnectionCountRequest});
            return ((GetAllCurrentConnectionCountResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetAllCurrentConnectionCount(GetAllCurrentConnectionCountRequest GetAllCurrentConnectionCountRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetAllCurrentConnectionCount", new object[] {
                        GetAllCurrentConnectionCountRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetAllCurrentConnectionCountResponse EndGetAllCurrentConnectionCount(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetAllCurrentConnectionCountResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetAllCurrentConnectionCountAsync(GetAllCurrentConnectionCountRequest GetAllCurrentConnectionCountRequest)
        {
            this.GetAllCurrentConnectionCountAsync(GetAllCurrentConnectionCountRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetAllCurrentConnectionCountAsync(GetAllCurrentConnectionCountRequest GetAllCurrentConnectionCountRequest, object userState)
        {
            if ((this.GetAllCurrentConnectionCountOperationCompleted == null))
            {
                this.GetAllCurrentConnectionCountOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetAllCurrentConnectionCountOperationCompleted);
            }
            this.InvokeAsync("GetAllCurrentConnectionCount", new object[] {
                        GetAllCurrentConnectionCountRequest}, this.GetAllCurrentConnectionCountOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetAllCurrentConnectionCountOperationCompleted(object arg)
        {
            if ((this.GetAllCurrentConnectionCountCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetAllCurrentConnectionCountCompleted(this, new GetAllCurrentConnectionCountCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetInterfaceByID", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetInterfaceResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetInterfaceResponse GetInterfaceByID([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetInterfaceByIDRequest GetInterfaceByIDRequest)
        {
            object[] results = this.Invoke("GetInterfaceByID", new object[] {
                        GetInterfaceByIDRequest});
            return ((GetInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetInterfaceByID(GetInterfaceByIDRequest GetInterfaceByIDRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetInterfaceByID", new object[] {
                        GetInterfaceByIDRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetInterfaceResponse EndGetInterfaceByID(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetInterfaceByIDAsync(GetInterfaceByIDRequest GetInterfaceByIDRequest)
        {
            this.GetInterfaceByIDAsync(GetInterfaceByIDRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetInterfaceByIDAsync(GetInterfaceByIDRequest GetInterfaceByIDRequest, object userState)
        {
            if ((this.GetInterfaceByIDOperationCompleted == null))
            {
                this.GetInterfaceByIDOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetInterfaceByIDOperationCompleted);
            }
            this.InvokeAsync("GetInterfaceByID", new object[] {
                        GetInterfaceByIDRequest}, this.GetInterfaceByIDOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetInterfaceByIDOperationCompleted(object arg)
        {
            if ((this.GetInterfaceByIDCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetInterfaceByIDCompleted(this, new GetInterfaceByIDCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetInterfaceList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetInterfaceListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetInterfaceListResponse GetInterfaceList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetInterfaceListRequest GetInterfaceListRequest)
        {
            object[] results = this.Invoke("GetInterfaceList", new object[] {
                        GetInterfaceListRequest});
            return ((GetInterfaceListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetInterfaceList(GetInterfaceListRequest GetInterfaceListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetInterfaceList", new object[] {
                        GetInterfaceListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetInterfaceListResponse EndGetInterfaceList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetInterfaceListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetInterfaceListAsync(GetInterfaceListRequest GetInterfaceListRequest)
        {
            this.GetInterfaceListAsync(GetInterfaceListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetInterfaceListAsync(GetInterfaceListRequest GetInterfaceListRequest, object userState)
        {
            if ((this.GetInterfaceListOperationCompleted == null))
            {
                this.GetInterfaceListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetInterfaceListOperationCompleted);
            }
            this.InvokeAsync("GetInterfaceList", new object[] {
                        GetInterfaceListRequest}, this.GetInterfaceListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetInterfaceListOperationCompleted(object arg)
        {
            if ((this.GetInterfaceListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetInterfaceListCompleted(this, new GetInterfaceListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/InitializeInterface", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("InitializeInterfaceResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public InitializeInterfaceResponse InitializeInterface([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] InitializeInterfaceRequest InitializeInterfaceRequest)
        {
            object[] results = this.Invoke("InitializeInterface", new object[] {
                        InitializeInterfaceRequest});
            return ((InitializeInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginInitializeInterface(InitializeInterfaceRequest InitializeInterfaceRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("InitializeInterface", new object[] {
                        InitializeInterfaceRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public InitializeInterfaceResponse EndInitializeInterface(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((InitializeInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void InitializeInterfaceAsync(InitializeInterfaceRequest InitializeInterfaceRequest)
        {
            this.InitializeInterfaceAsync(InitializeInterfaceRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void InitializeInterfaceAsync(InitializeInterfaceRequest InitializeInterfaceRequest, object userState)
        {
            if ((this.InitializeInterfaceOperationCompleted == null))
            {
                this.InitializeInterfaceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnInitializeInterfaceOperationCompleted);
            }
            this.InvokeAsync("InitializeInterface", new object[] {
                        InitializeInterfaceRequest}, this.InitializeInterfaceOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnInitializeInterfaceOperationCompleted(object arg)
        {
            if ((this.InitializeInterfaceCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.InitializeInterfaceCompleted(this, new InitializeInterfaceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/ShutdownInterface", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ShutdownInterfaceResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public ShutdownInterfaceResponse ShutdownInterface([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] ShutdownInterfaceRequest ShutdownInterfaceRequest)
        {
            object[] results = this.Invoke("ShutdownInterface", new object[] {
                        ShutdownInterfaceRequest});
            return ((ShutdownInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginShutdownInterface(ShutdownInterfaceRequest ShutdownInterfaceRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ShutdownInterface", new object[] {
                        ShutdownInterfaceRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public ShutdownInterfaceResponse EndShutdownInterface(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ShutdownInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ShutdownInterfaceAsync(ShutdownInterfaceRequest ShutdownInterfaceRequest)
        {
            this.ShutdownInterfaceAsync(ShutdownInterfaceRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ShutdownInterfaceAsync(ShutdownInterfaceRequest ShutdownInterfaceRequest, object userState)
        {
            if ((this.ShutdownInterfaceOperationCompleted == null))
            {
                this.ShutdownInterfaceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnShutdownInterfaceOperationCompleted);
            }
            this.InvokeAsync("ShutdownInterface", new object[] {
                        ShutdownInterfaceRequest}, this.ShutdownInterfaceOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnShutdownInterfaceOperationCompleted(object arg)
        {
            if ((this.ShutdownInterfaceCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ShutdownInterfaceCompleted(this, new ShutdownInterfaceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetStatistics", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetStatisticsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetStatisticsResponse GetStatistics([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetStatisticsRequest GetStatisticsRequest)
        {
            object[] results = this.Invoke("GetStatistics", new object[] {
                        GetStatisticsRequest});
            return ((GetStatisticsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetStatistics(GetStatisticsRequest GetStatisticsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetStatistics", new object[] {
                        GetStatisticsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetStatisticsResponse EndGetStatistics(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetStatisticsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetStatisticsAsync(GetStatisticsRequest GetStatisticsRequest)
        {
            this.GetStatisticsAsync(GetStatisticsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetStatisticsAsync(GetStatisticsRequest GetStatisticsRequest, object userState)
        {
            if ((this.GetStatisticsOperationCompleted == null))
            {
                this.GetStatisticsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetStatisticsOperationCompleted);
            }
            this.InvokeAsync("GetStatistics", new object[] {
                        GetStatisticsRequest}, this.GetStatisticsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetStatisticsOperationCompleted(object arg)
        {
            if ((this.GetStatisticsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetStatisticsCompleted(this, new GetStatisticsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetCurrentBandwidth", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetCurrentBandwidthResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetCurrentBandwidthResponse GetCurrentBandwidth([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetCurrentBandwidthRequest GetCurrentBandwidthRequest)
        {
            object[] results = this.Invoke("GetCurrentBandwidth", new object[] {
                        GetCurrentBandwidthRequest});
            return ((GetCurrentBandwidthResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetCurrentBandwidth(GetCurrentBandwidthRequest GetCurrentBandwidthRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetCurrentBandwidth", new object[] {
                        GetCurrentBandwidthRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetCurrentBandwidthResponse EndGetCurrentBandwidth(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetCurrentBandwidthResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetCurrentBandwidthAsync(GetCurrentBandwidthRequest GetCurrentBandwidthRequest)
        {
            this.GetCurrentBandwidthAsync(GetCurrentBandwidthRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetCurrentBandwidthAsync(GetCurrentBandwidthRequest GetCurrentBandwidthRequest, object userState)
        {
            if ((this.GetCurrentBandwidthOperationCompleted == null))
            {
                this.GetCurrentBandwidthOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCurrentBandwidthOperationCompleted);
            }
            this.InvokeAsync("GetCurrentBandwidth", new object[] {
                        GetCurrentBandwidthRequest}, this.GetCurrentBandwidthOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetCurrentBandwidthOperationCompleted(object arg)
        {
            if ((this.GetCurrentBandwidthCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCurrentBandwidthCompleted(this, new GetCurrentBandwidthCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetFeatures", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetFeaturesResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetFeaturesResponse GetFeatures([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetFeaturesRequest GetFeaturesRequest)
        {
            object[] results = this.Invoke("GetFeatures", new object[] {
                        GetFeaturesRequest});
            return ((GetFeaturesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetFeatures(GetFeaturesRequest GetFeaturesRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetFeatures", new object[] {
                        GetFeaturesRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetFeaturesResponse EndGetFeatures(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetFeaturesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetFeaturesAsync(GetFeaturesRequest GetFeaturesRequest)
        {
            this.GetFeaturesAsync(GetFeaturesRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetFeaturesAsync(GetFeaturesRequest GetFeaturesRequest, object userState)
        {
            if ((this.GetFeaturesOperationCompleted == null))
            {
                this.GetFeaturesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFeaturesOperationCompleted);
            }
            this.InvokeAsync("GetFeatures", new object[] {
                        GetFeaturesRequest}, this.GetFeaturesOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetFeaturesOperationCompleted(object arg)
        {
            if ((this.GetFeaturesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFeaturesCompleted(this, new GetFeaturesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SaveProfiles", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SaveProfilesResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SaveProfilesResponse SaveProfiles([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SaveProfilesRequest SaveProfilesRequest)
        {
            object[] results = this.Invoke("SaveProfiles", new object[] {
                        SaveProfilesRequest});
            return ((SaveProfilesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSaveProfiles(SaveProfilesRequest SaveProfilesRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SaveProfiles", new object[] {
                        SaveProfilesRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SaveProfilesResponse EndSaveProfiles(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SaveProfilesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveProfilesAsync(SaveProfilesRequest SaveProfilesRequest)
        {
            this.SaveProfilesAsync(SaveProfilesRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveProfilesAsync(SaveProfilesRequest SaveProfilesRequest, object userState)
        {
            if ((this.SaveProfilesOperationCompleted == null))
            {
                this.SaveProfilesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSaveProfilesOperationCompleted);
            }
            this.InvokeAsync("SaveProfiles", new object[] {
                        SaveProfilesRequest}, this.SaveProfilesOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSaveProfilesOperationCompleted(object arg)
        {
            if ((this.SaveProfilesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SaveProfilesCompleted(this, new SaveProfilesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SaveConfiguration", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SaveConfigurationResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SaveConfigurationResponse SaveConfiguration([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SaveConfigurationRequest SaveConfigurationRequest)
        {
            object[] results = this.Invoke("SaveConfiguration", new object[] {
                        SaveConfigurationRequest});
            return ((SaveConfigurationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSaveConfiguration(SaveConfigurationRequest SaveConfigurationRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SaveConfiguration", new object[] {
                        SaveConfigurationRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SaveConfigurationResponse EndSaveConfiguration(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SaveConfigurationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveConfigurationAsync(SaveConfigurationRequest SaveConfigurationRequest)
        {
            this.SaveConfigurationAsync(SaveConfigurationRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveConfigurationAsync(SaveConfigurationRequest SaveConfigurationRequest, object userState)
        {
            if ((this.SaveConfigurationOperationCompleted == null))
            {
                this.SaveConfigurationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSaveConfigurationOperationCompleted);
            }
            this.InvokeAsync("SaveConfiguration", new object[] {
                        SaveConfigurationRequest}, this.SaveConfigurationOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSaveConfigurationOperationCompleted(object arg)
        {
            if ((this.SaveConfigurationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SaveConfigurationCompleted(this, new SaveConfigurationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/CommitSettings", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("CommitSettingsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public CommitSettingsResponse CommitSettings([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] CommitSettingsRequest CommitSettingsRequest)
        {
            object[] results = this.Invoke("CommitSettings", new object[] {
                        CommitSettingsRequest});
            return ((CommitSettingsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginCommitSettings(CommitSettingsRequest CommitSettingsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CommitSettings", new object[] {
                        CommitSettingsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public CommitSettingsResponse EndCommitSettings(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((CommitSettingsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void CommitSettingsAsync(CommitSettingsRequest CommitSettingsRequest)
        {
            this.CommitSettingsAsync(CommitSettingsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void CommitSettingsAsync(CommitSettingsRequest CommitSettingsRequest, object userState)
        {
            if ((this.CommitSettingsOperationCompleted == null))
            {
                this.CommitSettingsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCommitSettingsOperationCompleted);
            }
            this.InvokeAsync("CommitSettings", new object[] {
                        CommitSettingsRequest}, this.CommitSettingsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnCommitSettingsOperationCompleted(object arg)
        {
            if ((this.CommitSettingsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CommitSettingsCompleted(this, new CommitSettingsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/SaveBlockList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("SaveBlockListResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public SaveBlockListResponse SaveBlockList([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] SaveBlockListRequest SaveBlockListRequest)
        {
            object[] results = this.Invoke("SaveBlockList", new object[] {
                        SaveBlockListRequest});
            return ((SaveBlockListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginSaveBlockList(SaveBlockListRequest SaveBlockListRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SaveBlockList", new object[] {
                        SaveBlockListRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public SaveBlockListResponse EndSaveBlockList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((SaveBlockListResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveBlockListAsync(SaveBlockListRequest SaveBlockListRequest)
        {
            this.SaveBlockListAsync(SaveBlockListRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void SaveBlockListAsync(SaveBlockListRequest SaveBlockListRequest, object userState)
        {
            if ((this.SaveBlockListOperationCompleted == null))
            {
                this.SaveBlockListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSaveBlockListOperationCompleted);
            }
            this.InvokeAsync("SaveBlockList", new object[] {
                        SaveBlockListRequest}, this.SaveBlockListOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnSaveBlockListOperationCompleted(object arg)
        {
            if ((this.SaveBlockListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SaveBlockListCompleted(this, new SaveBlockListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/ModifyInterface", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ModifyInterfaceResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public ModifyInterfaceResponse ModifyInterface([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] ModifyInterfaceRequest ModifyInterfaceRequest)
        {
            object[] results = this.Invoke("ModifyInterface", new object[] {
                        ModifyInterfaceRequest});
            return ((ModifyInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginModifyInterface(ModifyInterfaceRequest ModifyInterfaceRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ModifyInterface", new object[] {
                        ModifyInterfaceRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public ModifyInterfaceResponse EndModifyInterface(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ModifyInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ModifyInterfaceAsync(ModifyInterfaceRequest ModifyInterfaceRequest)
        {
            this.ModifyInterfaceAsync(ModifyInterfaceRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ModifyInterfaceAsync(ModifyInterfaceRequest ModifyInterfaceRequest, object userState)
        {
            if ((this.ModifyInterfaceOperationCompleted == null))
            {
                this.ModifyInterfaceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnModifyInterfaceOperationCompleted);
            }
            this.InvokeAsync("ModifyInterface", new object[] {
                        ModifyInterfaceRequest}, this.ModifyInterfaceOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnModifyInterfaceOperationCompleted(object arg)
        {
            if ((this.ModifyInterfaceCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ModifyInterfaceCompleted(this, new ModifyInterfaceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/ShutdownConnectionsOnInterface", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ShutdownConnectionsOnInterfaceResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public ShutdownConnectionsOnInterfaceResponse ShutdownConnectionsOnInterface([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] ShutdownConnectionsOnInterfaceRequest ShutdownConnectionsOnInterfaceRequest)
        {
            object[] results = this.Invoke("ShutdownConnectionsOnInterface", new object[] {
                        ShutdownConnectionsOnInterfaceRequest});
            return ((ShutdownConnectionsOnInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginShutdownConnectionsOnInterface(ShutdownConnectionsOnInterfaceRequest ShutdownConnectionsOnInterfaceRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ShutdownConnectionsOnInterface", new object[] {
                        ShutdownConnectionsOnInterfaceRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public ShutdownConnectionsOnInterfaceResponse EndShutdownConnectionsOnInterface(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ShutdownConnectionsOnInterfaceResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ShutdownConnectionsOnInterfaceAsync(ShutdownConnectionsOnInterfaceRequest ShutdownConnectionsOnInterfaceRequest)
        {
            this.ShutdownConnectionsOnInterfaceAsync(ShutdownConnectionsOnInterfaceRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void ShutdownConnectionsOnInterfaceAsync(ShutdownConnectionsOnInterfaceRequest ShutdownConnectionsOnInterfaceRequest, object userState)
        {
            if ((this.ShutdownConnectionsOnInterfaceOperationCompleted == null))
            {
                this.ShutdownConnectionsOnInterfaceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnShutdownConnectionsOnInterfaceOperationCompleted);
            }
            this.InvokeAsync("ShutdownConnectionsOnInterface", new object[] {
                        ShutdownConnectionsOnInterfaceRequest}, this.ShutdownConnectionsOnInterfaceOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnShutdownConnectionsOnInterfaceOperationCompleted(object arg)
        {
            if ((this.ShutdownConnectionsOnInterfaceCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ShutdownConnectionsOnInterfaceCompleted(this, new ShutdownConnectionsOnInterfaceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetFileTransfers", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetFileTransfersResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetFileTransfersResponse GetFileTransfers([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetFileTransfersRequest GetFileTransfersRequest)
        {
            object[] results = this.Invoke("GetFileTransfers", new object[] {
                        GetFileTransfersRequest});
            return ((GetFileTransfersResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetFileTransfers(GetFileTransfersRequest GetFileTransfersRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetFileTransfers", new object[] {
                        GetFileTransfersRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetFileTransfersResponse EndGetFileTransfers(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetFileTransfersResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetFileTransfersAsync(GetFileTransfersRequest GetFileTransfersRequest)
        {
            this.GetFileTransfersAsync(GetFileTransfersRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetFileTransfersAsync(GetFileTransfersRequest GetFileTransfersRequest, object userState)
        {
            if ((this.GetFileTransfersOperationCompleted == null))
            {
                this.GetFileTransfersOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFileTransfersOperationCompleted);
            }
            this.InvokeAsync("GetFileTransfers", new object[] {
                        GetFileTransfersRequest}, this.GetFileTransfersOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetFileTransfersOperationCompleted(object arg)
        {
            if ((this.GetFileTransfersCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFileTransfersCompleted(this, new GetFileTransfersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GetLogMessages", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetLogMessagesResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GetLogMessagesResponse GetLogMessages([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GetLogMessagesRequest GetLogMessagesRequest)
        {
            object[] results = this.Invoke("GetLogMessages", new object[] {
                        GetLogMessagesRequest});
            return ((GetLogMessagesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGetLogMessages(GetLogMessagesRequest GetLogMessagesRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetLogMessages", new object[] {
                        GetLogMessagesRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GetLogMessagesResponse EndGetLogMessages(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GetLogMessagesResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetLogMessagesAsync(GetLogMessagesRequest GetLogMessagesRequest)
        {
            this.GetLogMessagesAsync(GetLogMessagesRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GetLogMessagesAsync(GetLogMessagesRequest GetLogMessagesRequest, object userState)
        {
            if ((this.GetLogMessagesOperationCompleted == null))
            {
                this.GetLogMessagesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetLogMessagesOperationCompleted);
            }
            this.InvokeAsync("GetLogMessages", new object[] {
                        GetLogMessagesRequest}, this.GetLogMessagesOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGetLogMessagesOperationCompleted(object arg)
        {
            if ((this.GetLogMessagesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetLogMessagesCompleted(this, new GetLogMessagesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/BlockAddress", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("BlockAddressResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public BlockAddressResponse BlockAddress([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] BlockAddressRequest BlockAddressRequest)
        {
            object[] results = this.Invoke("BlockAddress", new object[] {
                        BlockAddressRequest});
            return ((BlockAddressResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginBlockAddress(BlockAddressRequest BlockAddressRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("BlockAddress", new object[] {
                        BlockAddressRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public BlockAddressResponse EndBlockAddress(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((BlockAddressResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void BlockAddressAsync(BlockAddressRequest BlockAddressRequest)
        {
            this.BlockAddressAsync(BlockAddressRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void BlockAddressAsync(BlockAddressRequest BlockAddressRequest, object userState)
        {
            if ((this.BlockAddressOperationCompleted == null))
            {
                this.BlockAddressOperationCompleted = new System.Threading.SendOrPostCallback(this.OnBlockAddressOperationCompleted);
            }
            this.InvokeAsync("BlockAddress", new object[] {
                        BlockAddressRequest}, this.BlockAddressOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnBlockAddressOperationCompleted(object arg)
        {
            if ((this.BlockAddressCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.BlockAddressCompleted(this, new BlockAddressCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/GenerateStatistics", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GenerateStatisticsResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public GenerateStatisticsResponse GenerateStatistics([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] GenerateStatisticsRequest GenerateStatisticsRequest)
        {
            object[] results = this.Invoke("GenerateStatistics", new object[] {
                        GenerateStatisticsRequest});
            return ((GenerateStatisticsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginGenerateStatistics(GenerateStatisticsRequest GenerateStatisticsRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GenerateStatistics", new object[] {
                        GenerateStatisticsRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public GenerateStatisticsResponse EndGenerateStatistics(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((GenerateStatisticsResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GenerateStatisticsAsync(GenerateStatisticsRequest GenerateStatisticsRequest)
        {
            this.GenerateStatisticsAsync(GenerateStatisticsRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void GenerateStatisticsAsync(GenerateStatisticsRequest GenerateStatisticsRequest, object userState)
        {
            if ((this.GenerateStatisticsOperationCompleted == null))
            {
                this.GenerateStatisticsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGenerateStatisticsOperationCompleted);
            }
            this.InvokeAsync("GenerateStatistics", new object[] {
                        GenerateStatisticsRequest}, this.GenerateStatisticsOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnGenerateStatisticsOperationCompleted(object arg)
        {
            if ((this.GenerateStatisticsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GenerateStatisticsCompleted(this, new GenerateStatisticsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/BackupServerConfiguration", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("BackupServerConfigurationResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public BackupServerConfigurationResponse BackupServerConfiguration([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] BackupServerConfigurationRequest BackupServerConfigurationRequest)
        {
            object[] results = this.Invoke("BackupServerConfiguration", new object[] {
                        BackupServerConfigurationRequest});
            return ((BackupServerConfigurationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginBackupServerConfiguration(BackupServerConfigurationRequest BackupServerConfigurationRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("BackupServerConfiguration", new object[] {
                        BackupServerConfigurationRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public BackupServerConfigurationResponse EndBackupServerConfiguration(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((BackupServerConfigurationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void BackupServerConfigurationAsync(BackupServerConfigurationRequest BackupServerConfigurationRequest)
        {
            this.BackupServerConfigurationAsync(BackupServerConfigurationRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void BackupServerConfigurationAsync(BackupServerConfigurationRequest BackupServerConfigurationRequest, object userState)
        {
            if ((this.BackupServerConfigurationOperationCompleted == null))
            {
                this.BackupServerConfigurationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnBackupServerConfigurationOperationCompleted);
            }
            this.InvokeAsync("BackupServerConfiguration", new object[] {
                        BackupServerConfigurationRequest}, this.BackupServerConfigurationOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnBackupServerConfigurationOperationCompleted(object arg)
        {
            if ((this.BackupServerConfigurationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.BackupServerConfigurationCompleted(this, new BackupServerConfigurationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://cerberusllc.com/service/cerberusftpservice/RestoreServerConfiguration", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("RestoreServerConfigurationResponse", Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
        /// <summary>TODO</summary>
        public RestoreServerConfigurationResponse RestoreServerConfiguration([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://cerberusllc.com/service/cerberusftpservice")] RestoreServerConfigurationRequest RestoreServerConfigurationRequest)
        {
            object[] results = this.Invoke("RestoreServerConfiguration", new object[] {
                        RestoreServerConfigurationRequest});
            return ((RestoreServerConfigurationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public System.IAsyncResult BeginRestoreServerConfiguration(RestoreServerConfigurationRequest RestoreServerConfigurationRequest, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RestoreServerConfiguration", new object[] {
                        RestoreServerConfigurationRequest}, callback, asyncState);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public RestoreServerConfigurationResponse EndRestoreServerConfiguration(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((RestoreServerConfigurationResponse)(results[0]));
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void RestoreServerConfigurationAsync(RestoreServerConfigurationRequest RestoreServerConfigurationRequest)
        {
            this.RestoreServerConfigurationAsync(RestoreServerConfigurationRequest, null);
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public void RestoreServerConfigurationAsync(RestoreServerConfigurationRequest RestoreServerConfigurationRequest, object userState)
        {
            if ((this.RestoreServerConfigurationOperationCompleted == null))
            {
                this.RestoreServerConfigurationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRestoreServerConfigurationOperationCompleted);
            }
            this.InvokeAsync("RestoreServerConfiguration", new object[] {
                        RestoreServerConfigurationRequest}, this.RestoreServerConfigurationOperationCompleted, userState);
        }
        /// <summary>Auto-generated member.</summary>

        private void OnRestoreServerConfigurationOperationCompleted(object arg)
        {
            if ((this.RestoreServerConfigurationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RestoreServerConfigurationCompleted(this, new RestoreServerConfigurationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        /// <summary>Auto-generated member.</summary>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetBackupServersRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Credentials credentials;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Credentials
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string user;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string password;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class ImportFileResult
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string file;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string message;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool success;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class LogMessage
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string msg;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public ulong id;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public int type;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public System.DateTime time;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class FileTransfer
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string localFilename;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string remoteFilename;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string user;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong percentElapsed;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong currentPosition;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong totalSize;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public double transferRate;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string timeLeft;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public ulong ID;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public TransferType type;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool isSecure;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum TransferType
    {

        /// <uwagi/>
        Download,

        /// <uwagi/>
        Upload,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Features
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public int maxConnectionLimit;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableActiveDirectory;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableLDAPAuthentication;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableFIPS1402;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableRemoteAdmin;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableClientCertificateVerify;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableSshFtp;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableHttp;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableEvents;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableReports;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableServerSync;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public int productEditionCode;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class FileHitInfo
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string user;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public TransferType type;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class FileHit
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("list")]
        /// <summary>TODO</summary>
        public FileHitInfo[] list;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string file;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class PassiveOpts
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ipAddress;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string dnsAddress;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool dontUseExternalIPForLocal;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public PassiveMode mode;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum PassiveMode
    {

        /// <uwagi/>
        Auto,

        /// <uwagi/>
        DirectIP,

        /// <uwagi/>
        DNS,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class InterfaceOpts
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isActive;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowLogin;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public uint listenPort;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public int connectionLimit;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public PassiveOpts passiveSettings;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool requiresSecureControl;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool requiresSecureData;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string logoPath;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string loginIconPath;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string companyName;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowWebAccountRequest;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool showWelcomeMsg;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool redirectToHttps;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public int defaultWebDirList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool showTimezone;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool showLocalTime;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowUpdate;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool useHSTS;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string captchaPrivKey;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string captchaPubKey;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool captchaShowLogin;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool captchaShowReq;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Interface
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long id;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public InterfaceOpts options;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string ipAddress;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public uint type;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class LicenseInfo
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string name;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string companyName;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong issuedDate;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public int upgradeTimeRemaining;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public int clientCount;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public int daysValid;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong installedDate;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isValid;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isForCompanyUse;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string productEdition;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string errorMessage;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Connection
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong id;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long interfaceID;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string userName;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string lastCommand;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isSecure;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ipAddr;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public System.DateTime loginTime;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string commandProgress;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string client;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Group
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyAuthentication authMethod;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ProtocolsAllowed protocols;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("root", IsNullable = false)]
        /// <summary>TODO</summary>
        public VirtualDirectory[] rootList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool isAllowPasswordChange;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool isAllowPasswordChangeSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool isAnonymous;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool isAnonymousSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool isSimpleDirectoryMode;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool isSimpleDirectoryModeSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool isDisabled;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool isDisabledSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public int maxLoginsAllowed;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool maxLoginsAllowedSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool requireSecureControl;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool requireSecureControlSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool requireSecureData;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool requireSecureDataSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public System.DateTime disableAfterTime;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool disableAfterTimeSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public ulong maxUploadFilesize;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool maxUploadFilesizeSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string ipAllowedList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string desc;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string name;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class UserPropertyAuthentication
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public AuthenticationMethod method;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool methodSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public UserPropertyPriority priority;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool prioritySpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlTextAttribute()]
        /// <summary>TODO</summary>
        public string Value;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum AuthenticationMethod
    {

        /// <uwagi/>
        password,

        /// <uwagi/>
        public_key,

        /// <uwagi/>
        password_and_public_key,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum UserPropertyPriority
    {

        /// <uwagi/>
        user,

        /// <uwagi/>
        group,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class ProtocolsAllowed
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public UserPropertyPriority priority;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool prioritySpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool ftp;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool ftps;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool sftp;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool http;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool https;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class VirtualDirectory
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string name;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string path;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public DirectoryPermissions permissions;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class DirectoryPermissions
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowListFile;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowListDir;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowDownload;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowUpload;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowRename;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowDelete;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowDirectoryCreation;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowDisplayHidden;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowZip;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowUnzip;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool allowShare;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class groupMember
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string name;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class UserPropertyString
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string value;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public UserPropertyPriority priority;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool prioritySpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class UserPropertyULong
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public ulong value;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool valueSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public UserPropertyPriority priority;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool prioritySpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class UserPropertyDateTime
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public System.DateTime value;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool valueSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public UserPropertyPriority priority;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool prioritySpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class UserPropertyInt
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public int value;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool valueSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public UserPropertyPriority priority;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool prioritySpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class UserPropertyBool
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool value;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool valueSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public UserPropertyPriority priority;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool prioritySpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Password
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string value;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public PasswordType type;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool noExpire;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public System.DateTime lastChange;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum PasswordType
    {

        /// <uwagi/>
        plain,

        /// <uwagi/>
        sha1,

        /// <uwagi/>
        sha256,

        /// <uwagi/>
        sha512,

        /// <uwagi/>
        md5,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class User
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Password password;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyBool isAllowPasswordChange;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyBool isAnonymous;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyBool isSimpleDirectoryMode;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyBool isDisabled;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyInt maxLoginsAllowed;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyBool requireSecureControl;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyBool requireSecureData;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyDateTime disableAfterTime;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyAuthentication authMethod;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ProtocolsAllowed protocols;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyULong maxUploadFilesize;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UserPropertyString ipAllowedList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("group", IsNullable = false)]
        /// <summary>TODO</summary>
        public groupMember[] groupList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("root", IsNullable = false)]
        /// <summary>TODO</summary>
        public VirtualDirectory[] rootList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string fname;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string sname;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string email;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string tel;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string desc;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string name;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class NewWANIP
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ip;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool overridePassiveMode;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class AuthenticationType
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enabled;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string name;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string type;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string description;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Statistics
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong downloads;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong uploads;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong failedDownloads;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong failedUploads;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong totalConnections;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong currentConnections;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Status
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isStarted;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Statistics stats;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public double downBandwidth;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public double upBandwidth;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ulong totalConnections;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public ulong activeListenerConnections;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool activeListenerConnectionsSpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class ServerInformation
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Version version;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string hostname;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isStarted;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool isSuccess;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool isSuccessSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class Version
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public int maj;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public int min;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public int maint;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public int build;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class ChangeDescription
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Version ver;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("item")]
        /// <summary>TODO</summary>
        public string[] item;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public uint s;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(VersionUpdateInfo))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class VersionInfo
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Version ver;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public System.DateTime buildDate;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public ProductStatus status;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public ProcessArch arch;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public double minOS;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum ProductStatus
    {

        /// <uwagi/>
        production,

        /// <uwagi/>
        beta,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum ProcessArch
    {

        /// <uwagi/>
        Unknown,

        /// <uwagi/>
        I386,

        /// <uwagi/>
        Ia64,

        /// <uwagi/>
        Amd64,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    /// <summary>TODO</summary>
    public partial class VersionUpdateInfo : VersionInfo
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string downloadURL;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("changeDescList")]
        /// <summary>TODO</summary>
        public ChangeDescription[] changeDescList;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class UpdateInformation
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public VersionInfo currentVer;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public VersionUpdateInfo latestVer;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public System.DateTime lastChecked;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class ServerSummaryInfo
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isSslEnabled;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string sslKeyType;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public uint sslKeyBits;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public uint sslCipherMinSymmetricBits;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isClientVerifyEnabled;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isFipsEnabled;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isSoapWebEnabled;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool isSoapSecure;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public uint soapPort;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ipPublic;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public InterfaceStatus ftpStatus;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("ftpStatusMsgs")]
        /// <summary>TODO</summary>
        public string[] ftpStatusMsgs;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public InterfaceStatus sftpStatus;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("sftpStatusMsgs")]
        /// <summary>TODO</summary>
        public string[] sftpStatusMsgs;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public InterfaceStatus httpStatus;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("httpStatusMsgs")]
        /// <summary>TODO</summary>
        public string[] httpStatusMsgs;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool hipaaCompliant;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("hipaaCompliantMsgs")]
        /// <summary>TODO</summary>
        public string[] hipaaCompliantMsgs;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("generalMsgs")]
        /// <summary>TODO</summary>
        public string[] generalMsgs;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("securityMsgs")]
        /// <summary>TODO</summary>
        public string[] securityMsgs;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public VulnerabilityAssessmentStatus vulnerabilityStatus;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public UpdateInformation updateInfo;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum InterfaceStatus
    {

        /// <uwagi/>
        Secure,

        /// <uwagi/>
        NotSecure,

        /// <uwagi/>
        Disabled,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum VulnerabilityAssessmentStatus
    {

        /// <uwagi/>
        None,

        /// <uwagi/>
        Detected,

        /// <uwagi/>
        Disabled,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class MimeMapping
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ext;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string type;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class DbOperationResult
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool success;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string label;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class DbDriverDescription
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string name;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class DbConfig
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public DbDriverDescription driver;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string server;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public uint port;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string username;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string password;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string databaseName;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string databasePath;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class IpAccessRange
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string addressFrom;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string addressTo;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public IpAccessRangeType type;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum IpAccessRangeType
    {

        /// <uwagi/>
        single,

        /// <uwagi/>
        range,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class IpAccessEntry
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public IpAccessRange entry;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string note;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public System.DateTime blockedSince;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool blockedSinceSpecified;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public int blockForMinutes;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool blockForMinutesSpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class SyncServer
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string host;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ushort port;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool useSSL;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string username;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string password;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enableSync;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public SyncStatus lastSyncStatus;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public System.DateTime lastSyncTime;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string lastSyncStatusMessage;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public enum SyncStatus
    {

        /// <uwagi/>
        unknown,

        /// <uwagi/>
        success,

        /// <uwagi/>
        fail,
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class SyncServerConfig
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool enable;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public uint syncIntervalMinutes;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool syncOnServerChange;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://cerberusllc.com/common")]
    public partial class SyncServerType
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public SyncServerConfig config;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("server", IsNullable = false)]
        /// <summary>TODO</summary>
        public SyncServer[] servers;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetBackupServersResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public SyncServerType config;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SaveBackupServersRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public SyncServerType config;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SaveBackupServersResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SharePublicFileRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string username;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string password;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string shareRemoteFilePath;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public int shareDurationHours;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string sharePassword;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool shareDeleteOnExpire;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool shareDeleteOnExpireSpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SharePublicFileResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool success;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string link;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class AddIpRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("ipEntry")]
        /// <summary>TODO</summary>
        public IpAccessEntry[] ipEntry;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class AddIpResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string errorMsg;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class DeleteIpRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("ipEntry")]
        /// <summary>TODO</summary>
        public IpAccessRange[] ipEntry;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class DeleteIpResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string errorMsg;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class TestAndVerifyDatabaseRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public DbConfig config;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class TestAndVerifyDatabaseResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("result", Namespace = "http://cerberusllc.com/common")]
        /// <summary>TODO</summary>
        public DbOperationResult[] info;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class CreateStatisticsDatabaseRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public DbConfig config;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class CreateStatisticsDatabaseResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("result", Namespace = "http://cerberusllc.com/common")]
        /// <summary>TODO</summary>
        public DbOperationResult[] info;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class DropStatisticsDatabaseRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public DbConfig config;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class DropStatisticsDatabaseResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("result", Namespace = "http://cerberusllc.com/common")]
        /// <summary>TODO</summary>
        public DbOperationResult[] info;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetMimeMappingsRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetMimeMappingsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("map", Namespace = "http://cerberusllc.com/common")]
        /// <summary>TODO</summary>
        public MimeMapping[] mime;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SaveMimeMappingsRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("map", Namespace = "http://cerberusllc.com/common")]
        /// <summary>TODO</summary>
        public MimeMapping[] mime;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SaveMimeMappingsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string errorMsg;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class ServerSummaryStatusRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class ServerSummaryStatusResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ServerSummaryInfo result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class ServerInformationRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class ServerInformationResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public ServerInformation result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class CurrentStatusRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public long activeInterfaceId;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool activeInterfaceIdSpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class CurrentStatusResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Status result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class StartServerRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class StartServerResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class StopServerRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class StopServerResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class ServerStartedRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class ServerStartedResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class InitializeServerRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class InitializeServerResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class ShutdownServerRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class ShutdownServerResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetEventRulesRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetEventRulesResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string dataXml;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SetEventRulesRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string dataXml;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SetEventRulesResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class DeleteRequestedAccountsRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("guids")]
        /// <summary>TODO</summary>
        public string[] guids;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class DeleteRequestedAccountsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("errors")]
        /// <summary>TODO</summary>
        public string[] errors;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetRequestedAccountsRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetRequestedAccountsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string dataXml;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SetRequestedAccountsRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string dataXml;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SetRequestedAccountsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetAuthenticationListRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetAuthenticationListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string dataXml;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("authenticationList")]
        /// <summary>TODO</summary>
        public AuthenticationType[] authenticationList;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SetAuthenticationListRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string dataXml;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("authenticationList")]
        /// <summary>TODO</summary>
        public AuthenticationType[] authenticationList;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SetAuthenticationListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetHostnameRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetHostnameResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SetWANIPRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public NewWANIP newWANInfo;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SetWANIPResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class AddUserRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public User User;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class AddUserResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class AddGroupRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Group Group;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class AddGroupResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class DeleteUserRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string name;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class DeleteUserResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class DeleteGroupRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string name;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class DeleteGroupResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class AddRootRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string userName;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public VirtualDirectory Root;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class AddRootResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class DeleteRootRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string userName;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string dirName;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class DeleteRootResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetUserListRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetUserListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("UserList")]
        /// <summary>TODO</summary>
        public string[] UserList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetGroupListRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetGroupListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("GroupList")]
        /// <summary>TODO</summary>
        public string[] GroupList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetUserInformationRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string userName;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetUserInformationResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public User UserInformation;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetConnectedUserListRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetConnectedUserListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("ConnectionList")]
        /// <summary>TODO</summary>
        public Connection[] ConnectionList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class ChangePasswordRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string userName;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string oldPassword;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string newPassword;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class ChangePasswordResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class RenameUserRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string userName;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string newUserName;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class RenameUserResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string message;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class TerminateConnectionRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long ConnectionID;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class TerminateConnectionResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetProfilesRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetProfilesResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string data;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetGroupsRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetGroupsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string data;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetConfigurationRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetConfigurationResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string data;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetInterfacesRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetInterfacesResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string data;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetIPBlockListRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetIPBlockListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string data;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetAutoBlockListRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetAutoBlockListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string data;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetAppPathsRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetAppPathsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string AppDataPath;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string AppInstallPath;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetLicenseInfoRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetLicenseInfoResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public LicenseInfo LicenseInformation;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class VerifyLicenseRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string LicenseString;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class VerifyLicenseResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public LicenseInfo LicenseInformation;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetCurrentConnectionCountRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long InterfaceID;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetCurrentConnectionCountResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetAllCurrentConnectionCountRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetAllCurrentConnectionCountResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetInterfaceByIDRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long InterfaceID;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetInterfaceResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Interface Interface;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetInterfaceListRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetInterfaceListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("InterfaceList")]
        /// <summary>TODO</summary>
        public Interface[] InterfaceList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class InitializeInterfaceRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long InterfaceID;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class InitializeInterfaceResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class ShutdownInterfaceRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long InterfaceID;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class ShutdownInterfaceResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetStatisticsRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        /// <summary>TODO</summary>
        public bool includeFileStats;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>TODO</summary>
        public bool includeFileStatsSpecified;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetStatisticsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Statistics stats;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("list", Namespace = "http://cerberusllc.com/common", IsNullable = false)]
        /// <summary>TODO</summary>
        public FileHit[] fileStats;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetCurrentBandwidthRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetCurrentBandwidthResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public double up;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public double down;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetFeaturesRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetFeaturesResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public Features Features;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SaveProfilesRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ProfilesXML;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string GroupsXML;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SaveProfilesResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SaveConfigurationRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ConfigXML;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SaveConfigurationResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class CommitSettingsRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("InterfaceList")]
        /// <summary>TODO</summary>
        public Interface[] InterfaceList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ConfigXML;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class CommitSettingsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class SaveBlockListRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string IPBlockList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string AutoBlockListXML;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class SaveBlockListResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class ModifyInterfaceRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long InterfaceID;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public InterfaceOpts Opts;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class ModifyInterfaceResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class ShutdownConnectionsOnInterfaceRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public long InterfaceID;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class ShutdownConnectionsOnInterfaceResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetFileTransfersRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetFileTransfersResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("transferList")]
        /// <summary>TODO</summary>
        public FileTransfer[] transferList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GetLogMessagesRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GetLogMessagesResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("logList")]
        /// <summary>TODO</summary>
        public LogMessage[] logList;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class BlockAddressRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string ipaddress;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class BlockAddressResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class GenerateStatisticsRequest : AuthenticatedRequest
    {
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class GenerateStatisticsResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string filePath;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string errorMsg;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class BackupServerConfigurationRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string filePath;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string password;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class BackupServerConfigurationResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string filePath;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string errorMsg;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    /// <summary>TODO</summary>
    public partial class RestoreServerConfigurationRequest : AuthenticatedRequest
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string filePath;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string password;
    }

    /// <uwagi/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cerberusllc.com/service/cerberusftpservice")]
    public partial class RestoreServerConfigurationResponse
    {

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        [System.Xml.Serialization.XmlElementAttribute("importResult")]
        /// <summary>TODO</summary>
        public ImportFileResult[] importResult;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public string errorMsg;

        /// <uwagi/>
        /// <summary>Auto-generated member.</summary>
        public bool result;
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetBackupServersCompletedEventHandler(object sender, GetBackupServersCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetBackupServersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetBackupServersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetBackupServersResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetBackupServersResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SaveBackupServersCompletedEventHandler(object sender, SaveBackupServersCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SaveBackupServersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SaveBackupServersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SaveBackupServersResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SaveBackupServersResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SharePublicFileCompletedEventHandler(object sender, SharePublicFileCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SharePublicFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SharePublicFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SharePublicFileResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SharePublicFileResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void AddIpCompletedEventHandler(object sender, AddIpCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class AddIpCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal AddIpCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddIpResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((AddIpResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void DeleteIpCompletedEventHandler(object sender, DeleteIpCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class DeleteIpCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal DeleteIpCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeleteIpResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeleteIpResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void TestAndVerifyDatabaseCompletedEventHandler(object sender, TestAndVerifyDatabaseCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class TestAndVerifyDatabaseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal TestAndVerifyDatabaseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public TestAndVerifyDatabaseResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((TestAndVerifyDatabaseResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void CreateStatisticsDatabaseCompletedEventHandler(object sender, CreateStatisticsDatabaseCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class CreateStatisticsDatabaseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal CreateStatisticsDatabaseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public CreateStatisticsDatabaseResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((CreateStatisticsDatabaseResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void DropStatisticsDatabaseCompletedEventHandler(object sender, DropStatisticsDatabaseCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class DropStatisticsDatabaseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal DropStatisticsDatabaseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DropStatisticsDatabaseResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DropStatisticsDatabaseResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetMimeMappingsCompletedEventHandler(object sender, GetMimeMappingsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetMimeMappingsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetMimeMappingsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetMimeMappingsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetMimeMappingsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SaveMimeMappingsCompletedEventHandler(object sender, SaveMimeMappingsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SaveMimeMappingsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SaveMimeMappingsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SaveMimeMappingsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SaveMimeMappingsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void ServerSummaryStatusCompletedEventHandler(object sender, ServerSummaryStatusCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class ServerSummaryStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal ServerSummaryStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ServerSummaryStatusResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ServerSummaryStatusResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void ServerInformationCompletedEventHandler(object sender, ServerInformationCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class ServerInformationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal ServerInformationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ServerInformationResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ServerInformationResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void CurrentStatusCompletedEventHandler(object sender, CurrentStatusCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class CurrentStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal CurrentStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public CurrentStatusResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((CurrentStatusResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void StartServerCompletedEventHandler(object sender, StartServerCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class StartServerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal StartServerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public StartServerResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((StartServerResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void StopServerCompletedEventHandler(object sender, StopServerCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class StopServerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal StopServerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public StopServerResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((StopServerResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void ServerStartedCompletedEventHandler(object sender, ServerStartedCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class ServerStartedCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal ServerStartedCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ServerStartedResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ServerStartedResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void InitializeServerCompletedEventHandler(object sender, InitializeServerCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class InitializeServerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal InitializeServerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public InitializeServerResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((InitializeServerResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void ShutdownServerCompletedEventHandler(object sender, ShutdownServerCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class ShutdownServerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal ShutdownServerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ShutdownServerResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ShutdownServerResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetEventRulesCompletedEventHandler(object sender, GetEventRulesCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetEventRulesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetEventRulesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetEventRulesResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetEventRulesResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SetEventRulesCompletedEventHandler(object sender, SetEventRulesCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SetEventRulesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SetEventRulesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SetEventRulesResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SetEventRulesResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void DeleteRequestedAccountsCompletedEventHandler(object sender, DeleteRequestedAccountsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class DeleteRequestedAccountsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal DeleteRequestedAccountsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeleteRequestedAccountsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeleteRequestedAccountsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetRequestedAccountsCompletedEventHandler(object sender, GetRequestedAccountsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetRequestedAccountsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetRequestedAccountsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetRequestedAccountsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetRequestedAccountsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SetRequestedAccountsCompletedEventHandler(object sender, SetRequestedAccountsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SetRequestedAccountsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SetRequestedAccountsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SetRequestedAccountsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SetRequestedAccountsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetAuthenticationListCompletedEventHandler(object sender, GetAuthenticationListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetAuthenticationListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetAuthenticationListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetAuthenticationListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetAuthenticationListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SetAuthenticationListCompletedEventHandler(object sender, SetAuthenticationListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SetAuthenticationListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SetAuthenticationListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SetAuthenticationListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SetAuthenticationListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetHostnameCompletedEventHandler(object sender, GetHostnameCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetHostnameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetHostnameCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetHostnameResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetHostnameResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SetWANIPCompletedEventHandler(object sender, SetWANIPCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SetWANIPCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SetWANIPCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SetWANIPResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SetWANIPResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void AddUserCompletedEventHandler(object sender, AddUserCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class AddUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal AddUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddUserResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((AddUserResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void AddGroupCompletedEventHandler(object sender, AddGroupCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class AddGroupCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal AddGroupCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddGroupResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((AddGroupResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void DeleteUserCompletedEventHandler(object sender, DeleteUserCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class DeleteUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal DeleteUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeleteUserResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeleteUserResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void DeleteGroupCompletedEventHandler(object sender, DeleteGroupCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class DeleteGroupCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal DeleteGroupCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeleteGroupResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeleteGroupResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void AddRootCompletedEventHandler(object sender, AddRootCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class AddRootCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal AddRootCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddRootResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((AddRootResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void DeleteRootCompletedEventHandler(object sender, DeleteRootCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class DeleteRootCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal DeleteRootCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeleteRootResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeleteRootResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetUserListCompletedEventHandler(object sender, GetUserListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetUserListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetUserListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetUserListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetUserListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetGroupListCompletedEventHandler(object sender, GetGroupListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetGroupListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetGroupListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetGroupListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetGroupListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetUserInformationCompletedEventHandler(object sender, GetUserInformationCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetUserInformationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetUserInformationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetUserInformationResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetUserInformationResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetConnectedUserListCompletedEventHandler(object sender, GetConnectedUserListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetConnectedUserListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetConnectedUserListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetConnectedUserListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetConnectedUserListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void ChangePasswordCompletedEventHandler(object sender, ChangePasswordCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class ChangePasswordCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal ChangePasswordCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ChangePasswordResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ChangePasswordResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void RenameUserCompletedEventHandler(object sender, RenameUserCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class RenameUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal RenameUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public RenameUserResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((RenameUserResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void TerminateConnectionCompletedEventHandler(object sender, TerminateConnectionCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class TerminateConnectionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal TerminateConnectionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public TerminateConnectionResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((TerminateConnectionResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetProfilesCompletedEventHandler(object sender, GetProfilesCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetProfilesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetProfilesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetProfilesResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetProfilesResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetGroupsCompletedEventHandler(object sender, GetGroupsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetGroupsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetGroupsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetGroupsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetGroupsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetConfigurationCompletedEventHandler(object sender, GetConfigurationCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetConfigurationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetConfigurationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetConfigurationResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetConfigurationResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetInterfacesCompletedEventHandler(object sender, GetInterfacesCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetInterfacesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetInterfacesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetInterfacesResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetInterfacesResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetIPBlockListCompletedEventHandler(object sender, GetIPBlockListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetIPBlockListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetIPBlockListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetIPBlockListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetIPBlockListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetAutoBlockListCompletedEventHandler(object sender, GetAutoBlockListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetAutoBlockListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetAutoBlockListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetAutoBlockListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetAutoBlockListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetAppPathsCompletedEventHandler(object sender, GetAppPathsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetAppPathsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetAppPathsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetAppPathsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetAppPathsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetLicenseInfoCompletedEventHandler(object sender, GetLicenseInfoCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetLicenseInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetLicenseInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetLicenseInfoResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetLicenseInfoResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void VerifyLicenseCompletedEventHandler(object sender, VerifyLicenseCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class VerifyLicenseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal VerifyLicenseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public VerifyLicenseResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((VerifyLicenseResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetCurrentConnectionCountCompletedEventHandler(object sender, GetCurrentConnectionCountCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetCurrentConnectionCountCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetCurrentConnectionCountCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetCurrentConnectionCountResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetCurrentConnectionCountResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetAllCurrentConnectionCountCompletedEventHandler(object sender, GetAllCurrentConnectionCountCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetAllCurrentConnectionCountCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetAllCurrentConnectionCountCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetAllCurrentConnectionCountResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetAllCurrentConnectionCountResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetInterfaceByIDCompletedEventHandler(object sender, GetInterfaceByIDCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetInterfaceByIDCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetInterfaceByIDCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetInterfaceResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetInterfaceResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetInterfaceListCompletedEventHandler(object sender, GetInterfaceListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetInterfaceListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetInterfaceListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetInterfaceListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetInterfaceListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void InitializeInterfaceCompletedEventHandler(object sender, InitializeInterfaceCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class InitializeInterfaceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal InitializeInterfaceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public InitializeInterfaceResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((InitializeInterfaceResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void ShutdownInterfaceCompletedEventHandler(object sender, ShutdownInterfaceCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class ShutdownInterfaceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal ShutdownInterfaceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ShutdownInterfaceResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ShutdownInterfaceResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetStatisticsCompletedEventHandler(object sender, GetStatisticsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetStatisticsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetStatisticsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetStatisticsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetStatisticsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetCurrentBandwidthCompletedEventHandler(object sender, GetCurrentBandwidthCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetCurrentBandwidthCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetCurrentBandwidthCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetCurrentBandwidthResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetCurrentBandwidthResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetFeaturesCompletedEventHandler(object sender, GetFeaturesCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetFeaturesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetFeaturesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetFeaturesResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetFeaturesResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SaveProfilesCompletedEventHandler(object sender, SaveProfilesCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SaveProfilesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SaveProfilesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SaveProfilesResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SaveProfilesResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SaveConfigurationCompletedEventHandler(object sender, SaveConfigurationCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SaveConfigurationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SaveConfigurationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SaveConfigurationResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SaveConfigurationResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void CommitSettingsCompletedEventHandler(object sender, CommitSettingsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class CommitSettingsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal CommitSettingsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public CommitSettingsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((CommitSettingsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void SaveBlockListCompletedEventHandler(object sender, SaveBlockListCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class SaveBlockListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal SaveBlockListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SaveBlockListResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((SaveBlockListResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void ModifyInterfaceCompletedEventHandler(object sender, ModifyInterfaceCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class ModifyInterfaceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal ModifyInterfaceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ModifyInterfaceResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ModifyInterfaceResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void ShutdownConnectionsOnInterfaceCompletedEventHandler(object sender, ShutdownConnectionsOnInterfaceCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class ShutdownConnectionsOnInterfaceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal ShutdownConnectionsOnInterfaceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ShutdownConnectionsOnInterfaceResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ShutdownConnectionsOnInterfaceResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetFileTransfersCompletedEventHandler(object sender, GetFileTransfersCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetFileTransfersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetFileTransfersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetFileTransfersResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetFileTransfersResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GetLogMessagesCompletedEventHandler(object sender, GetLogMessagesCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GetLogMessagesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GetLogMessagesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetLogMessagesResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GetLogMessagesResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void BlockAddressCompletedEventHandler(object sender, BlockAddressCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class BlockAddressCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal BlockAddressCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public BlockAddressResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((BlockAddressResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void GenerateStatisticsCompletedEventHandler(object sender, GenerateStatisticsCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class GenerateStatisticsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal GenerateStatisticsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GenerateStatisticsResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((GenerateStatisticsResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void BackupServerConfigurationCompletedEventHandler(object sender, BackupServerConfigurationCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class BackupServerConfigurationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal BackupServerConfigurationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public BackupServerConfigurationResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((BackupServerConfigurationResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    /// <summary>TODO</summary>
    public delegate void RestoreServerConfigurationCompletedEventHandler(object sender, RestoreServerConfigurationCompletedEventArgs e);

    /// <remarks/>
    /// <summary>Auto-generated member.</summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    /// <summary>TODO</summary>
    public partial class RestoreServerConfigurationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>Auto-generated member.</summary>

        private object[] results;
        /// <summary>Auto-generated member.</summary>

        internal RestoreServerConfigurationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public RestoreServerConfigurationResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((RestoreServerConfigurationResponse)(this.results[0]));
            }
        }
    }
}

