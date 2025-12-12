'use strict';

// Touch detection
const isTouchDevice = 'ontouchstart' in window || navigator.maxTouchPoints > 0;

// Prefers reduced motion detection
const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

// Initialize Bento Grid interactions
document.addEventListener('DOMContentLoaded', () => {
    const tiles = document.querySelectorAll('[data-bento-tile]');
    
    tiles.forEach(tile => {
        // Keyboard Enter activation
        tile.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                const linkUrl = tile.getAttribute('data-link-url');
                if (linkUrl) {
                    window.location.href = linkUrl;
                } else {
                    const link = tile.querySelector('a');
                    if (link) {
                        link.click();
                    }
                }
            }
        });
        
        // Micro-parallax on hover (only for non-touch, non-reduced-motion)
        if (!isTouchDevice && !prefersReducedMotion) {
            tile.addEventListener('mouseenter', () => {
                tile.style.transform = 'translateY(-4px)';
            });
            
            tile.addEventListener('mouseleave', () => {
                tile.style.transform = 'translateY(0)';
            });
        }
    });
});
