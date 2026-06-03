<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RestoreWizard.ascx.cs" Inherits="FuseCP.Portal.RestoreWizard" %>
<%@ Register Src="UserControls/FileLookup.ascx" TagName="FileLookup" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-body form-horizontal">
    <div class="Huge">
        <asp:Literal ID="litRestoreType" runat="server"></asp:Literal>
    </div>
    <br />
    <table class="table table-borderless align-middle mb-0">
        <tr>
            <td class="SubHead" >
                <asp:Label ID="lblBackupLocation" runat="server" meta:resourcekey="lblBackupLocation" Text="Backup Location:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control" resourcekey="ddlLocation" AutoPostBack="True"
                    OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged">
                    <asp:ListItem Value="1">SpaceFolder</asp:ListItem>
                    <asp:ListItem Value="2">ServerFolder</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <asp:Panel ID="SpaceFolderPanel" runat="server">
        <table class="table table-borderless align-middle mb-0">
            <tr>
                <td class="SubHead" >
                    <asp:Label ID="lblSpace" runat="server" meta:resourcekey="lblSpace" Text="Space:"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlSpace" runat="server" CssClass="form-control" AutoPostBack="True"
                        OnSelectedIndexChanged="ddlSpace_SelectedIndexChanged">
                    </asp:DropDownList>&nbsp;
                </td>
            </tr>
            <tr>
                <td class="SubHead">
                    <asp:Label ID="lblSpaceFile" runat="server" meta:resourcekey="lblSpaceFile" Text="File:"></asp:Label>
                </td>
                <td>
                    <fcp:FileLookup id="spaceFile" runat="server" ValidationGroup="Backup" IncludeFiles="true">
                    </fcp:FileLookup>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="ServerFolderPanel" runat="server">
        <table class="table table-borderless align-middle mb-0">
            <tr>
                <td class="SubHead" >
                    <asp:Label ID="lblServerPath" runat="server" meta:resourcekey="lblServerPath" Text="Path:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtServerPath" runat="server" CssClass="form-control" Width="400px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireServerPath" runat="server" Display="Dynamic" ControlToValidate="txtServerPath"
                        ErrorMessage="*" ValidationGroup="Backup" meta:resourcekey="valRequireServerPath"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnRestore" CssClass="btn btn-success" runat="server" OnClick="btnRestore_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnRestoreText"/> </asp:LinkButton>
</div>

