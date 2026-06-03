<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSServersAddserver.ascx.cs" Inherits="FuseCP.Portal.RDSServersAddserver" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script type="text/javascript" src="/JavaScript/jquery-1.4.4.min.js"></script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="row mb-3">
        <asp:Label runat="server" CssClass="form-label col-sm-4" AssociatedControlID="txtServerName">
            <asp:Localize ID="locServerName" runat="server" meta:resourcekey="locServerName" Text="Server Fully Qualified Domain Name:"></asp:Localize>
        </asp:Label>
        <div class="col-sm-8">
            <asp:TextBox ID="txtServerName" runat="server" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valServerName" runat="server" ErrorMessage="*" ControlToValidate="txtServerName"></asp:RequiredFieldValidator>
        </div>
        <asp:Label runat="server" CssClass="form-label col-sm-4" AssociatedControlID="txtServerComments">
            <asp:Localize ID="locServerComments" runat="server" meta:resourcekey="locServerComments" Text="Server Comments:"></asp:Localize>
        </asp:Label>
        <div class="col-sm-8">
            <asp:TextBox ID="txtServerComments" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <asp:Label runat="server" CssClass="form-label col-sm-4" AssociatedControlID="ddlRdsController">
            <asp:Localize ID="locRDSController" runat="server" meta:resourcekey="locRDSController" Text="RDS Controller:"></asp:Localize>
        </asp:Label>
        <div class="col-sm-8">
            <asp:DropDownList ID="ddlRdsController" runat="server" CssClass="form-control" />
        </div>
    </div>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
        <i class="bi bi-x-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCancel"/>
    </asp:LinkButton>
    &nbsp;
	<asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" OnClientClick="ShowProgressDialog('Adding server...');">
        <i class="bi bi-check-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnAddRDSServer"/>
	</asp:LinkButton>
</div>
