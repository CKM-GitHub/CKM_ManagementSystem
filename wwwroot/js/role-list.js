document.addEventListener("DOMContentLoaded", function () {
    const deleteButtons = document.querySelectorAll(".delete-role-btn");

    deleteButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            const roleCode = button.dataset.roleCode;
            const deleteUrl = button.dataset.deleteUrl;

            if (!roleCode || !deleteUrl) {
                return;
            }

            const deleteMessage = "Are you sure you want to delete this role?";

            showDelete(deleteMessage, function () {
                const tokenInput = document.querySelector(
                    'input[name="__RequestVerificationToken"]'
                );

                const token = tokenInput ? tokenInput.value : "";

                $("#alertModal").modal("hide");

                setTimeout(function () {
                    $.ajax({
                        url: deleteUrl,
                        type: "POST",
                        data: {
                            roleCode: roleCode,
                            __RequestVerificationToken: token
                        },
                        success: function (res) {
                            if (res.success) {
                                showSuccess(
                                    res.message ||
                                    "Role has been deleted successfully."
                                );

                                $("#alertModal").one(
                                    "hidden.bs.modal",
                                    function () {
                                        location.reload();
                                    }
                                );
                            } else {
                                showError(
                                    res.message ||
                                    "Failed to delete the role."
                                );
                            }
                        },
                        error: function (xhr, status, error) {
                            let errMsg =
                                "A system error occurred: " + error;

                            if (
                                xhr.responseJSON &&
                                xhr.responseJSON.message
                            ) {
                                errMsg = xhr.responseJSON.message;
                            }

                            showError(errMsg);
                        }
                    });
                }, 350);
            });
        });
    });
});