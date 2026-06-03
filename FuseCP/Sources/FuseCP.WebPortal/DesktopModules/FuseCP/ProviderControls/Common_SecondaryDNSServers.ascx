<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Common_SecondaryDNSServers.ascx.cs" Inherits="FuseCP.Portal.ProviderControls.Common_SecondaryDNSServers" %>
<%@ Register Src="../UserControls/SelectIPAddress.ascx" TagName="SelectIPAddress" TagPrefix="uc1" %>
<table class="table table-borderless align-middle mb-0">
    <tr>
        <td class="align-top">
            <asp:ListBox ID="lbServices" runat="server" Width="200px" Rows="3" CssClass="form-control"></asp:ListBox></td>
        <td class="align-top">
            <asp:ImageButton ID="btnRemove" runat="server" SkinID="DeleteSmall"
				meta:resourcekey="btnRemove" OnClick="btnRemove_Click" /></td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddlService" runat="server" CssClass="form-control">
            </asp:DropDownList></td>
        <td><asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton></td>
    </tr>
</table>
