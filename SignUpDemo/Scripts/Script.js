(function () {
    $('#reset').on('click', function() {
        resetValidationMessages();
        $("#sign-up-form input").not(':hidden').each(function () {
            this.value = null;
        });
        return false;
    });

    $('#sign-up-form').on('submit', function (event) {
        var form = this;

        if (validateForm(form)) {
            event.preventDefault();
        }

        if (event.isDefaultPrevented()) {

            $.ajax({
                type: "POST",
                url: '/Home/Index',
                data: $(form).serialize(),
                success: function (result) {
                    if (result.Status == 200) {
                        $('.toast.success .toast-body').text(result.Message);
                        $('.toast.success').toast("show");
                        $('#reset').click();
                    }
                    else {
                        $('.toast.error .toast-body').text(result.Message);
                        $('.toast.error').toast("show");
                    }
                },
                error: function (xhr) {
                    $('.toast.error .toast-body').text("Error has occurred.");
                    $('.toast.error').toast("show");
                }
            });
        }
    });
})();

function genericValidationFailed() {
    $("#GenEmailErr").show();
    $("#GenPassErr").show();
}

function checkPassword() {
    var passToCheck = $("#Password").val();
    var pattern = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}/;
    if (pattern.test(passToCheck)) {
        return true;
    }
    return false;
}

function checkEmail() {
    var emailToCheck = $("#Email").val();
    var pattern = /^[a-zA-Z0-9+.-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9]{2,63}.*/i;
    if (pattern.test(emailToCheck)) {
        return true;
    }
    return false;
}

function resetValidationMessages() {
    $('#sign-up-form').removeClass('was-validated');
    $("#GenEmailErr").hide();
    $("#InvEmail").hide();
    $("#Email").removeClass("is-invalid");

    $("#GenPassErr").hide();
    $("#InvPass").hide();
    $("#Password").removeClass("is-invalid");
}

function validateForm(form) {

    resetValidationMessages();
    var basicValidationPassed = form.checkValidity();
    if (basicValidationPassed) {
        var validEmail = checkEmail();
        var validPassword = checkPassword();
        var failedValidation = false;
        if (validEmail === false) {
            $("#InvEmail").show();
            $("#Email").addClass("is-invalid");
            failedValidation = true;
        }
        if (validPassword === false) {
            $("#InvPass").show();
            $("#Password").addClass("is-invalid");
            failedValidation = true;
        }
        if (failedValidation) {
            event.preventDefault();
            event.stopPropagation();
            form.classList.remove('was-validated');
            return false;
        }
        else {
            event.preventDefault();
            event.stopPropagation();
            form.classList.add('was-validated');
        }
    }
    else {
        genericValidationFailed();
        event.preventDefault();
        event.stopPropagation();
        form.classList.add('was-validated');
        return false;
    }
    return true;
}
