<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailDomainsEditDomain.ascx.cs" Inherits="FuseCP.Portal.MailDomainsEditDomain" %>

<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
	TagPrefix="fcp" %>


<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/mail-confirmation.js"></script>

<div class="card-body form-horizontal">
    <div class="Huge">
        <asp:Literal ID="litDomainName" runat="server"></asp:Literal>
    </div>
    <div class="card-body form-horizontal" >
        <div class="FormButtonsBar">
            <asp:Button ID="btnAddPointer" runat="server" meta:resourcekey="btnAddPointer" Text="Add Pointer" CssClass="btn btn-success" OnClick="btnAddPointer_Click" />
        </div>
        <asp:GridView id="gvPointers" Runat="server" EnableViewState="True" AutoGenerateColumns="false"
            ShowHeader="false"
            CssSelectorClass="NormalGridView"
            EmptyDataText="gvPointers" DataKeyNames="DomainID" OnRowDeleting="gvPointers_RowDeleting">
            <Columns>
	            <asp:TemplateField HeaderText="gvPointersName">
		            <ItemStyle Wrap="false"></ItemStyle>
		            <ItemTemplate>
                        <%# Eval("DomainName") %>
                        <asp:LinkButton id="cmdDeletePointer" CssClass="btn btn-danger" runat="server" CommandName='delete' CommandArgument='<%# Eval("DomainId") %>' OnClientClick="return confirm('Remove pointer?');" Visible='<%# !(bool)Eval("IsPreviewDomain") %>'> 
                            <i class="bi bi-trash"></i> 
                        </asp:LinkButton>
		            </ItemTemplate>
	            </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div class="card-body form-horizontal">
        <asp:PlaceHolder ID="providerControl" runat="server"></asp:PlaceHolder>
    </div>
</div>

<div class="card-footer text-end">
    <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return fuseCpConfirmWithProgress('Are you sure you want to delete this Domain?', 'Deleting Domain...');"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" OnClientClick="ShowProgressDialog('Updating Domain Settings...');"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdateText"/> </asp:LinkButton>
</div>

