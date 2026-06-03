<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharePointUsersEditUser.ascx.cs" Inherits="FuseCP.Portal.SharePointUsersEditUser" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc3" %>
<%@ Register Src="UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="uc2" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<div class="card-body form-horizontal">
    <table class="table table-borderless align-middle mb-0 w-100">
        <tr>
            <td class="SubHead text-nowrap"><asp:Label ID="lblUserName" runat="server" meta:resourcekey="lblUserName" Text="User Name:"></asp:Label></td>
            <td class="NormalBold">
                <uc3:UsernameControl ID="usernameControl" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="SubHead align-top">
                <asp:Label ID="lblUserPassword" runat="server" meta:resourcekey="lblUserPassword" Text="User Password:"></asp:Label></td>
            <td class="Normal align-top">
                <uc2:PasswordControl ID="passwordControl" runat="server" />
            </td>
        </tr>
    </table>
    
    <fcp:CollapsiblePanel id="secGroups" runat="server"
        TargetControlID="GroupsPanel" meta:resourcekey="secGroups" Text="Member Of">
    </fcp:CollapsiblePanel>
    <asp:Panel ID="GroupsPanel" runat="server" Height="0" style="overflow:hidden">
        <table class="table table-borderless align-middle mb-0 w-100" id="tblGroups" runat="server">
            <tr>
                <td colspan="2">
	                <asp:checkboxlist id="dlGroups" RepeatColumns="2" CssClass="NormalBold" DataTextField="Name"
		                DataValueField="Name" Runat="server"></asp:checkboxlist>
                </td>
            </tr>
        </table>
    </asp:Panel>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return confirm('Delete this user?');"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
	<asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSave"/> </asp:LinkButton>
</div>
