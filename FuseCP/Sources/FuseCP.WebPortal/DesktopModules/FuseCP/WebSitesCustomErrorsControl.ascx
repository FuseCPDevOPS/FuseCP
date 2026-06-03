<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesCustomErrorsControl.ascx.cs" Inherits="FuseCP.Portal.WebSitesCustomErrorsControl" %>


<div class="fcp-p-20 fcp-mb-10">
<table class="table table-borderless align-middle mb-0">
  <tr id="rowAspNet" runat="server">
    <td class="SubHead">
        <asp:Label ID="lblErrorMode" runat="server" meta:resourcekey="lblErrorMode"></asp:Label>:
    </td>
    <td class="Normal">
        <asp:DropDownList ID="ddlErrorMode" runat="server" CssClass="form-control"></asp:DropDownList>
    </td>
  </tr>
</table>
</div>

<div class="FormButtonsBar">
    <asp:Button id="btnAdd" runat="server" meta:resourcekey="btnAddError" Text="Add Custom Error" CssClass="btn btn-primary" OnClick="btnAdd_Click" CausesValidation="False"></asp:Button>
</div>
<asp:GridView id="gvErrorPages" Runat="server" AutoGenerateColumns="False"
    CssSelectorClass="NormalGridView"
    OnRowCommand="gvErrorPages_RowCommand" OnRowDataBound="gvErrorPages_RowDataBound"
    EmptyDataText="gvErrorPages">
    <columns>
	    <asp:TemplateField HeaderText="gvErrorPagesCode">
		    <itemtemplate>
			    <asp:TextBox id="txtErrorCode" Runat="server" Width="55px" CssClass="form-control" Text='<%# Eval("ErrorCode") %>'></asp:TextBox>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <itemtemplate>
			    . 
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <itemtemplate>
            <asp:TextBox id="txtErrorSubcode" Runat="server" Width="55px" CssClass="form-control" Text='<%# GetSubCode(Eval("ErrorSubcode")) %>'>
			    </asp:TextBox>
		    </itemtemplate>
	    </asp:TemplateField>
	    <asp:TemplateField HeaderText="gvErrorPagesHandlerType">
		    <itemtemplate>
			    <asp:dropdownlist id="ddlHandlerType" Width="100px" Runat="server"
			        CssClass="form-control">
			    </asp:dropdownlist>
		    </itemtemplate>
	    </asp:TemplateField>
	    <asp:TemplateField HeaderText="gvErrorPagesErrorContent">
		    <itemtemplate>
			    <asp:TextBox id="txtErrorContent" Runat="server" CssClass="form-control" Text='<%# Eval("ErrorContent") %>'>
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
