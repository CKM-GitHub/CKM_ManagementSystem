const imageUpload = document.getElementById("image-upload");       // Drr ka Image preview 
const avatarPreview = document.getElementById("avatar-preview");
const defaultIcon = document.getElementById("default-icon");

imageUpload.addEventListener("change", function () {
    const file = this.files[0];

    if (file) {
        avatarPreview.src = URL.createObjectURL(file);
        avatarPreview.classList.remove("d-none");
        defaultIcon.classList.add("d-none");
    } else {
        avatarPreview.classList.add("d-none");
        defaultIcon.classList.remove("d-none");
    }
});

document.getElementById("clearBtn").addEventListener("click", function () {

    const form = document.getElementById("UserForm");
    const imageUpload = document.getElementById("image-upload");
    const preview = document.getElementById("avatar-preview");
    const defaultIcon = document.getElementById("default-icon");

    form.reset();

    form.querySelectorAll('input[type="text"], input[type="email"], input[type="password"]').forEach(input => { input.value = "";});

    form.querySelectorAll("select").forEach(select => {
        select.selectedIndex = 0;
    });

    $(form).validate().resetForm();

    form.querySelectorAll("span[data-valmsg-for]").forEach(span => {
        span.textContent = "";
        span.classList.remove("field-validation-error");
        span.classList.add("field-validation-valid");
    });

    form.querySelectorAll(".input-validation-error").forEach(input => {input.classList.remove("input-validation-error");});

    document.getElementById("genderMale").checked = true;
    document.getElementById("genderFemale").checked = false;

    document.getElementById("statusActive").checked = true;
    document.getElementById("statusInactive").checked = false;

    const termsCheckbox = form.querySelector(
        'input[name="AcceptTerms"]'
    );

    if (termsCheckbox) {
        termsCheckbox.checked = false;
    }

    if (imageUpload) {
        imageUpload.value = "";
    }

    if (preview) {
        preview.src = "";
        preview.classList.add("d-none");
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

document.addEventListener("DOMContentLoaded", function () {

    var successModal = document.getElementById('successModal');

    if (successModal) {
        var modal = new bootstrap.Modal(successModal);
        modal.show();
    }
});