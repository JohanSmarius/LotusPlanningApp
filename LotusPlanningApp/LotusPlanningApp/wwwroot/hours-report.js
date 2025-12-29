// Chart.js integration for Hours Report

let hoursChart = null;

window.renderHoursChart = function(chartDataJson) {
    const ctx = document.getElementById('hoursChart');
    if (!ctx) {
        console.error('Canvas element not found');
        return;
    }

    const chartData = JSON.parse(chartDataJson);

    // Destroy existing chart if it exists
    if (hoursChart) {
        hoursChart.destroy();
    }

    // Create new chart
    hoursChart = new Chart(ctx, {
        type: 'doughnut',
        data: chartData,
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        font: {
                            size: 12
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const label = context.label || '';
                            const value = context.parsed;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((value / total) * 100).toFixed(1);
                            return label + ': ' + value.toFixed(1) + ' uur (' + percentage + '%)';
                        }
                    }
                }
            }
        }
    });
};
