var fcp = {
    messages: new ScpMessager('#message-area'),
    fileBrowser: new ScpFileBrowser(),
    dialogs: new ScpDialogs()
};


$(document).on('click', '.processing-dialog', function (e) {
    fcp.dialogs.showProcessDialog();
});


$(document).ready(function () {

    //bootstrap jquery validate styles fix
    BindBootstrapValidationStyles();


    $.validator.addMethod("synchronousRemote", function(value, element, param) {
        if (this.optional(element)) {
            return "dependency-mismatch";
        }

        var previous = this.previousValue(element);
        if (!this.settings.messages[element.name]) {
            this.settings.messages[element.name] = {};
        }
        previous.originalMessage = this.settings.messages[element.name].remote;
        this.settings.messages[element.name].remote = previous.message;

        param = typeof param === "string" && { url: param } || param;

        if (previous.old === value) {
            return previous.valid;
        }

        previous.old = value;
        var validator = this;
        this.startRequest(element);
        var data = {};
        data[element.name] = value;
        var valid = "pending";
        $.ajax($.extend(true, {
            url: param,
            async: false,
            mode: "abort",
            port: "validate" + element.name,
            dataType: "json",
            data: data,
            success: function(response) {
                validator.settings.messages[element.name].remote = previous.originalMessage;
                valid = response === true || response === "true";
                if (valid) {
                    var submitted = validator.formSubmitted;
                    validator.prepareElement(element);
                    validator.formSubmitted = submitted;
                    validator.successList.push(element);
                    delete validator.invalid[element.name];
                    validator.showErrors();
                } else {
                    var errors = {};
                    var message = response || validator.defaultMessage(element, "remote");
                    errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
                    validator.invalid[element.name] = true;
                    validator.showErrors(errors);
                }
                previous.valid = valid;
                validator.stopRequest(element, valid);
            }
        }, param));
        return valid;
    }, "Please fix this field.");
});

function BindBootstrapValidationStyles() {
    $.validator.setDefaults({
        highlight: function (element) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid');
        },
        errorElement: 'span',
        errorClass: 'invalid-feedback',
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    });

    $('.bs-val-styles').each(function () {
        var form = $(this);
        var formData = $.data(form[0]);
        if (formData && formData.validator) {

            var settings = formData.validator.settings;
            // Store existing event handlers in local variables
            var oldErrorPlacement = settings.errorPlacement;
            var oldSuccess = settings.success;

            settings.errorPlacement = function (label, element) {

                oldErrorPlacement(label, element);

                $(element).addClass('is-invalid');
                label.addClass('invalid-feedback');
            };

            settings.success = function (label, element) {
                $(element).removeClass('is-invalid');

                oldSuccess(label);
            }
        }
    });

    $('.input-validation-error').each(function () {
        $(this).addClass('is-invalid');
    });

    $('.field-validation-error').each(function () {
        $(this).addClass('invalid-feedback');
    });
}


$.fn.clearValidation = function () { var v = $(this).validate(); $('[name]', this).each(function () { v.successList.push(this); v.showErrors(); }); v.resetForm(); v.reset(); $(this).find('.is-invalid').removeClass('is-invalid'); };


function isMobileDevice() {
    return (/android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(navigator.userAgent.toLowerCase()));
}

