function getElementRect(element) {
    if (!element) {
        return null;
    }
    return element.getBoundingClientRect();
}