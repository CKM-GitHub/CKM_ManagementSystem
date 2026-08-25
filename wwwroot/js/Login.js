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
        Swal.fire({
            icon: 'error',
            title: 'Login Failed',
            text: errorMessage,
            confirmButtonText: 'Try Again',
            confirmButtonColor: '#0066ff',
            customClass: {
                popup: 'custom-swal-popup',
                title: 'custom-swal-title',
                htmlContainer: 'custom-swal-text',
                confirmButton: 'custom-swal-button'
            }
        }).then(() => {
            passwordInput.attr('type', 'password');
            toggleIcon
                .removeClass('fa-eye-slash')
                .addClass('fa-eye');
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

    const successTitle = $('.login-body').data('success-title');
    const successMessage = $('.login-body').data('success-message');
    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: successTitle,
            text: successMessage,
            confirmButtonText: 'OK',
            confirmButtonColor: '#0066ff',
            customClass: {
                popup: 'custom-swal-popup',
                title: 'custom-swal-title',
                htmlContainer: 'custom-swal-text',
                confirmButton: 'custom-swal-button'
            }
        });
    }
});