const chartElement = document.querySelector('[data-weekly-progress-chart]');

if (chartElement && window.ApexCharts) {
  const points = JSON.parse(chartElement.dataset.points ?? '[]');
  const hasData = points.some((point) => point.completed > 0 || point.failed > 0 || point.skipped > 0);

  const chart = new window.ApexCharts(chartElement, {
    chart: {
      type: 'bar',
      height: 280,
      stacked: true,
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
      },
      {
        name: 'Pulados',
        data: points.map((point) => point.skipped)
      }
    ],
    colors: ['#34d399', '#fb7185', '#fbbf24'],
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
    },
    noData: {
      text: hasData ? undefined : 'Sem registros na semana'
    }
  });

  chart.render();
}
