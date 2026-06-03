<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserSelector.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.UserControls.UserSelector" %>

<asp:UpdatePanel ID="MainUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <fieldset>
            <div class="row">
                <div class="col-md-12">
                    <div class="row mb-3">
                        <asp:Label runat="server" CssClass="form-label col-sm-2" AssociatedControlID="txtDisplayName">
                            <asp:Localize ID="DisplayNameLabel" runat="server" meta:resourcekey="DisplayNameLabel" Text="First Name:" />
                        </asp:Label>
                        <div class="col-sm-10">
                            <div class="input-group">
                                <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                <asp:LinkButton ID="UserLookUp" runat="server" CausesValidation="false" OnClick="UserLookUp_Click" meta:resourcekey="UserLookup" CssClass="btn btn-primary fcp-user-lookup-btn" />
                                <asp:LinkButton ID="cmdClear" runat="server" CssClass="btn btn-primary" meta:resourcekey="cmdClear" OnClick="cmdClear_Click" CausesValidation="False"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </fieldset>
        <asp:Panel ID="AddAccountsPanel" runat="server" style="display:none">
            <div class="widget">
                <div class="widget-header clearfix">
                    <h3>
                        <i class="bi bi-person"></i>
                        <asp:Localize ID="headerAddAccounts" runat="server" meta:resourcekey="headerAddAccounts"></asp:Localize>
                    </h3>
                </div>
                <div class="widget-content Popup">
                    <asp:UpdatePanel ID="AddAccountsUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <div class="text-end fcp-mb-4">
			                    <asp:Localize ID="locIncludeSearch" runat="server" Text="Include in search:"></asp:Localize>
                            </div>
                            <div class="FormButtonsBarClean">
                                <div class="FormButtonsBarCleanRight">
                                    <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch">
                                        <div class="d-flex flex-wrap gap-2 align-items-center">
                                            <div class="input-group">
                                        <asp:DropDownList ID="ddlSearchColumn" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="DisplayName" meta:resourcekey="ddlSearchColumnDisplayName">DisplayName</asp:ListItem>
                                            <asp:ListItem Value="PrimaryEmailAddress" meta:resourcekey="ddlSearchColumnEmail">Email</asp:ListItem>
                                        </asp:DropDownList>
                                                </div>
                                                <div class="input-group">
                                        <asp:TextBox ID="txtSearchValue" runat="server" CssClass="form-control"></asp:TextBox>
                                            <span class="input-group-btn">
                                        <asp:LinkButton ID="cmdSearch" Runat="server" meta:resourcekey="cmdSearch" CausesValidation="false" OnClick="cmdSearch_Click" CssClass="btn btn-primary"><i class="bi bi-search" aria-hidden="true"></i></asp:LinkButton>
                                    </span></div></div>
                                     </asp:Panel>
                                </div>
                            </div>
                            <div class="Popup-Scroll">
			                    <asp:GridView ID="gvPopupAccounts" runat="server" meta:resourcekey="gvPopupAccounts" AutoGenerateColumns="False" CssSelectorClass="NormalGridView" CssClass="fcp-responsive-grid" DataKeyNames="AccountName" OnRowCommand="gvPopupAccounts_RowCommand" OnSorting="OnSorting" AllowSorting="true">
				                    <Columns>
					                    <asp:TemplateField meta:resourcekey="gvAccountsDisplayName" SortExpression="DisplayName">
						                    <ItemStyle></ItemStyle>
						                    <ItemTemplate>
							                    <asp:Image ID="img1" runat="server" ImageUrl='<%# GetAccountImage() %>' ImageAlign="AbsMiddle" />
							                    <asp:LinkButton ID="cmdSelectAccount" CommandName="SelectAccount" CommandArgument='<%# Eval("AccountId")%>' runat="server" Text='<%# Eval("DisplayName") %>'></asp:LinkButton>
						                    </ItemTemplate>
					                    </asp:TemplateField>
					                    <asp:TemplateField meta:resourcekey="gvAccountsEmail" >
						                    <ItemStyle></ItemStyle>
						                    <ItemTemplate>
							                    <asp:Literal ID="litPrimaryEmailAddress" runat="server" Text='<%# Eval("PrimaryEmailAddress") %>'></asp:Literal>
						                    </ItemTemplate>
					                    </asp:TemplateField>
				                    </Columns>
			                    </asp:GridView>
		                    </div>
	                    </ContentTemplate>
                    </asp:UpdatePanel>
                    <br /><br />
                    <asp:LinkButton id="btnCancelAdd" CssClass="btn btn-warning" runat="server" CausesValidation="False">
                        <i class="bi bi-x-lg me-1"></i>
                        <asp:Localize runat="server" meta:resourcekey="btnCancelText"/>
                    </asp:LinkButton>
                    &nbsp;
                </div>
            </div>
        </asp:Panel>
        <asp:Button ID="btnSelectAccountFake" runat="server" style="display:none" />
        <ajaxToolkit:ModalPopupExtender ID="SelectAccountsModal" runat="server" TargetControlID="btnSelectAccountFake" PopupControlID="AddAccountsPanel" BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelAdd" />
	</ContentTemplate>
</asp:UpdatePanel>
