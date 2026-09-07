$(document).ready(function () {
    function setFocusToSearch() {
        const $searchInput = $('#searchTermInput');
        if ($searchInput.length) {
            setTimeout(() => {
                $searchInput.focus();
                const strLength = $searchInput.val().length;
                if (strLength > 0) {
                    $searchInput[0].setSelectionRange(strLength, strLength);
                }
            }, 100)
        }
    }
    setFocusToSearch();
    
    const alertModalEl = document.getElementById("alertModal");
    if (alertModalEl) {
        alertModalEl.addEventListener('hidden.bs.modal', function () {
            setFocusToSearch();
        });
    }

    if (typeof successMessage !== 'undefined' && successMessage !== '') {
        showSuccess(successMessage);
    } else if (typeof errorMessage !== 'undefined' && errorMessage !== '') {
        showError(errorMessage);
    }
    
    $(document).on('click', '.delete-btn', function (e) {
        e.preventDefault();
        const $form = $(this).closest('.delete-form');

        showDelete("You won't be able to revert this!", function () {
            $form.submit();
        });
    });
});