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
        // Note: Only some devices support pressure (like Apple Pencil, 3D Touch, Force Touch trackpads)
        let pressure = 0;
        
        // Check if pressure is actually supported and available
        // event.pressure is a number from 0.0 to 1.0
        // 0.5 is the default for devices that don't support pressure
        // But we need to check if it's actually changing to know if it's supported
        if (typeof event.pressure === 'number') {
            pressure = event.pressure;
            
            // For devices without pressure support, pressure will always be 0.5
            // We don't want to activate 3D Touch for these devices
            // Only use pressure if it's NOT the default 0.5 OR if it is 0.5 but the device supports pressure
            // The safest approach: use the actual pressure value as-is
            // If device doesn't support pressure, it will be 0.5 and won't reach our 0.5 threshold (we use >0.5)
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
