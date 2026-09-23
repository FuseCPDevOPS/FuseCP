// Shared security panel toggling for Helicon Ape controls.
(function () {
    function init() {
        // This script renders before the theme's jQuery (registered later in the page).
        if (typeof window.jQuery === "undefined") {
            return;
        }

        $(document).ready(function () {
            $("#ShowSecurityPanelButton").click(function () {
                $("#ShowSecurityPanelButton").slideUp();
                $("#SecurityPanel").slideDown();
            });
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
}());
