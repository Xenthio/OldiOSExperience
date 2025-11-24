// Image preloading functionality with concurrency control
window.preloadImages = function (imagePaths, dotNetRef) {
    if (!imagePaths || imagePaths.length === 0) {
        return;
    }

    // Create a hidden container for preloaded images
    let container = document.getElementById('preload-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'preload-container';
        container.style.position = 'absolute';
        container.style.left = '-9999px';
        container.style.top = '-9999px';
        container.style.width = '1px';
        container.style.height = '1px';
        container.style.overflow = 'hidden';
        document.body.appendChild(container);
    }

    // Concurrency control - load 8 images at a time to avoid overwhelming the browser
    const maxConcurrent = 8;
    let currentIndex = 0;
    let activeRequests = 0;

    function loadNextImage() {
        // If we've processed all images, we're done
        if (currentIndex >= imagePaths.length && activeRequests === 0) {
            return;
        }

        // Load up to maxConcurrent images at once
        while (activeRequests < maxConcurrent && currentIndex < imagePaths.length) {
            const path = imagePaths[currentIndex];
            currentIndex++;
            activeRequests++;

            const img = new Image();
            
            img.onload = () => {
                activeRequests--;
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync('OnImageLoaded');
                }
                loadNextImage(); // Load next image in queue
            };
            
            img.onerror = () => {
                activeRequests--;
                console.warn(`Failed to preload image: ${path}`);
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync('OnImageError', path);
                }
                loadNextImage(); // Load next image in queue
            };
            
            // Start loading the image
            img.src = path;
            
            // Add to container to keep reference
            container.appendChild(img);
        }
    }

    // Start the loading process
    loadNextImage();
};
