
function TreeSelected(p, e, t) {
    $PopOff();
    if (e.which === 1 || e.button === 0) {
        var data = [
            { key: 'o', vlu: t.id },
            { key: 'g', vlu: t.getAttribute('data-tag') }
        ];
        var elms = document.querySelectorAll('input[data-name="XysFormEV"]');
        if (elms != null) {
            for (var i = 0; i < elms.length; i++) {
                elms[i].style.borderColor = '#d0d0d0';
            }
        }

        $WaitOn();
        $ApiRequest(p + '/TreeClicked', JSON.stringify(data));
    }
    if (e.which === 3 || e.button === 2) {
        if (t == null) { return; }
        var data = [
           { key: 'o', vlu: t.id },
           { key: 'g', vlu: t.getAttribute('data-tag') },
           { key: 'y', vlu: e.clientY },
           { key: 'x', vlu: e.clientX }
        ];
        $WaitOn();
        $ApiRequest(p + '/TreeSelected', JSON.stringify(data));
    }
}

function AddSectionPop(p, t) {
    var data = [
       { key: 'o', vlu: t }
    ];
    $WaitOn();
    $ApiRequest(p + '/AddSectionPop', JSON.stringify(data));
}
function AddSection(p, t) {
    var sn = document.getElementById('sectionname');
    var sl = document.getElementById('sectionlabel');

    var data = [
       { key: 'o', vlu: t },
       { key: 'n', vlu: sn.value },
       { key: 'm', vlu: sl.value }
    ];
    $PopOff();
    $WaitOn();
    $ApiRequest(p + '/AddSection', JSON.stringify(data));
}
function AddElementPop(p, t) {
    var data = [
       { key: 'o', vlu: t }
    ];
    $WaitOn();
    $ApiRequest(p + '/AddElementPop', JSON.stringify(data));
}
function AddElement(p, t) {
    var en = document.getElementById('elementname');
    var el = document.getElementById('elementlabel');

    var data = [
       { key: 'o', vlu: t },
       { key: 'n', vlu: en.value },
       { key: 'm', vlu: el.value }
    ];
    $PopOff();
    $WaitOn();
    $ApiRequest(p + '/AddElement', JSON.stringify(data));
}

function allowDrop(ev) {
    ev.preventDefault();
}
function drag(ev) {
    var me = document.getElementById(ev.target.id);
    if (me == null) { return; }
    var mp = me.getAttribute('data-type');
    var mps = document.querySelectorAll("[data-type='" + mp + "']");
    for (var i = 0; i < mps.length; i++) {
        mps[i].setAttribute('ondragover', 'allowDrop(event)')
    }
    ev.dataTransfer.setData("text", ev.target.id);
}
function drop(ev) {
    ev.preventDefault();
    if (ev == null) { return; }
    if (ev.target != null) {
        var p = ev.target.getAttribute('data-name');
        var srcid = ev.dataTransfer.getData("text");
        var mp = ev.target.getAttribute('data-type');
        var mps = document.querySelectorAll("[data-type='" + mp + "']");
        for (var i = 0; i < mps.length; i++) {
            mps[i].removeAttribute('ondragover')
        }
        var data = [
            { key: 's', vlu: srcid },
            { key: 't', vlu: ev.target.id }
        ];
        $ApiRequest(p + '/SwitchElements', JSON.stringify(data));
    }
}

function MoveUpSection(p, t) {
    var data = [
       { key: 'o', vlu: t }
    ];
    $PopOff();
    $WaitOn();
    $ApiRequest(p + '/MoveUpSection', JSON.stringify(data));
}
function MoveDownSection(p, t) {
    var data = [
       { key: 'o', vlu: t }
    ];
    $PopOff();
    $WaitOn();
    $ApiRequest(p + '/MoveDownSection', JSON.stringify(data));
}
function DeleteSection(p, t) {
    var data = [
       { key: 'o', vlu: t }
    ];
    $PopOff();
    $WaitOn();
    $ApiRequest(p + '/DeleteSection', JSON.stringify(data));
}

function MoveUpElement(p, t) {
    var data = [
       { key: 'o', vlu: t }
    ];
    $PopOff();
    $WaitOn();
    $ApiRequest(p + '/MoveUpElement', JSON.stringify(data));
}
function MoveDownElement(p, t) {
    var data = [
       { key: 'o', vlu: t }
    ];
    $PopOff();
    $WaitOn();
    $ApiRequest(p + '/MoveDownElement', JSON.stringify(data));
}
function DeleteElement(p, t) {
    var data = [
       { key: 'o', vlu: t }
    ];
    $PopOff();
    $WaitOn();
    $ApiRequest(p + '/DeleteElement', JSON.stringify(data));
}

function SaveProperty(p, g, m, c, t) {
    var v;
    switch (t.type) {
        case 'checkbox':
            if (t.checked == true) {
                v = '1';
            } else {
                v = '0';
            }
            break;
        default:
            v = t.value;
            break;
    }
    var data = [
       { key: 'g', vlu: g },
       { key: 'm', vlu: m },
       { key: 'c', vlu: c },
       { key: 'v', vlu: v }
    ];
    $ApiRequest(p + '/SaveProperty', JSON.stringify(data));
}
function ViewCode(p) {
    $ApiRequest(p);
}
function BackDesign(p) {
    $ApiRequest(p);
}

function CopyFormConfirm(p) {
    $WaitOn();
    $ApiRequest(p + '/CopyFormConfirm', this);
}