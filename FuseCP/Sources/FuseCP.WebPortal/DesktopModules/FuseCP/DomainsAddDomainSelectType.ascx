<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DomainsAddDomainSelectType.ascx.cs" Inherits="FuseCP.Portal.DomainsAddDomainSelectType" EnableViewState="false" %>

<div class="card-body form-horizontal">

    <p>
        <asp:Localize ID="IntroPar" runat="server" meta:resourcekey="IntroPar" />
    </p>
    <div class="row mb-3">
         <div class="col-sm-3 col-md-2 mb-2 mb-sm-0 d-grid"><asp:HyperLink ID="DomainLink" CssClass="btn btn-primary" runat="server"><i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="DomainLink"/></asp:HyperLink></div>
         <div class="col-sm-9 col-md-10"><asp:Localize ID="DomainDescription" runat="server" meta:resourcekey="DomainDescription" /></div>
     </div>
     <div class="row mb-3">
         <div class="col-sm-3 col-md-2 mb-2 mb-sm-0 d-grid"><asp:HyperLink ID="SubDomainLink" CssClass="btn btn-primary" runat="server"><i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="SubDomainLink"/></asp:HyperLink></div>
         <div class="col-sm-9 col-md-10"><asp:Localize ID="SubDomainDescription" runat="server" meta:resourcekey="SubDomainDescription" /></div>
     </div>
    <div id="ProviderSubDomainPanel" runat="server" class="row mb-3">
        <div class="col-sm-3 col-md-2 mb-2 mb-sm-0 d-grid"><asp:HyperLink ID="ProviderSubDomainLink" CssClass="btn btn-primary" runat="server"><i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="ProviderSubDomainLink"/></asp:HyperLink></div>
        <div class="col-sm-9 col-md-10"><asp:Localize ID="ProviderSubDomainDescription" runat="server" meta:resourcekey="ProviderSubDomainDescription" /></div>
<!--    
    <p>
        <b><asp:HyperLink ID="DomainPointerLink" runat="server" meta:resourcekey="DomainPointerLink">Domain Alias</asp:HyperLink></b><br />
        <asp:Localize ID="DomainPointerDescription" runat="server" meta:resourcekey="DomainPointerDescription" />
    </p>
-->
</div>
</div>

<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>
</div>
