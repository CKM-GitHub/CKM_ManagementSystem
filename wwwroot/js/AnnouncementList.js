document.addEventListener("DOMContentLoaded", function () {

    const viewButtons =
        document.querySelectorAll(".announcement-view-btn");


    viewButtons.forEach(function (button) {

        button.addEventListener("click", async function () {

            const announcementId =
                button.dataset.id;

            if (!announcementId) {
                return;
            }

            try {

                const response =
                    await fetch(
                        `/Announcements/GetAnnouncementDetail?id=${announcementId}`
                    );

                if (!response.ok) {
                    throw new Error(
                        "Failed to load announcement details."
                    );
                }

                const result =
                    await response.json();


                if (!result.success || !result.data) {

                    if (typeof showError === "function") {
                        showError(
                            result.message ||
                            "Announcement details not found."
                        );
                    }

                    return;
                }


                const data = result.data;


                document.getElementById(
                    "detailTitle"
                ).textContent =
                    data.title || "-";


                document.getElementById(
                    "detailAnnouncedBy"
                ).textContent =
                    data.announcedBy || "-";


                document.getElementById(
                    "detailTarget"
                ).textContent =
                    data.target || "-";


                document.getElementById(
                    "detailContent"
                ).textContent =
                    data.content || "-";


                const announcedDate =
                    data.announcedDate
                        ? new Date(data.announcedDate)
                        : null;


                document.getElementById(
                    "detailAnnouncedDate"
                ).textContent =
                    announcedDate
                        ? announcedDate.toLocaleDateString(
                            "en-GB"
                        )
                        : "-";


                const priorityElement =
                    document.getElementById(
                        "detailPriority"
                    );


                priorityElement.className =
                    "priority-badge";


                switch (data.priority) {

                    case 1:

                        priorityElement.textContent =
                            "High";

                        priorityElement.classList.add(
                            "priority-high"
                        );

                        break;


                    case 2:

                        priorityElement.textContent =
                            "Medium";

                        priorityElement.classList.add(
                            "priority-medium"
                        );

                        break;


                    case 3:

                        priorityElement.textContent =
                            "Low";

                        priorityElement.classList.add(
                            "priority-low"
                        );

                        break;


                    default:

                        priorityElement.textContent =
                            "-";

                        break;
                }


                const modalElement =
                    document.getElementById(
                        "announcementDetailsModal"
                    );


                const modal =
                    bootstrap.Modal.getOrCreateInstance(
                        modalElement
                    );


                modal.show();

            }
            catch (error) {

                if (typeof showError === "function") {

                    showError(
                        "Unable to load announcement details."
                    );

                }
                else {

                    console.error(error);

                }
            }

        });

    });
    //Delete//
    const deleteButtons =
        document.querySelectorAll(
            ".announcement-delete-btn"
        );

    deleteButtons.forEach(function (button) {

        button.addEventListener(
            "click",
            function () {

                const announcementId =
                    button.dataset.id;

                if (!announcementId) {
                    return;
                }

                showDelete(
                    "Are you sure you want to delete this announcement?",
                    function () {

                        const tokenInput =
                            document.querySelector(
                                'input[name="__RequestVerificationToken"]'
                            );

                        const token =
                            tokenInput
                                ? tokenInput.value
                                : "";

                        $.ajax({

                            url:
                                "/Announcements/DeleteAnnouncement",

                            type:
                                "POST",

                            data:
                            {
                                id:
                                    announcementId,

                                __RequestVerificationToken:
                                    token
                            },

                            success:
                                function (response) {

                                    if (response.success) {

                                        showSuccess(
                                            response.message ||
                                            "Announcement deleted successfully.",
                                            function () {

                                                location.reload();

                                            }
                                        );

                                    }
                                    else {

                                        showError(
                                            response.message ||
                                            "Failed to delete the announcement."
                                        );

                                    }

                                },

                            error:
                                function () {

                                    showError(
                                        "Unable to delete the announcement."
                                    );

                                }

                        });

                    }
                );

            }

        );

    });

});
