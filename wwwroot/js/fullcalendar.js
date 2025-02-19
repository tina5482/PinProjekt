// Declare global variable
var globalDotNetHelper = null;
var serviceDurations = {
    "Šminkanje": 120,
    "Lash Lift": 60,
    "Brow Lift": 60,
    "Ekstenzije trepavica": 90,
    "Default": 60
};

// ✅ Store the current view and date
var currentView = "timeGridWeek";
var currentDate = new Date();

// ✅ Initialize FullCalendar
window.initFullCalendar = (dotNetHelper, events) => {
    console.log("📅 FullCalendar se inicijalizira...");

    var calendarEl = document.getElementById('calendar');
    if (!calendarEl) {
        console.error("❌ Element #calendar nije pronađen!");
        return;
    }

    // ✅ Assign globalDotNetHelper
    globalDotNetHelper = dotNetHelper;

    calendarEl.innerHTML = '';

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: currentView,
        allDaySlot: false,
        slotDuration: "00:15:00",
        slotMinTime: "08:00:00",
        slotMaxTime: "20:00:00",
        initialDate: currentDate,
        events: events.map(event => {
            let eventStart = new Date(event.start);
            let now = new Date();

            return {
                id: event.id,
                title: event.createdBy ? `${event.title} (${event.createdBy})` : event.title,
                start: event.start,
                end: event.end,
                serviceType: event.serviceType,
                backgroundColor: eventStart < now ? "#ff6666" : "#3788d8", // 🔴 Red for past, 🔵 Blue for future
                borderColor: eventStart < now ? "#cc0000" : "#205090"
            };
        }),
        eventClick: function (info) {
            showEventOptions(info.event);
        },
        datesSet: function (info) {
            currentView = info.view.type;
            currentDate = info.start;
        }
    });

    calendar.render();
    console.log("✅ FullCalendar je uspješno inicijaliziran.");
};

// ✅ Show Edit/Delete Modal
function showEventOptions(event) {
    if (!globalDotNetHelper) {
        console.error("❌ Greška: globalDotNetHelper nije definiran!");
        return;
    }

    let existingModal = document.getElementById("eventOptionsModal");
    if (existingModal) {
        existingModal.remove();
    }

    let modal = document.createElement("div");
    modal.id = "eventOptionsModal";
    modal.innerHTML = `
        <div class="modal-overlay" onclick="closeEventOptions()"></div>
        <div class="modal-content">
            <h3>Uredi ili Obriši Termin</h3>
            <label>Početak:</label>
            <input type="text" id="editStart" />
            <br><br>
            <button onclick="saveEditedEvent(${event.id})">Spremi novo vrijeme</button>
            <button onclick="deleteEvent(${event.id})">Obriši termin</button>
            <button onclick="closeEventOptions()">Zatvori</button>
        </div>
    `;

    document.body.appendChild(modal);

    // ✅ Ensure Flatpickr initializes properly
    setTimeout(() => {
        try {
            flatpickr("#editStart", {
                enableTime: true,
                dateFormat: "Y-m-d H:i",
                minTime: "08:00",
                maxTime: "20:00",
                minuteIncrement: 15
            }).setDate(event.start);
        } catch (error) {
            console.error("❌ Greška prilikom inicijalizacije Flatpickr-a:", error);
        }
    }, 300);
}

// ✅ Save Edited Appointment
function saveEditedEvent(eventId) {
    let newStart = document.getElementById("editStart").value;
    if (!globalDotNetHelper) {
        console.error("❌ Greška: globalDotNetHelper nije definiran!");
        return;
    }

    globalDotNetHelper.invokeMethodAsync('EditAppointment', eventId, newStart)
        .then(() => {
            console.log("✅ Termin je uspješno ažuriran.");
            closeEventOptions();
            refreshCalendar();
        })
        .catch(error => console.error("❌ Greška prilikom ažuriranja termina:", error));
}

// ✅ Delete Appointment
function deleteEvent(eventId) {
    if (!globalDotNetHelper) {
        console.error("❌ Greška: globalDotNetHelper nije definiran!");
        return;
    }

    globalDotNetHelper.invokeMethodAsync('CancelAppointment', eventId)
        .then(() => {
            console.log("✅ Termin uspješno obrisan.");
            closeEventOptions();
            refreshCalendar();
        })
        .catch(error => console.error("❌ Greška prilikom brisanja termina:", error));
}

// ✅ Refresh Calendar
function refreshCalendar() {
    if (!globalDotNetHelper) {
        console.error("❌ Greška: globalDotNetHelper nije definiran!");
        return;
    }

    globalDotNetHelper.invokeMethodAsync('RefreshCalendar')
        .then(() => console.log("✅ Kalendar osvježen"))
        .catch(error => console.error("❌ Greška prilikom osvježavanja kalendara:", error));
}

// ✅ Close Modal
function closeEventOptions() {
    let modal = document.getElementById("eventOptionsModal");
    if (modal) modal.remove();
}
