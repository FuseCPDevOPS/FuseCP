<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserDeleteUserAccount.ascx.cs" Inherits="FuseCP.Portal.UserDeleteUserAccount" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-body form-horizontal">
<table>
	<tr>
		<td class="Normal">
			<asp:Label ID="lblAccountContains" runat="server" meta:resourcekey="lblAccountContains" Text="The account contains ..."></asp:Label>
			<br/>
			<br/>
			<asp:GridView ID="gvPackages" Runat="server" AutoGenerateColumns="False"
			    EmptyDataText="gvPackages"
			    CssSelectorClass="NormalGridView">
				<columns>
					<asp:BoundField DataField="PackageName" HeaderText="gvPackagesPackage"></asp:BoundField>
				</columns>
			</asp:GridView>
			<br/>
		</td>
	</tr>
	<tr>
		<td class="Normal">
		    <asp:Label ID="lblWarning" runat="server" meta:resourcekey="lblWarning" Text="Deleting this user also..."></asp:Label>
			<br/>
			<br/>
		</td>
	</tr>
	<tr>
		<td class="Normal">
			<asp:CheckBox ID="chkConfirm" Runat="server" meta:resourcekey="chkConfirm" Text="Yes, I understand ..."></asp:CheckBox>
			<br/>
		</td>
	</tr>
</table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="ShowProgressDialog('Deleting...');"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>
</div>
