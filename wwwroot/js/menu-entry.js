$(document).ready(function () {
    const $menuForm = $('#menuForm');
    const $displayText = $('#DisplayText');
    const $parentMenu = $('#ParentMenu');
    const $displayOrder = $('#DisplayOrder');
    const $typeParent = $('#typeParent');
    const $statusActive = $('#statusActive');
    toggleParentMenu();
    truncateParentMenuOptions(35);
    //$.validator.setDefaults(showError(errorMessage));
    const validator = $menuForm.validate();
    validator.settings.onfocusout = false;
    validator.settings.onclick = false;
    validator.settings.onkeyup = false;

    $menuForm.on('input change', 'input, select, textarea',
        function () {
            const $input = $(this);
           
            if (
                $input.hasClass('input-validation-error') ||
                $input.siblings('[data-valmsg-for="' + $input.attr('name') + '"]'
                ).hasClass('field-validation-error')
            ) {
                validator.element($input);
            }
        }
    );
    
    $menuForm.on('keydown', 'input, select, textarea',
        function (e) {
            if (e.key !== 'Enter') {
                return;
            }
            e.preventDefault();
            const $currentInput = $(this);
            if ($currentInput.is('#DisplayOrder')) {
                const val = $currentInput.val();
                const $msg = $currentInput.siblings('.char-limit-msg');

                if (val !== '' && !/^\d+$/.test(val)) {
                    $currentInput.addClass('border-danger');
                    $msg.removeClass('d-none').text('Please enter numbers only (0-9)!');
                    focusAtEnd($currentInput);
                    return false;
                }
            }
            const isCurrentValid = validator.element($currentInput);
            if (!isCurrentValid) {
                focusAtEnd($currentInput);
                return false;
            }
        const $focusable = $menuForm.find('input, select, textarea')
        .filter(function () {
            const $el = $(this);
            return $el.is(':visible:enabled:not([readonly]):not([type="hidden"]):not([type="radio"]):not([type="checkbox"])')
                && ($el.prop('required') || $el.attr('data-val-required'));
        });
    const currentIndex = $focusable.index($currentInput);
    const nextIndex = currentIndex + 1;
    if (currentIndex !== -1 && nextIndex < $focusable.length) {
        focusAtEnd($focusable.eq(nextIndex));
    } else {
        $('#btnRegister').focus();
    }
    });

    $menuForm.off('submit').on('submit', function (e) {
        e.preventDefault();
        
        if (!$parentMenu.prop('disabled') && $parentMenu.val() === '') {
            $parentMenu.addClass('input-validation-error');
            const $span = $menuForm.find(`[data-valmsg-for="${$parentMenu.attr('name')}"]`);
            if ($span.length > 0) {
                $span.removeClass('field-validation-valid')
                    .addClass('field-validation-error')
                    .text('Please choose a Parent Menu.');
            }
            setTimeout(function () {
                $parentMenu.focus();
            }, 50);
            return false;
        }
        const $fields = $menuForm.find('input, select, textarea')
            .filter(function () {
                const $field = $(this);
                return $field.is(':visible:enabled:not([readonly]):not([type="hidden"]):not([type="radio"]):not([type="checkbox"])')
                    && $field.attr('data-val') === 'true';
            });
        let firstInvalid = null;
        for (let i = 0; i < $fields.length; i++) {
            const $field = $fields.eq(i);
            const isValid = validator.element($field);
            if (!isValid) {
                firstInvalid = $field;
                break;
            }
        }
        if (firstInvalid) {
            firstInvalid.addClass('input-validation-error');
            const fieldName = firstInvalid.attr('name');
            const $span = $menuForm.find(`[data-valmsg-for="${fieldName}"]`);
            const errorMessage = validator.errorMap[fieldName];
            if ($span.length > 0 && errorMessage) {
                $span.removeClass('field-validation-valid')
                    .addClass('field-validation-error')
                    .text(errorMessage);
            }

            setTimeout(function () {
                focusAtEnd(firstInvalid);
            }, 50);
            return false;
        }
        $parentMenu.prop('disabled', false);
        $('input[name="MenuType"]').prop('disabled', false);
        this.submit();
    });

    function updateCharLimitMessage($input) {
        const maxLength = parseInt($input.attr('maxlength'), 10);
        const currentLength = $input.val().length;
        const $limitMsg = $input.siblings('.char-limit-msg');
        if (maxLength && currentLength > maxLength) {
            $limitMsg.removeClass('d-none');
        } else {
            $limitMsg.addClass('d-none');
        }
    }
    $menuForm.find('input[maxlength]').on('input keyup', function () {
        updateCharLimitMessage($(this));
    });
    $menuForm.find('input[maxlength]')
        .each(function () {
            updateCharLimitMessage($(this));
        });
    function truncateParentMenuOptions(maxChars = 35) {
        if ($parentMenu.length > 0) {
            $parentMenu.find('option').each(function () {
                const $option = $(this);
                const text = $option.text();
                if (text.length > maxChars) {
                    $option.attr('title', text);
                    $option.text(text.substring(0, maxChars) + "...");
                }
            });
        }
    }

    if (typeof errorMessage !== 'undefined' && errorMessage !== '') {
        setTimeout(function () {
            //showError(errorMessage);
            const $firstError = $('.input-validation-error:visible').first();
            if ($firstError.length > 0) {
                focusAtEnd($firstError);
            } else {
                focusAtEnd($displayText);
            }
        }, 300);
    } else if (typeof successMessage !== 'undefined' && successMessage !== '') {
            let pageNum = $('#currentPageNum').val() || 1;
            const redirectUrl = menuListUrl + '?page=' + pageNum;
            showSuccess(successMessage);
            const alertModalEl = document.getElementById("alertModal");
            if (alertModalEl) {
                $(alertModalEl).one('hidden.bs.modal', function () {
                    window.location.assign(redirectUrl);
                });
            }
    } else {
            setTimeout(function () {
                if ($displayText.length > 0) {
                    if ($displayText.val().trim() === '') {
                        $displayText.data('has-focused-empty', true);
                    }
                    focusAtEnd($displayText);
                }
            }, 300);
    }
    $displayOrder.on('input keyup change', function (e) {
        const $input = $(this);
        const val = $input.val();
        const $msg = $input.siblings('.char-limit-msg');
        
        if (val === '') {
            $input.removeClass('border-danger');
            $msg.addClass('d-none');
            return;
        }
        if (val !== '' && !/^\d+$/.test(val)) {
            $input.addClass('border-danger');
            $msg.removeClass('d-none').text('Please enter numbers only (0-9)!');
            return;
        }
        if (val.length > 3) {
            val = val.substring(0, 3);
            $input.val(val);
            $input.addClass('border-danger');
            $msg.removeClass('d-none').text('Display Order cannot exceed 3 digits!');
        }
        else {
            $input.removeClass('border-danger');
            $msg.addClass('d-none');
        }
        if (typeof validator !== 'undefined') {
            validator.element($input);
        }
    });
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
    function toggleParentMenu() {
        if (typeof isEditMode !== 'undefined' && isEditMode) {
            $parentMenu.prop('disabled', true).addClass('bg-light');
            return;
        }
        const selectedType = $('input[name="MenuType"]:checked').val();
        if (selectedType == 'Sub') {
            $parentMenu.prop('disabled', false).removeClass('bg-light');
        } else {
            $parentMenu.prop('disabled', true).addClass('bg-light');
            $parentMenu.prop('selectedIndex', 0);
        }
    }
    if (typeof isEditMode === 'undefined' || !isEditMode) {
        $('input[name="MenuType"]').change(function () {
            toggleParentMenu();
        });
        $parentMenu.on('change', function () {
            if ($(this).val() !== '') {
                $(this).removeClass('input-validation-error error');
                $(this).closest('.mb-3, .form-group')
                    .find('.field-validation-error, span.text-danger')
                    .removeClass('field-validation-error')
                    .addClass('field-validation-valid')
                    .empty();
            }
        });
    }
    $("#btnClear").click(function (e) {
        e.preventDefault();
        $menuForm.find('input:not([type="radio"]):not([type="checkbox"]):not([type="hidden"]), textarea').val('');
        $menuForm.find('select').prop('selectedIndex', 0);
        $typeParent.prop('checked', true);
        $statusActive.prop('checked', true);
        validator.resetForm();
        $menuForm.find('.field-validation-error')
            .removeClass('field-validation-error')
            .addClass('field-validation-valid')
            .empty();
        $menuForm.find('.input-validation-error, .border-danger').removeClass('input-validation-error border-danger');
        $menuForm.find('.char-limit-msg').addClass('d-none');
        $menuForm.find('input, select, textarea').removeData('has-focused-empty');
        $('.alert, .alert-danger, [asp-validation-summary], .validation-summary-errors')
            .addClass('d-none')
            .hide()
            .empty();
        setTimeout(function () {
            toggleParentMenu();
            if ($displayText.val().trim() === '') {
                $displayText.data('has-focused-empty', true);
            }
            focusAtEnd($displayText);
        }, 100);
    });
});
