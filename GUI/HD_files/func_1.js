// JS OBFUSCATED TO AVOID COPY

jQuery(function(_0xce83x1) {
    'use strict'; 
    _0xce83x1('#wizard_container')['wizard']({
        stepsWrapper: '#wrapped',
        submit: '.submit',
        unidirectional: false,
        beforeSelect: function(_0xce83x4, _0xce83x5) {
            if (_0xce83x1('input#website')['val']()['length'] != 0) {
                return false
            };
            if (!_0xce83x5['isMovingForward']) {
                return true
            };
            var _0xce83x6 = _0xce83x1(this)['wizard']('state')['step']['find'](':input');
            return !_0xce83x6['length'] || !!_0xce83x6['valid']()
        }
    })['validate']({
        errorPlacement: function(_0xce83x2, _0xce83x3) {
            if (_0xce83x3['is'](':radio') || _0xce83x3['is'](':checkbox')) {
                _0xce83x2['insertBefore'](_0xce83x3['next']())
            } else {
                _0xce83x2['insertAfter'](_0xce83x3)
            }
        }
    });
    _0xce83x1('#progressbar')['progressbar']();
    _0xce83x1('#wizard_container')['wizard']({
        afterSelect: function(_0xce83x4, _0xce83x5) {
            _0xce83x1('#progressbar')['progressbar']('value', _0xce83x5['percentComplete']);
            _0xce83x1('#location')['text']('' + _0xce83x5['stepsComplete'] + ' of ' + _0xce83x5['stepsPossible'] + ' completed')
        }
    })
});
$('#wizard_container')['wizard']({
    transitions: {
        branchtype: function(_0xce83x7, _0xce83x8) {
            var _0xce83x9 = _0xce83x7['find'](':checked')['val']();
            if (!_0xce83x9) {
                $('form')['valid']()
            };
            return _0xce83x9
        }
    }
});

function getVals(_0xce83xb, _0xce83xc) {
    switch (_0xce83xc) {
        case 'name_field':
            var _0xce83xd = $(_0xce83xb)['val']();
            $('#name_field')['text'](_0xce83xd);
            break;
        case 'email_field':
            var _0xce83xd = $(_0xce83xb)['val']();
            $('#email_field')['text'](_0xce83xd);
            break
    }
}