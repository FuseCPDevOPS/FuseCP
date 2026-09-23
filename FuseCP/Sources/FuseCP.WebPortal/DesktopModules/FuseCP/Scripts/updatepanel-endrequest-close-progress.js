// Shared helper to close progress dialog after UpdatePanel async callbacks.
(function () {
    function init() {
        // This script renders before the theme's jQuery (registered later in the page).
        if (typeof window.jQuery === "undefined") {
            return;
        }

        $(document).ready(function () {
            if (!window.Sys || !Sys.WebForms || !Sys.WebForms.PageRequestManager) {
                return;
            }

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                if (typeof CloseProgressDialog === "function") {
                    CloseProgressDialog();
                }
            });
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
}());
