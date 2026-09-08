function showAlert(type, title, message) {
    const alertModal = document.getElementById("alertModal");
    const alertIcon = document.getElementById("alertIcon");
    const alertTitle = document.getElementById("alertTitle");
    const alertMessage = document.getElementById("alertMessage");
    const normalButtons = document.getElementById("normalButtons");
    const deleteButtons = document.getElementById("deleteButtons");

    alertTitle.textContent = title;
    alertMessage.textContent = message;

    alertIcon.className = "alert-icon";

    normalButtons.classList.remove("d-none");
    deleteButtons.classList.add("d-none");

    if (type === "success") {
        alertIcon.innerHTML = '<i class="bi bi-check-lg"></i>';

        alertIcon.classList.add("alert-success");
    }
    else if (type === "error") {
        alertIcon.innerHTML = '<i class="bi bi-x-lg"></i>';

        alertIcon.classList.add("alert-error");
    }
    else if (type === "delete") {
        alertIcon.innerHTML = '<i class="bi bi-trash"></i>';

        alertIcon.classList.add("alert-delete")

        normalButtons.classList.add("d-none");
        deleteButtons.classList.remove("d-none");

    }

    const modal = new bootstrap.Modal(alertModal);

    modal.show();
}
/*Success function*/
function showSuccess(message){
    showAlert(
        "success",
        "Success!",
        message
    );
}

/*Error function*/
function showError(message) {
    showAlert(
        "error",
        "Error!",
        message
    );
}

/*Delete function*/
function showDelete(message, onConfirm) {
    showAlert(
        "delete",
        "Delete!",
        message
    );

    const confirmButton = document.getElementById("confirmDeleteButton");

    confirmButton.onclick = function () {
        onConfirm();
    };
}