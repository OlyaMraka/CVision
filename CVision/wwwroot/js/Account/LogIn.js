function togglePassword(inputId, btn) {
    const input = document.getElementById(inputId);
    const isHidden = input.type === 'password';
    input.type = isHidden ? 'text' : 'password';

    btn.innerHTML = isHidden
        ? `<svg width="16" height="16" viewBox="0 0 24 24" fill="none"
                                        stroke="currentColor" stroke-width="2"
                                        stroke-linecap="round" stroke-linejoin="round">
                                     <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                                     <circle cx="12" cy="12" r="3"/>
                                   </svg>`
        : `<svg width="16" height="16" viewBox="0 0 24 24" fill="none"
                                        stroke="currentColor" stroke-width="2"
                                        stroke-linecap="round" stroke-linejoin="round">
                                     <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8
                                              a18.45 18.45 0 0 1 5.06-5.94"/>
                                     <path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8
                                              a18.5 18.5 0 0 1-2.16 3.19"/>
                                     <line x1="1" y1="1" x2="23" y2="23"/>
                                   </svg>`;
}

document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const emailInput = document.getElementById('Email');
    const passwordInput = document.getElementById('loginPassword');
    const emailErrorSpan = document.querySelector('span[data-valmsg-for="Email"]');
    const passwordErrorSpan = document.querySelector('span[data-valmsg-for="Password"]');
    const summaryAlert = document.getElementById('summaryAlert');

    form.addEventListener('submit', function (e) {
        emailErrorSpan.textContent = '';
        passwordErrorSpan.textContent = '';

        let isValid = true;

        if (!emailInput.value.trim()) {
            emailErrorSpan.textContent = 'Введіть електронну пошту';
            emailInput.classList.add('input-error');
            isValid = false;
        } else if (!isValidEmail(emailInput.value)) {
            emailErrorSpan.textContent = 'Невірний формат email';
            emailInput.classList.add('input-error');
            isValid = false;
        } else {
            emailInput.classList.remove('input-error');
        }

        if (!passwordInput.value.trim()) {
            passwordErrorSpan.textContent = 'Введіть пароль';
            passwordInput.classList.add('input-error');
            isValid = false;
        } else {
            passwordInput.classList.remove('input-error');
        }

        if (!isValid) {
            e.preventDefault();
            summaryAlert.style.display = 'none';
            return false;
        }

        summaryAlert.style.display = 'none';
    });

    emailInput.addEventListener('input', function () {
        if (this.value.trim()) {
            emailErrorSpan.textContent = '';
            this.classList.remove('input-error');
        }
    });

    passwordInput.addEventListener('input', function () {
        if (this.value.trim()) {
            passwordErrorSpan.textContent = '';
            this.classList.remove('input-error');
        }
    });
});

function isValidEmail(email) {
    const emailRegex = /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/;
    return emailRegex.test(email);
}