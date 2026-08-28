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
        $('.input-validation-error:visible').first().focus();
    }
    else if (typeof successMessage !== 'undefined' && successMessage !== '') {
        const modalElement = document.getElementById('successModal');
        if (modalElement) {
            const successModal = new bootstrap.Modal(modalElement);
            successModal.show();
            $(modalElement).on('hidden.bs.modal', function () {
                $displayText.focus();
            });
        }
    }
    else {
        setTimeout(function () {
            $displayText.focus();
        }, 100);
        
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
            $current.focus();
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
                $nextRequired.focus();
            } else if (currentIndex < $allInputs.length - 1) {
                $allInputs.eq(currentIndex + 1).focus();
            } else {
                $menuForm.submit();
            }
        }
    });

    $menuForm.on('submit', function (e) {
        if (!$menuForm.valid()) {
            e.preventDefault();
            $menuForm.find('.input-validation-error, .error').filter(":visible").first().focus();
            return false;
        }
        $parentMenu.prop('disabled', false);
        $('input[name="MenuType"]').prop('disabled', false);
    });
    function toggleParentMenu() {
        if (typeof isEditMode !== 'undefined' && isEditMode) {
            $parentMenu.prop('disabled', true).addClass('bg-light');
            //$hiddenParentMenu.prop('disabled', true);
            return;
        }
        const selectedType = $('input[name="MenuType"]:checked').val();

        if (selectedType === 'Sub') {
            $parentMenu.prop('disabled', false).removeClass('bg-light');
            //$hiddenParentMenu.prop('disabled', true);
           
                
        } else {
            $parentMenu.prop('disabled', true).addClass('bg-light');
            $parentMenu.prop('selectedIndex', 0);
            //$hiddenParentMenu.prop('disabled', false);
            
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
                $displayText.focus();
            }, 10);
    });
}); 