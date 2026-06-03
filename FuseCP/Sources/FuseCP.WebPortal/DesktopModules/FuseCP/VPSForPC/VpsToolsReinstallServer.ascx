<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsToolsReinstallServer.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsToolsReinstallServer" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="Server48" runat="server" />
				    <fcp:FormTitle ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Re-install Server" />
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
			        <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_tools" />	
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
                    <asp:ValidationSummary ID="validatorsSummary" runat="server" 
                        ValidationGroup="Reinstall" ShowMessageBox="True" ShowSummary="False" />
                    
		            <p class="SubTitle">
		                <asp:Localize ID="locSubTitle" runat="server"
		                    meta:resourcekey="locSubTitle" Text="Re-install VPS Server" />
		            </p>
                    <p>
                        <asp:Localize ID="locDescription" runat="server"
		                    meta:resourcekey="locDescription" Text="This wizard will re-create VPS with the same configuration settings from scratch and then apply current OS template." />
                    </p>
                    <p>
                        <asp:CheckBox ID="chkConfirmReinstall" runat="server" CssClass="NormalBold"
				                    meta:resourcekey="chkConfirmReinstall" Text="Yes, I confirm re-installation of this VPS" />
                    </p>
				    
				    <table class="table table-borderless align-middle mb-0">
				        <tr>
				            <td class="align-top">
				                <asp:Localize ID="locPassword" runat="server"
		                            meta:resourcekey="locPassword" Text="New administrator password:" />
				            </td>
				            <td>
				                 <fcp:PasswordControl id="password" runat="server"
			                        ValidationGroup="Reinstall"></fcp:PasswordControl>
				            </td>
				        </tr>
				        <tr>
				            <td colspan="2">
				                <asp:CheckBox ID="chkPreserveExistingFiles" runat="server" CssClass="NormalBold"
				                    meta:resourcekey="chkPreserveExistingFiles" Text="Save existing VPS hard drive files" />
				                <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				                <asp:Localize ID="locPreserveHelp" runat="server"
		                            meta:resourcekey="locPreserveHelp" Text="All files from existing hard drive will be copied to &quot;old&quot; disk folder on hard drive of new VPS." />
				            </td>
				        </tr>
				    </table>
				    <br />
                    <table class="table table-borderless align-middle mb-0" id="AdminOptionsPanel" runat="server">
				        <tr>
				            <td>
				                <asp:CheckBox ID="chkSaveVhd" runat="server"
				                    meta:resourcekey="chkSaveVhd" Text="Do not delete VPS virtual hard drive file" />
				            </td>
				        </tr>
				        <tr>
				            <td>
				                <asp:CheckBox ID="chkExport" runat="server"
				                    meta:resourcekey="chkExport" Text="Export VPS before re-installation to the following folder:" />
				            </td>
				        </tr>
				        <tr>
				            <td class="fcp-ps-20">
				                <asp:TextBox ID="txtExportPath" runat="server" Width="300px" CssClass="form-control"></asp:TextBox>
				                
				                <asp:RequiredFieldValidator ID="ExportPathValidator" runat="server" Text="*" Display="Dynamic"
                                        ControlToValidate="txtExportPath" meta:resourcekey="ExportPathValidator" SetFocusOnError="true"
                                        ValidationGroup="Reinstall">*</asp:RequiredFieldValidator>
				            </td>
				        </tr>
				    </table>
				    
                    <p>
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnReinstall" CssClass="btn btn-success" runat="server" onclick="btnUpdate_Click" ValidationGroup="Reinstall"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnReinstall"/> </asp:LinkButton>
                    </p>
			    </div>
		    </div>
            </div>
            </div>
		    <div class="Right">
			    <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
		    </div>
