$(document).ready(function () {
    const $parentMenu = $('#ParentMenu');
    truncateParentMenuOptions();

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
    function truncateParentMenuOptions(maxChars = 35) {
        if ($parentMenu.length > 0) {
            $parentMenu.find('option').each(function () {
                const $option = $(this);
                const text = $option.text();
                if (text.length > maxChars) {
                    $option.attr('title', text);
                    $option.text(text.substring(0, maxChars) + "...");
                }
            });
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