function EMPSelected(t, elm) {
    var empid = document.getElementById(elm);
    if (empid != null) {
        if(t.value ==''){
            empid.value = '';
        } else {
            const regex = /\[(.*?)\]/;
            const match = t.value.match(regex);
            empid.value = match[1];
            t.value = t.value.split('[')[0]
        }
    }
}