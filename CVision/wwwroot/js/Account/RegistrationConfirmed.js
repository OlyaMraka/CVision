document.getElementById('registrationConfirmedOverlay').addEventListener('click', function (e) {
    if (e.target === this) {
        window.location.href = '/';
    }
});