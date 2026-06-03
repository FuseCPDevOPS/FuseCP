<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostingPlans.ascx.cs" Inherits="FuseCP.Portal.HostingPlans" %>
<%@ Import Namespace="FuseCP.Portal" %>
<%@ Register Src="UserControls/ServerDetails.ascx" TagName="ServerDetails" TagPrefix="uc3" %>
<div class="FormButtonsBar right">
	<asp:LinkButton ID="btnAddItem" runat="server" CssClass="btn btn-primary" OnClick="btnAddItem_Click" >
        <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddItem"/>
	</asp:LinkButton>
</div>
<asp:GridView id="gvPlans" runat="server" AutoGenerateColumns="False"
	DataSourceID="odsPlans" AllowPaging="True" AllowSorting="True" EmptyDataText="gvPlans"
	CssSelectorClass="NormalGridView">
	<Columns>
		<asp:TemplateField SortExpression="PlanName" HeaderText="gvPlansName">
			<ItemStyle></ItemStyle>
			<ItemTemplate>
				<b><asp:hyperlink id="lnkEdit" runat="server" NavigateUrl='<%# EditUrl("PlanID", Eval("PlanID").ToString(), "edit_plan", "UserID=" + Eval("UserID").ToString()) %>'>
					<%# PortalAntiXSS.EncodeOld((string)Eval("PlanName")) %>
				</asp:hyperlink></b><br />
				<%# PortalAntiXSS.EncodeOld((string)Eval("PlanDescription")) %>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
				<asp:hyperlink id="lnkCopy" runat="server" CssClass="btn btn-secondary btn-sm" NavigateUrl='<%# EditUrl("PlanID", Eval("PlanID").ToString(), "edit_plan", "UserID=" + Eval("UserID").ToString(), "TargetAction=Copy") %>'><i class="bi bi-copy me-1"></i><asp:Localize runat="server" meta:resourcekey="lnkCopy"/></asp:hyperlink>
			</ItemTemplate>
		</asp:TemplateField>
        <asp:TemplateField SortExpression="ServerName" HeaderText="gvPlansServer"
                HeaderStyle-Wrap="false">
            <ItemStyle Wrap="false"></ItemStyle>
            <ItemTemplate>
		         <uc3:ServerDetails ID="serverDetails" runat="server"
		            ServerID='<%# Eval("ServerID") %>'
		            ServerName='<%# Eval("ServerName") %>' />
            </ItemTemplate>
        </asp:TemplateField>
		<asp:BoundField DataField="PackagesNumber" SortExpression="PackagesNumber" HeaderText="gvPlansSpaces"></asp:BoundField>
	</Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsPlans" runat="server" SelectMethod="GetRawHostingPlans"
TypeName="FuseCP.Portal.HostingPlansHelper" OnSelected="odsPlans_Selected"></asp:ObjectDataSource>
