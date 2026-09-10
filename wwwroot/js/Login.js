$(function () {
    const loginForm = $('#loginForm');
    const passwordInput = $('#passwordInput');
    const toggleIcon = $('#toggleIcon');
    const emailInput = $('#emailInput');
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
    const errorMessage = $('.login-body').data('error');
    if (errorMessage) {
        showError(errorMessage, "Login Failed");
        $('#alertModal').one('hidden.bs.modal', function () {
            passwordInput.attr('type', 'password');
            toggleIcon.removeClass('fa-eye-salash').addClass('fa-eye');
            setTimeout(function () {
                passwordInput.focus();
            }, 100);
        });
    }
    else {
        setTimeout(function () {
            emailInput.focus();
        }, 100);
    }

    const successTitle = $('.login-body').data('success-title') || 'Success!';
    const successMessage = $('.login-body').data('success-message');
    if (successMessage) {
        showSuccess(successMessage, successTitle);
    }
});