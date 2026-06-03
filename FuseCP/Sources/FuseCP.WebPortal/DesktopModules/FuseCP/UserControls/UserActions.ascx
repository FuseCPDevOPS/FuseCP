<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserActions.ascx.cs" Inherits="FuseCP.Portal.UserActions" %>
<%@ Register Src="../ExchangeServer/UserControls/MailboxPlanSelector.ascx" TagName="MailboxPlanSelector" TagPrefix="fcp" %>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/user-actions.js"></script>

<style type="text/css">
    .accounts-without-phone-list li {
        list-style-type: circle;
        margin-left: 10px;
    }
</style>

<asp:UpdatePanel ID="tblActions" runat="server" CssClass="NormalBold user-actions-toolbar" UpdateMode="Conditional" ChildrenAsTriggers="true" >
    <ContentTemplate>
        <div class="input-group user-actions-group">
        <asp:DropDownList ID="ddlUserActions" runat="server" CssClass="form-select" resourcekey="ddlUserActions" 
            AutoPostBack="True">
            <asp:ListItem Value="0">Actions</asp:ListItem>
            <asp:ListItem Value="1">Disable</asp:ListItem>
            <asp:ListItem Value="2">Enable</asp:ListItem>
            <asp:ListItem Value="3">SetServiceLevel</asp:ListItem>
            <asp:ListItem Value="4">SetVIP</asp:ListItem>
            <asp:ListItem Value="5">UnsetVIP</asp:ListItem>
            <asp:ListItem Value="6">SetMailboxPlan</asp:ListItem>
            <asp:ListItem Value="7">SendBySms</asp:ListItem>
            <asp:ListItem Value="8">SendByEmail</asp:ListItem>
        </asp:DropDownList>
        <span class="user-actions-apply">
            <asp:LinkButton id="btnApply" CssClass="btn btn-primary" runat="server" OnClick="btnApply_Click" OnClientClick="return ShowUserActionProgress(this);"><asp:Localize runat="server" meta:resourcekey="btnApplyText"/> </asp:LinkButton>
        </span>
        </div>

        
        <ajaxToolkit:ModalPopupExtender ID="Modal" runat="server" EnableViewState="true" TargetControlID="FakeModalPopupTarget"
             PopupControlID="FakeModalPopupTarget" BackgroundCssClass="modalBackground" DropShadow="false" />
        
        <%--Set Service Level--%>
        <asp:Panel ID="ServiceLevelPanel" runat="server" Style="display: none">
            <div class="widget">
             <div class="widget-header clearfix">
                           <h3><i class="bi bi-shield"></i>  <asp:Localize ID="headerServiceLevel" runat="server" meta:resourcekey="headerServiceLevel"></asp:Localize></h3>
                 </div>
                    <div class="widget-content Popup">
                    <asp:Literal ID="litServiceLevel" runat="server" meta:resourcekey="litServiceLevel"></asp:Literal>
                    <br/>
                    <asp:DropDownList ID="ddlServiceLevels" runat="server" CssClass="form-control" />
                    </div>
					<div class="popup-buttons text-end">
                    <asp:LinkButton id="btnServiceLevelCancel" CssClass="btn btn-warning" runat="server" OnClick="btnModalCancel_OnClick" CausesValidation="false"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnServiceLevelCancelText"/> </asp:LinkButton>&nbsp;
                    <asp:LinkButton id="btnServiceLevelOk" CssClass="btn btn-success" runat="server" OnClick="btnModalOk_Click" OnClientClick="return CloseAndShowUserActionProgress('Setting Service Level...')"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnServiceLevelOkText"/> </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>

        <%--Set MailboxPlan--%>
        <asp:Panel ID="MailboxPlanPanel" runat="server" Style="display: none">
            <div class="widget">
             <div class="widget-header clearfix">
                           <h3><i class="bi bi-envelope"></i>  <asp:Localize ID="headerMailboxPlanLabel" runat="server" meta:resourcekey="headerMailboxPlanLabel"></asp:Localize></h3>
                   </div>
                    <div class="widget-content Popup">
                    <asp:Literal ID="litMailboxPlan" runat="server" meta:resourcekey="litMailboxPlan"></asp:Literal>
                    <br/>
                    <fcp:MailboxPlanSelector ID="mailboxPlanSelector" runat="server" />
                    </div>
					<div class="popup-buttons text-end">
                    <asp:LinkButton id="btnMailboxPlanCancel" CssClass="btn btn-warning" runat="server" OnClick="btnModalCancel_OnClick" CausesValidation="false"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnMailboxPlanCancelText"/> </asp:LinkButton>&nbsp;
                    <asp:LinkButton id="btnMailboxPlanOk" CssClass="btn btn-success" runat="server" OnClick="btnModalOk_Click" OnClientClick="return CloseAndShowUserActionProgress('Setting Mailbox Plan ...')"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnMailboxPlanOkText"/> </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>
        
        <%--Send password reset notification--%>
        <asp:Panel ID="PasswordResetNotificationPanel" runat="server" Style="display: none">
            <div class="widget">
             <div class="widget-header clearfix">
                           <h3><i class="bi bi-key"></i>  <asp:Localize ID="headerPasswordResetSendBySms" runat="server" meta:resourcekey="headerPasswordResetSendBySms"></asp:Localize><h3 />
                    </div>
                    <div class="widget-content Popup">
                    <asp:Literal ID="litAccountsWithoutPhone" runat="server" meta:resourcekey="litAccountsWithoutPhone"></asp:Literal>
                    <br/>
                    <ul class="accounts-without-phone-list">
                        <asp:Repeater runat="server" ID="repAccountsWithoutPhone">
                            <ItemTemplate>
                                <li> <%# Eval("DisplayName") %> </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                     </div>
					<div class="popup-buttons text-end">
                    <asp:LinkButton id="btnPasswordResetNotificationSendCancel" CssClass="btn btn-warning" runat="server" OnClick="btnModalCancel_OnClick" CausesValidation="False"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnPasswordResetNotificationSendCancelText"/> </asp:LinkButton>&nbsp;
                    <asp:LinkButton id="btnPasswordResetNotificationSend" CssClass="btn btn-success" runat="server" OnClick="btnModalOk_Click" OnClientClick="return CloseAndShowUserActionProgress('Sending password reset notification ...')"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnPasswordResetNotificationText"/> </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>

        
        <asp:Button ID="FakeModalPopupTarget" runat="server" Style="display: none;" />
    </ContentTemplate>
    
    <Triggers>
        <asp:PostBackTrigger ControlID="btnServiceLevelOk" />
        <asp:PostBackTrigger ControlID="btnMailboxPlanOk" />
        <asp:PostBackTrigger ControlID="btnPasswordResetNotificationSend" />
        <asp:PostBackTrigger ControlID="btnApply" />
    </Triggers>
</asp:UpdatePanel>
