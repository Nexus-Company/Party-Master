const playerSckt = new WebSocket(`${scktURl}Player/Connect`),
    interactSckt = new WebSocket(`${scktURl}Interact/Connect`),
    modalSearch = $('#modalSearch'),
    toMoment = (value) => moment(value).format('mm:ss')
slider = new Slider('#time', {
    formatter: function (value) {
        try {
            return toMoment(state.progressMilisseconds);
        } catch (e) {
            return '';
        }
    }
});
var state, last, playerList, showing, account;

var actual, playerList, showing, account;

$(document).ready(async function () {
    playerSckt.onmessage = playerScktMessage;
    await setActual();

    if (account != undefined) {
        interactSckt.onmessage = interactScktMessage;
    }

    $('#schKey')
        .on('keyup', searchKeyUp)
    $(".commands .svg-inline--fa").on("mouseover", function () {
        $(this).addClass("active");
        console.log('in');
    });
    $(".commands .svg-inline--fa").on('mouseout', function () {
        $(this).removeClass("active");
        console.log('out');
    });
    setUpdateTimer();
});

function interactScktMessage(obj) {
    let state = JSON.parse(obj.data);

    console.log(state);
}

function playerScktMessage(obj) {
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
    let li = $('<li class="music">');
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

    if (last == undefined)
        last = Date.now();
    
    state.progressMilisseconds = state.progressMilisseconds + (Date.now() - last);
    last = Date.now();
    slider.setValue((state.progressMilisseconds / state.track.duration) * 100);
    $('#actual')
        .text(toMoment(state.progressMilisseconds));

    setTimeout(setUpdateTimer, 100);
}

async function voteSkip() {
    await $.ajax({
        type: 'POST',
        url: `${apiUrl}Interact/Vote/Skip`,
        xhrFields: {
            withCredentials: true
        }
    });
}

async function searchKeyUp(obj) {
    let query = $(obj.target).val();
    var searchRst = await $.ajax({
        type: 'GET',
        url: `${apiUrl}Search?q=${encodeURIComponent(query)}`,
        xhrFields: {
            withCredentials: true
        }
    });

    var list = $('#schResult');

    list.empty();

    for (var i = 0; i < searchRst.length; i++) {
        let item = musicItem(searchRst[i]);
        let add = $('<div class="Add"><i class="fa-solid fa-plus" /></>');
        item.prepend(add);
        list.append(item);
        add.on('click', voteAdd)
    }
}

async function voteAdd(event) {
    let id = $(event.target)
        .parent()
        .parent()
        .data('spotify');

    modalSearch
        .modal('hide');

    await $.ajax({
        type: 'POST',
        url: `${apiUrl}Interact/Vote/Add?trackId=${id}`,
        xhrFields: {
            withCredentials: true
        }
    });
}