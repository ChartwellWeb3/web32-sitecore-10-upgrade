
$(function () {
  var lang = $('body').hasClass('fr') ? 'fr' : 'en';
  var tomorrow = moment().add(1, 'days').format('L');

  //bootstrap datepickers
  $('.input-group.date').datepicker({
    format: "mm/dd/yyyy",
    startDate: tomorrow,
    daysOfWeekDisabled: [0, 6],
    language: lang,
    clearBtn: true
  });
});