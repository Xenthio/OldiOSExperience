// This object will manage the battery API interaction
window.batteryManager = {
    dotNetHelper: null,
    battery: null,
    boundSendUpdate: null, // Store the bound function

    // Called from Blazor to start monitoring
    start: function (dotNetHelper) {
        this.dotNetHelper = dotNetHelper;
        // Create a version of sendUpdate that is permanently bound to 'this'
        this.boundSendUpdate = this.sendUpdate.bind(this);

        if ('getBattery' in navigator) {
            navigator.getBattery().then(battery => {
                // --- FIX: GUARD CLAUSE ---
                // Check if the received object has the properties we expect.
                // If not, it's not a valid BatteryManager object.
                if (typeof battery.level === 'undefined' || typeof battery.charging === 'undefined') {
                    console.error("Battery API Error: The browser returned an unexpected object instead of a BatteryManager. This might be caused by a browser extension or a non-standard browser.", battery);
                    // Stop execution to prevent a crash.
                    return;
                }

                this.battery = battery;

                // Send the initial status
                this.boundSendUpdate();

                // Use the bound function for event handlers
                this.battery.onchargingchange = this.boundSendUpdate;
                this.battery.onlevelchange = this.boundSendUpdate;
            });
        } else {
            console.warn("Battery Status API is not supported by this browser.");
        }
    },

    // Sends the current battery status to the Blazor component
    sendUpdate: function () {
        // 'this' will now always be batteryManager
        if (this.dotNetHelper && this.battery) {
            // This call should now have the correct boolean and number values
            this.dotNetHelper.invokeMethodAsync('UpdateBatteryStatus', this.battery.charging, this.battery.level);
        }
    },

    // Called from Blazor when the component is disposed
    stop: function () {
        // To stop listening, we just set the event handlers to null.
        if (this.battery) {
            this.battery.onchargingchange = null;
            this.battery.onlevelchange = null;
        }
        this.dotNetHelper = null;
        this.battery = null;
        this.boundSendUpdate = null;
    }
};