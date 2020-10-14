'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify-es').default;
var replace = require('gulp-replace');

sass.compiler = require('node-sass');

gulp.task('sass:chartwell', function () {
  return gulp.src('./Assets/src/sass/chartwell.scss')
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    .pipe(gulp.dest('./Assets/Styles/'));
});

gulp.task('sass:vendor', function () {
  return gulp.src('./Assets/src/sass/vendor.scss')
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    .pipe(gulp.dest('./Assets/Styles/'));
});

gulp.task('sass:wufoo', function () {
  return gulp.src('./Assets/src/sass/wufoo.scss')
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    .pipe(gulp.dest('./Assets/Styles/'));
});

gulp.task('sass:chartwell:mobile', function () {
  return gulp.src('./Assets/src/sass/chartwell-mobile.scss')
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    .pipe(gulp.dest('./Assets/Styles/'));
});

gulp.task('sass:watch', function () {
  gulp.watch('./Assets/src/sass/**/*.scss', gulp.series('sass:vendor', 'sass:chartwell', 'sass:chartwell:mobile'));
});

gulp.task('sass:chartwell:watch', function () {
  gulp.watch('./Assets/src/sass/chartwell/**/*.scss', gulp.series('sass:chartwell'));
});

gulp.task('sass:landingpages', function () {
  return gulp.src('./Assets/src/landingPages/scss/styles.scss')
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    .pipe(gulp.dest('./Assets/Styles/landingpages/'));
});
gulp.task('sass:landingpages:watch', function () {
  gulp.watch('./Assets/src/landingpages/**/*.scss', gulp.series('sass:landingpages'));
});

gulp.task('js:all', done => {
  gulp.series("js:chartwell", "js:vendor")(done());
});

/*
 * IMPORTANT: UPDATE THIS MANIFEST WHENEVER THE CONTENTS OF VENDOR.JS CHANGES
 * 
 * FOLDER: Assets/js/vendor/
 * 
 * autocomplete.js has variables that are specific to different environments.
 * 
 * GET RID OF JQUERYUI <-- to-do 
 * 
*/
gulp.task('js:vendor', function () {
  return gulp.src(
    [
      // DO NOT DELETE these next 2
      // besides our other uses, the jquery and jquery.validate a dependencies for asp.net MVC. 
      // You must have a verion of them in the repo at all times
      'node_modules/jquery/dist/jquery.js', // v3.4.1
      'node_modules/jquery-validation/dist/jquery.validate.js', // v1.9.1

      // DO NOT DELETE the following are from microsoft and tied into MVC.
      'Assets/src/js/vendor/jquery.validate.unobtrusive.js', // v3.2.11
      'Assets/src/js/vendor/jquery.unobtrusive-ajax.js',  // v3.2.6

      'node_modules/bootstrap/dist/js/bootstrap.bundle.js', // 4.3.1
      'node_modules/moment/min/moment-with-locales.js', // 2.24.0

      'node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker.js', // 1.9.0
      'node_modules/bootstrap-datepicker/dist/locales/bootstrap-datepicker.fr.min.js', // 1.9.0

      'node_modules/jquery-mask-plugin/dist/jquery.mask.js', // 1.14.16

      'node_modules/typeahead.js/dist/typeahead.jquery.js', // 0.11.1 this is a bit obsolete, but it's a tiny plugin and i want to get rid of jquery ui

      'node_modules/baguettebox.js/dist/baguetteBox.js', // 1.11.0

      'node_modules/isotope-layout/dist/isotope.pkgd.js', // 3.0.6



      //'Assets/src/js/vendor/jquery-ui-1.12.1.js', 
      // jquery ui is obsolete. no new dev 
      // use BOOTSTRAP alternatives.for datepicker https://uxsolutions.github.io/bootstrap-datepicker/
      // and twitter/typeadhead.js for typeaheads


      //'Assets/src/js/vendor/jquery.maskedinput.js', // obsolete. last dev 5 years ago replace with https://github.com/RobinHerbots/Inputmask

      //'Assets/src/js/vendor/jquery.dynamicmaxheight.js', // delete do this with vanilla js/css
      //'Assets/src/js/vendor/jquery.fancybox.js', // delete replace with BOOTSTRAP modal
    ]
  ).pipe(concat('vendor.js'))
    .pipe(gulp.dest('Assets/js/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/'));
});

/*
 * IMPORTANT: UPDATE THIS MANIFEST WHENEVER THE CONTENTS OF CHARTWELL.JS CHANGES
 * 
 * FOLDER "Assets/js/chartwell"
    datepicker-fr.js,
    datepicker-en.js,
    layout.js,
    LocationBasedSearch.js,
    self-executing-scripts.js,
    questionnaire.js,
    homepage-newest-residences.js

 * autocomplete.js has variables that are specific to different environments.
 * 
*/

gulp.task('js:chartwell', function () {
  return gulp.src(
    [

      'Assets/src/js/chartwell/autocomplete.js',
      'Assets/src/js/chartwell/layout.js', // needs cleanup

      'Assets/src/js/chartwell/search.js',
      //'Assets/src/js/chartwell/LocationBasedSearch.js',
      'Assets/src/js/chartwell/pvdatepicker.js', 

      'Assets/src/js/chartwell/self-executing-scripts.js',
      'Assets/src/js/chartwell/questionnaire.js',

      'Assets/src/js/chartwell/StarRating.js',
      'Assets/src/js/chartwell/homepage-newest-residences.js'          
    ]
  ).pipe(concat('chartwell.js'))
    .pipe(gulp.dest('Assets/js/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/'));
});
gulp.task('js:chartwell:watch', function () {
  gulp.watch('./Assets/src/js/chartwell/**/*.js', gulp.series('js:chartwell'));
});

gulp.task('js:mobile', function () {
  return gulp.src(
    [
      'Assets/src/js/chartwell/mobile-self-executing.js'
    ]
  ).pipe(gulp.dest('Assets/js/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/'));
});
gulp.task('js:mobile:watch', function () {
  gulp.watch('./Assets/src/js/charwell/mobile-self-executing.js', gulp.series('js:mobile'));
});

gulp.task('js:scenarioslandingPage', function () {
  return gulp.src(
    [
      'Assets/src/landingPages/js/in-view.min.js',
      'Assets/src/landingPages/js/scripts.js'
    ]
  ).pipe(concat('scenarioslandingpage.js'))
    .pipe(gulp.dest('Assets/js/landingpages/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/landingpages/'));
});
gulp.task('js:landingpages:watch', function () {
  gulp.watch('./Assets/src/**/*.js', gulp.series('js:scenarioslandingPage'));
});

gulp.task('js:questionnaires', function () {
  return gulp.src(
    [
      'Assets/src/js/questionnaires/**/runtime*.js',
      'Assets/src/js/questionnaires/**/polyfills*.js',
      'Assets/src/js/questionnaires/**/main*.js',
    ]
  ).pipe(concat('questionnaires.js'))
    .pipe(gulp.dest('Assets/js/questionnaires/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/questionnaires/'));
});

gulp.task('css:questionnaires', function () {
  return gulp.src(
    [
      'Assets/src/js/questionnaires/**/*.css'
    ]
  ).pipe(concat('questionnaires.css'))
    .pipe(gulp.dest('Assets/Styles/questionnaires/'));
});



gulp.task('js:budgetassistant-en', function () {
  return gulp.src(
    [
      'Assets/src/js/chartwell/budgetAssistant.js'
    ]
  ).pipe(gulp.dest('Assets/js/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/'));
});

gulp.task('js:budgetassistant-fr', function () {
  return gulp.src(
    [
      'Assets/src/js/chartwell/budgetAssistant-fr.js'
    ]
  ).pipe(gulp.dest('Assets/js/'))
    .pipe(uglify())
    .pipe(gulp.dest('Assets/js/'));
});
gulp.task('js:budgetAssistant', done => {
  gulp.series('js:budgetassistant-en', 'js:budgetassistant-fr')(done());
});

gulp.task('serialization:watch', function () {
  gulp.watch('C:\inetpub\wwwroot\sc92dev\App_Data\serialization/**/*', gulp.series('serializationcopy'));
});

gulp.task('serializationcopy', function () {
  return gulp.src(
    [
      'C:\inetpub\wwwroot\sc92sc.dev.local\App_Data\serialization/**/*'
    ]
  ).pipe(gulp.dest('App_Data/serialization'));
});

gulp.task('clean-cachebuster-js', function () {
  return gulp.src(
    [
      'Views/BaseLayout/**/*.cshtml'
    ]
  ).pipe(replace(/\?v=chart_\d*/g, "")).pipe(gulp.dest('Views/BaseLayout'));
});

gulp.task('cachebuster-js', function () {
  return gulp.src(
    [
      'Views/BaseLayout/**/*.cshtml'
    ]
  ).pipe(replace(/~\/Assets\/js\/\S*\.js/g, function (match) {
    return match.concat("?v=chart_".concat(Date.now()));
  })).pipe(gulp.dest('Views/BaseLayout'));
});

gulp.task('clean-cachebuster-css', function () {
  return gulp.src(
    [
      'Views/BaseLayout/**/*.cshtml'
    ]
  ).pipe(replace(/\?v=chart_\d*/g, "")).pipe(gulp.dest('Views/BaseLayout'));
});

gulp.task('cachebuster-css', function () {
  return gulp.src(
    [
      'Views/BaseLayout/**/*.cshtml'
    ]
  ).pipe(replace(/~\/Assets\/Styles\/\S*\.css/g, function (match) {
    return match.concat("?v=chart_".concat(Date.now()));
  })).pipe(gulp.dest('Views/BaseLayout'));
});

gulp.task('cachebuster', done => {
  gulp.series('clean-cachebuster-js', 'clean-cachebuster-css','cachebuster-js', 'cachebuster-css')(done());
});

gulp.task('kitchenSink', done => {
  gulp.series('sass:vendor', 'sass:chartwell', 'sass:wufoo', 'sass:landingpages', 'sass:chartwell:mobile', 'js:scenarioslandingPage', 'js:chartwell', 'js:vendor', 'js:budgetAssistant', 'js:mobile', 'js:questionnaires', 'css:questionnaires', 'cachebuster')(done());
});