document.addEventListener("DOMContentLoaded", function () {

    const successMessage = document.getElementById("successMessage");
    const errorMessage = document.getElementById("errorMessage");

    if (successMessage && typeof showSuccess === "function") {
        showSuccess(successMessage.value);
    }

    if (errorMessage && typeof showError === "function") {
        showError(errorMessage.value);
    }

    let activeTooltip = null;

    document.body.addEventListener('mouseenter', function (e) {
        const el = e.target;

        if (el && el.classList.contains('custom-tooltip')) {
            const tooltipText = el.getAttribute('data-tooltip-title');
            const isTruncated = el.scrollWidth > el.clientWidth;

            if (activeTooltip) {
                activeTooltip.dispose();
                activeTooltip = null;
            }

            if (isTruncated && tooltipText) {
                el.setAttribute('title', tooltipText);

                activeTooltip = new bootstrap.Tooltip(el, {
                    placement: 'bottom',
                    trigger: 'manual'
                });

                activeTooltip.show();
            }
        }
    }, true);

    document.querySelectorAll(".delete-form").forEach(function (form) {

        const deleteButton = form.querySelector(".delete-icon");

        if (!deleteButton) return;

        deleteButton.addEventListener("click", function (e) {
            e.preventDefault();          

            const staffCodeInput = form.querySelector('input[name="staffCode"]');
            const staffCode = staffCodeInput ? staffCodeInput.value : "";

            showDelete(
                `Are you sure you want to delete user ${staffCode}?`,
                function () {
                    form.submit();
                }
            );
        });
    });

});