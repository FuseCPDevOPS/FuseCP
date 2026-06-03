<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangePublicFolderGeneralSettings.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangePublicFolderGeneralSettings" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/AccountsList.ascx" TagName="AccountsList" TagPrefix="fcp" %>
<%@ Register Src="UserControls/MailboxSelector.ascx" TagName="MailboxSelector" TagPrefix="fcp" %>
<%@ Register Src="UserControls/PublicFolderTabs.ascx" TagName="PublicFolderTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register src="UserControls/AccountsListWithPermissions.ascx" tagname="AccountsListWithPermissions" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangePublicFolder48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Distribution List"></asp:Localize>
					-
					<asp:Literal ID="litDisplayName" runat="server" Text="John Smith" />
                        </h3>
                </div>
				<div class="card-body form-horizontal fcp-modern-page">
				<fcp:PublicFolderTabs id="tabs" runat="server" SelectedTab="public_folder_settings" />
                    <div class="card tab-content">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
					<table>
						<tr>
							<td class="FormLabel150"><asp:Localize ID="locName" runat="server" meta:resourcekey="locName" Text="Folder Name: *"></asp:Localize></td>
							<td>
								<asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
								<asp:RequiredFieldValidator ID="valDisplayName" runat="server" meta:resourcekey="valRequireName" ControlToValidate="txtName"
									ErrorMessage="Enter Folder Name" ValidationGroup="EditPublicFolder" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
							</td>
						</tr>

						<tr>
						    <td></td>
							<td>
							    <br />
							    <asp:LinkButton id="btnMailDisable" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnMailDisable_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnMailDisable"/> </asp:LinkButton>&nbsp;
                                <asp:LinkButton id="btnMailEnable" CssClass="btn btn-success" runat="server" OnClick="btnMailEnable_Click" CausesValidation="false"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnMailEnable"/> </asp:LinkButton>

							</td>
						</tr>
					    
					    <tr><td>&nbsp;</td></tr>
                        					    <tr><td>&nbsp;</td></tr>
						<tr>
							<td colspan="2"><asp:Localize ID="locAllAccounts" runat="server" meta:resourcekey="locAllAccounts" Text="Accounts:"></asp:Localize></td>
						</tr>
						<tr>
						    <td colspan="2">                                
                            	<fcp:AccountsListWithPermissions ID="allAccounts" runat="server" MailboxesEnabled="true" EnableMailboxOnly="true" DistributionListsEnabled="true" SecurityGroupsEnabled="false"/>
                                
                            </td>
						</tr>
						<tr><td>&nbsp;</td></tr>
						
						<tr>
						    <td class="text-nowrap">
						        <br />
						        <asp:CheckBox ID="chkHideAddressBook" runat="server" meta:resourcekey="chkHideAddressBook" Text="Hide from Address Book" />
						    </td>
						</tr>
					</table>
					

				</div>
                    </div>
				    <div class="card-footer text-end">
					    <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" ValidationGroup="EditPublicFolder"> <i class="bi bi-folder me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>
					    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="EditPublicFolder" />
				    </div>
