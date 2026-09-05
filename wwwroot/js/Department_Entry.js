document.addEventListener("DOMContentLoaded", function () {

    const departmentCode =
        document.getElementById("departmentCode");

    const departmentName =
        document.querySelector("[name='DepartmentName']");

    const form =
        document.getElementById("departmentEntryForm");

    const clearButton =
        document.getElementById("btnClear");


    const isEditMode =
        departmentCode &&
        departmentCode.hasAttribute("readonly");


    if (isEditMode) {

        if (departmentName) {
            departmentName.focus();
        }

    }
    else {

        if (departmentCode) {

            departmentCode.focus();

            departmentCode.addEventListener(
                "keydown",
                function (event) {

                    if (event.key === "Enter") {

                        event.preventDefault();

                        if (departmentName) {
                            departmentName.focus();
                        }
                    }
                }
            );
        }

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
        const successMessage = document.getElementById("successMessage");

        if (successMessage) {
            showSuccess(successMessage.value);

            const alertModal = document.getElementById("alertModal");

            if (alertModal) {
                alertModal.addEventListener(
                    "hidden.bs.modal",
                    function () {
                        departmentCode.focus();
                    }
                );
            }
        }
    });