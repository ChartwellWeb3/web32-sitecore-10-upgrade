!function(t,e){"object"==typeof exports&&"object"==typeof module?module.exports=e():"function"==typeof define&&define.amd?define([],e):"object"==typeof exports?exports.inView=e():t.inView=e()}(this,function(){return function(t){function e(o){if(n[o])return n[o].exports;var i=n[o]={exports:{},id:o,loaded:!1};return t[o].call(i.exports,i,i.exports,e),i.loaded=!0,i.exports}var n={};return e.m=t,e.c=n,e.p="",e(0)}([function(t,e,n){"use strict";var o=function(t){return t&&t.__esModule?t:{default:t}}(n(2));t.exports=o.default},function(t,e){t.exports=function(t){var e=typeof t;return null!=t&&("object"==e||"function"==e)}},function(t,e,n){"use strict";function o(t){return t&&t.__esModule?t:{default:t}}Object.defineProperty(e,"__esModule",{value:!0});var i=o(n(9)),r=o(n(3)),u=n(4);e.default=function(){if("undefined"!=typeof window){var t={history:[]},e={offset:{},threshold:0,test:u.inViewport},n=(0,i.default)(function(){t.history.forEach(function(e){t[e].check()})},100);["scroll","resize","load"].forEach(function(t){return addEventListener(t,n)}),window.MutationObserver&&addEventListener("DOMContentLoaded",function(){new MutationObserver(n).observe(document.body,{attributes:!0,childList:!0,subtree:!0})});var o=function(n){if("string"==typeof n){var o=[].slice.call(document.querySelectorAll(n));return t.history.indexOf(n)>-1?t[n].elements=o:(t[n]=(0,r.default)(o,e),t.history.push(n)),t[n]}};return o.offset=function(t){if(void 0===t)return e.offset;var n=function(t){return"number"==typeof t};return["top","right","bottom","left"].forEach(n(t)?function(n){return e.offset[n]=t}:function(o){return n(t[o])?e.offset[o]=t[o]:null}),e.offset},o.threshold=function(t){return"number"==typeof t&&t>=0&&t<=1?e.threshold=t:e.threshold},o.test=function(t){return"function"==typeof t?e.test=t:e.test},o.is=function(t){return e.test(t,e)},o.offset(0),o}}()},function(t,e){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var n=function(){function t(t,e){for(var n=0;n<e.length;n++){var o=e[n];o.enumerable=o.enumerable||!1,o.configurable=!0,"value"in o&&(o.writable=!0),Object.defineProperty(t,o.key,o)}}return function(e,n,o){return n&&t(e.prototype,n),o&&t(e,o),e}}(),o=function(){function t(e,n){(function(t,e){if(!(t instanceof e))throw new TypeError("Cannot call a class as a function")})(this,t),this.options=n,this.elements=e,this.current=[],this.handlers={enter:[],exit:[]},this.singles={enter:[],exit:[]}}return n(t,[{key:"check",value:function(){var t=this;return this.elements.forEach(function(e){var n=t.options.test(e,t.options),o=t.current.indexOf(e),i=o>-1,r=!n&&i;n&&!i&&(t.current.push(e),t.emit("enter",e)),r&&(t.current.splice(o,1),t.emit("exit",e))}),this}},{key:"on",value:function(t,e){return this.handlers[t].push(e),this}},{key:"once",value:function(t,e){return this.singles[t].unshift(e),this}},{key:"emit",value:function(t,e){for(;this.singles[t].length;)this.singles[t].pop()(e);for(var n=this.handlers[t].length;--n>-1;)this.handlers[t][n](e);return this}}]),t}();e.default=function(t,e){return new o(t,e)}},function(t,e){"use strict";Object.defineProperty(e,"__esModule",{value:!0}),e.inViewport=function(t,e){var n=t.getBoundingClientRect(),o=n.top,i=n.right,r=n.bottom,u=n.left,f=n.width,s=n.height,a=r,c=window.innerWidth-u,l=window.innerHeight-o,d=i,h=e.threshold*f,p=e.threshold*s;return a>e.offset.top+p&&c>e.offset.right+h&&l>e.offset.bottom+p&&d>e.offset.left+h}},function(t,e){(function(e){var n="object"==typeof e&&e&&e.Object===Object&&e;t.exports=n}).call(e,function(){return this}())},function(t,e,n){var o=n(5),i="object"==typeof self&&self&&self.Object===Object&&self,r=o||i||Function("return this")();t.exports=r},function(t,e,n){var o=n(1),i=n(8),r=n(10),u="Expected a function",f=Math.max,s=Math.min;t.exports=function(t,e,n){function a(e){var n=p,o=v;return p=v=void 0,x=e,m=t.apply(o,n)}function c(t){var n=t-b;return void 0===b||n>=e||n<0||C&&t-x>=y}function l(){var t=i();return c(t)?d(t):void(g=setTimeout(l,function(t){var n=e-(t-b);return C?s(n,y-(t-x)):n}(t)))}function d(t){return g=void 0,O&&p?a(t):(p=v=void 0,m)}function h(){var t=i(),n=c(t);if(p=arguments,v=this,b=t,n){if(void 0===g)return function(t){return x=t,g=setTimeout(l,e),w?a(t):m}(b);if(C)return g=setTimeout(l,e),a(b)}return void 0===g&&(g=setTimeout(l,e)),m}var p,v,y,m,g,b,x=0,w=!1,C=!1,O=!0;if("function"!=typeof t)throw new TypeError(u);return e=r(e)||0,o(n)&&(w=!!n.leading,y=(C="maxWait"in n)?f(r(n.maxWait)||0,e):y,O="trailing"in n?!!n.trailing:O),h.cancel=function(){void 0!==g&&clearTimeout(g),x=0,p=b=v=g=void 0},h.flush=function(){return void 0===g?m:d(i())},h}},function(t,e,n){var o=n(6);t.exports=function(){return o.Date.now()}},function(t,e,n){var o=n(7),i=n(1),r="Expected a function";t.exports=function(t,e,n){var u=!0,f=!0;if("function"!=typeof t)throw new TypeError(r);return i(n)&&(u="leading"in n?!!n.leading:u,f="trailing"in n?!!n.trailing:f),o(t,e,{leading:u,maxWait:e,trailing:f})}},function(t,e){t.exports=function(t){return t}}])});var isMobile=!1,isScenariosPage=!0;function getIsScenarioPage(){isScenariosPage=-1!=window.location.href.toLowerCase().indexOf("managing-emotions-as-parents-age")||-1!=window.location.href.toLowerCase().indexOf("gerer-emotions-parents-qui-vieillissent")}function sizeVideosToFit(){var t;document.querySelector("#CXVideoContainer")&&(t=document.querySelector("#CXVideoContainer").querySelectorAll("iframe")[0]).setAttribute("height",t.offsetWidth/1.91+"px");document.querySelector("#CXVideoContainer")&&(t=document.querySelector("#CXVideoContainer").querySelectorAll("iframe")[0]).setAttribute("height",t.offsetWidth/1.91+"px")}$(function(){var t=$("body").hasClass("fr")?"fr":"en",e=moment().add(1,"days").format("L");document.body.classList.contains("mobile-view")&&(isMobile=!0),console.log(isMobile),$("#visitdate").prop("readonly","readonly").datepicker(),$(".input-group.date").datepicker({format:"mm/dd/yyyy",startDate:e,language:t,clearBtn:!0,container:"#dateContainer"})}),document.addEventListener("DOMContentLoaded",function(){sizeVideosToFit(),getIsScenarioPage()});