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
        const attachment = btn.attr('data-attachment');
        
        $('#editTaskId').val(id);
        $('#editTitle').val(title || '');
        $('#editDescription').val(description || '');
        $('#editDueDate').val(dueDate || '');
        $('#editAttachments').val(attachment || '');
        const $attachmentPreview = $('#editAttachmentPreview');
        const $attachmentImage = $('#editAttachmentImage');

        if (attachment) {
            $attachmentImage.attr('src', attachment);
            $attachmentPreview.addClass('is-visible');
        } else {
            $attachmentImage.attr('src', '');
            $attachmentPreview.removeClass('is-visible');
        }
        
        setSelectByText('#editAssignee', assigneeText);
        setSelectByText('#editPriority', priorityText);
        setSelectByText('#editStatus', statusText);
    });
    $(document).on('change', '#editFileInput', function () {
            const file = this.files[0];
            if (!file) {
                return;
            }
            const reader = new FileReader();
            reader.onload = function (event) {

                $('#editAttachmentImage')
                    .attr('src', event.target.result);

                $('#editAttachmentPreview')
                    .addClass('is-visible');
            };

            reader.readAsDataURL(file);
        }
    );
    $(document).on(
        'click',
        '.upload-zone',
        function () {

            const fileInput =
                $('#editFileInput')[0];

            if (fileInput) {
                fileInput.click();
            }
        }
    );
    $(document).on('click','#editFileInput',
        function (e) {
            e.stopPropagation();
        }
    );
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
        const searchText = $.trim(textToFind || '');
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