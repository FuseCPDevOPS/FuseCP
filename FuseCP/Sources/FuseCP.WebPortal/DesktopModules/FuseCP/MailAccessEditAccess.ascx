<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailAccessEditAccess.ascx.cs" Inherits="FuseCP.Portal.MailAccessEditAccess" %>

<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
	TagPrefix="fcp" %>


<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />

<div class="card-body form-horizontal">
    <div class="Huge">
        <asp:Literal ID="litDomainName" runat="server"></asp:Literal>
    </div>
    <div class="card-body form-horizontal">
        <asp:PlaceHolder ID="providerControl" runat="server"></asp:PlaceHolder>
    </div>
</div>

<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" OnClientClick="ShowProgressDialog('Updating Domain Settings...');"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdateText"/> </asp:LinkButton>
</div>

