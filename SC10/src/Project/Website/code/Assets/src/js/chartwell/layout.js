// layout needs


// left navigation transformation for mobile version ----------------------------------------
$(window).on('load resize', function () {
  if ($(window).width() < 992 && $("body").hasClass("mobile-view")) {
      $(".btn-group-vertical").addClass("btn-group btn-group-lg");
      $(".btn-group-vertical").removeClass("btn-group-vertical");
  }
});



// Contact Form Validation
$(document).ready(function () {

  $('#visitdate').attr('readonly', 'readonly').datepicker();
  /* permanent Banner move after .resQuote */

  // Residences without image
  //var bgAddressCleanup = /\"|\'|\)/g;
  if (document.getElementById('bgPropertyImage')) { //existence check first
      if ($(window).width() > 992) {
        if ($('#bgPropertyImage').attr('style').indexOf('none') > -1) {
          $('#bgPropertyImage').css('height', '50px');
          $('.resContainerUp').css('margin-top', '-45px');
        }
      }
      else {
        if ($('#bgPropertyImage').attr('style').indexOf('none') > -1) {
          $('#bgPropertyImage').css('height', '50px');
          $('.resContainerUp').css('margin-top', '-45px');
        }
      }
  }	
});// document ready



// .gridCity > .col-md-4:nth-child(3n+1) START
	$(".gridCity > .col-md-4:nth-child(3n+1)").css("clear", "left");

	$('*[data-ajax-mode="replace"]').click(
    function() {
        setTimeout(
            function() {
                $(".gridCity > .col-md-4:nth-child(3n+1)").css("clear", "left");
            },
            4000);
    });
// END gridCity

