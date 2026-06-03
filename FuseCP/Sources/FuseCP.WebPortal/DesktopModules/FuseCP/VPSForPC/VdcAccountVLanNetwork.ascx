<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VdcAccountVLanNetwork.ascx.cs"
    Inherits="FuseCP.Portal.VPSForPC.VdcAccountVLanNetwork" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>

        <div class="card">
                <div class="card-header">
                    <asp:Image ID="Image1" SkinID="VLanNetwork" runat="server" />
                    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Available VLans"></asp:Localize>
                </div>
                <div class="card-body form-horizontal">
                    <div class="FormButtonsBar right">
                    <asp:LinkButton id="btnAddVlan" CssClass="btn btn-success" runat="server" OnClick="btnAddVlan_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddVlanText"/> </asp:LinkButton>
                    </div>
                    </div>
            <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="vdc_account_vlan_network" />
            <div class="card tab-content">
                <div class="card-body form-horizontal">
                    <asp:GridView ID="gvVlans" runat="server" AutoGenerateColumns="false" CssSelectorClass="NormalGridView"
                        EmptyDataText="User has no VLANs" OnRowCommand="gvVlans_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="VLanID" HeaderText="VLan" />
                            <asp:BoundField DataField="Comment" HeaderText="Comment" ItemStyle-Wrap="true" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName='DeleteItem' CommandArgument='<%# Eval("VLanID") %>' OnClientClick="return confirm('Remove this item?');"> 
                                        <i class="bi bi-trash"></i> 
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
                </div>
            </div>
            <div class="alert alert-info">
                <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
            </div>
