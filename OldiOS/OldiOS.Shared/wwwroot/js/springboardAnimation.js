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

export function animateTo(el, start, end, durationMs, dotNetRef) {
    if (!el) return;
    const startTime = performance.now();
    const diff = end - start;
    function easeOutCubic(t) { return 1 - Math.pow(1 - t, 3); }
    function frame(now) {
        const elapsed = now - startTime;
        const progress = Math.min(elapsed / durationMs, 1);
        const eased = easeOutCubic(progress);
        const current = start + diff * eased;
        el.style.transform = `translate3d(${current}px,0,0)`;
        if (progress < 1) {
            requestAnimationFrame(frame);
        } else {
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnAnimationCompleted', end);
            }
        }
    }
    requestAnimationFrame(frame);
}