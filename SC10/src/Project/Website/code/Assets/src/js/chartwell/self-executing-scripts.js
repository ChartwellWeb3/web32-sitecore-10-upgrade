//TO DO: Go through every one of these ones that aren't commented and
//    get rid of them if possible. 
//    or document/comment so we know what they are.

/*
$('.responsive-tabs').responsiveTabs({
  accordionOn: ['xs', 'sm']
});*/


/*$(window).on('load resize', function () {
  if ($(window).width() < 992) {
    $(".btn-group-vertical").addClass("btn-group btn-group-lg");
    $(".btn-group-vertical").removeClass("btn-group-vertical");
  } else {
    $(".btn-group").not('.btn-horizontal').addClass("btn-group-vertical");
    $(".btn-group-vertical").removeClass("btn-group btn-group-lg");
   }
});*/

$(document).ready(function () {

  var propNavDistFromTop = null;

  //sticky top nav
  if ($(window).width() > 767) {
    var offsetFromTop = $("#globalHeaderNavContainer").offset().top;
    $(window).scroll(function () {
      //console.log(offsetFromTop + ":" + $(this).scrollTop());
      if ($(this).scrollTop() > offsetFromTop) {
        $("#globalHeaderNavContainer").addClass("fixed-top");
        // add padding top to show content behind navbar
        $('body').css('padding-top', $("#globalHeaderNavContainer").outerHeight() + 'px');
      } else {
        $('#globalHeaderNavContainer').removeClass("fixed-top");
        // remove padding top from body
        $('body').css('padding-top', '0');
      }

      if ($("body").hasClass("responsive") || $("body").hasClass("tablet-view")) {

        if ($("#PropertyNavTop").length) {
          propNavDistFromTop = $("#PropertyNavTop").offset().top - $(this).scrollTop();
        }

        if ($("body").hasClass("responsive")) {
          $("#ContactUsForm").addClass("sticky-top");
          $("#GeneralContactForm").addClass("sticky-top");
          $("#RegionalContactForm").addClass("sticky-top");
          if ($("#GeneralContactForm").length) {
            $("#GeneralContactForm").css("top", ~~($("#globalHeaderNavContainer").height() + 20) + "px");
          }
          if ($("#RegionalContactForm").length) {
            $("#RegionalContactForm").css("top", ~~($("#globalHeaderNavContainer").height() + 20) + "px");
          }
        }

        if (propNavDistFromTop <= 0) {
          $("#PropertyNavigationContainer").addClass("isSticky")
          if ($("body").hasClass("responsive")) {
            $("#ContactUsForm").addClass("isSticky");
            $("#ContactUsForm").css("top", ~~($("#PropertyNavigationContainer").height() + 20) + "px");
          }
        } else {
          $("#PropertyNavigationContainer").removeClass("isSticky")
          if ($("body").hasClass("responsive")) {
            $("#ContactUsForm").removeClass("isSticky");
            $("#ContactUsForm").css("top", "0px");
          }
        }




      }



    });
  } // end if



  
  //following masks are for the jquery-mask-plugin
  var masks = {};
  masks.postalCode = 'SRS RSR';
  masks.phoneNumber = '(000) 000-0000';
  $('input#PostalCode').mask(masks.postalCode, {
    translation: {
      'R': { pattern: /[0-9]/ }
    }
  });
  $('input.phoneNumber').mask(masks.phoneNumber);

  $('.dropdown-toggle').dropdown();
  $('#carouselNewDevelopment').carousel();

  // input masks
  $('[mask]').each(function (e) {
    $(this).mask($(this).attr('mask'));
  });

  //if we have a hash in the location, check to see if there is a tabbed container with an id that matches the hash
  var locHash = location.hash ? location.hash : false;
  if (location.hash && $('.responsive-tabs-container a[href="' + locHash + '"]')) {
    $('a[href="' + locHash + '"]').click();
  }

  // for the find a residence page. we need to move the second search bar into the FindAResidenceDiv
  if ($("#FindAResidenceDiv").length) {
    $("#mainBlock #searchBarform").appendTo("#FindAResidenceDiv");
    $("#FindAResidenceDiv #searchBarform .col-xs-12.col-md-2").addClass("col-md-12").removeClass("col-md-2");
    $("#FindAResidenceDiv #searchBarform .residenceSearchformColumn").addClass("col-md-4");
  }

  // VIDEOS
  var $videoSrc;

  var vidsInList = $('.videoItem').map(function () {
    return $(this).data('src');
  }).get();

  $('.videoItem').click(function () {
    $videoSrc = $(this).data("src");
    getYTScript(function () {
      $("#gtmGenericVideo").attr('src', $videoSrc + "?modestbranding=1&showinfo=0&rel=0&iv_load_policy=3&enablejsapi=1");
    });
  });
  $('#videoModal').on('show.bs.modal', function (event) {
    $("#gtmGenericVideo").data('youtube', $videoSrc);
    $("#gtmGenericVideo").show();
    checkVideoIndexes();
  })
  $('#videoModal').on('hide.bs.modal', function (event) {
    $("#gtmGenericVideo").hide();
    $("#gtmGenericVideo").attr('src', "");
    $("#gtmGenericVideo").data('youtube', "");
    var e = document.getElementById("youtubeScript");
    if (e) { e.parentNode.removeChild(e); }
  })

  $('#videoModal #previous-button').click(function () {
    var currentIndex = vidsInList.findIndex(vid => vid == $("#gtmGenericVideo").data('youtube'));
    if (currentIndex > 0) {
      getYTScript(function () {
        $("#gtmGenericVideo").attr('src', vidsInList[currentIndex - 1] + "?modestbranding=1&showinfo=0&rel=0&iv_load_policy=3&enablejsapi=1");
        $("#gtmGenericVideo").data('youtube', vidsInList[currentIndex - 1]);
        checkVideoIndexes();
      });
    }
  });
  $('#videoModal #next-button').click(function () {
    var currentIndex = vidsInList.findIndex(vid => vid == $("#gtmGenericVideo").data('youtube'));
    if (currentIndex < (vidsInList.length - 1)) {
      getYTScript(function () {
        $("#gtmGenericVideo").attr('src', vidsInList[currentIndex + 1] + "?modestbranding=1&showinfo=0&rel=0&iv_load_policy=3&enablejsapi=1");
        $("#gtmGenericVideo").data('youtube', vidsInList[currentIndex + 1]);
        checkVideoIndexes();
      });
    }
  });

  function getYTScript(callback) {
    var e = document.getElementById("youtubeScript");
    if (e) {
      e.parentNode.removeChild(e);
    }

    var s = document.createElement('script');
    s.setAttribute('src', 'https://www.youtube.com/iframe_api');
    s.setAttribute('id', 'youtubeScript');
    s.onload = callback;
    document.body.appendChild(s);
  }

  function checkVideoIndexes() {
    var currentIndex = vidsInList.findIndex(vid => vid == $("#gtmGenericVideo").data('youtube'));
    //console.log(vidsInList);
    //console.log(currentIndex);
    currentIndex <= 0 ? $('#videoModal #previous-button').hide() : $('#videoModal #previous-button').show();
    currentIndex == (vidsInList.length - 1) ? $('#videoModal #next-button').hide() : $('#videoModal #next-button').show();
  }


  //ratings form
  $('#ratingsForm').submit(function (event) {
    if (!$('#CaptchaImagesList:checked').val() || ($('#CaptchaImagesList:checked').val().toLowerCase() != $('#Captcha_CaptchaAnswerImageName').val().toLowerCase())) {
      event.preventDefault();
      $('#CaptchaImagesList:first-child').addClass('input-validation-error');
      $('.text-danger[data-valmsg-for="CaptchaImagesList"]').text($('#CaptchaImagesList:first-child').data('val-required'));
      $('.text-danger[data-valmsg-for="CaptchaImagesList"]').addClass('field-validation-error').removeClass('field-validation-valid');
    } else {
      $('#CaptchaImagesList:first-child').removeClass('input-validation-error');
      $('.text-danger[data-valmsg-for="CaptchaImagesList"]').text("");
      $('.text-danger[data-valmsg-for="CaptchaImagesList"]').removeClass('field-validation-error').addClass('field-validation-valid');
    }
  });


  // SLIDER
  var sitename = location.host;
  var sitepath = location.pathname.replace("photos", "Contactus");
  var siteurl = "https://" + sitename + sitepath + "?photos=true";

  var href = sitepath;
  var i = 0;
  var direction = $(window).width() < 992 ? 'y' : 'x';

  //new gallery stuff
  if ($('.photoGallery').length) {
    console.log("has gallery");
    baguetteBox.run('.photoGallery', { animation: 'slideIn', buttons: true });
  }

  //moments that matter
  $('.grid').isotope({
    // set itemSelector so .grid-sizer is not used in layout
    itemSelector: '.grid-item',
    percentPosition: true,
    masonry: {
      // use element for option
      columnWidth: '.grid-sizer'
    }
  })

  //$('.collapse').collapse('show');
  $('.navbar-toggle').click(function () {
    $('.collapse').collapse('hide');
  })

  //this is to update contact us phone number in the footer if the user is on a property page.
  var btnPhoneNumber = '1-855-461-0685';
  if ($('body').hasClass('mobile-view') || $('body').hasClass('tablet-view')) {
    btnPhoneNumber = $('.phoneNumber')[0].text;
    console.log(btnPhoneNumber);
    if (btnPhoneNumber != '1-855-461-0685') {
      $('#devicePhoneText').text(btnPhoneNumber);
      $('#MobileFooterContactFormPhoneNumber').prop('href', 'tel:' + btnPhoneNumber);
    }
  }

  //add a class to #mainRow if it is a property page
  if ($('.resName').length && $('.resAddress').length) {
    $('#mainRow').addClass('resContainer');
  }


  $(document).click(function () {
    if ($('#PropertyLeftNav').hasClass('expanded')) {
      toggleLeftNav();
    }
  });
  $("#PropertyLeftNav").click(function (e) {
    e.stopPropagation();
  });

  //slideshow on homepage - in case we want specific images with the tabs. otherwise it's just a bootstrap carousel
  $(".sliderTabsWrap .hpSlideToggler").on("click", function () {
    var classToAdd = "";
    if ($(this).hasClass("nav-link")) {
      classToAdd = ($(this).attr("href")).replace("#","");
    } else {
      classToAdd = $(this).attr("aria-controls");
    }
    $("#VideoContainerRelative").attr("class", "").addClass(classToAdd);
  });
  $('#IntroBackgroundSlides').carousel();

  //duplicate ID finder
  var allElements = document.getElementsByTagName("*"), allIds = {}, dupIDs = [];
  for (var i = 0, n = allElements.length; i < n; ++i) {
    var el = allElements[i];
    if (el.id) {
      if (allIds[el.id] !== undefined) dupIDs.push(el.id);
      allIds[el.id] = el.name || el.id;
    }
  }
  if (dupIDs.length) { console.error("Duplicate ID's:", dupIDs); } else { console.log("No Duplicates Detected"); }


});
function toggleLeftNav() {
  $('#PropertyLeftNav').toggleClass('expanded');
  $('#leftNavExpander a .glyphicon').toggleClass('glyphicon-resize-full').toggleClass('glyphicon-resize-small');
}


