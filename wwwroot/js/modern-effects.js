// Modern Effects & Animations
document.addEventListener('DOMContentLoaded', function () {

    // ========================================
    // SPOTLIGHT EFFECT (Aceternity Style)
    // ========================================
    const spotlight = document.getElementById('spotlight');
    const spotlightContainer = document.querySelector('.spotlight-container');

    if (spotlight && spotlightContainer) {
        spotlightContainer.addEventListener('mousemove', (e) => {
            const rect = spotlightContainer.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            spotlight.style.left = `${x}px`;
            spotlight.style.top = `${y}px`;
            spotlight.style.opacity = '1';
        });

        spotlightContainer.addEventListener('mouseleave', () => {
            spotlight.style.opacity = '0';
        });
    }

    // ========================================
    // INTERSECTION OBSERVER - SCROLL ANIMATIONS
    // ========================================
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-on-scroll');
                // Optionally unobserve after animation
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    // Observe all cards and elements
    document.querySelectorAll('.stat-card, .hover-lift-card, .modern-card, .modern-card-glass').forEach(el => {
        observer.observe(el);
    });

    // ========================================
    // STAGGER ANIMATION FOR STAT CARDS
    // ========================================
    const statCards = document.querySelectorAll('.stat-card');
    statCards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.15}s`;
    });

    // ========================================
    // MAGNETIC HOVER EFFECT (HeroUI Style)
    // ========================================
    document.querySelectorAll('.modern-btn').forEach(button => {
        button.addEventListener('mousemove', (e) => {
            const rect = button.getBoundingClientRect();
            const x = e.clientX - rect.left - rect.width / 2;
            const y = e.clientY - rect.top - rect.height / 2;

            button.style.transform = `translate(${x * 0.1}px, ${y * 0.1}px)`;
        });

        button.addEventListener('mouseleave', () => {
            button.style.transform = 'translate(0, 0)';
        });
    });

    // ========================================
    // 3D TILT EFFECT FOR CARDS (Aceternity Style)
    // Modified: Tilt only works on RIGHT HALF of card, LEFT HALF = NO ANIMATION
    // ========================================
    document.querySelectorAll('.hover-lift-card').forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const centerX = rect.width / 2;
            const centerY = rect.height / 2;

            // Check if mouse is on RIGHT HALF (50% to 100% width)
            if (x >= centerX) {
                // RIGHT HALF: Apply smooth tilt animation
                card.style.transition = 'transform 0.3s ease-out';
                const rotateX = (y - centerY) / 20;
                const rotateY = (centerX - x) / 20;

                card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-8px)`;
            } else {
                // LEFT HALF: NO ANIMATION - Remove all transforms instantly
                card.style.transition = 'none';
                card.style.transform = 'none';
            }
        });

        card.addEventListener('mouseleave', () => {
            card.style.transition = 'transform 0.3s ease-out';
            card.style.transform = 'none';
        });
    });

    // ========================================
    // GRADIENT TEXT SHIMMER
    // ========================================
    const gradientTexts = document.querySelectorAll('.gradient-text');
    gradientTexts.forEach(text => {
        text.classList.add('gradient-text-animated');
    });

    // ========================================
    // SMOOTH SCROLL FOR ANCHOR LINKS
    // ========================================
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            if (href !== '#' && href !== '#!') {
                e.preventDefault();
                const target = document.querySelector(href);
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });

    // ========================================
    // ACTIVE NAVIGATION STATE
    // ========================================
    const currentLocation = window.location.pathname;
    document.querySelectorAll('.nav-link, .modern-sidebar-item').forEach(link => {
        const linkPath = link.getAttribute('href');
        if (linkPath && currentLocation.includes(linkPath) && linkPath !== '/') {
            link.classList.add('active');
        } else if (linkPath === '/' && currentLocation === '/') {
            link.classList.add('active');
        }
    });

    // ========================================
    // PARALLAX EFFECT ON SCROLL
    // ========================================
    let ticking = false;
    window.addEventListener('scroll', () => {
        if (!ticking) {
            window.requestAnimationFrame(() => {
                const scrolled = window.pageYOffset;
                const parallaxElements = document.querySelectorAll('.parallax');

                parallaxElements.forEach(el => {
                    const speed = el.dataset.speed || 0.5;
                    el.style.transform = `translateY(${scrolled * speed}px)`;
                });

                ticking = false;
            });
            ticking = true;
        }
    });

    // ========================================
    // FLOATING ANIMATION FOR ICONS
    // ========================================
    document.querySelectorAll('.stat-card-icon').forEach((icon, index) => {
        icon.style.animation = `float 3s ease-in-out ${index * 0.2}s infinite`;
    });

    // ========================================
    // RIPPLE EFFECT ON BUTTONS
    // ========================================
    document.querySelectorAll('.modern-btn').forEach(button => {
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple-effect');

            this.appendChild(ripple);

            setTimeout(() => ripple.remove(), 600);
        });
    });

    // ========================================
    // NAVBAR SCROLL BEHAVIOR
    // ========================================
    let lastScroll = 0;
    const navbar = document.querySelector('nav');

    window.addEventListener('scroll', () => {
        const currentScroll = window.pageYOffset;

        if (currentScroll > 100) {
            navbar.style.boxShadow = '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)';
        } else {
            navbar.style.boxShadow = '0 1px 3px 0 rgba(0, 0, 0, 0.1)';
        }

        lastScroll = currentScroll;
    });

    // ========================================
    // LOADING ANIMATION COMPLETE
    // ========================================
    setTimeout(() => {
        document.body.classList.add('loaded');
    }, 100);
});
