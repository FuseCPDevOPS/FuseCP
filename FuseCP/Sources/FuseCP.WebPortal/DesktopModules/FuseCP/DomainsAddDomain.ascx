<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DomainsAddDomain.ascx.cs" Inherits="FuseCP.Portal.DomainsAddDomain" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="UserControls/DomainControl.ascx" TagName="DomainControl" TagPrefix="fcp" %>
<%@ Register Src="UserControls/CollapsiblePanel.ascx" TagPrefix="fcp" TagName="CollapsiblePanel" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />

<asp:ValidationSummary ID="summary" runat="server" ShowMessageBox="true" ShowSummary="true" ValidationGroup="Domain" />

<div id="DomainPanel" runat="server" class="py-3 ps-2">
        <fcp:DomainControl ID="DomainName" runat="server" RequiredEnabled="True" ValidationGroup="Domain"></fcp:DomainControl>
</div>
<div class="card-body">
    <fcp:CollapsiblePanel id="OptionsPanelHeader" runat="server"
        TargetControlID="OptionsPanel" resourcekey="OptionsPanelHeader" Text="Provisioning options">
    </fcp:CollapsiblePanel>
    <asp:Panel ID="OptionsPanel" runat="server">
        
        <br />
        <asp:Panel id="CreateFuseCP" runat="server" CssClass="pb-3">
            <asp:CheckBox ID="CreateWebSite" runat="server" meta:resourcekey="CreateWebSite" Text="Create Web Site" CssClass="input-group" Checked="true" /><br />
            <div class="ps-3">
                <asp:Localize ID="DescribeCreateWebSite" runat="server" meta:resourcekey="DescribeCreateWebSite">Description...</asp:Localize>
            </div>
            <div class="d-flex flex-wrap gap-2 align-items-center ps-3">
		        <asp:Label ID="lblHostName" runat="server" meta:resourcekey="lblHostName" Text="Host name:"></asp:Label>
			    <asp:TextBox ID="txtHostName" runat="server" CssClass="form-control" Text="www"></asp:TextBox>
            </div>
        </asp:Panel>

        <asp:Panel id="PointFuseCP" runat="server" CssClass="pb-3">
            <asp:CheckBox ID="PointWebSite" runat="server" meta:resourcekey="PointWebSite" Text="Assign to Web Site" CssClass="input-group"
                AutoPostBack="true" /><br />
            <div class="ps-3">
                <asp:DropDownList ID="WebSitesList" Runat="server" CssClass="form-control" DataTextField="Name" DataValueField="ID"></asp:DropDownList>
            </div>
        </asp:Panel>
        
        <asp:Panel id="PointMailDomainPanel" runat="server" CssClass="pb-3">
            <asp:CheckBox ID="PointMailDomain" runat="server" meta:resourcekey="PointMailDomain" Text="Assign to Mail Domain" CssClass="input-group"
                AutoPostBack="true" /><br />
            <div class="ps-3">
                <asp:DropDownList ID="MailDomainsList" Runat="server" CssClass="form-control" DataTextField="Name" DataValueField="ID"></asp:DropDownList>
            </div>
        </asp:Panel>
        
        <asp:Panel id="EnableDnsPanel" runat="server" CssClass="pb-3">
            <asp:CheckBox ID="EnableDns" runat="server" meta:resourcekey="EnableDns" Text="Enable DNS" CssClass="input-group"
                Checked="true" /><br />
            <div class="ps-3">
                <asp:Localize ID="DescribeEnableDns" runat="server" meta:resourcekey="DescribeEnableDns">Description...</asp:Localize>
            </div>
        </asp:Panel>
 
        <asp:Panel id="PreviewDomainPanel" runat="server" CssClass="pb-3">
            <asp:CheckBox ID="CreatePreviewDomain" runat="server" meta:resourcekey="CreatePreviewDomain"
                Text="Create Preview Domain" CssClass="input-group" /><br />
            <div class="ps-3">
                <asp:Localize ID="DescribeCreatePreviewDomain" runat="server" meta:resourcekey="DescribeCreatePreviewDomain">Description...</asp:Localize>
            </div>
        </asp:Panel>       
        <asp:Panel id="AllowSubDomainsPanel" runat="server" CssClass="pb-3">
            <asp:CheckBox ID="AllowSubDomains" runat="server" meta:resourcekey="AllowSubDomains" Text="Allow sub-domains" CssClass="input-group" /><br />
            <div class="ps-3">
                <asp:Localize ID="DescribeAllowSubDomains" runat="server" meta:resourcekey="DescribeAllowSubDomains">Description...</asp:Localize>
            </div>
        </asp:Panel>
        
    </asp:Panel>

</div>

<div class="card-footer text-end">
     <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
         <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/>
 	</asp:LinkButton>
     <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" OnClientClick="ShowProgressDialog('Adding Domain...');" ValidationGroup="Domain"> 
         <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAdd"/> 
    </asp:LinkButton>
</div>
