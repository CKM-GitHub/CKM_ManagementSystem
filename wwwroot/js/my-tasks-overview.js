document.addEventListener('DOMContentLoaded', function () {

    const editButtons =
        document.querySelectorAll('.btn-edit-my-task');

    const statusSelect =
        document.getElementById('editTaskStatus');

    const editForm =
        document.getElementById('editMyTaskForm');

    const dateError =
        document.getElementById('editDateError');

    const messageHolder =
        document.getElementById('myTaskPageMessages');


    
    editButtons.forEach(function (button) {

        button.addEventListener('click', function () {

            setValue(
                'editTaskId',
                this.dataset.id
            );

            setValue(
                'editTaskTitle',
                this.dataset.title
            );

            setValue(
                'editTaskDescription',
                this.dataset.description
            );

            setValue(
                'editTaskAssignee',
                this.dataset.assignee
            );

            setValue(
                'editTaskDueDate',
                this.dataset.dueDate
            );

            setValue(
                'editTaskPriority',
                this.dataset.priority
            );

            setValue(
                'editTaskStartDate',
                this.dataset.startDate
            );

            setValue(
                'editTaskEndDate',
                this.dataset.endDate
            );

            setStatus(
                this.dataset.status || ''
            );

            if (dateError) {
                dateError.classList.add('d-none');
            }
        });
    });


  

    function setValue(elementId, value) {

        const element =
            document.getElementById(elementId);

        if (element) {
            element.value = value || '';
        }
    }


    

    function setStatus(statusName) {

        if (!statusSelect) {
            return;
        }

        statusSelect.value = '';

        const target =
            (statusName || '')
                .trim()
                .toLowerCase();

        if (!target) {
            return;
        }

        Array.from(statusSelect.options)
            .forEach(function (option) {

                const optionText =
                    option.text
                        .trim()
                        .toLowerCase();

                if (optionText === target) {
                    statusSelect.value =
                        option.value;
                }
            });
    }


    

    if (editForm) {

        editForm.addEventListener(
            'submit',
            function (event) {

                const startDate =
                    document.getElementById(
                        'editTaskStartDate'
                    )?.value || '';

                const endDate =
                    document.getElementById(
                        'editTaskEndDate'
                    )?.value || '';

                if (
                    startDate &&
                    endDate &&
                    endDate < startDate
                ) {
                    event.preventDefault();

                    if (dateError) {
                        dateError.classList.remove(
                            'd-none'
                        );
                    }

                    return;
                }

                if (dateError) {
                    dateError.classList.add(
                        'd-none'
                    );
                }
            }
        );
    }


    if (messageHolder) {

        const successMessage =
            messageHolder.dataset.success || '';

        const errorMessage =
            messageHolder.dataset.error || '';

        if (
            successMessage &&
            typeof showSuccess === 'function'
        ) {
            showSuccess(successMessage);
        }
        else if (
            errorMessage &&
            typeof showError === 'function'
        ) {
            showError(errorMessage);
        }
    }

});