// JavaScript Slick customization - Home Newest Residences

if ($('.newestResidences').length != 0) {
  $('.newestResidences').slick({
    accessibility: 0,
    slidesToShow: 1,
    slidesToScroll: 1,
    arrows: true,
    fade: true,
    asNavFor: '.newestResidences-nav',
    mobileFirst: true,
    autoplay: true,
    autoplaySpeed: 3000,
  });

  $('.newestResidences-nav').slick({
    accessibility: 0,
    slidesToShow: 5,
    slidesToScroll: 1,
    asNavFor: '.newestResidences',
    dots: true,
    centerMode: true,
    focusOnSelect: true,
    responsive: [{ breakpoint: 1024, settings: { slidesToShow: 3, slidesToScroll: 3, infinite: true, dots: true } }]
  });

}
