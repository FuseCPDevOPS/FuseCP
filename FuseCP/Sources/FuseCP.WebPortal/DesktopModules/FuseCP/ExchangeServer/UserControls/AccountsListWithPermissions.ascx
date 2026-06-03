<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountsListWithPermissions.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.UserControls.AccountsListWithPermissions" %>
<%@ Register Src="../../UserControls/PopupHeader.ascx" TagName="PopupHeader" TagPrefix="fcp" %>

<asp:UpdatePanel ID="AccountsUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
    
	<div class="FormButtonsBarClean">
        <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" OnClick="btnDelete_Click"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
		<asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>
	</div>
    
	<asp:GridView ID="gvAccounts" runat="server" meta:resourcekey="gvAccounts" AutoGenerateColumns="False"
		Width="600px" CssSelectorClass="NormalGridView" OnRowDataBound="gvAccounts_RowDataBound"
		DataKeyNames="AccountName" >
		<Columns>
			<asp:TemplateField>
				<HeaderTemplate>
					<asp:CheckBox ID="chkSelectAll" runat="server" onclick="javascript:SelectAllCheckboxes(this);" />
				</HeaderTemplate>
				<ItemTemplate>
					<asp:CheckBox ID="chkSelect" runat="server" />
					<asp:Literal ID="litAccountType" runat="server" Visible="false" Text='<%# Eval("AccountType") %>'></asp:Literal>
				</ItemTemplate>
				<ItemStyle Width="10px" />
			</asp:TemplateField>
			<asp:TemplateField meta:resourcekey="gvAccountsDisplayName" HeaderText="gvAccountsDisplayName">
				<HeaderStyle Wrap="false" />
				<ItemStyle Wrap="false"></ItemStyle>
				<ItemTemplate>
					<asp:Image ID="img1" runat="server" ImageUrl='<%# GetAccountImage((int)Eval("AccountType")) %>' ImageAlign="AbsMiddle" />
					<asp:Literal ID="litDisplayName" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Literal>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField meta:resourcekey="gvAccountsEmail" HeaderText="gvAccountsEmail">
				<HeaderStyle Wrap="false" />
				<ItemStyle Wrap="false"></ItemStyle>
				<ItemTemplate>
					<asp:Literal ID="litPrimaryEmailAddress" runat="server" Text='<%# Eval("PrimaryEmailAddress") %>'></asp:Literal>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField meta:resourcekey="gvAccountsPermissions" HeaderText="Folder Permission">
				<HeaderStyle Wrap="false" />
				<ItemStyle Wrap="false"></ItemStyle>
				<ItemTemplate>					
					<asp:DropDownList ID="ddlPermissions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPermissions_SelectedIndexChanged" >
						<asp:ListItem Text="None" Value="None"></asp:ListItem>
                        <asp:ListItem Text="Owner" Value="Owner"></asp:ListItem>
						<asp:ListItem Text="Publishing Editor" Value="PublishingEditor"></asp:ListItem>
						<asp:ListItem Text="Editor" Value="Editor"></asp:ListItem>
						<asp:ListItem Text="Publishing Author" Value="PublishingAuthor"></asp:ListItem>
						<asp:ListItem Text="Author" Value="Author" Selected="True"></asp:ListItem>
						<asp:ListItem Text="Reviewer" Value="Reviewer"></asp:ListItem>
						<asp:ListItem Text="Contributor" Value="Contributor"></asp:ListItem>
					</asp:DropDownList>
                    <asp:HiddenField runat="server" ID="PermissionLabel" />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>


<asp:Panel ID="AddAccountsPanel" runat="server" style="display:none">
	<div class="widget">
             <div class="widget-header clearfix">
				 <h3><i class="bi bi-person"></i>  <asp:Localize ID="headerAddAccounts" runat="server" meta:resourcekey="headerAddAccounts"></asp:Localize></h3>
			</div>
                    <div class="widget-content Popup">
<asp:UpdatePanel ID="AddAccountsUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
	</ContentTemplate>
</asp:UpdatePanel>
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
                    <asp:CheckBox ID="chkIncludeSharedMailbox" runat="server" Text="Shared Mailbox" Checked="true"
							meta:resourcekey="chkIncludeSharedMailbox" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />
							
					<asp:CheckBox ID="chkIncludeContacts" runat="server" Text="Contacts" Checked="true"
							meta:resourcekey="chkIncludeContacts" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />
					<asp:CheckBox ID="chkIncludeLists" runat="server" Text="Distribution Lists" Checked="true"
							meta:resourcekey="chkIncludeLists" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />
                    <asp:CheckBox ID="chkIncludeGroups" runat="server" Text="Groups" Checked="true"
							meta:resourcekey="chkIncludeGroups" AutoPostBack="true" CssClass="col-12 col-sm-6" OnCheckedChanged="chkIncludeMailboxes_CheckedChanged" />

                                        </div>
                                  </div>
							</div>
						</div>
					</div>
				</span>
				<span class="input-group-btn">
					   <asp:LinkButton ID="cmdSearch" Runat="server" meta:resourcekey="cmdSearch" CausesValidation="false" OnClick="cmdSearch_Click" CssClass="btn btn-primary"><i class="bi bi-search" aria-hidden="true"></i></asp:LinkButton>
				</span></div>
                        </asp:Panel>
                    </div>
                </div></div>
                <div class="Popup-Scroll">
					<asp:GridView ID="gvPopupAccounts" runat="server" meta:resourcekey="gvPopupAccounts" AutoGenerateColumns="False"
					 CssSelectorClass="NormalGridView"
						DataKeyNames="AccountName">
						<Columns>
							<asp:TemplateField>
								<HeaderTemplate>
									<asp:CheckBox ID="chkSelectAll" runat="server" onclick="javascript:SelectAllCheckboxes(this);" />
								</HeaderTemplate>
								<ItemTemplate>
									<asp:CheckBox ID="chkSelect" runat="server" />
									<asp:Literal ID="litAccountType" runat="server" Visible="false" Text='<%# Eval("AccountType") %>'></asp:Literal>
								</ItemTemplate>
								<ItemStyle Width="10px" />
							</asp:TemplateField>
							<asp:TemplateField meta:resourcekey="gvAccountsDisplayName">
								<ItemStyle></ItemStyle>
								<ItemTemplate>
									<asp:Image ID="img1" runat="server" ImageUrl='<%# GetAccountImage((int)Eval("AccountType")) %>' ImageAlign="AbsMiddle" />
									<asp:Literal ID="litDisplayName" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Literal>
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
			<br /><br />
			<asp:LinkButton id="btnCancelAdd" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnAddSelected" CssClass="btn btn-success" runat="server" OnClick="btnAddSelected_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddSelectedText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<asp:Button ID="btnAddAccountsFake" runat="server" style="display:none" />
<ajaxToolkit:ModalPopupExtender ID="AddAccountsModal" runat="server"
	TargetControlID="btnAddAccountsFake" PopupControlID="AddAccountsPanel"
	BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelAdd" />

	</ContentTemplate>
</asp:UpdatePanel>
