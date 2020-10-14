$(document).ready(function () {
  if ($("body").hasClass("mobile-view")) {
    if ($("#PropertyNavigation").length) {
      $("#PropertyNavigation .propNavItem a span.propertySection").html(function (index, currentHtml) {
        return currentHtml.replace(" &", "&nbsp;&");
      }).html(function (index, currentHtml) {
        return currentHtml.replace(" ", "<br>");
      }).html(function (index, currentHtml) {
        return currentHtml.replace(" ", "<br>");
      });
      console.log($("#PropertyNavigation").height());
      $("#PropertyDetailsContainer").css("min-height", $("#PropertyNavigation").height() + "px");
    }

    var tabs = document.querySelectorAll('.tabToAccordionsOnMobile > .tab-pane');
    for (var i = 0; i < tabs.length; ++i) {
      if (tabs[i].classList.contains('active')) {
        tabs[i].classList.add('show');
        tabs[i].classList.remove('active');
      }
    }

    $('.tab-pane').on('show.bs.collapse', function (e) {
      $('.tab-pane').not(e.currentTarget).collapse('hide');
      console.log('tab shown');
    })

    //for covid
    if ($("#CovidTabs").length) {
      $("#CovidTabs .tab-pane").removeClass("show");
      $("#CovidTabs .tab-pane .tabTitle").addClass("collapsed");
      $("#CovidTabs .tab-pane .tabTitle").attr("aria-expanded", "false")
    }
  }




});