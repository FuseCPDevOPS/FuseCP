<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesAddVirtualDir.ascx.cs" Inherits="FuseCP.Portal.WebSitesAddVirtualDir" %>
<%@ Register Src="UserControls/FileLookup.ascx" TagName="FileLookup" TagPrefix="uc1" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc2" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport ID="asyncTasks" runat="server" />

<div class="card-body form-horizontal">
    <fieldset>
        <div class="row">
            <div class="col-sm-12">
                <div class="row mb-3">
                    <label for="txtSmtpServer" class="col-sm-2 form-label">
                                                <asp:Localize ID="locDirectoryName" runat="server" meta:resourcekey="lblAppDirectoryName" Text="Directory name:" />
                    </label>
                    <div class="col-sm-4">
                        <uc2:UsernameControl ID="virtDirName" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <div class="row mb-3">
                    <label for="txtSmtpPort" class="col-sm-2 form-label">
                                                <asp:Localize ID="locFolder" runat="server" meta:resourcekey="lblFolder" Text="Folder:" />
                    </label>
                    <div class="col-sm-4">
                        <uc1:FileLookup ID="fileLookup" runat="server" ValidationEnabled="true" />
                    </div>
                </div>
            </div>
        </div>
    </fieldset>
</div>
<div class="card-footer text-end">
    <asp:LinkButton ID="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"><i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel" />
    </asp:LinkButton>
    <asp:LinkButton ID="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click"><i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText" />
    </asp:LinkButton>
    &nbsp;
</div>

