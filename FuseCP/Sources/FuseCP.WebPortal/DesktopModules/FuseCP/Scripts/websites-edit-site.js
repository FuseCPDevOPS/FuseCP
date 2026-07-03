// Shared helpers for WebSitesEditSite page.
(function (global) {
    "use strict";

    function confirmAndShow(message, progressText) {
        if (!global.confirm(message)) {
            return false;
        }

        if (typeof global.ShowProgressDialog === "function") {
            global.ShowProgressDialog(progressText);
        }

        return true;
    }

    function confirmationSITE() {
        return confirmAndShow("Are you sure you want to delete Web site?", "Deleting Web site...");
    }

    function confirmationFPSE() {
        return confirmAndShow("Are you sure you want to delete Frontpage account?", "Uninstalling Frontpage...");
    }

    function confirmationWMSVC() {
        return confirmAndShow("Are you sure you want to disable Remote Management?", "Disabling Remote Management...");
    }

    function confirmationWebDeployPublishing() {
        return confirmAndShow("Are you sure you want to disable Web Publishing?", "Disabling Web Publishing...");
    }


    function normalizeNavTabsMarkup() {
        if (!global.jQuery) {
            return;
        }

        global.jQuery(document).ready(function () {
            // DataList with RepeatLayout="Flow" renders the tab <li>s inside
            // <span> wrappers. Instead of unwrapping them (which breaks the
            // DataList postback/command routing), make those wrappers invisible
            // to the layout engine so the <li>s act as direct children of <ul>.
            global.jQuery("<style>" +
                ".fcp-modern-nav-tabs > span, " +
                ".fcp-modern-nav-tabs > span > span { display: contents; }" +
                "</style>").appendTo("head");
        });
    }

    global.confirmationSITE = confirmationSITE;
    global.confirmationFPSE = confirmationFPSE;
    global.confirmationWMSVC = confirmationWMSVC;
    global.confirmationWebDeployPublishing = confirmationWebDeployPublishing;

    normalizeNavTabsMarkup();
}(window));
