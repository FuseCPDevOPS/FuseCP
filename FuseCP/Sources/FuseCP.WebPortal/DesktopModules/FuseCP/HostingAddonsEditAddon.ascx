<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostingAddonsEditAddon.ascx.cs" Inherits="FuseCP.Portal.HostingAddonsEditAddon" %>
<%@ Register Src="HostingPlansQuotas.ascx" TagName="HostingPlansQuotas" TagPrefix="uc1" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>

<asp:Panel ID="HostingAddonsEditPanel" runat="server" DefaultButton="btnSave" >
    <div class="card-body form-horizontal">
        <asp:UpdatePanel runat="server" ID="updatePanelUsers">
            <ContentTemplate>
                <asp:Label ID="lblMessage" runat="server" CssClass="NormalBold" ForeColor="red"></asp:Label>
                <div class="row mb-3">
                    <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="txtPlanName">
                        <asp:Localize ID="lblAddOnName" runat="server" meta:resourcekey="lblAddOnName" Text="Add-On Name:"></asp:Localize>
                    </asp:Label>
                    <div class="col-sm-10">
                        <div class="input-group col-sm-12">
                            <asp:TextBox ID="txtPlanName" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valPlanName" runat="server" ControlToValidate="txtPlanName" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="txtPlanDescription">
                        <asp:Localize ID="lblAddOnDescription" runat="server" meta:resourcekey="lblAddOnDescription" Text="Add-On Description:"></asp:Localize>
                    </asp:Label>
                    <div class="col-sm-10">
                        <div class="input-group col-sm-12">
                                <asp:TextBox ID="txtPlanDescription" runat="server" CssClass="form-control" Rows="4" TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <fcp:CollapsiblePanel id="secQuotas" runat="server" TargetControlID="QuotasPanel" meta:resourcekey="secQuotas" Text="Quotas"></fcp:CollapsiblePanel>
                <asp:Panel ID="QuotasPanel" runat="server" Height="0" style="overflow:hidden;">
                    <uc1:HostingPlansQuotas id="hostingPlansQuotas" runat="server" IsPlan="false"></uc1:HostingPlansQuotas>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="card-footer text-end">
        <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click">
            <i class="bi bi-trash me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnDeleteText"/>
        </asp:LinkButton>
        &nbsp;
        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
            <i class="bi bi-x-lg me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnCancelText"/>
        </asp:LinkButton>
        &nbsp;
        <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click">
            <i class="bi bi-floppy me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnSaveText"/>
        </asp:LinkButton>
    </div>
</asp:Panel>
