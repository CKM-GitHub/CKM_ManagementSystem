$(function () {
    $('.password-toggle-custom').on('click', function () {
        const passwordInput = $('#passwordInput');
        const toggleIcon = $('#toggleIcon');

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
            $('#Email').val('');
            $('#passwordInput').val('');
            $('#passwordInput').attr('type', 'password');
            $('#toggleIcon')
                .removeClass('fa-eye-slash')
                .addClass('fa-eye');
        });
    }
});