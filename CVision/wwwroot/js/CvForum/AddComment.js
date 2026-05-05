function hideAllInlineCommentEditors() {
    document.querySelectorAll('[id^="commentEditBox-"]').forEach(box => {
        box.style.display = 'none';
    });

    document.querySelectorAll('[id^="commentContent-"]').forEach(content => {
        content.style.display = 'block';
    });
}

function showCommentForm() {
    hideAllInlineCommentEditors();

    const editor = document.getElementById('commentEditor');
    const btn = document.getElementById('btnOpenEditor');

    if (editor && btn) {
        const parentInput = document.querySelector('input[name="ParentCommentId"]');
        if (parentInput) parentInput.value = "";

        const textarea = editor.querySelector('textarea');
        textarea.placeholder = "Ваш відгук про це CV...";

        editor.style.display = 'block';
        btn.style.display = 'none';
        textarea.focus();
    }
}

function showReplyForm(commentId, authorName) {
    hideAllInlineCommentEditors();

    const editor = document.getElementById('commentEditor');
    const btn = document.getElementById('btnOpenEditor');
    const parentInput = document.querySelector('input[name="ParentCommentId"]');
    const textarea = editor.querySelector('textarea');

    if (editor && parentInput) {
        parentInput.value = commentId;
        textarea.placeholder = `Відповідь для ${authorName}...`;

        editor.style.display = 'block';
        if (btn) btn.style.display = 'none';
        textarea.focus();
    }
}

function hideCommentForm() {
    const editor = document.getElementById('commentEditor');
    const btn = document.getElementById('btnOpenEditor');

    if (editor) editor.style.display = 'none';
    if (btn) btn.style.display = 'flex';
}

function startCommentEdit(commentId) {
    hideCommentForm();
    hideAllInlineCommentEditors();

    const contentBlock = document.getElementById(`commentContent-${commentId}`);
    const editBox = document.getElementById(`commentEditBox-${commentId}`);
    const input = document.getElementById(`commentEditInput-${commentId}`);

    if (!contentBlock || !editBox || !input) {
        return;
    }

    contentBlock.style.display = 'none';
    editBox.style.display = 'block';
    input.focus();
    input.setSelectionRange(input.value.length, input.value.length);
}

function cancelCommentEdit(commentId) {
    const contentBlock = document.getElementById(`commentContent-${commentId}`);
    const editBox = document.getElementById(`commentEditBox-${commentId}`);
    const input = document.getElementById(`commentEditInput-${commentId}`);
    const text = document.getElementById(`commentText-${commentId}`);

    if (input && text) {
        input.value = text.textContent.trim();
    }

    if (contentBlock) contentBlock.style.display = 'block';
    if (editBox) editBox.style.display = 'none';
}