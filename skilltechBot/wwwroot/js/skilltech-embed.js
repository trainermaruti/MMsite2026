/**
 * SkillTech Navigator - Widget Embed Script
 * This script dynamically loads the SkillTech chat widget on any website
 */

(function () {
    'use strict';

    // Configuration - Update this with your actual domain
    const WIDGET_BASE_URL = window.SkillTechConfig?.baseUrl || 'https://yourdomain.com';

    // Prevent multiple instances
    if (window.SkillTechWidgetLoaded) {
        console.warn('SkillTech Widget already loaded');
        return;
    }
    window.SkillTechWidgetLoaded = true;

    // Load CSS
    const linkElement = document.createElement('link');
    linkElement.rel = 'stylesheet';
    linkElement.href = `${WIDGET_BASE_URL}/css/chat-widget.css`;
    document.head.appendChild(linkElement);

    // Load Google Fonts
    const fontLink = document.createElement('link');
    fontLink.rel = 'stylesheet';
    fontLink.href = 'https://fonts.googleapis.com/css2?family=Inter+Tight:wght@300;400;500;600;700&display=swap';
    document.head.appendChild(fontLink);

    // Create Widget HTML
    const widgetHTML = `
        <div class="chat-widget-container" id="chatWidgetContainer">
            <div class="chat-popup" id="chatPopup">
                <div class="chat-window">
                    <div class="chat-header">
                        <div class="header-title">
                            <div class="bot-avatar-small">ST</div>
                            <div class="header-info">
                                <h3>SkillTech Navigator</h3>
                                <div class="status-line">
                                    <span class="status-dot"></span>
                                    <span>Online</span>
                                </div>
                            </div>
                        </div>
                        <button class="close-btn" id="closeBtn" aria-label="Close chat">×</button>
                    </div>

                    <div class="chat-messages" id="chatMessages">
                        <div class="welcome-screen" id="welcomeScreen">
                            <img src="${WIDGET_BASE_URL}/images/skilltech-logo.png" alt="SkillTech Logo" class="welcome-logo" />
                            <h2>Welcome to SkillTech Navigator</h2>
                            <p>I can help you choose the right Azure certification, explore our courses, check pricing plans, or book mentorship with Maruti Makwana.</p>
                            <div class="quick-actions">
                                <button class="quick-action-btn" onclick="sendQuickMessage('I am new to Cloud and Azure')">
                                    I'm new to Cloud & Azure
                                </button>
                                <button class="quick-action-btn" onclick="sendQuickMessage('I need a certification')">
                                    I need a certification
                                </button>
                                <button class="quick-action-btn" onclick="sendQuickMessage('Tell me about pricing')">
                                    Tell me about pricing
                                </button>
                            </div>
                        </div>

                        <div class="typing-indicator" id="typingIndicator">
                            <div class="message-avatar">ST</div>
                            <div class="typing-dots">
                                <div class="typing-dot"></div>
                                <div class="typing-dot"></div>
                                <div class="typing-dot"></div>
                            </div>
                        </div>
                    </div>

                    <div class="chat-input-area">
                        <div class="chat-input-container">
                            <input type="text" class="chat-input" id="messageInput" 
                                   placeholder="Ask about courses, certifications, or pricing..." 
                                   autocomplete="off" />
                            <button class="send-btn" id="sendBtn" onclick="sendMessage()">➤</button>
                        </div>
                    </div>
                </div>
            </div>

            <button class="chat-launcher" id="chatLauncher" aria-label="Open SkillTech Chat">
                <span class="launcher-icon open-icon">💬</span>
                <span class="launcher-icon close-icon">✕</span>
            </button>
        </div>
    `;

    // Inject widget into page when DOM is ready
    function injectWidget() {
        const container = document.createElement('div');
        container.innerHTML = widgetHTML;
        document.body.appendChild(container.firstElementChild);

        // Load widget functionality script
        const script = document.createElement('script');
        script.src = `${WIDGET_BASE_URL}/js/chat-widget.js`;
        script.async = true;
        document.body.appendChild(script);
    }

    // Wait for DOM to be ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', injectWidget);
    } else {
        injectWidget();
    }

    // Expose configuration for chat-widget.js
    window.SkillTechAPI_URL = window.SkillTechConfig?.apiUrl || `${WIDGET_BASE_URL}/api/chat`;
})();
