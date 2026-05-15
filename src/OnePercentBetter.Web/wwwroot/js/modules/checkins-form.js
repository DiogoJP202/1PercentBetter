const form = document.querySelector('[data-checkin-form]');

if (form) {
  const ranges = [...form.querySelectorAll('[data-checkin-score]')];
  const totalElement = form.querySelector('[data-checkin-total]');

  const syncScores = () => {
    let total = 0;

    ranges.forEach((range) => {
      const value = Number.parseInt(range.value, 10) || 0;
      const output = range.parentElement?.querySelector('[data-checkin-score-output]');
      total += value;

      if (output) {
        output.textContent = value.toString();
      }
    });

    if (totalElement) {
      totalElement.textContent = total.toString();
    }
  };

  ranges.forEach((range) => {
    range.addEventListener('input', syncScores);
    range.addEventListener('change', syncScores);
  });

  syncScores();
}
