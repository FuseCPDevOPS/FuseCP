<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatisticsEditStatistics.ascx.cs" Inherits="FuseCP.Portal.StatisticsEditStatistics" %>
<div class="card-body form-horizontal">
    <div class="row" id="newRow" runat="server">
        <div class="mb-3 col-sm-8">
            <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="lblDomainName">
                <asp:Localize ID="lblWebSite" runat="server" meta:resourcekey="lblWebSite" Text="Web Site:"></asp:Localize>
            </asp:Label>
            <div class="d-flex flex-wrap gap-2 align-items-center">
                <asp:Label ID="lblDomainName" runat="server" CssClass="h3"></asp:Label>
                <asp:DropDownList ID="ddlWebSites" runat="server" CssClass="form-control" DataTextField="Name" DataValueField="Name"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="valRequireWebSite" runat="server" ErrorMessage="*" ControlToValidate="ddlWebSites"></asp:RequiredFieldValidator></td>
			</div>
        </div>
    </div>
    <asp:PlaceHolder ID="providerControl" runat="server"></asp:PlaceHolder>
</div>

<div class="card-footer text-end">
    <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return confirm('Delete Site?');">
        <i class="bi bi-trash me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnDeleteText"/>
    </asp:LinkButton>
    &nbsp;
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
        <i class="bi bi-x-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCancel"/>
    </asp:LinkButton>
    &nbsp;
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click">
        <i class="bi bi-arrow-clockwise me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnUpdate"/>
    </asp:LinkButton>
</div>
