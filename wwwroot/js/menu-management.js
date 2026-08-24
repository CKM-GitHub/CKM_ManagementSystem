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

    if (typeof successMessage !== 'undefined' && successMessage !== '') {
        Swal.fire({
            title: 'Successfully!!',
            text: successMessage,
            icon: undefined,
            iconHtml: '<i class="bi bi-check-circle-fill text-success"></i>',
            confirmButtonText: 'OK',
            customClass: {
                popup: 'custom-modal-popup',
                title: 'custom-modal-title',
                htmlContainer: 'custom-modal-text',
                actions: 'swal-single-buttons',
                confirmButton: 'custom-modal-btn custom-modal-btn-success',
                icon: 'custom-swal-icon'
            },
            didClose: () => {
                setFocusToSearch();
            }
        });
    } else if (typeof errorMessage !== 'undefined' && errorMessage !== '') {
        Swal.fire({
            icon: 'error',
            title: 'Failed',
            text: errorMessage,
            buttonsStyling: false,
            customClass: {
                popup: 'custom-modal-popup',
                title: 'custom-modal-title',
                htmlContainer: 'custom-modal-text',
                confirmButton: 'custom-modal-btn custom-modal-btn-confirm'
            },
            didClose: () => {
                setFocusToSearch();
            }
        });
    }
    
    $(document).on('click', '.delete-btn', function (e) {
        e.preventDefault();
        const $form = $(this).closest('.delete-form');
     
        Swal.fire({
            title: 'Are you sure?',
            text: "Are you sure you want to delete this menu?",
            icon: 'warning',
            iconColor: '#dc3545',
            showCancelButton: true,
            confirmButtonText: '<i class="bi bi-trash-fill me-1"></i> Yes, delete it!',
            cancelButtonText: '<i class="bi bi-x-lg me-1"></i> Cancel',
            buttonsStyling: false,
            customClass: {
                popup: 'custom-modal-popup',
                title: 'custom-modal-title',
                htmlContainer: 'custom-modal-text',
                actions: 'swal-two-buttons',
                confirmButton: 'custom-modal-btn custom-modal-btn-confirm ',
                cancelButton: 'custom-modal-btn custom-modal-btn-cancel'
            },
            didClose: () => {
                setFocusToSearch();
            }
        }).then((result) => {
            if (result.isConfirmed) {
                Swal.fire({
                    title: 'Deleting...',
                    text: 'Please wait a moment',
                    allowOutsideClick: false,
                    buttonsStyling: false,
                    customClass: {
                        popup: 'custom-modal-popup',
                        title: 'custom-modal-title',
                        htmlContainer:'custom-modal-text'
                    },
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });
                $form.submit();
            }
        });
    });
});