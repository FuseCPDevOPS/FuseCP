// Shared SearchBox client behavior for autocomplete and submit flow.
(function () {
    function parseExtraAjaxData(expression) {
        if (!expression) {
            return {};
        }

        try {
            return (new Function("return ({" + expression + "});"))();
        } catch (e) {
            return {};
        }
    }

    function init() {
        // This script renders before the theme's jQuery (registered later in the page).
        if (typeof window.jQuery === "undefined") {
            return;
        }

        $(document).ready(function () {
            var cfg = document.getElementById("searchBoxConfig");
            if (!cfg) {
                return;
            }

            var filterColumns = cfg.getAttribute("data-filter-columns") || "";
            var extraDataExpression = cfg.getAttribute("data-ajax-data") || "";
            var submitId = cfg.getAttribute("data-submit-id") || "";

            $("#tbSearch").keypress(function (e) {
                if (e.keyCode !== 13) {
                    $("#tbSearchText").val("");
                    $("#tbObjectId").val("");
                    $("#tbPackageId").val("");
                    $("#tbAccountId").val("");
                }
            });

            $("#tbSearch").autocomplete({
                zIndex: 0,
                source: function (request, response) {
                    var payload = {
                        fullType: "TableSearch",
                        FilterValue: request.term,
                        FilterColumns: filterColumns
                    };

                    payload = $.extend(payload, parseExtraAjaxData(extraDataExpression));

                    $.ajax({
                        type: "post",
                        dataType: "json",
                        data: payload,
                        url: "AjaxHandler.ashx",
                        success: function (data) {
                            response($.map(data, function (item) {
                                var type = $('#ddlFilterColumn option[value="' + item.ColumnType + '"]').text();
                                if (type == null || type.length === 0) {
                                    type = item.ColumnType;
                                }
                                $("#ddlFilterColumn :selected").removeAttr("selected");
                                return {
                                    label: item.TextSearch + " [" + type + "]",
                                    code: item
                                };
                            }));
                        }
                    });
                },
                select: function (event, ui) {
                    var item = ui.item;
                    if (item.code.url != null) {
                        window.location.href = item.code.url;
                    } else {
                        $("#ddlFilterColumn").val(item.code.ColumnType);
                        $("#tbSearchText").val(item.code.TextSearch);

                        var submit = document.getElementById(submitId);
                        if (submit && typeof submit.click === "function") {
                            submit.click();
                        } else if (submit) {
                            $(submit).trigger("click");
                        }
                    }
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
