$(document).ready(function () {
    const $menuForm = $('#menuForm');
    const $displayText = $('#DisplayText');
    const $parentMenu = $('#ParentMenu');
    const $displayOrder = $('#DisplayOrder');
    const $typeParent = $('#typeParent');
    const $statusActive = $('#statusActive');
    toggleParentMenu();
    $.validator.setDefaults({
        showErrors: function (errorMap, errorList) { }
    });
    const validator = $menuForm.validate();
    validator.settings.onfocusout = false;
    validator.settings.onclick = false;
    validator.settings.onkeyup = false;
    $menuForm.on('input change', 'input, select, textarea', function () {
        const $input = $(this);
        if ($input.val().trim() !== '') {
            $input.removeClass('input-validation-error error');
            const fieldName = $input.attr('name');
            const $span = $menuForm.find(`[data-valmsg-for="${fieldName}"]`);
            if ($span.length > 0) {
                $span.removeClass('field-validation-error')
                    .addClass('field-validation-valid')
                    .empty();
            }
        }
    });
    $menuForm.on('keydown', 'input, select, textarea', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            if ($(this).attr('type') !== 'submit' && $(this).prop('tagName') !== 'BUTTON') {
                e.preventDefault();
                const $currentInput = $(this);
                if ($currentInput.attr('id')) {
                    sessionStorage.setItem('lastFocusedElementId', $currentInput.attr('id'));
                }
                if ($currentInput.val().trim() === '' && $currentInput.data('has-focused-empty')) {
                    $menuForm.submit();
                    return false;
                }
                const $focusable = $menuForm.find('input, select, textarea').filter(function () {
                    const $el = $(this);
                    return $el.is(':visible:enabled:not([readonly]):not([type="hidden"]):not([type="radio"]):not([type="checkbox"])')
                        && ($el.prop('required') || $el.attr('data-val-required'));
                });
                const currentIndex = $focusable.index($currentInput);

                if (currentIndex === -1 || $currentInput.is(':radio') || $currentInput.is(':checkbox')) {
                    $menuForm.submit();
                    return false;
                }
                const $nextIndex = $focusable.index($currentInput) + 1;
                if ($nextIndex < $focusable.length) {
                    const $nextInput = $focusable.eq($nextIndex);
                    if ($nextInput.val().trim() === '') {
                        $nextInput.data('has-focused-empty', true);
                    }
                    focusAtEnd($nextInput);
                } else {
                    $menuForm.find('button[type="submit"], input[type="submit"]').first().click();
                }
            }
        }
    });
    $menuForm.off('submit').on('submit', function (e) {
        e.preventDefault();
        const isValid = validator.form();
        $menuForm.find('.field-validation-error')
            .removeClass('field-validation-error')
            .addClass('field-validation-valid')
            .empty();
        $menuForm.find('.input-validation-error').removeClass('input-validation-error');
        if (!isValid) {
            if (validator.errorList.length > 0) {
                const firstError = validator.errorList[0];
                const $firstElement = $(firstError.element);
                $firstElement.addClass('input-validation-error');
                const fieldName = firstError.element.name;
                const $span = $menuForm.find(`[data-valmsg-for="${fieldName}"]`);
                if ($span.length > 0) {
                    $span.removeClass('field-validation-valid')
                        .addClass('field-validation-error')
                        .text(firstError.message);
                }
            }
            return false;
        }
        Swal.fire({
            title: 'Saving...',
            text: 'Please wait a moment',
            allowOutsideClick: false,
            buttonsStyling: false,
            customClass: {
                popup: 'custom-modal-popup',
                title: 'custom-modal-title',
                htmlContainer: 'custom-modal-text'
            },
            didOpen: () => {
                Swal.showLoading();
            }
        });
        $parentMenu.prop('disabled', false);
        $('input[name="MenuType"]').prop('disabled', false);
        this.submit();
    });
    $menuForm.find('input[maxlength]').on('input keyup', function () {
        const $input = $(this);
        const maxLength = parseInt($input.attr('maxlength'), 10);
        const currentLength = $input.val().length;
        const $limitMsg = $input.siblings('.char-limit-msg');
        if (maxLength && currentLength >= maxLength) {
            $limitMsg.removeClass('d-none');
        } else {
            $limitMsg.addClass('d-none');
        }
    });
    if ($('.input-validation-error:visible').length > 0) {
        const $firstError = $('.input-validation-error:visible').first();
        setTimeout(function () {
            focusAtEnd($firstError);
        }, 300);
    } else if (typeof successMessage !== 'undefined' && successMessage !== '') {
        const modalElement = document.getElementById('successModal');
        if (modalElement) {
            let successModal = new bootstrap.Modal(modalElement);
            let pageNum = $('#currentPageNum').val() || 1;
            
            const redirectUrl = menuListUrl + '?page=' + pageNum;
            Swal.fire({
                icon: 'success',
                title: 'Successfully!!',
                text: successMessage,
                confirmButtonText: 'OK',
                buttonsStyling: false,
                customClass: {
                    popup: 'custom-modal-popup',
                    title: 'custom-modal-title',
                    htmlContainer: 'custom-modal-text',
                    confirmButton: 'custom-modal-btn custom-modal-btn-confirm'
                },
                didClose: () => {
                    window.location.assign(redirectUrl);
                }
            });
        } else {
            setTimeout(function () {
                if ($displayText.length > 0) {
                    if ($displayText.val().trim() === '') {
                        $displayText.data('has-focused-empty', true);
                    }
                    focusAtEnd($displayText);
                }
            }, 300);
            $(modalElement).find('.btn, [data-bs-dismiss="modal"]').one('click', function () {
                window.location.assign(redirectUrl);
            });
            $(modalElement).on('hide.bs.modal', function () {
                window.location.assign(redirectUrl);
            });
        }
    }
    else {
        setTimeout(function () {
            if ($displayText.length > 0) {
                if ($displayText.val().trim() === '') {
                    $displayText.data('has-focused-empty', true);
                }
                focusAtEnd($displayText);
            }
        }, 300);
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
        $menuForm.find('input[type="text"], input[type="number"], textarea').val('');
        $menuForm.find('select').prop('selectedIndex', 0);
        $typeParent.prop('checked', true);
        $statusActive.prop('checked', true);
        validator.resetForm();
        $menuForm.find('.field-validation-error')
            .removeClass('field-validation-error')
            .addClass('field-validation-valid')
            .empty();
        $menuForm.find('.input-validation-error').removeClass('input-validation-error');
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
