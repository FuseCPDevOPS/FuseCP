<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeContacts.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeContacts" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
    <div class="card-header">
        <h3 class="card-title">
            <asp:Image ID="Image1" SkinID="ExchangeContact48" runat="server" />
            <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Contact"></asp:Localize>
        </h3>
    </div>
    <div class="FormButtonsBar right">
        <asp:LinkButton id="btnCreateContact" CssClass="btn btn-primary" runat="server" OnClick="btnCreateContact_Click">
            <i class="bi bi-person-plus me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnCreateContact"/>
        </asp:LinkButton>
    </div>
    <div class="card-body form-horizontal">
        <fcp:SimpleMessageBox id="messageBox" runat="server" />
        <div class="row">
            <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch" CssClass="col-md-7 offset-md-5 text-end d-flex flex-wrap gap-2 align-items-center exchange-search-inline">
                <asp:Localize ID="locSearch" runat="server" meta:resourcekey="locSearch" Visible="false"></asp:Localize>
                <div class="mb-0">
                    <div class="input-group">
                        <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-control" AutoPostBack="True" onselectedindexchanged="ddlPageSize_SelectedIndexChanged">
                            <asp:ListItem>10</asp:ListItem>
                            <asp:ListItem Selected="True">20</asp:ListItem>
                            <asp:ListItem>50</asp:ListItem>
                            <asp:ListItem>100</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="mb-0">
                    <div class="input-group">
                        <asp:DropDownList ID="ddlSearchColumn" runat="server" CssClass="form-control">
                            <asp:ListItem Value="DisplayName" meta:resourcekey="ddlSearchColumnDisplayName">DisplayName</asp:ListItem>
                            <asp:ListItem Value="PrimaryEmailAddress" meta:resourcekey="ddlSearchColumnEmail">Email</asp:ListItem>
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
    <asp:GridView ID="gvContacts" runat="server" AutoGenerateColumns="False" EnableViewState="true"
        EmptyDataText="gvContacts" CssSelectorClass="NormalGridView"
        OnRowCommand="gvContacts_RowCommand" AllowPaging="True" AllowSorting="True"
        DataSourceID="odsAccountsPaged" PageSize="20">
        <Columns>
            <asp:TemplateField HeaderText="gvContactsDisplayName" SortExpression="DisplayName">
                <ItemStyle></ItemStyle>
                <ItemTemplate>
                    <asp:hyperlink id="lnk1" runat="server" NavigateUrl='<%# GetContactEditUrl(Eval("AccountId").ToString()) %>'>
                        <%# Eval("DisplayName") %>
                    </asp:hyperlink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="gvContactsEmail" DataField="PrimaryEmailAddress" SortExpression="PrimaryEmailAddress" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("AccountId") %>' OnClientClick="return confirm('Remove this item?');">
                        <i class="bi bi-trash"></i>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="odsAccountsPaged" runat="server" EnablePaging="True"
        SelectCountMethod="GetExchangeAccountsPagedCount"
        SelectMethod="GetExchangeAccountsPaged"
        SortParameterName="sortColumn"
        TypeName="FuseCP.Portal.ExchangeHelper"
        OnSelected="odsAccountsPaged_Selected">
        <SelectParameters>
            <asp:QueryStringParameter Name="itemId" QueryStringField="ItemID" DefaultValue="0" />
            <asp:Parameter Name="accountTypes" DefaultValue="2" />
            <asp:ControlParameter Name="filterColumn" ControlID="ddlSearchColumn" PropertyName="SelectedValue" />
            <asp:ControlParameter Name="filterValue" ControlID="txtSearchValue" PropertyName="Text" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <div class="card-footer">
        <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota" Text="Total Contacts Created:"></asp:Localize>
        &nbsp;&nbsp;&nbsp;
        <fcp:QuotaViewer ID="contactsQuota" runat="server" QuotaTypeId="2" />
    </div>
