<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailboxSelector.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.UserControls.MailboxSelector" %>

<asp:UpdatePanel ID="MainUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
            <div class="input-group">
<asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                <span class="d-flex">
                                                <asp:LinkButton ID="ImageButton1" runat="server" CausesValidation="false" OnClick="ImageButton1_Click" meta:resourcekey="UserLookup" CssClass="btn btn-primary" />
                                                    
                                                <asp:LinkButton ID="cmdClear" runat="server" CssClass="btn btn-primary" meta:resourcekey="cmdClear" OnClick="cmdClear_Click" CausesValidation="False"/>
                                            </span>
                                        </div>
<asp:Panel ID="AddAccountsPanel" CssClass="container" runat="server" style="display:none">
	<div class="widget">
             <div class="widget-header clearfix">
                           <h3><i class="bi bi-person"></i> <asp:Localize ID="headerAddAccounts" runat="server" meta:resourcekey="headerAddAccounts"></asp:Localize></h3>
			 </div>
                    <div class="widget-content Popup">
<asp:UpdatePanel ID="AddAccountsUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
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
                    <div class="btn-group" role="group">
                        <div class="dropdown dropdown-lg">
                            <button type="button" class="btn btn-secondary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false"></button>
                            <div class="dropdown-menu dropdown-menu-end" role="menu">
                                  <div class="row mb-3">
                                    <asp:Localize ID="locIncludeSearch" runat="server" Text="Include in search:"></asp:Localize>
                                      <br />
					<asp:CheckBox ID="chkIncludeMailboxes" runat="server" Text="Accounts" Checked="true"
							meta:resourcekey="chkIncludeMailboxes" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />
                    <asp:CheckBox ID="chkIncludeRooms" runat="server" Text="Rooms" Checked="true"
						    meta:resourcekey="chkIncludeRooms" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />
                    <asp:CheckBox ID="chkIncludeEquipment" runat="server" Text="Equipment" Checked="true"
							meta:resourcekey="chkIncludeEquipment" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />
                    <asp:CheckBox ID="chkIncludeSharedMailbox" runat="server" Text="Shared" Checked="true"
							meta:resourcekey="chkIncludeSharedMailbox" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />

					<asp:CheckBox ID="chkIncludeContacts" runat="server" Text="Contacts" Checked="true"
							meta:resourcekey="chkIncludeContacts" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />
					<asp:CheckBox ID="chkIncludeLists" runat="server" Text="Distribution Lists" Checked="true"
							meta:resourcekey="chkIncludeLists" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />
                                        </div>
                                  </div>
                            </div>
                        </div>
                    </div>
                </span>
                <span class="input-group-btn">
                       <asp:LinkButton ID="cmdSearch" Runat="server" meta:resourcekey="cmdSearch" CausesValidation="false" OnClick="cmdSearch_Click" CssClass="btn btn-primary"><i class="bi bi-search" aria-hidden="true"></i></asp:LinkButton>
                </span></div></div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="Popup-Scroll">
					<asp:GridView ID="gvPopupAccounts" runat="server" meta:resourcekey="gvPopupAccounts" AutoGenerateColumns="False"
					 CssSelectorClass="NormalGridView" CssClass="fcp-responsive-grid" AllowSorting="true"
						DataKeyNames="AccountName" OnRowCommand="gvPopupAccounts_RowCommand"  OnSorting="OnSorting">
						<Columns>
							<asp:TemplateField meta:resourcekey="gvAccountsDisplayName" SortExpression="DisplayName">
								<ItemStyle></ItemStyle>
								<ItemTemplate>
									<asp:Image ID="img1" runat="server" ImageUrl='<%# GetAccountImage((int)Eval("AccountType")) %>' ImageAlign="AbsMiddle" />
									<asp:LinkButton ID="cmdSelectAccount" CommandName="SelectAccount"
									CommandArgument='<%# Eval("AccountName").ToString() + "^" + Eval("DisplayName").ToString()+ "^" + Eval("PrimaryEmailAddress")+ "^" + Eval("AccountId")%>'
									runat="server" Text='<%# Eval("DisplayName") %>'></asp:LinkButton>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField meta:resourcekey="gvAccountsEmail">
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
			<asp:LinkButton id="btnCancelAdd" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<asp:Button ID="btnSelectAccountFake" runat="server" style="display:none" />
<ajaxToolkit:ModalPopupExtender ID="SelectAccountsModal" runat="server"
	TargetControlID="btnSelectAccountFake" PopupControlID="AddAccountsPanel"
	BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelAdd" />
	
	</ContentTemplate>
</asp:UpdatePanel>
