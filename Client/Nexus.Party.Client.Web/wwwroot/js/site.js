// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const scktURl = 'wss://localhost:44383/api/'
    , apiUrl = 'https://localhost:44383/api/';

var prefersDarkMode = window.matchMedia('(prefers-color-scheme: dark)');

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
