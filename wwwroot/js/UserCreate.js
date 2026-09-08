const imageUpload = document.getElementById("image-upload");
const avatarPreview = document.getElementById("avatar-preview");
const defaultIcon = document.getElementById("default-icon");

let currentObjectUrl = null;

if (imageUpload && avatarPreview && defaultIcon) {
    imageUpload.addEventListener("change", function () {
        const file = this.files[0];

        if (currentObjectUrl) {
            URL.revokeObjectURL(currentObjectUrl);
            currentObjectUrl = null;
        }

        if (file) {
            currentObjectUrl = URL.createObjectURL(file);
            avatarPreview.src = currentObjectUrl;
            avatarPreview.classList.remove("d-none");
            defaultIcon.classList.add("d-none");
        } else {
            avatarPreview.src = "";
            avatarPreview.classList.add("d-none");
            defaultIcon.classList.remove("d-none");
        }
    });
}

document.querySelectorAll("input").forEach((input, index, inputs) => {
    input.addEventListener("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            const nextInput = inputs[index + 1];

            if (nextInput) {
                nextInput.focus();
            }
        }
    });
});

const clearButton = document.getElementById("clearBtn");

if (clearButton) {
    clearButton.addEventListener("click", function () {
        const form = document.getElementById("UserForm");
        if (!form) return;

        form.reset();

        form.querySelectorAll(
            'input[type="text"], input[type="email"], input[type="password"]'
        ).forEach(input => {
            input.value = "";
        });

        form.querySelectorAll("select").forEach(select => {
            select.selectedIndex = 0;
        });

        if (typeof $ !== "undefined" && $.fn && $.fn.validate) {
            $(form).validate().resetForm();
        }

        form.querySelectorAll("span[data-valmsg-for]").forEach(span => {
            span.textContent = "";
            span.classList.remove("field-validation-error");
            span.classList.add("field-validation-valid");
        });

        form.querySelectorAll(".input-validation-error").forEach(input => {
            input.classList.remove("input-validation-error");
        });

        const genderMale = document.getElementById("genderMale");
        const genderFemale = document.getElementById("genderFemale");
        if (genderMale) genderMale.checked = true;
        if (genderFemale) genderFemale.checked = false;

        const statusActive = document.getElementById("statusActive");
        const statusInactive = document.getElementById("statusInactive");
        if (statusActive) statusActive.checked = true;
        if (statusInactive) statusInactive.checked = false;

        const termsCheckbox = form.querySelector('input[name="AcceptTerms"]');
        if (termsCheckbox) {
            termsCheckbox.checked = false;

            const termsError = document.getElementById("acceptTermsError");

            if (termsError) {
                termsError.textContent = "";
            }
            termsCheckbox.classList.remove("is-invalid");
        }

        if (imageUpload) {
            imageUpload.value = "";
        }

        if (currentObjectUrl) {
            URL.revokeObjectURL(currentObjectUrl);
            currentObjectUrl = null;
        }

        if (avatarPreview) {
            avatarPreview.src = "";
            avatarPreview.classList.add("d-none");
        }

        if (defaultIcon) {
            defaultIcon.classList.remove("d-none");
        }

        const tempImage = document.getElementById("TempImageName");
        if (tempImage) {
            tempImage.value = "";
        }

        const firstInput = form.querySelector(
            'input[type="text"], input[type="email"], input[type="password"]'
        );
        if (firstInput) {
            firstInput.focus();
        }
    });
}

document.addEventListener("DOMContentLoaded", function () {
    const successMessage = document.getElementById("successMessage");

    if (successMessage && typeof showSuccess === "function") {
        showSuccess(successMessage.value);

        const alertModal = document.getElementById("alertModal");
        const firstInput = document.querySelector(
            "#UserForm input:not([type='hidden'])"
        );

        if (alertModal && firstInput) {
            alertModal.addEventListener(
                "hidden.bs.modal",
                function () {
                    firstInput.focus();
                },
                { once: true }
            );
        }
    }

    const passwordInput = document.getElementById("Password");
    const confirmPasswordInput = document.getElementById("ConfirmPassword");
    const confirmPasswordMessage = document.querySelector(
        '[data-valmsg-for="ConfirmPassword"]'
    );

    if (!passwordInput || !confirmPasswordInput || !confirmPasswordMessage) {
        return;
    }

    function setValidationMessage(message) {
        confirmPasswordMessage.textContent = message;
        confirmPasswordMessage.classList.remove("field-validation-valid");
        confirmPasswordMessage.classList.add("field-validation-error");
        confirmPasswordInput.classList.add("input-validation-error");
    }

    function clearValidationMessage() {
        confirmPasswordMessage.textContent = "";
        confirmPasswordMessage.classList.remove("field-validation-error");
        confirmPasswordMessage.classList.add("field-validation-valid");
        confirmPasswordInput.classList.remove("input-validation-error");
    }

    function validatePasswordMatch() {
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;

        if (password.length === 0 || confirmPassword.length === 0) {
            clearValidationMessage();
            return true;
        }

        if (password !== confirmPassword) {
            setValidationMessage(
                "Password and Confirm Password do not match."
            );

            return false;
        }

        clearValidationMessage();
        return true;
    }
    confirmPasswordInput.addEventListener("blur", validatePasswordMatch);

    const form = document.getElementById("UserForm");

    if (form) {
        form.addEventListener("submit", function (event) {
            if (!validatePasswordMatch()) {
                event.preventDefault();
                confirmPasswordInput.focus();
            }
        });
    }
});


const form = document.getElementById("UserForm");
const acceptTerms = document.getElementById("AcceptTerms");
const acceptTermError = document.getElementById("acceptTermsError");

form.addEventListener("submit", function (event) {
    if (!acceptTerms.checked) {
        event.preventDefault();

        acceptTermsError.textContent =
            "You must agree to the Terms of Service and Privacy Policy.";

        acceptTerms.classList.add("is-invalid");

        return;
    }
    acceptTermsError.textContent = "";
    acceptTerms.classList.remove("is-invalid");
});

acceptTerms.addEventListener("change", function () {

    if (acceptTerms.checked) {
        acceptTermsError.textContent = "";
        acceptTerms.classList.remove("is-invalid");
    }
});