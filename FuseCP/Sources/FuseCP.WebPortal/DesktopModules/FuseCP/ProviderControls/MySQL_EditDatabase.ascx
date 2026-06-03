<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MySQL_EditDatabase.ascx.cs" Inherits="FuseCP.Portal.ProviderControls.MySQL_EditDatabase" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<fcp:CollapsiblePanel id="secDataFiles" runat="server" IsCollapsed="true"
    TargetControlID="FilesPanel" meta:resourcekey="secDataFiles" Text="Database Files">
</fcp:CollapsiblePanel>
<asp:Panel ID="FilesPanel" runat="server" Height="0" style="overflow:hidden">
    <table id="tblFiles" runat="server" class="table table-borderless align-middle mb-0 w-100">
        <tr>
            <td  class="Medium">
                <asp:Label ID="lblDataFile" runat="server" meta:resourcekey="lblDataFile" Text="Data File"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="align-top">
                <table class="table table-borderless align-middle mb-0">
	                <tr>
		                <td class="SubHead text-nowrap"><asp:Label ID="lblDataSize" runat="server" meta:resourcekey="lblSize" Text="Size, KB:"></asp:Label></td>
		                <td class="Normal"><asp:Literal id="litDataSize" Runat="server" Text="0"></asp:Literal></td>
	                </tr>
                </table>
            </td>
        </tr>
    </table> 
</asp:Panel>
<fcp:CollapsiblePanel id="secMainTools" runat="server" IsCollapsed="true"
    TargetControlID="MainToolsPanel" meta:resourcekey="secMainTools" Text="Maintenance Tools">
</fcp:CollapsiblePanel>
<asp:Panel ID="MainToolsPanel" runat="server" Height="0" style="overflow:hidden">
    <table class="table table-borderless align-middle mb-0">
        <tr>
            <td>
                <asp:LinkButton id="btnBackup" CssClass="btn btn-primary" runat="server" OnClick="btnBackup_Click" CausesValidation="false"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnBackupText"/> </asp:LinkButton>&nbsp;
                <asp:LinkButton id="btnRestore" CssClass="btn btn-warning" runat="server" OnClick="btnRestore_Click" CausesValidation="false"> <i class="bi bi-repeat me-1"></i><asp:Localize runat="server" meta:resourcekey="btnRestoreText"/> </asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Panel>

<fcp:CollapsiblePanel id="secHousekeepingTools" runat="server" IsCollapsed="true"
    TargetControlID="HousekeepingToolsPanel" meta:resourcekey="secHousekeepingTools" Text="Housekeeping Tools">
</fcp:CollapsiblePanel>
<asp:Panel ID="HousekeepingToolsPanel" runat="server" Height="0" style="overflow:hidden">
    <table class="table table-borderless align-middle mb-0">
        <tr>
            <td>
                <asp:Button ID="btnTruncate" runat="server" meta:resourcekey="btnTruncate" CausesValidation="false" 
                    Text="Truncate Files" CssClass="btn btn-primary" OnClick="btnTruncate_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
