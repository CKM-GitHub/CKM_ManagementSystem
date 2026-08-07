
$(document).ready(function () {
    if (typeof successMessage !== 'undefined' && successMessage !== '') {
        var modalElement = document.getElementById('successModal');

        if (modalElement) {
            var successModal = new bootstrap.Modal(modalElement);
            successModal.show();
        }
    }

    $(document).on('click', '.delete-btn', function (e) {
        e.preventDefault();

        const menuId = $(this).data('menuId');
        const token = $('#antiForgeryForm input[name="__RequestVerificationToken"]').val();
        const deleteUrl = '/Menu/DeleteMenu';
        if (!menuId) {
            return;
        }
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
                confirmButton: 'btn btn-danger custom-modal-btn me-2 ',
                cancelButton: 'btn btn-secondary custom-modal-btn'
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
                $.ajax({
                    url: deleteUrl,
                    type: 'POST',
                    data: { menuId: menuId },
                    headers: {
                        'RequestVerificationToken': token
                    },
                    success: function (data) {
                        if (data.success) {
                            Swal.fire({
                                title: 'Deleted Successfully!!',
                                text: data.message,
                                icon: undefined,
                                iconHtml: '<i class="bi bi-check-circle-fill text-success"></i>',
                                confirmButtonText: 'OK',
                                customClass: {
                                    popup: 'custom-modal-popup',
                                    title: 'custom-modal-title',
                                    htmlContainer: 'custom-modal-text',
                                    confirmButton: 'custom-modal-btn',
                                    icon: 'custom-swal-icon'
                                }
                            }).then(() => {
                                location.reload();
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Failed',
                                text: data.message,
                                buttonsStyling: false,
                                customClass: {
                                    popup: 'custom-modal-popup',
                                    title: 'custom-modal-title',
                                    htmlContainer: 'custom-modal-text',
                                    confirmButton: 'custom-modal-btn'
                                }
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: 'Something went wrong on the server.',
                            confirmButtonText: 'OK',
                            buttonsStyling: false,
                            customClass: {
                                popup: 'custom-modal-popup',
                                title: 'custom-modal-title',
                                htmlContainer: 'custom-modal-text',
                                confirmButton: 'custom-modall-btn'
                            }
                        });
                    }

                });
            }
        });
    });
});