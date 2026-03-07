function ReloadTopDept(p, d, t ) {
    $WaitOn();
    var data = [
        { key: 'd', vlu: document.getElementById(d).value },
        { key: 't', vlu: t }
    ];
    $ApiRequest(p, JSON.stringify(data));
}
function ClearTopDept() {
    var topdept = document.getElementById('TOPDEPT');
    topdept.innerHTML = '';
}
function DEPTDetail(p, t) {
    $WaitOn();
    var data = [
        { key: 't', vlu: t }
    ];
    $ApiRequest(p, JSON.stringify(data));
}
function PopAddRecord(p,t) {
    $WaitOn();
    var data = [
        { key: 't', vlu: t }
    ];
    $ApiRequest(p, JSON.stringify(data));
}
function PopEditRecord(p, t) {
    $WaitOn();
    var data = [
        { key: 't', vlu: t }
    ];
    $ApiRequest(p, JSON.stringify(data));
}
function RefreshTableRow(t, d) {
    var tableCell = document.getElementById(t);
    if (tableCell) {
        var tableRow = tableCell.parentNode;
        var cellCount = tableRow.cells.length;
        var dt = d.split('|')
        for (var i = 0; i < cellCount; i++) {
            switch (i) {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    tableRow.cells[i].innerText = dt[i];
                    break;
                default:
                    break;
            }
        }
            console.log(tableCell.innerText);
    }
}
function TreeSelected(p, e, t) {
    $PopOff();
    if (e.which === 1 || e.button === 0) {
        var data = [
           { key: 'o', vlu: t.id }
        ];
        $WaitOn();
        $ApiRequest(p + '/TreeClicked', JSON.stringify(data));
    }
    if (e.which === 3 || e.button === 2) {
        if (t == null) { return; }
        var data = [
           { key: 'o', vlu: t.id },
           { key: 'y', vlu: e.clientY },
           { key: 'x', vlu: e.clientX }
        ];
        $WaitOn();
        $ApiRequest(p + '/TreeSelected', JSON.stringify(data));
    }
}