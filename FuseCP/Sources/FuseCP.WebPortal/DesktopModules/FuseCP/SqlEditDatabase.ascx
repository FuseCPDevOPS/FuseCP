<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SqlEditDatabase.ascx.cs" Inherits="FuseCP.Portal.SqlEditDatabase" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc2" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>

<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
	TagPrefix="fcp" %>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/mail-confirmation.js"></script>


<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />

<div class="card-body form-horizontal">
	<table class="table table-borderless align-middle mb-0 w-100">
		<tr>
			<td class="SubHead" ><asp:Label ID="lblDatabaseName" runat="server" meta:resourcekey="lblDatabaseName" Text="Database name:"></asp:Label></td>
			<td class="NormalBold">
                <uc2:UsernameControl ID="usernameControl" runat="server" />
			</td>
		</tr>
        <tr>
            <td class="SubHead" ><asp:Label ID="lblDBInternalServer" runat="server" meta:resourcekey="lblDBInternalServer" Text="Internal Server:"></asp:Label></td>
			<td class="Normal"><asp:Literal ID="litDBInternalServer" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <td class="SubHead" ><asp:Label ID="lblDBExternalServer" runat="server" meta:resourcekey="lblDBExternalServer" Text="External Server:"></asp:Label></td>		
			<td class="Normal"><asp:Literal ID="litDBExternalServer" runat="server"></asp:Literal></td>
        </tr>
	</table>
	<br />
	
    <fcp:CollapsiblePanel id="secUsers" runat="server"
        TargetControlID="UsersPanel" meta:resourcekey="secUsers" Text="Database Users">
    </fcp:CollapsiblePanel>
    <asp:Panel ID="UsersPanel" runat="server" Height="0" style="overflow:hidden">
	    <table class="table table-borderless align-middle mb-0 w-100">
		    <tr>
			    <td colspan="2">
				    <asp:CheckBoxList ID="dlUsers" runat="server" CssClass="NormalBold" DataTextField="Name" DataValueField="Name"
					    RepeatColumns="2"></asp:CheckBoxList>
			    </td>
		    </tr>
	    </table>
	</asp:Panel>
	
    <asp:PlaceHolder ID="providerControl" runat="server"></asp:PlaceHolder>

</div>
<div class="card-footer text-end">
	<asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return fuseCpConfirmWithProgress('Are you sure you want to delete this Database?', 'Deleting Database...');"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
	<asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick="ShowProgressDialog('Saving Database...');"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>
</div>

