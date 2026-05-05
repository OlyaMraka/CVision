const input = document.getElementById('cvFile');
const display = document.getElementById('fileName');
const form = document.querySelector('.upload-cv-input');
const uploadBlock = document.querySelector('.upload-cv-block');

input.addEventListener('change', () => {
    if (input.files.length) {
        display.textContent = input.files[0].name;
        display.style.display = 'inline-block';
    }
});