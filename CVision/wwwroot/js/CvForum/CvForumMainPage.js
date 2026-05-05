const path = window.location.pathname.toLowerCase();
const buttons = document.querySelectorAll('.publications-buttons a');

buttons.forEach(btn => {
    if (btn.href.toLowerCase().includes(path)) {
        btn.classList.add('active');
    }
});