// File Upload Preview


const fileInput =
    document.getElementById("fileInput");

const selectedFiles =
    document.getElementById("selectedFiles");

const uploadPlaceholder =
    document.getElementById("uploadPlaceholder");

if (fileInput && selectedFiles && uploadPlaceholder) {

    fileInput.addEventListener("change", function () {

        selectedFiles.innerHTML = "";

        if (this.files.length === 0) {
            uploadPlaceholder.style.display = "block";
            return;
        }

        uploadPlaceholder.style.display = "none";

        Array.from(this.files).forEach(function (file) {

            const fileCard =
                document.createElement("div");

            fileCard.classList.add(
                "selected-file-card"
            );

            const fileIcon =
                document.createElement("div");

            fileIcon.classList.add(
                "selected-file-icon"
            );

            fileIcon.innerHTML =
                '<i class="fa-solid fa-file"></i>';

            const fileName =
                document.createElement("div");

            fileName.classList.add(
                "selected-file-name"
            );

            fileName.textContent =
                file.name;

            fileCard.appendChild(fileIcon);
            fileCard.appendChild(fileName);

            selectedFiles.appendChild(fileCard);
        });
    });
}


// Reusable Custom Dropdown

function setupTaskDropdown(
    buttonId,
    menuId,
    valueId,
    textId,
    onChange = null
) {
    const button =
        document.getElementById(buttonId);

    const menu =
        document.getElementById(menuId);

    const valueInput =
        document.getElementById(valueId);

    const textElement =
        document.getElementById(textId);

    if (!button ||
        !menu ||
        !valueInput ||
        !textElement) {

        return;
    }


    // Open / close dropdown
    button.addEventListener(
        "click",
        function (event) {

            event.stopPropagation();

            document
                .querySelectorAll(
                    ".task-custom-select-menu.show"
                )
                .forEach(function (openMenu) {

                    if (openMenu !== menu) {
                        openMenu.classList.remove(
                            "show"
                        );
                    }
                });

            menu.classList.toggle("show");
        }
    );


    // Event delegation
    // Works for both static and dynamically loaded options
    menu.addEventListener(
        "click",
        function (event) {

            const option =
                event.target.closest(
                    ".task-custom-select-option"
                );

            if (!option) {
                return;
            }

            valueInput.value =
                option.dataset.value;

            textElement.textContent =
                option.textContent.trim();

            menu.classList.remove("show");


            // Trigger normal change event
            valueInput.dispatchEvent(
                new Event(
                    "change",
                    {
                        bubbles: true
                    }
                )
            );


            if (typeof onChange === "function") {
                onChange(
                    option.dataset.value
                );
            }
        }
    );
}


// Assignee Elements

const assigneeMenu =
    document.getElementById("assigneeMenu");

const assigneeValue =
    document.getElementById("assigneeValue");

const assigneeText =
    document.getElementById("assigneeText");



// Load Assignees by Project

async function loadAssignees(projectCode) {

    if (!assigneeMenu ||
        !assigneeValue ||
        !assigneeText) {

        return;
    }


    // Reset current assignee
    assigneeMenu.innerHTML = "";

    assigneeValue.value = "";

    assigneeText.textContent =
        "Select Assignee";


    if (!projectCode) {
        return;
    }


    try {

        const response =
            await fetch(
                `/Tasks/GetAssignees?projectCode=${encodeURIComponent(projectCode)}`
            );


        if (!response.ok) {

            console.error(
                "Failed to load assignees."
            );

            return;
        }


        const assignees =
            await response.json();


        assignees.forEach(
            function (item) {

                const option =
                    document.createElement(
                        "button"
                    );

                option.type = "button";

                option.className =
                    "task-custom-select-option";

                option.dataset.value =
                    item.value;

                option.textContent =
                    item.text;

                assigneeMenu.appendChild(
                    option
                );
            }
        );

    } catch (error) {

        console.error(
            "Error loading assignees:",
            error
        );
    }
}




// Project
setupTaskDropdown(
    "projectButton",
    "projectMenu",
    "projectValue",
    "projectText",
    function (projectCode) {

        loadAssignees(projectCode);
    }
);


// Assignee
setupTaskDropdown(
    "assigneeButton",
    "assigneeMenu",
    "assigneeValue",
    "assigneeText"
);


// Priority
setupTaskDropdown(
    "priorityButton",
    "priorityMenu",
    "priorityValue",
    "priorityText"
);


// Status
setupTaskDropdown(
    "statusButton",
    "statusMenu",
    "statusValue",
    "statusText"
);



// Close Dropdown When Clicking Outside


document.addEventListener(
    "click",
    function () {

        document
            .querySelectorAll(
                ".task-custom-select-menu.show"
            )
            .forEach(function (menu) {

                menu.classList.remove(
                    "show"
                );
            });
    }
);



// Success / Error Alert


document.addEventListener(
    "DOMContentLoaded",
    function () {

        const successMessage =
            document.getElementById(
                "successMessage"
            );

        const errorMessage =
            document.getElementById(
                "errorMessage"
            );


        if (
            successMessage &&
            typeof showSuccess === "function"
        ) {

            showSuccess(
                successMessage.value
            );
        }


        if (
            errorMessage &&
            typeof showError === "function"
        ) {

            showError(
                errorMessage.value
            );
        }
    }
);



// Clear Button

const clearButton =
    document.querySelector(
        ".btn-cancel"
    );


if (clearButton) {

    clearButton.addEventListener(
        "click",
        function () {

            // Clear files
            if (fileInput) {
                fileInput.value = "";
            }

            if (selectedFiles) {
                selectedFiles.innerHTML = "";
            }

            if (uploadPlaceholder) {
                uploadPlaceholder.style.display =
                    "block";
            }


            // Clear Project
            const projectValue =
                document.getElementById(
                    "projectValue"
                );

            const projectText =
                document.getElementById(
                    "projectText"
                );

            if (projectValue) {
                projectValue.value = "";
            }

            if (projectText) {
                projectText.textContent =
                    "Select Project";
            }


            // Clear Assignee
            if (assigneeValue) {
                assigneeValue.value = "";
            }

            if (assigneeText) {
                assigneeText.textContent =
                    "Select Assignee";
            }

            if (assigneeMenu) {
                assigneeMenu.innerHTML = "";
            }


            // Clear Priority
            const priorityValue =
                document.getElementById(
                    "priorityValue"
                );

            const priorityText =
                document.getElementById(
                    "priorityText"
                );

            if (priorityValue) {
                priorityValue.value = "";
            }

            if (priorityText) {
                priorityText.textContent =
                    "Select Priority";
            }


            // Clear Status
            const statusValue =
                document.getElementById(
                    "statusValue"
                );

            const statusText =
                document.getElementById(
                    "statusText"
                );

            if (statusValue) {
                statusValue.value = "";
            }

            if (statusText) {
                statusText.textContent =
                    "Select Status";
            }


            // Close open dropdowns
            document
                .querySelectorAll(
                    ".task-custom-select-menu.show"
                )
                .forEach(function (menu) {

                    menu.classList.remove(
                        "show"
                    );
                });
        }
    );
}