<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseStorageCreateDriveMap.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.EnterpriseStorageCreateDriveMap" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EmailAddress.ascx" TagName="EmailAddress" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script type="text/javascript" src="/JavaScript/jquery-1.4.4.min.js"></script>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/enterprise-storage-drive-map.js"></script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
<div class="card-header">
    <h3 class="card-title">
        <asp:Image ID="imgESDM" SkinID="EnterpriseStorageDriveMaps48" runat="server" />
        <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Create New Drive Map"></asp:Localize>
    </h3>
</div>
<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="row">
        <div class="col d-flex flex-wrap gap-2 align-items-center">
            <label class="col-sm-2 form-label">
                <asp:Localize ID="locDriveLetter" runat="server" meta:resourcekey="locDriveLetter" Text="Select Drive Letter:"></asp:Localize>
            </label>
            <div class="row mb-3">
                <div class="input-group">
                    <asp:DropDownList ID="ddlLetters" runat="server" CssClass="form-control" />
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col d-flex flex-wrap gap-2 align-items-center">
            <label class="col-sm-2 form-label">
                <asp:Localize ID="locFolder" runat="server" meta:resourcekey="locFolder" Text="Storage Folder:"></asp:Localize>
            </label>
            <div class="row mb-3">
                <div class="input-group">
                    <div class="Folders" style="display:inline;">
                        <asp:DropDownList ID="ddlFolders" runat="server" CssClass="form-control" />  
                        <asp:HiddenField id="txtFolderName" runat="server"/>
                    </div>
                    <div class="Url" style="display:inline;">
                        &nbsp;&nbsp;
                        <span class="input-group-text">
                            <i class="bi bi-hdd-stack" aria-hidden="true"></i>
                        </span>
                        <span class="input-group-text" style="background-color:#ffffff;">
                            <asp:Literal ID="lbFolderUrl" runat="server"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col d-flex flex-wrap gap-2 align-items-center">
            <label class="col-sm-2 form-label">
                <asp:Localize ID="locDriveLabel" runat="server" meta:resourcekey="locDriveLabel" Text="Label As:"></asp:Localize>
            </label>
            <div class="row mb-3">
                <div class="input-group">
                    <asp:TextBox ID="txtLabelAs" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCreate" CssClass="btn btn-success" runat="server" OnClick="btnCreate_Click" ValidationGroup="CreateDriveMap">
        <i class="bi bi-check-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCreateText"/>
    </asp:LinkButton>
    <asp:ValidationSummary ID="valSummary" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="CreateDriveMap" />
</div>	
