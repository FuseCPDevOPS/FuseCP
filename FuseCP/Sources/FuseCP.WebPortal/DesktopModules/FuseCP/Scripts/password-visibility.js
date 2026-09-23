// Shared helper for password fields that should mask on blur and reveal on focus.
(function (global) {
    "use strict";

    function init() {
        // This script renders before the theme's jQuery (registered later in the page).
        if (!global.jQuery) {
            return;
        }

        global.jQuery(function ($) {
            $(".hideContentOnBlur")
                .off("blur.fusecpPassword focus.fusecpPassword")
                .on("blur.fusecpPassword", function () {
                    this.type = "password";
                })
                .on("focus.fusecpPassword", function () {
                    this.type = "text";
                });
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
}(window));
