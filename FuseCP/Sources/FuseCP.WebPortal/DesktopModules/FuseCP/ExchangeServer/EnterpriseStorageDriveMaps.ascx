<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseStorageDriveMaps.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.EnterpriseStorageDriveMaps" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
<div class="card-header">
    <h3 class="card-title">
        <asp:Image ID="imgESDM" SkinID="EnterpriseStorageDriveMaps48" runat="server" />
        <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Drive Maps"></asp:Localize>
    </h3>
</div>
<div class="FormButtonsBar right">
    <asp:LinkButton id="btnAddDriveMap" CssClass="btn btn-success" runat="server" OnClick="btnAddDriveMap_Click">
        <i class="bi bi-check-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnAddDriveMap"/>
    </asp:LinkButton>
</div>
<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="row">
        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch" CssClass="col-md-5 offset-md-7 text-end d-flex flex-wrap gap-2 align-items-center">
            <asp:Localize ID="locSearch" runat="server" meta:resourcekey="locSearch" Visible="false"></asp:Localize>
            <div class="row mb-3">
                <div class="input-group">
                    <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-control" AutoPostBack="True" onselectedindexchanged="ddlPageSize_SelectedIndexChanged">   
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
<asp:GridView ID="gvDriveMaps" runat="server" AutoGenerateColumns="False" EnableViewState="true"
    EmptyDataText="gvDriveMaps" CssSelectorClass="NormalGridView"
    OnRowCommand="gvDriveMaps_RowCommand" AllowPaging="True" AllowSorting="True"
    DataSourceID="odsEnterpriseDriveMapsPaged" PageSize="20">
    <Columns>
        <asp:TemplateField HeaderText="gvDrive">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Image ID="img1" runat="server" ImageUrl='<%# GetDriveImage() %>' ImageAlign="AbsMiddle" />
                <asp:Literal id="litDrive" runat="server" Text='<%# string.Format("{0}:", Eval("DriveLetter")) %>'></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvLabelAs">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Literal id="litLabelAs" runat="server" Text='<%# Eval("LabelAs") %>'></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvFolderUrl">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Literal id="litFolderUrl" runat="server" Text='<%# (Eval("Folder.UncPath") ?? Eval("Folder.Url")).ToString()  %>'></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton id="imgDelDriveMap" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("Folder.Name") %>' OnClientClick="return confirm('Are you sure you want to delete selected map drive?')">
                    <i class="bi bi-trash"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsEnterpriseDriveMapsPaged" runat="server" EnablePaging="True"
    SelectCountMethod="GetEnterpriseDriveMapsPagedCount"
    SelectMethod="GetEnterpriseDriveMapsPaged"
    SortParameterName="sortColumn"
    TypeName="FuseCP.Portal.EnterpriseStorageHelper">
    <SelectParameters>
        <asp:QueryStringParameter Name="itemId" QueryStringField="ItemID" DefaultValue="0" />
        <asp:ControlParameter Name="filterValue" ControlID="txtSearchValue" PropertyName="Text" />
    </SelectParameters>
</asp:ObjectDataSource>
