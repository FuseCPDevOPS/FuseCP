<%@ Control AutoEventWireup="true" %>
<%@ Register TagPrefix="fcp" TagName="SiteFooter" Src="~/DesktopModules/FuseCP/SkinControls/SiteFooter.ascx" %>
<%@ Register TagPrefix="fcp" TagName="Logo" Src="~/DesktopModules/FuseCP/SkinControls/Logo.ascx" %>
<%@ Register TagPrefix="fcp" TagName="ThemeScripts" Src="~/DesktopModules/FuseCP/SkinControls/ThemeScripts.ascx" %>
<style>
    body {
        background-color: var(--fcp-bg);
    }
</style>
<div class="middle-content page-login">
    <div class="login-table-row">
        <div class="top-bar text-center login-table-cell">
            <a href='<%= Page.ResolveUrl("~/") %>'>
                <asp:Image runat="server" SkinID="Logo" alt="" />
            </a>
        </div>
    </div>
    <div class="container-fluid primary-content" id="SkinContent">
        <asp:ScriptManager ID="scriptManager" runat="server"></asp:ScriptManager>
        <div class="row">
            <div class="col-sm-4 centering">
                <asp:PlaceHolder ID="ContentPane" runat="server"></asp:PlaceHolder>
                <div class="login-footer">
                    <fcp:SiteFooter ID="SiteFooter2" runat="server" />
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Javascript -->
<fcp:ThemeScripts ID="themeScripts" runat="server" />
<script src='<%= ResolveUrl("~/App_Themes/" + Page.Theme + "/js/fcp-form-layouts.js") %>'></script>
