const getActionKind = (form) => {
  if (form.dataset.actionKind) {
    return form.dataset.actionKind.toLowerCase();
  }

  try {
    const path = new URL(form.action, window.location.origin).pathname;
    const tail = path.split('/').filter(Boolean).pop();
    return (tail || '').toLowerCase();
  } catch {
    return '';
  }
};

const isQuickActionForm = (form) => {
  const method = (form.method || '').toUpperCase();
  if (method !== 'POST') {
    return false;
  }

  const actionKind = getActionKind(form);
  return ['complete', 'fail', 'skip', 'cancel', 'postpone'].includes(actionKind)
    || form.hasAttribute('data-async-action');
};

const asyncForms = [...document.querySelectorAll('form')].filter(isQuickActionForm);

if (asyncForms.length > 0) {
  const taskStatusClasses = [
    'status-pill-warning',
    'status-pill-info',
    'status-pill-success',
    'status-pill-danger',
    'status-pill-neutral'
  ];

  const habitStatusClasses = [
    'status-pill-warning',
    'status-pill-info',
    'status-pill-success',
    'status-pill-danger',
    'status-pill-neutral'
  ];

  const taskStatusMap = {
    complete: { key: 'completed', label: 'Concluída', className: 'status-pill-success' },
    cancel: { key: 'cancelled', label: 'Cancelada', className: 'status-pill-danger' },
    postpone: { key: 'postponed', label: 'Adiada', className: 'status-pill-neutral' }
  };

  const habitStatusMap = {
    complete: { label: 'Concluído', className: 'status-pill-success', icon: 'check' },
    fail: { label: 'Falhou', className: 'status-pill-danger', icon: 'x' },
    skip: { label: 'Pulado', className: 'status-pill-warning', icon: 'skip-forward' }
  };

  const defaultMessages = {
    complete: 'Atualizado com sucesso.',
    fail: 'Atualizado com sucesso.',
    skip: 'Atualizado com sucesso.',
    cancel: 'Atualizado com sucesso.',
    postpone: 'Atualizado com sucesso.'
  };

  const getNotyf = () => {
    if (!window.Notyf) {
      return null;
    }

    if (!window.__dailyActionNotyf) {
      window.__dailyActionNotyf = new window.Notyf({
        duration: 2600,
        position: { x: 'right', y: 'top' },
        types: [{ type: 'error', background: '#f43f5e', icon: false }]
      });
    }

    return window.__dailyActionNotyf;
  };

  const notify = (message, type = 'success') => {
    if (!message) {
      return;
    }

    const notyf = getNotyf();
    if (!notyf) {
      return;
    }

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

  const setSubmitting = (form, submitting) => {
    form.dataset.submitting = submitting ? 'true' : 'false';

    form.querySelectorAll('button').forEach((button) => {
      button.disabled = submitting;
      button.classList.toggle('opacity-70', submitting);
      button.classList.toggle('cursor-not-allowed', submitting);
    });
  };

  const postForm = async (form) => {
    const response = await fetch(form.action, {
      method: form.method || 'POST',
      body: new FormData(form),
      headers: {
        'X-Requested-With': 'XMLHttpRequest'
      }
    });

    if (!response.ok) {
      throw new Error('request_failed');
    }
  };

  const toggleTaskEmptyStates = (board) => {
    const openList = board.querySelector('[data-task-open-today-list]');
    const resolvedList = board.querySelector('[data-task-resolved-today-list]');
    const openEmpty = board.querySelector('[data-task-open-empty]');
    const resolvedEmpty = board.querySelector('[data-task-resolved-empty]');

    if (openList && openEmpty) {
      const hasOpenCards = !!openList.querySelector('[data-task-card]');
      openEmpty.classList.toggle('hidden', hasOpenCards);
    }

    if (resolvedList && resolvedEmpty) {
      const hasResolvedCards = !!resolvedList.querySelector('[data-task-card]');
      resolvedEmpty.classList.toggle('hidden', hasResolvedCards);
    }
  };

  const updateOverdueCounterTone = (counterElement, overdueCount) => {
    if (!counterElement) {
      return;
    }

    counterElement.classList.toggle('text-rose-300', overdueCount > 0);
    counterElement.classList.toggle('text-slate-200', overdueCount <= 0);
  };

  const parseIsoDate = (value) => {
    if (!value || !/^\d{4}-\d{2}-\d{2}$/.test(value)) {
      return null;
    }

    const [year, month, day] = value.split('-').map(Number);
    if (!year || !month || !day) {
      return null;
    }

    return new Date(year, month - 1, day);
  };

  const formatIsoDate = (value) => {
    const parsedDate = parseIsoDate(value);
    if (!parsedDate) {
      return '';
    }

    const day = String(parsedDate.getDate()).padStart(2, '0');
    const month = String(parsedDate.getMonth() + 1).padStart(2, '0');
    const year = parsedDate.getFullYear();
    return `${day}/${month}/${year}`;
  };

  const toIsoDate = (dateValue) => {
    const year = dateValue.getFullYear();
    const month = String(dateValue.getMonth() + 1).padStart(2, '0');
    const day = String(dateValue.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  };

  const addDays = (isoDate, days) => {
    const parsedDate = parseIsoDate(isoDate);
    if (!parsedDate) {
      return isoDate;
    }

    parsedDate.setDate(parsedDate.getDate() + days);
    return toIsoDate(parsedDate);
  };

  const isOpenTaskStatus = (statusKey) => statusKey === 'pending' || statusKey === 'inprogress' || statusKey === 'postponed';

  const getStatusKey = (taskCard) => (taskCard.dataset.taskStatusKey || '').toLowerCase();

  const isTaskOverdue = (statusKey, taskDate, dueDate, today) => {
    if (!isOpenTaskStatus(statusKey)) {
      return false;
    }

    return (!!dueDate && dueDate < today) || (!!taskDate && taskDate < today);
  };

  const captureTaskSnapshot = (taskCard, today) => {
    const statusKey = getStatusKey(taskCard);
    const taskDate = taskCard.dataset.taskDate || '';
    const dueDate = taskCard.dataset.taskDueDate || '';
    const isOpen = isOpenTaskStatus(statusKey);

    return {
      statusKey,
      taskDate,
      dueDate,
      isOpen,
      isCompleted: statusKey === 'completed',
      isToday: isOpen && !!taskDate && taskDate === today,
      isOverdue: isTaskOverdue(statusKey, taskDate, dueDate, today)
    };
  };

  const readCounter = (board, key) => {
    const element = board.querySelector(`[data-task-counter="${key}"]`);
    if (!element) {
      return null;
    }

    const parsed = Number.parseInt(element.textContent || '0', 10);
    return Number.isNaN(parsed) ? 0 : parsed;
  };

  const writeCounter = (board, key, value) => {
    const element = board.querySelector(`[data-task-counter="${key}"]`);
    if (!element) {
      return;
    }

    const safeValue = Math.max(0, value);
    element.textContent = safeValue.toString();
    if (key === 'overdue') {
      updateOverdueCounterTone(element, safeValue);
    }
  };

  const updateTaskSummaryCounters = (board, before, after) => {
    if (!board) {
      return;
    }

    const todayCount = readCounter(board, 'today');
    const pendingCount = readCounter(board, 'pending');
    const overdueCount = readCounter(board, 'overdue');
    const completedCount = readCounter(board, 'completed');

    if (todayCount === null || pendingCount === null || overdueCount === null || completedCount === null) {
      return;
    }

    let nextToday = todayCount;
    let nextPending = pendingCount;
    let nextOverdue = overdueCount;
    let nextCompleted = completedCount;

    if (before.isOpen && !after.isOpen) {
      nextPending -= 1;
    }
    if (!before.isOpen && after.isOpen) {
      nextPending += 1;
    }

    if (before.isToday && !after.isToday) {
      nextToday -= 1;
    }
    if (!before.isToday && after.isToday) {
      nextToday += 1;
    }

    if (before.isOverdue && !after.isOverdue) {
      nextOverdue -= 1;
    }
    if (!before.isOverdue && after.isOverdue) {
      nextOverdue += 1;
    }

    if (before.isCompleted && !after.isCompleted) {
      nextCompleted -= 1;
    }
    if (!before.isCompleted && after.isCompleted) {
      nextCompleted += 1;
    }

    writeCounter(board, 'today', nextToday);
    writeCounter(board, 'pending', nextPending);
    writeCounter(board, 'overdue', nextOverdue);
    writeCounter(board, 'completed', nextCompleted);
  };

  const applyPostponeDateVisual = (taskCard, today) => {
    const currentTaskDate = taskCard.dataset.taskDate || '';
    const baseDate = currentTaskDate || today;
    const nextTaskDate = addDays(baseDate, 1);

    taskCard.dataset.taskDate = nextTaskDate;

    const dateLabel = taskCard.querySelector('[data-task-date-label]');
    if (dateLabel) {
      dateLabel.textContent = formatIsoDate(nextTaskDate) || 'Sem data';
    }

    const currentDueDate = taskCard.dataset.taskDueDate || '';
    let nextDueDate = currentDueDate;
    if (nextDueDate && nextDueDate < nextTaskDate) {
      nextDueDate = nextTaskDate;
      taskCard.dataset.taskDueDate = nextDueDate;
    }

    const dueDateLabel = taskCard.querySelector('[data-task-due-date-label]');
    if (dueDateLabel) {
      dueDateLabel.textContent = nextDueDate ? formatIsoDate(nextDueDate) : 'Sem prazo';
    }
  };

  const moveTaskToResolvedToday = (taskCard) => {
    const board = taskCard.closest('[data-task-board]') || document.querySelector('[data-task-board]');
    if (!board) {
      return;
    }

    const taskDate = taskCard.dataset.taskDate;
    const today = board.dataset.today;
    if (!taskDate || !today || taskDate !== today) {
      return;
    }

    const resolvedList = board.querySelector('[data-task-resolved-today-list]');
    if (!resolvedList) {
      return;
    }

    resolvedList.append(taskCard);
    toggleTaskEmptyStates(board);
  };

  const markTaskResolvedVisual = (taskCard) => {
    const actions = taskCard.querySelector('[data-task-actions]');
    if (!actions) {
      return;
    }

    actions.querySelectorAll('form[data-task-status-form], form[action*="/Tasks/Complete"], form[action*="/Tasks/Cancel"], form[action*="/Tasks/Postpone"]').forEach((statusForm) => {
      statusForm.classList.add('hidden');
    });

    if (!actions.querySelector('[data-task-resolved-label]')) {
      const label = document.createElement('span');
      label.className = 'status-pill status-pill-success';
      label.dataset.taskResolvedLabel = 'true';
      label.textContent = 'Resolvida hoje';
      actions.append(label);
    }
  };

  const applyTaskUpdate = (form, board) => {
    const actionKind = getActionKind(form);
    if (!actionKind) {
      return;
    }

    const taskCard = form.closest('[data-task-card]');
    if (!taskCard) {
      return;
    }

    const today = board?.dataset.today || '';
    const beforeSnapshot = today ? captureTaskSnapshot(taskCard, today) : null;
    const statusPill = taskCard.querySelector('[data-task-status-pill]');
    const mappedStatus = taskStatusMap[actionKind];

    if (statusPill && mappedStatus) {
      statusPill.classList.remove(...taskStatusClasses);
      statusPill.classList.add(mappedStatus.className);
      statusPill.textContent = mappedStatus.label;
    }

    if (mappedStatus) {
      taskCard.dataset.taskStatus = mappedStatus.label;
      taskCard.dataset.taskStatusKey = mappedStatus.key;
    }

    if (actionKind === 'postpone' && today) {
      applyPostponeDateVisual(taskCard, today);
    }

    if (actionKind === 'complete' || actionKind === 'cancel' || actionKind === 'postpone') {
      markTaskResolvedVisual(taskCard);
      moveTaskToResolvedToday(taskCard);
    }

    if (board && beforeSnapshot && today) {
      const afterSnapshot = captureTaskSnapshot(taskCard, today);
      updateTaskSummaryCounters(board, beforeSnapshot, afterSnapshot);
    }
  };

  const findHabitStatusPill = (habitCard) => {
    if (!habitCard) {
      return null;
    }

    const dataPill = habitCard.querySelector('[data-habit-today-pill]');
    if (dataPill) {
      return dataPill;
    }

    return [...habitCard.querySelectorAll('.status-pill')].find((pill) => {
      const content = (pill.textContent || '').trim().toLowerCase();
      return content.startsWith('status:') || content.startsWith('hoje:');
    }) ?? null;
  };

  const findHabitActionsContainer = (form, habitCard) => {
    return habitCard?.querySelector('[data-habit-actions]')
      || form.closest('[data-habit-actions]')
      || form.parentElement;
  };

  const applyHabitUpdate = (form) => {
    const actionKind = getActionKind(form);
    if (!actionKind) {
      return;
    }

    const habitCard = form.closest('[data-habit-card]') || form.closest('article');
    if (!habitCard) {
      return;
    }

    const mappedStatus = habitStatusMap[actionKind];
    const todayPill = findHabitStatusPill(habitCard);

    if (todayPill && mappedStatus) {
      todayPill.classList.remove(...habitStatusClasses);
      todayPill.classList.add(mappedStatus.className);

      const rawText = (todayPill.textContent || '').trim().toLowerCase();
      const prefix = rawText.startsWith('status:') ? 'Status:' : 'Hoje:';
      todayPill.innerHTML = `<i data-lucide="${mappedStatus.icon}"></i>${prefix} ${mappedStatus.label}`;
    }

    const actions = findHabitActionsContainer(form, habitCard);
    if (actions) {
      actions.querySelectorAll('form[data-habit-status-form], form[action*="/Habits/Complete"], form[action*="/Habits/Fail"], form[action*="/Habits/Skip"]').forEach((statusForm) => {
        statusForm.classList.add('hidden');
      });

      if (!actions.querySelector('[data-habit-registered-label]')) {
        const label = document.createElement('span');
        label.className = 'status-pill status-pill-neutral';
        label.dataset.habitRegisteredLabel = 'true';
        label.textContent = 'Registrado hoje';
        actions.append(label);
      }
    }

    refreshIcons();
  };

  async function handleSubmit(event) {
    const form = event.currentTarget;
    const board = form.closest('[data-task-board]') || document.querySelector('[data-task-board]');

    if (form.dataset.confirm && event.defaultPrevented) {
      return;
    }

    if (form.dataset.submitting === 'true') {
      event.preventDefault();
      return;
    }

    event.preventDefault();
    setSubmitting(form, true);

    try {
      await postForm(form);

      if (form.closest('[data-task-card]')) {
        applyTaskUpdate(form, board);
      }

      if (form.action.includes('/Habits/')) {
        applyHabitUpdate(form);
      }

      const actionKind = getActionKind(form);
      notify(form.dataset.successMessage || defaultMessages[actionKind] || 'Atualizado.');
      refreshIcons();
    } catch {
      notify('Não foi possível atualizar agora. Tente novamente.', 'error');
    } finally {
      setSubmitting(form, false);
      if (board) {
        toggleTaskEmptyStates(board);
      }
    }
  }

  asyncForms.forEach((form) => {
    form.addEventListener('submit', handleSubmit);
  });
}
