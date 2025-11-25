using System;
using OldiOS.Shared.CoreServices.FileSystem;
using OldiOS.Shared.Models;

namespace OldiOS.Shared.CoreServices;

/// <summary>
/// Handles the initial setup of the iOS virtual file system.
/// Creates the root structure and installs all system apps.
/// Equivalent to XGUI-3's Setup system for Windows.
/// </summary>
public class SystemSetup
{
    private readonly IVirtualFileSystem _fileSystem;
    private readonly SpringboardService _springboardService;

    public SystemSetup(IVirtualFileSystem fileSystem, SpringboardService springboardService)
    {
        _fileSystem = fileSystem;
        _springboardService = springboardService;
    }

    /// <summary>
    /// Performs the initial system setup, creating all necessary directories
    /// and installing system apps as .app bundles in /Applications
    /// </summary>
    public void Initialize()
    {
        // Create system directories (VirtualFileSystem constructor already does this,
        // but we ensure they exist)
        EnsureSystemDirectories();
        
        // Install all registered apps as .app bundles
        InstallSystemApps();
    }

    /// <summary>
    /// Ensures all required system directories exist
    /// </summary>
    private void EnsureSystemDirectories()
    {
        // iOS root directories
        _fileSystem.CreateDirectory("/Applications");
        _fileSystem.CreateDirectory("/System");
        _fileSystem.CreateDirectory("/System/Library");
        _fileSystem.CreateDirectory("/System/Library/CoreServices");
        _fileSystem.CreateDirectory("/System/Library/Frameworks");
        _fileSystem.CreateDirectory("/System/Library/PrivateFrameworks");
        
        // User directories
        _fileSystem.CreateDirectory("/var");
        _fileSystem.CreateDirectory("/var/mobile");
        _fileSystem.CreateDirectory("/var/mobile/Applications");
        _fileSystem.CreateDirectory("/var/mobile/Library");
        _fileSystem.CreateDirectory("/var/mobile/Library/Preferences");
        _fileSystem.CreateDirectory("/var/mobile/Library/Caches");
        _fileSystem.CreateDirectory("/var/mobile/Documents");
        _fileSystem.CreateDirectory("/var/mobile/Media");
        _fileSystem.CreateDirectory("/var/mobile/Media/DCIM");
        _fileSystem.CreateDirectory("/var/mobile/Media/iTunes_Control");
        _fileSystem.CreateDirectory("/var/mobile/Media/iTunes_Control/Music");
        
        // Create SpringBoard.app (the home screen itself)
        _fileSystem.CreateDirectory("/System/Library/CoreServices/SpringBoard.app");
    }

    /// <summary>
    /// Installs all apps registered in SpringboardService as .app bundles
    /// </summary>
    private void InstallSystemApps()
    {
        var allApps = _springboardService.AllApps;
        
        foreach (var appInfo in allApps)
        {
            InstallApp(appInfo);
        }
    }

    /// <summary>
    /// Installs a single app as a .app bundle in the virtual file system
    /// </summary>
    public void InstallApp(AppInfo appInfo)
    {
        var descriptor = new AppDescriptor
        {
            Name = appInfo.Name,
            BundleId = appInfo.BundleId,
            IconPath = appInfo.IconPath,
            ComponentType = appInfo.ComponentType,
            ComponentTypeName = appInfo.ComponentType?.FullName ?? string.Empty,
            IsSystemApp = true,
            Description = $"{appInfo.Name} application"
        };

        FakeApp.CreateAppBundle(_fileSystem, descriptor);
    }

    /// <summary>
    /// Installs a user app (goes to /var/mobile/Applications instead of /Applications)
    /// </summary>
    public void InstallUserApp(AppDescriptor descriptor)
    {
        descriptor.IsSystemApp = false;
        FakeApp.CreateAppBundle(_fileSystem, descriptor);
    }

    /// <summary>
    /// Uninstalls an app by removing its .app bundle
    /// </summary>
    public void UninstallApp(string bundleId)
    {
        // Check user apps first
        var userAppPath = $"/var/mobile/Applications/{bundleId}.app";
        if (_fileSystem.DirectoryExists(userAppPath))
        {
            _fileSystem.DeleteDirectory(userAppPath, recursive: true);
            return;
        }

        // System apps typically can't be uninstalled, but we support it for testing
        var systemAppPath = $"/Applications/{bundleId}.app";
        if (_fileSystem.DirectoryExists(systemAppPath))
        {
            _fileSystem.DeleteDirectory(systemAppPath, recursive: true);
        }
    }

    /// <summary>
    /// Gets the path where an app's sandbox data is stored
    /// </summary>
    public string GetAppSandboxPath(string bundleId)
    {
        return $"/var/mobile/Applications/{bundleId}";
    }

    /// <summary>
    /// Gets the Documents directory for an app's sandbox
    /// </summary>
    public string GetAppDocumentsPath(string bundleId)
    {
        var sandboxPath = GetAppSandboxPath(bundleId);
        var documentsPath = $"{sandboxPath}/Documents";
        _fileSystem.CreateDirectory(documentsPath);
        return documentsPath;
    }

    /// <summary>
    /// Gets the Library directory for an app's sandbox
    /// </summary>
    public string GetAppLibraryPath(string bundleId)
    {
        var sandboxPath = GetAppSandboxPath(bundleId);
        var libraryPath = $"{sandboxPath}/Library";
        _fileSystem.CreateDirectory(libraryPath);
        return libraryPath;
    }
}
