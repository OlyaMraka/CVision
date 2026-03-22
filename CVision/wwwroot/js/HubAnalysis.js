(function () {
    'use strict';

    /* ── DOM ───────────────────────────────────────────────── */
    const fileInput         = document.getElementById('cvFileInput');
    const pickBtn           = document.getElementById('pickFileBtn');
    const analyzeBtn        = document.getElementById('analyzeBtn');
    const dropZone          = document.getElementById('dropZone');
    const uploadPlaceholder = document.getElementById('uploadPlaceholder');
    const filePreviewWrap   = document.getElementById('filePreviewWrap');
    const filePreviewImg    = document.getElementById('filePreviewImg');
    const filePreviewIcon   = document.getElementById('filePreviewIcon');
    const fileExtBadge      = document.getElementById('fileExtBadge');
    const fileNameLabel     = document.getElementById('fileNameLabel');
    const summaryEmpty      = document.getElementById('summaryEmpty');
    const summaryResult     = document.getElementById('summaryResult');
    const totalScore        = document.getElementById('totalScore');
    const scoreLabel        = document.getElementById('scoreLabel');
    const scoreDesc         = document.getElementById('scoreDesc');
    const feedbackText      = document.getElementById('feedbackText');
    const hubLoader         = document.getElementById('hubLoader');
    const hubResults        = document.getElementById('hubResults');
    const sectionsGrid      = document.getElementById('sectionsGrid');
    const sectionsMeta      = document.getElementById('sectionsMeta');

    let selectedFile = null;


    /* ════════════════════════════════════════════════════════
       A. ВИБІР ФАЙЛУ
    ════════════════════════════════════════════════════════ */
    pickBtn.addEventListener('click', () => fileInput.click());

    fileInput.addEventListener('change', () => {
        if (fileInput.files.length) handleFile(fileInput.files[0]);
    });

    function handleFile(file) {
        const allowed = [
            'application/pdf',
            'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
        ];
        const ext = file.name.split('.').pop().toLowerCase();

       

        selectedFile = file;

        uploadPlaceholder.style.display = 'none';
        filePreviewWrap.style.display   = 'block';
        fileNameLabel.style.display     = 'inline-block';
        fileNameLabel.textContent       = file.name;
        filePreviewIcon.style.display   = 'block';
        filePreviewImg.style.display    = 'none';
        fileExtBadge.textContent        = ext.toUpperCase();
        analyzeBtn.disabled             = false;
    }


    /* ════════════════════════════════════════════════════════
       B. DRAG & DROP
    ════════════════════════════════════════════════════════ */
    ['dragenter', 'dragover'].forEach(ev => {
        dropZone.addEventListener(ev, e => {
            e.preventDefault();
            dropZone.classList.add('drag-over');
        });
    });

    ['dragleave', 'drop'].forEach(ev => {
        dropZone.addEventListener(ev, e => {
            e.preventDefault();
            dropZone.classList.remove('drag-over');
        });
    });

    dropZone.addEventListener('drop', e => {
        const file = e.dataTransfer?.files?.[0];
        if (file) handleFile(file);
    });


    /* ════════════════════════════════════════════════════════
       C. КНОПКА "АНАЛІЗУВАТИ" → POST /Hub/Analyze
    ════════════════════════════════════════════════════════ */
    analyzeBtn.addEventListener('click', () => {
        if (!selectedFile) return;
        startAnalysis();
    });

    async function startAnalysis() {
        /* Блокуємо UI, показуємо лоадер */
        setLoading(true);

        
            const formData = new FormData();
            formData.append('file', selectedFile);

            /*
                RequestVerificationToken — читаємо з прихованого input
                який ASP.NET Core рендерить автоматично при наявності
                @Html.AntiForgeryToken() у View або Layout.

                Якщо токена немає — додай у _Layout.cshtml перед </body>:
                @Html.AntiForgeryToken()
            */
            const token = document
                .querySelector('input[name="__RequestVerificationToken"]')
                ?.value ?? '';

            const response = await fetch('/Hub/Analyze', {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': token
                }
            });

            /*
                Парсимо відповідь як JSON в будь-якому випадку —
                і при успіху (200 OK), і при помилці (400 Bad Request).
                Контролер завжди повертає JSON.
            */
            const data = await response.json();
            

          
            showResults(data);
            
    }

    /* Вмикає / вимикає стан завантаження */
    function setLoading(isLoading) {
        analyzeBtn.disabled = isLoading;
        pickBtn.disabled    = isLoading;

        if (isLoading) {
            hubLoader.classList.add('is-active');
            hubResults.classList.remove('is-active');
            summaryEmpty.style.display  = 'block';
            summaryResult.style.display = 'none';
        } else {
            hubLoader.classList.remove('is-active');
        }
    }


    /* ════════════════════════════════════════════════════════
       D. ПОКАЗ РЕЗУЛЬТАТІВ
    ════════════════════════════════════════════════════════ */
    function showResults(data) {
        setLoading(false);

        /* Summary */
        summaryEmpty.style.display  = 'none';
        summaryResult.style.display = 'block';

        animateNumber(totalScore, 0, data.score, 900, '%');
        scoreLabel.textContent   = getScoreLabel(data.score);
        scoreDesc.textContent    = getScoreDesc(data.score);
        feedbackText.textContent = data.feedback;

        /* Секції */
        const count = data.sectionResults?.length ?? 0;
        sectionsMeta.textContent = `На основі ${count * 2} критеріїв оцінки`;

        renderSections(data.sectionResults ?? []);

        hubResults.classList.add('is-active');
    }


    /* ════════════════════════════════════════════════════════
       E. РЕНДЕР КАРТОК СЕКЦІЙ з анімацією
    ════════════════════════════════════════════════════════ */
    function renderSections(sections) {
        sectionsGrid.innerHTML = '';

        sections.forEach((section, i) => {
            const scoreClass = section.score >= 80 ? 'high'
                : section.score >= 55 ? 'medium'
                    : 'low';

            const card = document.createElement('div');
            card.className = 'hub-section-card';
            card.innerHTML = `
                <div class="hub-section-card__header">
                    <div class="hub-section-card__icon">
                        ${getSectionIcon(section.title)}
                    </div>
                    <div class="hub-section-card__name">${escapeHtml(section.title)}</div>
                    <div class="hub-section-card__score hub-section-card__score--${scoreClass}"
                         data-score="${section.score}">0%</div>
                </div>
                <div class="hub-section-card__bar">
                    <div class="hub-section-card__bar-fill"
                         data-width="${section.score}"></div>
                </div>
                <div class="hub-section-card__text">"${escapeHtml(section.content)}"</div>
            `;

            sectionsGrid.appendChild(card);

            /* Кожна картка з'являється з затримкою 80мс — staggered */
            setTimeout(() => {
                card.classList.add('is-visible');

                const bar = card.querySelector('.hub-section-card__bar-fill');
                if (bar) bar.style.width = bar.dataset.width + '%';

                const scoreEl = card.querySelector('.hub-section-card__score');
                if (scoreEl) animateNumber(scoreEl, 0, section.score, 700, '%');

            }, 100 + i * 80);
        });
    }


    /* ════════════════════════════════════════════════════════
       F. УТИЛІТИ
    ════════════════════════════════════════════════════════ */

    /* Анімований лічильник — ease-out cubic */
    function animateNumber(el, from, to, duration, suffix) {
        const start = performance.now();
        (function step(now) {
            const p = Math.min((now - start) / duration, 1);
            const e = 1 - Math.pow(1 - p, 3);
            el.textContent = Math.round(from + (to - from) * e) + (suffix || '');
            if (p < 1) requestAnimationFrame(step);
        }(performance.now()));
    }

    function getScoreLabel(s) {
        if (s >= 85) return 'Дуже високий потенціал';
        if (s >= 70) return 'Хороший рівень';
        if (s >= 50) return 'Є що покращити';
        return 'Потребує доопрацювання';
    }

    function getScoreDesc(s) {
        if (s >= 85) return 'Ваше резюме входить в топ 5% серед кандидатів на роль Senior розробника.';
        if (s >= 70) return 'Ваше резюме вище середнього. Кілька покращень виведуть його на топ рівень.';
        return 'Є важливі зони для покращення. Дотримуйтесь рекомендацій AI.';
    }

    function getSectionIcon(title) {
        const t = title.toLowerCase();
        if (t.includes('контакт'))
            return `<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/></svg>`;
        if (t.includes('досвід') || t.includes('робот'))
            return `<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="7" width="20" height="14" rx="2"/><path d="M16 7V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v2"/></svg>`;
        if (t.includes('навич') || t.includes('skill'))
            return `<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/></svg>`;
        if (t.includes('освіт'))
            return `<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 10v6M2 10l10-5 10 5-10 5z"/><path d="M6 12v5c3 3 9 3 12 0v-5"/></svg>`;
        if (t.includes('проєкт') || t.includes('project'))
            return `<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>`;
        if (t.includes('summary') || t.includes('про себе') || t.includes('опис'))
            return `<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>`;
        /* дефолтна іконка */
        return `<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><path d="M12 8v4l3 3"/></svg>`;
    }

    /*
        Захист від XSS — екрануємо дані що прийшли з сервера
        перед вставкою в innerHTML.
    */
    function escapeHtml(str) {
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    /*
        Toast замість alert() — не блокує UI.
        type: 'error' | 'info'
    */
    function showToast(message, type) {
        const existing = document.getElementById('hub-toast');
        if (existing) existing.remove();

        const toast = document.createElement('div');
        toast.id = 'hub-toast';
        toast.textContent = message;

        Object.assign(toast.style, {
            position:     'fixed',
            bottom:       '2rem',
            left:         '50%',
            transform:    'translateX(-50%) translateY(8px)',
            background:   type === 'error' ? '#e8547a' : '#c47c2a',
            color:        '#fff',
            padding:      '.7rem 1.4rem',
            borderRadius: '50px',
            fontSize:     '.88rem',
            fontWeight:   '600',
            fontFamily:   "'DM Sans', sans-serif",
            boxShadow:    '0 4px 20px rgba(0,0,0,.18)',
            zIndex:       '9999',
            opacity:      '0',
            transition:   'opacity .2s, transform .2s'
        });

        document.body.appendChild(toast);

        /* Показати */
        requestAnimationFrame(() => {
            toast.style.opacity   = '1';
            toast.style.transform = 'translateX(-50%) translateY(0)';
        });

        /* Сховати через 4 секунди */
        setTimeout(() => {
            toast.style.opacity   = '0';
            toast.style.transform = 'translateX(-50%) translateY(8px)';
            setTimeout(() => toast.remove(), 220);
        }, 4000);
    }

}());
