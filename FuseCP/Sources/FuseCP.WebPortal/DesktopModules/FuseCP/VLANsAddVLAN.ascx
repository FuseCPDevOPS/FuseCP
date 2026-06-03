<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VLANsAddVLAN.ascx.cs" Inherits="FuseCP.Portal.VLANsAddVLANs" %>
<%@ Register Src="UserControls/EditVLANControl.ascx" TagName="EditVLANControl" TagPrefix="fcp" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<div class="card-body form-horizontal">

    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <asp:ValidationSummary ID="validatorsSummary" runat="server"  ValidationGroup="EditVLAN" ShowMessageBox="True" ShowSummary="False" />
	<asp:CustomValidator ID="consistentVLANs" runat="server" ErrorMessage="Wrong VLAN." ValidationGroup="EditVLAN" Display="dynamic" ServerValidate="CheckVLANs" /> 
	<table class="table table-borderless align-middle mb-0">
	    <tr>
		    <td class="SubHead text-nowrap">
                <asp:Localize ID="locServer" runat="server" meta:resourcekey="locServer" Text="Server:"></asp:Localize>
		    </td>
		    <td>
		        <asp:dropdownlist id="ddlServer" CssClass="form-select" runat="server" DataTextField="ServerName" DataValueField="ServerID"></asp:dropdownlist>
		    </td>
	    </tr>
	    <tr>
		    <td class="SubHead text-nowrap">
                <asp:Localize ID="lblVLAN" runat="server" meta:resourcekey="lblVLAN" Text="VLAN:"></asp:Localize>
		    </td>
		    <td>
		        <div class="d-flex flex-wrap gap-2 align-items-center">
		        <fcp:EditVLANControl id="startVLAN" runat="server" ValidationGroup="EditVLAN" Required="true"/>
			    &nbsp;
                <asp:Localize ID="locTo" runat="server" meta:resourcekey="locTo" Text="to"></asp:Localize>
                &nbsp;
		        <fcp:EditVLANControl id="endVLAN" runat="server" ValidationGroup="EditVLAN" />
		        </div>
		    </td>
	    </tr>
	    <tr>
		    <td class="SubHead align-top text-nowrap"><asp:Localize ID="lblComments" runat="server" meta:resourcekey="lblComments" Text="Comments:"></asp:Localize></td>
		    <td class="NormalBold">
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
    <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" ValidationGroup="EditVLAN">
        <i class="bi bi-check-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnAdd"/>
    </asp:LinkButton>
</div>
