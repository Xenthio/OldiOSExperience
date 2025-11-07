namespace OldiOSExperience.Models
{
    public class AppInfo
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string IconPath { get; set; }
        
        /// <summary>Type of component to launch when app is opened</summary>
        public Type? ComponentType { get; set; }
        
        /// <summary>Bundle identifier (e.g., com.apple.settings)</summary>
        public string? BundleId { get; set; }
    }
}