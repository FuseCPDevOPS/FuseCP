<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesEditWebGroup.ascx.cs" Inherits="FuseCP.Portal.WebSitesEditWebGroup" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc2" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<div class="card-body form-horizontal">
<table class="table table-borderless align-middle mb-0 w-100">
	<tr>
		<td>
            <table class="table table-borderless align-middle mb-0 w-100">
	            <tr>
		            <td class="SubHead" ><asp:Label ID="lblGroupName" runat="server" meta:resourcekey="lblGroupName" Text="Group name:"></asp:Label></td>
		            <td class="NormalBold">
                        <uc2:UsernameControl ID="usernameControl" runat="server" />
                    </td>
	            </tr>
            </table>
            
            <fcp:CollapsiblePanel id="secUsers" runat="server"
                TargetControlID="UsersPanel" meta:resourcekey="secUsers" Text="Members">
            </fcp:CollapsiblePanel>
	        <asp:Panel ID="UsersPanel" runat="server" Height="0" style="overflow:hidden">
                <table class="table table-borderless align-middle mb-0 w-100">
	                <tr>
		                <td colspan="2">
			                <asp:checkboxlist id="dlUsers" RepeatColumns="2" CssClass="NormalBold" Runat="server"
				                DataValueField="Name" DataTextField="Name"></asp:checkboxlist>
		                </td>
	                </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdate"/> </asp:LinkButton>
</div>
