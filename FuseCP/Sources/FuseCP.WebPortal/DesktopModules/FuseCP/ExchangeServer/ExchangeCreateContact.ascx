<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeCreateContact.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeCreateContact" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
    <div class="card-header">
        <h3 class="card-title">
            <asp:Image ID="Image1" SkinID="ExchangeContactAdd48" runat="server" />
            <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Create Contact"></asp:Localize>
        </h3>
    </div>
    <div class="card-body form-horizontal">
        <fcp:SimpleMessageBox id="messageBox" runat="server" />
        <div class="row mb-3">
            <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="txtDisplayName">
                <asp:Localize ID="locDisplayName" runat="server" meta:resourcekey="locDisplayName" Text="Display Name:"></asp:Localize>
            </asp:Label>
            <div class="col-sm-10">
                <div class="input-group">
                    <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireDisplayName" runat="server" meta:resourcekey="valRequireDisplayName" ControlToValidate="txtDisplayName" ErrorMessage="Enter Display Name" ValidationGroup="CreateContact" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>
        <div class="row mb-3">
            <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="txtEmail">
                <asp:Localize ID="locEmail" runat="server" meta:resourcekey="locEmail" Text="E-mail Address:"></asp:Localize>
            </asp:Label>
            <div class="col-sm-10">
                <div class="input-group">
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireAccount" runat="server" meta:resourcekey="valRequireAccount" ControlToValidate="txtEmail" ErrorMessage="Enter E-mail address" ValidationGroup="CreateContact" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="valCorrectEmail" runat="server" ErrorMessage="Enter correct e-mail address" ControlToValidate="txtEmail" Display="Dynamic" meta:resourcekey="valCorrectEmail" ValidationExpression="^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,25}$" ValidationGroup="CreateContact">*</asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
    </div>
    <div class="card-footer text-end">
        <asp:LinkButton id="btnCreate" CssClass="btn btn-success" runat="server" OnClick="btnCreate_Click" ValidationGroup="CreateContact">
            <i class="bi bi-person-plus me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnCreateText"/>
        </asp:LinkButton>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="CreateContact" />
    </div>
