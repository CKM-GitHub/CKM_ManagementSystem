
$(document).ready(function () {
    const $menuForm = $('#menuForm');
    const $displayText = $('#DisplayText');
    const $parentMenu = $('#ParentMenu');
    const $displayOrder = $('#DisplayOrder');
    const $typeParent = $('#typeParent');
    const $statusActive = $('#statusActive');

    const validator = $menuForm.validate({
        onsubmit: false,
        onfocusout: false,
        onclick: false,
        onkeyup: false
    });

    toggleParentMenu();

    if (typeof successMessage !== 'undefined' && successMessage !== '') {
        showSuccess(successMessage);
        $displayText.focus();
    }
    else {
        setTimeout(function () {
            focusAtEnd($displayText);
        }, 300);
    }
    $menuForm.on(
        'input',
        'input[type="text"], input[type="number"], textarea',
        function () {
            const $input = $(this);
            if ($input.hasClass('input-validation-error')) {
                validator.element($input);
            }
            updateCharLimitMessage($input);
        }
    );

    $menuForm.on('change','select',
        function () {
            const $select = $(this);
            if ($select.hasClass('input-validation-error')) {
                validator.element($select);
            }
        }
    );

    function updateCharLimitMessage($input) {
        if (!$input.attr('maxlength')) {
            return;
        }
        const maxLength =
            parseInt(
                $input.attr('maxlength'),
                10
            );
        const currentLength =$input.val().length;
        const $limitMsg =$input.siblings('.char-limit-msg');

        if (maxLength &&currentLength >= maxLength
        ) {
            $limitMsg.removeClass(
                'd-none'
            );
        }
        else {

            $limitMsg.addClass(
                'd-none'
            );
        }
    }


    $menuForm.find('input[maxlength]')
        .each(function () {
            updateCharLimitMessage(
                $(this)
            );
        });
    $menuForm.on('keydown', 'input, select, textarea', function (e) {
        if (e.key !== 'Enter') {
            return;
        }
        
        e.preventDefault();
        const $currentInput = $(this);

        const isCurrentValid = validator.element($currentInput);
        if (!isCurrentValid) {
            focusAtEnd($currentInput);
            return false;
        }
        const $focusable = $menuForm.find('input, select, textarea')
            .filter(function () {
                const $field = $(this);
                return $field.is(':visible:enabled:not([readonly]):not([type="hidden"]):not([type="radio"]):not([type="checkbox"])')
                    &&($field.prop('required') || $field.attr('data-val-required'));
            });
        
        const currentIndex = $focusable.index($currentInput);
        const nextIndex = currentIndex + 1;
        if (currentIndex !== -1 && nextIndex < $focusable.length) {
            focusAtEnd($focusable.eq(nextIndex));
        } else {
            $('#btnRegister').focus();
        }
        return false;
    });
    $menuForm.on('submit', function (e) {
        e.preventDefault();
        validator.resetForm();
        $menuForm.find('.field-validation-error')
            .removeClass('field-validation-error')
            .addClass('field-validation-valid')
            .text('');
        const isValid = validator.form();
        const invalidElements = validator.invalidElements();
        let firstInvalid = invalidElements.length > 0 ? $(invalidElements[0]) : null;
        if (invalidElements.length > 1) {
            for (let i = 1; i < invalidElements.length; i++) {
                const $el = $(invalidElements[i]);
                $el.removeClass('input-validation-error');
                const name = $el.attr('name');
                if (name) {
                    $(`[data-valmsg-for="${name}"]`)
                        .removeClass('field-validation-error')
                        .addClass('field-validation-valid')
                        .text('');
                }
            }
        }
        if (!firstInvalid) {
            const selectedType = $('input[name="MenuType"]:checked').val();
            if (selectedType === 'Sub' && ($parentMenu.val() === '0' || $parentMenu.val() === '' || $parentMenu.val() === null)) {
                $parentMenu.addClass('input-validation-error');
                const $parentError = $('[data-valmsg-for="ParentMenuId"], .parent-menu-error, #parentMenuError').first();
                if ($parentError.length > 0) {
                    $parentError.removeClass('field-validation-valid')
                        .addClass('field-validation-error')
                        .text('Please select a Parent Menu.');
                }
                firstInvalid = $parentMenu;
            }
        }

        if (firstInvalid) {
            setTimeout(function () {
                focusAtEnd(firstInvalid);
            }, 50);
            return false;
        }

        $parentMenu.prop('disabled', false);
        this.submit();
    });
    function toggleParentMenu() {
        const selectedType = $('input[name="MenuType"]:checked').val();
        if (selectedType === 'Sub') {
            $parentMenu.prop('disabled', false).removeClass('bg-light');

        } else {
            $parentMenu.val('0').prop('disabled', true).addClass('bg-light');
            $parentMenu.removeClass('input-validation-error error');
            const $parentError = $('[data-valmsg-for="ParentMenuId"], .parent-menu-error, #parentMenuError').first();
            if ($parentError.length > 0) {
                $parentError.removeClass('field-validation-error').addClass('field-validation-valid').text('');
            }
        }
    }

    function revalidateDisplayOrder() {
        if ($menuForm.data('validator') && $displayOrder.val() !== '') {
            $menuForm.validate().element('#DisplayOrder');
        }
    }

    toggleParentMenu();

    $('input[name="MenuType"]').change(function () {
        toggleParentMenu();
        revalidateDisplayOrder();
    });
    $parentMenu.on('change', function () {
        revalidateDisplayOrder();
        if ($(this).val() !== '' && $(this).val() !== '0') {
            $(this).removeClass('input-validation-error error');
            const $parentError =
                $(
                    '[data-valmsg-for="ParentMenuId"], ' +
                    '.parent-menu-error, ' +
                    '#parentMenuError'
                ).first();
            if ($parentError.length > 0) {
                $parentError.removeClass('field-validation-error')
                    .addClass('field-validation-valid')
                    .text('');
            }
        }
        else if (validator) {
            validator.element(this);
        }
    });
    $displayOrder.on('keyup input change', function () {
        revalidateDisplayOrder();
    });
    
    $('.limit-input').on('input keyup', function () {
        const $input = $(this);
        const maxLen = parseInt($input.attr('maxlength')) || 100;
        const currentLen = $input.val().length;
        const $msg = $input.siblings('.char-limit-msg');
        if (currentLen >= maxLen) {
            $input.addClass('border-danger');
            $msg.removeClass('d-none').text('Maximum ' + maxLen +' characters limit reached!');
        } else {
            $input.removeClass('border-danger');
            $msg.addClass('d-none');
        }
    });

    $displayOrder.on('input', function (e) {
        const $input = $(this);
        const val = $input.val();
        const $msg = $input.siblings('.char-limit-msg');
        if (val !== '' && !/^\d+$/.test(val)) {
            $input.addClass('border-danger');
            $msg.removeClass('d-none').text('Please enter numbers only (0-9)!');
            return;
        }
        if (val.length > 3) {
            $input.addClass('border-danger');
            $msg.removeClass('d-none').text('Display Order cannot exceed 3 digits!');
        }
        else {
            $input.removeClass('border-danger');
            $msg.addClass('d-none');
        }
    });
    function focusAtEnd($input) {
        if (!$input || $input.length === 0) { return; }
        const element = $input[0];
        $input.focus();
        if (typeof element.selectionStart === 'number') {
            const length = $input.val().length;
            element.selectionStart = length;
            element.selectionEnd = length;
        }
    }
        $("#btnClear").click(function (e) {
            e.preventDefault();

            $menuForm.find('input[type="text"], ' + 'input[type="number"], ' + 'textarea').val('');
            $menuForm.find('select').prop('selectedIndex', 0);
            $typeParent.prop('checked', true);
            $statusActive.prop('checked', true);
            validator.resetForm();
            $menuForm.find('.field-validation-error')
                .removeClass('field-validation-error')
                .text('');
            $menuForm.find('.char-limit-msg')
                .addClass('d-none').text('');
            toggleParentMenu();
            setTimeout(function () {
                focusAtEnd($displayText);
            }, 100);
    });
}); 