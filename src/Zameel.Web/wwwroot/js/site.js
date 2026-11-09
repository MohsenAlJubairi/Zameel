// Register PWA Service Worker
if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
        navigator.serviceWorker.register('/sw.js').catch(err => {
            console.warn('SW registration skipped:', err);
        });
    });
}

// Anti-Duplicate Submit (منع الإرسال المكرر)
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function (e) {
            // Check if form is valid if using unobtrusive validation
            if (window.jQuery && jQuery(this).valid && !jQuery(this).valid()) {
                return;
            }

            const submitBtn = this.querySelector('button[type="submit"], input[type="submit"]');
            if (submitBtn && !submitBtn.disabled) {
                submitBtn.disabled = true;
                const originalText = submitBtn.innerHTML || submitBtn.value;
                submitBtn.setAttribute('data-original-text', originalText);
                if (submitBtn.tagName === 'INPUT') {
                    submitBtn.value = 'جاري المعالجة...';
                } else {
                    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span> جاري المعالجة...';
                }
            }
        });
    });

    // Zameel Navigation Dock Micro-Interactions (Fluid Motion & Tactile Feedback)
    const navItems = document.querySelectorAll('.bottom-nav-item, .bottom-nav-add-btn');
    navItems.forEach(item => {
        item.addEventListener('click', function () {
            this.style.transition = 'transform 0.12s cubic-bezier(0.34, 1.56, 0.64, 1)';
            this.style.transform = 'scale(0.9)';
            setTimeout(() => {
                this.style.transform = '';
            }, 180);

            if (this.classList.contains('bottom-nav-item')) {
                document.querySelectorAll('.bottom-nav-item').forEach(el => el.classList.remove('active'));
                this.classList.add('active');
            }
        });
    });
});
