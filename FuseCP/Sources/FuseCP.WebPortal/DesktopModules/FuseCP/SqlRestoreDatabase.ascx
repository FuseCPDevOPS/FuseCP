<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SqlRestoreDatabase.ascx.cs" Inherits="FuseCP.Portal.SqlRestoreDatabase" %>
<%@ Register Src="UserControls/FileLookup.ascx" TagName="FileLookup" TagPrefix="uc1" %>
<div class="card-body form-horizontal">
    <div class="Huge">
        <asp:Literal id="litDatabaseName" runat="server"></asp:Literal>
    </div>
    <br />
    <div class="FormRow">
        <asp:Label ID="lblRestoreFrom" runat="server" meta:resourcekey="lblRestoreFrom" Text="Restore From:"></asp:Label>
    </div>
    
	<table width=100%>
		<tr>
			<td class="Normal"><asp:radiobutton id="radioUpload" meta:resourcekey="radioUpload" Checked="True" GroupName="media" Text="Uploaded File" Runat="server"
					AutoPostBack="True" OnCheckedChanged="radioUpload_CheckedChanged"></asp:radiobutton></td>
		</tr>
		<tr>
			<td class="Normal"><asp:radiobutton id="radioFile" meta:resourcekey="radioFile" GroupName="media" Text="Hosting Space File" Runat="server"
					AutoPostBack="True" OnCheckedChanged="radioUpload_CheckedChanged"></asp:radiobutton></td>
		</tr>
		<tr>
			<td class="Normal" id="cellUploadFile" runat="server">
				<table width=100%>
					<tr>
						<td>
                            <asp:FileUpload ID="uploadFile" runat="server" Width="300px" /></td>
					</tr>
					<tr>
						<td class="Small text-nowrap"><asp:Label ID="lblAllowedFiles1" runat="server" meta:resourcekey="lblAllowedFiles" Text=".ZIP, .BAK and .SQL files are allowed"></asp:Label></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="Normal" id="cellFile" runat="server">
				<table width=100%>
					<tr>
						<td>
                            <uc1:FileLookup ID="fileLookup" runat="server" IncludeFiles="true" />
                        </td>
					</tr>
					<tr>
						<td class="Small text-nowrap"><asp:Label ID="lblAllowedFiles2" runat="server" meta:resourcekey="lblAllowedFiles" Text=".ZIP, .BAK and .SQL files are allowed"></asp:Label></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
	<asp:LinkButton id="btnRestore" CssClass="btn btn-success" runat="server" OnClick="btnRestore_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnRestore"/> </asp:LinkButton>
</div>
