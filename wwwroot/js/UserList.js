document.addEventListener("DOMContentLoaded", function () {

    var successModal = document.getElementById('successModal');

    if (successModal) {
        var modal = new bootstrap.Modal(successModal);
        modal.show();
    }

    var errorModal = document.getElementById('errorModal');

    if (errorModal) {
        var modal = new bootstrap.Modal(errorModal);
        modal.show();
    }
    const deleteIcons = document.querySelectorAll('.delete-icon');

    deleteIcons.forEach(icon => {
        icon.addEventListener('click', function (e) {
            e.preventDefault();

            const form = this.closest('.delete-form');
            const staffCode = form.querySelector('input[name="staffCode"]').value;

            Swal.fire({
                title: 'ဖျက်မှာ သေချာလား သူငယ်ချင်း',
                html: `<div style="text-align: left; padding: 10px 0;">
                        <p style="margin-bottom: 8px; font-size: 15px;">
                            <strong>Staff Code:</strong> <span style="color: #d33;">${staffCode}</span>
                        </p>
                        <hr style="margin: 12px 0;">
                        <p style="font-size: 14px; color: grey; margin: 0;">
                            All user data will be permanently removed.
                        </p>
                      </div>`,
                iconHtml: '<i class="fas fa-user-slash" style="font-size: 48px;"></i>',
                showCancelButton: true,
                cancelButtonText: 'မသေချာဘူး',
                confirmButtonText: 'ဖျက်မည်',
                reverseButtons: true,
                buttonstyling: false,
                width: 450,
                customClass: {
                    actions: 'swal2-actions-right',
                    title : 'custom-modal-title',
                    confirmButton: 'custom-confirm-btn',
                    cancelButton: 'custom-cancel-btn'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    form.submit();

                    Swal.fire({
                        text: 'အောင်မြင်စွာဖျက်ပြီးပါပြီ',
                        icon: 'success',
                        timer: 5000,
                        showConfirmButton: false
                    });
                }
            });
        });
    });
});   