document.addEventListener("DOMContentLoaded", function () {

    $(function () {
        $('.tooltip-cell').each(function () {
            if (this.scrollWidth > this.clientWidth) {
                var $el = $(this);
                $el.attr('title', $el.data('title'));
                $el.tooltip(); 
            }
        });
    });

    document.querySelectorAll(".delete-icon").forEach(function (button) {
        button.addEventListener("click", function () {
            const deleteForm = button.closest(".delete-form");
            if (!deleteForm) return;

            showDelete(
                "Are you sure you want to delete this task priority?",
                function () {
                    deleteForm.submit();
                }
            );
        });
    });

    const successMsg = document.getElementById("successMessage");
    const errorMsg = document.getElementById("errorMessage");

    if (successMsg && successMsg.value.trim() !== "") {
        showSuccess(successMsg.value);
    }

    if (errorMsg && errorMsg.value.trim() !== "") {
        showError(errorMsg.value);
    }

    const modalEl = document.getElementById("taskPriorityModal");
    const form = document.getElementById("taskPriorityForm");
    const createButton = document.getElementById("createPriorityBtn");
    const modeInput = document.getElementById("Mode");

    if (!modalEl || !form) {
        return; 
    }

    if (createButton) {
        createButton.addEventListener("click", function () {
            form.reset();

            if (modeInput) {
                modeInput.value = "Entry";
            }

            const codeInput = document.getElementById("Code");
            if (codeInput) {
                codeInput.removeAttribute("readonly");
            }

            const sortOrderInput = document.getElementById("SortOrder");
            if (sortOrderInput) {
                sortOrderInput.value = "0";
            }

            form.querySelectorAll(".text-danger").forEach(function (element) {
                element.textContent = "";
            });

            const modalTitle = document.getElementById("createTaskPriorityModalLabel");
            const submitButton = form.querySelector("button[type='submit']");

            if (modalTitle) {
                modalTitle.innerText = "Add New Task Priority";
            }

            if (submitButton) {
                submitButton.innerText = "Save Priority";
            }

            form.action =
                form.getAttribute("data-create-url") ||
                "/TaskPriority/CreatePriority";
        });
    }

    document.querySelectorAll(".update-priority-btn").forEach(function (button) {
        button.addEventListener("click", function (e) {
            e.preventDefault();

            const codeInput = document.getElementById("Code");
            const nameInput = document.getElementById("Name");
            const descInput = document.getElementById("Description");
            const sortInput = document.getElementById("SortOrder");

            if (codeInput) {
                codeInput.value = button.dataset.priorityCode || "";
                codeInput.setAttribute("readonly", "readonly");
            }

            if (nameInput) {
                nameInput.value = button.dataset.priorityName || "";
            }

            if (descInput) {
                descInput.value = button.dataset.description || "";
            }

            if (sortInput) {
                sortInput.value = button.dataset.sortOrder || "0";
            }

            if (modeInput) {
                modeInput.value = "Edit";
            }

            const modalTitle = document.getElementById("createTaskPriorityModalLabel");
            const submitButton = form.querySelector("button[type='submit']");

            if (modalTitle) {
                modalTitle.innerText = "Edit Task Priority";
            }

            if (submitButton) {
                submitButton.innerText = "Update Priority";
            }

            form.action =
                button.dataset.updateUrl ||
                "/TaskPriority/UpdatePriority";
        });
    });

});