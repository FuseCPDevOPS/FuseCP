<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PhoneNumbersEditPhoneNumber.ascx.cs" Inherits="FuseCP.Portal.PhoneNumbersEditPhoneNumber" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>

<div class="card-body form-horizontal">

    <fcp:SimpleMessageBox id="messageBox" runat="server" />

    <asp:ValidationSummary ID="validatorsSummary" runat="server" 
            ValidationGroup="EditAddress" ShowMessageBox="True" ShowSummary="False" />

    <table class="table table-borderless align-middle mb-0">
	    <tr>
		    <td class="FormLabel150"><asp:Localize ID="locServer" runat="server" meta:resourcekey="locServer" Text="Server:"></asp:Localize></td>
		    <td>
		        <asp:dropdownlist id="ddlServer" CssClass="form-control" runat="server" DataTextField="ServerName" DataValueField="ServerID"></asp:dropdownlist>
		    </td>
	    </tr>
	    <tr id="PhoneNumbersRow" runat="server">
		    <td class="FormLabel150"><asp:Localize ID="Localize1" runat="server" meta:resourcekey="lblPhoneNumbers" Text="Phone Numbers:"></asp:Localize></td>
		    <td>
		    
		        <asp:TextBox id="Phone" runat="server" MaxLength="45" CssClass="form-control"/>
                <asp:RequiredFieldValidator ID="requireStartPhoneValidator" runat="server" meta:resourcekey="requireStartPhoneValidator"
                    ControlToValidate="Phone" SetFocusOnError="true" Text="*" Enabled="false" ValidationGroup="EditAddress" ErrorMessage="Enter Phone Number" />					            

		    </td>
	    </tr>
	    <tr>
		    <td class="FormLabel150"><asp:Localize ID="lblComments" runat="server" meta:resourcekey="lblComments" Text="Comments:"></asp:Localize></td>
		    <td><asp:textbox id="txtComments" CssClass="form-control" runat="server" Rows="3" TextMode="MultiLine"></asp:textbox></td>
	    </tr>
    </table>

</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" ValidationGroup="EditAddress"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdate"/> </asp:LinkButton>
</div>

