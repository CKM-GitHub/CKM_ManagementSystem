
    document.addEventListener("DOMContentLoaded", function () {

        const deleteButtons =
    document.querySelectorAll(".delete-icon");

    const deleteModalElement =
    document.getElementById("deleteDepartmentModal");

    const deleteCodeInput =
    document.getElementById("deleteDepartmentCode");

    const deleteForm =
    document.getElementById("deleteDepartmentForm");

    const confirmDeleteButton =
    document.getElementById("confirmDeleteDepartment");

    if (!deleteModalElement ||
    !deleteCodeInput ||
    !deleteForm ||
    !confirmDeleteButton) {
            return;
        }

    const deleteModal =
    new bootstrap.Modal(deleteModalElement);

    deleteButtons.forEach(function (button) {

        button.addEventListener("click", function () {

            const departmentCode =
                button.dataset.departmentCode;

            deleteCodeInput.value =
                departmentCode ?? "";

            deleteModal.show();
        });

        });

    confirmDeleteButton.addEventListener(
    "click",
    function () {

                if (!deleteCodeInput.value) {
                    return;
                }

    confirmDeleteButton.disabled = true;
    deleteForm.submit();
        });
        const successModalElement =
            document.getElementById("successModal");

        if (successModalElement) {
            const successModal =
                new bootstrap.Modal(successModalElement);

            successModal.show();
        }
    });
