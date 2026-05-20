const form = document.querySelector('[data-task-form]');

if (form) {
  const titleInput = form.querySelector('[data-task-title-input]');
  const descriptionInput = form.querySelector('[data-task-description-input]');
  const colorInput = form.querySelector('[data-task-color-input]');
  const iconInput = form.querySelector('[data-task-icon-input]');
  const iconLabel = form.querySelector('[data-task-icon-label]');
  const iconPreview = form.querySelector('[data-task-icon-preview]');
  const iconOptions = [...form.querySelectorAll('[data-task-icon-option]')];
  const previewTitle = form.querySelector('[data-task-preview-title]');
  const previewIcon = form.querySelector('[data-task-preview-icon]');
  const warning = form.querySelector('[data-task-recurrence-warning]');
  const previewIconWrap = form.querySelector('[data-task-preview-icon]');
  const colorSwatches = [...form.querySelectorAll('[data-task-color-swatch]')];

  const refreshIcons = () => {
    if (window.lucide) {
      window.lucide.createIcons();
    }
  };

  const recurrenceHints = [
    'todo dia',
    'todos os dias',
    'diario',
    'diaria',
    'semanal',
    'segunda',
    'terca',
    'quarta',
    'quinta',
    'sexta',
    'sabado',
    'domingo',
    'cada dia'
  ];

  const normalize = (value) => (value || '')
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');

  const looksRecurring = () => {
    const haystack = `${normalize(titleInput?.value)} ${normalize(descriptionInput?.value)}`;
    return recurrenceHints.some((term) => haystack.includes(term));
  };

  const syncIcon = (icon, name) => {
    const selectedIcon = icon || iconInput?.value || 'list-checks';
    const selectedName = name
      || iconOptions.find((option) => option.dataset.taskIconOption === selectedIcon)?.dataset.taskIconName
      || selectedIcon;

    if (iconInput) {
      iconInput.value = selectedIcon;
    }

    if (iconLabel) {
      iconLabel.textContent = selectedName;
    }

    if (iconPreview) {
      iconPreview.innerHTML = `<i data-lucide="${selectedIcon}" class="h-4 w-4"></i>`;
    }

    if (previewIcon) {
      previewIcon.innerHTML = `<i data-lucide="${selectedIcon}" class="h-6 w-6"></i>`;
    }

    iconOptions.forEach((option) => {
      const isSelected = option.dataset.taskIconOption === selectedIcon;
      option.classList.toggle('border-violet-300/80', isSelected);
      option.classList.toggle('bg-violet-300/10', isSelected);
    });

    refreshIcons();
  };

  const syncPreview = () => {
    if (previewTitle && titleInput) {
      const title = titleInput.value.trim();
      previewTitle.textContent = title || 'Sua próxima ação';
    }

    if (previewIconWrap && colorInput) {
      previewIconWrap.style.color = colorInput.value || '#a78bfa';
    }

    if (warning) {
      warning.classList.toggle('hidden', !looksRecurring());
    }

    refreshIcons();
  };

  colorSwatches.forEach((swatch) => {
    swatch.addEventListener('click', () => {
      if (!colorInput) {
        return;
      }

      colorInput.value = swatch.dataset.taskColorSwatch || colorInput.value;
      syncPreview();
    });
  });

  iconOptions.forEach((option) => {
    option.addEventListener('click', () => {
      syncIcon(option.dataset.taskIconOption, option.dataset.taskIconName);
    });
  });

  [titleInput, descriptionInput, colorInput].forEach((element) => {
    element?.addEventListener('input', syncPreview);
    element?.addEventListener('change', syncPreview);
  });

  syncIcon(iconInput?.value);
  syncPreview();
}
