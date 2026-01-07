// ================================================
// SKILLTECH CHAT WIDGET - JAVASCRIPT
// ================================================

let messageHistory = [];

// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    initializeWidget();
});

// ================================================
// WIDGET INITIALIZATION
// ================================================

function initializeWidget() {
    const chatLauncher = document.getElementById('chatLauncher');
    const chatPopup = document.getElementById('chatPopup');
    const closeBtn = document.getElementById('closeBtn');
    const messageInput = document.getElementById('messageInput');

    // Launcher button click - toggle chat
    if (chatLauncher) {
        chatLauncher.addEventListener('click', function () {
            toggleChat();
        });
    }

    // Close button click
    if (closeBtn) {
        closeBtn.addEventListener('click', function () {
            closeChat();
        });
    }

    // Handle Enter key in input
    if (messageInput) {
        messageInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                sendMessage();
            }
        });
    }

    // Close on Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && chatPopup.classList.contains('open')) {
            closeChat();
        }
    });
}

// ================================================
// TOGGLE CHAT OPEN/CLOSE
// ================================================

function toggleChat() {
    const chatPopup = document.getElementById('chatPopup');
    const chatLauncher = document.getElementById('chatLauncher');

    if (chatPopup.classList.contains('open')) {
        closeChat();
    } else {
        openChat();
    }
}

function openChat() {
    const chatPopup = document.getElementById('chatPopup');
    const chatLauncher = document.getElementById('chatLauncher');
    const messageInput = document.getElementById('messageInput');

    chatPopup.classList.add('open');
    chatLauncher.classList.add('active');

    // Focus input after animation
    setTimeout(() => {
        if (messageInput) {
            messageInput.focus();
        }
    }, 400);
}

function closeChat() {
    const chatPopup = document.getElementById('chatPopup');
    const chatLauncher = document.getElementById('chatLauncher');

    chatPopup.classList.remove('open');
    chatLauncher.classList.remove('active');
}

// ================================================
// QUICK MESSAGE HANDLER
// ================================================

function sendQuickMessage(message) {
    const messageInput = document.getElementById('messageInput');
    if (messageInput) {
        messageInput.value = message;
        sendMessage();
    }
}

// ================================================
// SEND MESSAGE
// ================================================

async function sendMessage() {
    const input = document.getElementById('messageInput');
    const message = input.value.trim();

    if (!message) return;

    // Hide welcome screen
    const welcomeScreen = document.getElementById('welcomeScreen');
    if (welcomeScreen) {
        welcomeScreen.style.display = 'none';
    }

    // Add user message
    addMessage(message, 'user');

    // Clear input and disable
    input.value = '';
    input.disabled = true;
    document.getElementById('sendBtn').disabled = true;

    // Show typing indicator
    showTypingIndicator();

    try {
        const apiUrl = window.SkillTechAPI_URL || '/api/chat';
        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                message: message,
                history: messageHistory
            })
        });

        const data = await response.json();

        hideTypingIndicator();

        if (data.success) {
            addMessage(data.reply, 'bot', data.goal);
        } else {
            addMessage(data.errorMessage || 'Sorry, I encountered an error. Please try again.', 'bot');
        }
    } catch (error) {
        hideTypingIndicator();
        addMessage('Sorry, I could not connect to the server. Please check your connection and try again.', 'bot');
    }

    // Re-enable input
    input.disabled = false;
    document.getElementById('sendBtn').disabled = false;
    input.focus();
}

// ================================================
// ADD MESSAGE TO CHAT
// ================================================

function addMessage(content, role, goal = null) {
    const messagesContainer = document.getElementById('chatMessages');
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${role}`;

    const avatar = document.createElement('div');
    avatar.className = 'message-avatar';

    if (role === 'user') {
        avatar.textContent = 'U';
    } else {
        avatar.textContent = 'ST';
    }

    const contentDiv = document.createElement('div');
    contentDiv.className = 'message-content';

    const textDiv = document.createElement('div');
    // Use formatText to convert markdown to HTML
    textDiv.innerHTML = formatText(content);
    contentDiv.appendChild(textDiv);

    if (goal && role === 'bot') {
        const goalBadge = document.createElement('div');
        goalBadge.className = 'goal-badge';
        goalBadge.textContent = `${goal}`;
        contentDiv.appendChild(goalBadge);
    }

    const timeDiv = document.createElement('div');
    timeDiv.className = 'message-time';
    timeDiv.textContent = new Date().toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit'
    });
    contentDiv.appendChild(timeDiv);

    if (role === 'user') {
        messageDiv.appendChild(contentDiv);
        messageDiv.appendChild(avatar);
    } else {
        messageDiv.appendChild(avatar);
        messageDiv.appendChild(contentDiv);
    }

    // Insert before typing indicator
    const typingIndicator = document.getElementById('typingIndicator');
    messagesContainer.insertBefore(messageDiv, typingIndicator);

    // Scroll to bottom
    messagesContainer.scrollTop = messagesContainer.scrollHeight;

    // Add to history
    messageHistory.push({
        role: role,
        content: content,
        timestamp: new Date().toISOString()
    });
}

// ================================================
// FORMAT TEXT (MARKDOWN SUPPORT)
// ================================================

function formatText(text) {
    if (!text) return "";

    // 1. First handle markdown links BEFORE sanitization (to preserve URLs)
    // Temporarily replace [text](url) with a placeholder
    const linkPlaceholders = [];
    text = text.replace(/\[([^\]]+)\]\(([^)]+)\)/g, function (match, linkText, url) {
        const placeholder = `__LINK_${linkPlaceholders.length}__`;
        linkPlaceholders.push({ text: linkText, url: url });
        return placeholder;
    });

    // 2. Sanitize to prevent HTML injection (security)
    let clean = text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

    // 3. Restore markdown links as proper HTML
    linkPlaceholders.forEach((link, index) => {
        const linkHtml = `<a href="${link.url}" target="_blank" style="color:#FF9D00; text-decoration:underline; font-weight:500;">${link.text}</a>`;
        clean = clean.replace(`__LINK_${index}__`, linkHtml);
    });

    // 4. Handle raw URLs (that aren't already in links)
    clean = clean.replace(/(?<!href=")(https?:\/\/[^\s&<>"]+)/g, function (url) {
        return `<a href="${url}" target="_blank" style="color:#FF9D00; text-decoration:underline; font-weight:500;">${url}</a>`;
    });

    // 5. Format **Bold** text
    clean = clean.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');

    // 6. Format *Italic* text (only single asterisks not part of bold)
    clean = clean.replace(/(?<!\*)\*([^\*]+?)\*(?!\*)/g, '<em>$1</em>');

    // 7. Format Bullet Points (lines starting with ✓, •, *, or -)
    clean = clean.replace(/^[✓•\*\-]\s+(.*)$/gm, '<li style="margin-bottom:5px;">$1</li>');

    // Wrap consecutive <li> in <ul>
    if (clean.includes('<li>')) {
        clean = clean.replace(/(<li>.*?<\/li>(\s*<li>.*?<\/li>)*)/gs, '<ul style="margin: 10px 0; padding-left: 20px; list-style-type: disc;">$1</ul>');
    }

    // 8. Handle Newlines (convert \n to <br>)
    clean = clean.replace(/\n/g, '<br>');

    return clean;
}

// ================================================
// TYPING INDICATOR
// ================================================

function showTypingIndicator() {
    const indicator = document.getElementById('typingIndicator');
    indicator.style.display = 'flex';
    const messagesContainer = document.getElementById('chatMessages');
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

function hideTypingIndicator() {
    document.getElementById('typingIndicator').style.display = 'none';
}
