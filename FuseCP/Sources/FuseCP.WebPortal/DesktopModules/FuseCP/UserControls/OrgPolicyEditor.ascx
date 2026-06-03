<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrgPolicyEditor.ascx.cs" Inherits="FuseCP.Portal.UserControls.OrgPolicyEditor" %>

<asp:UpdatePanel runat="server" ID="OrgPolicyPanel" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:CheckBox id="chkEnablePolicy" runat="server" meta:resourcekey="chkEnablePolicy" Text="Enable Policy"
            CssClass="NormalBold" AutoPostBack="true" OnCheckedChanged="chkEnablePolicy_CheckedChanged"/>
        <table id="PolicyBlock" runat="server" >
            <tr>
                <td colspan="2" class="fcp-pt-10">
	                <asp:GridView id="gvAdditionalGroups" runat="server"  EnableViewState="true" AutoGenerateColumns="false"
		                meta:resourcekey="gvAdditionalGroups" CssSelectorClass="NormalGridView" OnRowCommand="gvAdditionalGroup_RowCommand" DataKeyNames="GroupId">
		                <Columns>
                            <asp:TemplateField meta:resourcekey="gvAdditionalGroupEdit" HeaderText="gvAdditionalGroupEdit">
                                <ItemTemplate>
                                    <asp:ImageButton ID="cmdEdit" runat="server" SkinID="EditSmall" CommandName="EditItem" AlternateText="Edit record" CommandArgument='<%# Eval("GroupId") %>' ></asp:ImageButton>
                                </ItemTemplate>
                                </asp:TemplateField>
			                <asp:TemplateField meta:resourcekey="gvAdditionalGroup" HeaderText="gvAdditionalGroup">
				                <ItemStyle></ItemStyle>
				                <ItemTemplate>
					                <asp:Literal id="litDisplayAdditionalGroup" runat="server" Text='<%# Eval("GroupName") %>'></asp:Literal>
                                </ItemTemplate>
			                </asp:TemplateField>
			                <asp:TemplateField>
                                <ItemStyle Width="30px" Wrap="false"></ItemStyle>
				                <ItemTemplate>
					                <asp:LinkButton id="imgDelAdditionalGroup" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("GroupId") %>' OnClientClick="return confirm('Are you sure you want to delete selected group?')"> <i class="bi bi-trash"></i> </asp:LinkButton>
				                </ItemTemplate>
			                </asp:TemplateField>
		                </Columns>
	                </asp:GridView>
                </td>
            </tr>
            <tr>
                <td class="Normal fcp-pt-10" >
                    <asp:Label ID="lblAdditionalGroupName" runat="server" meta:resourcekey="lblAdditionalGroupName" Text="Display Name:"/>
                </td>
                <td class="Normal fcp-pt-10" >
                    <asp:TextBox ID="txtAdditionalGroup" runat="server" CssClass="form-control"/>
                    <asp:RequiredFieldValidator ID="valRequireAdditionalGroup" runat="server" meta:resourcekey="valRequireAdditionalGroup" ControlToValidate="txtAdditionalGroup"
					    ErrorMessage="Enter Display Name" Display="Dynamic" Text="*" ValidationGroup="SettingsAdditionalGroupEditor" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="valDuplicateAdditionalGroup" runat="server" meta:resourcekey="valDuplicateAdditionalGroup" ControlToValidate="txtAdditionalGroup"
                        OnServerValidate="DuplicateName_Validation" ErrorMessage="Duplicate Display Name" ValidateEmptyText="false" Display="Dynamic" Text="*"
                        ValidationGroup="SettingsAdditionalGroupEditor" SetFocusOnError="True"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="fcp-pt-10">
                    <div class="FormButtonsBarClean">
                        <asp:LinkButton id="btnUpdateAdditionalGroup" CssClass="btn btn-primary" runat="server" OnClick="btnUpdateAdditionalGroup_Click" ValidationGroup="SettingsAdditionalGroupEditor"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdateAdditionalGroupText"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnAddAdditionalGroup" CssClass="btn btn-success" runat="server" OnClick="btnAddAdditionalGroup_Click" ValidationGroup="SettingsAdditionalGroupEditor"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddAdditionalGroupText"/> </asp:LinkButton> 
                    </div>
                </td>
            </tr>
        </table>
	</ContentTemplate>
</asp:UpdatePanel>

