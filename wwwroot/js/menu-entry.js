
$(document).ready(function () {
    if (typeof successMessage !== 'undefined' && successMessage !== '') {
        var modalElement = document.getElementById('successModal');
        if (modalElement) {
            var successModal = new bootstrap.Modal(modalElement);
            successModal.show();
            $(modalElement).on('hidden.bs.modal', function () {
                $('#DisplayText').focus();
            });
        }
    }
    else {
        setTimeout(function () {
            $('#DisplayText').focus();
        }, 100);
        
    }
    function toggleParentMenu() {
        var selectedType = $('input[name="MenuType"]:checked').val();
        if (selectedType === 'Sub') {
            $('#ParentMenu').prop('disabled', false);
            $('#hiddenParentMenu').prop('disabled', true);
                
        } else {
            $('#ParentMenu').val('0').prop('disabled', true);
            $('#hiddenParentMenu').prop('disabled', false);
        }
    }
    toggleParentMenu();

    $('input[name="MenuType"]').change(function () {
        toggleParentMenu();
        revalidateDisplayOrder();
    });
    $('#ParentMenu').change(function () {
        revalidateDisplayOrder();
    });
    $('#DisplayOrder').on('keyup input change', function () {
        revalidateDisplayOrder();
    });
    function revalidateDisplayOrder() {
        var form = $('#menuForm');
        if (form.data('validator') && $('#DisplayOrder').val() !== '') {
            form.validate().element('#DisplayOrder');
        }
    }
   
        $("#btnClear").click(function (e) {
            e.preventDefault();

            var form = $('#menuForm');

            form.find('input[type="text"], input[type= "number"], textarea').val('');
            form.find('select').prop('selectedIndex', 0);
            $('#typeParent').prop('checked', true);

            if (form.data('validator')) {
                form.data('validator').resetForm();
                form.find('.field-validation-error').empty();
                form.find('.input-validation-error').removeClass('input-validation-error');
            }
            setTimeout(function () {
                $('#typeParent').prop('checked', true);
                toggleParentMenu();
                $('#DisplayText').focus();
            }, 10);
    });
}); 