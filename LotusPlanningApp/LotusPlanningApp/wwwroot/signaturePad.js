// signaturePad.js - Signature pad helper for Blazor interop

window.signaturePad = (function () {
    const pads = {};

    function getCanvas(canvasId) {
        return document.getElementById(canvasId);
    }

    function init(canvasId) {
        const canvas = getCanvas(canvasId);
        if (!canvas) return;

        const state = {
            drawing: false,
            lastX: 0,
            lastY: 0,
        };
        pads[canvasId] = state;

        const ctx = canvas.getContext('2d');
        ctx.strokeStyle = '#000000';
        ctx.lineWidth = 2;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';

        function getPos(e) {
            const rect = canvas.getBoundingClientRect();
            const scaleX = canvas.width / rect.width;
            const scaleY = canvas.height / rect.height;
            if (e.touches) {
                return {
                    x: (e.touches[0].clientX - rect.left) * scaleX,
                    y: (e.touches[0].clientY - rect.top) * scaleY,
                };
            }
            return {
                x: (e.clientX - rect.left) * scaleX,
                y: (e.clientY - rect.top) * scaleY,
            };
        }

        function startDraw(e) {
            e.preventDefault();
            state.drawing = true;
            const pos = getPos(e);
            state.lastX = pos.x;
            state.lastY = pos.y;
        }

        function draw(e) {
            if (!state.drawing) return;
            e.preventDefault();
            const pos = getPos(e);
            ctx.beginPath();
            ctx.moveTo(state.lastX, state.lastY);
            ctx.lineTo(pos.x, pos.y);
            ctx.stroke();
            state.lastX = pos.x;
            state.lastY = pos.y;
        }

        function stopDraw(e) {
            state.drawing = false;
        }

        canvas.addEventListener('mousedown', startDraw);
        canvas.addEventListener('mousemove', draw);
        canvas.addEventListener('mouseup', stopDraw);
        canvas.addEventListener('mouseleave', stopDraw);
        canvas.addEventListener('touchstart', startDraw, { passive: false });
        canvas.addEventListener('touchmove', draw, { passive: false });
        canvas.addEventListener('touchend', stopDraw);
    }

    function clear(canvasId) {
        const canvas = getCanvas(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);
    }

    function isEmpty(canvasId) {
        const canvas = getCanvas(canvasId);
        if (!canvas) return true;
        const ctx = canvas.getContext('2d');
        const data = ctx.getImageData(0, 0, canvas.width, canvas.height).data;
        return !data.some(v => v !== 0);
    }

    function toDataUrl(canvasId) {
        const canvas = getCanvas(canvasId);
        if (!canvas) return '';
        return canvas.toDataURL('image/png');
    }

    return { init, clear, isEmpty, toDataUrl };
})();
