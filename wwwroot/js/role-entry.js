$(document).ready(function () {
    
    const $roleForm = $('#roleForm');

    const isEditMode =
        $roleForm.data('edit-mode') === true ||
        $roleForm.data('edit-mode') === 'true';

    const roleListUrl =
        $roleForm.data('role-list-url') || '#';
    if (!isEditMode) {
        $('#RoleCode').focus();
    }

    function getActionClass($el) {
        if ($el.hasClass('chk-read')) return '.chk-read';
        if ($el.hasClass('chk-write')) return '.chk-write';
        if ($el.hasClass('chk-delete')) return '.chk-delete';
        return '';
    }

    $(document).on('change', '.chk-write, .chk-delete', function () {
        if ($(this).is(':checked')) {
            $(this).closest('tr').find('.chk-read').prop('checked', true);
        }
    });

    $(document).on('change', '.chk-read', function () {
        if (!$(this).is(':checked')) {
            $(this).closest('tr').find('.chk-write, .chk-delete').prop('checked', false);
        }
    });

    $(document).on('change', '.parent-menu-row input[type="checkbox"]', function () {
        const $parentRow = $(this).closest('tr');
        const parentId = $parentRow.data('menu-id');
        const isChecked = $(this).is(':checked');
        const actionClass = getActionClass($(this));
        const $children = $(`tr.child-menu-row[data-parent-id="${parentId}"]`);

        if (actionClass === '.chk-read') {
            if (isChecked) {
                $children.find('.chk-read').prop('checked', true);
            } else {
                $parentRow.find('.chk-write, .chk-delete').prop('checked', false);
                $children.find('input[type="checkbox"]').prop('checked', false);
            }
        } else if (actionClass) {
            $children.find(actionClass).prop('checked', isChecked);
            if (isChecked) {
                $parentRow.find('.chk-read').prop('checked', true);
                $children.find('.chk-read').prop('checked', true);
            }
        }
    });

    $(document).on('change', '.child-menu-row input[type="checkbox"]', function () {
        const $childRow = $(this).closest('tr');
        const parentId = $childRow.data('parent-id');
        const actionClass = getActionClass($(this));

        if (actionClass) {
            const $totalChildren = $(`tr.child-menu-row[data-parent-id="${parentId}"]`);
            const $checkedChildren = $totalChildren.find(`${actionClass}:checked`);
            const $parentRow = $(`tr.parent-menu-row[data-menu-id="${parentId}"]`);

            if (actionClass === '.chk-read') {
                if ($(this).is(':checked')) {
                    $parentRow.find('.chk-read').prop('checked', true);
                } else if ($checkedChildren.length === 0) {
                    $parentRow.find('.chk-read').prop('checked', false);
                }
            } else {
                const allChecked = ($totalChildren.length === $checkedChildren.length && $totalChildren.length > 0);
                $parentRow.find(actionClass).prop('checked', allChecked);

                if ($(this).is(':checked')) {
                    $childRow.find('.chk-read').prop('checked', true);
                    $parentRow.find('.chk-read').prop('checked', true);
                }
            }
        }
    });

    $('#roleForm').on('submit', function (e) {
        const $form = $(this);
        if (!$form.valid()) return;

        e.preventDefault();

        $('#valRoleCode').text('');
        $('#valDisplayName').text('');

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function (response) {
                let msg = "";
                if (typeof response === "string") {
                    msg = response;
                } else if (response) {
                    msg = response.message || response.Message || response.msg || "Successfully Saved!";
                } else {
                    msg = "Successfully Saved!";
                }

                const isSuccess = response && (response.success === true || response.Success === true);

                if (isSuccess) {
                    showSuccess(msg);
                    $('#alertModal').one('hidden.bs.modal', function () {
                        if (isEditMode) {
                            window.location.href = roleListUrl;
                        } else {
                            $('#btnClear').click();
                        }
                    });
                } else {
                    if (msg.includes("Role Code")) {
                        $('#valRoleCode').text(msg);
                        $('#RoleCode').focus();
                    } else if (msg.includes("Role Name") || msg.includes("Display Name")) {
                        $('#valDisplayName').text(msg);
                        $('#DisplayName').focus();
                    } else {
                        showError(msg);
                    }
                }
            },
            error: function () {
                showError('An error occurred while saving.');
            }
        });
    });

    $('#roleForm').on('keydown', 'input, textarea', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            if ($(this).is('button, :submit, :checkbox, :radio')) return;

            e.preventDefault();

            const id = $(this).attr('id');

            if (id === 'RoleCode') {
                $('#DisplayName').focus();
            } else if (id === 'DisplayName') {
                $('#Description').focus();
            } else if (id === 'Description') {
                $('#statusActive').focus();
            }
        }
    });

    $(document).on('input keyup paste', '#RoleCode', function () {
        const $this = $(this);
        $('#valRoleCode').text('');
        setTimeout(function () {
            const cleanValue = $this.val().replace(/[^a-zA-Z0-9_-]/g, '');
            $this.val(cleanValue);
        }, 0);
    });

    $(document).on('input keyup paste', '#DisplayName', function () {
        $('#valDisplayName').text('');
    });

    $('#btnClear').on('click', function () {
        const $form = $('#roleForm');
        $form[0].reset();
        $('#valRoleCode').text('');
        $('#valDisplayName').text('');
        $form.find('.field-validation-error').empty().addClass('field-validation-valid').removeClass('field-validation-error');
        $form.find('.input-validation-error').removeClass('input-validation-error');
        $('#statusActive').prop('checked', true);
        $('#RoleCode').focus();
    });
});