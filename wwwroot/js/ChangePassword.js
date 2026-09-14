document.addEventListener("DOMContentLoaded", function () {
    const successMessage = document.getElementById("successMessage");
    const errorMessage = document.getElementById("errorMessage");
    const logoutForm = document.getElementById("logoutForm");

    if (successMessage && typeof showSuccess === "function") {
        showSuccess(successMessage.value, function () {
            if (logoutForm) {
                logoutForm.submit();
            }
        });
    }

    if (errorMessage && typeof showError === "function") {
        showError(errorMessage.value);
    }

    const inputs = document.querySelectorAll(
        ".change-password-wrapper input"
    );

    inputs.forEach(function (input, index) {
        input.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                event.preventDefault();

                const nextInput = inputs[index + 1];

                if (nextInput) {
                    nextInput.focus();
                }
                else {
                    document.querySelector(".btn-save").focus();
                }
            }
        });
    });
});