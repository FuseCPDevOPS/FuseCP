<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpaceImportResources.ascx.cs" Inherits="FuseCP.Portal.SpaceImportResources" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/space-import-resources.js"></script>

<div class="card-body form-horizontal">
	<asp:treeview runat="server" id="tree" populatenodesfromclient="true" NodeIndent="10"
		showexpandcollapse="true" expanddepth="0" ontreenodepopulate="tree_TreeNodePopulate" OnTreeNodeCheckChanged="tree_TreeNodeCheckChanged" OnTreeNodeExpanded="tree_TreeNodeExpanded">
		<LevelStyles>
		    <asp:TreeNodeStyle CssClass="FileManagerTreeNode" />
		    <asp:TreeNodeStyle CssClass="FileManagerTreeNode" />
		</LevelStyles>
	</asp:treeview>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnImport" CssClass="btn btn-success" runat="server" OnClick="btnImport_Click" OnClientClick="return confirm('Proceed?');"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnImportText"/> </asp:LinkButton>
</div>
