<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditDomainsList.ascx.cs" Inherits="FuseCP.Portal.UserControls.EditDomainsList" %>
<div>
	<div class="FormButtonsBar">
		<asp:Button id="btnAddDomain" runat="server" meta:resourcekey="btnAddDomain" Text="Add"
			CssClass="btn btn-success" CausesValidation="false" OnClick="BtnAddClick"/>
	</div>

	<asp:GridView id="gvDomains" Runat="server" AutoGenerateColumns="False"
		CssSelectorClass="NormalGridView"
		OnRowCommand="ListRowCommand" OnRowDataBound="ListRowDataBound"
		EmptyDataText="gvDomains" ShowHeader="False">
		<columns>
			<asp:TemplateField HeaderText="gvDomainsLabel" ItemStyle-Wrap="false">
				<itemtemplate>
					<asp:Label id="lblDomainName" Runat="server"></asp:Label>
				</itemtemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="gvDomainsName">
				<itemtemplate>
					<asp:TextBox id="txtDomainName" Runat="server" Width="200px" CssClass="form-control" MaxLength="255">
					</asp:TextBox>
					<asp:RegularExpressionValidator id="valCorrectDomainName" runat="server" CssClass="form-label"
						ValidationExpression="^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.){1,10}[a-zA-Z]{2,15}$"
						ErrorMessage="&nbsp;*" ControlToValidate="txtDomainName" Display="Dynamic"></asp:RegularExpressionValidator>
				</itemtemplate>
			</asp:TemplateField>
			<asp:TemplateField>
				<itemtemplate>
					<asp:LinkButton id="cmdDeleteDomain" CssClass="btn btn-danger" runat="server" CommandName='delete_item' CausesValidation="false"> 
                        <i class="bi bi-trash"></i> 
                    </asp:LinkButton>
				</itemtemplate>
				<ItemStyle HorizontalAlign="Center" />
			</asp:TemplateField>
		</columns>
	</asp:GridView>

</div>
