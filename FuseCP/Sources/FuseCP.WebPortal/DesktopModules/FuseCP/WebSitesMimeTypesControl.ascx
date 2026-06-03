<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesMimeTypesControl.ascx.cs" Inherits="FuseCP.Portal.WebSitesMimeTypesControl" %>
<div >
    <div class="FormButtonsBar">
        <asp:Button id="btnAdd" runat="server" meta:resourcekey="btnAddMime" Text="Add MIME" CssClass="btn btn-success" CausesValidation="false" OnClick="btnAdd_Click"/>
    </div>

    <asp:GridView id="gvMimeTypes" Runat="server" AutoGenerateColumns="False"
        CssSelectorClass="NormalGridView"
	    OnRowCommand="gvMimeTypes_RowCommand" OnRowDataBound="gvMimeTypes_RowDataBound"
	    EmptyDataText="gvMimeTypes">
	    <columns>
		    <asp:TemplateField HeaderText="gvMimeTypesExtension">
			    <itemtemplate>
				    <asp:TextBox id="txtExtension" Runat="server" CssClass="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "Extension") %>'>
				    </asp:TextBox>
			    </itemtemplate>
		    </asp:TemplateField>
		    <asp:TemplateField HeaderText="gvMimeTypesMIMEtype">
			    <itemtemplate>
				    <asp:TextBox id="txtMimeType" Runat="server" CssClass="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "MimeType") %>'>
				    </asp:TextBox>
			    </itemtemplate>
		    </asp:TemplateField>
	        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
		        <itemtemplate>
			        <asp:LinkButton id="cmdDeleteMime" CssClass="btn btn-danger" runat="server" CommandName='delete_item' CausesValidation="false"> 
                        <i class="bi bi-trash"></i> 
                    </asp:LinkButton>
		        </itemtemplate>
	        </asp:TemplateField>
	    </columns>
    </asp:GridView>

</div>
