<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupsList.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.UserControls.GroupsList" %>
<%@ Register Src="../../UserControls/PopupHeader.ascx" TagName="PopupHeader" TagPrefix="fcp" %>

<asp:UpdatePanel ID="GroupsUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
    
	<div class="FormButtonsBarClean">
		<asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" OnClick="btnDelete_Click"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
        <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>
	</div>
	<asp:GridView ID="gvGroups" runat="server" meta:resourcekey="gvAccounts" AutoGenerateColumns="False"
		Width="600px" CssSelectorClass="NormalGridView"
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
			<asp:TemplateField meta:resourcekey="gvGroupsDisplayName" HeaderText="gvGroupsDisplayName">
				<HeaderStyle Wrap="false" />
				<ItemStyle Wrap="false"></ItemStyle>
				<ItemTemplate>
					<asp:Image ID="img1" runat="server" ImageUrl='<%# GetAccountImage((int)Eval("AccountType")) %>' ImageAlign="AbsMiddle" />
					<asp:Literal ID="litDisplayName" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Literal>
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>


<asp:Panel ID="AddGroupsPanel" runat="server" style="display:none">
	<div class="widget">
             <div class="widget-header clearfix">
                           <h3><i class="bi bi-person"></i>  <asp:Localize ID="headerAddGroups" runat="server" meta:resourcekey="headerAddGroups"></asp:Localize></h3>
			</div>
                    <div class="widget-content Popup">
    <asp:UpdatePanel ID="AddGroupsUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="FormButtonsBarClean">
                <div class="FormButtonsBarCleanRight">
                    <asp:Panel ID="SearchPanel" runat="server" DefaultButton="cmdSearch">
                         <div class="d-flex flex-wrap gap-2 align-items-center">
                                            <div class="input-group">
                        <asp:DropDownList ID="ddlSearchColumn" runat="server" CssClass="form-control">
                            <asp:ListItem Value="DisplayName" meta:resourcekey="ddlSearchColumnDisplayName">DisplayName</asp:ListItem>
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
				<asp:GridView ID="gvPopupGroups" runat="server" meta:resourcekey="gvPopupGroups" AutoGenerateColumns="False"
				 CssSelectorClass="NormalGridView" CssClass="fcp-responsive-grid"
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
						<asp:TemplateField meta:resourcekey="gvGroupsDisplayName">
							<ItemStyle></ItemStyle>
							<ItemTemplate>
								<asp:Image ID="img1" runat="server" ImageUrl='<%# GetAccountImage((int)Eval("AccountType")) %>' ImageAlign="AbsMiddle" />
								<asp:Literal ID="litDisplayName" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Literal>
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
			</div>
	    </ContentTemplate>
    </asp:UpdatePanel>
			<br /><br />
			<asp:LinkButton id="btnCancelAdd" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnAddSelected" CssClass="btn btn-success" runat="server" OnClick="btnAddSelected_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddSelectedText"/> </asp:LinkButton>
		</div>
	</div>
</asp:Panel>

<asp:Button ID="btnAddAccountsFake" runat="server" style="display:none" />
<ajaxToolkit:ModalPopupExtender ID="AddGroupsModal" runat="server"
	TargetControlID="btnAddAccountsFake" PopupControlID="AddGroupsPanel"
	BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelAdd" />

	</ContentTemplate>
</asp:UpdatePanel>
