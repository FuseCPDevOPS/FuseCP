<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserCustomersSummary.ascx.cs" Inherits="FuseCP.Portal.UserCustomersSummary" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="UserControls/SearchBox.ascx" TagName="SearchBox" TagPrefix="uc1" %>

<%@ Import Namespace="FuseCP.Portal" %>
<div class="FormButtonsBar right fcp-home-create-toolbar">
	<div class="right">
		<asp:LinkButton id="btnCreate" CssClass="btn btn-primary" runat="server" OnClick="btnCreate_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCreate"/> </asp:LinkButton>
	</div>
	<div class="Right">
		<%-- <asp:Panel ID="tblSearch" runat="server" CssClass="NormalBold">
            <uc1:SearchBox ID="searchBox" runat="server" />
		</asp:Panel> --%>
	</div>
</div>
<div class="card-body form-horizontal">

	<fcp:CollapsiblePanel id="allCustomers" runat="server"
		TargetControlID="AllCustomersPanel" resourcekey="AllCustomersPanel" Text="All Customers">
	</fcp:CollapsiblePanel>
	<asp:Panel ID="AllCustomersPanel" runat="server" CssClass="FormRow">
		<asp:HyperLink ID="lnkAllCustomers" runat="server" Text="All Customers" meta:resourcekey="lnkAllCustomers"></asp:HyperLink>
	</asp:Panel>
	<fcp:CollapsiblePanel id="byStatus" runat="server"
		TargetControlID="ByStatusPanel" resourcekey="ByStatusPanel" Text="By Status">
	</fcp:CollapsiblePanel>
	<asp:Panel ID="ByStatusPanel" runat="server" CssClass="FormRow">
		<asp:Repeater ID="repUserStatuses" runat="server" EnableViewState="false">
			<ItemTemplate>
				<asp:HyperLink ID="lnkBrowseStatus" runat="server" NavigateUrl='<%# GetUserCustomersPageUrl("StatusID", Eval("StatusID").ToString()) %>'>
					<%# PanelFormatter.GetAccountStatusName((int)Eval("StatusID"))%> (<%# Eval("UsersNumber")%>)
				</asp:HyperLink>
			</ItemTemplate>
			<SeparatorTemplate>&nbsp;&nbsp;</SeparatorTemplate>
		</asp:Repeater>
	</asp:Panel>
	<fcp:CollapsiblePanel id="byRole" runat="server"
		TargetControlID="ByRolePanel" resourcekey="ByRolePanel" Text="By Role">
	</fcp:CollapsiblePanel>
	<asp:Panel ID="ByRolePanel" runat="server" CssClass="FormRow">
		<asp:Repeater ID="repUserRoles" runat="server" EnableViewState="false">
			<ItemTemplate>
				<asp:HyperLink ID="lnkBrowseRole" runat="server" NavigateUrl='<%# GetUserCustomersPageUrl("RoleID", Eval("RoleID").ToString()) %>'>
					<%# PanelFormatter.GetUserRoleName((int)Eval("RoleID"))%> (<%# Eval("UsersNumber")%>)
				</asp:HyperLink>
			</ItemTemplate>
			<SeparatorTemplate>&nbsp;&nbsp;</SeparatorTemplate>
		</asp:Repeater>
	</asp:Panel>
</div>
