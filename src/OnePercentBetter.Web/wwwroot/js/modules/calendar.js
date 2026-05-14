const calendarElement = document.querySelector('[data-habit-calendar]');

if (calendarElement && window.FullCalendar) {
  const calendar = new window.FullCalendar.Calendar(calendarElement, {
    initialView: 'dayGridMonth',
    height: 'auto',
    locale: 'pt-br',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,listWeek'
    },
    events: []
  });

  calendar.render();
}
