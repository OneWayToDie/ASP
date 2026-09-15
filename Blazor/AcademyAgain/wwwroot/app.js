(() => {
    const originals = new WeakMap();

    const rowMenu = {
        ref: null,
        register(dotNetRef) {
            this.ref = dotNetRef;
        },
        unregister() {
            this.ref = null;
        },
        rowKey(target) {
            const row = target && target.closest ? target.closest('table.context-table tr') : null;
            if (!row) return null;
            const cell = row.querySelector('[data-item-key]');
            return cell ? cell.getAttribute('data-item-key') : null;
        },
    };

    window.AcademyRowMenu = rowMenu;

    window.AcademySearch = {
        key: 'academySearchHistory',
        getHistory() {
            try {
                const raw = localStorage.getItem(this.key);
                const arr = raw ? JSON.parse(raw) : [];
                return Array.isArray(arr) ? arr.filter((x) => typeof x === 'string' && x.trim()) : [];
            } catch {
                return [];
            }
        },
        setHistory(items) {
            try {
                const clean = Array.isArray(items) ? items.filter((x) => typeof x === 'string' && x.trim()).slice(0, 10) : [];
                localStorage.setItem(this.key, JSON.stringify(clean));
            } catch { /* хранилище недоступно — игнорируем */ }
        },
        clearHistory() {
            try {
                localStorage.removeItem(this.key);
            } catch { /* хранилище недоступно — игнорируем */ }
        },
    };

    window.downloadTextFile = function (filename, text) {
        const blob = new Blob([text], { type: 'text/csv;charset=utf-8' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        setTimeout(() => URL.revokeObjectURL(url), 1000);
    };

    document.addEventListener('contextmenu', (event) => {
        if (!rowMenu.ref) return;
        const key = rowMenu.rowKey(event.target);
        if (!key || event.target.closest('.row-menu')) return;
        event.preventDefault();
        rowMenu.ref.invokeMethodAsync('RowContextMenu', key, event.clientX, event.clientY);
    });

    document.addEventListener('click', (event) => {
        if (!rowMenu.ref) return;
        if (event.target.closest('.row-menu, .row-menu-backdrop')) return;
        const key = rowMenu.rowKey(event.target);
        if (key) {
            rowMenu.ref.invokeMethodAsync('RowSelect', key);
        }
    });

    document.addEventListener('change', (event) => {
        const target = event.target;

        if (target && target.type === 'file' && target.getAttribute && target.getAttribute('data-photo-input')) {
            const preview = document.getElementById(target.getAttribute('data-photo-input'));
            if (!preview) return;

            if (!originals.has(preview)) {
                originals.set(preview, preview.getAttribute('src') || '');
            }

            const file = target.files && target.files[0];
            if (file) {
                preview.src = URL.createObjectURL(file);
                preview.style.display = 'inline-block';
            } else {
                preview.src = originals.get(preview) || '';
                preview.style.display = preview.getAttribute('src') ? 'inline-block' : 'none';
            }
            return;
        }

        if (target && target.id === 'removePhoto') {
            const form = target.closest('form');
            const fileInput = form && form.querySelector('input[data-photo-input]');
            const preview = fileInput && document.getElementById(fileInput.getAttribute('data-photo-input'));
            if (!preview) return;

            if (target.checked) {
                preview.src = '';
                preview.style.display = 'none';
            } else if (originals.has(preview)) {
                preview.src = originals.get(preview);
                preview.style.display = originals.get(preview) ? 'inline-block' : 'none';
            }
        }
    });

    document.addEventListener('mousemove', (event) => {
        const page = document.querySelector('[data-notfound]');
        const pupil = page && page.querySelector('.eye-pupil');
        if (!pupil) return;

        const eye = pupil.closest('.eye-eye');
        if (!eye) return;

        const rect = eye.getBoundingClientRect();
        const centerX = rect.left + rect.width / 2;
        const centerY = rect.top + rect.height / 2;

        const dx = event.clientX - centerX;
        const dy = event.clientY - centerY;

        const maxShift = rect.width * 0.16;
        const dist = Math.hypot(dx, dy);
        const factor = dist > 0 ? Math.min(1, maxShift / dist) : 0;

        pupil.style.transform =
            `translate(${dx * factor}px, ${dy * factor}px)`;
    });
})();