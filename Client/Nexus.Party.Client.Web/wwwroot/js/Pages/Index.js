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
    let li = $('<li class="music-list">');
    li.attr('data-spotify', msc.urls.spotify);
    li.click(musicListClick);

    let banner = $('<div class="banner">')
    let img = $('<img class="banner">');
    img.attr('src', msc.album.images[1].url);

    banner.append('<i class="fa-solid fa-pause"></i>');
    banner.append('<i class="fa-solid fa-play "></i>');
    banner.append(img);

    li.append(banner);

    let text = $('<div class="text">')

    let name = $('<h5>')
    name.text(msc.name);
    text.append(name);

    let artist = $('<ul id="artists">')
    addArtists(msc.artists, artist);
    text.append(artist);

    li.append(text);
    return li;
}

function addArtists(artists, list) {
    list.empty();

    for (var i = 0; i < artists.length; i++) {
        var artist = $('<li class="artist">');

        artist.text(artists[i].name);

        list.append(artist);
    }
}

function musicListClick(event) {
    let target = $(event.target).data('spotify');

    if (target != undefined) {
        window.open(target, '_blank');
    }
}