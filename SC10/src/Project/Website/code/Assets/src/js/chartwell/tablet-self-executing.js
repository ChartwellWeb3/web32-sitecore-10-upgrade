$(document).ready(function () {
  if ($("#PropertyNavigation").length) {
    $("#PropertyNavigation .propNavItem a span.propertySection").html(function (index, currentHtml) {
      return currentHtml.replace(" &", "&nbsp;&");
    }).html(function (index, currentHtml) {
      return currentHtml.replace(" ", "<br>");
    }).html(function (index, currentHtml) {
      return currentHtml.replace(" ", "<br>");
    })
  }
});