function showAlert(type, title, message) {
    const alertModal = document.getElementById("alertModal");
    const alertIcon = document.getElementById("alertIcon");
    const alertTitle = document.getElementById("alertTitle");
    const alertMessage = document.getElementById("alertMessage");
    const normalButtons = document.getElementById("normalButtons");
    const deleteButtons = document.getElementById("deleteButtons");

    if (!alertModal) return;

   
    const defaultTitle = type === "success" ? "Success!" : (type === "error" ? "Error!" : "Confirm Delete");
    const defaultMessage = type === "success" ? "Operation completed successfully." : "Something went wrong.";

    alertTitle.innerText = (title && title.trim() !== "") ? title : defaultTitle;
    alertMessage.innerText = (message && message.trim() !== "") ? message : defaultMessage;

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
        alertIcon.classList.add("alert-delete");

        normalButtons.classList.add("d-none");
        deleteButtons.classList.remove("d-none");
    }

    const modal = bootstrap.Modal.getOrCreateInstance(alertModal);
    modal.show();
}


function showSuccess(message) {
    showAlert("success", "Success!", message);
}


function showError(message) {
    showAlert("error", "Error!", message);
}


function showDelete(message, onConfirm) {
    showAlert("delete", "Delete!", message);

    const confirmButton = document.getElementById("confirmDeleteButton");
    if (confirmButton) {
        confirmButton.onclick = function () {
            onConfirm();
        };
    }
}