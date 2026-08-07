
$(document).ready(function () {
    const $menuForm = $('#menuForm');
    const $displayText = $('#DisplayText');
    const $parentMenu = $('#ParentMenu');
    const $hiddenParentMenu = $('#hiddenParentMenu');
    const $displayOrder = $('#DisplayOrder');
    const $typeParent = $('#typeParent');

    if (typeof successMessage !== 'undefined' && successMessage !== '') {
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
    function toggleParentMenu() {
        const selectedType = $('input[name="MenuType"]:checked').val();
        if (selectedType === 'Sub') {
            $parentMenu.prop('disabled', false).removeClass('bg-light');
            $hiddenParentMenu.prop('disabled', true);
                
        } else {
            $parentMenu.val('0').prop('disabled', true).addClass('bg-light');
            $hiddenParentMenu.prop('disabled', false);
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
    });
    $displayOrder.on('keyup input change', function () {
        revalidateDisplayOrder();
    });
    
   
        $("#btnClear").click(function (e) {
            e.preventDefault();

            $menuForm.find('input[type="text"], input[type= "number"], textarea').val('');
            $menuForm.find('select').prop('selectedIndex', 0);
            $typeParent.prop('checked', true);

            if ($menuForm.data('validator')) {
                $menuForm.data('validator').resetForm();
                $menuForm.find('.field-validation-error')
                    .removeClass('.field-validation-error')
                    .addClass('.field-validation-valid')
                    .empty();

                $menuForm.find('.input-validation-error').removeClass('input-validation-error');
            }
            setTimeout(function () {
                toggleParentMenu();
                $displayText.focus();
            }, 10);
    });
}); 