// --- Dynamic genre wallpaper / slideshow engine ---
// Wallpaper sources live in wwwroot/wallpapers/<genre>/ only.
// Covers inside music are NEVER used as background.
window.nightfallTheme = (function () {
    const INTERVAL_MS = 18000; // slideshow, 15-20s range
    const CSS_VAR = '--bg-cover';

    let genre = null;
    let slides = [];
    let index = 0;
    let armed = false;   // true after first play in current session
    let frozen = false;  // manual pin/freeze, independent of music
    let timer = null;

    function apply() {
        const url = slides[index];
        if (!url) {
            document.documentElement.style.removeProperty(CSS_VAR);
            document.documentElement.classList.remove('has-cover');
            return;
        }
        document.documentElement.style.setProperty(CSS_VAR, 'url("' + url + '")');
        document.documentElement.classList.add('has-cover');
    }

    function stopTimer() {
        if (timer) { clearInterval(timer); timer = null; }
    }

    function startTimer() {
        stopTimer();
        if (frozen || !armed || slides.length < 2) return;
        timer = setInterval(function () {
            index = (index + 1) % slides.length;
            apply();
        }, INTERVAL_MS);
    }

    return {
        // Preload slide list for a genre. If already armed (music played),
        // applies the first slide and (re)starts the slideshow timer.
        // A genre without wallpapers falls back to the default cover (panda).
        setSlides(g, urls) {
            genre = g;
            slides = (urls || []).slice();
            index = 0;
            frozen = false;
            if (!slides.length) {
                stopTimer();
                document.documentElement.style.removeProperty(CSS_VAR);
                return;
            }
            if (armed) {
                apply();
                startTimer();
            }
        },
        // Called when playback starts: reveal the wallpaper ("panda until music").
        play() {
            armed = true;
            if (!slides.length) return;
            apply();
            startTimer();
        },
        // Called on music pause: keep the last shown slide, stop the timer.
        pause() {
            stopTimer();
        },
        resume() {
            if (!slides.length) return;
            armed = true;
            startTimer();
        },
        // Manual freeze: stop the timer, keep the pinned slide (gif keeps moving).
        freeze() {
            frozen = true;
            stopTimer();
        },
        unfreeze() {
            frozen = false;
            startTimer();
        },
        // Show + freeze on the specific slide (used by Settings thumbnails).
        pin(i) {
            if (!slides.length || i < 0 || i >= slides.length) return;
            index = i;
            frozen = true;
            stopTimer();
            apply();
        },
        show(i) {
            if (!slides.length || i < 0 || i >= slides.length) return;
            index = i;
            apply();
        },
        state() {
            return { genre: genre, count: slides.length, index: index, frozen: frozen, armed: armed };
        },
        // Toggle the vinyl player theme (hides the loud fullscreen cover).
        setTheme(theme) {
            document.documentElement.classList.toggle('theme-vinyl', theme === 'vinyl');
        }
    };
})();