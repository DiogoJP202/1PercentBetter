const form = document.querySelector('[data-note-form]');

if (form) {
  const titleInput = form.querySelector('[data-note-title-input]');
  const contentInput = form.querySelector('[data-note-content-input]');
  const dateInput = form.querySelector('[data-note-date-input]');
  const typeInput = form.querySelector('[data-note-type-input]');
  const tagsInput = form.querySelector('[data-note-tags-input]');

  const previewTitle = form.querySelector('[data-note-preview-title]');
  const previewContent = form.querySelector('[data-note-preview-content]');
  const previewDate = form.querySelector('[data-note-preview-date]');
  const previewType = form.querySelector('[data-note-preview-type]');
  const previewTags = form.querySelector('[data-note-preview-tags]');

  const titleCounter = form.querySelector('[data-note-title-counter]');
  const contentCounter = form.querySelector('[data-note-content-counter]');

  const maxTitle = Number(titleInput?.getAttribute('maxlength') || 160);
  const maxContent = Number(contentInput?.getAttribute('maxlength') || 6000);

  const normalizeText = (value) => (value || '').trim();

  const formatDate = (value) => {
    const dateValue = normalizeText(value);
    if (!dateValue) {
      return 'Sem data';
    }

    const parsedDate = new Date(`${dateValue}T12:00:00`);
    if (Number.isNaN(parsedDate.getTime())) {
      return dateValue;
    }

    return parsedDate.toLocaleDateString('pt-BR');
  };

  const parseTags = (value) => {
    if (!value) {
      return [];
    }

    return value
      .split(',')
      .map((tag) => tag.trim())
      .filter((tag) => tag.length > 0)
      .filter((tag, index, tags) => tags.findIndex((candidate) => candidate.toLowerCase() === tag.toLowerCase()) === index)
      .slice(0, 8);
  };

  const syncTitle = () => {
    const rawTitle = titleInput?.value || '';
    const title = normalizeText(rawTitle);

    if (previewTitle) {
      previewTitle.textContent = title || 'Sua nova anotacao';
    }

    if (titleCounter) {
      titleCounter.textContent = `${rawTitle.length}/${maxTitle}`;
    }
  };

  const syncContent = () => {
    const rawContent = contentInput?.value || '';
    const content = normalizeText(rawContent);

    if (previewContent) {
      previewContent.textContent = content || 'Descreva fatos, sinais e proximos passos para transformar observacoes em melhoria continua.';
    }

    if (contentCounter) {
      contentCounter.textContent = `${rawContent.length}/${maxContent}`;
    }
  };

  const syncType = () => {
    if (!previewType || !typeInput) {
      return;
    }

    const selectedText = typeInput.options[typeInput.selectedIndex]?.text || 'Sem tipo';
    previewType.textContent = selectedText;
  };

  const syncDate = () => {
    if (!previewDate) {
      return;
    }

    previewDate.textContent = formatDate(dateInput?.value || '');
  };

  const syncTags = () => {
    if (!previewTags) {
      return;
    }

    const tags = parseTags(tagsInput?.value || '');
    previewTags.innerHTML = '';

    if (!tags.length) {
      const emptyLabel = document.createElement('span');
      emptyLabel.className = 'text-xs text-slate-500';
      emptyLabel.textContent = 'Sem tags por enquanto.';
      previewTags.appendChild(emptyLabel);
      return;
    }

    tags.forEach((tag) => {
      const badge = document.createElement('span');
      badge.className = 'status-pill status-pill-neutral';
      badge.textContent = tag;
      previewTags.appendChild(badge);
    });
  };

  [titleInput, contentInput, dateInput, typeInput, tagsInput].forEach((field) => {
    field?.addEventListener('input', () => {
      syncTitle();
      syncContent();
      syncType();
      syncDate();
      syncTags();
    });
  });

  syncTitle();
  syncContent();
  syncType();
  syncDate();
  syncTags();
}
