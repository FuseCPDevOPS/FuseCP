<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignedRDSServers.ascx.cs" Inherits="FuseCP.Portal.RDS.AssignedRDSServers" %>
<%@ Import Namespace="FuseCP.Portal" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
<div class="card-header">
    <asp:Image ID="imgRDSServers" SkinID="EnterpriseRDSServers48" runat="server" />
    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Assigned RDS Servers"></asp:Localize>
</div>
<div class="FormButtonsBar right">
    <asp:LinkButton ID="btnAddServerToOrg" runat="server" CssClass="btn btn-primary" OnClick="btnAddServerToOrg_Click">
        <i class="bi bi-plus-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnAddServerToOrg" />
    </asp:LinkButton>
</div>
<div class="card-body">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="row">
        <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch" CssClass="col-md-7 offset-md-5 text-end d-flex flex-wrap gap-2 align-items-center">
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
<asp:GridView ID="gvRDSAssignedServers" runat="server" AutoGenerateColumns="False" EnableViewState="true" EmptyDataText="gvRDSAssignedServers" CssSelectorClass="NormalGridView" OnRowCommand="gvRDSAssignedServers_RowCommand" AllowPaging="True" AllowSorting="True" DataSourceID="odsRDSAssignedServersPaged" PageSize="20">
    <Columns>
        <asp:TemplateField HeaderText="gvRDSServerName" SortExpression="Name">
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:Label id="litRDSServerName" runat="server">
                    <%# Eval("Name") %>
                </asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemStyle></ItemStyle>
            <ItemTemplate>
                <asp:LinkButton ID="EnableLinkButton" CssClass="btn btn-success" runat="server" Visible='<%# Eval("RdsCollectionId") != null && !Convert.ToBoolean(Eval("ConnectionEnabled")) %>' CommandName="EnableItem" CommandArgument='<%# Eval("Id") %>' meta:resourcekey="cmdEnable"></asp:LinkButton>
                <asp:LinkButton ID="DisableLinkButton" CssClass="btn btn-danger" runat="server" Visible='<%# Eval("RdsCollectionId") != null && Convert.ToBoolean(Eval("ConnectionEnabled")) %>' CommandName="DisableItem" CommandArgument='<%# Eval("Id") %>' meta:resourcekey="cmdDisable"></asp:LinkButton>                                    
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="imgRemove1" CssClass="btn btn-sm btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("Id") %>' Visible='<%# Eval("RdsCollectionId") == null %>' OnClientClick="return confirm('Are you sure you want to remove selected server?')"> 
                    <i class="bi bi-trash"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<div class="card-footer">
    <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota" Text="RDS Servers:"></asp:Localize>
    &nbsp;&nbsp;&nbsp;
    <fcp:QuotaViewer ID="rdsServersQuota" runat="server" QuotaTypeId="2" DisplayGauge="true"/>
</div>
<asp:ObjectDataSource ID="odsRDSAssignedServersPaged" runat="server" EnablePaging="True" SelectCountMethod="GetOrganizationRdsServersPagedCount" SelectMethod="GetOrganizationRdsServersPaged" SortParameterName="sortColumn" TypeName="FuseCP.Portal.RDSHelper">
    <SelectParameters>
        <asp:QueryStringParameter Name="itemId" QueryStringField="ItemID" DefaultValue="0" />
        <asp:ControlParameter Name="filterValue" ControlID="txtSearchValue" PropertyName="Text" />
    </SelectParameters>
</asp:ObjectDataSource>
