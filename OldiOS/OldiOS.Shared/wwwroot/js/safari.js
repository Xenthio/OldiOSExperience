// Safari browser JavaScript interop
// Handles web browsing functionality for the iOS 5 Safari replica

window.safariInterop = {
    // iPhone 4 / iOS 5 User Agent
    iPhone4UserAgent: 'Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3',
    
    // Try to set a custom user agent (note: this doesn't actually work in modern browsers)
    // This is here for documentation purposes - actual user agent can't be changed in browser
    setUserAgent: function(userAgent) {
        // Modern browsers don't allow changing the user agent via JavaScript
        // This would need to be done via browser extensions or dev tools
        console.log('User agent would be set to:', userAgent);
        console.log('Note: Browsers do not allow changing user agent via JavaScript');
        return false;
    },
    
    // Check if a URL can be loaded in an iframe
    // This makes a HEAD request to check for X-Frame-Options
    checkIframeCompatibility: async function(url) {
        try {
            const response = await fetch(url, { 
                method: 'HEAD',
                mode: 'no-cors' // Avoid CORS errors
            });
            
            // Note: With no-cors mode, we can't actually read headers
            // So we'll just return true and let the iframe try
            return true;
        } catch (error) {
            console.log('Error checking URL:', error);
            return false;
        }
    },
    
    // Load URL in iframe
    loadUrlInIframe: function(iframeId, url) {
        const iframe = document.getElementById(iframeId);
        if (!iframe) {
            console.error('Iframe not found:', iframeId);
            return '';  // Return empty string instead of false
        }
        
        try {
            // Normalize URL
            let normalizedUrl = url.trim();
            
            // Add protocol if missing
            if (!normalizedUrl.match(/^https?:\/\//i)) {
                // Check if it looks like a domain
                if (normalizedUrl.includes('.') && !normalizedUrl.includes(' ')) {
                    normalizedUrl = 'https://' + normalizedUrl;
                } else {
                    // Treat as search query
                    normalizedUrl = 'https://www.google.com/search?q=' + encodeURIComponent(normalizedUrl);
                }
            }
            
            iframe.src = normalizedUrl;
            return normalizedUrl;
        } catch (error) {
            console.error('Error loading URL:', error);
            return '';  // Return empty string instead of false
        }
    },
    
    // Navigate back in iframe
    navigateBack: function(iframeId) {
        try {
            const iframe = document.getElementById(iframeId);
            if (iframe && iframe.contentWindow) {
                iframe.contentWindow.history.back();
                return true;
            }
        } catch (error) {
            console.error('Cannot navigate back:', error);
        }
        return false;
    },
    
    // Navigate forward in iframe
    navigateForward: function(iframeId) {
        try {
            const iframe = document.getElementById(iframeId);
            if (iframe && iframe.contentWindow) {
                iframe.contentWindow.history.forward();
                return true;
            }
        } catch (error) {
            console.error('Cannot navigate forward:', error);
        }
        return false;
    },
    
    // Reload iframe
    reload: function(iframeId) {
        try {
            const iframe = document.getElementById(iframeId);
            if (iframe) {
                iframe.src = iframe.src;
                return true;
            }
        } catch (error) {
            console.error('Cannot reload:', error);
        }
        return false;
    },
    
    // Get current URL from iframe (may fail due to same-origin policy)
    getCurrentUrl: function(iframeId) {
        try {
            const iframe = document.getElementById(iframeId);
            if (iframe && iframe.contentWindow) {
                return iframe.contentWindow.location.href;
            }
        } catch (error) {
            // Expected to fail for cross-origin iframes
            // Return the src attribute instead
            const iframe = document.getElementById(iframeId);
            return iframe ? iframe.src : '';
        }
        return '';
    },
    
    // Setup iframe error handling
    setupIframeErrorHandling: function(iframeId, dotNetReference) {
        const iframe = document.getElementById(iframeId);
        if (!iframe) return;
        
        // Listen for load event
        iframe.addEventListener('load', function() {
            try {
                // Try to access the iframe content (will fail for cross-origin)
                const url = iframe.contentWindow.location.href;
                dotNetReference.invokeMethodAsync('OnIframeLoaded', url);
            } catch (error) {
                // Cross-origin iframe - that's ok
                dotNetReference.invokeMethodAsync('OnIframeLoaded', iframe.src);
            }
        });
        
        // Listen for error event
        iframe.addEventListener('error', function(event) {
            dotNetReference.invokeMethodAsync('OnIframeError', 'Failed to load page');
        });
    }
};
