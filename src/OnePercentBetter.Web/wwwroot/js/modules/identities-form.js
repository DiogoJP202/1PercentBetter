const form = document.querySelector('[data-identity-form]');

if (form) {
  const nameInput = form.querySelector('[data-identity-name-input]');
  const statementInput = form.querySelector('[data-identity-statement-input]');
  const colorInput = form.querySelector('[data-identity-color-input]');
  const colorSwatches = [...form.querySelectorAll('[data-identity-color-swatch]')];
  const iconInput = form.querySelector('[data-identity-icon-input]');
  const iconOptions = [...form.querySelectorAll('[data-identity-icon-option]')];
  const iconLabel = form.querySelector('[data-identity-icon-label]');
  const iconPreview = form.querySelector('[data-identity-icon-preview]');
  const previewIcon = form.querySelector('[data-identity-preview-icon]');
  const previewName = form.querySelector('[data-identity-preview-name]');
  const previewStatement = form.querySelector('[data-identity-preview-statement]');

  const defaultColor = '#22c55e';
  const defaultIcon = 'user-round-check';
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
    const name = nameInput?.value.trim() || 'Sua nova identidade';
    const statement = statementInput?.value.trim() || statementInput?.placeholder || 'Eu sou uma pessoa que evolui um pouco todos os dias.';

    if (previewName) {
      previewName.textContent = name;
    }

    if (previewStatement) {
      previewStatement.textContent = statement;
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
      const isSelected = swatch.dataset.identityColorSwatch?.toLowerCase() === selectedColor.toLowerCase();
      swatch.classList.toggle('ring-2', isSelected);
      swatch.classList.toggle('ring-emerald-300', isSelected);
      swatch.classList.toggle('ring-offset-2', isSelected);
      swatch.classList.toggle('ring-offset-slate-950', isSelected);
    });
  };

  const syncIcon = (icon, name) => {
    const selectedIcon = safeIcon(icon || iconInput?.value);
    const selectedName = name || iconOptions.find((option) => option.dataset.identityIconOption === selectedIcon)?.dataset.identityIconName || selectedIcon;

    if (iconInput) {
      iconInput.value = selectedIcon;
    }

    if (iconLabel) {
      iconLabel.textContent = selectedName;
    }

    renderIcon(iconPreview, selectedIcon, 'h-4 w-4');
    renderIcon(previewIcon, selectedIcon, 'h-7 w-7');

    iconOptions.forEach((option) => {
      const isSelected = option.dataset.identityIconOption === selectedIcon;
      option.classList.toggle('border-emerald-300/70', isSelected);
      option.classList.toggle('bg-emerald-300/10', isSelected);
    });
  };

  [nameInput, statementInput].forEach((field) => {
    field?.addEventListener('input', syncPreview);
  });

  colorInput?.addEventListener('input', () => syncColor(colorInput.value));

  colorSwatches.forEach((swatch) => {
    swatch.addEventListener('click', () => syncColor(swatch.dataset.identityColorSwatch));
  });

  iconOptions.forEach((option) => {
    option.addEventListener('click', () => {
      syncIcon(option.dataset.identityIconOption, option.dataset.identityIconName);
    });
  });

  syncPreview();
  syncColor(colorInput?.value);
  syncIcon(iconInput?.value);
}
