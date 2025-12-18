/**
 * Snake Timeline Animation
 * Progressive Enhancement with GSAP & ScrollTrigger
 * Production-ready with feature detection & performance gating
 */

// Feature Detection
const features = {
    reducedMotion: window.matchMedia('(prefers-reduced-motion: reduce)').matches,
    isMobile: window.innerWidth < 768,
    isTouch: 'ontouchstart' in window,
    hasGoodNetwork: navigator.connection ? navigator.connection.effectiveType !== 'slow-2g' : true
};

// Early exit if reduced motion preference
if (features.reducedMotion) {
    console.log('[SnakeTimeline] Reduced motion detected, skipping animations');
    
    // Show all nodes immediately
    document.querySelectorAll('.timeline-node').forEach(node => {
        node.classList.add('is-visible');
    });
}

// Initialize timeline animation
async function initSnakeTimeline() {
    // Guard: Skip heavy animations on mobile or poor network
    if (features.isMobile || !features.hasGoodNetwork) {
        console.log('[SnakeTimeline] Mobile or slow network detected, using simple reveal');
        simpleRevealAnimation();
        return;
    }

    try {
        // Dynamically import GSAP (code splitting)
        const { default: gsap } = await import('https://cdn.jsdelivr.net/npm/gsap@3.12.5/+esm');
        const { default: ScrollTrigger } = await import('https://cdn.jsdelivr.net/npm/gsap@3.12.5/dist/ScrollTrigger.min.js/+esm');
        
        gsap.registerPlugin(ScrollTrigger);
        
        // Animate SVG Path
        animatePath(gsap, ScrollTrigger);
        
        // Animate Nodes on Scroll
        animateNodes(gsap, ScrollTrigger);
        
        console.log('[SnakeTimeline] GSAP animations initialized');
    } catch (error) {
        console.warn('[SnakeTimeline] Failed to load GSAP, falling back to simple animation', error);
        simpleRevealAnimation();
    }
}

// Generate SVG Path based on node positions
function generatePath(nodes) {
    if (!nodes.length) return '';
    
    const path = ['M'];
    const containerWidth = document.querySelector('.snake-timeline-container').offsetWidth;
    const centerX = containerWidth / 2;
    
    nodes.forEach((node, index) => {
        const rect = node.getBoundingClientRect();
        const containerRect = node.closest('.snake-timeline-container').getBoundingClientRect();
        const y = rect.top - containerRect.top + rect.height / 2;
        
        if (index === 0) {
            path.push(`${centerX},${y}`);
        } else {
            // Create smooth curves between nodes
            const prevNode = nodes[index - 1];
            const prevRect = prevNode.getBoundingClientRect();
            const prevY = prevRect.top - containerRect.top + prevRect.height / 2;
            
            const isLeft = node.classList.contains('timeline-node-left');
            const controlX = isLeft ? centerX - 100 : centerX + 100;
            const midY = (prevY + y) / 2;
            
            path.push(`Q ${controlX},${midY} ${centerX},${y}`);
        }
    });
    
    return path.join(' ');
}

// Animate SVG Path Drawing
function animatePath(gsap, ScrollTrigger) {
    const pathElement = document.querySelector('.snake-path-line');
    if (!pathElement) return;
    
    const nodes = document.querySelectorAll('.timeline-node');
    const pathD = generatePath(Array.from(nodes));
    pathElement.setAttribute('d', pathD);
    
    gsap.to(pathElement, {
        strokeDashoffset: 0,
        ease: 'none',
        scrollTrigger: {
            trigger: '.snake-timeline-container',
            start: 'top 80%',
            end: 'bottom 20%',
            scrub: 1
        }
    });
}

// Animate Timeline Nodes
function animateNodes(gsap, ScrollTrigger) {
    const nodes = document.querySelectorAll('.timeline-node');
    
    nodes.forEach((node, index) => {
        gsap.to(node, {
            opacity: 1,
            y: 0,
            duration: 0.8,
            ease: 'power2.out',
            scrollTrigger: {
                trigger: node,
                start: 'top 85%',
                toggleActions: 'play none none reverse'
            }
        });
    });
}

// Fallback: Simple Intersection Observer Animation
function simpleRevealAnimation() {
    const nodes = document.querySelectorAll('.timeline-node');
    
    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.2,
            rootMargin: '0px 0px -100px 0px'
        });
        
        nodes.forEach(node => observer.observe(node));
    } else {
        // Fallback: Show all nodes immediately
        nodes.forEach(node => node.classList.add('is-visible'));
    }
}

// Initialize on DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSnakeTimeline);
} else {
    initSnakeTimeline();
}

// Re-calculate path on resize (debounced)
let resizeTimeout;
window.addEventListener('resize', () => {
    clearTimeout(resizeTimeout);
    resizeTimeout = setTimeout(() => {
        const pathElement = document.querySelector('.snake-path-line');
        if (pathElement && !features.isMobile) {
            const nodes = document.querySelectorAll('.timeline-node');
            const pathD = generatePath(Array.from(nodes));
            pathElement.setAttribute('d', pathD);
        }
    }, 300);
});
