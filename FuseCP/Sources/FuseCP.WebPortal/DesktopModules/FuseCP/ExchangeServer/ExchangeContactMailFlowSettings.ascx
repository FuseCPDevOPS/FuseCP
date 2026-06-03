<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeContactMailFlowSettings.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeContactMailFlowSettings" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ContactTabs.ascx" TagName="ContactTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="UserControls/AcceptedSenders.ascx" TagName="AcceptedSenders" TagPrefix="fcp" %>
<%@ Register Src="UserControls/RejectedSenders.ascx" TagName="RejectedSenders" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeContact48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Contact"></asp:Localize>
					-
					<asp:Literal ID="litDisplayName" runat="server" Text="John Smith" />
                        </h3>
                </div>
				<div class="card-body form-horizontal fcp-modern-page">
				<fcp:ContactTabs id="tabs" runat="server" SelectedTab="contact_mailflow" />
                    <div class="card tab-content">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />		
					
					<fcp:CollapsiblePanel id="secAcceptMessagesFrom" runat="server"
                        TargetControlID="AcceptMessagesFrom" meta:resourcekey="secAcceptMessagesFrom" Text="Accept Messages From">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="AcceptMessagesFrom" runat="server" Height="0" style="overflow:hidden;">
					    <fcp:AcceptedSenders id="acceptAccounts" runat="server" />
					    <asp:CheckBox ID="chkSendersAuthenticated" runat="server" meta:resourcekey="chkSendersAuthenticated" Text="Require that all senders are authenticated" />
					</asp:Panel>
					
					
					<fcp:CollapsiblePanel id="secRejectMessagesFrom" runat="server"
                        TargetControlID="RejectMessagesFrom" meta:resourcekey="secRejectMessagesFrom" Text="Reject Messages From">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="RejectMessagesFrom" runat="server" Height="0" style="overflow:hidden;">
					    <fcp:RejectedSenders id="rejectAccounts" runat="server" />
					</asp:Panel>
					

				</div>
			</div>
				    <div class="card-footer text-end">
					    <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" ValidationGroup="EditContact"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>&nbsp;
					    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="EditContact" />
				    </div>
