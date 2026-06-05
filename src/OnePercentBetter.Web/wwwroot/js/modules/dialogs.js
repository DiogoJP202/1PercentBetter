export function bindConfirmDialogs(root = document) {
  if (!window.Swal) {
    return;
  }

  root.querySelectorAll('form[data-confirm]').forEach((form) => {
    if (form.hasAttribute('data-async-action')) {
      return;
    }

    if (form.dataset.confirmBound === 'true') {
      return;
    }

    form.dataset.confirmBound = 'true';
    form.addEventListener('submit', async (event) => {
      if (form.dataset.confirmed === 'true') {
        delete form.dataset.confirmed;
        return;
      }

      event.preventDefault();

      const tone = form.dataset.confirmTone ?? 'default';
      const result = await window.Swal.fire({
        title: form.dataset.confirmTitle ?? 'Confirmar ação?',
        text: form.dataset.confirmText ?? 'Esta ação será aplicada agora.',
        icon: form.dataset.confirmIcon ?? 'warning',
        showCancelButton: true,
        confirmButtonText: form.dataset.confirmButton ?? 'Confirmar',
        cancelButtonText: form.dataset.cancelButton ?? 'Cancelar',
        reverseButtons: true,
        background: '#0f172a',
        color: '#e2e8f0',
        confirmButtonColor: tone === 'danger' ? '#f43f5e' : '#10b981',
        cancelButtonColor: '#334155'
      });

      if (!result.isConfirmed) {
        return;
      }

      form.dataset.confirmed = 'true';

      if (typeof form.requestSubmit === 'function') {
        form.requestSubmit();
        return;
      }

      form.submit();
    });
  });
}
