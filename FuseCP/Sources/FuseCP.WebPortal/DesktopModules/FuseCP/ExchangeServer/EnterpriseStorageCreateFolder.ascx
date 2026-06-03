<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseStorageCreateFolder.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.EnterpriseStorageCreateFolder" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EmailAddress.ascx" TagName="EmailAddress" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />
<div class="card-header">
    <h3 class="card-title">
        <asp:Image ID="imgESS" SkinID="EnterpriseStorageSpace48" runat="server" />
        <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Create New Folder"></asp:Localize>
    </h3>
</div>
<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="row">
        <div class="col d-flex flex-wrap gap-2 align-items-center">
            <label class="col-sm-2 form-label">
                <asp:Localize ID="locFolderName" runat="server" meta:resourcekey="locFolderName" Text="Folder Name: *"></asp:Localize>
            </label>
            <div class="row mb-3">
                <div class="input-group">
                    <asp:TextBox ID="txtFolderName" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireFolderName" runat="server" 
                        meta:resourcekey="valRequireFolderName" ControlToValidate="txtFolderName"
                        ErrorMessage="Enter Folder Name" ValidationGroup="CreateFolder" 
                        Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col d-flex flex-wrap gap-2 align-items-center">
            <label class="col-sm-2 form-label">
                <asp:Localize ID="locFolderSize" runat="server" meta:resourcekey="locFolderSize" Text="Folder Limit Size (Gb):"></asp:Localize>
            </label>
            <div class="row mb-3">
                <div class="input-group">
                    <asp:TextBox ID="txtFolderSize" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireFolderSize" runat="server" meta:resourcekey="valRequireFolderSize"
                        ControlToValidate="txtFolderSize" ErrorMessage="Enter Folder Size" ValidationGroup="CreateFolder" 
                        Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <%--
                    01.09.2015 roland.breitschaft@x-company.de 
                    Problem: Portal will raise an Error for the Range-Validator. It could not convert the double-Value
                    Fix: Set the minimum Value to 0                                            
                    --%>
                    <%--<asp:RangeValidator ID="rangeFolderSize" runat="server" ControlToValidate="txtFolderSize" MaximumValue="99999999" MinimumValue="0.01" Type="Double"
                    ValidationGroup="CreateFolder" Display="Dynamic" Text="*" SetFocusOnError="True"
                    ErrorMessage="The quota you've entered exceeds the available quota for organization" />--%>
                    <asp:RangeValidator ID="rangeFolderSize" runat="server" ControlToValidate="txtFolderSize"
                        MaximumValue="99999999" MinimumValue="0" Type="Double" ValidationGroup="CreateFolder"
                        Display="Dynamic" Text="*" SetFocusOnError="True"
                        ErrorMessage="The quota you've entered exceeds the available quota for organization" />
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col d-flex flex-wrap gap-2 align-items-center">
            <label class="col-sm-2 form-label">
                <asp:Localize ID="locQuotaType" runat="server" meta:resourcekey="locQuotaType" Text="Quota Type:"></asp:Localize>
            </label>
            <div class="row mb-3">
                <div class="input-group fcp-pt-6">
                    <asp:RadioButton ID="rbtnQuotaSoft" runat="server" meta:resourcekey="rbtnQuotaSoft" Text="Soft" GroupName="QuotaType" />
                    <asp:RadioButton ID="rbtnQuotaHard" runat="server" meta:resourcekey="rbtnQuotaHard" Text="Hard" GroupName="QuotaType" Checked="true" />
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col d-flex flex-wrap gap-2 align-items-center">
            <label class="col-sm-2 form-label">
                <asp:Localize ID="locAddDefaultGroup" runat="server" meta:resourcekey="locAddDefaultGroup" Text="Add Default Group:"></asp:Localize>
            </label>
            <div class="row mb-3">
                <div class="input-group fcp-pt-6">
                    <asp:CheckBox ID="chkAddDefaultGroup" runat="server" Checked="false"></asp:CheckBox>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCreate" CssClass="btn btn-success" runat="server" OnClick="btnCreate_Click" ValidationGroup="CreateFolder">
        <i class="bi bi-check-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCreateText"/>
    </asp:LinkButton>
    <asp:ValidationSummary ID="valSummary" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="CreateFolder" />
</div>
