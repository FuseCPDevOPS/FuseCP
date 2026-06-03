<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsDetailsConfiguration.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsDetailsConfiguration" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CheckBoxOption.ascx" TagName="CheckBoxOption" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="ServerConfig48" runat="server" />
				    <fcp:FormTitle ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Configuration" />
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
			        <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_config" />	
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
                   
                    <fcp:CollapsiblePanel id="secSoftware" runat="server"
                        TargetControlID="SoftwarePanel" meta:resourcekey="secSoftware" Text="Software">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="SoftwarePanel" runat="server" Height="0" CssClass="fcp-p-5" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td><asp:Localize ID="locOperatingSystem" runat="server"
                                    meta:resourcekey="locOperatingSystem" Text="Operating system:"></asp:Localize></td>
                                <td>
                                    <asp:Literal ID="litOperatingSystem" runat="server" Text="[OS]"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td><asp:Localize ID="locAdministratorPassword" runat="server"
                                    meta:resourcekey="locAdministratorPassword" Text="Administrator password:"></asp:Localize></td>
                                <td>
                                    ********
                                    <asp:LinkButton ID="btnChangePasswordPopup" runat="server" CausesValidation="false"
                                        Text="Change" meta:resourcekey="btnChangePasswordPopup"></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    

                    <fcp:CollapsiblePanel id="secResources" runat="server"
                        TargetControlID="ResourcesPanel" meta:resourcekey="secResources" Text="Resources">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="ResourcesPanel" runat="server" Height="0" CssClass="fcp-p-10" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td class="Medium"><asp:Localize ID="lblCpu" runat="server"
                                        meta:resourcekey="lblCpu" Text="CPU:" /></td>
                                <td class="MediumBold">
                                    <asp:Literal ID="litCpu" runat="server" Text="[cpu]"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td class="Medium"><asp:Localize ID="lblRam" runat="server"
                                        meta:resourcekey="lblRam" Text="RAM:" /></td>
                                <td class="MediumBold">
                                    <asp:Literal ID="litRam" runat="server" Text="[ram]"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td class="Medium"><asp:Localize ID="lblHdd" runat="server"
                                        meta:resourcekey="lblHdd" Text="HDD:" /></td>
                                <td class="MediumBold">
                                    <asp:Literal ID="litHdd" runat="server" Text="[hdd]"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <fcp:CollapsiblePanel id="secActions" runat="server"
                        TargetControlID="ActionsPanel" meta:resourcekey="secActions" Text="Allowed actions">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="ActionsPanel" runat="server" Height="0" CssClass="fcp-p-5" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0" >
                            <tr>
                                <td >
                                    <fcp:CheckBoxOption id="optionStartShutdown" runat="server"
                                        Text="Start, Turn off and Shutdown" meta:resourcekey="optionStartShutdown" Value="True" />
                                </td>
                                <td>
                                    <fcp:CheckBoxOption id="optionReset" runat="server"
                                        Text="Reset" meta:resourcekey="optionReset" Value="True" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <fcp:CheckBoxOption id="optionPauseResume" runat="server"
                                        Text="Pause, Resume" meta:resourcekey="optionPauseResume" Value="False" />
                                </td>
                                <td>
                                    <fcp:CheckBoxOption id="optionReinstall" runat="server"
                                        Text="Re-install" meta:resourcekey="optionReinstall" Value="True" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <fcp:CheckBoxOption id="optionReboot" runat="server"
                                        Text="Reboot" meta:resourcekey="optionReboot" Value="True" />
                                </td>
                                <td>
                                    
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <fcp:CollapsiblePanel id="secNetwork" runat="server"
                        TargetControlID="NetworkPanel" meta:resourcekey="secNetwork" Text="Network">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="NetworkPanel" runat="server" Height="0" CssClass="fcp-p-5" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td><fcp:CheckBoxOption id="optionExternalNetwork" runat="server"
                                        Text="External network enabled" meta:resourcekey="optionExternalNetwork" Value="True" />
                                </td>
                            </tr>
                            <tr>
                                <td><fcp:CheckBoxOption id="optionPrivateNetwork" runat="server"
                                        Text="Private network enabled" meta:resourcekey="optionPrivateNetwork" Value="True" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <p class="fcp-p-5">
                        <asp:LinkButton id="btnEdit" CssClass="btn btn-success" runat="server" OnClick="btnEdit_Click" CausesValidation="false" Enabled="false"> <i class="bi bi-pencil me-1"></i><asp:Localize runat="server" meta:resourcekey="btnEditText"/> </asp:LinkButton>
                    </p>

			    </div>
		    </div>
	    </div>
</div>
		    <div class="Right">
			    <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
		    </div>


<asp:Panel ID="ChangePasswordPanel" runat="server" style="display:none">
	 <div class="widget">
             <div class="widget-header clearfix">
                           <h3><i class="bi bi-i-cursor"></i>  <asp:Localize ID="locChangePassword" runat="server" Text="Change Administrator Password" meta:resourcekey="locChangePassword"></asp:Localize></h3>
			</div>
                    <div class="widget-content Popup">
			<table class="table table-borderless align-middle mb-0 fcp-ms-20" >
			    <tr>
			        <td>
			            <asp:Localize ID="locNewPassword" runat="server" Text="Enter new password:"
				            meta:resourcekey="locNewPassword"></asp:Localize>
			        </td>
			    </tr>
			    <tr>
			        <td>
			            <fcp:PasswordControl id="password" runat="server"
			                ValidationGroup="ChangePassword"></fcp:PasswordControl>
			        </td>
			    </tr>
			</table>                       
			</div>
					<div class="popup-buttons text-end">
		    <asp:LinkButton id="btnCancelChangePassword" CssClass="btn btn-warning" runat="server" CausesValidation="false"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelChangePasswordText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnChangePassword" CssClass="btn btn-primary" runat="server" OnClick="btnChangePassword_Click" ValidationGroup="ChangePassword"> <i class="bi bi-key me-1"></i><asp:Localize runat="server" meta:resourcekey="btnChangePasswordText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="ChangePasswordModal" runat="server" BehaviorID="PasswordModal"
	TargetControlID="btnChangePasswordPopup" PopupControlID="ChangePasswordPanel"
	BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelChangePassword" />

