<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VdcFastCreateServer.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VdcFastCreateServer" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="AddServer48" runat="server" />
                    <asp:Localize ID="Localize1" runat="server" meta:resourcekey="locTitle" Text="Create New VM"></asp:Localize>
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                    <div class="card tab-content">
                <div class="card-body form-horizontal">
<%--			        <fcp:ServerTabs id="tabs" runat="server" />	
--%>                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
                    <asp:ValidationSummary ID="validatorsSummary" runat="server" 
                        ValidationGroup="Tools" ShowMessageBox="True" ShowSummary="False" />
                    
		            <p class="SubTitle">
		                <asp:Localize ID="locSubTitle" runat="server"
		                    meta:resourcekey="locSubTitle" Text="Fast Create VM" />
		            </p>
                    <p>
                        <asp:Localize ID="locDescription" runat="server"
		                    meta:resourcekey="locDescription" Text="This wizard will Create new VM." />
                    </p>
                    <p class="warningText">
                        <asp:Localize ID="Localize2" runat="server"
		                    meta:resourcekey="locWarningCloning" Text="VM will be stopped before cloning, are you Ok with this?" />
                    </p>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                        ValidationGroup="VpsWizard" ShowMessageBox="True" ShowSummary="False" />

                    <table class="table table-borderless align-middle mb-0">
				        <tr>
                            <td class="FormLabel150"><asp:Localize ID="locOperatingSystem" runat="server"
                                meta:resourcekey="locOperatingSystem" Text="Virtual Machine:"></asp:Localize></td>
                            <td>
                                <asp:DropDownList ID="listOperatingSystems" runat="server"
                                    DataValueField="Path" DataTextField="Name">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="OperatingSystemValidator" runat="server" Text="*" Display="Dynamic"
                                    ControlToValidate="listOperatingSystems" meta:resourcekey="OperatingSystemValidator" SetFocusOnError="true"
                                    ValidationGroup="VpsWizard">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr><td>&nbsp;</td></tr>
                        <tr>
                            <td class="FormLabel150 align-top"><asp:Localize ID="Localize3" runat="server"
                                meta:resourcekey="VMName" Text="VM Name:"></asp:Localize></td>
                            <td>
                                <asp:TextBox id="txtVmName" runat="server" ValidationGroup="VpsWizard">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="vmNameValidator" runat="server" Text="*" Display="Dynamic"
                                    ControlToValidate="txtVmName" meta:resourcekey="vmNameValidator" SetFocusOnError="true"
                                    ValidationGroup="VpsWizard">*</asp:RequiredFieldValidator>

                            </td>
				        </tr>
				    </table>
                    <p>
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click" meta:resourcekey="btnCancel"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnCreate" CssClass="btn btn-success" runat="server" OnClick="btnCreate_Click" ValidationGroup="Tools"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCreateText"/> </asp:LinkButton>
                    </p>
			    </div>
		    </div>
                    </div>
            </div>
		    <div class="Right">
			    <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
		    </div>
