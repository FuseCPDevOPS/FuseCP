<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeDisclaimerGeneralSettings.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeDisclaimerGeneralSettings" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/AccountsList.ascx" TagName="AccountsList" TagPrefix="fcp" %>
<%@ Register Src="UserControls/MailboxSelector.ascx" TagName="MailboxSelector" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script src='/tinymce/tinymce.min.js'></script>
<fcp:EnableAsyncTasksSupport ID="asyncTasks" runat="server" />
<script src='/DesktopModules/FuseCP/Scripts/exchange-disclaimer-general-settings.js'></script>
                <div class="card-header">
                    <h3 class="card-title">
                        <asp:Image ID="Image1" SkinID="ExchangeDisclaimers48" runat="server" />
                        <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Distribution List"></asp:Localize>
                        -
                        <asp:Literal ID="litDisplayName" runat="server" Text="" />
                    </h3>
                </div>
                <div class="card-body form-horizontal">
                    <fcp:SimpleMessageBox ID="messageBox" runat="server" />
                    <div class="row mb-3">
                        <asp:Label ID="locDisplayName" meta:resourcekey="locDisplayName" runat="server" Text="Display Name:" CssClass="col-sm-2 col-form-label" AssociatedControlID="txtDisplayName"></asp:Label>
                        <div class="col-sm-10 d-flex flex-wrap gap-2 align-items-center">
                            <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" ValidationGroup="CreateMailbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valRequireDisplayName" runat="server" meta:resourcekey="valRequireDisplayName" ControlToValidate="txtDisplayName"
                                ErrorMessage="Enter Display Name" ValidationGroup="EditList" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
                        </div>
                        <asp:Label ID="locNotes" meta:resourcekey="locNotes" runat="server" Text="Text:" CssClass="col-sm-2 col-form-label" AssociatedControlID="txtNotes"></asp:Label>
                        <div class="col-sm-10 d-flex flex-wrap gap-2 align-items-center">
                            <asp:TextBox ID="txtNotes" runat="server" CssClass="tinymce" Rows="15" cols="20" TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="card-footer text-end">
                    <asp:LinkButton ID="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" ValidationGroup="EditList"><i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText" />
                    </asp:LinkButton>
                    &nbsp;
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="EditList" />
                </div>
