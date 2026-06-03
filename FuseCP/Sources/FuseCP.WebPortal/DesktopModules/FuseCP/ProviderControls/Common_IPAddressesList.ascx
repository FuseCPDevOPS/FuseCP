<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Common_IPAddressesList.ascx.cs" Inherits="FuseCP.Portal.ProviderControls.Common_IPAddressesList" %>
<%@ Register Src="../UserControls/SelectIPAddress.ascx" TagName="SelectIPAddress" TagPrefix="uc1" %>
<table class="table table-borderless align-middle mb-0 w-100">
    <tr>
        <td class="align-top">
            <asp:ListBox ID="lbAddresses" runat="server" Width="200px" Rows="3" CssClass="form-control"></asp:ListBox></td>
        <td class="align-top">
            <asp:ImageButton ID="btnRemove" runat="server" SkinID="DeleteSmall" meta:resourcekey="btnRemove"
				OnClick="btnRemove_Click" /></td>
    </tr>
    <tr>
        <td colspan="2">
            <uc1:SelectIPAddress ID="ipAddress" runat="server" ServerIdParam="ServerID" />
            <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>
        </td>
    </tr>
</table>
