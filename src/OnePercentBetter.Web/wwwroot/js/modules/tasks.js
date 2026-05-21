const form = document.querySelector('[data-task-form]');

if (form) {
  const titleInput = form.querySelector('[data-task-title-input]');
  const descriptionInput = form.querySelector('[data-task-description-input]');
  const colorInput = form.querySelector('[data-task-color-input]');
  const iconInput = form.querySelector('[data-task-icon-input]');
  const iconLabel = form.querySelector('[data-task-icon-label]');
  const iconPreview = form.querySelector('[data-task-icon-preview]');
  const iconOptions = [...form.querySelectorAll('[data-task-icon-option]')];
  const iconOpen = form.querySelector('[data-task-icon-open]');
  const iconModal = form.querySelector('[data-task-icon-modal]');
  const iconCloseButtons = [...form.querySelectorAll('[data-task-icon-close]')];
  const previewTitle = form.querySelector('[data-task-preview-title]');
  const previewIcon = form.querySelector('[data-task-preview-icon]');
  const warning = form.querySelector('[data-task-recurrence-warning]');
  const previewIconWrap = form.querySelector('[data-task-preview-icon]');
  const colorSwatches = [...form.querySelectorAll('[data-task-color-swatch]')];
  const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;

  const tagsOpen = form.querySelector('[data-task-tags-open]');
  const tagsModal = form.querySelector('[data-task-tags-modal]');
  const tagsCloseButtons = [...form.querySelectorAll('[data-task-tags-close]')];
  const tagList = form.querySelector('[data-task-tag-manage-list]');
  const tagChipList = form.querySelector('[data-task-tag-chip-list]');
  const tagEmpty = form.querySelector('[data-task-tag-empty]');
  const tagManageEmpty = form.querySelector('[data-task-tag-manage-empty]');
  const tagNameInput = form.querySelector('[data-task-tag-name]');
  const tagColorInput = form.querySelector('[data-task-tag-color]');
  const tagIdInput = form.querySelector('[data-task-tag-edit-id]');
  const tagSaveButton = form.querySelector('[data-task-tag-save]');
  const tagClearButton = form.querySelector('[data-task-tag-clear]');
  const tagError = form.querySelector('[data-task-tag-error]');
  const tagSaveUrl = form.dataset.taskTagSaveUrl;
  const tagDeleteUrl = form.dataset.taskTagDeleteUrl;

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

  const refreshIcons = () => {
    if (window.lucide) {
      window.lucide.createIcons();
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

  const setModalVisible = (modal, visible) => {
    if (!modal) {
      return;
    }

    modal.classList.toggle('hidden', !visible);
    modal.classList.toggle('flex', visible);
  };

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
      option.classList.toggle('border-emerald-300/70', isSelected);
      option.classList.toggle('bg-emerald-300/10', isSelected);
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

  const escapeHtml = (value) => String(value ?? '')
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;');

  const setTagError = (message = '') => {
    if (!tagError) {
      return;
    }

    tagError.textContent = message;
  };

  const resetTagEditor = () => {
    if (tagIdInput) {
      tagIdInput.value = '';
    }

    if (tagNameInput) {
      tagNameInput.value = '';
    }

    if (tagColorInput) {
      tagColorInput.value = '#a78bfa';
    }

    setTagError('');
  };

  const ensureTagVisibility = () => {
    const hasTags = !!tagList?.querySelector('[data-task-tag-row]');

    if (tagChipList) {
      tagChipList.classList.toggle('hidden', !hasTags);
    }

    if (tagEmpty) {
      tagEmpty.classList.toggle('hidden', hasTags);
    }

    if (tagManageEmpty) {
      tagManageEmpty.classList.toggle('hidden', hasTags);
    }
  };

  const buildTagChipRow = (tag, checked = true) => {
    const safeId = Number.parseInt(tag.id, 10);
    const inputId = `task-tag-${safeId}`;

    return `
      <label for="${inputId}"
             class="group cursor-pointer"
             data-task-tag-chip-row="${safeId}">
        <input id="${inputId}"
               type="checkbox"
               name="SelectedTagIds"
               value="${safeId}"
               ${checked ? 'checked' : ''}
               class="peer sr-only"
               data-task-tag-checkbox="${safeId}" />
        <span class="flex min-h-24 flex-col justify-center rounded-2xl border border-white/10 bg-white/[0.03] p-4 transition hover:border-emerald-300/60 hover:bg-emerald-300/10 peer-checked:border-emerald-300 peer-checked:bg-emerald-300/15 peer-checked:shadow-[0_0_0_1px_rgba(110,231,183,0.35)]">
          <span class="inline-flex items-center gap-2 text-sm font-semibold text-slate-300 transition peer-checked:text-white">
            <span class="h-2.5 w-2.5 rounded-full" style="background:${escapeHtml(tag.color)}" data-task-tag-chip-dot></span>
            <span data-task-tag-chip-name>${escapeHtml(tag.name)}</span>
          </span>
          <span class="mt-2 text-xs text-slate-500 transition group-hover:text-slate-300">Clique para selecionar.</span>
        </span>
      </label>
    `;
  };

  const buildManageTagRow = (tag) => {
    const safeId = Number.parseInt(tag.id, 10);
    return `
      <article class="flex items-center justify-between gap-3 rounded-xl border border-white/10 bg-white/[0.03] px-3 py-2"
               data-task-tag-row="${safeId}"
               data-task-tag-name="${escapeHtml(tag.name)}"
               data-task-tag-color="${escapeHtml(tag.color)}">
        <div class="min-w-0">
          <div class="inline-flex items-center gap-2 text-sm font-semibold text-slate-200">
            <span class="h-2.5 w-2.5 rounded-full" style="background:${escapeHtml(tag.color)}" data-task-tag-row-dot></span>
            <span class="truncate" data-task-tag-row-name>${escapeHtml(tag.name)}</span>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <button type="button" class="btn-secondary min-h-0 px-3 py-1 text-xs" data-task-tag-edit="${safeId}">
            <i data-lucide="pencil" class="h-3.5 w-3.5"></i>
            Editar
          </button>
          <button type="button" class="btn-danger min-h-0 px-3 py-1 text-xs" data-task-tag-delete="${safeId}">
            <i data-lucide="trash-2" class="h-3.5 w-3.5"></i>
            Excluir
          </button>
        </div>
      </article>
    `;
  };

  const upsertTagChip = (tag) => {
    if (!tagChipList) {
      return;
    }

    const safeId = Number.parseInt(tag.id, 10);
    const existing = tagChipList.querySelector(`[data-task-tag-chip-row="${safeId}"]`);
    if (!existing) {
      tagChipList.insertAdjacentHTML('beforeend', buildTagChipRow(tag, true));
      ensureTagVisibility();
      refreshIcons();
      return;
    }

    const nameElement = existing.querySelector('[data-task-tag-chip-name]');
    const dotElement = existing.querySelector('[data-task-tag-chip-dot]');
    const checkbox = existing.querySelector(`[data-task-tag-checkbox="${safeId}"]`);

    if (nameElement) {
      nameElement.textContent = tag.name;
    }

    if (dotElement) {
      dotElement.style.background = tag.color;
    }

    if (checkbox) {
      checkbox.checked = true;
    }
  };

  const upsertTagManageRow = (tag) => {
    if (!tagList) {
      return;
    }

    const safeId = Number.parseInt(tag.id, 10);
    const existing = tagList.querySelector(`[data-task-tag-row="${safeId}"]`);
    if (!existing) {
      tagList.insertAdjacentHTML('beforeend', buildManageTagRow(tag));
      refreshIcons();
      return;
    }

    existing.setAttribute('data-task-tag-name', tag.name);
    existing.setAttribute('data-task-tag-color', tag.color);

    const nameElement = existing.querySelector('[data-task-tag-row-name]');
    const dotElement = existing.querySelector('[data-task-tag-row-dot]');

    if (nameElement) {
      nameElement.textContent = tag.name;
    }

    if (dotElement) {
      dotElement.style.background = tag.color;
    }
  };

  const removeTagFromDom = (tagId) => {
    const safeId = Number.parseInt(tagId, 10);
    tagList?.querySelector(`[data-task-tag-row="${safeId}"]`)?.remove();
    tagChipList?.querySelector(`[data-task-tag-chip-row="${safeId}"]`)?.remove();
    ensureTagVisibility();
  };

  const saveTag = async () => {
    if (!tagSaveUrl || !tagNameInput || !tagColorInput) {
      return;
    }

    const name = tagNameInput.value.trim();
    if (!name) {
      setTagError('Informe o nome da tag.');
      return;
    }

    setTagError('');
    tagSaveButton?.setAttribute('disabled', 'disabled');

    const payload = new FormData();
    if (token) {
      payload.append('__RequestVerificationToken', token);
    }

    if (tagIdInput?.value) {
      payload.append('Id', tagIdInput.value);
    }

    payload.append('Name', name);
    payload.append('Color', tagColorInput.value || '#a78bfa');

    try {
      const response = await fetch(tagSaveUrl, {
        method: 'POST',
        body: payload,
        headers: {
          'X-Requested-With': 'XMLHttpRequest'
        }
      });

      const result = await response.json().catch(() => ({}));
      if (!response.ok) {
        setTagError(result.error || 'Não foi possível salvar a tag.');
        return;
      }

      const tag = {
        id: result.id,
        name: result.name,
        color: result.color || '#a78bfa'
      };

      upsertTagManageRow(tag);
      upsertTagChip(tag);
      ensureTagVisibility();
      resetTagEditor();
      showMessage('Tag salva.');
    } catch {
      setTagError('Não foi possível salvar a tag agora.');
    } finally {
      tagSaveButton?.removeAttribute('disabled');
    }
  };

  const deleteTag = async (tagId) => {
    if (!tagDeleteUrl || !tagId) {
      return;
    }

    const confirmed = window.confirm('Excluir esta tag? Ela será removida das tarefas vinculadas.');
    if (!confirmed) {
      return;
    }

    const payload = new FormData();
    if (token) {
      payload.append('__RequestVerificationToken', token);
    }
    payload.append('id', String(tagId));

    try {
      const response = await fetch(tagDeleteUrl, {
        method: 'POST',
        body: payload,
        headers: {
          'X-Requested-With': 'XMLHttpRequest'
        }
      });

      const result = await response.json().catch(() => ({}));
      if (!response.ok) {
        setTagError(result.error || 'Não foi possível excluir a tag.');
        return;
      }

      removeTagFromDom(tagId);
      setTagError('');
      showMessage('Tag excluída.');
    } catch {
      setTagError('Não foi possível excluir a tag agora.');
    }
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

  iconOpen?.addEventListener('click', () => setModalVisible(iconModal, true));
  iconCloseButtons.forEach((button) => {
    button.addEventListener('click', () => setModalVisible(iconModal, false));
  });
  iconModal?.addEventListener('click', (event) => {
    if (event.target === iconModal) {
      setModalVisible(iconModal, false);
    }
  });
  iconOptions.forEach((option) => {
    option.addEventListener('click', () => {
      syncIcon(option.dataset.taskIconOption, option.dataset.taskIconName);
      setModalVisible(iconModal, false);
    });
  });

  tagsOpen?.addEventListener('click', () => {
    resetTagEditor();
    setModalVisible(tagsModal, true);
    tagNameInput?.focus();
  });
  tagsCloseButtons.forEach((button) => {
    button.addEventListener('click', () => setModalVisible(tagsModal, false));
  });
  tagsModal?.addEventListener('click', (event) => {
    if (event.target === tagsModal) {
      setModalVisible(tagsModal, false);
    }
  });

  tagClearButton?.addEventListener('click', resetTagEditor);
  tagSaveButton?.addEventListener('click', saveTag);
  tagNameInput?.addEventListener('keydown', (event) => {
    if (event.key === 'Enter') {
      event.preventDefault();
      saveTag();
    }
  });

  tagList?.addEventListener('click', (event) => {
    const editButton = event.target.closest('[data-task-tag-edit]');
    if (editButton) {
      const tagId = editButton.dataset.taskTagEdit;
      const row = tagList.querySelector(`[data-task-tag-row="${tagId}"]`);
      if (!row) {
        return;
      }

      if (tagIdInput) {
        tagIdInput.value = tagId || '';
      }

      if (tagNameInput) {
        tagNameInput.value = row.getAttribute('data-task-tag-name') || '';
        tagNameInput.focus();
      }

      if (tagColorInput) {
        tagColorInput.value = row.getAttribute('data-task-tag-color') || '#a78bfa';
      }

      setTagError('');
      return;
    }

    const deleteButton = event.target.closest('[data-task-tag-delete]');
    if (deleteButton) {
      const tagId = deleteButton.dataset.taskTagDelete;
      deleteTag(tagId);
    }
  });

  [titleInput, descriptionInput, colorInput].forEach((element) => {
    element?.addEventListener('input', syncPreview);
    element?.addEventListener('change', syncPreview);
  });

  document.addEventListener('keydown', (event) => {
    if (event.key !== 'Escape') {
      return;
    }

    setModalVisible(iconModal, false);
    setModalVisible(tagsModal, false);
  });

  syncIcon(iconInput?.value);
  syncPreview();
  ensureTagVisibility();
}
