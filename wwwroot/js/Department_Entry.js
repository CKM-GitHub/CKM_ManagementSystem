document.addEventListener("DOMContentLoaded", function () {

    const departmentCode = document.getElementById("departmentCode");

    if (departmentCode) {
        departmentCode.focus();
    }

});
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("departmentEntryForm");
    const departmentCode = document.getElementById("departmentCode");

    if (departmentCode) {
        departmentCode.focus();
    }

    if (form && departmentCode) {
        form.addEventListener("reset", function () {
            setTimeout(function () {
                departmentCode.focus();
            }, 0);
        });
    }
});