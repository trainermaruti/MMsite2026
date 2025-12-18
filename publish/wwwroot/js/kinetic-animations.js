// Kinetic Animations with GSAP
document.addEventListener('DOMContentLoaded', function() {
    initScrollAnimations();
    initTimelineAnimations();
    initCardEffects();
});

// Scroll Animations for fade-up elements
function initScrollAnimations() {
    if (typeof gsap === 'undefined') return;
    
    gsap.registerPlugin(ScrollTrigger);
    
    // Fade up elements
    document.querySelectorAll('.fade-up').forEach(el => {
        gsap.from(el, {
            scrollTrigger: {
                trigger: el,
                start: 'top 85%',
                toggleActions: 'play none none none'
            },
            y: 50,
            opacity: 0,
            duration: 1,
            ease: 'power2.out'
        });
    });
    
    // Counter animation for stats
    document.querySelectorAll('.stat-number').forEach(stat => {
        const target = parseInt(stat.getAttribute('data-count'));
        if (!isNaN(target)) {
            gsap.from(stat, {
                scrollTrigger: {
                    trigger: stat,
                    start: 'top 80%',
                    toggleActions: 'play none none none'
                },
                textContent: 0,
                duration: 2,
                ease: 'power2.out',
                snap: { textContent: 1 },
                onUpdate: function() {
                    stat.textContent = Math.ceil(stat.textContent) + '+';
                }
            });
        }
    });
}

// Timeline specific animations
function initTimelineAnimations() {
    if (typeof gsap === 'undefined') return;
    
    // Animate timeline cards on scroll
    document.querySelectorAll('.timeline-card').forEach((card, index) => {
        gsap.from(card, {
            scrollTrigger: {
                trigger: card,
                start: 'top 90%',
                toggleActions: 'play none none none'
            },
            y: 60,
            opacity: 0,
            duration: 0.8,
            delay: index * 0.15,
            ease: 'power3.out'
        });
    });
    
    // Animate icon badges
    document.querySelectorAll('.timeline-node-wrapper > div:first-child').forEach((badge, index) => {
        gsap.from(badge, {
            scrollTrigger: {
                trigger: badge,
                start: 'top 90%',
                toggleActions: 'play none none none'
            },
            scale: 0,
            rotation: 180,
            opacity: 0,
            duration: 0.6,
            delay: index * 0.15,
            ease: 'back.out(1.7)'
        });
    });
}

// 3D Card Tilt Effects
function initCardEffects() {
    if (typeof gsap === 'undefined') return;
    
    document.querySelectorAll('.timeline-card').forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            const centerX = rect.width / 2;
            const centerY = rect.height / 2;
            const rotateX = (y - centerY) / 50;
            const rotateY = (centerX - x) / 50;
            
            gsap.to(card, {
                rotateX: rotateX,
                rotateY: rotateY,
                duration: 0.4,
                transformPerspective: 1500,
                ease: 'power2.out'
            });
        });
        
        card.addEventListener('mouseleave', () => {
            gsap.to(card, {
                rotateX: 0,
                rotateY: 0,
                duration: 0.6,
                ease: 'power2.out'
            });
        });
    });
}
