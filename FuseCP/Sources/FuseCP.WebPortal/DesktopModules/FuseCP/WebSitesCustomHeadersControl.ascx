<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesCustomHeadersControl.ascx.cs" Inherits="FuseCP.Portal.WebSitesCustomHeadersControl" %>
<div >
    <div class="FormButtonsBar">
        <asp:Button id="btnAdd" runat="server" meta:resourcekey="btnAdd" Text="Add Custom Header" CssClass="btn btn-primary" CausesValidation="false" OnClick="btnAdd_Click"/>
    </div>
    <asp:GridView id="gvCustomHeaders" Runat="server" AutoGenerateColumns="False"
        CssSelectorClass="NormalGridView"
	    OnRowCommand="gvCustomHeaders_RowCommand" OnRowDataBound="gvCustomHeaders_RowDataBound"
	    EmptyDataText="gvCustomHeaders">
	    <columns>
		    <asp:TemplateField HeaderText="gvCustomHeadersName">
			    <itemtemplate>
				    <asp:TextBox id="txtName" Runat="server" CssClass="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "Key") %>'>
				    </asp:TextBox>
			    </itemtemplate>
		    </asp:TemplateField>
		    <asp:TemplateField HeaderText="gvCustomHeadersValue">
			    <itemtemplate>
				    <asp:TextBox id="txtValue" Runat="server" CssClass="form-control" Text='<%# DataBinder.Eval(Container.DataItem, "Value") %>'>
				    </asp:TextBox>
			    </itemtemplate>
		    </asp:TemplateField>
	        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
		        <itemtemplate>
			        <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName='delete_item' CausesValidation="false"> 
                        <i class="bi bi-trash"></i> 
                    </asp:LinkButton>
		        </itemtemplate>
	        </asp:TemplateField>
	    </columns>
    </asp:GridView>
</div>
