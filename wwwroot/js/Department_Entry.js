document.addEventListener("DOMContentLoaded", function () {

    const departmentCode = document.getElementById("departmentCode");
    const form = document.getElementById("departmentEntryForm");
    const clearButton = document.getElementById("btnClear");

    if (departmentCode) {
        departmentCode.focus();
    }

    if (clearButton && form) {
        clearButton.addEventListener("click", function () {

            form.reset();

            document.querySelector("[name='DepartmentCode']").value = "";
            document.querySelector("[name='DepartmentName']").value = "";
            document.querySelector("[name='Description']").value = "";

            const activeRadio = document.querySelector(
                "[name='Status'][value='true']"
            );

            if (activeRadio) {
                activeRadio.checked = true;
            }

            document.querySelectorAll(".validation-message")
                .forEach(s => s.textContent = "");

            const summary = document.querySelector(".validation-summary");

            if (summary) {
                summary.innerHTML = "";
            }

            departmentCode.focus();
        });
    }
    const successModalElement =
        document.getElementById("successModal");

    if (successModalElement) {
        const successModal =
            new bootstrap.Modal(successModalElement);

        successModal.show();
    }
});