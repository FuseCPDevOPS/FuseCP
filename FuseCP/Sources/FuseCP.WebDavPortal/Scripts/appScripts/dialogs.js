function ScpDialogs() {
    this.settings = {
        dialogId: "#confirm-dialog",
        processDialogId: "#processDialog",
        inlineProcessDialog: '.small-processing'
    };
}

ScpDialogs.prototype =
{
    showConfirmDialog: function(title, content, positiveButton, positiveClickFunction, dialogId) {
        dialogId = dialogId || this.settings.dialogId;

        //title replace
        if (title) {
            $(dialogId).find('.modal-title').empty();
            $(dialogId).find('.modal-title').text(title);
        }

        //body replace
        $(dialogId).find('.modal-body').empty();
        $(dialogId).find('.modal-body').html(content);

        //positive button replace
        if (positiveButton) {
            $(dialogId).find('.modal-footer .positive-button').empty();
            $(dialogId).find('.modal-footer .positive-button').text(positiveButton);
        }

        //binding click event
        $(dialogId).find('.modal-footer .positive-button').unbind('click');
        $(dialogId).find('.modal-footer .positive-button').click(positiveClickFunction);

        bootstrap.Modal.getOrCreateInstance($(dialogId).get(0)).show();
    },

    showProcessDialog: function() {
        bootstrap.Modal.getOrCreateInstance($(this.settings.processDialogId).get(0)).show();
    },

    hideProcessDialog: function() {
        bootstrap.Modal.getOrCreateInstance($(this.settings.processDialogId).get(0)).hide();
    },

    showInlineProcessing: function(itemId) {
        $(itemId).parent().find(this.settings.inlineProcessDialog).show();
    },

    hideInlineProcessing: function (itemId) {
        $(itemId).parent().find(this.settings.inlineProcessDialog).hide();
    }
};

