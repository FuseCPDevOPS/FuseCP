<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseStorageFolderGeneralSettings.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.EnterpriseStorageFolderGeneralSettings" %>

<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EnterpriseStoragePermissions.ascx" TagName="ESPermissions" TagPrefix="fcp"%>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" Namespace="FuseCP.Portal.ExchangeServer.UserControls" Assembly="FuseCP.Portal.Modules" %>
<%@ Register Src="UserControls/EnterpriseStorageEditFolderTabs.ascx" TagName="CollectionTabs" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeList48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Folder"></asp:Localize>

					<asp:Literal ID="litFolderName" runat="server" Text="Folder 32" />
                        </h3>
                </div>
                <div class="card-body form-horizontal fcp-modern-page">
                            <fcp:CollectionTabs id="tabs" runat="server" SelectedTab="enterprisestorage_folder_settings" />
                        <div class="card tab-content">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
                    <fcp:CollapsiblePanel id="colFolderGeneralSettings" runat="server"
                        TargetControlID="panelFolderGeneralSettings" meta:resourcekey="colFolderGeneralSettings" Text="">
                    </fcp:CollapsiblePanel>		
                    
                     <asp:Panel runat="server" ID="panelFolderGeneralSettings">    
					    <table>
						    <tr>
							    <td class="FormLabel150"><asp:Localize ID="locFolderName" runat="server" meta:resourcekey="locFolderName" Text="Folder Name:"></asp:Localize></td>
							    <td>
								    <asp:TextBox ID="txtFolderName" runat="server" CssClass="form-control" ></asp:TextBox>
								    <asp:RequiredFieldValidator ID="valRequireFolderName" runat="server" meta:resourcekey="valRequireFolderName" ControlToValidate="txtFolderName"
									    ErrorMessage="Enter Folder Name" ValidationGroup="EditFolder" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
							    </td>
						    </tr>
                            <tr>
							    <td class="FormLabel150"><asp:Localize ID="locFolderSize" runat="server" meta:resourcekey="locFolderSize" Text="Folder Limit Size (Gb):"></asp:Localize></td>
							    <td>
								    <asp:TextBox ID="txtFolderSize" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="valRequireFolderSize" runat="server" meta:resourcekey="valRequireFolderSize" ControlToValidate="txtFolderSize"
									    ErrorMessage="Enter Folder Size" ValidationGroup="EditFolder" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rangeFolderSize" runat="server" ControlToValidate="txtFolderSize" MaximumValue="99999999" MinimumValue="0" Type="Double"
                                        ValidationGroup="EditFolder" Display="Dynamic" Text="*" SetFocusOnError="True"
                                        ErrorMessage="The quota you've entered exceeds the available quota for organization" />
                                    <%--<asp:RangeValidator ID="rangeFolderSize" runat="server" ControlToValidate="txtFolderSize" MaximumValue="99999999" MinimumValue="0.01" Type="Double"
                                        ValidationGroup="EditFolder" Display="Dynamic" Text="*" SetFocusOnError="True"
                                        ErrorMessage="The quota you've entered exceeds the available quota for organization" />--%>
							    </td>
						    </tr>
                            <tr>
                                <td class="FormLabel150"><asp:Localize ID="locQuotaType" runat="server" meta:resourcekey="locQuotaType" Text="Quota Type:"></asp:Localize></td>
                                <td class="FormRBtnL">
                                    <asp:RadioButton ID="rbtnQuotaSoft" runat="server" meta:resourcekey="rbtnQuotaSoft" Text="Soft" GroupName="QuotaType" Checked="true" />
                                    <asp:RadioButton ID="rbtnQuotaHard" runat="server" meta:resourcekey="rbtnQuotaHard" Text="Hard" GroupName="QuotaType" />
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="FormLabel150"><asp:Localize ID="locFolderUrl" runat="server" meta:resourcekey="locFolderUrl" Text="Folder Url:"></asp:Localize></td>
                                <td><asp:Label runat="server" ID="lblFolderUrl" /></td>
                            </tr>
                            <tr id="uncPathRow" runat="server" Visible="false">
                                <td class="FormLabel150"><asp:Localize ID="locUncPath" runat="server" meta:resourcekey="locUncPath" Text="UNC Path:"></asp:Localize></td>
                                <td><asp:Label runat="server" ID="lblUncPath" /></td>
                            </tr>
                            <tr id="abeRow" runat="server" Visible="false">
                                <td class="FormLabel150"><asp:Localize ID="locABE" runat="server" meta:resourcekey="locABE" Text="Enable Access-Based Enumeration:"></asp:Localize></td>
                                <td class="FormRBtnL">
                                    <asp:CheckBox runat="server" ID="chkAbe" meta:resourcekey="chkAbe"/>
                                </td>
                            </tr>
                            <tr id="edaRow" runat="server" Visible="false">
                                <td class="FormLabel150"><asp:Localize ID="locEda" runat="server" meta:resourcekey="locEda" Text="Encrypt data access:"></asp:Localize></td>
                                <td class="FormRBtnL">
                                    <asp:CheckBox runat="server" ID="chkEda" meta:resourcekey="chkEda"/>
                                </td>
                            </tr>

                            <tr><td>&nbsp;</td></tr>
					    </table>
					</asp:Panel>


				</div>
                        </div>

				    <div class="card-footer text-end">
                        <asp:LinkButton id="btnMigrate" CssClass="btn btn-warning" runat="server" OnClick="btnMigrate_Click"> <i class="bi bi-copy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnMigrateText"/> </asp:LinkButton>&nbsp;
					    <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" ValidationGroup="EditFolder"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>&nbsp;
					    <asp:ValidationSummary ID="valSummary" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="EditFolder" />
				    </div>
