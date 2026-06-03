<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Organizations.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.Organizations" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/Quota.ascx" TagName="Quota" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-header">
    <h3 class="card-title">
        <asp:Image ID="Image1" SkinID="Organization48" runat="server" />
        <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Organizations"></asp:Localize>
    </h3>
</div>
<div class="FormButtonsBar right">
    <asp:LinkButton id="btnCreate" CssClass="btn btn-primary" runat="server" OnClick="btnCreate_Click">
        <i class="bi bi-plus-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCreateText"/>
    </asp:LinkButton>
</div>
<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="text-end fcp-mb-4">
        <asp:CheckBox ID="chkRecursive" runat="server" Text="Show Reseller Organizations" meta:resourcekey="chkRecursive" AutoPostBack="true" CssClass="Normal" />
    </div>
    <div class="row">
        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch" CssClass="col-md-7 offset-md-5 text-end d-flex flex-wrap gap-2 align-items-center exchange-search-inline">
            <div class="mb-0">
                <div class="input-group">
                    <asp:DropDownList ID="ddlSearchColumn" runat="server" CssClass="form-control align-middle" >
                        <asp:ListItem Value="ItemName" meta:resourcekey="ddlSearchColumnItemName">OrganizationName</asp:ListItem>
                        <asp:ListItem Value="Username" meta:resourcekey="ddlSearchColumnUsername">OwnerUsername</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="mb-0">
                <div class="input-group">
                    <asp:TextBox ID="txtSearchValue" runat="server" CssClass="form-control"></asp:TextBox>
                    <span class="input-group-btn">
                        <asp:LinkButton ID="cmdSearch" runat="server" CausesValidation="false" CssClass="btn btn-primary">
                            <i class="bi bi-search" aria-hidden="true"></i>
                        </asp:LinkButton>
                    </span>
                </div>
            </div>
        </asp:Panel>
    </div>
</div>
<asp:GridView ID="gvOrgs" runat="server" AutoGenerateColumns="False" DataSourceID="odsOrgsPaged" meta:resourcekey="gvOrgs" CssSelectorClass="NormalGridView" OnRowCommand="gvOrgs_RowCommand" AllowPaging="True" AllowSorting="True" EnableViewState="false">
    <Columns>
        <asp:BoundField meta:resourcekey="gvOrgsID" DataField="OrganizationID" />
        <asp:TemplateField meta:resourcekey="gvOrgsName" SortExpression="ItemName">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <div class="fcp-p-7">
                    <asp:hyperlink id="lnk1" runat="server" EnableViewState="false" CssClass="NormalBold" NavigateUrl='<%# GetOrganizationEditUrl(Eval("ItemID").ToString()) %>'>
                        <%# Eval("ItemName") %>
                    </asp:hyperlink>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression="PackageName" meta:resourcekey="gvOrgsSpace">
            <ItemStyle Wrap="False"></ItemStyle>
            <ItemTemplate>
                <asp:hyperlink id="lnk4" runat="server" NavigateUrl='<%# GetSpaceHomePageUrl((int)Eval("PackageID")) %>'>
                    <%# Eval("PackageName") %>
                </asp:hyperlink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression="Username" meta:resourcekey="gvOrgsUser">
            <ItemStyle Wrap="False"></ItemStyle>
                <ItemTemplate>
                    <asp:hyperlink id="lnk3" runat="server" NavigateUrl='<%# GetUserHomePageUrl((int)Eval("UserID")) %>'>
                        <%# Eval("Username") %>
                    </asp:hyperlink>
                </ItemTemplate>
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField meta:resourcekey="gvOrgsDefault">
            <ItemTemplate>
                <div class="text-center">
                    <input type="radio" name="DefaultOrganization" value='<%# Eval("ItemID") %>' <%# IsChecked(Convert.ToString(Eval("IsDefault")), Eval("ItemID").ToString()) %>/>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("ItemID") %>' OnClientClick="return confirm('Remove this item?');">
                    <i class="bi bi-trash"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<div class="card-body">
    <asp:ObjectDataSource ID="odsOrgsPaged" runat="server" EnablePaging="True" SelectCountMethod="GetOrganizationsPagedCount" SelectMethod="GetOrganizationsPaged" SortParameterName="sortColumn" TypeName="FuseCP.Portal.OrganizationsHelper" OnSelected="odsOrgsPaged_Selected">
        <SelectParameters>
            <asp:QueryStringParameter Name="packageId" QueryStringField="SpaceID" DefaultValue="-1" />
            <asp:ControlParameter Name="recursive" ControlID="chkRecursive" PropertyName="Checked" DefaultValue="False" />
            <asp:ControlParameter Name="filterColumn" ControlID="ddlSearchColumn" PropertyName="SelectedValue" />
            <asp:ControlParameter Name="filterValue" ControlID="txtSearchValue" PropertyName="Text" />
        </SelectParameters>
    </asp:ObjectDataSource>
</div>
<div class="card-footer">
    <div class="row">
        <div class="col-md-6">
            <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota" Text="Total Organizations Created:"></asp:Localize>
            &nbsp;&nbsp;&nbsp;
            <fcp:Quota ID="orgsQuota" runat="server" QuotaName="HostedSolution.Organizations" />
        </div>
        <div class="col-md-6 text-end">
            <asp:LinkButton id="btnSetDefaultOrganization" CssClass="btn btn-success" runat="server" OnClick="btnSetDefaultOrganization_Click">
                <i class="bi bi-check-lg me-1"></i>
                <asp:Localize runat="server" meta:resourcekey="btnSetDefaultOrganizationText"/>
            </asp:LinkButton>
            &nbsp;
        </div>
    </div>
</div>
