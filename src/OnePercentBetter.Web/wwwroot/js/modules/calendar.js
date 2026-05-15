const calendarElement = document.querySelector('[data-habit-calendar]');
const dayPanel = document.querySelector('[data-calendar-day-panel]');
const filterButtons = [...document.querySelectorAll('[data-calendar-filter]')];
const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

const statusClasses = {
  success: 'status-pill-success',
  warning: 'status-pill-warning',
  danger: 'status-pill-danger',
  info: 'status-pill-info',
  neutral: 'status-pill-neutral'
};

const escapeHtml = (value) => String(value ?? '')
  .replaceAll('&', '&amp;')
  .replaceAll('<', '&lt;')
  .replaceAll('>', '&gt;')
  .replaceAll('"', '&quot;')
  .replaceAll("'", '&#039;');

const getDateKey = (value) => {
  if (!value) {
    return '';
  }

  return value.substring(0, 10);
};

const getActiveTypes = () => filterButtons
  .filter((button) => button.getAttribute('aria-pressed') !== 'false')
  .map((button) => button.dataset.calendarFilter)
  .filter(Boolean);

const setText = (selector, value) => {
  const element = dayPanel?.querySelector(selector);
  if (element) {
    element.textContent = value ?? '-';
  }
};

const showMessage = (message, type = 'success') => {
  if (!window.Notyf || !message) {
    return;
  }

  const notyf = new window.Notyf({
    duration: 2600,
    position: { x: 'right', y: 'top' },
    types: [
      {
        type: 'error',
        background: '#f43f5e',
        icon: false
      }
    ]
  });

  if (type === 'success') {
    notyf.success(message);
    return;
  }

  notyf.open({ type, message });
};

const refreshIcons = () => {
  if (window.lucide) {
    window.lucide.createIcons();
  }
};

const buildEditUrl = (baseUrl, id) => `${baseUrl}?id=${encodeURIComponent(id)}`;
const buildCheckInUrl = (baseUrl, date) => `${baseUrl}?date=${encodeURIComponent(date)}`;

const renderImprovementHabits = (habits, date) => {
  const list = dayPanel?.querySelector('[data-calendar-improvement-list]');
  const empty = dayPanel?.querySelector('[data-calendar-improvement-empty]');
  const editUrl = dayPanel?.dataset.habitEditUrl;

  if (!list || !empty) {
    return;
  }

  empty.classList.toggle('hidden', habits.length > 0);

  list.innerHTML = habits.map((habit) => {
    const statusClass = statusClasses[habit.statusTone] ?? statusClasses.neutral;
    const meta = [
      habit.suggestedTime ? `${habit.suggestedTime}` : null,
      habit.goalTitle,
      habit.locationName
    ].filter(Boolean).join(' · ');

    return `
      <article class="rounded-2xl border border-white/10 bg-white/[0.03] p-3">
        <div class="flex items-start gap-3">
          <span class="grid h-10 w-10 shrink-0 place-items-center rounded-xl border border-white/10 bg-slate-950/50" style="color:${escapeHtml(habit.color)}">
            <i data-lucide="${escapeHtml(habit.icon)}" class="h-5 w-5"></i>
          </span>
          <div class="min-w-0 flex-1">
            <div class="flex flex-wrap items-center justify-between gap-2">
              <h4 class="min-w-0 flex-1 truncate text-sm font-black text-white">${escapeHtml(habit.title)}</h4>
              <span class="status-pill ${statusClass}">${escapeHtml(habit.statusLabel)}</span>
            </div>
            ${meta ? `<p class="mt-1 text-xs text-slate-500">${escapeHtml(meta)}</p>` : ''}
            ${habit.identityName ? `<p class="mt-1 text-xs text-slate-400">${escapeHtml(habit.identityName)}</p>` : ''}
            <div class="mt-3 flex flex-wrap gap-2">
              ${habit.isCompleted ? '' : `<button type="button" class="btn-primary min-h-0 px-3 py-1 text-xs" data-calendar-complete-habit="${habit.id}" data-calendar-date="${date}"><i data-lucide="check" class="h-3.5 w-3.5"></i>Concluir</button>`}
              ${editUrl ? `<a class="btn-secondary min-h-0 px-3 py-1 text-xs" href="${buildEditUrl(editUrl, habit.id)}"><i data-lucide="pencil" class="h-3.5 w-3.5"></i>Editar</a>` : ''}
            </div>
          </div>
        </div>
      </article>
    `;
  }).join('');
};

const renderCommonHabits = (habits) => {
  const list = dayPanel?.querySelector('[data-calendar-common-list]');
  const empty = dayPanel?.querySelector('[data-calendar-common-empty]');

  if (!list || !empty) {
    return;
  }

  empty.classList.toggle('hidden', habits.length > 0);

  list.innerHTML = habits.map((habit) => `
    <article class="rounded-2xl border border-violet-300/20 bg-violet-300/10 p-3">
      <div class="flex items-center gap-3">
        <span class="grid h-9 w-9 place-items-center rounded-xl border border-violet-300/20 bg-slate-950/40 text-violet-200">
          <i data-lucide="calendar-clock" class="h-4 w-4"></i>
        </span>
        <div class="min-w-0">
          <h4 class="truncate text-sm font-black text-white">${escapeHtml(habit.name)}</h4>
          <p class="mt-1 text-xs text-violet-100/80">${habit.scheduledTime ? `Horário: ${escapeHtml(habit.scheduledTime)}` : 'Sem horário definido'}</p>
        </div>
      </div>
    </article>
  `).join('');
};

const renderCheckIn = (checkIn, date) => {
  const card = dayPanel?.querySelector('[data-calendar-checkin-card]');
  const action = dayPanel?.querySelector('[data-calendar-checkin-action]');
  const checkInUrl = dayPanel?.dataset.checkinEditUrl;

  if (!card) {
    return;
  }

  if (action && checkInUrl) {
    action.href = buildCheckInUrl(checkInUrl, date);
    action.textContent = checkIn ? 'Editar check-in' : 'Criar check-in';
  }

  if (!checkIn) {
    card.innerHTML = `
      <div class="rounded-2xl border border-dashed border-white/15 p-4 text-sm text-slate-400">
        Nenhum check-in registrado para este dia.
      </div>
    `;
    return;
  }

  card.innerHTML = `
    <article class="rounded-2xl border border-violet-300/20 bg-violet-300/10 p-4">
      <div class="flex items-start gap-3">
        <span class="text-3xl leading-none" aria-hidden="true">${escapeHtml(checkIn.moodFace)}</span>
        <div class="min-w-0">
          <div class="text-xs font-semibold uppercase tracking-wide text-violet-100/80">Check-in registrado</div>
          <div class="mt-1 text-lg font-black text-white">${escapeHtml(checkIn.totalScore)}/15 · ${escapeHtml(checkIn.moodLabel)}</div>
          ${checkIn.smallWin ? `<p class="mt-2 text-sm text-violet-50">${escapeHtml(checkIn.smallWin)}</p>` : ''}
          ${checkIn.mainDifficulty ? `<p class="mt-1 text-xs text-violet-100/80">${escapeHtml(checkIn.mainDifficulty)}</p>` : ''}
        </div>
      </div>
    </article>
  `;
};

const renderDay = (detail) => {
  if (!dayPanel || !detail) {
    return;
  }

  const date = getDateKey(detail.date);
  dayPanel.dataset.selectedDate = date;

  setText('[data-calendar-day-title]', detail.dateLabel || 'Detalhes do dia');
  setText('[data-calendar-day-subtitle]', detail.plannedCount > 0 ? 'Veja o que estava planejado e o que já foi registrado.' : 'Nenhum hábito de melhoria planejado para esta data.');
  setText('[data-calendar-day-date]', date);
  setText('[data-calendar-day-planned]', detail.plannedCount);
  setText('[data-calendar-day-completed]', detail.completedCount);
  setText('[data-calendar-day-pending]', detail.pendingCount);

  renderImprovementHabits(detail.improvementHabits ?? [], date);
  renderCommonHabits(detail.commonHabits ?? []);
  renderCheckIn(detail.checkIn, date);
  refreshIcons();
};

const loadDay = async (date) => {
  const dayUrl = calendarElement?.dataset.dayUrl;
  if (!dayUrl || !date) {
    return;
  }

  const url = new URL(dayUrl, window.location.origin);
  url.searchParams.set('date', date);

  const response = await fetch(url, {
    headers: {
      'X-Requested-With': 'XMLHttpRequest'
    }
  });

  if (!response.ok) {
    showMessage('Não foi possível carregar os detalhes do dia.', 'error');
    return;
  }

  renderDay(await response.json());
};

const updateHabitStatus = async (habitId, date, status = 'Completed') => {
  const statusUrl = dayPanel?.dataset.statusUrl;
  const token = tokenInput?.value;

  if (!statusUrl || !habitId || !date) {
    return;
  }

  const payload = new FormData();
  payload.append('id', habitId);
  payload.append('date', date);
  payload.append('status', status);

  if (token) {
    payload.append('__RequestVerificationToken', token);
  }

  const response = await fetch(statusUrl, {
    method: 'POST',
    body: payload,
    headers: {
      'X-Requested-With': 'XMLHttpRequest'
    }
  });

  const result = await response.json().catch(() => ({}));

  if (!response.ok) {
    showMessage(result.error || 'Não foi possível atualizar o hábito.', 'error');
    return;
  }

  showMessage(result.message || 'Hábito atualizado.');
  window.__calendarInstance?.refetchEvents();
  await loadDay(date);
};

dayPanel?.addEventListener('click', (event) => {
  const completeButton = event.target.closest('[data-calendar-complete-habit]');
  if (!completeButton) {
    return;
  }

  updateHabitStatus(completeButton.dataset.calendarCompleteHabit, completeButton.dataset.calendarDate);
});

filterButtons.forEach((button) => {
  button.addEventListener('click', () => {
    const isPressed = button.getAttribute('aria-pressed') !== 'false';
    button.setAttribute('aria-pressed', isPressed ? 'false' : 'true');
    button.classList.toggle('opacity-60', isPressed);
    window.__calendarInstance?.refetchEvents();
  });
});

if (calendarElement && window.FullCalendar) {
  const eventsUrl = calendarElement.dataset.eventsUrl;
  const today = calendarElement.dataset.today;

  const calendar = new window.FullCalendar.Calendar(calendarElement, {
    initialView: 'dayGridMonth',
    height: 'auto',
    locale: 'pt-br',
    eventDisplay: 'block',
    nowIndicator: true,
    selectable: true,
    dayMaxEvents: 4,
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,listWeek'
    },
    buttonText: {
      today: 'Hoje',
      month: 'Mês',
      week: 'Semana',
      list: 'Lista'
    },
    noEventsContent: 'Nenhum item encontrado neste período.',
    events: async (fetchInfo, successCallback, failureCallback) => {
      if (!eventsUrl) {
        successCallback([]);
        return;
      }

      const url = new URL(eventsUrl, window.location.origin);
      url.searchParams.set('start', getDateKey(fetchInfo.startStr));
      url.searchParams.set('end', getDateKey(fetchInfo.endStr));
      url.searchParams.set('types', getActiveTypes().join(','));

      try {
        const response = await fetch(url, {
          headers: {
            'X-Requested-With': 'XMLHttpRequest'
          }
        });

        if (!response.ok) {
          failureCallback(new Error('Falha ao carregar eventos.'));
          return;
        }

        successCallback(await response.json());
      } catch (error) {
        failureCallback(error);
      }
    },
    dateClick: (info) => {
      loadDay(info.dateStr);
    },
    eventClick: (info) => {
      info.jsEvent.preventDefault();
      loadDay(info.event.extendedProps.date || getDateKey(info.event.startStr));
    },
    eventContent: (arg) => ({
      html: `
        <span class="calendar-event-content">
          <span class="calendar-event-dot"></span>
          <span class="calendar-event-title">${escapeHtml(arg.event.title)}</span>
        </span>
      `
    }),
    eventDidMount: (info) => {
      const props = info.event.extendedProps;
      const details = [props.typeLabel, props.statusLabel, props.notes].filter(Boolean).join(' · ');
      info.el.setAttribute('title', details || info.event.title);
    }
  });

  window.__calendarInstance = calendar;
  calendar.render();
  loadDay(today);
}
