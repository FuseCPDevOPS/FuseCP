<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FtpAccountEditAccount.ascx.cs" Inherits="FuseCP.Portal.FtpAccountEditAccount" %>
<%@ Register Src="UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="uc3" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc4" %>
<%@ Register Src="UserControls/FileLookup.ascx" TagName="FileLookup" TagPrefix="uc2" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/mail-confirmation.js"></script>

<fcp:EnableAsyncTasksSupport ID="asyncTasks" runat="server" />


<div class="card-body form-horizontal">
    <div class="row mb-3">
        <asp:Label ID="lblUserName" runat="server" meta:resourcekey="lblUserName" Text="User name:" CssClass="form-label col-sm-2" AssociatedControlID="usernameControl"></asp:Label>
        <div class="col-sm-8">
            <uc4:UsernameControl ID="usernameControl" runat="server" />
        </div>
    </div>
    <uc3:PasswordControl ID="passwordControl" runat="server" />
    <br>
    <div class="mb-3 inline-form">
        <asp:Label ID="lblHomeFolder" runat="server" meta:resourcekey="lblHomeFolder" Text="Home folder:" CssClass="form-label col-sm-2">
            <asp:Localize ID="locMailboxSizeLimit" runat="server" meta:resourcekey="lblMailboxSizeLimit" />
        </asp:Label>
        <div class="col-sm-8">
            <uc2:FileLookup ID="fileLookup" runat="server" />
        </div>
    </div>
    <br />
    <asp:PlaceHolder ID="providerControl" runat="server"></asp:PlaceHolder>
</div>
<div class="card-footer text-end">
    <asp:LinkButton ID="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return fuseCpConfirmWithProgress('Are you sure you want to delete this FTP Account?', 'Deleting FTP Account...');">
        <i class="bi bi-trash me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnDeleteText" />
    </asp:LinkButton>
    &nbsp;
    <asp:LinkButton ID="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
        <i class="bi bi-x-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCancel" />
    </asp:LinkButton>
    &nbsp;
    <asp:LinkButton ID="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick="ShowProgressDialog('Updating FTP Account...');">
        <i class="bi bi-floppy me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnSaveText" />
    </asp:LinkButton>
</div>
