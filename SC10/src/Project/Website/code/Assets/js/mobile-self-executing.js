$(document).ready(function(){if($("body").hasClass("mobile-view")){$("#PropertyNavigation").length&&($("#PropertyNavigation .propNavItem a span.propertySection").html(function(a,e){return e.replace(" &","&nbsp;&")}).html(function(a,e){return e.replace(" ","<br>")}).html(function(a,e){return e.replace(" ","<br>")}),console.log($("#PropertyNavigation").height()),$("#PropertyDetailsContainer").css("min-height",$("#PropertyNavigation").height()+"px"));for(var a=document.querySelectorAll(".tabToAccordionsOnMobile > .tab-pane"),e=0;e<a.length;++e)a[e].classList.contains("active")&&(a[e].classList.add("show"),a[e].classList.remove("active"));$(".tab-pane").on("show.bs.collapse",function(a){$(".tab-pane").not(a.currentTarget).collapse("hide"),console.log("tab shown")}),$("#CovidTabs").length&&($("#CovidTabs .tab-pane").removeClass("show"),$("#CovidTabs .tab-pane .tabTitle").addClass("collapsed"),$("#CovidTabs .tab-pane .tabTitle").attr("aria-expanded","false"))}});