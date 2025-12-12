/**
 * Theme Toggle System
 * Maruti Makwana Training Portal
 * Handles light/dark theme switching with persistence
 */

(function() {
    'use strict';
    
    const THEME_KEY = 'mm-theme-preference';
    const THEMES = { DARK: 'dark', LIGHT: 'light' };
    const html = document.documentElement;
    
    /**
     * Apply theme to document
     * @param {string} theme - 'light' or 'dark'
     */
    function applyTheme(theme) {
        const isLight = theme === THEMES.LIGHT;
        
        // Update HTML class
        html.classList.toggle('light-mode', isLight);
        
        // Update data attribute
        html.setAttribute('data-theme', theme);
        
        // Update toggle button icon if present
        updateToggleIcon(theme);
        
        // Respect reduced motion
        if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
            html.style.transition = 'none';
            setTimeout(() => html.style.transition = '', 0);
        }
    }
    
    /**
     * Update theme toggle button icon
     * @param {string} theme - Current theme
     */
    function updateToggleIcon(theme) {
        const toggleBtn = document.querySelector('[data-theme-toggle]');
        if (!toggleBtn) return;
        
        const icon = toggleBtn.querySelector('i, svg');
        if (!icon) return;
        
        // If using Font Awesome
        if (icon.tagName === 'I') {
            icon.className = theme === THEMES.LIGHT ? 'fas fa-sun' : 'fas fa-moon';
        }
        
        // Update aria-label
        toggleBtn.setAttribute(
            'aria-label',
            theme === THEMES.LIGHT ? 'Switch to dark mode' : 'Switch to light mode'
        );
        
        toggleBtn.setAttribute(
            'title',
            theme === THEMES.LIGHT ? 'Switch to dark mode' : 'Switch to light mode'
        );
    }
    
    /**
     * Get current theme
     * @returns {string} - 'light' or 'dark'
     */
    function getCurrentTheme() {
        // Check localStorage first
        const stored = localStorage.getItem(THEME_KEY);
        if (stored && (stored === THEMES.LIGHT || stored === THEMES.DARK)) {
            return stored;
        }
        
        // Fall back to system preference
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches) {
            return THEMES.LIGHT;
        }
        
        return THEMES.DARK; // Default
    }
    
    /**
     * Toggle between themes
     */
    function toggleTheme() {
        const current = html.classList.contains('light-mode') ? THEMES.LIGHT : THEMES.DARK;
        const next = current === THEMES.LIGHT ? THEMES.DARK : THEMES.LIGHT;
        
        applyTheme(next);
        localStorage.setItem(THEME_KEY, next);
        
        // Dispatch custom event for other scripts
        window.dispatchEvent(new CustomEvent('themechange', { detail: { theme: next } }));
    }
    
    /**
     * Initialize theme system
     */
    function init() {
        const currentTheme = getCurrentTheme();
        applyTheme(currentTheme);
        
        // Attach toggle listener
        const toggleBtn = document.querySelector('[data-theme-toggle]');
        if (toggleBtn) {
            toggleBtn.addEventListener('click', toggleTheme);
        }
        
        // Listen for system theme changes
        if (window.matchMedia) {
            window.matchMedia('(prefers-color-scheme: light)').addEventListener('change', (e) => {
                // Only auto-switch if user hasn't set a preference
                if (!localStorage.getItem(THEME_KEY)) {
                    applyTheme(e.matches ? THEMES.LIGHT : THEMES.DARK);
                }
            });
        }
    }
    
    /**
     * Public API
     */
    window.MMTheme = {
        set: function(theme) {
            if (theme !== THEMES.LIGHT && theme !== THEMES.DARK) {
                console.warn('Invalid theme. Use "light" or "dark".');
                return;
            }
            applyTheme(theme);
            localStorage.setItem(THEME_KEY, theme);
        },
        get: function() {
            return html.classList.contains('light-mode') ? THEMES.LIGHT : THEMES.DARK;
        },
        toggle: toggleTheme,
        reset: function() {
            localStorage.removeItem(THEME_KEY);
            const systemTheme = window.matchMedia('(prefers-color-scheme: light)').matches 
                ? THEMES.LIGHT 
                : THEMES.DARK;
            applyTheme(systemTheme);
        }
    };
    
    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
    
    /* 
     * SERVER-SIDE PERSISTENCE (OPTIONAL):
     * To persist theme preference server-side:
     * 1. Add theme field to user profile/settings table
     * 2. On theme change, send AJAX request: 
     *    fetch('/api/user/theme', { method: 'POST', body: JSON.stringify({ theme }) })
     * 3. On page load, read from server and set: MMTheme.set(serverTheme)
     */
})();
