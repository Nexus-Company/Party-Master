// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const apiEndPoint = 'localhost:44383/',
    scktURl = `wss://${apiEndPoint}api/`,
    apiUrl = `https://${apiEndPoint}api/`;

var prefersDarkMode = window.matchMedia('(prefers-color-scheme: dark)');
$(document).ready(async function () {
    await setAccount();
});

function setTheme(darkMode) {
    var body = $(document.body);

    if (darkMode) {
        body.attr('data-bs-theme', 'dark');
    } else {
        body.attr('data-bs-theme', 'light');
    }
}

setTheme(prefersDarkMode.matches);

prefersDarkMode.addEventListener('change', function (e) {
    setTheme(e.matches);
});

function redirectToLogin() {
    window.location = `https://${apiEndPoint}google/authorize`
}

function redirectToLogout() {
    window.location = `${apiUrl}Authentication/Logout`
}

async function setAccount() {
    try {
        account = await $.ajax({
            url: `${apiUrl}Accounts/Me`,
            xhrFields: {
                withCredentials: true
            }
        });

        if (account == undefined) {
            return;
        }

        let panel = $('.user-panel');

        panel.removeClass('invisible')

        panel.find('img')
            .attr('src', account.pictureUrl);

        panel.find('#shortName')
            .text(account.name);

        $('#loginPanel')
            .addClass('invisible')
    } catch (e) {
        console.log(e);
    }
}

function showToast(body = 'Hello, world! This is a toast message.', theme = 'danger') {
    var toastElement = $('<div class="toast align-items-center border-0" role="alert" aria-live="assertive" aria-atomic="true">' +
        '<div class="d-flex">' +
        '<div class="toast-body">' +
        '</div>' +
        '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>' +
        '</div>' +
        '</div>');

    toastElement
        .find('.toast-body')
        .append(body);

    toastElement.addClass('text-bg-' + theme);
    toastElement.on('hidden.bs.toast', function () {
        // Remove o elemento HTML do DOM
        toastElement.remove();
    });

    $('.toast-container').append(toastElement);

    var toast = new bootstrap.Toast(toastElement);

    // Show the toast
    toast.show();
}

function showLoadingModal() {

}