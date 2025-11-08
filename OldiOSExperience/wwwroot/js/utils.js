function getElementRect(element) {
    if (!element) {
        return null;
    }
    return element.getBoundingClientRect();
}
function getElementHeight(element) {
    if (!element) {
        return 0;
    }
    return element.getBoundingClientRect().height;
}

// Keep track of the Blazor component reference for the resize event
let blazorResizeRef = null;

// The function that gets called on window resize
const resizeHandler = () => {
    if (blazorResizeRef) {
        blazorResizeRef.invokeMethodAsync('OnBrowserResize');
    }
};

// Functions to add and remove the listener
function addResizeListener(dotNetObjectRef) {
    blazorResizeRef = dotNetObjectRef;
    window.addEventListener('resize', resizeHandler);
}

function removeResizeListener(dotNetObjectRef) {
    window.removeEventListener('resize', resizeHandler);
    blazorResizeRef = null;
}