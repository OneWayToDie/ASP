window.nightfallAudio = (function () {
    let audio = null;
    let audioCtx = null;
    let analyser = null;
    let dataArray = null;
    let timeData = null;
    let buffers = null;
    const VOL_KEY = 'nf-volume';

    function ensureContext() {
        if (audioCtx) return;
        audioCtx = new (window.AudioContext || window.webkitAudioContext)();
        analyser = audioCtx.createAnalyser();
        analyser.fftSize = 512;
        analyser.smoothingTimeConstant = 0.7;
        dataArray = new Uint8Array(analyser.frequencyBinCount);
        timeData = new Uint8Array(analyser.fftSize);
        buffers = null;
    }

    function connect(audioEl) {
        audio = audioEl;
        ensureContext();
        if (!buffers) {
            buffers = audioCtx.createMediaElementSource(audio);
        }
        buffers.connect(analyser);
        analyser.connect(audioCtx.destination);
        // expose analyser for visualizer
        window.nightfallAudioVisualizerAnalyser = analyser;
    }

    async function playTrack(src) {
        if (!audio) return;
        const origin = window.location.origin;
        const fullSrc = (src.indexOf('http') === 0) ? src : origin + src;
        if (audio.src !== fullSrc) {
            audio.src = src;
        }
        if (audioCtx && audioCtx.state === 'suspended') {
            try { await audioCtx.resume(); } catch (e) {}
        }
        try { await audio.play(); } catch (e) {}
    }

    function pause() {
        if (audio) audio.pause();
    }

    function resume() {
        if (audio) audio.play().catch(() => {});
    }

    function seek(seconds) {
        if (audio && isFinite(audio.duration)) {
            audio.currentTime = seconds;
        }
    }

    function loadVolume() {
        try {
            var v = localStorage.getItem(VOL_KEY);
            if (v !== null) {
                var n = parseFloat(v);
                if (!isNaN(n)) return Math.max(0, Math.min(1, n));
            }
        } catch (_) {}
        return 1.0;
    }

    function saveVolume(v) {
        try { localStorage.setItem(VOL_KEY, String(v)); } catch (_) {}
    }

    return {
        init(hasDtRef) {
            const el = document.getElementById('nightfall-audio');
            if (!el) return;
            try {
                connect(el);
            } catch (e) {
                try { console.error('nightfallAudio.connect failed', e); } catch (_) {}
            }
            window._nightfallAudioRef = hasDtRef || window._nightfallAudioRef;
            el.addEventListener('ended', function () {
                if (window._nightfallAudioRef) window._nightfallAudioRef.invokeMethodAsync('OnEnded');
            });
            el.addEventListener('timeupdate', function () {
                if (window._nightfallAudioRef) window._nightfallAudioRef.invokeMethodAsync('OnTimeUpdate', el.currentTime, el.duration);
            });
            el.addEventListener('play', function () {
                if (window._nightfallAudioRef) window._nightfallAudioRef.invokeMethodAsync('OnPlayingChanged', true);
            });
            el.addEventListener('pause', function () {
                if (window._nightfallAudioRef) window._nightfallAudioRef.invokeMethodAsync('OnPlayingChanged', false);
            });
            audio.volume = loadVolume();
        },
        dispose() {
            window._nightfallAudioRef = null;
            const el = document.getElementById('nightfall-audio');
            if (el) {
                el.pause();
                try {
                    el.src = '';
                    el.load();
                } catch (e) {}
            }
        },
        playTrack,
        pause,
        resume,
        seek,
        setVolume(v) {
            var clamped = Math.max(0, Math.min(1, v));
            if (audio) audio.volume = clamped;
            saveVolume(clamped);
        },
        getVolume() { return audio ? audio.volume : loadVolume(); },
        setVolumeStyle(el, pct) {
            var p = Math.max(0, Math.min(100, pct));
            var trackColor = 'rgba(255,255,255,0.08)';
            var fillColor = '#ffffff';
            el.style.background = 'linear-gradient(to right, ' + fillColor + ' 0%, ' + fillColor + ' ' + p + '%, ' + trackColor + ' ' + p + '%)';
        },
        isInit() { return audio != null; }
    };
})();

// --- Task completion exit animation (fly right + fade, then height collapse) ---
window.nightfallTaskAnim = (function () {
    function resolve(el) {
        if (typeof el === 'string') return document.getElementById(el);
        return el;
    }
    function animateAndRemove(el, dtRef, onDone, direction) {
        el = resolve(el);
        if (!el) { if (dtRef) dtRef.invokeMethodAsync(onDone || 'OnAnimComplete'); return; }
        el.classList.add(direction === 'left' ? 'task-leaving-left' : 'task-leaving');
        el.style.pointerEvents = 'none';
        setTimeout(function () {
            var h = el.offsetHeight;
            el.style.height = h + 'px';
            el.style.transition = 'height .35s ease, margin .2s ease, padding .2s ease, opacity .3s ease';
            requestAnimationFrame(function () {
                el.style.height = '0px';
                el.style.margin = '0';
                el.style.padding = '0';
                el.style.overflow = 'hidden';
            });
            setTimeout(function () {
                if (dtRef) dtRef.invokeMethodAsync(onDone || 'OnAnimComplete');
            }, 360);
        }, 450);
    }
    return { animateAndRemove: animateAndRemove };
})();

// --- Canvas visualizer (single global analyser used by any canvas) ---
window.nightfallVisualizer = (function () {
    let canvas = null;
    let ctx = null;
    let rafId = null;
    let analyser = null;
    let dataArray = null;
    let timeData = null;
    let bars = 64;
    let color = '#ffffff';

    function getAnalyser() {
        // The analyser lives in window.nightfallAudio module; grab it if created
        return window.nightfallAudioVisualizerAnalyser || null;
    }

    return {
        attach(canvasEl, barCount, colorHex) {
            let el;
            if (canvasEl) {
                // best-effort: Blazor ElementReference -> resolve by id if it has one
                if (typeof canvasEl === 'object' && canvasEl.id) {
                    el = document.getElementById(canvasEl.id);
                } else if (typeof canvasEl === 'string') {
                    el = document.getElementById(canvasEl);
                } else if (canvasEl.tagName) {
                    el = canvasEl;
                }
            }
            if (!el) el = document.getElementById('player-canvas');
            if (!el) return;
            canvas = el;
            ctx = canvas.getContext('2d');
            bars = barCount || 64;
            color = colorHex || '#ffffff';
            analyser = window.nightfallAudioVisualizerAnalyser || null;
            if (analyser) {
                dataArray = new Uint8Array(analyser.frequencyBinCount);
                timeData = new Uint8Array(analyser.fftSize);
            }
            // size canvas to its display size
            const rect = canvas.getBoundingClientRect();
            canvas.width = rect.width || canvas.clientWidth || 800;
            canvas.height = rect.height || canvas.clientHeight || 300;
            this.start();
        },
        start() {
            if (rafId) cancelAnimationFrame(rafId);
            const loop = () => {
                draw();
                rafId = requestAnimationFrame(loop);
            };
            loop();
        },
        stop() {
            if (rafId) { cancelAnimationFrame(rafId); rafId = null; }
        },
        resize(w, h) {
            if (canvas) {
                canvas.width = w;
                canvas.height = h;
            }
        }
    };

    function draw() {
        if (!canvas || !ctx) return;
        // (re)acquire analyser lazily in case audio wasn't initialized yet
        if (!analyser) analyser = window.nightfallAudioVisualizerAnalyser || null;
        if (!analyser) return;

        const w = canvas.width;
        const h = canvas.height;
        if (w === 0 || h === 0) return;

        if (!dataArray) dataArray = new Uint8Array(analyser.frequencyBinCount);
        if (!timeData) timeData = new Uint8Array(analyser.fftSize);

        ctx.clearRect(0, 0, w, h);

        analyser.getByteFrequencyData(dataArray);
        analyser.getByteTimeDomainData(timeData);

        // Spectrum bars
        const n = bars;
        const barW = w / n;
        for (let i = 0; i < n; i++) {
            const idx = Math.floor((i / n) * dataArray.length);
            const v = dataArray[idx] / 255;
            const barH = Math.max(2, v * h * 0.9);
            const x = i * barW;
            const y = h - barH;
            const grad = ctx.createLinearGradient(0, y, 0, h);
            grad.addColorStop(0, color);
            grad.addColorStop(1, 'rgba(255,255,255,0.15)');
            ctx.fillStyle = grad;
            ctx.globalAlpha = 0.9;
            ctx.fillRect(x + 1, y, barW - 2, barH);
        }

        // Oscilloscope line overlay
        ctx.globalAlpha = 1;
        ctx.strokeStyle = color;
        ctx.lineWidth = 2;
        ctx.beginPath();
        const step = w / timeData.length;
        for (let i = 0; i < timeData.length; i++) {
            const v = (timeData[i] / 128 - 1) * -1; // 0..255 -> -1..1
            const x = i * step;
            const y = h / 2 + v * (h / 2 - 10);
            if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
        }
        ctx.stroke();
        ctx.shadowBlur = 0;
    }
})();

// --- Persistence layer (localStorage) ---
window.nightfallPersist = (function () {
    const PREFIX = 'nf-';
    function load(key, fallback) {
        try {
            const raw = localStorage.getItem(PREFIX + key);
            if (raw === null) return fallback !== undefined ? fallback : null;
            return JSON.parse(raw);
        } catch (e) { return fallback !== undefined ? fallback : null; }
    }
    function save(key, value) {
        try { localStorage.setItem(PREFIX + key, JSON.stringify(value)); }
        catch (e) { /* storage full or unavailable */ }
    }
    function remove(key) {
        try { localStorage.removeItem(PREFIX + key); }
        catch (e) { }
    }
    return { load: load, save: save, remove: remove };
})();
