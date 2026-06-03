<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingsEditor.ascx.cs" Inherits="FuseCP.Portal.SettingsEditor" %>
<asp:UpdatePanel runat="server" ID="updatePanelUsers" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate> 

<div class="card-body form-horizontal">
    <div class="row mb-3">
        <div class="col-sm-12 col-md-6 col-lg-4">
            <asp:DropDownList ID="ddlOverride" runat="server" CssClass="form-control"
                resourcekey="ddlOverride" AutoPostBack="true" OnSelectedIndexChanged="ddlOverride_SelectedIndexChanged">
                <asp:ListItem>UseHost</asp:ListItem>
                <asp:ListItem>OverrideHost</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <asp:PlaceHolder ID="settingsPlace" runat="server"></asp:PlaceHolder>
</div>

    </ContentTemplate>
</asp:UpdatePanel>

<div class="card-footer d-flex justify-content-end align-items-center flex-wrap gap-2">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"><i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/></asp:LinkButton>
    <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" ValidationGroup="SettingsEditor"><i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSave"/></asp:LinkButton>
</div>
