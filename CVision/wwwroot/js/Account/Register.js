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

(function localizeRegisterErrors() {
    const translations = {
        "A user with this information already exists!": "Користувач з такими даними вже існує.",
        "Email is required!": "Введіть електронну пошту.",
        "Email must be shorter than 40 characters!": "Email має містити не більше 40 символів.",
        "Email must be longer than 4 characters!": "Email має містити щонайменше 4 символи.",
        "Username is required!": "Введіть ім'я користувача.",
        "Username must be shorter than 40 characters!": "Ім'я має містити не більше 40 символів.",
        "Username must be longer than 4 characters!": "Ім'я має містити щонайменше 4 символи.",
        "Password is required!": "Введіть пароль.",
        "Password must be longer than 8 characters!": "Пароль має містити щонайменше 8 символів.",
        "Password must contain at least one uppercase letter!": "Пароль має містити щонайменше одну велику літеру.",
        "Password must contain at least one digit!": "Пароль має містити щонайменше одну цифру.",
        "Password must contain special characters!": "Пароль має містити щонайменше один спеціальний символ."
    };

    const errorNodes = document.querySelectorAll('.form-alert li, .field-validation-error, [data-valmsg-summary="true"] li');
    errorNodes.forEach((node) => {
        const original = (node.textContent || '').trim();
        if (original && translations[original]) {
            node.textContent = translations[original];
        }
    });
})();