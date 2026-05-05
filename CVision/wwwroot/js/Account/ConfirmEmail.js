document.getElementById('confirmOverlay').addEventListener('click', function (e) {
    if (e.target === this) {
        window.location.href = '/';
    }
});