<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FuseCP.WebPortal.DefaultPage" %>

<!DOCTYPE html>
<html runat="server" id="htmltheme">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=9">
    <title>Solid CP</title>
    <link runat="server" rel="stylesheet" href="~/Styles/Import.css" type="text/css" id="AdaptersInvariantImportCSS" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
    <!-- Fav and touch icons -->
	<link rel="apple-touch-icon" sizes="57x57" href="/apple-touch-icon-57x57.png">
    <link rel="apple-touch-icon" sizes="60x60" href="/apple-touch-icon-60x60.png">
    <link rel="apple-touch-icon" sizes="72x72" href="/apple-touch-icon-72x72.png">
    <link rel="apple-touch-icon" sizes="76x76" href="/apple-touch-icon-76x76.png">
    <link rel="apple-touch-icon" sizes="114x114" href="/apple-touch-icon-114x114.png">
    <link rel="apple-touch-icon" sizes="120x120" href="/apple-touch-icon-120x120.png">
    <link rel="apple-touch-icon" sizes="144x144" href="/apple-touch-icon-144x144.png">
    <link rel="apple-touch-icon" sizes="152x152" href="/apple-touch-icon-152x152.png">
    <link rel="apple-touch-icon" sizes="180x180" href="/apple-touch-icon-180x180.png">
    <link rel="icon" type="image/png" href="/favicon-32x32.png" sizes="32x32">
    <link rel="icon" type="image/png" href="/android-chrome-192x192.png" sizes="192x192">
    <link rel="icon" type="image/png" href="/favicon-96x96.png" sizes="96x96">
    <link rel="icon" type="image/png" href="/favicon-16x16.png" sizes="16x16">
    <link rel="manifest" href="/manifest.json">
    <link rel="mask-icon" href="/safari-pinned-tab.svg" color="#5bbad5">
    <meta name="msapplication-TileColor" content="#da532c">
    <meta name="msapplication-TileImage" content="/mstile-144x144.png">
    <meta name="theme-color" content="#ffffff">
<!--[if lt IE 7]>
    <link runat="server" rel="stylesheet" href="~/Styles/BrowserSpecific/IEMenu6.css" type="text/css" id="IEMenu6CSS" />
<![endif]--> 
<!--[if gt IE 6]>
    <link runat="server" rel="stylesheet" href="~/Styles/BrowserSpecific/IEMenu7.css" type="text/css" id="IEMenu7CSS" />
<![endif]--> 
</head>
<body class="fixed-top-active dashboard">
    <form id="form1" runat="server"  autocomplete="off">
        <asp:PlaceHolder ID="skinPlaceHolder" runat="server"></asp:PlaceHolder>
    </form>
    <script type="text/javascript">
        (function () {
            document.addEventListener('submit', function (event) {
                var form = event.target;
                if (form && form.tagName === 'FORM') {
                    if (form.getAttribute('data-submitting') === 'true') {
                        event.preventDefault();
                    } else {
                        form.setAttribute('data-submitting', 'true');

                        var submitControls = form.querySelectorAll('button[type="submit"], input[type="submit"]');
                        for (var i = 0; i < submitControls.length; i++) {
                            if (submitControls[i].getAttribute('data-allow-multi-submit') !== 'true') {
                                submitControls[i].setAttribute('disabled', 'disabled');
                            }
                        }

                        window.setTimeout(function () {
                            form.removeAttribute('data-submitting');
                            for (var j = 0; j < submitControls.length; j++) {
                                submitControls[j].removeAttribute('disabled');
                            }
                        }, 15000);
                    }
                }
            }, true);
        })();
    </script>
    <script type="text/javascript">
        (function () {
            function byIdOrSuffix(id) {
                if (!id) return null;
                return document.getElementById(id) || document.querySelector('[id$="' + id + '"]');
            }

            function getValidators() {
                if (window.Page_Validators && window.Page_Validators.length) {
                    return window.Page_Validators;
                }
                return document.querySelectorAll('[data-val-controltovalidate], [controltovalidate]');
            }

            function getTargetId(validator) {
                return validator.controltovalidate ||
                    validator.getAttribute('controltovalidate') ||
                    validator.getAttribute('data-val-controltovalidate');
            }

            var hasValidated = false;

            function isStarMarker(validator) {
                var text = (validator.textContent || validator.innerText || '').trim();
                var errorMessage = (validator.errormessage || validator.getAttribute('errormessage') || validator.getAttribute('data-val-errormessage') || '').trim();
                return text === '*' || errorMessage === '*';
            }

            function isValidatorInvalid(validator) {
                return validator.isvalid === false || validator.getAttribute('data-val-isvalid') === 'False';
            }

            function hasVisibleMessage(validator) {
                var text = (validator.textContent || validator.innerText || '').trim();
                if (isStarMarker(validator) && text === '*') {
                    text = '';
                }

                var style = window.getComputedStyle ? window.getComputedStyle(validator) : null;
                var hidden = validator.hidden || validator.getAttribute('aria-hidden') === 'true' || validator.style.display === 'none' || (style && style.display === 'none');
                return !hidden && text.length > 0;
            }

            function detectExistingValidationState() {
                var validators = getValidators();

                for (var i = 0; i < validators.length; i++) {
                    if (isValidatorInvalid(validators[i]) && hasVisibleMessage(validators[i])) {
                        return true;
                    }
                }

                return false;
            }

            function refreshValidationUi() {
                var validators = getValidators();

                for (var i = 0; i < validators.length; i++) {
                    var validator = validators[i];
                    var target = byIdOrSuffix(getTargetId(validator));
                    if (!target) continue;

                    var invalid = isValidatorInvalid(validator) && (hasValidated || hasVisibleMessage(validator));

                    target.classList.toggle('is-invalid', invalid);
                    target.setAttribute('aria-invalid', invalid ? 'true' : 'false');

                    var inputGroup = target.closest ? target.closest('.input-group') : null;
                    var icon = inputGroup ? inputGroup.querySelector('.input-group-text') : null;
                    if (icon) {
                        icon.classList.toggle('is-invalid', invalid);
                    }

                    if (isStarMarker(validator)) {
                        validator.classList.add('fcp-validator-marker');
                    }
                }
            }

            var originalPageClientValidate = window.Page_ClientValidate;
            if (typeof originalPageClientValidate === 'function') {
                window.Page_ClientValidate = function (validationGroup) {
                    hasValidated = true;
                    var result = originalPageClientValidate(validationGroup);
                    window.setTimeout(refreshValidationUi, 0);
                    return result;
                };
            }

            document.addEventListener('submit', function () {
                hasValidated = true;
                window.setTimeout(refreshValidationUi, 0);
            }, true);

            document.addEventListener('change', function (event) {
                var target = event.target;
                if (!target || !target.id) return;
                if (target.matches('input,select,textarea')) {
                    window.setTimeout(refreshValidationUi, 0);
                }
            }, true);

            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', function () {
                    hasValidated = detectExistingValidationState();
                    refreshValidationUi();
                });
            } else {
                hasValidated = detectExistingValidationState();
                refreshValidationUi();
            }
        })();
    </script>
</body>
</html>
