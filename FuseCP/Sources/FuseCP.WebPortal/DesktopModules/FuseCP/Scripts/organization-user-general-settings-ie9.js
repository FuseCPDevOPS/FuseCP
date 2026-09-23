// Legacy IE-specific fallback for thumbnail upload controls.
(function () {
    function init() {
        // This script renders before the theme's jQuery (registered later in the page).
        if (typeof window.jQuery === "undefined") {
            return;
        }

        var agentStr = navigator.userAgent;
        if ((agentStr.indexOf("Trident/5") > -1) || (agentStr.indexOf("Trident/4") > -1) || (agentStr.indexOf("Trident") === -1)) {
            $(function () {
                $("#divUpThumbnailphoto").show();
                $(".btnload").hide();
            });
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
}());
