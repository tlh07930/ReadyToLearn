// Lightweight Web Speech API helper.
// Automatically wires elements with `data-tts` and `data-tts-trigger`.
// data-tts: text to speak
// data-tts-trigger: space-separated tokens: "click", "hover", "focus"
// data-tts-spell: if present (any value), spell the text as separate letters (useful for acronyms)
// data-tts-spell-delay: optional integer (ms) to override default delay between letters (default 0 = immediate)
(function () {
    const Tts = {
        _utterance: null,
        _voice: null,
        _timeouts: [],
        supports: function () {
            return 'speechSynthesis' in window && typeof SpeechSynthesisUtterance === 'function';
        },
        _pickVoice: function () {
            if (!this.supports()) return null;
            const voices = window.speechSynthesis.getVoices();
            const enVoice = voices.find(v => v.lang && v.lang.toLowerCase().startsWith('en-us'));
            return enVoice || voices[0] || null;
        },
        _clearScheduled: function () {
            this._timeouts.forEach(t => clearTimeout(t));
            this._timeouts = [];
        },
        speak: function (text, options = {}) {
            if (!this.supports() || !text) return;
            this._clearScheduled();
            this.stop();
            this._utterance = new SpeechSynthesisUtterance(text);
            this._utterance.lang = options.lang || 'en-US';
            this._utterance.rate = options.rate || 1.0;
            this._utterance.pitch = options.pitch || 1.0;
            this._utterance.volume = typeof options.volume === 'number' ? options.volume : 1.0;
            const setVoiceLater = () => {
                const v = this._pickVoice();
                if (v) this._utterance.voice = v;
            };
            setVoiceLater();
            if (speechSynthesis.getVoices().length === 0) {
                window.speechSynthesis.onvoiceschanged = () => {
                    setVoiceLater();
                };
            }
            window.speechSynthesis.speak(this._utterance);
        },
        // Spell text by queuing each letter/digit as its own utterance immediately (no artificial delay).
        // If a non-zero delay is provided via options.delayBetween, fall back to timed scheduling.
        speakSpell: function (text, options = {}) {
            if (!this.supports() || !text) return;
            this._clearScheduled();
            this.stop();
            // Extract letters and digits
            const chars = [];
            for (let ch of text) {
                if (/[A-Za-z0-9]/.test(ch)) chars.push(ch.toUpperCase());
            }
            if (chars.length === 0) return;
            const delayBetween = typeof options.delayBetween === 'number' ? options.delayBetween : 0; // default 0 = immediate queue
            const voice = this._pickVoice();

            if (delayBetween === 0) {
                // Queue utterances immediately; the browser will play them in order.
                chars.forEach(ch => {
                    const u = new SpeechSynthesisUtterance(ch);
                    u.lang = options.lang || 'en-US';
                    u.rate = options.rate || 1.0;
                    u.pitch = options.pitch || 1.0;
                    if (voice) u.voice = voice;
                    window.speechSynthesis.speak(u);
                });
            } else {
                // Fall back to scheduled utterances when a pause is required.
                chars.forEach((ch, idx) => {
                    const t = setTimeout(() => {
                        const u = new SpeechSynthesisUtterance(ch);
                        u.lang = options.lang || 'en-US';
                        u.rate = options.rate || 1.0;
                        u.pitch = options.pitch || 1.0;
                        if (voice) u.voice = voice;
                        window.speechSynthesis.speak(u);
                    }, idx * delayBetween);
                    this._timeouts.push(t);
                });
            }
        },
        stop: function () {
            if (!this.supports()) return;
            this._clearScheduled();
            window.speechSynthesis.cancel();
            this._utterance = null;
        }
    };

    // Expose API
    window.tts = {
        speak: (text, options) => Tts.speak(text, options),
        speakSpell: (text, options) => Tts.speakSpell(text, options),
        stop: () => Tts.stop(),
        supports: () => Tts.supports()
    };

    // Auto-wire elements with data-tts
    function attachHandlers() {
        if (!document.querySelectorAll) return;
        document.querySelectorAll('[data-tts]').forEach(el => {
            const text = el.getAttribute('data-tts') || el.textContent || '';
            const triggers = (el.getAttribute('data-tts-trigger') || '').toLowerCase().split(/\s+/);
            const spell = el.hasAttribute('data-tts-spell');
            const spellDelayAttr = el.getAttribute('data-tts-spell-delay');
            const parsedDelay = spellDelayAttr ? parseInt(spellDelayAttr, 10) : undefined;
            const speakFn = () => {
                if (spell) window.tts.speakSpell(text, { delayBetween: parsedDelay });
                else window.tts.speak(text);
            };
            const stopFn = () => window.tts.stop();

            if (triggers.includes('hover')) {
                el.addEventListener('mouseenter', speakFn);
                el.addEventListener('mouseleave', stopFn);
                el.addEventListener('touchstart', speakFn);
                el.addEventListener('touchend', stopFn);
            }
            if (triggers.includes('focus')) {
                el.addEventListener('focus', speakFn);
                el.addEventListener('blur', stopFn);
            }
            if (triggers.includes('click')) {
                el.addEventListener('click', (e) => {
                    speakFn();
                });
            }
        });
    }

    document.addEventListener('DOMContentLoaded', attachHandlers);
})();