document.addEventListener("DOMContentLoaded", function () {

    const departmentCode =
        document.getElementById("departmentCode");

    const departmentName =
        document.getElementById("DepartmentName");

    const form =
        document.getElementById("departmentEntryForm");

    const clearButton =
        document.getElementById("btnClear");


    // Initial focus
    if (departmentCode && departmentName) {

        if (departmentCode.readOnly) {
            departmentName.focus();
        } else {
            departmentCode.focus();
        }
    }


    if (departmentCode &&
        departmentName &&
        !departmentCode.readOnly) {

        departmentCode.addEventListener(
            "keydown",
            function (event) {

                if (event.key === "Enter") {

                    event.preventDefault();

                    departmentName.focus();
                }
            }
        );
    }


    
    if (clearButton && form) {

        clearButton.addEventListener("click", function () {

            form.reset();

            departmentCode.value = "";
            departmentName.value = "";

            const description =
                document.querySelector("[name='Description']");

            if (description) {
                description.value = "";
            }

            const activeRadio =
                document.querySelector(
                    "[name='Status'][value='true']"
                );

            if (activeRadio) {
                activeRadio.checked = true;
            }

            document
                .querySelectorAll(".validation-message")
                .forEach(function (message) {
                    message.textContent = "";
                });

            const summary =
                document.querySelector(".validation-summary");

            if (summary) {
                summary.innerHTML = "";
            }

            departmentCode.focus();
        });
    }


    // Success Modal
    const successModalElement =
        document.getElementById("successModal");

    if (successModalElement) {

        const successModal =
            new bootstrap.Modal(successModalElement);

        successModalElement.addEventListener(
            "hidden.bs.modal",
            function () {

                if (departmentCode.readOnly) {
                    departmentName.focus();
                } else {
                    departmentCode.focus();
                }
            }
        );

        successModal.show();
    }
});