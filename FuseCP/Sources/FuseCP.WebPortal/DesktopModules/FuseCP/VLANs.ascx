<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VLANs.ascx.cs" Inherits="FuseCP.Portal.VLANs" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>

<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/email-selection.js"></script>

<fcp:SimpleMessageBox ID="messageBox" runat="server" />


<div class="FormButtonsBar right mb-2">
    <asp:LinkButton ID="btnAddItem" runat="server" CssClass="btn btn-primary" OnClick="btnAddItem_Click">
        <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddItem" />
    </asp:LinkButton>
</div>

<div class="card-body form-horizontal fcp-vlans-search-area">
    <div class="row">
        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch" CssClass="col-12 text-end d-flex flex-wrap justify-content-end align-items-center fcp-vlans-search">
            <div class="mb-0 fcp-vlans-filter">
                <asp:DropDownList ID="ddlItemsPerPage" runat="server" CssClass="form-select"
                    AutoPostBack="True" OnSelectedIndexChanged="ddlItemsPerPage_SelectedIndexChanged">
                    <asp:ListItem Value="10">10</asp:ListItem>
                    <asp:ListItem Value="20">20</asp:ListItem>
                    <asp:ListItem Value="50">50</asp:ListItem>
                    <asp:ListItem Value="100">100</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="mb-0 fcp-vlans-filter">
                <asp:DropDownList ID="ddlSearchColumn" runat="server" CssClass="form-select">
                    <asp:ListItem Value="Vlan" meta:resourcekey="liVLAN">VLAN</asp:ListItem>
                    <asp:ListItem Value="ServerName" meta:resourcekey="liServer">Server</asp:ListItem>
                    <asp:ListItem Value="Username" meta:resourcekey="liUsername">Username</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="mb-0 fcp-vlans-search-input-wrap">
                <div class="input-group fcp-vlans-search-input">
                    <asp:TextBox ID="txtSearchValue" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:LinkButton ID="cmdSearch" runat="server" CausesValidation="false" CssClass="btn btn-primary">
                        <i class="bi bi-search" aria-hidden="true"></i>
                    </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>
    </div>
</div>

<asp:GridView ID="gvVLANs" runat="server" AutoGenerateColumns="False"
    AllowSorting="True" EmptyDataText="gvVLANs"
    CssSelectorClass="NormalGridView" CssClass="fcp-vlans-grid" EmptyDataRowStyle-CssClass="fcp-vlans-empty-row" DataKeyNames="VlanID"
    AllowPaging="True" DataSourceID="odsVLANs">
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:CheckBox ID="chkSelectAll" runat="server" onclick="checkAll(this);" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" runat="server" onclick="unCheckSelectAll(this);" Enabled='<%# ((int)Eval("PackageID") == 0) %>' />
            </ItemTemplate>
            <ItemStyle Width="10px" />
        </asp:TemplateField>
        <asp:TemplateField SortExpression="Vlan" HeaderText="gvVLANsVLAN">
            <ItemTemplate>
                <asp:HyperLink NavigateUrl='<%# EditUrl("VlanID", DataBinder.Eval(Container.DataItem, "VlanID").ToString(), "edit_vlan", "ReturnUrl=" + GetReturnUrl()) %>' runat="server" ID="Hyperlink2">
					<%# Eval("Vlan") %>
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="ServerName" SortExpression="ServerName" HeaderText="gvVLANsServer" ItemStyle-Wrap="false"></asp:BoundField>
        <asp:TemplateField HeaderText="hVLANsUser" meta:resourcekey="hVLANsUser" SortExpression="Username">
            <ItemTemplate>
                <%# Eval("UserName") %>&nbsp;
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="hVLANsSpace" meta:resourcekey="hVLANsSpace" SortExpression="PackageName">
            <ItemTemplate>
                <asp:HyperLink ID="lnkSpace" runat="server" NavigateUrl='<%# GetSpaceHomeUrl((int)Eval("PackageID")) %>'>
			        <%# Eval("PackageName") %>
                </asp:HyperLink>&nbsp;
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Comments" HeaderText="gvVLANsComments"></asp:BoundField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsVLANs" runat="server" EnablePaging="True"
    SelectCountMethod="GetVLANsPagedCount"
    SelectMethod="GetVLANsPaged"
    SortParameterName="sortColumn"
    TypeName="FuseCP.Portal.VLANsHelper"
    OnSelected="odsVLANs_Selected">
    <SelectParameters>
        <asp:Parameter Name="serverId" DefaultValue="0" />
        <asp:ControlParameter Name="filterColumn" ControlID="ddlSearchColumn" PropertyName="SelectedValue" />
        <asp:ControlParameter Name="filterValue" ControlID="txtSearchValue" PropertyName="Text" />
    </SelectParameters>
</asp:ObjectDataSource>

<div class="card-footer text-end">
    <asp:Button ID="btnDeleteSelected" runat="server" Text="Delete Selected"
        meta:resourcekey="btnDeleteSelected" CssClass="btn btn-danger"
        CausesValidation="false" OnClick="btnDeleteSelected_Click"></asp:Button>
</div>
