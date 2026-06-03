<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsMoveServer.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsMoveServer" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CheckBoxOption.ascx" TagName="CheckBoxOption" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="Server48" runat="server" />
				    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Move VPS"></asp:Localize>
			    </div>
			    <div class="card-body form-horizontal">
    			    	<fcp:Menu id="menu" runat="server" SelectedItem="" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
                    <asp:ValidationSummary ID="validatorsSummary" runat="server" 
                        ValidationGroup="MoveWizard" ShowMessageBox="True" ShowSummary="False" />
                        
                    
                    <table class="table table-borderless align-middle mb-0">
                        <tr>
                            <td class="FormLabel150">
                                <asp:Localize ID="locSourceService" runat="server" meta:resourcekey="locSourceService" Text="Source Service:"></asp:Localize>
                            </td>
                            <td>
                                <asp:Literal ID="SourceHyperVService" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td class="FormLabel150">
                                <asp:Localize ID="locDestinationService" runat="server" meta:resourcekey="locDestinationService" Text="Destination Service:"></asp:Localize>
                            </td>
                            <td>
                                <asp:DropDownList ID="HyperVServices" runat="server" CssClass="form-control"
                                    DataValueField="ServiceId" DataTextField="FullServiceName"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredHyperVService" runat="server"
                                    ControlToValidate="HyperVServices" ValidationGroup="MoveWizard" meta:resourcekey="RequiredHyperVService"
                                    Display="Dynamic" SetFocusOnError="true" Text="*">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                    
                    <p>
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click" meta:resourcekey="btnCancel"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnMove" CssClass="btn btn-success" runat="server" OnClick="btnMove_Click" ValidationGroup="MoveWizard" meta:resourcekey="btnMove"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnMoveText"/> </asp:LinkButton>
                    </p>
                        
			    </div>
		    </div>
            </div>
            </div>
		    <div class="Right">
			    <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
		    </div>
