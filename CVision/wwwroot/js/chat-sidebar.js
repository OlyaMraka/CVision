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
    let editingMessageId = null;

    /* ─── DOM refs (resolved after DOMContentLoaded) ───────────── */

    const $ = id => document.getElementById(id);

    let sidebar, overlay, searchInput, searchResults, convList,
        convPanel, msgPanel, msgPeerName, messagesContainer,
        messageInput, sendBtn, toggleBtn;

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

        if (!sidebar) return; // sidebar not rendered (guest user)
        if (!searchInput || !searchResults || !convList || !convPanel || !msgPanel ||
            !msgPeerName || !messagesContainer || !messageInput || !sendBtn) {
            console.error('Chat sidebar init failed: some DOM elements are missing.');
            return;
        }

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

        messagesContainer.addEventListener('click', onMessagesClick);

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

        messagesContainer.innerHTML = '';
        messages.forEach(m => messagesContainer.appendChild(createBubbleElement(m)));

        scrollMessagesToBottom();
    }

    function appendMessage(msg) {
        messagesContainer.appendChild(createBubbleElement(msg));
        scrollMessagesToBottom();
    }

    function createBubbleElement(msg) {
        if (!msg || typeof msg !== 'object') {
            const fallback = document.createElement('div');
            fallback.className = 'chat-bubble chat-bubble--theirs';
            fallback.textContent = 'Неможливо відобразити повідомлення.';
            return fallback;
        }

        const isMine = msg.senderId !== activePeerId;
        const cls = isMine ? 'chat-bubble--mine' : 'chat-bubble--theirs';
        const timeStr = msg.createdAt ? formatTime(msg.createdAt) : '';

        const bubble = document.createElement('div');
        bubble.className = `chat-bubble ${cls}`;
        bubble.dataset.messageId = String(msg.id || '');

        if (isMine) {
            const actions = document.createElement('div');
            actions.className = 'chat-bubble__actions';

            const editBtn = document.createElement('button');
            editBtn.type = 'button';
            editBtn.className = 'chat-bubble__action-btn';
            editBtn.dataset.action = 'edit';
            editBtn.title = 'Редагувати';
            editBtn.setAttribute('aria-label', 'Редагувати');
            editBtn.innerHTML = '<i class="fa-solid fa-pen"></i>';

            const deleteBtn = document.createElement('button');
            deleteBtn.type = 'button';
            deleteBtn.className = 'chat-bubble__action-btn chat-bubble__action-btn--danger';
            deleteBtn.dataset.action = 'delete';
            deleteBtn.title = 'Видалити';
            deleteBtn.setAttribute('aria-label', 'Видалити');
            deleteBtn.innerHTML = '<i class="fa-solid fa-trash"></i>';

            actions.appendChild(editBtn);
            actions.appendChild(deleteBtn);
            bubble.appendChild(actions);
        }

        const textEl = document.createElement('span');
        textEl.className = 'chat-bubble__text';
        textEl.textContent = msg.content || '';
        bubble.appendChild(textEl);

        if (msg.isEdited) {
            const editedEl = document.createElement('span');
            editedEl.className = 'chat-bubble__edited';
            editedEl.textContent = 'відредаговано';
            bubble.appendChild(editedEl);
        }

        const timeEl = document.createElement('span');
        timeEl.className = 'chat-bubble__time';
        timeEl.textContent = timeStr;
        bubble.appendChild(timeEl);

        return bubble;
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

    /* ─── Edit / Delete message ──────────────────────────────────── */

    function onMessagesClick(e) {
        const actionBtn = e.target.closest('.chat-bubble__action-btn[data-action]');
        if (actionBtn) {
            const bubble = actionBtn.closest('.chat-bubble');
            if (!bubble) return;

            const messageId = getMessageIdFromBubble(bubble);
            if (messageId === null) {
                notifyChatError('Некоректний ID повідомлення. Оновіть сторінку.');
                return;
            }

            if (actionBtn.dataset.action === 'edit') {
                startEditMessage(messageId, bubble);
                return;
            }

            if (actionBtn.dataset.action === 'delete') {
                deleteMessage(messageId, bubble);
                return;
            }
        }

        const saveBtn = e.target.closest('.chat-bubble__edit-save');
        if (saveBtn) {
            const bubble = saveBtn.closest('.chat-bubble');
            if (!bubble) return;
            const messageId = getMessageIdFromBubble(bubble);
            const input = bubble.querySelector('.chat-bubble__edit-input');
            if (!input || messageId === null) {
                notifyChatError('Не вдалося зчитати дані повідомлення для редагування.');
                return;
            }
            saveEditMessage(messageId, input.value.trim(), bubble);
            return;
        }

        const cancelBtn = e.target.closest('.chat-bubble__edit-cancel');
        if (cancelBtn) {
            const bubble = cancelBtn.closest('.chat-bubble');
            if (!bubble) return;
            cancelEditMessage(bubble);
        }
    }

    function startEditMessage(messageId, bubble) {
        if (!Number.isFinite(messageId) || messageId <= 0 || !bubble || !bubble.isConnected) {
            notifyChatError('Неможливо перейти в режим редагування.');
            return;
        }

        if (editingMessageId !== null) {
            notifyChatError('Спершу завершіть попереднє редагування.');
            return;
        }

        if (!bubble.classList.contains('chat-bubble--mine')) {
            notifyChatError('Редагувати можна лише власні повідомлення.');
            return;
        }

        const textEl = bubble.querySelector('.chat-bubble__text');
        if (!textEl) return;

        editingMessageId = messageId;
        bubble.classList.add('chat-bubble--editing');

        const actionsEl = bubble.querySelector('.chat-bubble__actions');
        if (actionsEl) actionsEl.style.display = 'none';

        const initialText = textEl.textContent || '';
        textEl.style.display = 'none';

        const editWrap = document.createElement('div');
        editWrap.className = 'chat-bubble__edit-wrap';

        const input = document.createElement('textarea');
        input.className = 'chat-bubble__edit-input';
        input.value = initialText;

        const controls = document.createElement('div');
        controls.className = 'chat-bubble__edit-controls';

        const cancelBtn = document.createElement('button');
        cancelBtn.type = 'button';
        cancelBtn.className = 'chat-bubble__edit-cancel';
        cancelBtn.textContent = 'Скасувати';

        const saveBtn = document.createElement('button');
        saveBtn.type = 'button';
        saveBtn.className = 'chat-bubble__edit-save';
        saveBtn.textContent = 'Зберегти';

        controls.appendChild(cancelBtn);
        controls.appendChild(saveBtn);
        editWrap.appendChild(input);
        editWrap.appendChild(controls);
        bubble.appendChild(editWrap);

        input.focus();
        input.setSelectionRange(input.value.length, input.value.length);
        input.addEventListener('keydown', event => {
            if (event.key === 'Escape') {
                event.preventDefault();
                cancelEditMessage(bubble);
            }

            if (event.key === 'Enter' && !event.shiftKey) {
                event.preventDefault();
                saveEditMessage(messageId, input.value.trim(), bubble);
            }
        });
    }

    function cancelEditMessage(bubble) {
        editingMessageId = null;
        bubble.classList.remove('chat-bubble--editing');

        const actionsEl = bubble.querySelector('.chat-bubble__actions');
        if (actionsEl) actionsEl.style.display = '';

        const textEl = bubble.querySelector('.chat-bubble__text');
        if (textEl) textEl.style.display = '';

        const editWrap = bubble.querySelector('.chat-bubble__edit-wrap');
        if (editWrap) editWrap.remove();
    }

    async function saveEditMessage(messageId, content, bubble) {
        if (!Number.isFinite(messageId) || messageId <= 0 || !bubble || !bubble.isConnected) {
            notifyChatError('Неможливо зберегти зміни для цього повідомлення.');
            return;
        }

        const textEl = bubble.querySelector('.chat-bubble__text');
        if (!textEl) {
            cancelEditMessage(bubble);
            return;
        }

        const oldContent = (textEl.textContent || '').trim();
        if (!content) {
            notifyChatError('Текст повідомлення не може бути порожнім.');
            return;
        }
        if (content === oldContent) {
            cancelEditMessage(bubble);
            return;
        }

        const saveBtn = bubble.querySelector('.chat-bubble__edit-save');
        const cancelBtn = bubble.querySelector('.chat-bubble__edit-cancel');
        if (saveBtn) saveBtn.disabled = true;
        if (cancelBtn) cancelBtn.disabled = true;

        try {
            const res = await fetch('/api/chat/edit', {
                method: 'PUT',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: messageId, content }),
            });

            if (!res.ok) {
                notifyChatError(await readApiError(res, 'Не вдалося зберегти зміни повідомлення.'));
                return;
            }

            const updatedMessage = await res.json();
            textEl.textContent = updatedMessage.content || content;

            let editedEl = bubble.querySelector('.chat-bubble__edited');
            if (!editedEl) {
                editedEl = document.createElement('span');
                editedEl.className = 'chat-bubble__edited';
                editedEl.textContent = 'відредаговано';

                const timeEl = bubble.querySelector('.chat-bubble__time');
                if (timeEl) {
                    bubble.insertBefore(editedEl, timeEl);
                } else {
                    bubble.appendChild(editedEl);
                }
            }

            cancelEditMessage(bubble);
            loadConversations();
        } catch (error) {
            console.error('Edit message failed:', error);
            notifyChatError('Помилка мережі під час збереження повідомлення.');
        } finally {
            if (saveBtn) saveBtn.disabled = false;
            if (cancelBtn) cancelBtn.disabled = false;
        }
    }

    async function deleteMessage(messageId, bubble) {
        if (!Number.isFinite(messageId) || messageId <= 0 || !bubble || !bubble.isConnected) {
            notifyChatError('Неможливо видалити це повідомлення.');
            return;
        }

        const confirmed = window.confirm('Видалити повідомлення?');
        if (!confirmed) return;

        const actionButtons = bubble.querySelectorAll('.chat-bubble__action-btn');
        actionButtons.forEach(btn => { btn.disabled = true; });

        try {
            const res = await fetch(`/api/chat/delete/${messageId}`, {
                method: 'DELETE',
                credentials: 'same-origin',
            });

            if (!res.ok) {
                notifyChatError(await readApiError(res, 'Не вдалося видалити повідомлення.'));
                actionButtons.forEach(btn => { btn.disabled = false; });
                return;
            }

            if (editingMessageId === messageId) {
                editingMessageId = null;
            }

            bubble.remove();

            if (!messagesContainer.querySelector('.chat-bubble')) {
                messagesContainer.innerHTML = '<div class="chat-msg-loading">Поки немає повідомлень. Напишіть першим!</div>';
            }

            loadConversations();
        } catch (error) {
            console.error('Delete message failed:', error);
            notifyChatError('Помилка мережі під час видалення повідомлення.');
            actionButtons.forEach(btn => { btn.disabled = false; });
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

    function getMessageIdFromBubble(bubble) {
        if (!bubble || !bubble.dataset) return null;
        const messageId = Number(bubble.dataset.messageId);
        if (!Number.isFinite(messageId) || messageId <= 0) return null;
        return messageId;
    }

    function notifyChatError(message) {
        const fallback = 'Сталася помилка. Спробуйте ще раз.';
        window.alert(message || fallback);
    }

    async function readApiError(response, fallbackMessage) {
        try {
            const contentType = response.headers.get('content-type') || '';
            if (contentType.includes('application/json')) {
                const payload = await response.json();
                if (payload && typeof payload === 'object') {
                    const candidates = [payload.error, payload.message, payload.title];
                    const first = candidates.find(x => typeof x === 'string' && x.trim().length > 0);
                    if (first) return first;
                }
            }

            const text = await response.text();
            if (text && text.trim().length > 0) return text.trim();
        } catch (_) { /* ignore parse errors */ }

        return fallbackMessage;
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
