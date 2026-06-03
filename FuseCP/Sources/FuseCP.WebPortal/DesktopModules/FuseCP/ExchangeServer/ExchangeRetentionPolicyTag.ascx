<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeRetentionPolicyTag.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeRetentionPolicyTag" %>
<%@ Register Src="UserControls/SizeBox.ascx" TagName="SizeBox" TagPrefix="fcp" %><%@ Register Src="UserControls/DaysBox.ascx" TagName="DaysBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaEditor.ascx" TagName="QuotaEditor" TagPrefix="uc1" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>

<%@ Import Namespace="FuseCP.Portal" %>

<div class="card-header">
    <h3 class="card-title">
        <asp:Image ID="Image1" SkinID="ExchangeRetentionPolicyTag48" runat="server" />
        <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Retention policy tag"></asp:Localize>
    </h3>
</div>
<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <asp:GridView id="gvPolicy" runat="server"  EnableViewState="true" AutoGenerateColumns="false" EmptyDataText="gvPolicy" CssSelectorClass="NormalGridView" OnRowCommand="gvPolicy_RowCommand" >
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Image ID="imgType" runat="server" Width="16px" Height="16px" ImageUrl='<%# GetTagType((int)Eval("ItemID")) %>' ImageAlign="AbsMiddle" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Tag">
                <ItemStyle></ItemStyle>
                <ItemTemplate>
                    <asp:LinkButton ID="linkcmdEdit" runat="server" CommandName="EditItem" AlternateText="Edit record" CommandArgument='<%# Eval("TagId") %>' Enabled='<%# ((int)Eval("ItemID") == PanelRequest.ItemID) %>' >
                        <asp:Label id="lnkDisplayPolicy" runat="server" EnableViewState="true" ><%# PortalAntiXSS.Encode((string)Eval("TagName"))%></asp:Label>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="cmdEdit" runat="server" SkinID="EditSmall" CommandName="EditItem" CommandArgument='<%# Eval("TagId") %>' Visible='<%# ((int)Eval("ItemID") == PanelRequest.ItemID) %>' ></asp:ImageButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton id="imgDelPolicy" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("TagId") %>' OnClientClick="return confirm('Are you sure you want to delete selected policy tag?')" Visible='<%# ((int)Eval("ItemID") == PanelRequest.ItemID) %>'>
                        <i class="bi bi-trash"></i>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <fcp:CollapsiblePanel id="secPolicy" runat="server" TargetControlID="Policy" meta:resourcekey="secPolicy" Text="Policy"></fcp:CollapsiblePanel>
    <asp:Panel ID="Policy" runat="server" Height="0" style="overflow:hidden">
        <div class="row mb-3">
            <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="valRequirePolicy">
                <asp:Localize ID="lblPolicyTagName" runat="server" meta:resourcekey="lblPolicyTagName" Text="Tag Name:"></asp:Localize>
            </asp:Label>
            <div class="d-flex flex-wrap gap-2 align-items-center">
		        <asp:TextBox ID="txtPolicy" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valRequirePolicy" runat="server" meta:resourcekey="valRequirePolicy" ControlToValidate="txtPolicy"
		            ErrorMessage="Enter policy tag name" ValidationGroup="CreatePolicy" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
            </div>
        </div>
    </asp:Panel>
    <fcp:CollapsiblePanel id="secPolicyFeatures" runat="server" TargetControlID="PolicyFeatures" meta:resourcekey="secPolicyFeatures" Text="Policy Tag Features"></fcp:CollapsiblePanel>
    <asp:Panel ID="PolicyFeatures" runat="server" Height="0" style="overflow:hidden">
        <div class="row mb-3">
            <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="ddTagType">
                <asp:Localize ID="locType" runat="server" meta:resourcekey="locType" Text="Type :"></asp:Localize>
            </asp:Label>
            <div class="d-flex flex-wrap gap-2 align-items-center">
                <asp:DropDownList ID="ddTagType" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
        </div>
        <div class="row mb-3">
            <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="ageLimitForRetention">
                <asp:Localize ID="locAgeLimitForRetention" runat="server" meta:resourcekey="locAgeLimitForRetention" Text="Age limit for retention (Days):"></asp:Localize>
            </asp:Label>
            <div class="d-flex flex-wrap gap-2 align-items-center">
                 <uc1:QuotaEditor id="ageLimitForRetention" runat="server" QuotaTypeID="2" QuotaValue="1" QuotaMinValue="1" QuotaMaxValue="24855" ParentQuotaValue="-1"></uc1:QuotaEditor>
            </div>
        </div>
        <div class="row mb-3">
            <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="ddRetentionAction">
                <asp:Localize ID="locRetentionAction" runat="server" meta:resourcekey="locRetentionAction" Text="Retention action :"></asp:Localize>
            </asp:Label>
            <div class="d-flex flex-wrap gap-2 align-items-center">
                <asp:DropDownList ID="ddRetentionAction" runat="server" CssClass="form-contro"></asp:DropDownList>
            </div>
        </div>
    </asp:Panel>
    <div class="FormButtonsBarClean">
        <asp:LinkButton id="btnAddPolicy" CssClass="btn btn-success" runat="server" OnClick="btnAddPolicy_Click">
            <i class="bi bi-check-lg me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnAddPolicy"/>
        </asp:LinkButton>
        &nbsp;
        <asp:LinkButton id="btnUpdatePolicy" CssClass="btn btn-warning" runat="server" OnClick="btnUpdatePolicy_Click">
            <i class="bi bi-arrow-clockwise me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnUpdatePolicy"/>
        </asp:LinkButton>
        &nbsp;
        <asp:LinkButton id="btnCancelPolicy" CssClass="btn btn-warning" runat="server" OnClick="btnCancelPolicy_Click">
            <i class="bi bi-x-lg me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnCancelPolicy"/>
        </asp:LinkButton>
        &nbsp;
    </div>
    <asp:TextBox ID="txtStatus" runat="server" CssClass="TextBox400" MaxLength="128" ReadOnly="true"></asp:TextBox>
</div>
