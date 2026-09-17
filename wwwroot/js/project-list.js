document.addEventListener(
    "DOMContentLoaded",
    function () {

        const searchInput =
            document.getElementById(
                "searchKeyword"
            );

        const searchForm =
            document.getElementById(
                "projectSearchForm"
            );

        const deleteButtons =
            document.querySelectorAll(
                ".project-delete-btn"
            );

        if (searchInput) {
            searchInput.focus();
        }

        if (
            searchForm &&
            searchInput
        ) {
            searchForm.addEventListener(
                "submit",
                function () {
                    searchInput.value =
                        searchInput.value.trim();
                }
            );
        }

        deleteButtons.forEach(
            function (button) {

                button.addEventListener(
                    "click",
                    function () {

                        const projectCode =
                            button.dataset.projectCode;

                        const deleteUrl =
                            "/Project/DeleteProject";

                        if (
                            !projectCode ||
                            !deleteUrl
                        ) {
                            return;
                        }

                        const deleteMessage =
                            "Are you sure you want to delete this project?";

                        showDelete(
                            deleteMessage,
                            function () {

                                const tokenInput =
                                    document.querySelector(
                                        'input[name="__RequestVerificationToken"]'
                                    );

                                const token =
                                    tokenInput
                                        ? tokenInput.value
                                        : "";

                                setTimeout(
                                    function () {

                                        $.ajax({
                                            url:
                                                deleteUrl,

                                            type:
                                                "POST",

                                            data:
                                            {
                                                projectCode:
                                                    projectCode,

                                                __RequestVerificationToken:
                                                    token
                                            },

                                            success:
                                                function (res) {

                                                    if (
                                                        res.success
                                                    ) {
                                                        showSuccess(
                                                            res.message ||
                                                            "Project deleted successfully.",
                                                            function () {
                                                                location.reload();
                                                            }
                                                        );
                                                    }
                                                    else {
                                                        showError(
                                                            res.message ||
                                                            "Failed to delete the project."
                                                        );
                                                    }
                                                },

                                            error:
                                                function (
                                                    xhr,
                                                    status,
                                                    error
                                                ) {
                                                    let errMsg =
                                                        "A system error occurred: "
                                                        + error;

                                                    if (
                                                        xhr.responseJSON &&
                                                        xhr.responseJSON.message
                                                    ) {
                                                        errMsg =
                                                            xhr.responseJSON.message;
                                                    }

                                                    showError(
                                                        errMsg
                                                    );
                                                }
                                        });

                                    },
                                    350
                                );
                            }
                        );
                    }
                );
            }
        );
    }
);