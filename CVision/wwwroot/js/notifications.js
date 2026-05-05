/**
 * notifications.js
 * Notification dropdown: SignalR real-time push + REST API (list, mark read).
 */

(() => {
    'use strict';

    /* ─── State ─────────────────────────────────────────────────── */

    let isOpen = false;
    let toastTimer = null;

    /* ─── DOM refs ───────────────────────────────────────────────── */

    const $ = id => document.getElementById(id);

    let toggleBtn, badge, dropdown, list, markAllBtn, toast, toastTitle, toastMessage, toastClose;

    /* ─── Init ──────────────────────────────────────────────────── */

    document.addEventListener('DOMContentLoaded', () => {
        toggleBtn    = $('notifToggleBtn');
        badge        = $('notifBadge');
        dropdown     = $('notifDropdown');
        list         = $('notifList');
        markAllBtn   = $('notifMarkAllBtn');
        toast        = $('notifToast');
        toastTitle   = $('notifToastTitle');
        toastMessage = $('notifToastMessage');
        toastClose   = $('notifToastClose');

        if (!toggleBtn || !dropdown) return; // not signed in

        toggleBtn.addEventListener('click', e => {
            e.stopPropagation();
            isOpen ? close() : open();
        });

        document.addEventListener('click', e => {
            if (isOpen && !dropdown.contains(e.target) && e.target !== toggleBtn) {
                close();
            }
        });

        markAllBtn.addEventListener('click', markAllRead);
        toastClose.addEventListener('click', hideToast);

        loadUnreadCount();
        connectSignalR();

        // Fallback polling keeps badge/list fresh even if SignalR is temporarily unavailable.
        setInterval(async () => {
            await loadUnreadCount();
            if (isOpen) {
                await loadNotifications();
            }
        }, 15000);
    });

    /* ─── Open / Close ───────────────────────────────────────────── */

    function open() {
        isOpen = true;
        dropdown.classList.add('is-open');
        loadNotifications();
    }

    function close() {
        isOpen = false;
        dropdown.classList.remove('is-open');
    }

    /* ─── Badge ──────────────────────────────────────────────────── */

    function setBadge(count) {
        if (!badge) return;
        if (count > 0) {
            badge.textContent = count > 99 ? '99+' : count;
            badge.style.display = 'flex';
        } else {
            badge.style.display = 'none';
        }
    }

    async function loadUnreadCount() {
        try {
            const res = await fetch('/api/notifications/unread-count', { credentials: 'include' });
            if (!res.ok) return;
            const data = await res.json();
            setBadge(data.count);
        } catch { /* silent */ }
    }

    /* ─── Notifications list ─────────────────────────────────────── */

    async function loadNotifications() {
        list.innerHTML = '<div class="notif-empty"><i class="fa-solid fa-spinner fa-spin"></i>Завантаження...</div>';
        try {
            const res = await fetch('/api/notifications', { credentials: 'include' });
            if (!res.ok) throw new Error();
            const items = await res.json();
            renderList(items);
        } catch {
            list.innerHTML = '<div class="notif-empty"><i class="fa-regular fa-circle-xmark"></i>Не вдалось завантажити</div>';
        }
    }

    function renderList(items) {
        if (!items.length) {
            list.innerHTML = '<div class="notif-empty"><i class="fa-regular fa-bell-slash"></i>Немає сповіщень</div>';
            return;
        }

        list.innerHTML = items.map(n => `
            <div class="notif-item ${n.isRead ? '' : 'is-unread'}" data-id="${n.id}" role="button" tabindex="0">
                <div class="notif-item__icon">${typeIcon(n.type)}</div>
                <div class="notif-item__body">
                    <p class="notif-item__title">${escHtml(n.title)}</p>
                    <p class="notif-item__message">${escHtml(n.message)}</p>
                    <span class="notif-item__time">${formatTime(n.createdAt)}</span>
                </div>
            </div>`).join('');

        list.querySelectorAll('.notif-item').forEach(el => {
            el.addEventListener('click', () => onItemClick(el));
            el.addEventListener('keydown', e => { if (e.key === 'Enter') onItemClick(el); });
        });
    }

    async function onItemClick(el) {
        const id = Number(el.dataset.id);
        if (el.classList.contains('is-unread')) {
            el.classList.remove('is-unread');
            await markRead(id);
        }
    }

    /* ─── Mark read ──────────────────────────────────────────────── */

    async function markRead(id) {
        try {
            await fetch(`/api/notifications/${id}/mark-read`, {
                method: 'POST',
                credentials: 'include',
            });
            await loadUnreadCount();
        } catch { /* silent */ }
    }

    async function markAllRead() {
        try {
            await fetch('/api/notifications/mark-all-read', {
                method: 'POST',
                credentials: 'include',
            });
            list.querySelectorAll('.notif-item.is-unread').forEach(el => el.classList.remove('is-unread'));
            setBadge(0);
        } catch { /* silent */ }
    }

    /* ─── SignalR ─────────────────────────────────────────────────── */

    function connectSignalR() {
        if (typeof signalR === 'undefined') return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/notifications')
            .withAutomaticReconnect()
            .build();

        connection.on('ReceiveNotification', notification => {
            showToast(notification.title, notification.message);
            loadUnreadCount();
            if (isOpen) {
                loadNotifications();
            }
        });

        connection.start().catch(err => console.warn('Notification hub connection failed:', err));
    }

    /* ─── Toast ──────────────────────────────────────────────────── */

    function showToast(title, message) {
        if (!toast) return;
        toastTitle.textContent = title;
        toastMessage.textContent = message;
        toast.classList.add('is-visible');

        clearTimeout(toastTimer);
        toastTimer = setTimeout(hideToast, 5000);
    }

    function hideToast() {
        if (!toast) return;
        toast.classList.remove('is-visible');
        clearTimeout(toastTimer);
    }

    /* ─── Helpers ─────────────────────────────────────────────────── */

    function typeIcon(type) {
        const icons = {
            NewMessage: '<i class="fa-regular fa-comment-dots"></i>',
            NewComment: '<i class="fa-regular fa-message"></i>',
            NewContact: '<i class="fa-regular fa-address-card"></i>',
        };
        return icons[type] ?? '<i class="fa-regular fa-bell"></i>';
    }

    function formatTime(iso) {
        const d = new Date(iso);
        const now = new Date();
        const diff = Math.floor((now - d) / 1000);
        if (diff < 60) return 'щойно';
        if (diff < 3600) return `${Math.floor(diff / 60)} хв тому`;
        if (diff < 86400) return `${Math.floor(diff / 3600)} год тому`;
        return d.toLocaleDateString('uk-UA', { day: 'numeric', month: 'short' });
    }

    function escHtml(str) {
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

})();
