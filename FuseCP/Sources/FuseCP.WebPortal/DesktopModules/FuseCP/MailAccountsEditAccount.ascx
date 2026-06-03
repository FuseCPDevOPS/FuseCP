<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailAccountsEditAccount.ascx.cs"
    Inherits="FuseCP.Portal.MailAccountsEditAccount" %>
<%@ Register Src="UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="uc1" %>
<%@ Register TagPrefix="dnc" TagName="MailEditAddress" Src="MailEditAddress.ascx" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
    TagPrefix="fcp" %>
<fcp:EnableAsyncTasksSupport ID="asyncTasks" runat="server" />
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/mail-confirmation.js"></script>
<div class="card-body form-horizontal">
    <dnc:MailEditAddress ID="mailEditAddress" runat="server"></dnc:MailEditAddress>
    <uc1:PasswordControl ID="passwordControl" runat="server" ValidationGroup="ValidatePassword" />
    <br>
    <div class="mb-3 inline-form">
        <asp:Label runat="server" ID="lblMailboxSizeLimit" meta:resourcekey="lblMailboxSizeLimit" CssClass="form-label col-sm-2">
            <asp:Localize ID="locMailboxSizeLimit" runat="server" meta:resourcekey="lblMailboxSizeLimit" />
        </asp:Label>
        <div class="col-sm-8">
            <asp:Label runat="server" ID="lblMaxMailboxSizeLimit" AssociatedControlID="txtMailBoxSizeLimit" />
            <asp:TextBox runat="server" CssClass="form-control col-sm-4" ID="txtMailBoxSizeLimit" Text="0" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtMailBoxSizeLimit" ErrorMessage="*" Display="Dynamic" />
            <asp:CompareValidator runat="server" ID="CompareValidator1" ControlToValidate="txtMailBoxSizeLimit" Type="Integer" Operator="GreaterThanEqual" Display="Dynamic" ValueToCompare="0" meta:resourcekey="CompareValidator1" />
            <asp:RangeValidator runat="server" ID="MaxMailboxSizeLimitValidator" ControlToValidate="txtMailBoxSizeLimit" Type="Integer" MinimumValue="0" MaximumValue="0" Display="Dynamic" meta:resourcekey="MaxMailboxSizeLimitValidator" />
        </div>
    </div>
    <br />
    <asp:PlaceHolder ID="providerControl" runat="server"></asp:PlaceHolder>
</div>
<div class="card-footer text-end">
    <asp:LinkButton ID="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return fuseCpConfirmWithProgress('Are you sure you want to delete this Mail Account?', 'Deleting Mail Account...');"><i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText" />
    </asp:LinkButton>
    &nbsp;

    <asp:LinkButton ID="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"><i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel" />
    </asp:LinkButton>
    &nbsp;

    <asp:LinkButton ID="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick="ShowProgressDialog('Saving Mail Account...');"><i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText" />
    </asp:LinkButton>
</div>

