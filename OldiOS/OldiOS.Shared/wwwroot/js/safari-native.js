// Safari Native WebView Interop for MAUI platforms
// Handles native WebView integration in Blazor Hybrid apps

window.safariNativeInterop = {
    webView: null,
    dotNetRef: null,
    containerId: null,

    // Initialize the native WebView container
    initialize: function(containerId, dotNetReference) {
        this.containerId = containerId;
        this.dotNetRef = dotNetReference;
        
        console.log('Safari Native WebView initialized for container:', containerId);
        
        // Request native WebView creation via C# interop
        // The actual WebView will be created by platform-specific code
        if (window.chrome && window.chrome.webview) {
            // This is a WebView2 environment (Windows)
            this.initializeWindows();
        } else {
            // iOS/Android/macOS - signal readiness
            console.log('Native WebView environment detected');
        }
        
        return true;
    },

    // Load URL in the native WebView
    loadUrl: function(url) {
        console.log('Loading URL in native WebView:', url);
        
        // Normalize URL
        let normalizedUrl = url.trim();
        if (!normalizedUrl.match(/^https?:\/\//i)) {
            if (normalizedUrl.includes('.') && !normalizedUrl.includes(' ')) {
                normalizedUrl = 'https://' + normalizedUrl;
            } else {
                normalizedUrl = 'https://www.google.com/search?q=' + encodeURIComponent(normalizedUrl);
            }
        }
        
        // Signal to C# to load URL in native WebView
        if (this.dotNetRef) {
            // For now, we'll use a message passing approach
            // The actual navigation will be handled by platform-specific code
            window.location.hash = '#safari-nav:' + encodeURIComponent(normalizedUrl);
        }
        
        return normalizedUrl;
    },

    // Navigate back
    goBack: function() {
        console.log('Navigate back in native WebView');
        window.location.hash = '#safari-back';
    },

    // Navigate forward
    goForward: function() {
        console.log('Navigate forward in native WebView');
        window.location.hash = '#safari-forward';
    },

    // Reload current page
    reload: function() {
        console.log('Reload in native WebView');
        window.location.hash = '#safari-reload';
    },

    // Called from C# when navigation completes
    onNavigated: function(url) {
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('OnWebViewNavigated', url);
        }
    },

    // Called from C# when an error occurs
    onError: function(error) {
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('OnWebViewError', error);
        }
    },

    // Windows-specific initialization
    initializeWindows: function() {
        console.log('Initializing WebView2 (Windows)');
        // WebView2-specific setup if needed
    }
};
