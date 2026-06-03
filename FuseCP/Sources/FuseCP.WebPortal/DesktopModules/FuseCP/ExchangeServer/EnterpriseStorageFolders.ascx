<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseStorageFolders.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.EnterpriseStorageFolders" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/enterprise-storage-folders.js"></script>

<div class="card-header">
    <h3 class="card-title">
        <asp:Image ID="imgESS" SkinID="EnterpriseStorageSpace48" runat="server" />
        <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Shared Folders"></asp:Localize>
    </h3>
</div>
<div class="FormButtonsBar right">
    <asp:LinkButton id="btnAddFolder" CssClass="btn btn-success" runat="server" OnClick="btnAddFolder_Click">
        <i class="bi bi-folder me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnAddFolderText"/>
    </asp:LinkButton>
</div>
<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="row">
        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch" CssClass="col-md-6 offset-md-6 text-end d-flex flex-wrap gap-2 align-items-center">
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
<asp:HiddenField runat="server" ID="hdnGridState" Value="false" />
<asp:HiddenField runat="server" ID="hdnItemId"  />
<asp:GridView ID="gvFolders" runat="server" AutoGenerateColumns="False" EnableViewState="true"
    EmptyDataText="gvFolders" CssSelectorClass="NormalGridView" OnRowCommand="gvFolders_RowCommand" AllowPaging="True"
    AllowSorting="True" DataSourceID="odsEnterpriseFoldersPaged" PageSize="20">
    <Columns>
        <asp:TemplateField>
            <ItemStyle />
            <ItemTemplate>                        
                <asp:HiddenField ID="hdnFolderName" runat="server" Value='<%# Eval("Name") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvFolderName" SortExpression="FolderName">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:hyperlink id="lnkFolderName" runat="server" NavigateUrl='<%# GetFolderEditUrl(Eval("Name").ToString()) %>'>
                    <%# Eval("Name") %>
                </asp:hyperlink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvFolderQuota" SortExpression="FolderQuota">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Literal id="litFolderQuota" runat="server" Text='<%# ConvertMBytesToGB(Eval("FRSMQuotaMB")).ToString() + " Gb" %>'></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvFolderSize">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Literal id="litFolderSize" runat="server" Text='...'></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvFolderUrl">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Literal id="litFolderUrl" runat="server" Text='<%# (Eval("UncPath") ?? Eval("Url")).ToString() %>'></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvMappedDrive">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Image ID="img1" runat="server" ImageUrl='<%# GetDriveImage() %>' ImageAlign="AbsMiddle"  style="display:none"/>
                <asp:Literal id="litMappedDrive" runat="server" Text='...'></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton id="imgDelFolder" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("Name") %>' OnClientClick="return confirm('Confirming Deletion will result in the deletion of all files on this share.')">
                    <i class="bi bi-trash"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsEnterpriseFoldersPaged" runat="server" EnablePaging="True" SelectCountMethod="GetEnterpriseFoldersPagedCount"
    SelectMethod="GetEnterpriseFoldersPaged" SortParameterName="sortColumn" TypeName="FuseCP.Portal.EnterpriseStorageHelper">
    <SelectParameters>
        <asp:QueryStringParameter Name="itemId" QueryStringField="ItemID" DefaultValue="0" />
        <asp:ControlParameter Name="filterValue" ControlID="txtSearchValue" PropertyName="Text" />
    </SelectParameters>
</asp:ObjectDataSource>
<div class="card-footer">
    <asp:Localize ID="locQuotaFolders" runat="server" meta:resourcekey="locQuotaFolders" Text="Total Folders Allocated:"></asp:Localize>
    &nbsp;&nbsp;&nbsp;
    <fcp:QuotaViewer ID="foldersQuota" runat="server" QuotaTypeId="2" />
    <br />
    <asp:Localize ID="locQuotaSpace" runat="server" meta:resourcekey="locQuotaSpace" Text="Total Space Allocated (Gb):"></asp:Localize>
    &nbsp;&nbsp;&nbsp;
    <fcp:QuotaViewer ID="spaceQuota" runat="server" QuotaTypeId="3" />
    <br />
    <asp:Localize ID="locQuotaAvailableSpace" runat="server" meta:resourcekey="locQuotaAvailableSpace" Text="Used Diskspace (Mb):"></asp:Localize>
    &nbsp;&nbsp;&nbsp;
    <fcp:QuotaViewer ID="spaceAvailableQuota" runat="server" QuotaTypeId="2" />
</div>
