<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganizationCreateSecurityGroup.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.OrganizationCreateSecurityGroup" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EmailAddress.ascx" TagName="EmailAddress" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
    <div class="card-header">
        <h3 class="card-title">
            <asp:Image ID="Image1" SkinID="OrganizationUser48" runat="server" />
            <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Create Group"></asp:Localize>
        </h3>
    </div>
    <div class="card-body form-horizontal">
        <fcp:SimpleMessageBox id="messageBox" runat="server" />
        <div class="row mb-3">
            <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="txtDisplayName">
                <asp:Localize ID="locDisplayName" runat="server" meta:resourcekey="locDisplayName" Text="Display Name:"></asp:Localize>
            </asp:Label>
            <div class="col-sm-9">
                <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valRequireDisplayName" runat="server" meta:resourcekey="valRequireDisplayName" ControlToValidate="txtDisplayName" ErrorMessage="Enter Display Name" ValidationGroup="CreateGroup" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div class="card-footer text-end">
        <asp:LinkButton id="btnCreate" CssClass="btn btn-success" runat="server" OnClick="btnCreate_Click" ValidationGroup="CreateGroup">
            <i class="bi bi-plus-lg me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnCreateText"/>
        </asp:LinkButton>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="CreateGroup" />
    </div>
