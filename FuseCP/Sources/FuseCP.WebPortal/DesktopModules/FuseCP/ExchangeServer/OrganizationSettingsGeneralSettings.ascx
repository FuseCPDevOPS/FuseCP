<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganizationSettingsGeneralSettings.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.OrganizationSettingsGeneralSettings" %>



<%@ Register Src="UserControls/OrganizationSettingsTabs.ascx" TagName="CollectionTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/ItemButtonPanel.ascx" TagName="ItemButtonPanel" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script type="text/javascript">
	if (!window.jQuery) {
		document.write('<script src="/JavaScript/jquery-2.1.0.min.js"><\/script>');
	}
</script>

<fcp:EnableAsyncTasksSupport ID="asyncTasks" runat="server" />
				<div class="card-header">
                    <h3 class="card-title">
                    <asp:Image ID="Image1" SkinID="Organization48" runat="server" />
                    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Settings"></asp:Localize>

                    <asp:Literal ID="litOrganizationName" runat="server" Text="Organization" />
                        </h3>
                </div>
                <div class="card-body form-horizontal fcp-modern-page">
                    <fcp:CollectionTabs ID="tabs" runat="server" SelectedTab="organization_settings_general_settings" />
                    <div class="card tab-content">
                    <fcp:SimpleMessageBox ID="messageBox" runat="server" />
                            <fcp:CollapsiblePanel ID="colGeneralSettings" runat="server" TargetControlID="panelGeneralSettings" meta:ResourceKey="colGeneralSettings" Text="General settings"></fcp:CollapsiblePanel>

                            <asp:Panel runat="server" ID="panelGeneralSettings">
                                <table class="table table-borderless align-middle mb-0" id="GenerralSettignsTable" runat="server">
                                    <tr>
                                        <td class="Normal" >
                                            <asp:Label ID="lblOrganizationLogoUrl" runat="server"
                                                meta:resourcekey="lblOrganizationLogoUrl" Text="Minimum length:"></asp:Label></td>
                                        <td class="Normal">
                                            <asp:TextBox ID="txtOrganizationLogoUrl" runat="server" CssClass="form-control" Width="400px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="valRequireMinLength" runat="server" ControlToValidate="txtOrganizationLogoUrl" meta:resourcekey="valRequireOrganizationLogoUrl"
                                                ErrorMessage="*" ValidationGroup="SettingsEditor" Display="Dynamic"></asp:RequiredFieldValidator>
                                          </td>
                                    </tr>
    
                                </table>
                            </asp:Panel>


                </div>
                    </div>
                    <div class="card-footer text-end">
                        <fcp:ItemButtonPanel ID="buttonPanel" runat="server" ValidationGroup="SettingsEditor"
                            OnSaveClick="btnSave_Click" OnSaveExitClick="btnSaveExit_Click" />
                    </div>
