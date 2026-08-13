document.addEventListener("DOMContentLoaded", function () {

    var successModal = document.getElementById('successModal');

    if (successModal) {
        var modal = new bootstrap.Modal(successModal);
        modal.show();
    }

    var errorModal = document.getElementById('errorModal');

    if (errorModal) {
        var modal = new bootstrap.Modal(errorModal);
        modal.show();
    }

});