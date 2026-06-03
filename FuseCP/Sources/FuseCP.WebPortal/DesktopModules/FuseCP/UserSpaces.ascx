<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserSpaces.ascx.cs" Inherits="FuseCP.Portal.UserSpaces" %>
<%@ Import Namespace="FuseCP.Portal" %>
<%@ Register Src="UserControls/ServerDetails.ascx" TagName="ServerDetails" TagPrefix="uc3" %>
<%@ Register Src="UserControls/Comments.ascx" TagName="Comments" TagPrefix="uc4" %>
<%@ Register Src="UserOrganization.ascx" TagName="UserOrganization" TagPrefix="fcp" %>
<%@ Import Namespace="FuseCP.Portal" %>

<script src="/JavaScript/chosen.min.js" type="text/javascript"></script>

<asp:Panel id="ButtonsPanel" runat="server" CssClass="FormButtonsBar UserSpaces right">
    <asp:LinkButton id="btnAddItem" CssClass="btn btn-primary" runat="server" OnClick="btnAddItem_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddItem"/> </asp:LinkButton>
</asp:Panel>


<asp:Panel ID="UserPackagesPanel" runat="server" Visible="false" CssClass="fcp-user-packages-panel">
    <div class="card-body">
        <div class="space-select">
            <asp:DropDownList ID="ddlPackageSelect" OnSelectedIndexChanged="openSelectedPackage" AutoPostBack="true" CssClass="form-control" runat="server" Visible="false" />
        </div>
        <asp:Repeater ID="PackagesList" runat="server" EnableViewState="false">
            <ItemTemplate>
                    <div>
                        <asp:Repeater ID="PackageGroups" runat="server" DataSource='<%# GetIconsDataSource((int)Eval("PackageID"))  %>' > 
                            <ItemTemplate>
                                <h4>
                                <asp:Label ID="lblGroup" runat="server" CssClass="LinkText" Text='<%# Eval("Text") %>' />
                                </h4>
                                <asp:DataList ID="PackageIcons" runat="server" DataSource='<%# GetIconMenuItems(Eval("ChildItems")) %>'
                                    RepeatColumns="5" RepeatDirection="Horizontal">
                                    <ItemTemplate>
                                        <asp:Panel ID="IconPanel" runat="server" CssClass="Icon">
                                            <asp:HyperLink ID="imgLink" runat="server" NavigateUrl='<%# Eval("NavigateURL") %>'><asp:Image ID="imgIcon" runat="server" CssClass="fcp-package-icon" ImageUrl='<%# Eval("ImageUrl") %>' AlternateText='<%# Eval("Text") %>' ToolTip='<%# Eval("Text") %>' /></asp:HyperLink>
                                            <br />
                                            <asp:HyperLink ID="lnkIcon" runat="server" NavigateUrl='<%# Eval("NavigateURL") %>'><%# Eval("Text") %></asp:HyperLink>
                                        </asp:Panel>
                                        <asp:Panel ID="IconMenu" runat="server" CssClass="IconMenu" Visible='<%# IsIconMenuVisible(Eval("ChildItems")) %>'>
                                            <ul>
                                                <asp:Repeater ID="MenuItems" runat="server" DataSource='<%# GetIconMenuItems(Eval("ChildItems")) %>'>
                                                    <ItemTemplate>
                                                        <li><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("NavigateURL") %>'><%# Eval("Text") %></asp:HyperLink></li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ul>
                                        </asp:Panel>
                                        <ajaxToolkit:HoverMenuExtender TargetControlID="IconPanel" PopupControlID="IconMenu" runat="server"
                                            PopupPosition="Right" HoverCssClass="Icon Hover"></ajaxToolkit:HoverMenuExtender>
                                    </ItemTemplate>
                                </asp:DataList>

                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                <asp:Panel ID="OrgPanel" runat="server" Visible='<%# IsOrgPanelVisible((int)Eval("PackageID")) %>'>
                    <fcp:UserOrganization ID="UserOrganization" runat="server" PackageId='<%# (int)Eval("PackageID") %>' />
                </asp:Panel>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    
    <asp:Panel ID="EmptyPackagesList" runat="server" Visible="false" CssClass="card-body form-horizontal">
        <asp:Literal ID="litEmptyList" runat="server" EnableViewState="false"></asp:Literal>
    </asp:Panel>
</asp:Panel>



<asp:UpdatePanel runat="server" ID="ResellerPackagesPanel" Visible="false">
    <ContentTemplate>
<asp:GridView ID="gvPackages" runat="server" AutoGenerateColumns="False"
    EmptyDataText="gvPackages" CssSelectorClass="NormalGridView fcp-home-hosting-spaces-grid"
    AllowSorting="True" DataSourceID="odsPackages">
    <Columns>
        <asp:TemplateField SortExpression="PackageName" HeaderText="gvPackagesName">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
	            <asp:hyperlink id=lnkEdit runat="server" CssClass="Medium" NavigateUrl='<%# GetSpaceHomePageUrl((int)Eval("PackageID")) %>'>
		            <%# PortalAntiXSS.EncodeOld((string) Eval("PackageName")) %>
	            </asp:hyperlink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression="ServerName" HeaderText="gvPackagesServer">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
		         <uc3:ServerDetails ID="serverDetails" runat="server"
		            ServerID='<%# Eval("ServerID") %>'
		            ServerName='<%# Eval("ServerName") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField SortExpression="PurchaseDate" DataField="PurchaseDate" HeaderText="gvPackagesCreationDate" DataFormatString="{0:d}" >
            <ItemStyle Wrap="False" />
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:TemplateField SortExpression="StatusID" HeaderText="gvPackagesStatus">
            <ItemTemplate>
		         <%# PanelFormatter.GetPackageStatusName((int)Eval("StatusID"))%>
            </ItemTemplate>
        </asp:TemplateField>
		<asp:TemplateField ItemStyle-Wrap="false">
			<ItemTemplate><uc4:Comments id="Comments1" runat="server"
				    Comments='<%# Eval("Comments") %>'>
                </uc4:Comments></ItemTemplate>
		</asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsPackages" runat="server" SelectMethod="GetMyPackages"
    TypeName="FuseCP.Portal.PackagesHelper" OnSelected="odsPackages_Selected"></asp:ObjectDataSource>
    </ContentTemplate>
</asp:UpdatePanel>
