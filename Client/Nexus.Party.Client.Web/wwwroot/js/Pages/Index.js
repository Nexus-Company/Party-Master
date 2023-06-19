const sckt = new WebSocket(`${scktURl}Player/Connect`);
var actual, playerlist;

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
    $('.player #img').attr('src', msc.album.images[0].url);

    addArtists(msc.artists, $('.player #artists'));

    actual = msc;
    updatePlayerList();
}

async function updatePlayerList() {
    playerList = await $.get(`${apiUrl}Player/Queue`);
    var list = $('#nextList');

    list.empty();

    for (var i = 0; i < playerList.length; i++) {
        list.append(musicItem(playerList[i]));
    }
}

function musicItem(msc) {
    let ul = $('<li class="music-list card">');
    let banner = $('<img class="banner">');
    banner.attr('src', msc.album.images[0].url);
    ul.append(banner);

    let text = $('<div class="text">')

    let name = $('<h5>')
    name.text(msc.name);
    text.append(name);

    let artist = $('<ul id="artists">')
    addArtists(msc.artists, artist);
    text.append(artist);

    ul.append(text);
    return ul;
}


function addArtists(artists, list) {
    list.empty();

    for (var i = 0; i < artists.length; i++) {
        var artist = $('<li class="artist">');

        artist.text(artists[i].name);

        list.append(artist);
    }
}