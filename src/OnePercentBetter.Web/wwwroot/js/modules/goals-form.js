const form = document.querySelector('[data-goal-form]');

if (form) {
  const titleInput = form.querySelector('[data-goal-title-input]');
  const descriptionInput = form.querySelector('[data-goal-description-input]');
  const colorInput = form.querySelector('[data-goal-color-input]');
  const colorSwatches = [...form.querySelectorAll('[data-goal-color-swatch]')];
  const iconInput = form.querySelector('[data-goal-icon-input]');
  const iconOptions = [...form.querySelectorAll('[data-goal-icon-option]')];
  const iconLabel = form.querySelector('[data-goal-icon-label]');
  const iconPreview = form.querySelector('[data-goal-icon-preview]');
  const previewIcon = form.querySelector('[data-goal-preview-icon]');
  const previewTitle = form.querySelector('[data-goal-preview-title]');
  const previewDescription = form.querySelector('[data-goal-preview-description]');

  const defaultColor = '#38bdf8';
  const defaultIcon = 'target';
  const iconPattern = /^[a-z0-9-]+$/i;
  const colorPattern = /^#[0-9a-f]{6}$/i;

  const refreshIcons = () => {
    if (window.lucide) {
      window.lucide.createIcons();
    }
  };

  const safeIcon = (value) => {
    const candidate = (value || '').trim();
    return iconPattern.test(candidate) ? candidate : defaultIcon;
  };

  const safeColor = (value) => {
    const candidate = (value || '').trim();
    return colorPattern.test(candidate) ? candidate : defaultColor;
  };

  const renderIcon = (target, icon, sizeClass = 'h-4 w-4') => {
    if (!target) {
      return;
    }

    target.innerHTML = `<i data-lucide="${safeIcon(icon)}" class="${sizeClass}"></i>`;
    refreshIcons();
  };

  const syncPreview = () => {
    const title = titleInput?.value.trim() || 'Seu novo objetivo';
    const description = descriptionInput?.value.trim() || descriptionInput?.placeholder || 'Descreva o resultado esperado e por que ele importa.';

    if (previewTitle) {
      previewTitle.textContent = title;
    }

    if (previewDescription) {
      previewDescription.textContent = description;
    }
  };

  const syncColor = (color) => {
    const selectedColor = safeColor(color || colorInput?.value);

    if (colorInput) {
      colorInput.value = selectedColor;
    }

    if (previewIcon) {
      previewIcon.style.color = selectedColor;
    }

    colorSwatches.forEach((swatch) => {
      const isSelected = swatch.dataset.goalColorSwatch?.toLowerCase() === selectedColor.toLowerCase();
      swatch.classList.toggle('ring-2', isSelected);
      swatch.classList.toggle('ring-emerald-300', isSelected);
      swatch.classList.toggle('ring-offset-2', isSelected);
      swatch.classList.toggle('ring-offset-slate-950', isSelected);
    });
  };

  const syncIcon = (icon, name) => {
    const selectedIcon = safeIcon(icon || iconInput?.value);
    const selectedName = name || iconOptions.find((option) => option.dataset.goalIconOption === selectedIcon)?.dataset.goalIconName || selectedIcon;

    if (iconInput) {
      iconInput.value = selectedIcon;
    }

    if (iconLabel) {
      iconLabel.textContent = selectedName;
    }

    renderIcon(iconPreview, selectedIcon, 'h-4 w-4');
    renderIcon(previewIcon, selectedIcon, 'h-7 w-7');

    iconOptions.forEach((option) => {
      const isSelected = option.dataset.goalIconOption === selectedIcon;
      option.classList.toggle('border-sky-300/70', isSelected);
      option.classList.toggle('bg-sky-300/10', isSelected);
    });
  };

  [titleInput, descriptionInput].forEach((field) => {
    field?.addEventListener('input', syncPreview);
  });

  colorInput?.addEventListener('input', () => syncColor(colorInput.value));

  colorSwatches.forEach((swatch) => {
    swatch.addEventListener('click', () => syncColor(swatch.dataset.goalColorSwatch));
  });

  iconOptions.forEach((option) => {
    option.addEventListener('click', () => {
      syncIcon(option.dataset.goalIconOption, option.dataset.goalIconName);
    });
  });

  syncPreview();
  syncColor(colorInput?.value);
  syncIcon(iconInput?.value);
}
