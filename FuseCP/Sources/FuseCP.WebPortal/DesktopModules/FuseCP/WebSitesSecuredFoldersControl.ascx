<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesSecuredFoldersControl.ascx.cs" Inherits="FuseCP.Portal.WebSitesSecuredFoldersControl" %>
<%@ Import Namespace="FuseCP.Portal" %>

<div class="FormRow">
    <asp:Button id="btnToggleSecuredFolders" runat="server" meta:resourcekey="btnToggleSecuredFolders"
            Text="Enable Secured Folders" CssClass="btn btn-success" CausesValidation="false"
            OnClick="btnToggleSecuredFolders_Click"/>
</div>

<asp:Panel ID="SecuredFoldersPanel" runat="server">
    <div class="FormButtonsBar">
        <asp:Button id="btnAddFolder" runat="server" meta:resourcekey="btnAddFolder"
            Text="Add Folder" CssClass="btn btn-success" CausesValidation="false"
            OnClick="btnAddFolder_Click"/>
    </div>
    
    <asp:GridView id="gvFolders" Runat="server" EnableViewState="True" AutoGenerateColumns="false"
        ShowHeader="true"
        CssSelectorClass="NormalGridView" EmptyDataText="gvFolders"
        DataKeyNames="Path" OnRowDeleting="gvFolders_RowDeleting">
        <Columns>
            <asp:TemplateField HeaderText="gvFoldersName">
	            <ItemStyle CssClass="NormalBold"></ItemStyle>
	            <ItemTemplate>
		            <asp:hyperlink ID="lnkEditFolder" runat="server" NavigateUrl='<%# GetEditControlUrl("edit_webfolder", Eval("Path").ToString()) %>'>
			            <%# Eval("Path")%>
		            </asp:hyperlink>
	            </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <itemtemplate>
                    <asp:LinkButton id="cmdDeleteFolder" CssClass="btn btn-danger" runat="server" CommandName='delete' CausesValidation="false" OnClientClick="return confirm('Delete?');"> 
                        <i class="bi bi-trash"></i> 
                    </asp:LinkButton>
                </itemtemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    
    <br />
    <div class="FormButtonsBar">
        <asp:Button id="btnAddUser" runat="server" meta:resourcekey="btnAddUser"
            Text="Add User" CssClass="btn btn-success" CausesValidation="false"
            OnClick="btnAddUser_Click"/>
    </div>
    <asp:GridView id="gvUsers" Runat="server" EnableViewState="True" AutoGenerateColumns="false"
        ShowHeader="true"
        CssSelectorClass="NormalGridView" EmptyDataText="gvUsers"
        DataKeyNames="Name" OnRowDeleting="gvUsers_RowDeleting">
        <Columns>
            <asp:TemplateField HeaderText="gvUsersName">
	            <ItemStyle CssClass="NormalBold"></ItemStyle>
	            <ItemTemplate>
		            <asp:hyperlink ID="lnkEditUser" runat="server" NavigateUrl='<%# GetEditControlUrl("edit_webuser", Eval("Name").ToString()) %>'>
			            <%# Eval("Name") %>
		            </asp:hyperlink>
	            </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <itemtemplate>
                    <asp:LinkButton id="cmdDeleteUser" CssClass="btn btn-danger" runat="server" CommandName='delete' CausesValidation="false" OnClientClick="return confirm('Delete?');"> 
                        <i class="bi bi-trash"></i> 
                    </asp:LinkButton>
                </itemtemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    
    <br />
    <div class="FormButtonsBar">
        <asp:Button id="btnAddGroup" runat="server" meta:resourcekey="btnAddGroup"
            Text="Add Group" CssClass="btn btn-success" CausesValidation="false"
            OnClick="btnAddGroup_Click"/>
    </div>
    <asp:GridView id="gvGroups" Runat="server" EnableViewState="True" AutoGenerateColumns="false"
        ShowHeader="true"
        EmptyDataText="gvGroups" CssSelectorClass="NormalGridView"
        DataKeyNames="Name" OnRowDeleting="gvGroups_RowDeleting">
        <Columns>
            <asp:TemplateField HeaderText="gvGroupsName">
	            <ItemStyle CssClass="NormalBold"></ItemStyle>
	            <ItemTemplate>
		            <asp:hyperlink ID="lnkEditGroup" runat="server" NavigateUrl='<%# GetEditControlUrl("edit_webgroup", Eval("Name").ToString()) %>'>
			            <%# Eval("Name") %>
		            </asp:hyperlink>
	            </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <itemtemplate>
                    <asp:LinkButton id="cmdDeleteGroup" CssClass="btn btn-danger" runat="server" CommandName='delete' CausesValidation="false" OnClientClick="return confirm('Delete?');"> 
                        <i class="bi bi-trash"></i> 
                    </asp:LinkButton>
                </itemtemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>

