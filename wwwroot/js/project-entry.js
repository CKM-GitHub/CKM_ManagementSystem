function initTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

function getOrCreateModal(elementId) {
    var el = document.getElementById(elementId);
    return bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
}

function showSuccessAlert(message, title = 'Success!') {
    $('#alertIcon').attr('class', 'alert-icon alert-success').html('<i class="bi bi-check-lg"></i>');
    $('#alertTitle').text(title);
    $('#alertMessage').text(message);
    $('#btnAlertOk').show().removeClass('d-none');
    $('#deleteButtons').addClass('d-none');
    getOrCreateModal('alertModal').show();
}

function showErrorAlert(message, title = 'Error!') {
    $('#alertIcon').attr('class', 'alert-icon alert-error').html('<i class="bi bi-x-lg"></i>');
    $('#alertTitle').text(title);
    $('#alertMessage').text(message);
    $('#btnAlertOk').show().removeClass('d-none');
    $('#deleteButtons').addClass('d-none');
    getOrCreateModal('alertModal').show();
}

$(document).ready(function () {
    initTooltips();

    var isEditMode = $('#IsEdit').val() === 'true' || $('#IsEdit').val() === 'True';
    $(isEditMode ? '#ProjectName' : '#ProjectCode').focus();

    $('#memberModal').on('shown.bs.modal', function () {
        $('#btnModalSearch').click();
    });

    $('#ProjectCode').on('input', function () {
        $(this).val($(this).val().replace(/[^a-zA-Z0-9\-_]/g, ''));
    });

    $('#ProjectName').on('input', function () {
        $(this).val($(this).val().replace(/[^a-zA-Z0-9\s\-_]/g, ''));
    });

    $('#ProjectCode').on('blur', function () {
        if (isEditMode) return;
        var val = $(this).val().trim();
        if (val === '') {
            $('#valProjectCode').text('');
            return;
        }

        $.post('/Project/CheckDuplicateProjectCode', { projectCode: val }, function (res) {
            $('#valProjectCode').text((res && res.isDuplicate) ? 'Project Code already exists.' : '');
        });
    });

    $('#ProjectName').on('blur', function () {
        var val = $(this).val().trim();
        var codeVal = isEditMode ? $('#HiddenProjectCode').val() : $('#ProjectCode').val().trim();
        if (val === '') {
            $('#valProjectName').text('');
            return;
        }

        $.post('/Project/CheckDuplicateProjectName', { projectName: val, projectCode: codeVal, isEdit: isEditMode }, function (res) {
            $('#valProjectName').text((res && res.isDuplicate) ? 'Project Name already exists.' : '');
        });
    });

    $('#StartDate, #EndDate').on('change blur', function () {
        var startVal = $('#StartDate').val();
        var endVal = $('#EndDate').val();

        if (startVal && endVal) {
            var startDate = new Date(startVal);
            var endDate = new Date(endVal);

            if (endDate < startDate) {
                $('#valEndDate').text('Target End Date cannot be earlier than Start Date.');
            } else {
                $('#valEndDate').text('');
            }
        }
    });

    function updateRowNumbers() {
        $('#tblSelectedMembers tbody tr[data-staffcode]').each(function (index) {
            $(this).find('.row-no').text(index + 1);
        });
    }

    $('#btnModalSearch').on('click', function () {
        var searchText = $('#modalSearchText').val();
        var deptCode = $('#modalDeptSelect').val();

        $.post('/Project/SearchProjectMembers', { searchText: searchText, departmentCode: deptCode }, function (data) {
            var $tbody = $('#tblModalMembers tbody').empty();
            if (data && data.length > 0) {
                $.each(data, function (i, item) {
                    var img = item.image_URL || item.image_Url || item.Image_URL || '/images/default-avatar.png';
                    var dept = item.department_Name || item.Department_Name || '-';
                    var staffCode = item.staff_Code || item.Staff_Code;
                    var name = item.name || item.Name;

                    var isAlreadyAdded = $('#tblSelectedMembers tbody tr[data-staffcode="' + staffCode + '"]').length > 0;

                    var row = `<tr>
                        <td class="text-center">
                            <input type="checkbox" class="form-check-input chk-member" value="${staffCode}"
                                   data-name="${name}" data-img="${img}" ${isAlreadyAdded ? 'disabled checked' : ''} />
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <img src="${img}" onerror="this.onerror=null;this.src='/images/default-avatar.png';" class="rounded-circle me-2 flex-shrink-0" width="28" height="28" />
                                <span class="fw-semibold text-dark text-truncate">${staffCode}</span>
                            </div>
                        </td>
                        <td>
                            <span class="text-truncate-custom cursor-pointer" data-bs-toggle="tooltip" data-bs-placement="top" title="${name}">
                                ${name}
                            </span>
                        </td>
                        <td>
                            <span class="text-truncate-custom cursor-pointer" data-bs-toggle="tooltip" data-bs-placement="top" title="${dept}">
                                ${dept}
                            </span>
                        </td>
                    </tr>`;
                    $tbody.append(row);
                });
                initTooltips();
            } else {
                $tbody.append('<tr><td colspan="4" class="text-center text-muted py-3">No users found.</td></tr>');
            }
        });
    });

    $('#btnAddSelectedMembers').on('click', function () {
        var selected = $('#tblModalMembers .chk-member:checked:not(:disabled)');
        if (selected.length === 0) {
            getOrCreateModal('memberModal').hide();
            return;
        }

        $('#emptyMemberRow').remove();

        selected.each(function () {
            var code = $(this).val();
            var name = $(this).data('name');
            var img = $(this).data('img');

            var row = `<tr data-staffcode="${code}" data-img="${img}">
                <td class="text-center text-muted row-no"></td>
                <td>
                    <div class="d-flex align-items-center">
                        <img src="${img}" onerror="this.onerror=null;this.src='/images/default-avatar.png';" class="rounded-circle me-2 flex-shrink-0" width="28" height="28" />
                        <span class="fw-semibold text-dark text-truncate">${code}</span>
                    </div>
                </td>
                <td class="text-secondary">
                    <span class="text-truncate-custom cursor-pointer" data-bs-toggle="tooltip" data-bs-placement="top" title="${name}">
                        ${name}
                    </span>
                </td>
                <td class="text-end">
                    <button type="button" class="btn btn-sm text-danger border-0 p-0 btn-remove-member me-2" title="Remove Member">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>`;
            $('#tblSelectedMembers tbody').append(row);
        });

        updateRowNumbers();
        initTooltips();
        getOrCreateModal('memberModal').hide();
    });

    $(document).on('click', '.btn-remove-member', function () {
        $(this).closest('tr').remove();

        if ($('#tblSelectedMembers tbody tr[data-staffcode]').length === 0) {
            $('#tblSelectedMembers tbody').html('<tr id="emptyMemberRow"><td colspan="4" class="text-center text-muted py-3">No project members added.</td></tr>');
        } else {
            updateRowNumbers();
        }
    });

    $('#btnClear').on('click', function () {
        if (isEditMode) {
            window.location.href = '/Project/Index';
        } else {
            $('#projectForm')[0].reset();
            $('span.text-danger').text('');
            $('#ProjectManagerId').val('');
            $('#statusActive').prop('checked', true);
            $('#tblSelectedMembers tbody').html('<tr id="emptyMemberRow"><td colspan="4" class="text-center text-muted py-3">No project members added.</td></tr>');
            $('#ProjectCode').focus();
        }
    });

    $('#projectForm').on('submit', function (e) {
        e.preventDefault();

        var $form = $(this);
        if ($.data($form[0], 'validator') && !$form.valid()) {
            return;
        }

        var codeVal = isEditMode ? $('#HiddenProjectCode').val() : $('#ProjectCode').val().trim();
        var nameVal = $('#ProjectName').val().trim();

        if (!isEditMode && codeVal === '') {
            $('#valProjectCode').text('Project Code is required.');
            return;
        }

        if ($('#valProjectCode').text() !== '' || $('#valProjectName').text() !== '' || $('#valEndDate').text() !== '') {
            return;
        }

        var startDate = new Date($('#StartDate').val());
        var endDate = new Date($('#EndDate').val());
        if (endDate < startDate) {
            $('#valEndDate').text('Target End Date cannot be earlier than Start Date.');
            return;
        }

        var membersList = [];
        $('#tblSelectedMembers tbody tr[data-staffcode]').each(function () {
            membersList.push({
                Staff_Code: String($(this).attr('data-staffcode')),
                Name: $(this).find('td:nth-child(3) span').text().trim(),
                Image_URL: $(this).attr('data-img') || ''
            });
        });

        var selectedStatus = $('input[name="Status"]:checked').val() || 'Active';

        var payload = {
            ProjectCode: codeVal,
            ProjectName: nameVal,
            ProjectManagerId: $('#ProjectManagerId').val(),
            GitRepositoryUrl: $('#GitRepositoryUrl').val(),
            Description: $('#Description').val(),
            StartDate: $('#StartDate').val(),
            EndDate: $('#EndDate').val(),
            Status: selectedStatus,
            IsEdit: isEditMode,
            ProjectMembers: membersList
        };

        var token = $('input[name="__RequestVerificationToken"]').val();
        var $btn = $('#btnRegister');
        $btn.prop('disabled', true).text(isEditMode ? 'Updating...' : 'Creating...');

        $.ajax({
            url: '/Project/SaveProject',
            type: 'POST',
            contentType: 'application/json',
            headers: {
                "RequestVerificationToken": token
            },
            data: JSON.stringify(payload),
            success: function (response) {
                $btn.prop('disabled', false).text(isEditMode ? 'Update Project' : '+ Create Project');
                if (response && response.success) {
                    showSuccessAlert(response.message, 'Success!');

                    $('#alertModal').one('hidden.bs.modal', function () {
                        if (isEditMode) {
                            window.location.href = '/Project/Index';
                        } else {
                            $('#btnClear').click();
                        }
                    });
                } else {
                    showErrorAlert(response ? response.message : 'Operation failed.', 'Error');
                }
            },
            error: function (xhr) {
                $btn.prop('disabled', false).text(isEditMode ? 'Update Project' : '+ Create Project');
                showErrorAlert(xhr.responseText || 'Request failed.', 'Server Error');
            }
        });
    });
});
document.querySelectorAll('.member-avatar-fallback').forEach(function (img) {
    img.addEventListener('error', function () {
        if (this.src.endsWith('/images/default-avatar.png')) {
            return;
        }

        this.src = '/images/default-avatar.png';
    });
});