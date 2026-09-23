// Shared client-side helpers for Exchange mailbox creation pages.
(function (global) {
    "use strict";

    function byId(id) {
        return document.getElementById(id);
    }

    function valueOrEmpty(id) {
        var node = byId(id);
        return node ? node.value : "";
    }

    function buildDisplayName(displayNameId, firstNameId, initialsId, lastNameId) {
        var displayNode = byId(displayNameId);
        if (!displayNode) {
            return;
        }

        var parts = [];
        var first = valueOrEmpty(firstNameId);
        var initials = valueOrEmpty(initialsId);
        var last = valueOrEmpty(lastNameId);

        if (first) {
            parts.push(first);
        }

        if (initials) {
            parts.push(initials);
        }

        if (last) {
            parts.push(last);
        }

        displayNode.value = parts.join(" ");
    }

    function showProgress() {
        if (!global.jQuery) {
            return;
        }

        global.setTimeout(function () {
            var $ = global.jQuery;
            var modal = $("<div />");
            modal.addClass("modal");
            $("body").append(modal);

            var loading = $(".loading");
            if (!loading.length) {
                return;
            }

            loading.show();
            var top = Math.max($(global).height() / 2 - loading[0].offsetHeight / 2, 0);
            var left = Math.max($(global).width() / 2 - loading[0].offsetWidth / 2, 0);
            loading.css({ top: top, left: left });
        }, 200);
    }

    function wireFormProgress() {
        if (!global.jQuery) {
            return;
        }

        global.jQuery("form").off("submit.fcpMailboxProgress").on("submit.fcpMailboxProgress", function () {
            showProgress();
        });
    }

    global.FuseCPExchangeCreateMailbox = {
        buildDisplayName: buildDisplayName,
        wireFormProgress: wireFormProgress
    };

    function init() {
        // This script renders before the theme's jQuery (registered later in the page).
        if (!global.jQuery) {
            return;
        }

        wireFormProgress();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
}(window));
