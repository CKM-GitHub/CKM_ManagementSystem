$(document).ready(function () {
    truncateSelectOptions('.form-select', 25);
    $(document).on('click', '.btn-edit', function () {

        const btn = $(this);
        const id = btn.data('id');
        const title = btn.data('title');
        const description = btn.data('description');
        const assigneeText = btn.data('assignee');
        const dueDate = btn.data('duedate');
        const priorityText = btn.data('priority');
        const statusText = btn.data('status');

        $('#editTaskId').val(id);
        $('#editTitle').val(title || '');
        $('#editDescription').val(description || '');
        $('#editDueDate').val(dueDate || '');

        setSelectByText('#editAssignee', assigneeText);
        setSelectByText('#editPriority', priorityText);
        setSelectByText('#editStatus', statusText);
    });

    function truncateSelectOptions(selector, maxLength) {
        $(selector + ' option').each(function () {
            var text = $.trim($(this).text());
            if (text.length > maxLength) {
                $(this).text(text.substring(0, maxLength) + '...');
                $(this).attr('title', text);
            }
        });
    }
    function setSelectByText(selectId, textToFind) {

        const searchText =
            $.trim(textToFind || '');

        if (!searchText) {
            $(selectId).val('');
            return;
        }

        let found = false;

        $(selectId + ' option')
            .each(function () {

                const optionText =
                    $.trim($(this).text());

                if (optionText === searchText) {

                    $(this).prop(
                        'selected',
                        true
                    );

                    found = true;

                    return false;
                }
            });

        if (!found) {
            $(selectId).val('');
        }
    }


    if (
        typeof successMessage !== 'undefined' &&
        successMessage
    ) {
        showSuccess(successMessage);
    }


    if (
        typeof errorMessage !== 'undefined' &&
        errorMessage
    ) {
        showError(errorMessage);
    }

});