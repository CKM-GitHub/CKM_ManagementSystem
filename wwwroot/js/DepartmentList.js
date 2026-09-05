document.addEventListener("DOMContentLoaded", function () {

    const deleteButtons =
        document.querySelectorAll(".delete-icon");

    const deleteCodeInput =
        document.getElementById("deleteDepartmentCode");

    const deleteForm =
        document.getElementById("deleteDepartmentForm");

    if (!deleteCodeInput || !deleteForm) {
        return;
    }

    deleteButtons.forEach(function (button) {

        button.addEventListener("click", function () {

            const departmentCode =
                button.dataset.departmentCode;

            if (!departmentCode) {
                return;
            }

            showDelete(
                "Are you sure you want to delete this department?",
                function () {

                    deleteCodeInput.value =
                        departmentCode;

                    deleteForm.submit();
                }
            );

        });

    });


    const successMessage =
        document.getElementById("successMessage");

    if (successMessage) {

        showSuccess(
            successMessage.value
        );
    }


    const errorMessage =
        document.getElementById("errorMessage");

    if (errorMessage) {

        showError(
            errorMessage.value
        );
    }

});