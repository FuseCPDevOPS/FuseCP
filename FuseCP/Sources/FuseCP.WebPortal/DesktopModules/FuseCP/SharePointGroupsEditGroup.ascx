<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharePointGroupsEditGroup.ascx.cs" Inherits="FuseCP.Portal.SharePointGroupsEditGroup" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc2" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<div class="card-body form-horizontal">
    <table class="table table-borderless align-middle mb-0 w-100">
        <tr>
            <td class="SubHead text-nowrap"><asp:Label ID="lblGroupName" runat="server" meta:resourcekey="lblGroupName" Text="Group name:"></asp:Label></td>
            <td class="NormalBold">
                <uc2:UsernameControl ID="usernameControl" runat="server" />
            </td>
        </tr>
    </table>
    
    <fcp:CollapsiblePanel id="secUsers" runat="server"
        TargetControlID="UsersPanel" meta:resourcekey="secUsers" Text="Members">
    </fcp:CollapsiblePanel>
    <asp:Panel ID="UsersPanel" runat="server" Height="0" style="overflow:hidden">
        <table class="table table-borderless align-middle mb-0 w-100" id="tblUsers" runat="server">
            <tr>
                <td colspan="2">
	                <asp:checkboxlist id="dlUsers" RepeatColumns="2" CssClass="NormalBold" Runat="server"
		                DataValueField="Name" DataTextField="Name"></asp:checkboxlist>
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
