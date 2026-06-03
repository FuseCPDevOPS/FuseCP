<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginForgotPassword.ascx.cs" Inherits="FuseCP.Portal.LoginForgotPassword" %>
<div class="card-body form-horizontal fcp-forgot-password">
	<div class="row g-2 align-items-center mb-3">
		<asp:Label ID="lblUsername" runat="server" meta:resourcekey="lblUsername" Text="User name:" CssClass="col-sm-3 col-form-label mb-0"></asp:Label>
		<div class="col-sm-9">
			<asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
			<asp:RequiredFieldValidator ID="usernameValidator" runat="server" ControlToValidate="txtUsername" ErrorMessage="*"
				Display="Dynamic" SetFocusOnError="true" CssClass="text-danger fw-semibold ms-1"></asp:RequiredFieldValidator>
		</div>
	</div>
</div>
<div class="card-footer d-flex justify-content-between align-items-center flex-wrap gap-2">
	<asp:LinkButton ID="cmdBack" runat="server" CssClass="btn btn-secondary" CausesValidation="False" OnClick="cmdBack_Click"><i class="bi bi-arrow-left" aria-hidden="true"></i>&nbsp;<asp:Localize runat="server" meta:resourcekey="cmdBack" /> </asp:LinkButton>
	<asp:LinkButton ID="btnSend" CssClass="btn btn-success" runat="server" OnClick="btnSend_Click"><i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSendtext" /> </asp:LinkButton>
</div>
