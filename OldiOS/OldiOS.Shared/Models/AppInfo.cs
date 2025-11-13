using OldiOS.Shared.System;

namespace OldiOS.Shared.Models
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
        
        /// <summary>Status bar style for this app (defaults to Coloured)</summary>
        public StatusBarStyle StatusBarStyle { get; set; } = StatusBarStyle.Coloured;
    }
}