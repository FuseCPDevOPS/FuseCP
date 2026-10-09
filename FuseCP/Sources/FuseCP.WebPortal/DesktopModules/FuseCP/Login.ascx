<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="FuseCP.Portal.Login" %>

<div class="card-body mb-3" runat="server" id="userPwdDiv">
    <div class="row g-3 mb-3">
        <div class="col-sm-12">
            <div class="input-group">
                <span class="input-group-text"><i class="bi bi-person" aria-hidden="true"></i></span>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <asp:RequiredFieldValidator ID="usernameValidator" runat="server" ErrorMessage="*" Text="" ControlToValidate="txtUsername" Display="None" ValidationGroup="LoginForm"></asp:RequiredFieldValidator>
        </div>
    </div>
    <div class="row g-3 mb-3">
        <div class="col-sm-12">
            <div class="input-group">
                <span class="input-group-text"><i class="bi bi-lock" aria-hidden="true"></i></span>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
            </div>
            <asp:RequiredFieldValidator ID="passwordValidator" runat="server" ErrorMessage="*" Text="" ControlToValidate="txtPassword" Display="None" ValidationGroup="LoginForm"></asp:RequiredFieldValidator>
        </div>
    </div>
    <div class="row g-2 align-items-center mb-2">
        <div class="col-sm-6 mb-2 mb-sm-0">
            <asp:CheckBox ID="chkRemember" runat="server" meta:resourcekey="chkRemember" Text="Remember me on this computer"></asp:CheckBox>
        </div>
        <div class="col-sm-6 text-sm-end">
            <asp:LinkButton ID="btnLogin" runat="server" CssClass="btn btn-success" OnClick="btnLogin_Click" ValidationGroup="LoginForm" CausesValidation="True">
                <asp:Localize runat="server" meta:resourcekey="btnLogin" />&nbsp;<i class="bi bi-box-arrow-in-right" aria-hidden="true"></i></asp:LinkButton>
        </div>
    </div>
    <hr />
    <div class="row">
        <div class="col-sm-12">
            <h5><asp:Localize ID="forgotpass" runat="server" meta:resourcekey="forgotpass" /></h5>
            <p><asp:Localize ID="noworries" runat="server" meta:resourcekey="noworries" />
                <asp:LinkButton ID="cmdForgotPassword" runat="server" CssClass="color-green" CausesValidation="False" OnClick="cmdForgotPassword_Click"><asp:Localize runat="server" meta:resourcekey="cmdForgotPassword" /></asp:LinkButton>
                <asp:Localize ID="toresetyourpassword" runat="server" meta:resourcekey="toresetyourpassword" />.</p>
        </div>
    </div>
</div>
<div class="card-body mb-3" runat="server" id="tokenDiv" visible="false">
    <div class="row mb-3">
        <div class="col-sm-12">
            <div class="input-group">
                <span class="input-group-text"><i class="bi bi-lock" aria-hidden="true"></i></span>
                <asp:TextBox ID="txtPin" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <asp:RequiredFieldValidator ID="pinValidator" runat="server" ErrorMessage="*" Text="" ControlToValidate="txtPin" Display="None" ValidationGroup="PinForm"></asp:RequiredFieldValidator>
        </div>
    </div>
     <div class="row">
         <div class="col-sm-12">
            <div class="d-flex justify-content-end flex-wrap gap-2">
                <asp:LinkButton runat="server" ID="btnResendPin" CssClass="btn btn-success" OnClick="btnResendPin_Click">
                    <asp:Localize runat="server" meta:resourcekey="btResendPin" />
                </asp:LinkButton>
                <asp:LinkButton ID="StyleButton2" runat="server" CssClass="btn btn-success" OnClick="btnVerifyPin_Click" ValidationGroup="PinForm" CausesValidation="True">
                    <asp:Localize runat="server" meta:resourcekey="btnLogin" />&nbsp;<i class="bi bi-box-arrow-in-right" aria-hidden="true"></i>
                </asp:LinkButton>
            </div>
        </div>
    </div>
</div>
<div class="card-body mb-3" runat="server" id="passkeyDiv" visible="false">
    <div class="row mb-3">
        <div class="col-sm-12">
            <asp:Label runat="server" ID="lblPasskeyText" meta:resourcekey="lblPasskeyText" CssClass="fcp-field-note" Text="A passkey is registered for this account. Confirm it is you with your device."></asp:Label>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12 text-end">
            <asp:LinkButton runat="server" ID="btnPasskey" CssClass="btn btn-success" OnClick="btnPasskey_Click" CausesValidation="False">
                <asp:Localize runat="server" meta:resourcekey="btnPasskey" />&nbsp;<i class="bi bi-key" aria-hidden="true"></i>
            </asp:LinkButton>
        </div>
    </div>
    <div id="passkeyFallbackDiv" runat="server" visible="false" class="fcp-field-note mt-3">
        <asp:Label runat="server" ID="lblPasskeyFallback" meta:resourcekey="lblPasskeyFallback" Text="Passkey unavailable?"></asp:Label>
        &nbsp;
        <asp:LinkButton runat="server" ID="btnPasskeyFallback" OnClick="btnPasskeyFallback_Click" CausesValidation="False" CssClass="color-green">
            <asp:Localize runat="server" meta:resourcekey="btnPasskeyFallback" Text="Use a verification code instead" />
        </asp:LinkButton>
    </div>
</div>
<asp:HiddenField ID="hdnPasskeyOptions" runat="server" />
<asp:HiddenField ID="hdnPasskeyCeremonyId" runat="server" />
<asp:HiddenField ID="hdnPasskeyResponse" runat="server" />
<asp:HiddenField ID="hdnPasskeyTicket" runat="server" />
<asp:HiddenField ID="hdnPasskeyFallback" runat="server" />
<asp:HiddenField ID="hdnPinTicket" runat="server" />

<div class="card-footer">
    <div class="row g-3 align-items-end">
        <div class="col-6 fcp-login-select-group">
            <asp:Label ID="lblLanguage" runat="server" CssClass="form-label mb-1 fcp-login-select-label" meta:resourcekey="lblLanguage" Text="Preferred Language:" AssociatedControlID="ddlLanguage"></asp:Label>
            <span class="fcp-login-select-icon bi bi-translate" aria-hidden="true"></span>
            <asp:DropDownList ID="ddlLanguage" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged"></asp:DropDownList>
        </div>
        <div class="col-6 fcp-login-select-group">
            <asp:Label ID="lblTheme" runat="server" CssClass="form-label mb-1 fcp-login-select-label" meta:resourcekey="lblTheme" Text="Theme mode:" AssociatedControlID="ddlTheme"></asp:Label>
            <span class="fcp-login-select-icon bi bi-palette" aria-hidden="true"></span>
            <asp:DropDownList ID="ddlTheme" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>
    </div>
</div>

<script type="text/javascript">
    (function () {
        var themeCookieName = 'UserThemeMode';

        function byIdSuffix(idSuffix) {
            return document.querySelector('[id$="' + idSuffix + '"]');
        }

        function setCookie(name, value, days) {
            var expires = '';
            if (typeof days === 'number') {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = '; expires=' + date.toUTCString();
            }
            document.cookie = name + '=' + encodeURIComponent(value) + expires + '; path=/; SameSite=Lax';
        }

        function clearCookie(name) {
            document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; SameSite=Lax';
        }

        function applyThemeMode(mode) {
            var html = document.documentElement;
            html.classList.remove('light-theme', 'dark-theme');
            if (mode === 'light-theme' || mode === 'dark-theme') {
                html.classList.add(mode);
            }
        }

        function wireEnterToClick(inputSuffix, buttonSuffix) {
            var input = byIdSuffix(inputSuffix);
            if (!input) return;

            input.addEventListener('keydown', function (event) {
                if (event.key !== 'Enter') return;

                var button = byIdSuffix(buttonSuffix);
                if (!button) return;

                event.preventDefault();
                button.click();
            });
        }

        function wireThemeMode() {
            var themeSelect = byIdSuffix('ddlTheme');
            if (!themeSelect) return;

            themeSelect.addEventListener('change', function () {
                var selectedMode = (themeSelect.value || '').toLowerCase();

                if (selectedMode === 'auto') {
                    clearCookie(themeCookieName);
                    applyThemeMode('');
                } else if (selectedMode === 'dark-theme' || selectedMode === 'light-theme') {
                    setCookie(themeCookieName, selectedMode, 60);
                    applyThemeMode(selectedMode);
                } else {
                    setCookie(themeCookieName, 'light-theme', 60);
                    applyThemeMode('light-theme');
                }

                window.location.reload();
            });
        }

        function init() {
            wireEnterToClick('txtUsername', 'btnLogin');
            wireEnterToClick('txtPassword', 'btnLogin');
            wireEnterToClick('txtPin', 'StyleButton2');
            wireThemeMode();
        }

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', init);
        } else {
            init();
        }
    })();
</script>
