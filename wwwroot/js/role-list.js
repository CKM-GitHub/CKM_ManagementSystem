function deleteRole(roleCode, deleteUrl) {
    var deleteMessage = "Are you sure you want to delete this role?";

    showDelete(deleteMessage, function () {
        var token = $('input[name="__RequestVerificationToken"]').val();

        $('#alertModal').modal('hide');

        setTimeout(function () {
            $.ajax({
                url: deleteUrl,
                type: 'POST',
                data: {
                    roleCode: roleCode,
                    __RequestVerificationToken: token
                },
                success: function (res) {
                    if (res.success) {
                        showSuccess(res.message || 'Role has been deleted successfully.');

                        $('#alertModal').one('hidden.bs.modal', function () {
                            location.reload();
                        });
                    } else {
                        showError(res.message || 'Failed to delete the role.');
                    }
                },
                error: function (xhr, status, error) {
                    var errMsg = "A system error occurred: " + error;
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errMsg = xhr.responseJSON.message;
                    }
                    showError(errMsg);
                }
            });
        }, 350);
    });
}