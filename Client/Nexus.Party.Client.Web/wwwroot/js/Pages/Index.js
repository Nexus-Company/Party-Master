const sckt = new WebSocket(`${scktURl}Player/Connect`);
var actual;
$(document).ready(async function () {
    sckt.onmessage = scktMessage;
    setActual(await $.get(`${apiUrl}Player/Actual`));
});

function scktMessage(obj) {
    let state = JSON.parse(obj.data)
    setActual(state);
}

function setActual(msc) {
    $('.player #name').text(msc.name);
    $('.player #img').attr('src', msc.album.images[0].url)
    actual = msc;
    console.log(msc);
}

