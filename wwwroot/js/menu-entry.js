
$(document).ready(function () {
    if (typeof successMessage !== 'undefined' && successMessage !== '') {
        var modalElement = document.getElementById('successModal');
        if (modalElement) {
            var successModal = new bootstrap.Modal(modalElement);
            successModal.show();
        }
    }
    $('#DisplayText').focus();
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
        if ($('#DisplayOrder').val() !== '') {
            form.validate().element('#DisplayOrder');
        }
    }
   
    $("#btnClear").click(function () {
        setTimeout(function () {
            $('#typeParent').prop('checked', true);
            toggleParentMenu();
            $('#DisplayText').focus();
        }, 10);
    });
}); 