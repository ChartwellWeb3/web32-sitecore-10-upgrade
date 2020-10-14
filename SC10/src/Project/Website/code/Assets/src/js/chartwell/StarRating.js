﻿$(document).ready(function () {

  /* 1. Visualizing things on Hover - See next part for action on click */
  $('#stars li').on('mouseover', function () {
    var onStar = parseInt($(this).data('value'), 10); // The star currently mouse on

    // Now highlight all the stars that's not after the current hovered star
    $(this).parent().children('li.star').each(function (e) {
      if (e < onStar) {
        $(this).addClass('hover');
      }
      else {
        $(this).removeClass('hover');
      }
    });

  }).on('mouseout', function () {
    $(this).parent().children('li.star').each(function (e) {
      $(this).removeClass('hover');
    });
  });


  $("#flip").click(function () {
    $("#panel").slideToggle("fast", "swing");
  });



  /* 2. Action to perform on click */
  $('#stars li').on('click', function () {
    var onStar = parseInt($(this).data('value'), 10); // The star currently selected
    var stars = $(this).parent().children('li.star');
    $('#Ratings-error').css('display', 'none');

    for (i = 0; i < stars.length; i++) {
      $(stars[i]).removeClass('selected');
    }

    for (i = 0; i < onStar; i++) {
      $(stars[i]).addClass('selected');
    }
    $("#Ratings").val(onStar);
    console.log(onStar);

  });
});