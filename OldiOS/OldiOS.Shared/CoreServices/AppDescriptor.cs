using System;
using System.Text.Json;

namespace OldiOS.Shared.CoreServices;

/// <summary>
/// Describes an iOS app bundle (.app) that can be launched in the virtual operating system.
/// Equivalent to ProgramDescriptor in XGUI-3's FakeOS for Windows executables.
/// </summary>
public class AppDescriptor
{
    /// <summary>
    /// The display name of the app (CFBundleDisplayName)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The bundle identifier (CFBundleIdentifier), e.g., "com.apple.MobileSafari"
    /// </summary>
    public string BundleId { get; set; } = string.Empty;

    /// <summary>
    /// The path to the app icon
    /// </summary>
    public string IconPath { get; set; } = string.Empty;

    /// <summary>
    /// The Blazor component type name to create when this app is launched
    /// </summary>
    public string ComponentTypeName { get; set; } = string.Empty;

    /// <summary>
    /// The actual Type of the component (set at runtime)
    /// </summary>
    public Type? ComponentType { get; set; }

    /// <summary>
    /// Optional description of the app
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The version string (CFBundleVersion)
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Minimum iOS version required
    /// </summary>
    public string MinimumOSVersion { get; set; } = "5.0";

    /// <summary>
    /// Whether this is a system app (in /Applications) vs user app (in /var/mobile/Applications)
    /// </summary>
    public bool IsSystemApp { get; set; } = true;

    /// <summary>
    /// URL scheme handlers (e.g., "tel", "mailto", "http")
    /// </summary>
    public string[] UrlSchemes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Create an AppDescriptor for a launchable app
    /// </summary>
    public AppDescriptor() { }

    /// <summary>
    /// Create an AppDescriptor with basic info
    /// </summary>
    public AppDescriptor(string name, string bundleId, string iconPath, string componentTypeName)
    {
        Name = name;
        BundleId = bundleId;
        IconPath = iconPath;
        ComponentTypeName = componentTypeName;
    }

    /// <summary>
    /// Create an AppDescriptor with a component type
    /// </summary>
    public AppDescriptor(string name, string bundleId, string iconPath, Type componentType)
    {
        Name = name;
        BundleId = bundleId;
        IconPath = iconPath;
        ComponentType = componentType;
        ComponentTypeName = componentType.FullName ?? componentType.Name;
    }

    /// <summary>
    /// Gets the .app bundle path in the virtual file system
    /// </summary>
    public string GetAppBundlePath()
    {
        var basePath = IsSystemApp ? "/Applications" : "/var/mobile/Applications";
        return $"{basePath}/{BundleId}.app";
    }

    /// <summary>
    /// Gets the path to the Info.plist file
    /// </summary>
    public string GetInfoPlistPath()
    {
        return $"{GetAppBundlePath()}/Info.plist";
    }

    /// <summary>
    /// Serializes the app descriptor to JSON for storage
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Reads an app descriptor from JSON content
    /// </summary>
    public static AppDescriptor? FromJson(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<AppDescriptor>(json);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
