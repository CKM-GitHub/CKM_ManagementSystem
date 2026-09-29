document.addEventListener("DOMContentLoaded", function () {

    const chartCanvas = document.getElementById('tasksChart');

    if (chartCanvas) {
        const chartDataStr = chartCanvas.getAttribute('data-chart');

        if (chartDataStr) {
            const chartData = JSON.parse(chartDataStr);

            const colorPalette = chartData.colors || ['#3B82F6', '#F59E0B', '#8B5CF6', '#EF4444', '#10B981', '#6366F1'];

            new Chart(chartCanvas, {
                type: 'doughnut',
                data: {
                    labels: chartData.labels,
                    datasets: [{
                        data: chartData.values,
                        backgroundColor: colorPalette,
                        borderWidth: 0,
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    cutout: '75%',
                    plugins: {
                        legend: { display: false },
                        tooltip: {
                            enabled: true,
                            position: 'nearest', 
                            yAlign: 'bottom',  
                            backgroundColor: '#1E293B',
                            padding: 10,
                            cornerRadius: 6,
                            displayColors: false,
                            caretSize: 5,
                            caretPadding: 10 
                        }
                    },
                    animation: {
                        animateScale: true,
                        animateRotate: true
                    }
                }
            });
        }
    }

    const viewButtons = document.querySelectorAll('.btn-view');
    const announcementModalElement =
        document.getElementById('announcementModal');

    if (announcementModalElement) {

        const announcementModal =
            new bootstrap.Modal(announcementModalElement);

        const modalTitle =
            document.getElementById('announcementModalLabel');

        const modalName =
            document.getElementById('announcementName');

        const modalDept =
            document.getElementById('announcementDepartment');

        const modalContent =
            document.getElementById('announcementContent');

        viewButtons.forEach(button => {

            button.addEventListener('click', function () {

                const annId =
                    this.getAttribute('data-announcement-id');

                const annTitle =
                    this.getAttribute('data-title');

                modalTitle.textContent = annTitle;

                if (annId === "1" ||
                    annTitle.includes("Company rules")) {

                    modalName.textContent = "Daw Htet";
                    modalDept.textContent = "HR Department";

                    modalContent.textContent =
                        "Please review the updated leave application policies effective next month. All medical leave must be submitted within 48 hours with proper documentation.";

                } else {

                    modalName.textContent = "Admin User";
                    modalDept.textContent = "System";

                    modalContent.textContent =
                        "This is a placeholder description for the announcement: "
                        + annTitle;
                }

                announcementModal.show();
            });
        });
    }
});