// Shared startup hook for enterprise storage folders pages.
(function () {
    function init() {
        // This script renders before the theme's jQuery (registered later in the page).
        if (typeof window.jQuery === "undefined") {
            return;
        }

        $(document).ready(function () {
            if (typeof getFolderData === "function") {
                setTimeout(getFolderData, 3000);
            }
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
}());
