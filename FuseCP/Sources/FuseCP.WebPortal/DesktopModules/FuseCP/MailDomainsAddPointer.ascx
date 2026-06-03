<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailDomainsAddPointer.ascx.cs" Inherits="FuseCP.Portal.MailDomainsAddPointer" %>
<%@ Register Src="DomainsSelectDomainControl.ascx" TagName="DomainsSelectDomainControl" TagPrefix="uc1" %>
<div class="card-body form-horizontal">
    <table class="table table-borderless align-middle mb-0 w-100">
	    <tr>
            <td class="SubHead FormLabel200 text-nowrap"><asp:Label ID="lblDomainName" runat="server" meta:resourcekey="lblDomainName" Text="Domain name:"></asp:Label></td>
            <td>
                <uc1:DomainsSelectDomainControl ID="domainsSelectDomainControl" runat="server"
                    HideMailDomains="true" HideDomainsSubDomains="false" HidePreviewDomain="false" HideDomainPointers="true" HideIdnDomains="True"/>
		    </td>
	    </tr>
    </table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAdd"/> </asp:LinkButton>
</div>


