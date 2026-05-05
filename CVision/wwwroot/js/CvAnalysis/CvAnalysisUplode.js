const input = document.getElementById('cvFile');
const display = document.getElementById('fileName');
const form = document.querySelector('.upload-cv-input');
const loader = document.getElementById('analysisLoader');
const uploadBlock = document.querySelector('.upload-cv-block');

input.addEventListener('change', () => {
    if (input.files.length) {
        display.textContent = input.files[0].name;
        display.style.display = 'inline-block';
    }
});

form.addEventListener('submit', function (e) {
    if (input.files.length > 0) {
        document.querySelector('h1').classList.add('hidden');
        form.classList.add('hidden');
        document.querySelector('.disclaimer-content').classList.add('hidden');
        loader.style.display = 'block';
    }
});