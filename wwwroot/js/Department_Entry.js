document.addEventListener("DOMContentLoaded", function () {

    const departmentCode = document.getElementById("departmentCode");
    const form = document.getElementById("departmentEntryForm");
    const clearButton = document.getElementById("btnClear");

    if (departmentCode) {
        departmentCode.focus();
    }

    if (clearButton) {
        clearButton.addEventListener("click", function () {

            form.reset();

            document.querySelector("[name='DepartmentCode']").value = "";
            document.querySelector("[name='DepartmentName']").value = "";
            document.querySelector("[name='Description']").value = "";

            document.querySelectorAll("[name='Status']")
                .forEach(r => r.checked = false);

            document.querySelectorAll(".validation-message")
                .forEach(s => s.textContent = "");

            const summary = document.querySelector(".validation-summary");
            if (summary) {
                summary.innerHTML = "";
            }

            departmentCode.focus();
        });
    }

});