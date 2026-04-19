/**
 * chat-sidebar.js
 * Collapsible chat sidebar: conversations list, search users, send messages.
 */

(() => {
    'use strict';

    /* ─── State ─────────────────────────────────────────────────── */

    let activePeerId = null;
    let activePeerName = null;
    let searchDebounce = null;

    /* ─── DOM refs (resolved after DOMContentLoaded) ───────────── */

    const $ = id => document.getElementById(id);

    let sidebar, overlay, searchInput, searchResults, convList,
        convPanel, msgPanel, msgPeerName, messagesContainer,
        messageInput, sendBtn, toggleBtn, unreadBadge;

    /* ─── Init ──────────────────────────────────────────────────── */

    document.addEventListener('DOMContentLoaded', () => {
        sidebar           = $('chatSidebar');
        overlay           = $('chatOverlay');
        searchInput       = $('chatSearchInput');
        searchResults     = $('chatSearchResults');
        convList          = $('chatConvList');
        convPanel         = $('chatConvPanel');
        msgPanel          = $('chatMsgPanel');
        msgPeerName       = $('chatMsgPeerName');
        messagesContainer = $('chatMessages');
        messageInput      = $('chatMessageInput');
        sendBtn           = $('chatSendBtn');
        toggleBtn         = $('chatToggleBtn');
        unreadBadge       = $('chatUnreadBadge');

        if (!sidebar) return; // sidebar not rendered (guest user)

        searchInput.addEventListener('input', onSearchInput);

        convList.addEventListener('click', e => {
            const item = e.target.closest('.chat-conv-item[data-peer-id]');
            if (!item) return;
            openConversation(Number(item.dataset.peerId), item.dataset.peerName);
        });

        searchResults.addEventListener('click', e => {
            const item = e.target.closest('.chat-search-item[data-user-id]');
            if (!item) return;
            startChatFromSearch(Number(item.dataset.userId), item.dataset.userName);
        });
        messageInput.addEventListener('keydown', e => {
            if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); sendMessage(); }
        });

        loadConversations();
    });

    /* ─── Toggle ─────────────────────────────────────────────────── */

    function toggleSidebar() {
        if (sidebar.classList.contains('is-open')) {
            closeSidebar();
        } else {
            openSidebar();
        }
    }

    function openSidebar() {
        sidebar.classList.add('is-open');
        overlay.classList.add('is-visible');
        document.body.style.overflow = 'hidden';
    }

    function closeSidebar() {
        sidebar.classList.remove('is-open');
        overlay.classList.remove('is-visible');
        document.body.style.overflow = '';
    }

    /* ─── Panel navigation ───────────────────────────────────────── */

    function showConvPanel() {
        convPanel.style.display = 'flex';
        msgPanel.style.display = 'none';
        activePeerId = null;
        activePeerName = null;
        loadConversations();
    }

    function showMsgPanel(peerId, peerName) {
        activePeerId = peerId;
        activePeerName = peerName;
        msgPeerName.textContent = peerName;
        convPanel.style.display = 'none';
        msgPanel.style.display = 'flex';
        loadMessages(peerId);
    }

    /* ─── Conversations ──────────────────────────────────────────── */

    async function loadConversations() {
        try {
            const res = await fetch('/api/chat/conversations', { credentials: 'same-origin' });
            if (!res.ok) return;
            const conversations = await res.json();
            renderConversations(conversations);
            updateUnreadBadge(conversations);
        } catch (_) { /* network error – silently ignore */ }
    }

    function renderConversations(list) {
        if (!list || list.length === 0) {
            convList.innerHTML = '<div class="chat-conv-empty">Немає розмов.<br>Знайдіть користувача через пошук.</div>';
            return;
        }

        convList.innerHTML = list.map(c => {
            const initial = (c.otherUserName || '?')[0].toUpperCase();
            const preview = c.lastMessageContent
                ? escapeHtml(c.lastMessageContent).substring(0, 40)
                : '';
            const timeStr = c.lastMessageAt ? formatTime(c.lastMessageAt) : '';
            const unread  = c.unreadCount > 0
                ? `<span class="chat-unread-badge">${c.unreadCount}</span>`
                : '';

            return `<div class="chat-conv-item" data-peer-id="${c.otherUserId}" data-peer-name="${escapeHtml(c.otherUserName)}">
                <div class="chat-conv-avatar">${initial}</div>
                <div class="chat-conv-info">
                    <div class="chat-conv-info__name">${escapeHtml(c.otherUserName)}</div>
                    <div class="chat-conv-info__preview">${preview}</div>
                </div>
                <div class="chat-conv-meta">
                    <span class="chat-conv-meta__time">${timeStr}</span>
                    ${unread}
                </div>
            </div>`;
        }).join('');
    }

    function updateUnreadBadge(list) {
        if (!unreadBadge) return;
        const total = list.reduce((sum, c) => sum + (c.unreadCount || 0), 0);
        if (total > 0) {
            unreadBadge.textContent = total > 99 ? '99+' : total;
            unreadBadge.style.display = 'flex';
        } else {
            unreadBadge.style.display = 'none';
        }
    }

    /* ─── Open conversation ──────────────────────────────────────── */

    async function openConversation(peerId, peerName) {
        openSidebar();
        showMsgPanel(peerId, peerName);
        // mark messages as read
        try {
            await fetch(`/api/chat/conversation/${peerId}/mark-read`, {
                method: 'POST',
                credentials: 'same-origin',
            });
            loadConversations(); // refresh badge
        } catch (_) { /* ignore */ }
    }

    /* ─── Messages ───────────────────────────────────────────────── */

    async function loadMessages(peerId) {
        messagesContainer.innerHTML = '<div class="chat-msg-loading">Завантаження...</div>';
        try {
            const res = await fetch(`/api/chat/conversation/${peerId}`, { credentials: 'same-origin' });
            if (!res.ok) {
                messagesContainer.innerHTML = '<div class="chat-msg-loading">Помилка завантаження.</div>';
                return;
            }
            const messages = await res.json();
            renderMessages(messages);
        } catch (_) {
            messagesContainer.innerHTML = '<div class="chat-msg-loading">Помилка завантаження.</div>';
        }
    }

    function renderMessages(messages) {
        if (!messages || messages.length === 0) {
            messagesContainer.innerHTML = '<div class="chat-msg-loading">Поки немає повідомлень. Напишіть першим!</div>';
            return;
        }

        messagesContainer.innerHTML = messages.map(m => {
            const isMine = m.senderId !== activePeerId;
            const cls = isMine ? 'chat-bubble--mine' : 'chat-bubble--theirs';
            const timeStr = m.createdAt ? formatTime(m.createdAt) : '';
            return `<div class="chat-bubble ${cls}">
                ${escapeHtml(m.content)}
                <span class="chat-bubble__time">${timeStr}</span>
            </div>`;
        }).join('');

        scrollMessagesToBottom();
    }

    function appendMessage(msg) {
        const isMine = msg.senderId !== activePeerId;
        const cls = isMine ? 'chat-bubble--mine' : 'chat-bubble--theirs';
        const timeStr = msg.createdAt ? formatTime(msg.createdAt) : '';
        const bubble = document.createElement('div');
        bubble.className = `chat-bubble ${cls}`;
        bubble.innerHTML = `${escapeHtml(msg.content)}<span class="chat-bubble__time">${timeStr}</span>`;
        messagesContainer.appendChild(bubble);
        scrollMessagesToBottom();
    }

    function scrollMessagesToBottom() {
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    /* ─── Send message ────────────────────────────────────────────── */

    async function sendMessage() {
        const content = messageInput.value.trim();
        if (!content || !activePeerId) return;

        sendBtn.disabled = true;
        messageInput.disabled = true;

        try {
            const res = await fetch('/api/chat/send', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ receiverId: activePeerId, content }),
            });

            if (res.ok) {
                const msg = await res.json();
                messageInput.value = '';
                appendMessage(msg);
                loadConversations();
            }
        } catch (_) { /* ignore */ } finally {
            sendBtn.disabled = false;
            messageInput.disabled = false;
            messageInput.focus();
        }
    }

    /* ─── Search ──────────────────────────────────────────────────── */

    function onSearchInput() {
        clearTimeout(searchDebounce);
        const query = searchInput.value.trim();
        if (query.length < 3) {
            searchResults.style.display = 'none';
            searchResults.innerHTML = '';
            return;
        }
        searchDebounce = setTimeout(() => searchUsers(query), 280);
    }

    async function searchUsers(query) {
        try {
            const res = await fetch(
                `/api/contacts/search?query=${encodeURIComponent(query)}&limit=15`,
                { credentials: 'same-origin' }
            );
            if (!res.ok) return;
            const users = await res.json();
            renderSearchResults(users);
        } catch (_) { /* ignore */ }
    }

    function renderSearchResults(users) {
        if (!users || users.length === 0) {
            searchResults.innerHTML = '<div class="chat-search-item"><span class="chat-search-item__name" style="color:#b0997c">Нікого не знайдено</span></div>';
            searchResults.style.display = 'block';
            return;
        }

        searchResults.innerHTML = users.map(u => {
            const badge = u.isContact
                ? '<span class="chat-search-item__badge">контакт</span>'
                : '';
            return `<div class="chat-search-item" data-user-id="${u.id}" data-user-name="${escapeHtml(u.userName)}">
                <span class="chat-search-item__name">${escapeHtml(u.userName)}</span>
                <span class="chat-search-item__email">${escapeHtml(u.email || '')}</span>
                ${badge}
            </div>`;
        }).join('');

        searchResults.style.display = 'block';
    }

    async function startChatFromSearch(userId, userName) {
        searchInput.value = '';
        searchResults.style.display = 'none';
        searchResults.innerHTML = '';
        await ensureContact(userId);
        openConversation(userId, userName);
    }

    /* ─── Open chat from author link (Publication page) ──────────── */

    async function openChatWithAuthor(userId, userName) {
        await ensureContact(userId);
        openConversation(userId, userName);
    }

    /* ─── Ensure contact exists ──────────────────────────────────── */

    async function ensureContact(userId) {
        try {
            await fetch(`/api/contacts/${userId}`, {
                method: 'POST',
                credentials: 'same-origin',
            });
        } catch (_) { /* ignore – contact may already exist */ }
    }

    /* ─── Helpers ─────────────────────────────────────────────────── */

    function escapeHtml(str) {
        if (!str) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    function formatTime(iso) {
        const d = new Date(iso);
        if (isNaN(d)) return '';
        const now = new Date();
        const sameDay = d.toDateString() === now.toDateString();
        if (sameDay) {
            return d.toLocaleTimeString('uk-UA', { hour: '2-digit', minute: '2-digit' });
        }
        return d.toLocaleDateString('uk-UA', { day: '2-digit', month: '2-digit' });
    }

    /* ─── Public API ──────────────────────────────────────────────── */

    window.chatSidebar = {
        toggle:             toggleSidebar,
        open:               openSidebar,
        close:              closeSidebar,
        showConvPanel:      showConvPanel,
        openConversation:   openConversation,
        sendMessage:        sendMessage,
        startChatFromSearch: startChatFromSearch,
        openChatWithAuthor: openChatWithAuthor,
    };
})();
