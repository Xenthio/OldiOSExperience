export function setOffset(el, px, requestFrame) {
    if (!el) return;
    const apply = () => {
        el.style.transform = `translate3d(${px}px,0,0)`;
    };
    if (requestFrame) {
        requestAnimationFrame(apply);
    } else {
        apply();
    }
}

function parseTranslateX(el) {
    const style = getComputedStyle(el);
    const t = style.transform;
    if (!t || t === 'none') return 0;
    if (t.startsWith('matrix3d(')) {
        const parts = t.slice(9, -1).split(',').map(Number);
        // 13th index (0-based) is translateX in matrix3d
        return parts[12] || 0;
    }
    if (t.startsWith('matrix(')) {
        const parts = t.slice(7, -1).split(',').map(Number);
        // 5th index (0-based) is translateX in 2d matrix
        return parts[4] || 0;
    }
    return 0;
}

export function animateTo(el, start, end, durationMs, dotNetRef) {
    if (!el) return;
    // cancel any existing animation on this element
    if (el.__sbAnim?.rafId) {
        cancelAnimationFrame(el.__sbAnim.rafId);
    }
    const startTime = performance.now();
    const diff = end - start;
    function easeOutCubic(t) { return 1 - Math.pow(1 - t, 3); }
    const state = { canceled: false, rafId: 0 };
    el.__sbAnim = state;
    function frame(now) {
        if (state.canceled) return; // interrupted
        const elapsed = now - startTime;
        const progress = Math.min(elapsed / durationMs, 1);
        const eased = easeOutCubic(progress);
        const current = start + diff * eased;
        el.style.transform = `translate3d(${current}px,0,0)`;
        if (progress < 1) {
            state.rafId = requestAnimationFrame(frame);
        } else {
            el.__sbAnim = undefined;
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnAnimationCompleted', end);
            }
        }
    }
    state.rafId = requestAnimationFrame(frame);
}

export function cancelAnimation(el) {
    if (!el) return 0;
    const state = el.__sbAnim;
    if (state?.rafId) {
        cancelAnimationFrame(state.rafId);
    }
    if (state) state.canceled = true;
    const x = parseTranslateX(el);
    el.__sbAnim = undefined;
    return x;
}