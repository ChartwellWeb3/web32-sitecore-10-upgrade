!function(t,e,i,n){"use strict";var r,s=["","webkit","Moz","MS","ms","o"],o=e.createElement("div"),a="function",h=Math.round,u=Math.abs,c=Date.now;function l(t,e,i){return setTimeout(T(t,i),e)}function p(t,e,i){return!!Array.isArray(t)&&(f(t,i[e],i),!0)}function f(t,e,i){var r;if(t)if(t.forEach)t.forEach(e,i);else if(t.length!==n)for(r=0;r<t.length;)e.call(i,t[r],r,t),r++;else for(r in t)t.hasOwnProperty(r)&&e.call(i,t[r],r,t)}function d(e,i,n){var r="DEPRECATED METHOD: "+i+"\n"+n+" AT \n";return function(){var i=new Error("get-stack-trace"),n=i&&i.stack?i.stack.replace(/^[^\(]+?[\n$]/gm,"").replace(/^\s+at\s+/gm,"").replace(/^Object.<anonymous>\s*\(/gm,"{anonymous}()@"):"Unknown Stack Trace",s=t.console&&(t.console.warn||t.console.log);return s&&s.call(t.console,r,n),e.apply(this,arguments)}}r="function"!=typeof Object.assign?function(t){if(t===n||null===t)throw new TypeError("Cannot convert undefined or null to object");for(var e=Object(t),i=1;i<arguments.length;i++){var r=arguments[i];if(r!==n&&null!==r)for(var s in r)r.hasOwnProperty(s)&&(e[s]=r[s])}return e}:Object.assign;var v=d(function(t,e,i){for(var r=Object.keys(e),s=0;s<r.length;)(!i||i&&t[r[s]]===n)&&(t[r[s]]=e[r[s]]),s++;return t},"extend","Use `assign`."),m=d(function(t,e){return v(t,e,!0)},"merge","Use `assign`.");function g(t,e,i){var n,s=e.prototype;(n=t.prototype=Object.create(s)).constructor=t,n._super=s,i&&r(n,i)}function T(t,e){return function(){return t.apply(e,arguments)}}function y(t,e){return typeof t==a?t.apply(e&&e[0]||n,e):t}function E(t,e){return t===n?e:t}function I(t,e,i){f(S(e),function(e){t.addEventListener(e,i,!1)})}function A(t,e,i){f(S(e),function(e){t.removeEventListener(e,i,!1)})}function _(t,e){for(;t;){if(t==e)return!0;t=t.parentNode}return!1}function C(t,e){return t.indexOf(e)>-1}function S(t){return t.trim().split(/\s+/g)}function b(t,e,i){if(t.indexOf&&!i)return t.indexOf(e);for(var n=0;n<t.length;){if(i&&t[n][i]==e||!i&&t[n]===e)return n;n++}return-1}function P(t){return Array.prototype.slice.call(t,0)}function D(t,e,i){for(var n=[],r=[],s=0;s<t.length;){var o=e?t[s][e]:t[s];b(r,o)<0&&n.push(t[s]),r[s]=o,s++}return i&&(n=e?n.sort(function(t,i){return t[e]>i[e]}):n.sort()),n}function x(t,e){for(var i,r,o=e[0].toUpperCase()+e.slice(1),a=0;a<s.length;){if((r=(i=s[a])?i+o:e)in t)return r;a++}return n}var w=1;function O(e){var i=e.ownerDocument||e;return i.defaultView||i.parentWindow||t}var R="ontouchstart"in t,M=x(t,"PointerEvent")!==n,z=R&&/mobile|tablet|ip(ad|hone|od)|android/i.test(navigator.userAgent),N=25,X=1,Y=2,F=4,W=8,q=1,k=2,H=4,L=8,U=16,V=k|H,j=L|U,G=V|j,Z=["x","y"],B=["clientX","clientY"];function $(t,e){var i=this;this.manager=t,this.callback=e,this.element=t.element,this.target=t.options.inputTarget,this.domHandler=function(e){y(t.options.enable,[t])&&i.handler(e)},this.init()}function J(t,e,i){var r=i.pointers.length,s=i.changedPointers.length,o=e&X&&r-s==0,a=e&(F|W)&&r-s==0;i.isFirst=!!o,i.isFinal=!!a,o&&(t.session={}),i.eventType=e,function(t,e){var i=t.session,r=e.pointers,s=r.length;i.firstInput||(i.firstInput=K(e));s>1&&!i.firstMultiple?i.firstMultiple=K(e):1===s&&(i.firstMultiple=!1);var o=i.firstInput,a=i.firstMultiple,h=a?a.center:o.center,l=e.center=Q(r);e.timeStamp=c(),e.deltaTime=e.timeStamp-o.timeStamp,e.angle=nt(h,l),e.distance=it(h,l),function(t,e){var i=e.center,n=t.offsetDelta||{},r=t.prevDelta||{},s=t.prevInput||{};e.eventType!==X&&s.eventType!==F||(r=t.prevDelta={x:s.deltaX||0,y:s.deltaY||0},n=t.offsetDelta={x:i.x,y:i.y});e.deltaX=r.x+(i.x-n.x),e.deltaY=r.y+(i.y-n.y)}(i,e),e.offsetDirection=et(e.deltaX,e.deltaY);var p=tt(e.deltaTime,e.deltaX,e.deltaY);e.overallVelocityX=p.x,e.overallVelocityY=p.y,e.overallVelocity=u(p.x)>u(p.y)?p.x:p.y,e.scale=a?(f=a.pointers,d=r,it(d[0],d[1],B)/it(f[0],f[1],B)):1,e.rotation=a?function(t,e){return nt(e[1],e[0],B)+nt(t[1],t[0],B)}(a.pointers,r):0,e.maxPointers=i.prevInput?e.pointers.length>i.prevInput.maxPointers?e.pointers.length:i.prevInput.maxPointers:e.pointers.length,function(t,e){var i,r,s,o,a=t.lastInterval||e,h=e.timeStamp-a.timeStamp;if(e.eventType!=W&&(h>N||a.velocity===n)){var c=e.deltaX-a.deltaX,l=e.deltaY-a.deltaY,p=tt(h,c,l);r=p.x,s=p.y,i=u(p.x)>u(p.y)?p.x:p.y,o=et(c,l),t.lastInterval=e}else i=a.velocity,r=a.velocityX,s=a.velocityY,o=a.direction;e.velocity=i,e.velocityX=r,e.velocityY=s,e.direction=o}(i,e);var f,d;var v=t.element;_(e.srcEvent.target,v)&&(v=e.srcEvent.target);e.target=v}(t,i),t.emit("hammer.input",i),t.recognize(i),t.session.prevInput=i}function K(t){for(var e=[],i=0;i<t.pointers.length;)e[i]={clientX:h(t.pointers[i].clientX),clientY:h(t.pointers[i].clientY)},i++;return{timeStamp:c(),pointers:e,center:Q(e),deltaX:t.deltaX,deltaY:t.deltaY}}function Q(t){var e=t.length;if(1===e)return{x:h(t[0].clientX),y:h(t[0].clientY)};for(var i=0,n=0,r=0;r<e;)i+=t[r].clientX,n+=t[r].clientY,r++;return{x:h(i/e),y:h(n/e)}}function tt(t,e,i){return{x:e/t||0,y:i/t||0}}function et(t,e){return t===e?q:u(t)>=u(e)?t<0?k:H:e<0?L:U}function it(t,e,i){i||(i=Z);var n=e[i[0]]-t[i[0]],r=e[i[1]]-t[i[1]];return Math.sqrt(n*n+r*r)}function nt(t,e,i){i||(i=Z);var n=e[i[0]]-t[i[0]],r=e[i[1]]-t[i[1]];return 180*Math.atan2(r,n)/Math.PI}$.prototype={handler:function(){},init:function(){this.evEl&&I(this.element,this.evEl,this.domHandler),this.evTarget&&I(this.target,this.evTarget,this.domHandler),this.evWin&&I(O(this.element),this.evWin,this.domHandler)},destroy:function(){this.evEl&&A(this.element,this.evEl,this.domHandler),this.evTarget&&A(this.target,this.evTarget,this.domHandler),this.evWin&&A(O(this.element),this.evWin,this.domHandler)}};var rt={mousedown:X,mousemove:Y,mouseup:F},st="mousedown",ot="mousemove mouseup";function at(){this.evEl=st,this.evWin=ot,this.pressed=!1,$.apply(this,arguments)}g(at,$,{handler:function(t){var e=rt[t.type];e&X&&0===t.button&&(this.pressed=!0),e&Y&&1!==t.which&&(e=F),this.pressed&&(e&F&&(this.pressed=!1),this.callback(this.manager,e,{pointers:[t],changedPointers:[t],pointerType:"mouse",srcEvent:t}))}});var ht={pointerdown:X,pointermove:Y,pointerup:F,pointercancel:W,pointerout:W},ut={2:"touch",3:"pen",4:"mouse",5:"kinect"},ct="pointerdown",lt="pointermove pointerup pointercancel";function pt(){this.evEl=ct,this.evWin=lt,$.apply(this,arguments),this.store=this.manager.session.pointerEvents=[]}t.MSPointerEvent&&!t.PointerEvent&&(ct="MSPointerDown",lt="MSPointerMove MSPointerUp MSPointerCancel"),g(pt,$,{handler:function(t){var e=this.store,i=!1,n=t.type.toLowerCase().replace("ms",""),r=ht[n],s=ut[t.pointerType]||t.pointerType,o="touch"==s,a=b(e,t.pointerId,"pointerId");r&X&&(0===t.button||o)?a<0&&(e.push(t),a=e.length-1):r&(F|W)&&(i=!0),a<0||(e[a]=t,this.callback(this.manager,r,{pointers:e,changedPointers:[t],pointerType:s,srcEvent:t}),i&&e.splice(a,1))}});var ft={touchstart:X,touchmove:Y,touchend:F,touchcancel:W},dt="touchstart",vt="touchstart touchmove touchend touchcancel";function mt(){this.evTarget=dt,this.evWin=vt,this.started=!1,$.apply(this,arguments)}g(mt,$,{handler:function(t){var e=ft[t.type];if(e===X&&(this.started=!0),this.started){var i=function(t,e){var i=P(t.touches),n=P(t.changedTouches);e&(F|W)&&(i=D(i.concat(n),"identifier",!0));return[i,n]}.call(this,t,e);e&(F|W)&&i[0].length-i[1].length==0&&(this.started=!1),this.callback(this.manager,e,{pointers:i[0],changedPointers:i[1],pointerType:"touch",srcEvent:t})}}});var gt={touchstart:X,touchmove:Y,touchend:F,touchcancel:W},Tt="touchstart touchmove touchend touchcancel";function yt(){this.evTarget=Tt,this.targetIds={},$.apply(this,arguments)}g(yt,$,{handler:function(t){var e=gt[t.type],i=function(t,e){var i=P(t.touches),n=this.targetIds;if(e&(X|Y)&&1===i.length)return n[i[0].identifier]=!0,[i,i];var r,s,o=P(t.changedTouches),a=[],h=this.target;if(s=i.filter(function(t){return _(t.target,h)}),e===X)for(r=0;r<s.length;)n[s[r].identifier]=!0,r++;r=0;for(;r<o.length;)n[o[r].identifier]&&a.push(o[r]),e&(F|W)&&delete n[o[r].identifier],r++;if(!a.length)return;return[D(s.concat(a),"identifier",!0),a]}.call(this,t,e);i&&this.callback(this.manager,e,{pointers:i[0],changedPointers:i[1],pointerType:"touch",srcEvent:t})}});var Et=2500,It=25;function At(){$.apply(this,arguments);var t=T(this.handler,this);this.touch=new yt(this.manager,t),this.mouse=new at(this.manager,t),this.primaryTouch=null,this.lastTouches=[]}function _t(t){var e=t.changedPointers[0];if(e.identifier===this.primaryTouch){var i={x:e.clientX,y:e.clientY};this.lastTouches.push(i);var n=this.lastTouches;setTimeout(function(){var t=n.indexOf(i);t>-1&&n.splice(t,1)},Et)}}g(At,$,{handler:function(t,e,i){var n="touch"==i.pointerType,r="mouse"==i.pointerType;if(!(r&&i.sourceCapabilities&&i.sourceCapabilities.firesTouchEvents)){if(n)(function(t,e){t&X?(this.primaryTouch=e.changedPointers[0].identifier,_t.call(this,e)):t&(F|W)&&_t.call(this,e)}).call(this,e,i);else if(r&&function(t){for(var e=t.srcEvent.clientX,i=t.srcEvent.clientY,n=0;n<this.lastTouches.length;n++){var r=this.lastTouches[n],s=Math.abs(e-r.x),o=Math.abs(i-r.y);if(s<=It&&o<=It)return!0}return!1}.call(this,i))return;this.callback(t,e,i)}},destroy:function(){this.touch.destroy(),this.mouse.destroy()}});var Ct=x(o.style,"touchAction"),St=Ct!==n,bt="auto",Pt="manipulation",Dt="none",xt="pan-x",wt="pan-y",Ot=function(){if(!St)return!1;var e={},i=t.CSS&&t.CSS.supports;return["auto","manipulation","pan-y","pan-x","pan-x pan-y","none"].forEach(function(n){e[n]=!i||t.CSS.supports("touch-action",n)}),e}();function Rt(t,e){this.manager=t,this.set(e)}Rt.prototype={set:function(t){"compute"==t&&(t=this.compute()),St&&this.manager.element.style&&Ot[t]&&(this.manager.element.style[Ct]=t),this.actions=t.toLowerCase().trim()},update:function(){this.set(this.manager.options.touchAction)},compute:function(){var t=[];return f(this.manager.recognizers,function(e){y(e.options.enable,[e])&&(t=t.concat(e.getTouchAction()))}),function(t){if(C(t,Dt))return Dt;var e=C(t,xt),i=C(t,wt);if(e&&i)return Dt;if(e||i)return e?xt:wt;if(C(t,Pt))return Pt;return bt}(t.join(" "))},preventDefaults:function(t){var e=t.srcEvent,i=t.offsetDirection;if(this.manager.session.prevented)e.preventDefault();else{var n=this.actions,r=C(n,Dt)&&!Ot[Dt],s=C(n,wt)&&!Ot[wt],o=C(n,xt)&&!Ot[xt];if(r){var a=1===t.pointers.length,h=t.distance<2,u=t.deltaTime<250;if(a&&h&&u)return}if(!o||!s)return r||s&&i&V||o&&i&j?this.preventSrc(e):void 0}},preventSrc:function(t){this.manager.session.prevented=!0,t.preventDefault()}};var Mt=1,zt=2,Nt=4,Xt=8,Yt=Xt,Ft=16;function Wt(t){this.options=r({},this.defaults,t||{}),this.id=w++,this.manager=null,this.options.enable=E(this.options.enable,!0),this.state=Mt,this.simultaneous={},this.requireFail=[]}function qt(t){return t&Ft?"cancel":t&Xt?"end":t&Nt?"move":t&zt?"start":""}function kt(t){return t==U?"down":t==L?"up":t==k?"left":t==H?"right":""}function Ht(t,e){var i=e.manager;return i?i.get(t):t}function Lt(){Wt.apply(this,arguments)}function Ut(){Lt.apply(this,arguments),this.pX=null,this.pY=null}function Vt(){Lt.apply(this,arguments)}function jt(){Wt.apply(this,arguments),this._timer=null,this._input=null}function Gt(){Lt.apply(this,arguments)}function Zt(){Lt.apply(this,arguments)}function Bt(){Wt.apply(this,arguments),this.pTime=!1,this.pCenter=!1,this._timer=null,this._input=null,this.count=0}function $t(t,e){return(e=e||{}).recognizers=E(e.recognizers,$t.defaults.preset),new Jt(t,e)}Wt.prototype={defaults:{},set:function(t){return r(this.options,t),this.manager&&this.manager.touchAction.update(),this},recognizeWith:function(t){if(p(t,"recognizeWith",this))return this;var e=this.simultaneous;return e[(t=Ht(t,this)).id]||(e[t.id]=t,t.recognizeWith(this)),this},dropRecognizeWith:function(t){return p(t,"dropRecognizeWith",this)?this:(t=Ht(t,this),delete this.simultaneous[t.id],this)},requireFailure:function(t){if(p(t,"requireFailure",this))return this;var e=this.requireFail;return-1===b(e,t=Ht(t,this))&&(e.push(t),t.requireFailure(this)),this},dropRequireFailure:function(t){if(p(t,"dropRequireFailure",this))return this;t=Ht(t,this);var e=b(this.requireFail,t);return e>-1&&this.requireFail.splice(e,1),this},hasRequireFailures:function(){return this.requireFail.length>0},canRecognizeWith:function(t){return!!this.simultaneous[t.id]},emit:function(t){var e=this,i=this.state;function n(i){e.manager.emit(i,t)}i<Xt&&n(e.options.event+qt(i)),n(e.options.event),t.additionalEvent&&n(t.additionalEvent),i>=Xt&&n(e.options.event+qt(i))},tryEmit:function(t){if(this.canEmit())return this.emit(t);this.state=32},canEmit:function(){for(var t=0;t<this.requireFail.length;){if(!(this.requireFail[t].state&(32|Mt)))return!1;t++}return!0},recognize:function(t){var e=r({},t);if(!y(this.options.enable,[this,e]))return this.reset(),void(this.state=32);this.state&(Yt|Ft|32)&&(this.state=Mt),this.state=this.process(e),this.state&(zt|Nt|Xt|Ft)&&this.tryEmit(e)},process:function(t){},getTouchAction:function(){},reset:function(){}},g(Lt,Wt,{defaults:{pointers:1},attrTest:function(t){var e=this.options.pointers;return 0===e||t.pointers.length===e},process:function(t){var e=this.state,i=t.eventType,n=e&(zt|Nt),r=this.attrTest(t);return n&&(i&W||!r)?e|Ft:n||r?i&F?e|Xt:e&zt?e|Nt:zt:32}}),g(Ut,Lt,{defaults:{event:"pan",threshold:10,pointers:1,direction:G},getTouchAction:function(){var t=this.options.direction,e=[];return t&V&&e.push(wt),t&j&&e.push(xt),e},directionTest:function(t){var e=this.options,i=!0,n=t.distance,r=t.direction,s=t.deltaX,o=t.deltaY;return r&e.direction||(e.direction&V?(r=0===s?q:s<0?k:H,i=s!=this.pX,n=Math.abs(t.deltaX)):(r=0===o?q:o<0?L:U,i=o!=this.pY,n=Math.abs(t.deltaY))),t.direction=r,i&&n>e.threshold&&r&e.direction},attrTest:function(t){return Lt.prototype.attrTest.call(this,t)&&(this.state&zt||!(this.state&zt)&&this.directionTest(t))},emit:function(t){this.pX=t.deltaX,this.pY=t.deltaY;var e=kt(t.direction);e&&(t.additionalEvent=this.options.event+e),this._super.emit.call(this,t)}}),g(Vt,Lt,{defaults:{event:"pinch",threshold:0,pointers:2},getTouchAction:function(){return[Dt]},attrTest:function(t){return this._super.attrTest.call(this,t)&&(Math.abs(t.scale-1)>this.options.threshold||this.state&zt)},emit:function(t){if(1!==t.scale){var e=t.scale<1?"in":"out";t.additionalEvent=this.options.event+e}this._super.emit.call(this,t)}}),g(jt,Wt,{defaults:{event:"press",pointers:1,time:251,threshold:9},getTouchAction:function(){return[bt]},process:function(t){var e=this.options,i=t.pointers.length===e.pointers,n=t.distance<e.threshold,r=t.deltaTime>e.time;if(this._input=t,!n||!i||t.eventType&(F|W)&&!r)this.reset();else if(t.eventType&X)this.reset(),this._timer=l(function(){this.state=Yt,this.tryEmit()},e.time,this);else if(t.eventType&F)return Yt;return 32},reset:function(){clearTimeout(this._timer)},emit:function(t){this.state===Yt&&(t&&t.eventType&F?this.manager.emit(this.options.event+"up",t):(this._input.timeStamp=c(),this.manager.emit(this.options.event,this._input)))}}),g(Gt,Lt,{defaults:{event:"rotate",threshold:0,pointers:2},getTouchAction:function(){return[Dt]},attrTest:function(t){return this._super.attrTest.call(this,t)&&(Math.abs(t.rotation)>this.options.threshold||this.state&zt)}}),g(Zt,Lt,{defaults:{event:"swipe",threshold:10,velocity:.3,direction:V|j,pointers:1},getTouchAction:function(){return Ut.prototype.getTouchAction.call(this)},attrTest:function(t){var e,i=this.options.direction;return i&(V|j)?e=t.overallVelocity:i&V?e=t.overallVelocityX:i&j&&(e=t.overallVelocityY),this._super.attrTest.call(this,t)&&i&t.offsetDirection&&t.distance>this.options.threshold&&t.maxPointers==this.options.pointers&&u(e)>this.options.velocity&&t.eventType&F},emit:function(t){var e=kt(t.offsetDirection);e&&this.manager.emit(this.options.event+e,t),this.manager.emit(this.options.event,t)}}),g(Bt,Wt,{defaults:{event:"tap",pointers:1,taps:1,interval:300,time:250,threshold:9,posThreshold:10},getTouchAction:function(){return[Pt]},process:function(t){var e=this.options,i=t.pointers.length===e.pointers,n=t.distance<e.threshold,r=t.deltaTime<e.time;if(this.reset(),t.eventType&X&&0===this.count)return this.failTimeout();if(n&&r&&i){if(t.eventType!=F)return this.failTimeout();var s=!this.pTime||t.timeStamp-this.pTime<e.interval,o=!this.pCenter||it(this.pCenter,t.center)<e.posThreshold;if(this.pTime=t.timeStamp,this.pCenter=t.center,o&&s?this.count+=1:this.count=1,this._input=t,0===this.count%e.taps)return this.hasRequireFailures()?(this._timer=l(function(){this.state=Yt,this.tryEmit()},e.interval,this),zt):Yt}return 32},failTimeout:function(){return this._timer=l(function(){this.state=32},this.options.interval,this),32},reset:function(){clearTimeout(this._timer)},emit:function(){this.state==Yt&&(this._input.tapCount=this.count,this.manager.emit(this.options.event,this._input))}}),$t.VERSION="2.0.7",$t.defaults={domEvents:!1,touchAction:"compute",enable:!0,inputTarget:null,inputClass:null,preset:[[Gt,{enable:!1}],[Vt,{enable:!1},["rotate"]],[Zt,{direction:V}],[Ut,{direction:V},["swipe"]],[Bt],[Bt,{event:"doubletap",taps:2},["tap"]],[jt]],cssProps:{userSelect:"none",touchSelect:"none",touchCallout:"none",contentZooming:"none",userDrag:"none",tapHighlightColor:"rgba(0,0,0,0)"}};function Jt(t,e){var i;this.options=r({},$t.defaults,e||{}),this.options.inputTarget=this.options.inputTarget||t,this.handlers={},this.session={},this.recognizers=[],this.oldCssProps={},this.element=t,this.input=new((i=this).options.inputClass||(M?pt:z?yt:R?At:at))(i,J),this.touchAction=new Rt(this,this.options.touchAction),Kt(this,!0),f(this.options.recognizers,function(t){var e=this.add(new t[0](t[1]));t[2]&&e.recognizeWith(t[2]),t[3]&&e.requireFailure(t[3])},this)}function Kt(t,e){var i,n=t.element;n.style&&(f(t.options.cssProps,function(r,s){i=x(n.style,s),e?(t.oldCssProps[i]=n.style[i],n.style[i]=r):n.style[i]=t.oldCssProps[i]||""}),e||(t.oldCssProps={}))}Jt.prototype={set:function(t){return r(this.options,t),t.touchAction&&this.touchAction.update(),t.inputTarget&&(this.input.destroy(),this.input.target=t.inputTarget,this.input.init()),this},stop:function(t){this.session.stopped=t?2:1},recognize:function(t){var e=this.session;if(!e.stopped){var i;this.touchAction.preventDefaults(t);var n=this.recognizers,r=e.curRecognizer;(!r||r&&r.state&Yt)&&(r=e.curRecognizer=null);for(var s=0;s<n.length;)i=n[s],2===e.stopped||r&&i!=r&&!i.canRecognizeWith(r)?i.reset():i.recognize(t),!r&&i.state&(zt|Nt|Xt)&&(r=e.curRecognizer=i),s++}},get:function(t){if(t instanceof Wt)return t;for(var e=this.recognizers,i=0;i<e.length;i++)if(e[i].options.event==t)return e[i];return null},add:function(t){if(p(t,"add",this))return this;var e=this.get(t.options.event);return e&&this.remove(e),this.recognizers.push(t),t.manager=this,this.touchAction.update(),t},remove:function(t){if(p(t,"remove",this))return this;if(t=this.get(t)){var e=this.recognizers,i=b(e,t);-1!==i&&(e.splice(i,1),this.touchAction.update())}return this},on:function(t,e){if(t!==n&&e!==n){var i=this.handlers;return f(S(t),function(t){i[t]=i[t]||[],i[t].push(e)}),this}},off:function(t,e){if(t!==n){var i=this.handlers;return f(S(t),function(t){e?i[t]&&i[t].splice(b(i[t],e),1):delete i[t]}),this}},emit:function(t,i){this.options.domEvents&&function(t,i){var n=e.createEvent("Event");n.initEvent(t,!0,!0),n.gesture=i,i.target.dispatchEvent(n)}(t,i);var n=this.handlers[t]&&this.handlers[t].slice();if(n&&n.length){i.type=t,i.preventDefault=function(){i.srcEvent.preventDefault()};for(var r=0;r<n.length;)n[r](i),r++}},destroy:function(){this.element&&Kt(this,!1),this.handlers={},this.session={},this.input.destroy(),this.element=null}},r($t,{INPUT_START:X,INPUT_MOVE:Y,INPUT_END:F,INPUT_CANCEL:W,STATE_POSSIBLE:Mt,STATE_BEGAN:zt,STATE_CHANGED:Nt,STATE_ENDED:Xt,STATE_RECOGNIZED:Yt,STATE_CANCELLED:Ft,STATE_FAILED:32,DIRECTION_NONE:q,DIRECTION_LEFT:k,DIRECTION_RIGHT:H,DIRECTION_UP:L,DIRECTION_DOWN:U,DIRECTION_HORIZONTAL:V,DIRECTION_VERTICAL:j,DIRECTION_ALL:G,Manager:Jt,Input:$,TouchAction:Rt,TouchInput:yt,MouseInput:at,PointerEventInput:pt,TouchMouseInput:At,SingleTouchInput:mt,Recognizer:Wt,AttrRecognizer:Lt,Tap:Bt,Pan:Ut,Swipe:Zt,Pinch:Vt,Rotate:Gt,Press:jt,on:I,off:A,each:f,merge:m,extend:v,assign:r,inherit:g,bindFn:T,prefixed:x}),(void 0!==t?t:"undefined"!=typeof self?self:{}).Hammer=$t,"function"==typeof define&&define.amd?define(function(){return $t}):"undefined"!=typeof module&&module.exports?module.exports=$t:t.Hammer=$t}(window,document);