const form = document.querySelector('[data-habit-form]');

if (form) {
  const frequencySelect = form.querySelector('[data-habit-frequency-select]');
  const daysPanel = form.querySelector('[data-habit-frequency-days]');
  const titleInput = form.querySelector('[data-habit-title-input]');
  const triggerInput = form.querySelector('[data-habit-trigger-input]');
  const twoMinuteInput = form.querySelector('[data-habit-two-minute-input]');
  const colorInput = form.querySelector('[data-habit-color-input]');
  const colorSwatches = [...form.querySelectorAll('[data-habit-color-swatch]')];
  const iconInput = form.querySelector('[data-habit-icon-input]');
  const iconOpen = form.querySelector('[data-habit-icon-open]');
  const iconModal = form.querySelector('[data-habit-icon-modal]');
  const iconCloseButtons = [...form.querySelectorAll('[data-habit-icon-close]')];
  const iconOptions = [...form.querySelectorAll('[data-habit-icon-option]')];
  const iconLabel = form.querySelector('[data-habit-icon-label]');
  const iconPreview = form.querySelector('[data-habit-icon-preview]');
  const habitPreviewIcon = form.querySelector('[data-habit-preview-icon]');
  const previewTitle = form.querySelector('[data-habit-preview-title]');
  const previewTrigger = form.querySelector('[data-habit-preview-trigger]');
  const previewTwoMinute = form.querySelector('[data-habit-preview-two-minute]');
  const previewStack = form.querySelector('[data-habit-preview-stack]');
  const stackSelect = form.querySelector('[data-habit-stack-select]');
  const stackPreview = form.querySelector('[data-habit-stack-preview]');
  const locationSelect = form.querySelector('[data-habit-location-select]');
  const locationOpen = form.querySelector('[data-habit-location-open]');
  const locationModal = form.querySelector('[data-habit-location-modal]');
  const locationCloseButtons = [...form.querySelectorAll('[data-habit-location-close]')];
  const locationInput = form.querySelector('[data-habit-location-name]');
  const locationSave = form.querySelector('[data-habit-location-save]');
  const locationError = form.querySelector('[data-habit-location-error]');
  const locationSuggestions = [...form.querySelectorAll('[data-habit-location-suggestion]')];
  const simpleOpen = form.querySelector('[data-habit-simple-open]');
  const simpleModal = form.querySelector('[data-habit-simple-modal]');
  const simpleCloseButtons = [...form.querySelectorAll('[data-habit-simple-close]')];
  const simpleNameInput = form.querySelector('[data-habit-simple-name]');
  const simpleTimeInput = form.querySelector('[data-habit-simple-time]');
  const simpleIdInput = form.querySelector('[data-habit-simple-id]');
  const simpleExistingSelect = form.querySelector('[data-habit-simple-existing]');
  const simpleLoad = form.querySelector('[data-habit-simple-load]');
  const simpleSave = form.querySelector('[data-habit-simple-save]');
  const simpleSaveLabel = form.querySelector('[data-habit-simple-save-label]');
  const simpleError = form.querySelector('[data-habit-simple-error]');
  const simpleSuggestions = [...form.querySelectorAll('[data-habit-simple-suggestion]')];
  const simpleOptGroup = form.querySelector('[data-habit-simple-optgroup]');

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

  const stripSelectLabel = (value) => (value ?? '').replace(/\s+-\s+.*$/, '').trim();

  const syncDaysVisibility = () => {
    if (!frequencySelect || !daysPanel) {
      return;
    }

    const isSpecificDays = frequencySelect.value === '2';
    daysPanel.classList.toggle('hidden', !isSpecificDays);

    if (!isSpecificDays) {
      daysPanel.querySelectorAll('input[type="checkbox"]').forEach((checkbox) => {
        checkbox.checked = false;
      });
    }
  };

  const syncPreview = () => {
    const title = titleInput?.value.trim() || 'Seu novo hábito';
    const trigger = triggerInput?.value.trim() || triggerInput?.placeholder || '';
    const twoMinute = twoMinuteInput?.value.trim() || twoMinuteInput?.placeholder || '';
    const selectedStack = stackSelect?.selectedOptions?.[0];
    const baseHabit = selectedStack?.value ? stripSelectLabel(selectedStack.textContent) : '';

    if (previewTitle) {
      previewTitle.textContent = title;
    }

    if (previewTrigger) {
      previewTrigger.textContent = trigger;
    }

    if (previewTwoMinute) {
      previewTwoMinute.textContent = twoMinute;
    }

    const stackText = baseHabit
      ? `Depois de ${baseHabit}, eu irei ${title === 'Seu novo hábito' ? 'executar este hábito' : title}.`
      : '';

    if (previewStack) {
      previewStack.textContent = stackText;
    }

    if (stackPreview) {
      stackPreview.textContent = stackText || 'Escolha um hábito base para ver a frase de empilhamento.';
    }
  };

  const renderIcon = (target, icon, sizeClass = 'h-4 w-4') => {
    if (!target) {
      return;
    }

    target.innerHTML = `<i data-lucide="${icon}" class="${sizeClass}"></i>`;
    refreshIcons();
  };

  const syncColor = (color) => {
    const selectedColor = color || colorInput?.value;
    if (!selectedColor) {
      return;
    }

    if (colorInput) {
      colorInput.value = selectedColor;
    }

    if (habitPreviewIcon) {
      habitPreviewIcon.style.color = selectedColor;
    }

    colorSwatches.forEach((swatch) => {
      const isSelected = swatch.dataset.habitColorSwatch?.toLowerCase() === selectedColor.toLowerCase();
      swatch.classList.toggle('ring-2', isSelected);
      swatch.classList.toggle('ring-emerald-300', isSelected);
      swatch.classList.toggle('ring-offset-2', isSelected);
      swatch.classList.toggle('ring-offset-slate-950', isSelected);
    });
  };

  const syncIcon = (icon, name) => {
    const selectedIcon = icon || iconInput?.value || 'repeat-2';
    const selectedName = name || iconOptions.find((option) => option.dataset.habitIconOption === selectedIcon)?.dataset.habitIconName || selectedIcon;

    if (iconInput) {
      iconInput.value = selectedIcon;
    }

    if (iconLabel) {
      iconLabel.textContent = selectedName;
    }

    renderIcon(iconPreview, selectedIcon, 'h-4 w-4');
    renderIcon(habitPreviewIcon, selectedIcon, 'h-6 w-6');

    iconOptions.forEach((option) => {
      const isSelected = option.dataset.habitIconOption === selectedIcon;
      option.classList.toggle('border-emerald-300/70', isSelected);
      option.classList.toggle('bg-emerald-300/10', isSelected);
    });
  };

  const saveLocation = async () => {
    const name = locationInput?.value.trim();
    const url = form.dataset.habitLocationUrl;
    const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;

    if (!name || !url || !locationSelect) {
      if (locationError) {
        locationError.textContent = 'Informe o nome do local.';
      }
      return;
    }

    const payload = new FormData();
    payload.append('Name', name);

    if (token) {
      payload.append('__RequestVerificationToken', token);
    }

    if (locationSave) {
      locationSave.disabled = true;
      locationSave.classList.add('opacity-60');
    }

    if (locationError) {
      locationError.textContent = '';
    }

    try {
      const response = await fetch(url, {
        method: 'POST',
        body: payload,
        headers: {
          'X-Requested-With': 'XMLHttpRequest'
        }
      });

      const result = await response.json().catch(() => ({}));

      if (!response.ok) {
        const message = result.error || 'Não foi possível cadastrar o local.';
        if (locationError) {
          locationError.textContent = message;
        }
        showMessage(message, 'error');
        return;
      }

      let option = [...locationSelect.options].find((item) => item.value === result.value);
      if (!option) {
        option = new Option(result.text, result.value);
        locationSelect.add(option);
      }

      option.selected = true;
      locationSelect.value = result.value;

      if (locationInput) {
        locationInput.value = '';
      }

      setModalVisible(locationModal, false);
      showMessage('Local cadastrado.');
    } catch {
      const message = 'Não foi possível cadastrar o local agora.';
      if (locationError) {
        locationError.textContent = message;
      }
      showMessage(message, 'error');
    } finally {
      if (locationSave) {
        locationSave.disabled = false;
        locationSave.classList.remove('opacity-60');
      }
    }
  };

  const parseSimpleLabel = (label) => {
    const value = (label || '').trim();
    if (!value) {
      return { name: '', time: '' };
    }

    const match = value.match(/^(.*)\s+(?:às|as)\s+(\d{2}:\d{2})$/i);
    if (!match) {
      return { name: value, time: '' };
    }

    return {
      name: match[1].trim(),
      time: match[2]
    };
  };

  const resetSimpleForm = () => {
    if (simpleIdInput) {
      simpleIdInput.value = '';
    }

    if (simpleNameInput) {
      simpleNameInput.value = '';
    }

    if (simpleTimeInput) {
      simpleTimeInput.value = '';
    }

    if (simpleSaveLabel) {
      simpleSaveLabel.textContent = 'Salvar hábito simples';
    }

    if (simpleError) {
      simpleError.textContent = '';
    }
  };

  const loadSelectedSimpleHabit = () => {
    const selected = simpleExistingSelect?.selectedOptions?.[0];
    if (!selected || !selected.value) {
      if (simpleError) {
        simpleError.textContent = 'Selecione um hábito simples para editar.';
      }
      return;
    }

    const { name, time } = parseSimpleLabel(selected.textContent || '');

    if (simpleIdInput) {
      simpleIdInput.value = selected.value;
    }

    if (simpleNameInput) {
      simpleNameInput.value = name;
      simpleNameInput.focus();
    }

    if (simpleTimeInput) {
      simpleTimeInput.value = time;
    }

    if (simpleSaveLabel) {
      simpleSaveLabel.textContent = 'Atualizar hábito simples';
    }

    if (simpleError) {
      simpleError.textContent = '';
    }
  };

  const saveSimpleHabit = async () => {
    const name = simpleNameInput?.value.trim();
    const url = form.dataset.habitSimpleUrl;
    const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;
    const simpleId = Number.parseInt(simpleIdInput?.value || '', 10);
    const isEditing = Number.isInteger(simpleId) && simpleId > 0;

    if (!name || !url || !stackSelect || !simpleOptGroup) {
      if (simpleError) {
        simpleError.textContent = 'Informe o nome do hábito simples.';
      }
      return;
    }

    const payload = new FormData();
    if (isEditing) {
      payload.append('Id', String(simpleId));
    }
    payload.append('Name', name);

    if (simpleTimeInput?.value) {
      payload.append('ScheduledTime', simpleTimeInput.value);
    }

    if (token) {
      payload.append('__RequestVerificationToken', token);
    }

    if (simpleSave) {
      simpleSave.disabled = true;
      simpleSave.classList.add('opacity-60');
    }

    if (simpleError) {
      simpleError.textContent = '';
    }

    try {
      const response = await fetch(url, {
        method: 'POST',
        body: payload,
        headers: {
          'X-Requested-With': 'XMLHttpRequest'
        }
      });

      const result = await response.json().catch(() => ({}));

      if (!response.ok) {
        const message = result.error || 'Não foi possível salvar o hábito simples.';
        if (simpleError) {
          simpleError.textContent = message;
        }
        showMessage(message, 'error');
        return;
      }

      let option = [...stackSelect.options].find((item) => item.value === result.value);
      if (!option) {
        option = new Option(result.text, result.value);
        simpleOptGroup.append(option);
      } else {
        option.text = result.text;
      }

      option.selected = true;
      stackSelect.value = result.value;

      if (simpleExistingSelect) {
        const existingId = String(result.id ?? result.value?.replace('simple:', ''));
        let existingOption = [...simpleExistingSelect.options].find((item) => item.value === existingId);
        if (!existingOption) {
          existingOption = new Option(result.text, existingId);
          simpleExistingSelect.add(existingOption);
        } else {
          existingOption.text = result.text;
        }

        existingOption.selected = true;
        simpleExistingSelect.value = existingId;
      }

      resetSimpleForm();
      setModalVisible(simpleModal, false);
      syncPreview();
      showMessage(isEditing ? 'Hábito simples atualizado.' : 'Hábito simples cadastrado.');
    } catch {
      const message = 'Não foi possível salvar o hábito simples agora.';
      if (simpleError) {
        simpleError.textContent = message;
      }
      showMessage(message, 'error');
    } finally {
      if (simpleSave) {
        simpleSave.disabled = false;
        simpleSave.classList.remove('opacity-60');
      }
    }
  };

  frequencySelect?.addEventListener('change', syncDaysVisibility);
  [titleInput, triggerInput, twoMinuteInput, stackSelect].forEach((field) => {
    field?.addEventListener('input', syncPreview);
    field?.addEventListener('change', syncPreview);
  });

  colorInput?.addEventListener('input', () => syncColor(colorInput.value));
  colorSwatches.forEach((swatch) => {
    swatch.addEventListener('click', () => syncColor(swatch.dataset.habitColorSwatch));
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
      syncIcon(option.dataset.habitIconOption, option.dataset.habitIconName);
      setModalVisible(iconModal, false);
    });
  });

  locationOpen?.addEventListener('click', () => {
    setModalVisible(locationModal, true);
    locationInput?.focus();
  });
  locationCloseButtons.forEach((button) => {
    button.addEventListener('click', () => setModalVisible(locationModal, false));
  });
  locationModal?.addEventListener('click', (event) => {
    if (event.target === locationModal) {
      setModalVisible(locationModal, false);
    }
  });
  locationSave?.addEventListener('click', saveLocation);
  locationInput?.addEventListener('keydown', (event) => {
    if (event.key === 'Enter') {
      event.preventDefault();
      saveLocation();
    }
  });
  locationSuggestions.forEach((suggestion) => {
    suggestion.addEventListener('click', () => {
      if (locationInput) {
        locationInput.value = suggestion.dataset.habitLocationSuggestion ?? '';
        locationInput.focus();
      }
    });
  });

  simpleOpen?.addEventListener('click', () => {
    resetSimpleForm();
    setModalVisible(simpleModal, true);
    simpleNameInput?.focus();
  });
  simpleCloseButtons.forEach((button) => {
    button.addEventListener('click', () => setModalVisible(simpleModal, false));
  });
  simpleModal?.addEventListener('click', (event) => {
    if (event.target === simpleModal) {
      setModalVisible(simpleModal, false);
    }
  });
  simpleLoad?.addEventListener('click', loadSelectedSimpleHabit);
  simpleSave?.addEventListener('click', saveSimpleHabit);
  simpleExistingSelect?.addEventListener('change', () => {
    if (simpleError) {
      simpleError.textContent = '';
    }
  });
  simpleNameInput?.addEventListener('keydown', (event) => {
    if (event.key === 'Enter') {
      event.preventDefault();
      saveSimpleHabit();
    }
  });
  simpleTimeInput?.addEventListener('keydown', (event) => {
    if (event.key === 'Enter') {
      event.preventDefault();
      saveSimpleHabit();
    }
  });
  simpleSuggestions.forEach((suggestion) => {
    suggestion.addEventListener('click', () => {
      if (simpleNameInput) {
        simpleNameInput.value = suggestion.dataset.habitSimpleSuggestion ?? '';
        simpleNameInput.focus();
      }
    });
  });

  document.addEventListener('keydown', (event) => {
    if (event.key !== 'Escape') {
      return;
    }

    setModalVisible(iconModal, false);
    setModalVisible(locationModal, false);
    setModalVisible(simpleModal, false);
  });

  syncDaysVisibility();
  syncPreview();
  syncColor(colorInput?.value);
  syncIcon(iconInput?.value);
  resetSimpleForm();
}


