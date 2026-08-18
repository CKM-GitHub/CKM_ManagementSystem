document.addEventListener("DOMContentLoaded", function () {
    var successModal = document.getElementById("successModal");
    var errorModal = document.getElementById("errorModal");

    if (successModal) {
        new bootstrap.Modal(successModal).show();
    }
    if (errorModal) {
        new bootstrap.Modal(errorModal).show();
    }
});