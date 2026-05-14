export function showFlashMessages(messages) {
  if (!window.Notyf || !Array.isArray(messages)) {
    return;
  }

  const notyf = new window.Notyf({
    duration: 3200,
    position: { x: 'right', y: 'top' },
    types: [
      {
        type: 'warning',
        background: '#f59e0b',
        icon: false
      },
      {
        type: 'info',
        background: '#38bdf8',
        icon: false
      }
    ]
  });

  messages.forEach((item) => {
    if (!item?.message) {
      return;
    }

    if (item.type === 'success') {
      notyf.success(item.message);
      return;
    }

    notyf.open({
      type: item.type ?? 'info',
      message: item.message
    });
  });
}
