window.initFullCalendar = (dotNetHelper, events) => {
    console.log("FullCalendar se inicijalizira...");
    console.log("Zakazani termini:", events);

    var calendarEl = document.getElementById('calendar');

    if (!calendarEl) {
        console.error("Element #calendar nije pronađen!");
        return;
    }

    calendarEl.innerHTML = ''; // Resetiranje rasporeda

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'timeGridWeek',
        allDaySlot: false,
        slotDuration: "00:15:00", // Interval od 15 min
        slotMinTime: "08:00:00",
        slotMaxTime: "20:00:00",
        events: events.map(event => ({
            id: event.id,
            title: event.createdBy ? `${event.title} (${event.createdBy})` : event.title,
            start: event.start,
            end: event.end
        })),
        eventClick: function (info) {
            console.log("Kliknut termin:", info.event);
            dotNetHelper.invokeMethodAsync('CancelAppointment', parseInt(info.event.id));
        }
    });

    calendar.render();
    console.log("FullCalendar je uspješno inicijaliziran.");
};
