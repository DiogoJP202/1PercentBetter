const chartElement = document.querySelector('[data-checkins-chart]');
const detailPanel = document.querySelector('[data-checkin-detail-panel]');

const moodProfiles = {
  1: { face: '😞', label: 'Muito ruim' },
  2: { face: '🙁', label: 'Ruim' },
  3: { face: '😐', label: 'Neutro' },
  4: { face: '🙂', label: 'Bom' },
  5: { face: '😄', label: 'Muito bom' },
  VeryBad: { face: '😞', label: 'Muito ruim' },
  Bad: { face: '🙁', label: 'Ruim' },
  Neutral: { face: '😐', label: 'Neutro' },
  Good: { face: '🙂', label: 'Bom' },
  VeryGood: { face: '😄', label: 'Muito bom' }
};

const formatDate = (value) => {
  if (!value) {
    return '';
  }

  const date = new Date(value);
  return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short' }).format(date);
};

const getDateKey = (value) => {
  if (!value) {
    return '';
  }

  return value.substring(0, 10);
};

const setText = (selector, value) => {
  const element = detailPanel?.querySelector(selector);
  if (element) {
    element.textContent = value || '-';
  }
};

const setMood = (mood, exists) => {
  const faceElement = detailPanel?.querySelector('[data-checkin-detail-mood-face]');
  const labelElement = detailPanel?.querySelector('[data-checkin-detail-mood-label]');

  if (!faceElement || !labelElement) {
    return;
  }

  if (!exists) {
    faceElement.textContent = '-';
    labelElement.textContent = 'Sem humor registrado';
    return;
  }

  const profile = moodProfiles[mood] ?? moodProfiles[Number.parseInt(mood, 10)] ?? moodProfiles[3];
  faceElement.textContent = profile.face;
  labelElement.textContent = profile.label;
};

const setDetailAction = (date, exists) => {
  const action = detailPanel?.querySelector('[data-checkin-detail-action]');
  const editUrl = detailPanel?.dataset.editUrl;

  if (!action || !editUrl || !date) {
    return;
  }

  action.href = `${editUrl}?date=${encodeURIComponent(date)}`;
  action.innerHTML = `<i data-lucide="${exists ? 'pencil' : 'plus'}" class="h-4 w-4"></i>${exists ? 'Editar check-in' : 'Criar check-in'}`;

  if (window.lucide) {
    window.lucide.createIcons();
  }
};

const renderDetail = (detail) => {
  if (!detailPanel || !detail) {
    return;
  }

  const dateKey = getDateKey(detail.date);
  const title = detail.exists ? `Check-in de ${formatDate(detail.date)}` : `Sem check-in em ${formatDate(detail.date)}`;
  const status = detail.exists ? 'Registrado' : 'Pendente';
  const statusElement = detailPanel.querySelector('[data-checkin-detail-status]');

  setText('[data-checkin-detail-title]', title);
  setText('[data-checkin-detail-subtitle]', detail.exists ? 'Detalhes registrados para este dia.' : 'Você pode criar um check-in para esta data.');
  setText('[data-checkin-detail-day-score]', detail.exists ? detail.dayScore : '-');
  setText('[data-checkin-detail-energy]', detail.exists ? detail.energyLevel : '-');
  setText('[data-checkin-detail-productivity]', detail.exists ? detail.productivityLevel : '-');
  setText('[data-checkin-detail-total]', detail.exists ? `${detail.totalScore}/15` : '-');
  setText('[data-checkin-detail-tasks-planned]', detail.plannedTasks);
  setText('[data-checkin-detail-tasks-completed]', detail.completedTasks);
  setText('[data-checkin-detail-tasks-postponed]', detail.postponedTasks);
  setText('[data-checkin-detail-tasks-pending]', detail.pendingTasks);
  setText('[data-checkin-detail-win]', detail.smallWin);
  setText('[data-checkin-detail-difficulty]', detail.mainDifficulty);
  setText('[data-checkin-detail-task-blocker]', detail.taskBlocker);
  setText('[data-checkin-detail-adjustment]', detail.tomorrowAdjustment);
  setText('[data-checkin-detail-notes]', detail.notes);
  setMood(detail.mood, detail.exists);

  if (statusElement) {
    statusElement.textContent = status;
    statusElement.classList.toggle('status-pill-success', detail.exists);
    statusElement.classList.toggle('status-pill-warning', !detail.exists);
    statusElement.classList.remove('status-pill-neutral');
  }

  setDetailAction(dateKey, detail.exists);
};

const renderSummary = (point) => {
  if (!detailPanel || !point) {
    return;
  }

  const action = detailPanel.querySelector('[data-checkin-detail-action]');

  setText('[data-checkin-detail-title]', `Resumo de ${point.label}`);
  setText('[data-checkin-detail-subtitle]', point.summary);
  setText('[data-checkin-detail-day-score]', '-');
  setText('[data-checkin-detail-energy]', '-');
  setText('[data-checkin-detail-productivity]', '-');
  setText('[data-checkin-detail-total]', point.hasCheckIn ? `${point.score}/15` : '-');
  setText('[data-checkin-detail-tasks-planned]', '-');
  setText('[data-checkin-detail-tasks-completed]', '-');
  setText('[data-checkin-detail-tasks-postponed]', '-');
  setText('[data-checkin-detail-tasks-pending]', '-');
  setText('[data-checkin-detail-win]', point.summary);
  setText('[data-checkin-detail-difficulty]', '-');
  setText('[data-checkin-detail-task-blocker]', '-');
  setText('[data-checkin-detail-adjustment]', '-');
  setText('[data-checkin-detail-notes]', '-');
  setMood(null, false);

  if (action) {
    action.href = '#';
    action.textContent = 'Selecione um dia no modo mensal';
  }
};

const selectedDetailJson = document.querySelector('[data-checkin-selected-detail-json]');

if (selectedDetailJson?.textContent) {
  renderDetail(JSON.parse(selectedDetailJson.textContent));
}

if (chartElement && window.ApexCharts) {
  const points = JSON.parse(chartElement.dataset.points ?? '[]');
  const detailUrl = chartElement.dataset.detailUrl;
  const period = chartElement.dataset.period;
  const hasData = points.some((point) => point.hasCheckIn);
  const seriesName = period === 'month' ? 'Nota total do dia' : 'Média do período';
  const seriesData = points.map((point) => {
    const value = Number(point.score);
    return Number.isFinite(value) ? value : 0;
  });

  const missingMarkers = points
    .map((point, index) => (point.hasCheckIn
      ? null
      : {
          seriesIndex: 0,
          dataPointIndex: index,
          fillColor: '#0f172a',
          strokeColor: '#64748b',
          size: 3
        }))
    .filter((marker) => marker !== null);

  const chart = new window.ApexCharts(chartElement, {
    chart: {
      type: 'line',
      height: 360,
      toolbar: { show: false },
      foreColor: '#cbd5e1',
      events: {
        dataPointSelection: async (_event, _chartContext, config) => {
          const point = points[config.dataPointIndex];
          if (!point) {
            return;
          }

          if (!point.date || !detailUrl) {
            renderSummary(point);
            return;
          }

          const response = await fetch(`${detailUrl}?date=${encodeURIComponent(getDateKey(point.date))}`, {
            headers: {
              'X-Requested-With': 'XMLHttpRequest'
            }
          });

          if (!response.ok) {
            return;
          }

          renderDetail(await response.json());
        }
      }
    },
    series: [
      {
        name: seriesName,
        data: seriesData
      }
    ],
    colors: ['#34d399'],
    stroke: {
      curve: 'smooth',
      width: 3
    },
    markers: {
      size: 4,
      strokeWidth: 2,
      strokeColors: '#0f172a',
      hover: {
        sizeOffset: 2
      },
      discrete: missingMarkers
    },
    fill: {
      type: 'gradient',
      gradient: {
        shade: 'dark',
        shadeIntensity: 0.2,
        opacityFrom: 0.28,
        opacityTo: 0.04,
        stops: [0, 100]
      }
    },
    states: {
      hover: {
        filter: {
          type: 'lighten',
          value: 0.08
        }
      }
    },
    grid: {
      borderColor: 'rgba(148, 163, 184, 0.14)'
    },
    xaxis: {
      categories: points.map((point) => point.label),
      labels: {
        rotate: period === 'month' ? -45 : 0
      },
      axisBorder: {
        color: 'rgba(148, 163, 184, 0.2)'
      },
      axisTicks: {
        color: 'rgba(148, 163, 184, 0.2)'
      }
    },
    yaxis: {
      min: 0,
      max: 15,
      tickAmount: 5,
      labels: {
        formatter: (value) => (Number.isInteger(value) ? value : value.toFixed(1))
      }
    },
    tooltip: {
      theme: 'dark',
      intersect: true,
      shared: false,
      y: {
        formatter: (value, { dataPointIndex }) => {
          const point = points[dataPointIndex];
          return point?.hasCheckIn ? `${value}/15 - ${point.summary}` : 'Sem check-in';
        }
      }
    },
    dataLabels: {
      enabled: false
    },
    legend: {
      show: false
    },
    noData: {
      text: hasData ? undefined : 'Você ainda não possui check-ins neste período.'
    }
  });

  chart.render();
}
