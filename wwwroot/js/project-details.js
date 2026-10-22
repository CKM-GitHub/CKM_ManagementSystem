let currentProjectCode = '';

document.querySelectorAll('.view-details-btn').forEach(button => {
    button.addEventListener('click', function () {
        currentProjectCode = this.getAttribute('data-code');
        loadProjectDetails(currentProjectCode, 1);
    });
});

function loadProjectDetails(projectCode, pageNumber) {
    const detailsUrl = window.projectDetailsUrl;

    if (!detailsUrl) {
        console.error('Project details URL is missing.');
        return;
    }

    const params = new URLSearchParams({
        projectCode: projectCode,
        pageNumber: pageNumber,
        pageSize: 5
    });
    fetch(`${window.projectDetailsUrl}?${params.toString()}`)
        .then(response => response.json())
        .then(data => {
            if (!data.success) {
                alert(data.message);
                return;
            }

            const info = Array.isArray(data.projectInfo) ? data.projectInfo[0] : (data.projectInfo.rows ? data.projectInfo.rows[0] : data.projectInfo);

            document.getElementById('modalProjectName').innerText = info.ProjectName || info.projectName || '-';
            document.getElementById('modalManagerName').innerText = info.ProjectManagerName || info.projectManagerName || '-';
            document.getElementById('modalManagerCode').innerText = `(${info.ProjectManagerStaffCode || info.projectManagerStaffCode || ''})`;

            const managerImg = info.ProjectManagerImageURL || info.projectManagerImageURL;
            const img = document.getElementById('modalManagerImage');
            if (managerImg) {
                img.src = managerImg;
                img.style.display = 'block';
            } else {
                img.style.display = 'none';
            }

            const gitUrl = info.GitRepositoryUrl || info.gitRepositoryUrl;
            const gitLink = document.getElementById('modalGitUrl');
            if (gitUrl) {
                gitLink.href = gitUrl;
                gitLink.innerText = gitUrl;
                gitLink.setAttribute('target', '_blank');
            } else {
                gitLink.href = '#';
                gitLink.innerText = 'No Repository';
                gitLink.removeAttribute('target');
            }

            document.getElementById('modalDescription').innerText = info.Description || info.description || '-';

            const rawStart = info.StartDate || info.startDate;
            const rawEnd = info.EndDate || info.endDate;
            const startDate = rawStart ? new Date(rawStart).toLocaleDateString() : '';
            const endDate = rawEnd ? new Date(rawEnd).toLocaleDateString() : '';
            document.getElementById('modalDateRange').innerText = startDate && endDate ? `${startDate} ~ ${endDate}` : '-';

            let membersHtml = '';
            const members = data.projectMembers.rows || data.projectMembers;
            if (members && members.length > 0) {
                members.forEach((m, index) => {
                    const memberImg = m.Image_URL || m.image_URL || '/images/default-avatar.png';
                    const staffCode = m.Staff_Code || m.staff_Code || '';
                    const staffName = m.Staff_Name || m.staff_Name || '';

                    const rowNo = m.NO || m.no || (index + 1);
                    membersHtml += `
                                        <tr>
                                            <td class="text-center">${rowNo}</td>
                                            <td>
                                                <div class="d-flex align-items-center">
                                                    <img src="${memberImg}" class="avatar-img me-2 modal-member-avatar" alt="Avatar">
                                                    <span class="text-limit-menu" data-bs-toggle="tooltip" title="${staffCode}">${staffCode}</span>
                                                </div>
                                            </td>
                                            <td>
                                                <span class="text-limit-menu" data-bs-toggle="tooltip" title="${staffName}">${staffName}</span>
                                            </td>
                                        </tr>
                                    `;
                });
            } else {
                membersHtml = `<tr><td colspan="3" class="text-center text-muted py-3">No found project members.</td></tr>`;
            }
            document.getElementById('modalMembersTableBody').innerHTML = membersHtml;
            renderPaginationControls(data.currentPage, data.totalPages, data.totalMembers);

            var modalElement = document.getElementById('projectDetailsModal');
            var myModal = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
            myModal.show();
            var tooltipTriggerList = [].slice.call(modalElement.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        })
        .catch(error => console.error('Error loading project details:', error));
}

function renderPaginationControls(currentPage, totalPages, totalMembers, pageSize) {
    const container = document.getElementById('memberPaginationContainer');
    const count = Number(totalMembers) || 0;
    const currPage = Number(currentPage) || 1;
    const tPages = Number(totalPages) || 1;
    const size = Number(pageSize) || 5;

    const startItem = count === 0 ? 0 : ((currPage - 1) * size) + 1;
    const endItem = count === 0 ? 0 : Math.min(currPage * size, count);

    let paginationHtml = `
                        <div class="footer-text">
                            Showing ${startItem}-${endItem} of ${count} Members
                        </div>
                    `;

    if (tPages > 1) {
        paginationHtml += `<div class="pagination-box">`;
        const prevDisabled = currPage <= 1 ? 'disabled' : '';
        paginationHtml += `
                            <button type="button" class="page-btn btn btn-sm btn-outline-secondary ${prevDisabled}"
                                    ${currPage <= 1 ? 'disabled' : `onclick="changePage(${currPage - 1})"`}>
                                <i class="bi bi-chevron-left"></i>
                            </button>
                        `;

        for (let i = 1; i <= tPages; i++) {
            const activeClass = i === currPage ? 'btn-primary-pagination active' : 'btn-outline-secondary';
            paginationHtml += `
                                <button type="button" class="page-btn btn btn-sm ${activeClass}" onclick="changePage(${i})">
                                    ${i}
                                </button>
                            `;
        }

        const nextDisabled = currPage >= tPages ? 'disabled' : '';
        paginationHtml += `
                            <button type="button" class="page-btn btn btn-sm btn-outline-secondary ${nextDisabled}"
                                    ${currPage >= tPages ? 'disabled' : `onclick="changePage(${currPage + 1})"`}>
                                <i class="bi bi-chevron-right"></i>
                            </button>
                        `;

        paginationHtml += `</div>`;
    }

    container.className = 'table-footer d-flex justify-content-between align-items-center mt-3';
    container.innerHTML = paginationHtml;
}

window.changePage = function (pageNumber) {
    if (currentProjectCode) {
        loadProjectDetails(currentProjectCode, pageNumber);
    }
};
