window.addEventListener("load", (event) => {
    $ClickListener.push($ClsPop);
    $ResizeListener.push($ClsPop);
});

function txtblur(event, obj) {
    if (obj.value == '') {
        obj.style.border = '2px solid #E4702F';
    } else {
        obj.style.border = '';
    }
}
function Profile() {
    $WaitOn();
    $ApiRequest();
}
function SignOut() {
    $WaitOn();
    $ApiRequest();
}
function UpdatePhoto(f, s,  p) {
    var data = [
        { key: 'f', vlu: f },
        { key: 's', vlu: s },
        { key: 'p', vlu: p }
    ];
    $WaitOn();
    $ApiRequest('WebBase/UpdatePhoto', JSON.stringify(data));
}
function FileDownLoad(t) {
    var data = [{ key: 'FileId', vlu: t }];
    $WaitOn();
    $ApiRequest(this, JSON.stringify(data));
}
 