$(document).ready(function () {
    const $menuForm = $('#menuForm');
    const $displayText = $('#DisplayText');
    const $parentMenu = $('#ParentMenu');
    const $displayOrder = $('#DisplayOrder');
    const $typeParent = $('#typeParent');
    const $statusActive = $('#statusActive');

    const validator = $menuForm.validate();
    validator.settings.onfocusout = false;
    validator.settings.onclick = false;

    validator.settings.onkeyup = function (element) {
        if ($(element).hasClass('input-validation-error') || $(element).hasClass('error')) {
            this.element(element);
        }
    };
    if ($('.input-validation-error:visible').length > 0) {
        const $firstError = $('.input-validation-error:visible').first();
        focusAtEnd($firstError);
    }
    else if (typeof successMessage !== 'undefined' && successMessage !== '') {
        const modalElement = document.getElementById('successModal');
        if (modalElement) {
            const successModal = new bootstrap.Modal(modalElement);
            successModal.show();
            const urlParams = new URLSearchParams(window.location.search);
            const pageNum = urlParams.get('page') || 1;
            const redirectUrl = menuListUrl + '?page=' + pageNum;
            $(modalElement).find('.btn, [data-bs-dismiss="modal"]').one('click', function () {
                window.location.assign(menuListUrl);
            });
            $(modalElement).on('hide.bs.modal', function () {
                window.location.assign(menuListUrl);
            });
        }
    }
    else {
        setTimeout(function () {
            focusAtEnd($displayText);
        }, 100);
    }
    function focusAtEnd($input) {
        if (!$input || $input.length === 0) return;
        const element = $input[0];
        $input.focus();
        if (typeof element.selectionStart === "number") {
            const valueLength = $input.val().length;
            element.selectionStart = valueLength;
            element.selectionEnd = valueLength;
        } else if (typeof element.createTextRange !== "undefined") {
            element.focus();
            const range = element.createTextRange();
            range.collapse(false);
            range.select();
        }
    }
    $menuForm.on('keydown', ':input', function (e) {
        if (e.key !== 'Enter') {
            return
        };
        const $current = $(this);
        if ($current.is(':submit') || $current.is('textarea')) {
          return
        };
        e.preventDefault();
        const isCurrentRequired = $current.prop('required') || $current.data('val-required') !== undefined || $current.hasClass('required');
        if (isCurrentRequired && !validator.element($current)) {
            focusAtEnd($current);
            return;
        }

        const $allInputs = $menuForm.find(':input:visible:not(disabled)')
            .filter(function () {
                return this.type !== 'hidden' && this.type !== 'submit' && this.type !== 'button' && this.type !== 'radio' && this.type !== 'reset' && this.id !== 'btnClear';
            });
        const currentIndex = $allInputs.index($current);
        if (currentIndex !== -1) {
            const $nextRequired = $allInputs.slice(currentIndex + 1).filter(function () {
                return $(this).prop('required') || $(this).data('val-required') !== undefined || $(this).hasClass('required');
            }).first();
            if ($nextRequired.length > 0) {
                focusAtEnd($nextRequired);
            } else if (currentIndex < $allInputs.length - 1) {
                focusAtEnd($allInputs.eq(currentIndex + 1));
            } else {
                $menuForm.submit();
            }
        }
    });

    $menuForm.on('submit', function (e) {
        if (!$menuForm.valid()) {
            e.preventDefault();
            $menuForm.find('.input-validation-error, .error').filter(":visible").first();
            focusAtEnd($firstError);
            return false;
        }
        $parentMenu.prop('disabled', false);
        $('input[name="MenuType"]').prop('disabled', false);
    });
    function toggleParentMenu() {
        if (typeof isEditMode !== 'undefined' && isEditMode) {
            $parentMenu.prop('disabled', true).addClass('bg-light');
            return;
        }
        const selectedType = $('input[name="MenuType"]:checked').val();

        if (selectedType === 'Sub') {
            $parentMenu.prop('disabled', false).removeClass('bg-light');
        } else {
            $parentMenu.prop('disabled', true).addClass('bg-light');
            $parentMenu.prop('selectedIndex', 0);            
        }
        
    }

    function revalidateDisplayOrder() {
        if ($menuForm.data('validator') && $displayOrder.val() !== '') {
            validator.element('#DisplayOrder');
        }
    }

    toggleParentMenu();

    if (typeof isEditMode === 'undefined' || !isEditMode) {
        $('input[name="MenuType"]').change(function () {
            toggleParentMenu();
            revalidateDisplayOrder();
        });
        $parentMenu.on('change', function () {
            revalidateDisplayOrder();
            if ($(this).val() !== '') {
                $(this).removeClass('input-validation-error error');
                $(this).closest('.mb-3, .form-group')
                    .find('.field-validation-error, span.text-danger')
                    .removeClass('field-validation-error')
                    .addClass('field-validation-valid')
                    .empty();
            } else if (validator) {
                validator.element(this);
            }
        });
    }
    $displayOrder.on('keyup input change', function () {
        revalidateDisplayOrder();
    });

        $("#btnClear").click(function (e) {
            e.preventDefault();

            $menuForm.find('input[type="text"], input[type= "number"], textarea').val('');
            $menuForm.find('select').prop('selectedIndex', 0);
            $typeParent.prop('checked', true);
            $statusActive.prop('checked', true);
            validator.resetForm();
            $menuForm.find('.field-validation-error')
                    .removeClass('field-validation-error')
                    .addClass('field-validation-valid')
                    .empty();

            $menuForm.find('.input-validation-error').removeClass('input-validation-error');
            
            setTimeout(function () {
                toggleParentMenu();
                focusAtEnd($displayText);
            }, 10);
    });
}); 