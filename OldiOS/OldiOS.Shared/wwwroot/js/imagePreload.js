// Image preloading functionality
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

    // Preload each image
    imagePaths.forEach(path => {
        const img = new Image();
        
        img.onload = () => {
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnImageLoaded');
            }
        };
        
        img.onerror = () => {
            console.warn(`Failed to preload image: ${path}`);
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnImageError', path);
            }
        };
        
        // Start loading the image
        img.src = path;
        
        // Add to container to keep reference
        container.appendChild(img);
    });
};
