<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSCollections.ascx.cs" Inherits="FuseCP.Portal.RDS.RDSCollections" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport ID="asyncTasks" runat="server" />


<div class="card-header">
    <asp:Image ID="imgRDSCollections" SkinID="EnterpriseRDSCollections48" runat="server" />
    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="RDS Collections"></asp:Localize>
</div>
<div class="FormButtonsBar right">
    <asp:LinkButton ID="btnAddCollection" CssClass="btn btn-primary" runat="server" OnClick="btnAddCollection_Click">
        <i class="bi bi-plus-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnAddCollectionText" />
    </asp:LinkButton>
    &nbsp;
    <asp:LinkButton ID="btnImportCollection" CssClass="btn btn-warning" runat="server" OnClick="btnImportCollection_Click">
        <i class="bi bi-download me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnImportCollectionText" />
    </asp:LinkButton>
</div>

<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox ID="messageBox" runat="server" />
    <div class="row fcp-mb-15">
        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch" CssClass="col-md-7 offset-md-5 text-end d-flex flex-wrap gap-2 align-items-center">
            <asp:Localize ID="locSearch" runat="server" meta:resourcekey="locSearch" Visible="false"></asp:Localize>
            <div class="row mb-3">
                <div class="input-group">
                    <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                        <asp:ListItem>10</asp:ListItem>
                        <asp:ListItem Selected="True">20</asp:ListItem>
                        <asp:ListItem>50</asp:ListItem>
                        <asp:ListItem>100</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="row mb-3">
                <div class="input-group">
                    <asp:TextBox ID="txtSearchValue" runat="server" CssClass="form-control"></asp:TextBox>
                    <div class="d-flex">
                        <asp:LinkButton ID="cmdSearch" runat="server" CausesValidation="false" CssClass="btn btn-primary">
                            <i class="bi bi-search" aria-hidden="true"></i>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</div>
<asp:GridView ID="gvRDSCollections" runat="server" AutoGenerateColumns="False" EnableViewState="true"
    EmptyDataText="gvRDSCollections" CssSelectorClass="NormalGridView" OnRowCommand="gvRDSCollections_RowCommand" AllowPaging="True"
    AllowSorting="True" DataSourceID="odsRDSCollectionsPaged" PageSize="20">
    <Columns>
        <asp:TemplateField HeaderText="gvCollectionName" SortExpression="DisplayName">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:LinkButton ID="lnkCollectionName" meta:resourcekey="lnkCollectionName" runat="server" CommandName="EditCollection" CommandArgument='<%# Eval("Id") %>' OnClientClick="ShowProgressDialog('Loading ...');return true;"><%# Eval("DisplayName").ToString() %></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvServer">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Literal ID="litServer" runat="server" Text='<%#GetServerName(Eval("Id").ToString())%>'></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemStyle Width="65px" HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:LinkButton ID="lnkRemove" runat="server" CssClass="btn btn-danger" CommandName="DeleteItem" CommandArgument='<%# Eval("Id") %>' OnClientClick="return confirm('Are you sure you want to remove selected rds collection?')">
                    <i class="bi bi-trash"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<div class="card-footer">
    <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota" Text="Collections Created:"></asp:Localize>&nbsp;	
    <fcp:QuotaViewer ID="collectionsQuota" runat="server" QuotaTypeId="2" DisplayGauge="true" />
</div>
<asp:ObjectDataSource ID="odsRDSCollectionsPaged" runat="server" EnablePaging="True" SelectCountMethod="GetRDSCollectonsPagedCount"
    SelectMethod="GetRDSCollectonsPaged" SortParameterName="sortColumn" TypeName="FuseCP.Portal.RDSHelper">
    <SelectParameters>
        <asp:QueryStringParameter Name="itemId" QueryStringField="ItemID" DefaultValue="0" />
        <asp:ControlParameter Name="filterValue" ControlID="txtSearchValue" PropertyName="Text" />
    </SelectParameters>
</asp:ObjectDataSource>
