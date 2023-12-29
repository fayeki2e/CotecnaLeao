// Code goes here
/*
document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth'
    });
    calendar.render();
});
*/
$(document)
  .ready(function () {

    // page is now ready, initialize the calendar...

    var calendar = $('#calendar')
      .fullCalendar({
        // put your options and callbacks here
        header: {
          left: 'prev,next today',
          center: 'title',
          right: 'year,month,basicWeek'

          },
          weekNumberCalculation: 'ISO',
          firstDay: 0,
        height: 'auto',
        selectable: false,
        dragabble: false,
        defaultView: 'year',
        yearColumns: 3,
        durationEditable: false,
        bootstrap: false,

        events: [{
          title: 'Some event',
          start: new Date('2017-1-10'),
          end: new Date('2017-1-20'),
          id: 1,
          allDay: true,
          editable: true,
          eventDurationEditable: true
        },
          {
            title: 'Republic Day',
            start: '2019-01-26',
            color: '#F64846',   // an option!
            textColor: 'black', // an option!
            background: 'red'
          },
          {
            title: 'Chatrpati Shivaji Maharaj Jayanti',
            start: '2019-02-19',
            color: '#F64846',   // an option!
            textColor: 'black', // an option!
            background: 'red'
          },
          {
            title: 'Holi',
            start: '2019-03-02',
            color: '#F64846',   // an option!
            textColor: 'black', // an option!
            background: 'red'
          },
          {
            title: 'Half-Day',
            start: '2019-01-02',
            color: '#B3BBC2',   // an option!
            textColor: 'black', // an option!
            background: '#B3BBC2'
          },
          {
            title: 'Annual leave',
            start: '2018-12-26',
            end:'2018-12-31',
            color: '#1B8EB7',   // an option!
            textColor: 'black', // an option!
            background: 'red'
          }],
        select: function (start, end, allDay) {
          var title = prompt('Event Title:')
          if (title) {
            var event = {
              title: title,
              start: start.clone(),
              end: end.clone(),
              allDay: true,
              editable: true,
              eventDurationEditable: true,
              eventStartEditable: true,
              color: ''
            }
            calendar.fullCalendar('renderEvent', event, true)
          }
        }
      })
  })