<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GlobalSearch.ascx.cs" Inherits="FuseCP.Portal.SkinControls.GlobalSearch" %>

<style>
    .ui-menu-item a {white-space: nowrap; }
</style>

<asp:Panel runat="server" ID="updatePanelUsers" UpdateMode="Conditional" ChildrenAsTriggers="true" CssClass="SearchQuery navbar-form ms-auto fcp-global-search" DefaultButton="ImageButton1">


                    <div class="input-group">
                        <asp:TextBox
                            ID="tbSearch"
                            runat="server"
                            CssClass="form-control"
                            style="vertical-align: middle; z-index: 100;"
                        >
                        </asp:TextBox>
                        <asp:TextBox
                            ID="tbSearchColumnType"
                            runat="server"
                            type="hidden"
                        >
                        </asp:TextBox>
                        <asp:TextBox
                            ID="tbSearchFullType"
                            runat="server"
                            type="hidden"
                        >
                        </asp:TextBox>
                        <asp:TextBox
                            ID="tbSearchText"
                            runat="server"
                            type="hidden"
                        >
                        </asp:TextBox>
                        <asp:TextBox
                            ID="tbObjectId"
                            runat="server"
                            type="hidden"
                        >
                        </asp:TextBox>
                        <asp:TextBox
                            ID="tbPackageId"
                            runat="server"
                            type="hidden"
                        >
                        </asp:TextBox>
                        <asp:TextBox
                            ID="tbAccountId"
                            runat="server"
                            type="hidden"
                        >
                        </asp:TextBox>
                        <asp:HiddenField
                            ID="hfNoResultsText"
                            runat="server"
                            Value="<%$ Resources:NoResults.Text %>"
                        />
                        <asp:HiddenField
                            ID="hfGoToSearchText"
                            runat="server"
                            Value="<%$ Resources:GoToSearch.Text %>"
                        />
                        <div class="d-flex">
                            <asp:LinkButton
                            ID="ImageButton1"
                            runat="server"
                            SkinID="SearchButton"
                            OnClick="btnSearchObject_Click"
                            CausesValidation="false"
                            CssClass="btn fcp-search-trigger"
                        >
                                <i class="bi bi-search" aria-hidden="true"></i>
                            </asp:LinkButton>
                        </div>             
                    </div>
</asp:Panel>
