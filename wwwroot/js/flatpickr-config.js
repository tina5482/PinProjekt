window.initFlatpickr = (id, bookedAppointments) => {
    flatpickr(id, {
        enableTime: true,
        dateFormat: "Y-m-d H:i",
        minTime: "08:00",
        maxTime: "20:00",
        minuteIncrement: 15,
        disable: bookedAppointments.map(a => {
            return {
                from: a.start,
                to: a.end
            };
        }),
        onChange: function (selectedDates, dateStr, instance) {
            instance.set("minTime", "08:00");
            instance.set("maxTime", "20:00");
        }
    });
};

