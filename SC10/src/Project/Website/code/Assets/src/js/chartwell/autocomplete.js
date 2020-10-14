//function getApiLocation() {
//  var autoCompleteAPILocationPrepend = '/sitecore/shell/api/sitecore/';
//  if (document.location.hostname !== 'chartwell') {
//    autoCompleteAPILocationPrepend = '/api/sitecore/';
//  }
//  return autoCompleteAPILocationPrepend;
//}
//function getApiLocation() {
//  var autoCompleteAPILocationPrepend = '/sitecore/shell/api/sitecore/';
//  if (document.location.hostname == 'chartwellcm' || document.location.hostname.indexOf('lab.') != -1 || document.location.hostname.indexOf('devops.') != -1) {
//    autoCompleteAPILocationPrepend = '/api/sitecore/';
//  }
//  return autoCompleteAPILocationPrepend;
//}

function getApiLocation(role) {
    return '/api/sitecore/';
}

$(document).ready(function () {
  var role = $('#ServerRole').val();
  var ApiLocation = getApiLocation(role);

  $("#searchPropertyName").typeahead(
    {
      hint: false,
      minlength: 3,
      highlight: true
    },
    {
      limit: 150,
      source: function (request, NULL, response) {
        $.ajax({
          async: true,
          url: ApiLocation + "Search/SearchProperty",
          type: "POST",
          dataType: "json",
          data: { Prefix: request, lang: $('#Language').val() },
          success: function (data) {
            response($.map(data, function (item) {
              return item.PropertyName;
            }));
          }
        });
      }
    }
  );

  $("#City").typeahead(
    {
      hint: false,
      minlength: 3,
      highlight: true
    },
    {
      limit: 150,
      source: function (request, NULL, response) {
        $.ajax({
          url: ApiLocation + "Search/CitySearch",
          type: "POST",
          dataType: "json",
          data: { term: request, lang: $('#Language').val() },
          success: function (data) {
            response($.map(data, function (item) {
              return item.City;
            }));
          }
        });
      }
    }
  );

  if ($("#FindAResidenceDiv").length) {

    // find a residence - toggle to disable/enable fields --------------------------------------------
    $(".citySearchField").focus(function () {
      $(".postalCode, .residenceSearchField").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
      $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
    });
    $(".postalCode").focus(function () {
      $(".citySearchField, .residenceSearchField").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
      $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
    });
    $(".residenceSearchField").focus(function () {
      $(".citySearchField, .postalCode").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
      $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
    });
    $(".residenceSearchField").typeahead(
      {
        hint: false,
        minlength: 3,
        highlight: true
      },
      {
        limit: 150,
        source: function (request, NULL, response) {
          $.ajax({
            async: true,
            url: ApiLocation + "Search/SearchProperty",
            type: "POST",
            dataType: "json",
            data: { Prefix: request, lang: $('#Language').val() },
            success: function (data) {
              response($.map(data, function (item) {
                return item.PropertyName;
              }));
            }
          });
        }
      }
    );

    $(".citySearchField").typeahead(
      {
        hint: false,
        minlength: 3,
        highlight: true
      },
      {
        limit: 150,
        source: function (request, NULL, response) {
          $.ajax({
            url: ApiLocation + "Search/CitySearch",
            type: "POST",
            dataType: "json",
            data: { term: request, lang: $('#Language').val() },
            success: function (data) {
              response($.map(data, function (item) {
                return item.City;
              }));
            }
          });
        }
      }
    );
  }




});



