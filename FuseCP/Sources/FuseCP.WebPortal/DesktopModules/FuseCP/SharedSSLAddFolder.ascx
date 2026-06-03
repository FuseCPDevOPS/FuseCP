<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharedSSLAddFolder.ascx.cs" Inherits="FuseCP.Portal.SharedSSLAddFolder" %>
<%@ Register Src="UserControls/FileLookup.ascx" TagName="FileLookup" TagPrefix="uc1" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc2" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-body form-horizontal">
    <table class="table table-borderless align-middle mb-0">
	    <tr>
		    <td class="SubHead" >
		        <asp:Label ID="lblDomain" runat="server" meta:resourcekey="lblDomain" Text="Domain:"></asp:Label>
		    </td>
		    <td class="Normal">
		        <asp:DropDownList ID="ddlDomains" runat="server" CssClass="form-control">
		        </asp:DropDownList>
                <asp:RequiredFieldValidator ID="valRequireDomain" runat="server" meta:resourcekey="valRequireDomain"
                    ErrorMessage="*" ControlToValidate="ddlDomains" Display="Dynamic"></asp:RequiredFieldValidator></td>
	    </tr>
	    <tr>
    		<td>&nbsp;</td>
	    </tr>
	    <tr>
		    <td class="SubHead align-top">
		        <asp:Label ID="lblWebSites" runat="server" meta:resourcekey="lblWebSites" Text="Web Site:"></asp:Label>
		    </td>
		    <td class="Normal align-top">
                <asp:DropDownList ID="ddlWebSites" runat="server" CssClass="form-control">
                </asp:DropDownList>&nbsp;&nbsp;
                <asp:RequiredFieldValidator ID="valRequireWebSite" runat="server" meta:resourcekey="valRequireWebSite"
                    ErrorMessage="*" ControlToValidate="ddlWebSites" Display="Dynamic"></asp:RequiredFieldValidator><br />
                <asp:Label ID="lblWebSitesComment" runat="server" meta:resourcekey="lblWebSitesComment" Text="* Created virtual directory will use security settings from this web site"></asp:Label>

	        </td>
	    </tr>
	    <tr>
    		<td>&nbsp;</td>
	    </tr>
	    <tr>
		    <td class="SubHead">
		        <asp:Label ID="lblDirectoryName" runat="server" meta:resourcekey="lblDirectoryName" Text="Directory name:"></asp:Label>
		    </td>
		    <td>
		        <uc2:UsernameControl ID="virtDirName" runat="server" />
		    </td>
	    </tr>
	    <tr>
		    <td class="SubHead align-top fcp-pt-7">
			    <asp:Label ID="lblFolder" runat="server" meta:resourcekey="lblFolder" Text="Folder:"></asp:Label>
	        </td>
		    <td class="Normal align-top">
                <uc1:FileLookup ID="fileLookup" runat="server" ValidationEnabled="true" />
    			
		    </td>
	    </tr>
    </table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" OnclientClick="ShowProgressDialog('Creating Shared SSL Folder...')"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>
</div>
