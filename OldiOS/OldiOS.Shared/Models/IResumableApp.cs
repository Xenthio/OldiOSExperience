namespace OldiOS.Shared.Models
{
    /// <summary>
    /// Interface for apps that support state persistence and resumption.
    /// Apps implementing this interface can save their state when moved to background
    /// and restore it when brought back to foreground.
    /// </summary>
    public interface IResumableApp
    {
        /// <summary>
        /// Saves the current state of the app to a dictionary.
        /// Called when the app is moved to background.
        /// </summary>
        /// <returns>Dictionary containing serializable state data</returns>
        Dictionary<string, object> SaveState();
        
        /// <summary>
        /// Restores the app state from a previously saved dictionary.
        /// Called when the app is brought back to foreground.
        /// </summary>
        /// <param name="state">Dictionary containing previously saved state data</param>
        void RestoreState(Dictionary<string, object> state);
    }
}
