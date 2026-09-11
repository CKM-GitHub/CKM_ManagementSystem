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

    if (normalButtons) normalButtons.classList.remove("d-none");
    if (deleteButtons) deleteButtons.classList.add("d-none");

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

        if (normalButtons) normalButtons.classList.add("d-none");
        if (deleteButtons) deleteButtons.classList.remove("d-none");
    }

    const modal = bootstrap.Modal.getOrCreateInstance(alertModal);
    modal.show();
}
/*Success function*/
function showSuccess(message, onOk) {
    showAlert(
        "success",
        "Success!",
        message
    );
    const okButton = document.querySelector("#normalButtons .btn");

    if (okButton) {
        okButton.onclick = function () {
            if (typeof onOk === "function") {
                onOk();
            }
        }
    }
}

function showError(message) {
    showAlert("error", "Error!", message);
}

function showDelete(message, onConfirm) {
    showAlert("delete", "Delete!", message);

const confirmButton = document.getElementById("confirmDeleteButton");
if (confirmButton) {
        $(confirmButton).off('click').one('click', function () {
            if (typeof onConfirm === 'function') {
                onConfirm();
            }
            const alertModal = document.getElementById("alertModal");
            if (alertModal) {
                const modal = bootstrap.Modal.getInstance(alertModal);
                if (modal) modal.hide();
            }
        });
    }
}