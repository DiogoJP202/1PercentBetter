const chartElement = document.querySelector('[data-weekly-progress-chart]');

if (chartElement && window.ApexCharts) {
  const points = JSON.parse(chartElement.dataset.points ?? '[]');

  const chart = new window.ApexCharts(chartElement, {
    chart: {
      type: 'bar',
      height: 280,
      toolbar: { show: false },
      foreColor: '#cbd5e1'
    },
    series: [
      {
        name: 'Concluidos',
        data: points.map((point) => point.completed)
      },
      {
        name: 'Falhas',
        data: points.map((point) => point.failed)
      }
    ],
    colors: ['#34d399', '#fb7185'],
    plotOptions: {
      bar: {
        borderRadius: 7,
        columnWidth: '48%'
      }
    },
    grid: {
      borderColor: 'rgba(148, 163, 184, 0.14)'
    },
    xaxis: {
      categories: points.map((point) => point.label)
    },
    yaxis: {
      min: 0,
      allowDecimals: false
    },
    tooltip: {
      theme: 'dark'
    }
  });

  chart.render();
}
