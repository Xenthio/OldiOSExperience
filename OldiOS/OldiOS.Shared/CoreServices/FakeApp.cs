using System = global::System;
using global::System.Text;
using OldiOS.Shared.CoreServices.FileSystem;

namespace OldiOS.Shared.CoreServices;

/// <summary>
/// Handles creation and parsing of .app bundles in the virtual file system.
/// Equivalent to FakeExecutable in XGUI-3's FakeOS for Windows .exe files.
/// 
/// In real iOS, .app is a directory bundle containing:
/// - Info.plist (app metadata)
/// - Executable binary
/// - Resources (icons, assets, etc.)
/// 
/// In our virtual iOS, the .app bundle contains:
/// - Info.plist with AppDescriptor JSON
/// - Icon files
/// </summary>
public static class FakeApp
{
    /// <summary>
    /// Creates a .app bundle in the virtual file system
    /// </summary>
    public static void CreateAppBundle(IVirtualFileSystem fs, AppDescriptor app)
    {
        var bundlePath = app.GetAppBundlePath();
        
        // Create the .app directory
        fs.CreateDirectory(bundlePath);
        
        // Create Info.plist with app descriptor
        var infoPlistPath = app.GetInfoPlistPath();
        var infoPlistContent = CreateInfoPlist(app);
        fs.WriteAllText(infoPlistPath, infoPlistContent);
        
        // Create a placeholder for the icon if path is specified
        if (!string.IsNullOrEmpty(app.IconPath))
        {
            var iconFileName = fs.GetFileName(app.IconPath);
            var iconDestPath = $"{bundlePath}/{iconFileName}";
            // Note: In a real implementation, we'd copy the actual icon
            // For now, we just store the reference path
            fs.WriteAllText($"{bundlePath}/IconRef.txt", app.IconPath);
        }
    }

    /// <summary>
    /// Reads an AppDescriptor from a .app bundle
    /// </summary>
    public static AppDescriptor? ReadAppBundle(IVirtualFileSystem fs, string bundlePath)
    {
        var infoPlistPath = $"{bundlePath}/Info.plist";
        
        if (!fs.FileExists(infoPlistPath))
            return null;

        try
        {
            var content = fs.ReadAllText(infoPlistPath);
            return ParseInfoPlist(content);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets all installed apps from the file system
    /// </summary>
    public static AppDescriptor[] GetInstalledApps(IVirtualFileSystem fs)
    {
        var apps = new global::System.Collections.Generic.List<AppDescriptor>();
        
        // Scan system apps
        if (fs.DirectoryExists("/Applications"))
        {
            foreach (var dir in fs.GetDirectories("/Applications"))
            {
                if (dir.EndsWith(".app", StringComparison.OrdinalIgnoreCase))
                {
                    var app = ReadAppBundle(fs, dir);
                    if (app != null)
                    {
                        app.IsSystemApp = true;
                        apps.Add(app);
                    }
                }
            }
        }
        
        // Scan user apps
        if (fs.DirectoryExists("/var/mobile/Applications"))
        {
            foreach (var dir in fs.GetDirectories("/var/mobile/Applications"))
            {
                if (dir.EndsWith(".app", StringComparison.OrdinalIgnoreCase))
                {
                    var app = ReadAppBundle(fs, dir);
                    if (app != null)
                    {
                        app.IsSystemApp = false;
                        apps.Add(app);
                    }
                }
            }
        }
        
        return apps.ToArray();
    }

    /// <summary>
    /// Creates a plist-like content storing the app descriptor
    /// We use a simplified JSON format for easier parsing in Blazor
    /// </summary>
    private static string CreateInfoPlist(AppDescriptor app)
    {
        // Store as JSON for simplicity (real iOS uses binary plist)
        return app.ToJson();
    }

    /// <summary>
    /// Parses the Info.plist content to get an AppDescriptor
    /// </summary>
    private static AppDescriptor? ParseInfoPlist(string content)
    {
        return AppDescriptor.FromJson(content);
    }

    /// <summary>
    /// Checks if a path is an app bundle
    /// </summary>
    public static bool IsAppBundle(string path)
    {
        return path.EndsWith(".app", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the bundle identifier from an app bundle path
    /// </summary>
    public static string GetBundleIdFromPath(string bundlePath)
    {
        var fileName = global::System.IO.Path.GetFileNameWithoutExtension(bundlePath);
        return fileName;
    }
}
