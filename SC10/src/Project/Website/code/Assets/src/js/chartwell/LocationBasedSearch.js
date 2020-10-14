if (navigator.geolocation) {
  navigator.geolocation.getCurrentPosition(onPositionUpdate);
}

function onPositionUpdate(position) {
  var role = $('#ServerRole').val();
  var ApiLocation = getApiLocation(role); //function is in autocomplete.js

  var Lat = position.coords.latitude;
  var Lng = position.coords.longitude;
  var contextLang = $('#Language').val();

  $.ajax({
    url: ApiLocation + "LocationBasedSearch/LatLngSearch",

    type: "GET",
    dataType: "json",
    async: true,
    data: { Latitude: Lat, Longitude: Lng, LocContextLang: contextLang },
    success: function (data) {
      if (data.RedirectUrl && window.location.href.split('/')[3] === "") {
        window.location = data.RedirectUrl;
      }


      var href = location.href;

      if (!href.includes("Lat")) {
        var hasQuery = href.indexOf("?") + 1;
        var hasHash = href.indexOf("#") + 1;
        var appendix = (hasQuery ? "&" : "?") + "Lat=" + data.Latitude + "&Lng=" + data.Longitude;

        var u = hasHash ? href.replace("#", appendix + "#") : href + appendix;
        var obj = { Title: "", Url: u };
        history.pushState(obj, obj.Title, obj.Url);
      }

      if (data.City !== null && data.City !== "") {
        //if any of the fields have a value or is currently focused, don't set the city field value
        if ($("#City").val() === "" && $("#PostalCode").val() === "" && $("#searchPropertyName").val() === "") {
          if (!($("#City").is(':focus') || $("#PostalCode").is(':focus') || $("#searchPropertyName").is(':focus'))) {
            $("#City").val(data.City);
          }
        }

        //why do we check for the 'resName' class? because we don't want this on property pages.
        if (data.hasOwnProperty('PropertyName') && data.PropertyName !== null && data.PropertyName !== "" && $('#locDiv').length > 0 && !($('.resName').length > 0)) {
          var link = $("<a>");
          link.attr("href", data.PropertyItemUrl);
          link.text(data.PropertyName);
          link.addClass("locDiv link");
          $("#locDiv .closest").html(link);
          $('#nearResidenceId').removeClass('empty');
        }
      }

    }
  });

}

