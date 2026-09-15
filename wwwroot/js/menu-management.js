$(document).ready(function () {
    const $parentMenu = $('#ParentMenu');

    truncateParentMenuOptions();
    setFocusToSearch();

    const successMessage =
        document.getElementById('successMessage');

    const errorMessage =
        document.getElementById('errorMessage');

    if (successMessage && successMessage.value.trim() !== '') {
        showSuccess(successMessage.value);
    }
    else if (errorMessage && errorMessage.value.trim() !== '') {
        showError(errorMessage.value);
    }

    function setFocusToSearch() {
        const $searchInput = $('#searchTermInput');

        if ($searchInput.length) {
            setTimeout(function () {
                $searchInput.focus();

                const value = $searchInput.val() || '';
                const strLength = value.length;

                if (strLength > 0) {
                    $searchInput[0].setSelectionRange(
                        strLength,
                        strLength
                    );
                }
            }, 100);
        }
    }

    function truncateParentMenuOptions(maxChars = 35) {
        if ($parentMenu.length > 0) {
            $parentMenu.find('option').each(function () {
                const $option = $(this);
                const text = $option.text();

                if (text.length > maxChars) {
                    $option.attr('title', text);
                    $option.text(
                        text.substring(0, maxChars) + '...'
                    );
                }
            });
        }
    }

    const alertModalEl =
        document.getElementById('alertModal');

    if (alertModalEl) {
        alertModalEl.addEventListener(
            'hidden.bs.modal',
            function () {
                setFocusToSearch();
            }
        );
    }

    $(document).on(
        'click',
        '.delete-btn',
        function (e) {
            e.preventDefault();

            const $form =
                $(this).closest('.delete-form');

            showDelete(
                "Are you sure you want to delete this menu?",
                function () {
                    $form.submit();
                }
            );
        }
    );
});