<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseStorageFolderSettingsOwaEditing.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.EnterpriseStorageFolderSettingsOwaEditing" %>


<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EnterpriseStorageOwaUsersList.ascx" TagName="OwaUsers" TagPrefix="fcp"%>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EnterpriseStorageEditFolderTabs.ascx" TagName="CollectionTabs" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeList48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Folder"></asp:Localize>

					<asp:Literal ID="litFolderName" runat="server" Text="Folder" />
                        </h3>
                </div>
				<div class="card-body form-horizontal fcp-modern-page">
				            <fcp:CollectionTabs id="tabs" runat="server" SelectedTab="enterprisestorage_folder_settings_owa_editing" />
                        <div class="card tab-content">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
                    <fcp:CollapsiblePanel id="colOwaEditing" runat="server"
                        TargetControlID="panelFolderPermissions" meta:resourcekey="colOwaEditing" Text="">
                    </fcp:CollapsiblePanel>		
                    
                     <asp:Panel runat="server" ID="panelFolderPermissions">                                                
					    <table>
						    <tr>
							    <td colspan="2">
                                    <fieldset id="OwaUsersPanel" runat="server">
                                        <legend><asp:Localize ID="locOwaEditingSection" runat="server" meta:resourcekey="locOwaEditingSection" Text="Users And Groups"></asp:Localize></legend>
                                        <fcp:OwaUsers id="owaUsers" runat="server" />
                                    </fieldset>
						    </tr>
					        <tr><td>&nbsp;</td></tr>

					    </table>
                    </asp:Panel>
				</div>
                </div>
				    <div class="card-footer text-end">
					    <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" ValidationGroup="EditFolder"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>
					    <asp:ValidationSummary ID="valSummary" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="EditFolder" />
				    </div>
