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
        
        // Per Mozilla docs, pressure behavior:
        // - Non-pressure devices (mouse, standard touch): exactly 0 or 0.5
        //   * 0 when not pressed
        //   * 0.5 when pressed (active buttons state)
        // - Pressure-sensitive devices: continuous values (0.01-0.49, 0.51-1.0)
        //   * Can report any value between 0 and 1 based on applied force
        if (typeof event.pressure === 'number') {
            pressure = event.pressure;
        }

        // Send pressure to Blazor
        // C# code detects pressure support by finding values != 0 and != 0.5
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
