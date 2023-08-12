﻿const sckt = new WebSocket(`${scktURl}Player/Connect`);
const toMoment = (value) => moment(value).format('mm:ss');
const slider = new Slider('#time', {
    formatter: function (value) {
        try {
            return toMoment(state.progressMilisseconds);
        } catch (e) {
            return '';
        }
    }
});
var state, playerList, showing, account;

$(document).ready(async function () {
    sckt.onmessage = scktMessage;
    await setActual();
    setUpdateTimer();
});

function scktMessage(obj) {
    let state = JSON.parse(obj.data)
    setActual(state);
}

async function setActual() {
    state = await $.get(`${apiUrl}Player/Actual`);
    let player = $('.player');
    let msc = state.track;

    player.find('#name')
        .text(msc.name);

    player.find('#img')
        .attr('src', msc.album.images[0].url);

    addArtists(msc.artists, player.find('#artists'));

    player.find('#total')
        .text(moment(msc.duration).format('mm:ss'));

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
    li.attr('data-spotify', msc.id);
    li.click(musicPlayClick);

    let banner = $('<div class="banner">')
    let img = $('<img class="banner">');
    img.attr('src', msc.album.images[1].url);
    img.click(musicListClick);

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
    msc.preview = new Audio(msc.previewURL);
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
    let target = $(event.target)
        .parent()
        .parent()
        .data('spotify');

    if (target != undefined) {
        window.open(`https://open.spotify.com/track/${target}`, '_blank');
    }
}

async function musicPlayClick(event) {
    let target = $(event.target)
        .parent()
        .parent()
        .data('spotify');

    var msc = $.grep(playerList, function (music) {
        return music.id === target;
    })[0];

    if (msc === undefined) {
        return;
    }

    if (showing != undefined && !showing.paused) {
        await showing.pause()
    }

    var prv = $('.preview')

    prv.removeClass('hide');

    prv.find('.name')
        .text(msc.name)

    prv.find('.spotify')
        .attr('href', msc.urls.spotify)

    prv.find('img')
        .attr('src', msc.album.images[1].url);

    addArtists(msc.artists, prv.find('#artists'));

    showing = new Audio(msc.previewUrl);
    showing.onended = function EndMusic() {
        $('.preview')
            .addClass('hide');
    }
    await showing.play();
}

function setUpdateTimer() {
    state.progressMilisseconds = state.progressMilisseconds + 100;
    slider.setValue((state.progressMilisseconds / state.track.duration) * 100);
    $('#actual')
        .text(toMoment(state.progressMilisseconds));
    setTimeout(setUpdateTimer, 100);
}