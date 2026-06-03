<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesEditHeliconApeFolderAuth.ascx.cs" Inherits="FuseCP.Portal.WebSitesEditHeliconApeFolderAuth" %>
<%@ Register Src="UserControls/FileLookup.ascx" TagName="FileLookup" TagPrefix="uc1" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<div class="card-body form-horizontal">
<table class="table table-borderless align-middle mb-0 w-100">
	<tr>
		<td>
            <table class="table table-borderless align-middle mb-0 w-100">
	            <tr>
		            <td class="SubHead" >
						<asp:Label ID="lblFolderTitle" runat="server" meta:resourcekey="lblFolderTitle" Text="AuthName"></asp:Label>
					</td>
		            <td class="NormalBold">
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox>
                    </td>
	            </tr>
	            <tr>
		            <td class="SubHead"><asp:Label ID="lblFolderName" runat="server" meta:resourcekey="lblFolderName" Text="Folder Path"></asp:Label></td>
		            <td class="NormalBold">
                        <uc1:FileLookup id="folderPath" runat="server">
                        </uc1:FileLookup></td>
	            </tr>
	            <tr>
		            <td class="SubHead"><asp:Label ID="lblAythType" runat="server" meta:resourcekey="lblAuthType" Text="AuthType"></asp:Label></td>
		            <td class="NormalBold">
                    <asp:RadioButtonList runat="server" ID="rblAuthType">
                    </asp:RadioButtonList>
                    </td>
	            </tr>
            </table>
            
            <fcp:CollapsiblePanel id="secUsers" runat="server"
                TargetControlID="UsersPanel" meta:resourcekey="secUsers" Text="Allowed Users">
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
            
            <fcp:CollapsiblePanel id="secGroups" runat="server"
                TargetControlID="GroupsPanel" meta:resourcekey="secGroups" Text="Allowed Groups">
            </fcp:CollapsiblePanel>
	        <asp:Panel ID="GroupsPanel" runat="server" Height="0" style="overflow:hidden">
                <table class="table table-borderless align-middle mb-0 w-100">
	                <tr>
		                <td colspan="2">
			                <asp:checkboxlist id="dlGroups" RepeatColumns="2" CssClass="NormalBold" Runat="server"
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
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" > <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdate"/> </asp:LinkButton>
 </div>
