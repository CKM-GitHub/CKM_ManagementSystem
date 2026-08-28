<<<<<<< HEAD
﻿document.addEventListener("DOMContentLoaded", function () {

    const departmentCode = document.getElementById("departmentCode");
    const form = document.getElementById("departmentEntryForm");
    const clearButton = document.getElementById("btnClear");
    const departmentName = document.getElementById("DepartmentName");
    const originalStatus =document.querySelector("[name='Status']:checked")?.value;

    if (departmentCode && departmentName) {
        if (departmentCode.readOnly) {
            departmentName.focus();
        } else {
            departmentCode.focus();
        }
    }

    if (clearButton && form) {
        clearButton.addEventListener("click", function () {

            const isEditMode = departmentCode.readOnly;

            if (!isEditMode) {
                departmentCode.value = "";
            }
            departmentName.value = "";

            const description = document.querySelector("[name='Description']");

            if (description) {
                description.value = "";
            }
            if (originalStatus) {
                const originalStatusRadio = document.querySelector(
                    `[name='Status'][value='${originalStatus}']`
                );

                if (originalStatusRadio) {
                    originalStatusRadio.checked = true;
                }
            }

            document.querySelectorAll(".validation-message").forEach(function (message) {
                message.textContent = "";
            });

            const summary = document.querySelector(".validation-summary");

            if (summary) {
                summary.innerHTML = "";
            }

            if (isEditMode) {
                departmentName.focus();
            } else {
                departmentCode.focus();
            }
        });
    }
    const successModalElement =
        document.getElementById("successModal");
=======
﻿    document.addEventListener("DOMContentLoaded", function () {

        const departmentCode = document.getElementById("departmentCode");
        const departmentName = document.querySelector("[name='DepartmentName']");
        const form = document.getElementById("departmentEntryForm");
        const clearButton = document.getElementById("btnClear");
        if (departmentCode) {
            departmentCode.focus();
>>>>>>> origin/main

            departmentCode.addEventListener("keydown", function (event) {

<<<<<<< HEAD
        successModalElement.addEventListener("hidden.bs.modal", function () {
            if (departmentCode.readOnly) {
                departmentName.focus();
            } else {
                departmentCode.focus();
            }

        });
=======
                if (event.key === "Enter") {
                    event.preventDefault();
>>>>>>> origin/main

                    if (departmentName) {
                        departmentName.focus();
                    }
                }
            });
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

            successModalElement.addEventListener("hidden.bs.modal", function () {
                departmentCode.focus();
            });

            successModal.show();
        }
    });