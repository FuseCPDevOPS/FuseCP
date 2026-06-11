// Shared client behavior for FileManager page actions and modal handling.
function checkAll(selectAllCheckbox) {
    // get all checkbox and select it
    $("td :checkbox").prop("checked", selectAllCheckbox.checked);
}

function unCheckSelectAll(selectCheckbox) {
    // if any item is unchecked, uncheck header checkbox as well
    if (!selectCheckbox.checked) {
        $("th :checkbox").prop("checked", false);
    }
}

function HighlightRow(chkB) {
    var xState = chkB.checked;
    var row = chkB.parentNode.parentNode;
    if (xState) {
        row.setAttribute("temp_class", row.className);
        row.className = "SelectedRow";
    } else {
        if (row.getAttribute("temp_class")) {
            row.className = row.getAttribute("temp_class");
        } else {
            row.className = "";
        }
    }
}

function pageLoad(sender, args) {
    if (!args.get_isPartialLoad()) {
        $addHandler(document, "keydown", onKeyDown);
    }
}

function HideAllModalDialogs() {
    if (!(window.Sys && Sys.Application && typeof Sys.Application.getComponents === "function")) {
        return;
    }
    if (!(window.AjaxControlToolkit && AjaxControlToolkit.ModalPopupBehavior)) {
        return;
    }

    var c = Sys.Application.getComponents();
    for (var i = 0; i < c.length; i++) {
        if (AjaxControlToolkit.ModalPopupBehavior.isInstanceOfType(c[i])) {
            c[i].hide();
        }
    }
}

function CloseFileManagerPopup(button) {
    HideAllModalDialogs();

    var popupContainer = null;
    if (button) {
        if (typeof button.closest === "function") {
            popupContainer = button.closest(".PopupContainer");
        } else {
            var node = button;
            while (node && node.className && node.className.indexOf("PopupContainer") < 0) {
                node = node.parentNode;
            }
            popupContainer = node;
        }
    }

    if (popupContainer) {
        popupContainer.style.display = "none";
    }

    return false;
}

function onKeyDown(e) {
    if (e && e.keyCode === Sys.UI.Key.esc) {
        // If the key pressed is escape, dismiss all modal dialogs.
        HideAllModalDialogs();
    }
}

function ShowUnzipFilesDialog() {
    ShowProgressDialog(FM_UNZIP_FILES_MESSAGE);
}

function ShowTextBox(scope) {
    var text;
    var textbox;
    text = $("." + scope + "Text");
    textbox = $("." + scope + "TextBox");
    if (text.is(":visible")) {
        textbox.val(text.text());
    } else {
        text.text(textbox.val());
    }
    text.toggle();
    textbox.toggle();
}

Sys.Application.add_load(modalPopupFocus);