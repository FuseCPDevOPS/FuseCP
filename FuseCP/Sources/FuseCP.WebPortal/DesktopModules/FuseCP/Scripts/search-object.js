// SearchObject popup dialog/filter initialization.
(function (global) {
    "use strict";

    function estop(e) {
        var evt = e || global.event;
        if (!evt) {
            return evt;
        }

        evt.cancelBubble = true;
        if (evt.stopPropagation) {
            evt.stopPropagation();
        }
        return evt;
    }

    global.CPopupDialog = function (el, e) {
        var node = el;
        if (typeof node === "string") {
            node = document.getElementById(node);
        }
        if (!node) {
            return;
        }

        var eventObj = estop(e);
        var oldclick = document.body.onclick;
        node.onclick = estop;

        function close() {
            node.style.display = "none";
            document.body.onclick = oldclick;
        }

        function show(x, y) {
            node.style.left = x ? x : eventObj.clientX + document.documentElement.scrollLeft + "px";
            node.style.top = y ? y : eventObj.clientY + document.documentElement.scrollTop + "px";
            node.style.display = "block";
            document.body.onclick = close;
        }

        show();
    };

    function init() {
        // This script renders before the theme's jQuery (registered later in the page).
        if (!global.jQuery) {
            return;
        }

        global.jQuery(function ($) {
            function loadFilters() {
                var value = $("#tbFilters").val();
                if (!value) {
                    return;
                }

                var typesSelected = JSON.parse(value);
                $("#mydialog input[rel]").each(function () {
                    var rel = $(this).attr("rel");
                    if (typesSelected.indexOf(rel) >= 0) {
                        $(this).val("1");
                    }
                });
            }

            $("#btnSelectFilter").click(function () {
                var typesSelected = [];
                $("#mydialog input[rel]").each(function () {
                    var val = $(this).attr("checked");
                    if (val) {
                        typesSelected.push($(this).attr("rel"));
                    }
                });
                $("#tbFilters").val(JSON.stringify(typesSelected));
                document.forms[0].submit();
            });

            loadFilters();
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
}(window));