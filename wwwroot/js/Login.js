$(function () {
    const loginForm = $('#loginForm');
    const passwordInput = $('#passwordInput');
    const toggleIcon = $('#toggleIcon');
    const emailInput = $('#emailInput');

    if ($('.field-validation-error, .validation-summary-errors').length > 0 && emailInput.val()) {
        passwordInput.focus();
    } else {
        setTimeout(function () {
            emailInput.focus();
        }, 100);
    }
    emailInput.on('keydown', function (e) {
        if (e.key == 'Enter') {
            e.preventDefault();
            passwordInput.focus();
        }
    });
    passwordInput.on('keydown', function (e) {
        if (e.key == 'Enter') {
            e.preventDefault();
            loginForm.submit();
        }
    });
    $('.password-toggle-custom').on('click', function () {       
        if (passwordInput.length === 0 || toggleIcon.length === 0) {
            return;
        }
        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
            toggleIcon.removeClass('fa-eye')
            toggleIcon.addClass('fa-eye-slash');
        }
        else {
            passwordInput.attr('type', 'password');
            toggleIcon.removeClass('fa-eye-slash')
            toggleIcon.addClass('fa-eye');
        }
    });

    const successTitle = $('.login-body').data('success-title') || 'Success!';
    const successMessage = $('.login-body').data('success-message');
    if (successMessage) {
        showSuccess(successMessage, successTitle);
    }
});