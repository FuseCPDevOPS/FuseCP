<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharePointEditSite.ascx.cs" Inherits="FuseCP.Portal.SharePointEditSite" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="uc3" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc3" %>
<%@ Register Src="UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="uc2" %>
<%@ Register Src="UserControls/EmailControl.ascx" TagName="EmailControl" TagPrefix="uc2" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-body form-horizontal">
<table class="table table-borderless align-middle mb-0 w-100" id="tblEditItem" runat="server">
	<tr>
		<td class="SubHead text-nowrap" width=200><asp:Label ID="lblWebSite" runat="server" meta:resourcekey="lblWebSite" Text="Web Site:"></asp:Label></td>
		<td class="NormalBold">
            <asp:DropDownList ID="ddlWebSites" runat="server" CssClass="form-control"
                DataTextField="Name" DataValueField="Name">
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="valRequireWebSite" runat="server" ErrorMessage="*" ControlToValidate="ddlWebSites"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblSiteLocaleID" runat="server" meta:resourcekey="lblSiteLocaleID" Text="Site Locale ID:"></asp:Label>
	    </td>
	    <td class="Normal">
            <asp:TextBox ID="txtLocaleID" runat="server" Width="50px" Text="1033"></asp:TextBox>
            <asp:HyperLink ID="localesList" runat="server" meta:resourcekey="localesList" Text="Full list of locales"
                Target="_blank" NavigateUrl="http://www.microsoft.com/globaldev/reference/lcid-all.mspx"></asp:HyperLink>
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblSiteOwner" runat="server" meta:resourcekey="lblSiteOwner" Text="Site Owner:"></asp:Label>
	    </td>
	    <td class="Normal">
            <asp:DropDownList ID="ddlSiteOwner" runat="server" CssClass="form-control"
                DataValueField="Name" DataTextField="Name">
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="valRequireSiteOwner" runat="server" ControlToValidate="ddlSiteOwner"
                ErrorMessage="*"></asp:RequiredFieldValidator></td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblOwnerEmail" runat="server" meta:resourcekey="lblOwnerEmail" Text="Owner E-mail:"></asp:Label>
	    </td>
	    <td class="Normal">
            <uc2:EmailControl id="txtOwnerEmail" runat="server">
            </uc2:EmailControl>
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblDatabaseVersion" runat="server" meta:resourcekey="lblDatabaseVersion" Text="Database Version:"></asp:Label>
	    </td>
	    <td class="Normal">
            <asp:DropDownList ID="ddlDatabaseVersion" runat="server"
                CssClass="form-control" meta:resourcekey="ddlDatabaseVersion">
            </asp:DropDownList>
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblDatabaseName" runat="server" meta:resourcekey="lblDatabaseName" Text="Content Database Name:"></asp:Label>
	    </td>
	    <td class="Normal">
	        <uc3:UsernameControl ID="databaseName" runat="server" />
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblDatabaseUser" runat="server" meta:resourcekey="lblDatabaseUser" Text="Database User:"></asp:Label>
	    </td>
	    <td class="Normal">
	        <uc3:UsernameControl ID="databaseUser" runat="server" />
        </td>
	</tr>
	<tr>
	    <td class="SubHead align-top">
	        <asp:Label ID="lblDatabasePassword" runat="server" meta:resourcekey="lblDatabasePassword" Text="Database Password:"></asp:Label>
	    </td>
		<td class="Normal">
            <uc3:PasswordControl ID="databasePassword" runat="server" />
        </td>
	</tr>
</table>

<table class="table table-borderless align-middle mb-0 w-100" id="tblViewItem" runat="server">
	<tr>
		<td class="SubHead text-nowrap"><asp:Label ID="lblWebSite2" runat="server" meta:resourcekey="lblWebSite" Text="Web Site:"></asp:Label></td>
		<td class="NormalBold">
		    <asp:Literal ID="litWebSite" runat="server"></asp:Literal>
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblSiteLocaleID2" runat="server" meta:resourcekey="lblSiteLocaleID" Text="Site Locale ID:"></asp:Label>
	    </td>
	    <td class="Normal">
            <asp:Literal ID="litLocaleID" runat="server"></asp:Literal>
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblSiteOwner2" runat="server" meta:resourcekey="lblSiteOwner" Text="Site Owner:"></asp:Label>
	    </td>
	    <td class="Normal">
	        <asp:Literal ID="litSiteOwner" runat="server"></asp:Literal>
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblOwnerEmail2" runat="server" meta:resourcekey="lblOwnerEmail" Text="Owner E-mail:"></asp:Label>
	    </td>
	    <td class="Normal">
	        <asp:Literal ID="litOwnerEmail" runat="server"></asp:Literal>
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblDatabaseName2" runat="server" meta:resourcekey="lblDatabaseName" Text="Content Database Name:"></asp:Label>
	    </td>
	    <td class="Normal">
	        <asp:Literal ID="litDatabaseName" runat="server"></asp:Literal>
        </td>
	</tr>
	<tr>
	    <td class="SubHead">
	        <asp:Label ID="lblDatabaseUser2" runat="server" meta:resourcekey="lblDatabaseUser" Text="Database User:"></asp:Label>
	    </td>
	    <td class="Normal">
	        <asp:Literal ID="litDatabaseUser" runat="server"></asp:Literal>
        </td>
	</tr>
</table>

<table>
    <tr>
        <td>
            <fcp:CollapsiblePanel id="secMainTools" runat="server" IsCollapsed="true"
                TargetControlID="ToolsPanel" meta:resourcekey="secMainTools" Text="SharePoint Site Tools">
            </fcp:CollapsiblePanel>
            <asp:Panel ID="ToolsPanel" runat="server" Height="0" style="overflow:hidden">
                <table class="table table-borderless align-middle mb-0" id="tblMaintenance" runat="server">
                    <tr>
                        <td>
                            <asp:Button ID="btnBackup" runat="server" meta:resourcekey="btnBackup" CausesValidation="false" 
                                Text="Backup Site" CssClass="btn btn-primary" OnClick="btnBackup_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnRestore" runat="server" meta:resourcekey="btnRestore" CausesValidation="false" 
                                Text="Restore Site" CssClass="btn btn-primary" OnClick="btnRestore_Click" />                    
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnWebParts" runat="server" meta:resourcekey="btnWebParts" CausesValidation="false" 
                                Text="WebParts Packages" CssClass="btn btn-primary" OnClick="btnWebParts_Click" />                    
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return confirm('Delete Site?');"  meta:resourcekey="btnDelete"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click" meta:resourcekey="btnCancel"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" meta:resourcekey="btnUpdate"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdateText"/> </asp:LinkButton>
</div>
