<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpaceEditDnsRecords.ascx.cs" Inherits="FuseCP.Portal.SpaceEditDnsRecords" %>
<%@ Register Src="GlobalDnsRecordsControl.ascx" TagName="GlobalDnsRecordsControl" TagPrefix="uc1" %>

<div class="card-body form-horizontal">
    <uc1:GlobalDnsRecordsControl ID="GlobalDnsRecordsControl1" runat="server" PackageIdParam="SpaceID" />
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnBack" CssClass="btn btn-warning" runat="server" OnClick="btnBack_Click" CausesValidation="false"> <i class="bi bi-arrow-left me-1"></i><asp:Localize runat="server" meta:resourcekey="btnBackText"/> </asp:LinkButton>
</div>
