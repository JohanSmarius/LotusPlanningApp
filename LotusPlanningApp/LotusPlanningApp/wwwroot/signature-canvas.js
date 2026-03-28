/**
 * Signature canvas utilities for the CompleteAssignment page.
 * Supports both touch (finger/stylus) and mouse input.
 */

/**
 * Initialises the signature canvas for drawing.
 * @param {string} canvasId - The id of the <canvas> element.
 * @param {string|null} existingDataUrl - Optional existing signature to pre-load.
 */
window.initSignatureCanvas = function (canvasId, existingDataUrl) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    let drawing = false;
    let lastX = 0;
    let lastY = 0;

    function getPos(e) {
        const rect = canvas.getBoundingClientRect();
        const scaleX = canvas.width / rect.width;
        const scaleY = canvas.height / rect.height;

        if (e.touches && e.touches.length > 0) {
            return {
                x: (e.touches[0].clientX - rect.left) * scaleX,
                y: (e.touches[0].clientY - rect.top) * scaleY
            };
        }
        return {
            x: (e.clientX - rect.left) * scaleX,
            y: (e.clientY - rect.top) * scaleY
        };
    }

    function startDraw(e) {
        e.preventDefault();
        drawing = true;
        const pos = getPos(e);
        lastX = pos.x;
        lastY = pos.y;
    }

    function draw(e) {
        if (!drawing) return;
        e.preventDefault();
        const pos = getPos(e);

        ctx.beginPath();
        ctx.moveTo(lastX, lastY);
        ctx.lineTo(pos.x, pos.y);
        ctx.strokeStyle = '#000';
        ctx.lineWidth = 2;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.stroke();

        lastX = pos.x;
        lastY = pos.y;
    }

    function stopDraw() {
        drawing = false;
    }

    // Remove any previously registered listeners stored on the element
    if (canvas._signatureListeners) {
        const l = canvas._signatureListeners;
        canvas.removeEventListener('mousedown', l.startDraw);
        canvas.removeEventListener('mousemove', l.draw);
        canvas.removeEventListener('mouseup', l.stopDraw);
        canvas.removeEventListener('mouseleave', l.stopDraw);
        canvas.removeEventListener('touchstart', l.startDraw);
        canvas.removeEventListener('touchmove', l.draw);
        canvas.removeEventListener('touchend', l.stopDraw);
    }

    // Store references so they can be removed on re-init
    canvas._signatureListeners = { startDraw, draw, stopDraw };

    // Mouse events
    canvas.addEventListener('mousedown', startDraw);
    canvas.addEventListener('mousemove', draw);
    canvas.addEventListener('mouseup', stopDraw);
    canvas.addEventListener('mouseleave', stopDraw);

    // Touch events
    canvas.addEventListener('touchstart', startDraw, { passive: false });
    canvas.addEventListener('touchmove', draw, { passive: false });
    canvas.addEventListener('touchend', stopDraw);

    // Load existing signature if provided
    if (existingDataUrl && existingDataUrl.startsWith('data:image')) {
        const img = new Image();
        img.onload = function () {
            ctx.drawImage(img, 0, 0);
        };
        img.src = existingDataUrl;
    }
};

/**
 * Returns the current canvas content as a PNG data URL.
 * Returns 'data:,' (empty) when nothing has been drawn.
 * @param {string} canvasId
 * @returns {string}
 */
window.getSignatureDataUrl = function (canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return 'data:,';

    // Check if the canvas is blank
    const ctx = canvas.getContext('2d');
    const pixels = ctx.getImageData(0, 0, canvas.width, canvas.height).data;
    const isBlank = pixels.every(p => p === 0);
    if (isBlank) return 'data:,';

    return canvas.toDataURL('image/png');
};

/**
 * Clears the signature canvas.
 * @param {string} canvasId
 */
window.clearSignatureCanvas = function (canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
};


/**
 * Returns the current canvas content as a PNG data URL.
 * Returns 'data:,' (empty) when nothing has been drawn.
 * @param {string} canvasId
 * @returns {string}
 */
window.getSignatureDataUrl = function (canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return 'data:,';

    // Check if the canvas is blank
    const ctx = canvas.getContext('2d');
    const pixels = ctx.getImageData(0, 0, canvas.width, canvas.height).data;
    const isBlank = pixels.every(p => p === 0);
    if (isBlank) return 'data:,';

    return canvas.toDataURL('image/png');
};

/**
 * Clears the signature canvas.
 * @param {string} canvasId
 */
window.clearSignatureCanvas = function (canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
};
