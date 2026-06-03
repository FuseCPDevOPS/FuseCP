<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSSetupLetter.ascx.cs" Inherits="FuseCP.Portal.RDS.RDSSeupLetter" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="UserControls/RDSCollectionTabs.ascx" TagName="CollectionTabs" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-header">
					<asp:Image ID="imgEditRDSCollection" SkinID="EnterpriseRDSCollections48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Setup Instructions"></asp:Localize>                    
                </div>
				<div class="card-body form-horizontal">     
                    <fcp:CollectionTabs id="tabs" runat="server" SelectedTab="rds_collection_edit_users" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">  
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    <fcp:CollapsiblePanel id="secEmail" runat="server" IsCollapsed="true"
                        TargetControlID="EmailPanel" meta:resourcekey="secEmail" Text="Send via E-Mail">
                    </fcp:CollapsiblePanel>
	                <asp:Panel ID="EmailPanel" runat="server" Height="0" style="overflow:hidden">
                        <table class="table table-borderless align-middle mb-0" id="tblEmail" runat="server">
                            <tr>
                                <td class="SubHead text-nowrap">
                                    <asp:Label ID="lblTo" runat="server" meta:resourcekey="lblTo" Text="To:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" Width="300px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="valRequireEmail" runat="server" ControlToValidate="txtTo" Display="Dynamic"
                                        ErrorMessage="Enter e-mail" ValidationGroup="SendEmail" meta:resourcekey="valRequireEmail"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblCC" runat="server" meta:resourcekey="lblCC" Text="CC:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:TextBox ID="txtCC" runat="server" CssClass="form-control" Width="300px"></asp:TextBox></td>
                            </tr>                            
                        </table>
                        <div class="card-footer text-end">
                        <asp:LinkButton id="btnExit" CssClass="btn btn-danger" runat="server" OnClick="btnExit_Click" OnClientClick="ShowProgressDialog('Loading ...');"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnExitText"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnSend" CssClass="btn btn-success" runat="server" OnClick="btnSend_Click" ValidationGroup="SendEmail"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSendtext"/> </asp:LinkButton>
			        </div>
                    </asp:Panel>
					
                    <div class="PreviewArea">
                        <asp:Literal ID="litContent" runat="server"></asp:Literal>
                    </div>										
				</div>
			</div>
		</div>
