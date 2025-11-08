namespace OldiOSExperience.Services
{
    public static class DisplaySettings
    {
        /// <summary>
        /// The native horizontal resolution of the simulated screen.
        /// </summary>
        public const double RESOLUTION_X = 640.0;

        /// <summary>
        /// The native vertical resolution of the simulated screen.
        /// </summary>
        public const double RESOLUTION_Y = 960.0;

        /// <summary>
        /// The calculated scale factor to fit the screen in the browser viewport.
        /// This is set at startup in Index.razor.
        /// </summary>
        public static double SCALEFACTOR { get; set; } = 1.0;
    }
}