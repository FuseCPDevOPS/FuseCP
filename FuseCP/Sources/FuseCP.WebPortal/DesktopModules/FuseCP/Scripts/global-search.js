// Shared GlobalSearch client behavior for autocomplete and quick-submit.
$(document).ready(function () {
    function safeTrim(value) {
        if ($ && typeof $.trim === "function") {
            return $.trim(value || "");
        }

        if (value === null || value === undefined) {
            return "";
        }

        return String(value).replace(/^\s+|\s+$/g, "");
    }

    $(".fcp-global-search").each(function () {
        var $root = $(this);
        var $search = $root.find("input[id$='tbSearch']");
        var $searchColumnType = $root.find("input[id$='tbSearchColumnType']");
        var $searchFullType = $root.find("input[id$='tbSearchFullType']");
        var $searchText = $root.find("input[id$='tbSearchText']");
        var $objectId = $root.find("input[id$='tbObjectId']");
        var $packageId = $root.find("input[id$='tbPackageId']");
        var $accountId = $root.find("input[id$='tbAccountId']");
        var $noResultsText = $root.find("input[id$='hfNoResultsText']");
        var $goToSearchText = $root.find("input[id$='hfGoToSearchText']");
        var $submit = $root.find("a[id$='ImageButton1']");
        var $mobileShell = $root.closest(".search-top");
        var isMobileShell = $mobileShell.length > 0;
        var isSubmitting = false;
        var noResultsText = safeTrim($noResultsText.val() || "");
        var goToSearchText = safeTrim($goToSearchText.val() || "");

        if (!noResultsText) {
            noResultsText = "No results found";
        }

        if (!goToSearchText) {
            goToSearchText = "Go to search page";
        }

        function buildNoResultsItem() {
            return {
                label: noResultsText,
                value: "",
                noResults: true
            };
        }

        function buildGoToSearchItem(term) {
            return {
                label: goToSearchText + ': "' + term + '"',
                value: term,
                goToSearch: true
            };
        }

        function clearSelectedEntityFields() {
            $searchColumnType.val("");
            $searchFullType.val("");
            $objectId.val("");
            $packageId.val("");
            $accountId.val("");
        }

        function getAutocompleteInstance($searchInput) {
            if (!$searchInput || !$searchInput.length || typeof $searchInput.autocomplete !== "function") {
                return null;
            }

            // jQuery UI 1.12+ exposes "instance"; older builds store instance in data().
            try {
                return $searchInput.autocomplete("instance")
                    || $searchInput.data("ui-autocomplete")
                    || $searchInput.data("autocomplete")
                    || null;
            } catch (e) {
                return $searchInput.data("ui-autocomplete")
                    || $searchInput.data("autocomplete")
                    || null;
            }
        }

        function getAutocompleteItemData($searchInput) {
            var instance = getAutocompleteInstance($searchInput);

            if (!instance || !instance.menu || !instance.menu.active || !instance.menu.active.length) {
                return null;
            }

            // jQuery UI stores selected menu metadata under different keys across versions.
            return instance.menu.active.data("item.autocomplete") || instance.menu.active.data("ui-autocomplete-item") || null;
        }

        function submitSearch() {
            if (!$submit.length || isSubmitting) {
                return;
            }

            isSubmitting = true;

            var submitElement = $submit.get(0);
            if (submitElement && typeof submitElement.click === "function") {
                submitElement.click();
            } else {
                $submit.trigger("click");
            }

            $submit.attr("disabled", "disabled");
        }

        function handleSelectedItem(event, item) {
            if (item && item.noResults) {
                if (event && event.preventDefault) {
                    event.preventDefault();
                }
                return false;
            }

            if (item && item.goToSearch) {
                if (event && event.preventDefault) {
                    event.preventDefault();
                }
                clearSelectedEntityFields();
                $searchText.val(safeTrim($search.val() || item.value || ""));
                submitSearch();
                return false;
            }

            var code = item && item.code ? item.code : item;

            if (!code || typeof code !== "object") {
                clearSelectedEntityFields();
                $searchText.val(safeTrim($search.val() || ""));
                submitSearch();
                return false;
            }

            $searchColumnType.val(code.ColumnType || code.columnType || "");
            $searchFullType.val(code.FullType || code.fullType || "");
            $searchText.val(code.TextSearch || code.textSearch || (item && (item.value || item.label)) || safeTrim($search.val() || ""));
            $objectId.val(code.ItemID || code.itemID || code.id || "");
            $packageId.val(code.PackageID || code.packageID || "");
            $accountId.val(code.AccountID || code.accountID || "");

            submitSearch();
            return false;
        }

        function suppressPlainEnter(e) {
            var key = e.which || e.keyCode;
            if (key !== 13) {
                return;
            }

            var instance = getAutocompleteInstance($search);
            var menuVisible = !!(instance && instance.menu && instance.menu.element && instance.menu.element.is(":visible"));
            var activeItem = menuVisible ? getAutocompleteItemData($search) : null;

            // Allow Enter when a real autocomplete item is actively selected.
            if (menuVisible && activeItem && !activeItem.noResults) {
                return;
            }

            // Always block Enter in textbox to prevent default SearchObject redirect.
            // Intentional navigation still works via explicit click/select actions.
            e.preventDefault();
            e.stopPropagation();
            if (e.stopImmediatePropagation) {
                e.stopImmediatePropagation();
            }
            return false;
        }

        $search.on("keydown", suppressPlainEnter);
        $search.on("keypress", suppressPlainEnter);

        if ($search.length === 0) {
            return;
        }

        $search.keypress(function (e) {
            if (e.keyCode !== 13) {
                $searchText.val("");
                $objectId.val("");
                $packageId.val("");
                $accountId.val("");
            }
        });

        $search.autocomplete({
            zIndex: 100,
            appendTo: "body",
            position: {
                my: "left top",
                at: "left bottom+6",
                of: isMobileShell ? $mobileShell.get(0) : $search.get(0),
                within: document.body,
                collision: "fit flip"
            },
            source: function (request, response) {
                $.ajax({
                    type: "post",
                    dataType: "json",
                    data: {
                        term: request.term
                    },
                    url: "AjaxHandler.ashx",
                    success: function (data) {
                        var mapped = $.map(data || [], function (item) {
                            return {
                                label: item.TextSearch + " [" + item.FullTypeLocalized + "]",
                                code: item
                            };
                        });

                        if (!mapped.length) {
                            var term = safeTrim(request.term || "");
                            var noResultsItems = [buildNoResultsItem()];

                            if (term.length > 0) {
                                noResultsItems.push(buildGoToSearchItem(term));
                            }

                            response(noResultsItems);
                            return;
                        }

                        response(mapped);
                    },
                    error: function () {
                        var term = safeTrim(request.term || "");
                        var errorItems = [buildNoResultsItem()];

                        if (term.length > 0) {
                            errorItems.push(buildGoToSearchItem(term));
                        }

                        response(errorItems);
                    }
                });
            },
            focus: function (event, ui) {
                if (ui.item && ui.item.noResults) {
                    event.preventDefault();
                    return false;
                }
            },
            select: function (event, ui) {
                return handleSelectedItem(event, ui && ui.item ? ui.item : null);
            },
            open: function () {
                var widget = $search.autocomplete("widget");
                var useShell = isMobileShell && $mobileShell.is(":visible");
                var width = useShell ? $mobileShell.outerWidth() : $search.outerWidth();

                widget
                    .addClass("fcp-global-search-menu")
                    .css({
                        width: width + "px",
                        zIndex: 2000
                    });

                if (useShell) {
                    widget.position({
                        my: "left top",
                        at: "left bottom+6",
                        of: $mobileShell.get(0),
                        within: document.body,
                        collision: "fit flip"
                    });

                    // Reposition once after the search shell transition settles.
                    setTimeout(function () {
                        widget.css("width", $mobileShell.outerWidth() + "px");
                        widget.position({
                            my: "left top",
                            at: "left bottom+6",
                            of: $mobileShell.get(0),
                            within: document.body,
                            collision: "fit flip"
                        });
                    }, 40);
                }
            }
        });

        var acInstance = getAutocompleteInstance($search);
        if (acInstance) {
            acInstance._renderItem = function (ul, item) {
                var $li = $("<li>")
                    .toggleClass("fcp-no-results", !!item.noResults)
                    .toggleClass("fcp-go-to-search", !!item.goToSearch);
                return $li.append($("<a>").text(item.label || "")).appendTo(ul);
            };
        }

        // Defensive hook for legacy jQuery UI behavior differences.
        // Ensures mouse selection always navigates immediately without Enter.
        $search.on("autocompleteselect", function (event, ui) {
            return handleSelectedItem(event, ui && ui.item ? ui.item : null);
        });

        if (document.referrer.search("pid=Login") > 0 || window.location.href.search("pid=SearchObject") > 0) {
            $search.focus();
        }
    });
});