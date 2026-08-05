$(document).ready(function () {
    if (typeof successMessage !== 'undefined' && successMessage !== '') {
        var modalElement = document.getElementById('successModal');

        if (modalElement) {
            var successModal = new bootstrap.Modal(modalElement);
            successModal.show();
        }
    }
});