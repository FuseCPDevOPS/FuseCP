<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VirtualServersAddServer.ascx.cs" Inherits="FuseCP.Portal.VirtualServersAddServer" %>
<div class="card-body form-horizontal">
    <div class="row mb-3">
        <asp:Label ID="lblServerName" runat="server" CssClass="form-label col-sm-2 fw-bold" meta:resourcekey="lblServerName" ></asp:Label>
        <div class="col-sm-10">
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" Text="New Server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valRequireServerName" runat="server" ControlToValidate="txtName" ErrorMessage="*"></asp:RequiredFieldValidator>
        </div>
        <asp:Label ID="lblServerComments" runat="server" CssClass="form-label col-sm-2 fw-bold" meta:resourcekey="lblServerComments" ></asp:Label>
        <div class="col-sm-10">
            <asp:TextBox ID="txtComments" runat="server" CssClass="form-control" Rows="4" TextMode="MultiLine"></asp:TextBox>
        </div>
    </div>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
        <i class="bi bi-x-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCancel"/>
    </asp:LinkButton>
    &nbsp;
    <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click">
        <i class="bi bi-check-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnAdd"/>
    </asp:LinkButton>
    &nbsp;
</div>
