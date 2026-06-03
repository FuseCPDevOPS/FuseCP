<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VLANsEditVLAN.ascx.cs" Inherits="FuseCP.Portal.VLANsEditVLAN" %>
<%@ Register Src="UserControls/EditVLANControl.ascx" TagName="EditVLANControl" TagPrefix="fcp" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>

<div class="card-body form-horizontal">
    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <asp:ValidationSummary ID="validatorsSummary" runat="server" ValidationGroup="EditVLAN" ShowMessageBox="True" ShowSummary="False" />
    <asp:CustomValidator ID="consistentVLAN" runat="server" ErrorMessage="Wrong VLAN." ValidationGroup="EditVLAN" Display="dynamic" ServerValidate="CheckVLAN" />
    <table class="table table-borderless align-middle mb-0">
	    <tr>
		    <td class="SubHead text-nowrap">
                <asp:Localize ID="locServer" runat="server" meta:resourcekey="locServer" Text="Server:"></asp:Localize>
		    </td>
		    <td>
		        <asp:dropdownlist id="ddlServer" CssClass="form-select" runat="server" DataTextField="ServerName" DataValueField="ServerID"></asp:dropdownlist>
		    </td>
	    </tr>
	    <tr id="VLANRow" runat="server">
		    <td class="SubHead text-nowrap">
                <asp:Localize ID="lblVLAN" runat="server" meta:resourcekey="lblVLAN" Text="VLAN:"></asp:Localize>
		    </td>
		    <td>
		        <fcp:EditVLANControl id="etVlan" runat="server" ValidationGroup="EditVLAN" Required="true" />
		    </td>
	    </tr>
	    <tr>
		    <td class="SubHead align-top text-nowrap">
                <asp:Localize ID="lblComments" runat="server" meta:resourcekey="lblComments" Text="Comments:"></asp:Localize>
		    </td>
		    <td>
                <asp:textbox id="txtComments" CssClass="form-control" runat="server" Rows="3" TextMode="MultiLine"></asp:textbox>
		    </td>
	    </tr>
    </table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
        <i class="bi bi-x-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCancel"/>
    </asp:LinkButton>
    &nbsp;
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" ValidationGroup="EditVLAN">
        <i class="bi bi-arrow-clockwise me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnUpdate"/>
    </asp:LinkButton>
</div>
