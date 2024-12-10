// JS OBFUSCATED TO AVOID COPY

(function(_0xfb19x1) {
    'use strict';
    _0xfb19x1(window)['on']('load', function() {
        _0xfb19x1('[data-loader="circle-side"]')['fadeOut']();
        _0xfb19x1('#preloader')['delay'](350)['fadeOut']('slow');
        _0xfb19x1('body')['delay'](350)['css']({
            '\x6F\x76\x65\x72\x66\x6C\x6F\x77': 'visible'
        })
    });
    _0xfb19x1('form#wrapped')['on']('submit', function() {
        var _0xfb19x2 = _0xfb19x1('form#wrapped');
        _0xfb19x2['validate']();
        if (_0xfb19x2['valid']()) {
            _0xfb19x1('#loader_form')['fadeIn']()
        }
    });
    _0xfb19x1('.container_radio.version_2 input[type="radio"], .container_check.version_2 input[type="checkbox"]')['click'](function() {
        _0xfb19x1(this)['parent']()['addClass']('active')['siblings']('label')['removeClass']('active')
    });
    _0xfb19x1('a[href^="#"].mobile_btn')['on']('click', function(_0xfb19x3) {
        _0xfb19x3['preventDefault']();
        var _0xfb19x4 = this['hash'];
        var _0xfb19x5 = _0xfb19x1(_0xfb19x4);
        _0xfb19x1('html, body')['stop']()['animate']({
            '\x73\x63\x72\x6F\x6C\x6C\x54\x6F\x70': _0xfb19x5['offset']()['top']
        }, 400, 'swing', function() {
            window['location']['hash'] = _0xfb19x4
        })
    });
    var _0xfb19x6 = new FloatLabels('form', {
        style: 1
    });

    function _0xfb19x7(_0xfb19x3) {
        _0xfb19x1(_0xfb19x3['target'])['prev']('.card-header')['find']('i.indicator')['toggleClass']('ti-minus ti-plus')
    }
    _0xfb19x1('.accordion_2')['on']('hidden.bs.collapse shown.bs.collapse', _0xfb19x7);

    function _0xfb19x8(_0xfb19x3) {
        _0xfb19x1(_0xfb19x3['target'])['prev']('.panel-heading')['find']('.indicator')['toggleClass']('ti-minus ti-plus')
    }
    _0xfb19x1('#faq_box a[href^="#"]')['on']('click', function() {
        if (location['pathname']['replace'](/^\//, '') == this['pathname']['replace'](/^\//, '') || location['hostname'] == this['hostname']) {
            var _0xfb19x4 = _0xfb19x1(this['hash']);
            _0xfb19x4 = _0xfb19x4['length'] ? _0xfb19x4 : _0xfb19x1('[name=' + this['hash']['slice'](1) + ']');
            if (_0xfb19x4['length']) {
                _0xfb19x1('html,body')['animate']({
                    scrollTop: _0xfb19x4['offset']()['top'] - 195
                }, 800);
                return false
            }
        }
    });
    _0xfb19x1('ul#cat_nav li a')['on']('click', function() {
        _0xfb19x1('ul#cat_nav li a.active')['removeClass']('active');
        _0xfb19x1(this)['addClass']('active')
    });
    var _0xfb19x9 = _0xfb19x1('.cd-overlay-nav'),
        _0xfb19xa = _0xfb19x1('.cd-overlay-content'),
        _0xfb19xb = _0xfb19x1('.cd-primary-nav'),
        _0xfb19xc = _0xfb19x1('.cd-nav-trigger');
    _0xfb19xd();
    _0xfb19x1(window)['on']('resize', function() {
        window['requestAnimationFrame'](_0xfb19xd)
    });
    _0xfb19xc['on']('click', function() {
        if (!_0xfb19xc['hasClass']('close-nav')) {
            _0xfb19xc['addClass']('close-nav');
            _0xfb19x9['children']('span')['velocity']({
                translateZ: 0,
                scaleX: 1,
                scaleY: 1
            }, 500, 'easeInCubic', function() {
                _0xfb19xb['addClass']('fade-in')
            })
        } else {
            _0xfb19xc['removeClass']('close-nav');
            _0xfb19xa['children']('span')['velocity']({
                translateZ: 0,
                scaleX: 1,
                scaleY: 1
            }, 500, 'easeInCubic', function() {
                _0xfb19xb['removeClass']('fade-in');
                _0xfb19x9['children']('span')['velocity']({
                    translateZ: 0,
                    scaleX: 0,
                    scaleY: 0
                }, 0);
                _0xfb19xa['addClass']('is-hidden')['one']('webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend', function() {
                    _0xfb19xa['children']('span')['velocity']({
                        translateZ: 0,
                        scaleX: 0,
                        scaleY: 0
                    }, 0, function() {
                        _0xfb19xa['removeClass']('is-hidden')
                    })
                });
                if (_0xfb19x1('html')['hasClass']('no-csstransitions')) {
                    _0xfb19xa['children']('span')['velocity']({
                        translateZ: 0,
                        scaleX: 0,
                        scaleY: 0
                    }, 0, function() {
                        _0xfb19xa['removeClass']('is-hidden')
                    })
                }
            })
        }
    });

    function _0xfb19xd() {
        var _0xfb19xe = (Math['sqrt'](Math['pow'](_0xfb19x1(window)['height'](), 2) + Math['pow'](_0xfb19x1(window)['width'](), 2)) * 2);
        _0xfb19x9['children']('span')['velocity']({
            scaleX: 0,
            scaleY: 0,
            translateZ: 0
        }, 50)['velocity']({
            height: _0xfb19xe + 'px',
            width: _0xfb19xe + 'px',
            top: -(_0xfb19xe / 2) + 'px',
            left: -(_0xfb19xe / 2) + 'px'
        }, 0);
        _0xfb19xa['children']('span')['velocity']({
            scaleX: 0,
            scaleY: 0,
            translateZ: 0
        }, 50)['velocity']({
            height: _0xfb19xe + 'px',
            width: _0xfb19xe + 'px',
            top: -(_0xfb19xe / 2) + 'px',
            left: -(_0xfb19xe / 2) + 'px'
        }, 0)
    }
})(window['jQuery'])