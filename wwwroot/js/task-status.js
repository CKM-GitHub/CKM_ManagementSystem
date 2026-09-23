document.addEventListener("DOMContentLoaded", function () {
    if (typeof initializeTruncatedTooltips === "function") {
        initializeTruncatedTooltips();
    }
    if (typeof successMessage !== "undefined" && successMessage &&
        successMessage.trim() !== "" && successMessage !== "null") {
        if (typeof showSuccess === "function") {
            showSuccess(successMessage, function () {
                location.reload();
            });
        }
    }
    if (typeof errorMessage !== "undefined" && errorMessage &&
        errorMessage.trim() !== "" && errorMessage !== "null") {
        if (typeof showError === "function") {
            showError(errorMessage);
        }
    }

    const modalElement = document.getElementById("addTaskStatusModal");
    const form = document.getElementById("createTaskStatusForm");
    const btnSave = document.getElementById("btnSaveTaskStatus");
    const addButton = document.querySelector(".add-btn");
    if (!modalElement || !form || !btnSave)
    {
        return;
    }

    const statusCodeInput = form.querySelector('[name="Status_Code"]');
    const statusNameInput = form.querySelector('[name="Status_Name"]');
    const descriptionInput = form.querySelector('[name="Description"]');
    const sortOrderInput = form.querySelector('[name="SortOrder"]');
    const modeInput = form.querySelector('[name="Mode"]');
    const modalTitle = document.getElementById("addTaskStatusModalLabel");
   
    function setCreateMode() {
        if (modeInput) { modeInput.value = "Create"; }
        if (statusCodeInput) { statusCodeInput.readOnly = false; }
        if (modalTitle) { modalTitle.textContent = "Add New Task Status"; }
        btnSave.textContent = "Save Task Status";
    }

    function setEditMode() {
        if (modeInput) { modeInput.value = "Edit"; }
        if (statusCodeInput) { statusCodeInput.readOnly = true; }
        if (modalTitle) { modalTitle.textContent = "Edit Task Status"; }
        btnSave.textContent = "Update Task Status";
    }

    function resetTaskStatusForm() {
        form.reset();
        if (sortOrderInput) {
            sortOrderInput.value = "0";
        }
        hideErrorMessages();
        form.querySelectorAll(".is-invalid").forEach(function (element) {
            element.classList.remove("is-invalid");
        });
    }

    if (addButton) {
        addButton.addEventListener("click", function () {
            resetTaskStatusForm();
            setCreateMode();
        });
    }

    modalElement.addEventListener("shown.bs.modal", function () {
        const firstInput = modalElement.querySelector(
            "input:not([type='hidden']):not([readonly]), select, textarea"
        );
        if (firstInput) {
            focusAndMoveCursorToEnd(firstInput);
        }
    });

    modalElement.addEventListener("hidden.bs.modal", function () {
        resetTaskStatusForm();
        setCreateMode();
    });

    form.querySelectorAll("input, select, textarea").forEach(function (input) {
        input.addEventListener("input", function () {
            hideErrorMessages();
            input.classList.remove("is-invalid");
        });
    });

    form.addEventListener("keydown", function (e) {
        if (e.key !== "Enter") {
            return;
        }
        e.preventDefault();
        const activeElement = document.activeElement;

        if (activeElement === statusCodeInput && statusCodeInput &&
            statusCodeInput.value.trim() === "")
        {
            showErrorMessages("Status Code is required.");
            focusAndMoveCursorToEnd(statusCodeInput);
            return;
        }
        if (activeElement === statusNameInput && statusNameInput &&
            statusNameInput.value.trim() === "") {
            showErrorMessages("Status Name is required.");
            focusAndMoveCursorToEnd(statusNameInput);
            return;
        }

        const focusableElements = Array.from(
            form.querySelectorAll("input:not([type='hidden']), select, textarea"))
                .concat(btnSave)
                .filter(el => !el.disabled);

        const currentIndex = focusableElements.indexOf(activeElement);

        if (currentIndex > -1 &&
            currentIndex < focusableElements.length - 1
        ) {
            focusAndMoveCursorToEnd(
                focusableElements[currentIndex + 1]
            );
            return;
        }
        triggerSaveProcess();
    });

    btnSave.addEventListener("click", function () {
        triggerSaveProcess();
    });

    async function triggerSaveProcess() {
        hideErrorMessages();

        if (statusCodeInput &&
            statusCodeInput.value.trim() === ""
        ) {
            showErrorMessages("Status Code is required.");
            focusAndMoveCursorToEnd(statusCodeInput);
            return;
        }

        if (statusNameInput &&
            statusNameInput.value.trim() === ""
        ) {
            showErrorMessages("Status Name is required.");
            focusAndMoveCursorToEnd(statusNameInput);
            return;
        }

        if (modeInput) {
            if (!modeInput.value) {
                modeInput.value = "Create";
            }

        }

        const formData = new FormData(form);
        try {
            const response = await fetch(
                "/TaskStatuses/TaskStatusesEntry",
                {
                    method: "POST",
                    body: formData
                }
            );
            const result = await response.json();

            if (result.success) {
                const modal = bootstrap.Modal.getInstance(modalElement);
                if (modal) {
                    modal.hide();
                }
                showSuccess(
                    result.message ||
                    "Task Status saved successfully!",
                    function () {
                        location.reload();
                    }
                );
                return;
            }
            showErrorMessages(
                result.message ||
                "Task Status could not be saved."
            );

        }
        catch (error) {
            console.error("Save Error:", error);
            showError("A system error occurred. Please try again.");
        }
    }

    document.addEventListener("click", async function (e) {
        const deleteButton = e.target.closest(".delete-icon");
        if (!deleteButton) {
            return;
        }
        e.preventDefault();
        const deleteForm = deleteButton.closest("form");
        if (!deleteForm) {
            return;
        }

        showDelete("Are you sure you want to delete this status?",
            async function () {
                const formData = new FormData(deleteForm);
                try {
                    const response = await fetch(
                        deleteForm.action,
                        {
                            method: "POST",
                            body: formData
                        });
                    const result = await response.json();
                    if (result.success) {
                        showSuccess(
                            result.message ||
                            "Task status deleted successfully!",
                            function () {
                                location.reload();
                            }
                        );
                    }
                    else {
                        showError(
                            result.message ||
                            "Task Status could not be deleted."
                        );
                    }
                }
                catch (error) {
                    console.error("Delete Error:",error);
                    showError(
                        "A system error occurred. Please try again."
                    );
                }
            }
        );
    });

    document.addEventListener("click", async function (e) {
        const editButton = e.target.closest(".btn-edit");
        if (!editButton) {
            return;
        }
        e.preventDefault();
        const statusCode = editButton.getAttribute("data-code");
        if (!statusCode) {
            showError("Status Code was not found.");
            return;
        }

        try {
            const response =
                await fetch(
                    `/TaskStatuses/GetTaskStatus?statusCode=${encodeURIComponent(statusCode)}`
                );

            const result =await response.json();
            console.log("GetTaskStatus result:",result);

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            if (!result.success) {
                showError(
                    result.message ||
                    "Task Status could not be loaded."
                );
                return;
            }
            hideErrorMessages();
            setEditMode();
            if (statusCodeInput) {
                statusCodeInput.value = result.data.status_Code || "";
            }
            if (statusNameInput) {
                statusNameInput.value = result.data.status_Name || "";
            }
            if (descriptionInput) {
                descriptionInput.value = result.data.description || "";
            }
            if (sortOrderInput) {
                sortOrderInput.value = result.data.sortOrder ?? 0;
            }
            const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
            modal.show();
        }
        catch (error) {
            console.error("Edit Error:", error);
            showError(
                "A system error occurred while loading the task status."
            );
        }
    });
});

function focusAndMoveCursorToEnd(element) {
    if (!element) {
        return;
    }
    element.focus();
    if (
        (element.tagName === "INPUT" ||
        element.tagName === "TEXTAREA") && element.type !== "number"
    ) {
        const length = element.value.length;
        element.setSelectionRange(
            length,
            length
        );
    }
}

function hideErrorMessages() {
    const errCode = document.getElementById("errorStatusCode");
    const errName = document.getElementById("errorStatusName");
    const errSortOrder = document.getElementById("errorSortOrder");

    if (errCode) {
        errCode.classList.add("d-none");
    }
    if (errName) {
        errName.classList.add("d-none");
    }
    if (errSortOrder) {
        errSortOrder.classList.add("d-none");
    }
    const inputCode = document.querySelector('[name="Status_Code"]');
    if (inputCode) {
        inputCode.classList.remove("is-invalid");
    }
    const inputName = document.querySelector('[name="Status_Name"]');
    if (inputName) {
        inputName.classList.remove("is-invalid");
    }
    const inputSortOrder = document.querySelector('[name="SortOrder"]');
    if (inputSortOrder) {
        inputSortOrder.classList.remove("is-invalid");
    }
}

function showErrorMessages(message) {
    if (!message) {
        return;
    }
    const lowerMessage = message.toLowerCase();
    if (lowerMessage.includes("code")) {
        const errCode =
            document.getElementById(
                "errorStatusCode"
            );

        const inputCode =
            document.querySelector(
                '[name="Status_Code"]'
            );
        if (errCode) {
            errCode.textContent = message;
            errCode.classList.remove(
                "d-none"
            );
        }

        if (inputCode) {
            inputCode.classList.add("is-invalid");
            focusAndMoveCursorToEnd(
                inputCode
            );
        }
        return;
    }

    if (lowerMessage.includes("name")) {
        const errName = document.getElementById("errorStatusName");
        const inputName = document.querySelector('[name="Status_Name"]');
        if (errName) {
            errName.textContent = message;
            errName.classList.remove("d-none");
        }
        if (inputName) {
            inputName.classList.add("is-invalid");
            focusAndMoveCursorToEnd(inputName);
        }
        return;
    }
    if (lowerMessage.includes("sort order") || lowerMessage.includes("sortorder")) {
        const errSortOrder = document.getElementById("errorSortOrder");
        const inputSortOrder = document.querySelector('[name="SortOrder"]');
        if (errSortOrder) {
            errSortOrder.textContent = message;
            errSortOrder.classList.remove("d-none");
        }
        if (inputSortOrder) {
            inputSortOrder.classList.add("is-invalid");
            focusAndMoveCursorToEnd(inputSortOrder);
        }
        return;
    }
    if (typeof showError === "function") {
        showError(message);
    }
}