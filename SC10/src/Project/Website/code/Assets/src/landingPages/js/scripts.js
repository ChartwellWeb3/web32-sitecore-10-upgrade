var isMobile = false;
var isScenariosPage = true;



function getIsScenarioPage() {
    if ((((window.location.href).toLowerCase()).indexOf("managing-emotions-as-parents-age") == -1) && (((window.location.href).toLowerCase()).indexOf("gerer-emotions-parents-qui-vieillissent") == -1)) {
        isScenariosPage = false;
    } else {
        isScenariosPage = true;
    }
}

function sizeVideosToFit() {
  if (document.querySelector('#CXVideoContainer')) {
    var element = document.querySelector('#CXVideoContainer').querySelectorAll('iframe')[0];
    element.setAttribute('height', (element.offsetWidth / 1.91) + 'px');
  }
  if (document.querySelector('#CXVideoContainer')) {
    var element = document.querySelector('#CXVideoContainer').querySelectorAll('iframe')[0];
    element.setAttribute('height', (element.offsetWidth / 1.91) + 'px');
  }
}

$(function () {
  var lang = $('body').hasClass('fr') ? 'fr' : 'en';
  var tomorrow = moment().add(1, 'days').format('L');

  if (document.body.classList.contains("mobile-view")) {
    isMobile = true;
  }
  console.log(isMobile);

  //$("#visitdate").prop("readonly", "readonly");
  $("#visitdate").prop("readonly", "readonly").datepicker();

  //bootstrap datepickers
  $('.input-group.date').datepicker({
      format: "mm/dd/yyyy",
      startDate: tomorrow,
      language: lang,
      clearBtn: true,
      container: "#dateContainer"
  });
});

document.addEventListener('DOMContentLoaded', function () {

    sizeVideosToFit();
    getIsScenarioPage();

});
