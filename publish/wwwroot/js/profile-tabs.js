// Profile Tabs Functionality
document.addEventListener('DOMContentLoaded', function() {
    // Tab switching
    const tabBtns = document.querySelectorAll('.profile-tab-btn');
    const tabPanes = document.querySelectorAll('.profile-tab-pane');

    tabBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const targetTab = this.getAttribute('data-tab');

            // Remove active class from all tabs and panes
            tabBtns.forEach(b => b.classList.remove('active'));
            tabPanes.forEach(p => p.classList.remove('active'));

            // Add active class to clicked tab and corresponding pane
            this.classList.add('active');
            document.getElementById('tab-' + targetTab).classList.add('active');

            // Save active tab to localStorage
            localStorage.setItem('activeProfileTab', targetTab);
        });
    });

    // Restore last active tab
    const savedTab = localStorage.getItem('activeProfileTab');
    if (savedTab) {
        const savedBtn = document.querySelector(`[data-tab="${savedTab}"]`);
        if (savedBtn) {
            savedBtn.click();
        }
    }

    // Initialize Quill rich text editor for Bio field
    const bioEditorDiv = document.getElementById('bio-editor');
    if (bioEditorDiv) {
        const bioHidden = document.getElementById('bio-hidden');
        
        const quill = new Quill('#bio-editor', {
            theme: 'snow',
            modules: {
                toolbar: [
                    [{ 'header': [1, 2, 3, false] }],
                    ['bold', 'italic', 'underline'],
                    [{ 'list': 'ordered'}, { 'list': 'bullet' }],
                    ['link'],
                    ['clean']
                ]
            },
            placeholder: 'Tell us about yourself...'
        });

        // Load existing content
        if (bioHidden.value) {
            quill.root.innerHTML = bioHidden.value;
        }

        // Update hidden field on form submit
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            form.addEventListener('submit', function() {
                bioHidden.value = quill.root.innerHTML;
            });
        });

        // Update on text change
        quill.on('text-change', function() {
            bioHidden.value = quill.root.innerHTML;
        });
    }

    // Profile image preview
    const profileImageInput = document.getElementById('profileImage');
    if (profileImageInput) {
        profileImageInput.addEventListener('change', function(e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function(event) {
                    const preview = document.getElementById('profile-preview');
                    if (preview) {
                        preview.src = event.target.result;
                    }
                };
                reader.readAsDataURL(file);
            }
        });
    }

    // Auto-dismiss alerts after 3 seconds
    setTimeout(() => {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(alert => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        });
    }, 3000);
});
