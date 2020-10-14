//re-enable search fields
$("#City, #PostalCode, #searchPropertyName").blur(function () {
  $("#City, #PostalCode, #searchPropertyName").prop('readonly', false).prop('disabled', false);
});

// Search bar - toggle to disable/enable fields --------------------------------------------
$("#City").focus(function () {
  $("#PostalCode, #searchPropertyName").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
  $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
});
$("#PostalCode").focus(function () {
  $("#City, #searchPropertyName").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
  $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
});
$("#searchPropertyName").focus(function () {
  $("#City, #PostalCode").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
  $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
});




// Search Page hover on panel -----------------------------------------------------------------
$(".gridCity .panel-primary > a > img, .gridCity .panel-heading, .gridCity .viewResBtn > a").hover(function () {
  $(this).closest(".panel").addClass("panelSearchHover");
}, function () {
  $(this).closest(".panel").removeClass("panelSearchHover");
});

$(".splitPageWrap .panel-assisted > a > img, .splitPageWrap .panel-assisted .panel-heading, .splitPageWrap .panel-assisted .viewResBtn > a").hover(function () {
  $(this).closest(".panel").addClass("panelSearchHoverAssisted");
}, function () {
  $(this).closest(".panel").removeClass("panelSearchHoverAssisted");
  });

$(document).ready(function () {

  
  $("#searchBarform").submit(function (event) {
    //if (!($("#PostalCode").val().match(/^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$/))) {
    var postalCodeValue =  $("#PostalCode").val().toString().trim();

    //if the postal code field has a value
    if (postalCodeValue.length > 0) {
      console.log("postal code length: " + postalCodeValue.trim().replace(" ","").length);
      //then we only accept entries of length 3 or 6
      if (!(postalCodeValue.trim().replace(" ", "").length == 3 || postalCodeValue.trim().replace(" ", "").length == 6)) {
        $('#PostalCodeFieldContainer div.form-group').popover('show');
        event.preventDefault();
      }
    }
  });

  $("#PostalCode").on("blur", function () {
    $('#PostalCodeFieldContainer div.form-group').popover('dispose');
  });

});