// Home button pressure sensing for 3D Touch simulation
window.homeButtonPressure = {
    element: null,
    dotNetHelper: null,
    isPressing: false,

    initialize: function (element, dotNetHelper) {
        this.element = element;
        this.dotNetHelper = dotNetHelper;

        // Add pointer event listeners
        element.addEventListener('pointerdown', this.handlePointerDown.bind(this));
        element.addEventListener('pointermove', this.handlePointerMove.bind(this));
        element.addEventListener('pointerup', this.handlePointerUp.bind(this));
        element.addEventListener('pointercancel', this.handlePointerUp.bind(this));
        element.addEventListener('pointerleave', this.handlePointerUp.bind(this));
    },

    handlePointerDown: function (event) {
        // Prevent default to avoid text selection and other default behaviors
        event.preventDefault();
        this.isPressing = true;
        this.updatePressure(event);
    },

    handlePointerMove: function (event) {
        if (this.isPressing) {
            // Prevent default to avoid selection during drag
            event.preventDefault();
            this.updatePressure(event);
        }
    },

    handlePointerUp: function (event) {
        // Prevent default behaviors
        event.preventDefault();
        this.isPressing = false;
        // Reset pressure to 0
        if (this.dotNetHelper) {
            this.dotNetHelper.invokeMethodAsync('OnHomeButtonPressure', 0);
        }
    },

    updatePressure: function (event) {
        // Get pressure from PointerEvent (0.0 to 1.0)
        // Note: Only some devices support pressure (like Apple Pencil, some trackpads)
        // For devices without pressure support, we simulate based on pointer type
        let pressure = event.pressure || 0.5;

        // For touch events without pressure support, simulate pressure
        if (event.pointerType === 'touch' && event.pressure === 0.5) {
            // Default touch pressure - could be enhanced with force touch detection
            pressure = 0.5;
        }

        // Send pressure to Blazor
        if (this.dotNetHelper) {
            this.dotNetHelper.invokeMethodAsync('OnHomeButtonPressure', pressure);
        }
    },

    cleanup: function () {
        if (this.element) {
            this.element.removeEventListener('pointerdown', this.handlePointerDown.bind(this));
            this.element.removeEventListener('pointermove', this.handlePointerMove.bind(this));
            this.element.removeEventListener('pointerup', this.handlePointerUp.bind(this));
            this.element.removeEventListener('pointercancel', this.handlePointerUp.bind(this));
            this.element.removeEventListener('pointerleave', this.handlePointerUp.bind(this));
        }
        this.element = null;
        this.dotNetHelper = null;
        this.isPressing = false;
    }
};

// Global functions called from Blazor
window.initializeHomeButtonPressure = function (element, dotNetHelper) {
    window.homeButtonPressure.initialize(element, dotNetHelper);
};

window.cleanupHomeButtonPressure = function () {
    window.homeButtonPressure.cleanup();
};
