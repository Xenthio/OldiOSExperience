namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Interface for platform-specific battery services.
    /// Implemented by native platforms (MAUI) to provide real battery information.
    /// </summary>
    public interface INativeBatteryService
    {
        /// <summary>
        /// Event fired when battery state changes.
        /// </summary>
        event EventHandler? BatteryChanged;

        /// <summary>
        /// Gets whether the device is currently charging.
        /// </summary>
        bool IsCharging { get; }

        /// <summary>
        /// Gets the battery charge level (0.0 to 1.0).
        /// </summary>
        double ChargeLevel { get; }

        /// <summary>
        /// Gets whether native battery information is available.
        /// </summary>
        bool IsAvailable { get; }
    }
}
