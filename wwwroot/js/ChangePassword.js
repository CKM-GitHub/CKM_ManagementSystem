document.addEventListener("DOMContentLoaded", function () {
    var successModal = document.getElementById("successModal");
    var errorModal = document.getElementById("errorModal");

    if (successModal) {
        new bootstrap.Modal(successModal).show();
    }
    if (errorModal) {
        new bootstrap.Modal(errorModal).show();
    }

    const inputs = document.querySelectorAll(".change-password-wrapper input").forEach((input, index, inputs) => {
        input.addEventListener("keydown", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();

                const nextInput = inputs[index + 1];

                if (nextInput) {
                    nextInput.focus();
                } else {
                    document.querySelector(".btn-save").focus();
                }
            }
        });
    });
});

