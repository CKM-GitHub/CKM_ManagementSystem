$(document).ready(function () {
    truncateSelectOptions('.form-select', 25);
    $(document).on('click', '.btn-edit', function () {
        var btn = $(this);
        var id = btn.data('id');
        var title = btn.data('title');
        var description = btn.data('description');
        var assigneeText = btn.data('assignee');
        var dueDate = btn.data('duedate');
        var priorityText = btn.data('priority');
        var statusText = btn.data('status');

        $('#editTaskId').val(id);
        $('#editTitle').val(title);
        $('#editDescription').val(description);
        $('#editDueDate').val(dueDate);

        setSelectByText('#editAssignee', assigneeText);
        setSelectByText('#editPriority', priorityText);
        setSelectByText('#editStatus', statusText);
    });
    function setSelectByText(selectId, textToFind) {
        var searchText = $.trim(textToFind);
        if (!searchText) return;

        $(selectId + ' option').filter(function () {
            var optText = $.trim($(this).text());
            return optText === searchText || optText.startsWith(searchText.substring(0, 20));
        }).prop('selected', true);
    }
    function truncateSelectOptions(selector, maxLength) {
        $(selector + ' option').each(function () {
            var text = $.trim($(this).text());
            if (text.length > maxLength) {
                $(this).text(text.substring(0, maxLength) + '...');
                $(this).attr('title', text); 
            }
        });
    }
    $('#btnRegister').click(function () {
        $('#editTaskForm').submit();
    });
});