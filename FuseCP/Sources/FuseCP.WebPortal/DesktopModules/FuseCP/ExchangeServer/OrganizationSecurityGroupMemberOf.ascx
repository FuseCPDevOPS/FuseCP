<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganizationSecurityGroupMemberOf.ascx.cs" Inherits="FuseCP.Portal.HostedSolution.OrganizationSecurityGroupMemberOf" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>

<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="UserControls/AccountsList.ascx" TagName="AccountsList" TagPrefix="fcp" %>
<%@ Register Src="UserControls/SecurityGroupTabs.ascx" TagName="SecurityGroupTabs" TagPrefix="fcp"%>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
    <div class="card-header">
        <h3 class="card-title">
            <asp:Image ID="Image1" SkinID="OrganizationUser48" runat="server" />
            <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit User"></asp:Localize>
            -
            <asp:Literal ID="litDisplayName" runat="server" Text="John Smith" />
        </h3>
    </div>
    <div class="card-body form-horizontal fcp-modern-page">
        <fcp:SecurityGroupTabs id="tabs" runat="server" SelectedTab="secur_group_memberof" />
        <div class="card tab-content">
            <fcp:SimpleMessageBox id="messageBox" runat="server" />
            <fcp:CollapsiblePanel id="secGroups" runat="server" TargetControlID="GroupsPanel" meta:resourcekey="secGroups" Text="Groups"></fcp:CollapsiblePanel>
            <asp:Panel ID="GroupsPanel" runat="server" Height="0" style="overflow:hidden;">
                <asp:UpdatePanel ID="GeneralUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <fcp:AccountsList id="groups" runat="server" MailboxesEnabled="false" EnableMailboxOnly="true" ContactsEnabled="false" DistributionListsEnabled="true" SecurityGroupsEnabled="true" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>
    </div>
    <div class="card-footer text-end">
        <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" ValidationGroup="EditMailbox">
            <i class="bi bi-floppy me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnSaveText"/>
        </asp:LinkButton>
        &nbsp;
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="EditMailbox" />
    </div>
