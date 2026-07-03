<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="FuseCP.Portal.VPS2012.UserControls.Menu" %>
<ul class="fcp-modern-nav-tabs nav nav-tabs" role="tablist">

	<asp:DataList runat="server" ID="MenuItems" EnableViewState="false" RepeatDirection="Horizontal" RepeatLayout="Flow">
	    <ItemTemplate>
	        <li class="nav-item" role="presentation">
                <asp:HyperLink ID="lnkItem" runat="server" CssClass="nav-link" NavigateUrl='<%# Eval("Url") %>' Text='<%# Eval("Text") %>'/>			                
            </li>
	    </ItemTemplate>
	    <SelectedItemTemplate>
            <li class="nav-item" role="presentation">
                <asp:HyperLink ID="lnkItem" runat="server" CssClass="nav-link active" aria-current="page"
	                NavigateUrl='<%# Eval("Url") %>'
	                Text='<%# Eval("Text") %>'/>			                
            </li>
        </SelectedItemTemplate>
	</asp:DataList>	
</ul>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/websites-edit-site.js?v=4"></script>
