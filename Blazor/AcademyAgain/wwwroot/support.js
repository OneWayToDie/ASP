(() => {
    const hub = {
        connection: null,
        refs: new Set(),
        isConnecting: null,

        async ensure() {
            if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
                return this.connection;
            }
            if (this.isConnecting) {
                return this.isConnecting;
            }
            if (!this.connection) {
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl('/chatHub')
                    .withAutomaticReconnect()
                    .build();
                this.connection.on('ReceiveMessage', (msg) => this.dispatch('OnSupportMessage', msg));
                this.connection.on('ConversationUpdated', (chatId) => this.dispatch('OnSupportConversationUpdated', chatId));
                this.connection.on('Banned', (chatId) => this.dispatch('OnSupportBanned', chatId));
                this.connection.on('ChatListUpdated', (chatId) => this.dispatch('OnSupportChatUpdated', chatId));
                this.connection.onreconnected(async () => {
                    for (const ref of this.refs) {
                        try {
                            await ref.invokeMethodAsync('OnSupportReconnected');
                        } catch { /* компонент не подписан — игнорируем */ }
                    }
                });
            }
            this.isConnecting = this.connection.start()
                .catch((err) => {
                    if (this.connection && this.connection.state === signalR.HubConnectionState.Disconnected) {
                        this.connection = null;
                    }
                    throw err;
                })
                .finally(() => { this.isConnecting = null; });
            return this.isConnecting;
        },

        dispatch(method, arg) {
            for (const ref of this.refs) {
                try {
                    ref.invokeMethodAsync(method, arg);
                } catch { /* компонент не подписан на событие — игнорируем */ }
            }
        },

        init(ref) {
            this.refs.add(ref);
        },

        async joinChat(chatId) {
            if (!chatId) return;
            await this.ensure();
            await this.connection.invoke('JoinChat', chatId);
        },

        async leaveChat(chatId) {
            if (!chatId || !this.connection) return;
            if (this.connection.state !== signalR.HubConnectionState.Connected) return;
            await this.connection.invoke('LeaveChat', chatId);
        },

        async send(chatId, body) {
            if (!chatId || !body || !body.trim()) return null;
            await this.ensure();
            await this.connection.invoke('JoinChat', chatId);
            return await this.connection.invoke('SendMessage', chatId, body);
        },

        async dispose(ref) {
            if (ref) {
                this.refs.delete(ref);
            }
            if (this.refs.size === 0 && this.connection) {
                try {
                    await this.connection.stop();
                } catch { /* соединение уже закрыто */ }
                this.connection = null;
            }
        },
    };

    window.AcademySupport = hub;

    const widget = {
        key: 'academySupportPos',
        restore() {
            try {
                const saved = localStorage.getItem(this.key);
                if (!saved) return;
                const pos = JSON.parse(saved);
                if (!pos || pos.left == null || pos.top == null) return;
                const el = document.getElementById('supportWidget');
                if (!el) return;
                el.style.left = pos.left + 'px';
                el.style.top = pos.top + 'px';
                el.style.bottom = 'auto';
            } catch { /* хранилище недоступно — оставляем позицию по умолчанию */ }
        },

        positionPanel() {
            const root = document.getElementById('supportWidget');
            const panel = root && root.querySelector('.support-panel');
            const fab = root && root.querySelector('.support-fab');
            if (!root || !panel) return;

            const margin = 8;
            const vw = document.documentElement.clientWidth;
            const vh = document.documentElement.clientHeight;

            panel.style.position = 'fixed';
            panel.style.left = 'auto';
            panel.style.right = 'auto';
            panel.style.top = 'auto';
            panel.style.bottom = 'auto';
            panel.style.width = Math.min(420, vw - margin * 2) + 'px';
            panel.style.bottom = 'auto';

            const pw = panel.offsetWidth || 420;
            let ph = panel.offsetHeight || 1040;
            const maxH = Math.min(1040, vh - margin * 2);
            if (ph > maxH) {
                ph = maxH;
            }

            // Якорь — реальный rect FAB (FAB всегда в DOM, при открытом окне он скрыт, но rect доступен).
            const fabRect = fab
                ? fab.getBoundingClientRect()
                : { left: 178, right: 234, top: vh - 144, bottom: vh - 88 };

            const spaceBelow = vh - fabRect.bottom;
            const spaceAbove = fabRect.top;
            const spaceRight = vw - fabRect.right;
            const spaceLeft = fabRect.left;

            // Вертикаль: вниз, если под FAB хватает места, иначе вверх (вплотную к иконке).
            let top;
            if (spaceBelow >= ph + margin || spaceBelow >= spaceAbove) {
                top = fabRect.bottom + margin;
            } else {
                top = fabRect.top - ph - margin;
            }
            top = Math.max(margin, Math.min(top, vh - ph - margin));

            // Горизонталь: вправо, если хватает места, иначе влево (вплотную к иконке).
            let left;
            if (spaceRight >= pw + margin || spaceRight >= spaceLeft) {
                left = fabRect.right + margin;
            } else {
                left = fabRect.left - pw - margin;
            }
            left = Math.max(margin, Math.min(left, vw - pw - margin));

            panel.style.top = top + 'px';
            panel.style.left = left + 'px';
        },

        clearPosition() {
            const root = document.getElementById('supportWidget');
            const panel = root && root.querySelector('.support-panel');
            if (panel) {
                panel.style.position = '';
                panel.style.left = '';
                panel.style.right = '';
                panel.style.top = '';
                panel.style.bottom = '';
                panel.style.width = '';
            }
        },
    };

    window.AcademySupportWidget = widget;

    // ===== Автопрокрутка чата + чип непрочитанных у скроллбара =====
    const chatScroll = {
        registry: new Map(),

        track(containerId, chipId) {
            const container = document.getElementById(containerId);
            if (!container) return;

            const existing = this.registry.get(containerId);
            if (existing && existing.container === container) return;
            if (existing && existing.observer) {
                existing.observer.disconnect();
                if (existing.chip) existing.chip.style.display = 'none';
            }

            const chip = document.getElementById(chipId);
            const seen = new Set();
            let atBottom = true;
            let pending = 0;

            const isAtBottom = () =>
                container.scrollTop + container.clientHeight >= container.scrollHeight - 24;

            const updateChip = () => {
                if (!chip) return;
                if (pending > 0) {
                    chip.textContent = String(pending);
                    chip.style.display = 'flex';
                } else {
                    chip.style.display = 'none';
                }
            };

            const scrollToBottom = () => {
                container.scrollTop = container.scrollHeight;
                atBottom = true;
                if (pending > 0) {
                    pending = 0;
                    updateChip();
                }
            };

            container.addEventListener('scroll', () => {
                atBottom = isAtBottom();
                if (atBottom && pending > 0) {
                    pending = 0;
                    updateChip();
                }
            }, { passive: true });

            if (chip) {
                chip.addEventListener('click', () => scrollToBottom());
            }

            container.querySelectorAll('.support-msg[data-msg-id]').forEach((el) => seen.add(el.dataset.msgId));

            const observer = new MutationObserver((mutations) => {
                let added = 0;
                for (const m of mutations) {
                    for (const node of m.addedNodes) {
                        if (node.nodeType !== Node.ELEMENT_NODE) continue;
                        const candidates = node.matches && node.matches('.support-msg[data-msg-id]')
                            ? [node]
                            : (node.querySelectorAll ? Array.from(node.querySelectorAll('.support-msg[data-msg-id]')) : []);
                        for (const el of candidates) {
                            if (!seen.has(el.dataset.msgId)) {
                                seen.add(el.dataset.msgId);
                                added++;
                            }
                        }
                    }
                }
                if (!added) return;
                if (atBottom) {
                    scrollToBottom();
                } else {
                    pending += added;
                    updateChip();
                }
            });
            observer.observe(container, { childList: true, subtree: true });

            this.registry.set(containerId, { container, chip, observer, seen });
            scrollToBottom();
        },
    };

    widget.chatTrack = (containerId, chipId) => chatScroll.track(containerId, chipId);

    let dragState = null;

    document.addEventListener('pointerdown', (e) => {
        if (dragState || e.button !== 0) return;
        const handle = e.target.closest('.support-drag');
        const root = document.getElementById('supportWidget');
        if (!handle || !root) return;

        const panel = root.querySelector('.support-panel');
        const movingPanel = !!(panel && panel.style.position === 'fixed');
        const el = movingPanel ? panel : root;
        const rect = el.getBoundingClientRect();

        dragState = {
            handle,
            pointerId: e.pointerId,
            x: e.clientX,
            y: e.clientY,
            startLeft: rect.left,
            startTop: rect.top,
            left: rect.left,
            top: rect.top,
            mode: movingPanel ? 'panel' : 'root',
            moved: false,
            captured: false,
            raf: 0,
        };
        document.addEventListener('pointermove', onDragMove);
        document.addEventListener('pointerup', onDragEnd);
        document.addEventListener('pointercancel', onDragEnd);
    });

    function onDragMove(e) {
        if (!dragState || e.pointerId !== dragState.pointerId) return;
        const dx = e.clientX - dragState.x;
        const dy = e.clientY - dragState.y;
        if (!dragState.moved && Math.hypot(dx, dy) > 5) {
            dragState.moved = true;
            dragState.captured = true;
            try {
                dragState.handle.setPointerCapture(e.pointerId);
            } catch { /* элемент уже отпущен — игнорируем */ }
            if (e.cancelable) e.preventDefault();
        }
        dragState.left = dragState.startLeft + dx;
        dragState.top = dragState.startTop + dy;
        if (!dragState.raf) {
            dragState.raf = requestAnimationFrame(applyDrag);
        }
    }

    function applyDrag() {
        if (!dragState) return;
        dragState.raf = 0;
        const root = document.getElementById('supportWidget');
        if (!root) return;
        const margin = 8;
        const vw = document.documentElement.clientWidth;
        const vh = document.documentElement.clientHeight;
        const el = dragState.mode === 'panel' ? root.querySelector('.support-panel') : root;
        if (!el) return;
        const w = el.offsetWidth || 420;
        const h = el.offsetHeight || 200;
        const left = Math.max(margin, Math.min(dragState.left, vw - w - margin));
        const top = Math.max(margin, Math.min(dragState.top, vh - h - margin));
        if (dragState.mode === 'panel') {
            el.style.left = left + 'px';
            el.style.top = top + 'px';
        } else {
            root.style.left = left + 'px';
            root.style.top = top + 'px';
            root.style.bottom = 'auto';
        }
    }

    function onDragEnd(e) {
        if (!dragState || e.pointerId !== dragState.pointerId) return;
        document.removeEventListener('pointermove', onDragMove);
        document.removeEventListener('pointerup', onDragEnd);
        document.removeEventListener('pointercancel', onDragEnd);
        if (dragState.captured) {
            try {
                dragState.handle.releasePointerCapture(e.pointerId);
            } catch { /* элемент уже отпущен — игнорируем */ }
        }
        if (dragState.raf) {
            cancelAnimationFrame(dragState.raf);
            dragState.raf = 0;
            applyDrag();
        }
        if (dragState.moved) {
            const root = document.getElementById('supportWidget');
            if (root) {
                root.dataset.dragged = '1';
                if (dragState.mode === 'root') {
                    const vw = document.documentElement.clientWidth;
                    const vh = document.documentElement.clientHeight;
                    const left = Math.max(8, Math.min(dragState.left, vw - root.offsetWidth - 8));
                    const top = Math.max(8, Math.min(dragState.top, vh - root.offsetHeight - 8));
                    try {
                        localStorage.setItem(widget.key, JSON.stringify({ left, top }));
                    } catch { /* хранилище недоступно — позиция не сохраняется */ }
                }
            }
        }
        dragState = null;
    }

    document.addEventListener('click', (e) => {
        const root = document.getElementById('supportWidget');
        if (!root || root.dataset.dragged !== '1') return;
        delete root.dataset.dragged;
        e.preventDefault();
        e.stopImmediatePropagation();
    }, true);
})();