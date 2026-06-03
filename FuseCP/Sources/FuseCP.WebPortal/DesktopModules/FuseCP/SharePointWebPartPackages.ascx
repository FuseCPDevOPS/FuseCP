<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharePointWebPartPackages.ascx.cs" Inherits="FuseCP.Portal.SharePointWebPartPackages" %>
<div class="card-body form-horizontal">
    <table class="table table-borderless align-middle mb-0 w-100">
	    <tr>
		    <td class="Huge" colspan="2"><asp:Literal id="litSiteName" runat="server"></asp:Literal></td>
	    </tr>
	    <tr>
	        <td colspan="2">
	            <br />
                <asp:Button id="btnInstall" runat="server" meta:resourcekey="btnInstall" Text="Install Package" CssClass="btn btn-success" CausesValidation="false" OnClick="btnInstall_Click"/>
	        </td>
	    </tr>
	    <tr>
		    <td>
    		
        		
                <asp:ListBox ID="lbWebPartPackages" runat="server" Rows="10" Width="300px">
                </asp:ListBox>



		    </td>
		    <td class="align-top">
                <asp:LinkButton id="btnUninstall" CssClass="btn btn-danger" runat="server" OnClick="btnUninstall_Click" CausesValidation="false" OnClientClick="return confirm('Uninstall?');"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUninstallText"/> </asp:LinkButton>
		    </td>
	    </tr>
    </table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>
</div>
