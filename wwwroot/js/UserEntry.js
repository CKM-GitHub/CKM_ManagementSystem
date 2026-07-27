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

const checkbok = document.getElementById("termsCheck");
const submitBtn = document.getElementById("submitBtn");

checkbok.addEventListener("change",
    function () {
        submitBtn.disabled = !this.checked;
    }
);