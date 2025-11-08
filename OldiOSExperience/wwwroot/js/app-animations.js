function calculateScatterVector(iconElement) {
    if (!iconElement) {
        return;
    }

    const pagesContainer = iconElement.closest('.springboard-pages-container');
    const parentPage = iconElement.closest('.springboard-page');

    if (!pagesContainer || !parentPage) {
        return;
    }

    const containerRect = pagesContainer.getBoundingClientRect();
    const iconRect = iconElement.getBoundingClientRect();
    const pageRect = parentPage.getBoundingClientRect();

    const iconRelativeX = (iconRect.left - pageRect.left) + (iconRect.width / 2);
    const iconRelativeY = (iconRect.top - pageRect.top) + (iconRect.height / 2);

    
    // --- Add your offset configuration here ---
    const originOffsetX = 0;    // Negative values move the origin left
    const originOffsetY = -30;  // Negative values move the origin up

    // Apply the offset to the center point calculation
    const vectorX = iconRelativeX - ((containerRect.width / 2) + originOffsetX);
    const vectorY = iconRelativeY - ((containerRect.height / 2) + originOffsetY);

    // --- Normalization Logic ---

    // 1. Calculate the magnitude (length) of the direction vector.
    const magnitude = Math.sqrt(vectorX * vectorX + vectorY * vectorY);

    // 2. Define how far you want every icon to travel.
    const scatterDistance = 200; // e.g., 200 pixels. Adjust for desired effect.

    let finalX, finalY;

    // 3. Avoid division by zero if an icon is perfectly centered.
    if (magnitude > 0) {
        // Normalize the vector by dividing by its magnitude, then scale by the desired distance.
        finalX = (vectorX / magnitude) * scatterDistance;
        finalY = (vectorY / magnitude) * scatterDistance;
    } else {
        // If magnitude is 0, the icon is at the center. It doesn't move.
        finalX = 0;
        finalY = 0;
    }

    // Set the final calculated properties.
    iconElement.style.setProperty('--scatter-x', `${finalX}px`);
    iconElement.style.setProperty('--scatter-y', `${finalY}px`);
}