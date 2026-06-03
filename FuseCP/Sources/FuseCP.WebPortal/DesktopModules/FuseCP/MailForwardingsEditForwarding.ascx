<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailForwardingsEditForwarding.ascx.cs"
	Inherits="FuseCP.Portal.MailForwardingsEditForwarding" %>
<%@ Register TagPrefix="dnc" TagName="EmailAddress" Src="MailEditAddress.ascx" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
	TagPrefix="fcp" %>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/mail-confirmation.js"></script>
<div class="card-body form-horizontal">
	<dnc:EmailAddress id="emailAddress" runat="server">
	</dnc:EmailAddress>
	<div class="row mb-3">
        <asp:Label ID="lblForwardsToEmail" CssClass="form-label col-sm-2" runat="server" meta:resourcekey="lblForwardsToEmail"
					Text="Forwards to e-mail:"></asp:Label>
        <div class="input-group col-sm-8">
            <asp:TextBox ID="txtForwardTo" runat="server" CssClass="form-control"></asp:TextBox>
				<asp:RequiredFieldValidator ID="valtxtForwardTo" runat="server" ErrorMessage="*" meta:resourcekey="valRequireEmail" 
					ControlToValidate="txtForwardTo" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
	</div>
	<asp:PlaceHolder ID="providerControl" runat="server"></asp:PlaceHolder>
</div>
<div class="card-footer text-end">
	<asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return fuseCpConfirmWithProgress('Are you sure you want to delete this Mail Alias?', 'Deleting Mail Alias...');"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
	<asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
	<asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick="ShowProgressDialog('Saving Mail Alias...');"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>
</div>

