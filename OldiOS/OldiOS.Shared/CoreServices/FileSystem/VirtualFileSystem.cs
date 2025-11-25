using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OldiOS.Shared.CoreServices.FileSystem;

/// <summary>
/// Implementation of the virtual file system for iOS simulation.
/// Simulates iOS's sandboxed file system structure.
/// </summary>
public class VirtualFileSystem : IVirtualFileSystem
{
    // Singleton instance
    public static VirtualFileSystem? Instance { get; private set; }

    // In-memory file system storage
    private readonly Dictionary<string, VirtualFile> _files = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _directories = new(StringComparer.OrdinalIgnoreCase);

    public event Action<string>? OnFileSystemChanged;

    public VirtualFileSystem()
    {
        Instance = this;
        InitializeFileSystem();
    }

    /// <summary>
    /// Initializes the iOS-like file system structure
    /// </summary>
    private void InitializeFileSystem()
    {
        // Create iOS root structure
        // /Applications - System apps
        CreateDirectory("/Applications");
        
        // /var/mobile - User data root
        CreateDirectory("/var/mobile");
        CreateDirectory("/var/mobile/Applications"); // User app sandboxes
        CreateDirectory("/var/mobile/Library");
        CreateDirectory("/var/mobile/Library/Preferences"); // UserDefaults storage
        CreateDirectory("/var/mobile/Documents");
        CreateDirectory("/var/mobile/Media");
        CreateDirectory("/var/mobile/Media/DCIM"); // Camera roll
        CreateDirectory("/var/mobile/Media/iTunes_Control"); // Music
        
        // /System - System files (read-only in real iOS)
        CreateDirectory("/System");
        CreateDirectory("/System/Library");
        CreateDirectory("/System/Library/CoreServices");
        CreateDirectory("/System/Library/Frameworks");
    }

    public string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "/";

        path = path.Replace('\\', '/');
        
        // Handle relative paths
        if (!path.StartsWith("/"))
            path = "/" + path;

        // Handle . and ..
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();
        
        foreach (var part in parts)
        {
            if (part == ".")
                continue;
            if (part == "..")
            {
                if (result.Count > 0)
                    result.RemoveAt(result.Count - 1);
                continue;
            }
            result.Add(part);
        }

        return "/" + string.Join("/", result);
    }

    public bool FileExists(string path)
    {
        path = NormalizePath(path);
        return _files.ContainsKey(path);
    }

    public bool DirectoryExists(string path)
    {
        path = NormalizePath(path);
        if (path == "/") return true;
        return _directories.Contains(path);
    }

    public Stream OpenRead(string path)
    {
        path = NormalizePath(path);
        if (!_files.TryGetValue(path, out var file))
            throw new FileNotFoundException($"File not found: {path}");
        return new MemoryStream(file.Contents, writable: false);
    }

    public Stream OpenWrite(string path)
    {
        path = NormalizePath(path);
        var stream = new VirtualFileStream(this, path);
        return stream;
    }

    public byte[] ReadAllBytes(string path)
    {
        path = NormalizePath(path);
        if (!_files.TryGetValue(path, out var file))
            throw new FileNotFoundException($"File not found: {path}");
        return file.Contents.ToArray();
    }

    public string ReadAllText(string path)
    {
        return Encoding.UTF8.GetString(ReadAllBytes(path));
    }

    public void WriteAllBytes(string path, byte[] contents)
    {
        path = NormalizePath(path);
        
        // Ensure parent directory exists
        var dir = GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !DirectoryExists(dir))
            CreateDirectory(dir);

        _files[path] = new VirtualFile
        {
            Path = path,
            Contents = contents,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
        
        NotifyChange(path);
    }

    public void WriteAllText(string path, string contents)
    {
        WriteAllBytes(path, Encoding.UTF8.GetBytes(contents));
    }

    public IEnumerable<string> GetFiles(string path, string searchPattern = "*")
    {
        path = NormalizePath(path);
        var prefix = path == "/" ? "/" : path + "/";
        
        return _files.Keys
            .Where(f => f.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Where(f => !f.Substring(prefix.Length).Contains('/')) // Only direct children
            .Where(f => MatchesPattern(GetFileName(f), searchPattern));
    }

    public IEnumerable<string> GetDirectories(string path)
    {
        path = NormalizePath(path);
        var prefix = path == "/" ? "/" : path + "/";
        
        return _directories
            .Where(d => d.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Where(d => !d.Substring(prefix.Length).Contains('/')) // Only direct children
            .Distinct();
    }

    public void CreateDirectory(string path)
    {
        path = NormalizePath(path);
        if (path == "/") return;
        
        // Create parent directories
        var parent = GetDirectoryName(path);
        if (!string.IsNullOrEmpty(parent) && parent != "/" && !DirectoryExists(parent))
            CreateDirectory(parent);
        
        _directories.Add(path);
        NotifyChange(path);
    }

    public void DeleteFile(string path)
    {
        path = NormalizePath(path);
        if (_files.Remove(path))
            NotifyChange(path);
    }

    public void DeleteDirectory(string path, bool recursive = false)
    {
        path = NormalizePath(path);
        
        if (recursive)
        {
            // Delete all files in directory
            var filesToDelete = _files.Keys
                .Where(f => f.StartsWith(path + "/", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var file in filesToDelete)
                _files.Remove(file);

            // Delete all subdirectories
            var dirsToDelete = _directories
                .Where(d => d.StartsWith(path + "/", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var dir in dirsToDelete)
                _directories.Remove(dir);
        }
        
        _directories.Remove(path);
        NotifyChange(path);
    }

    public string ResolvePath(string path) => NormalizePath(path);

    public string GetFileName(string path)
    {
        path = NormalizePath(path);
        var lastSlash = path.LastIndexOf('/');
        return lastSlash >= 0 ? path.Substring(lastSlash + 1) : path;
    }

    public string GetDirectoryName(string path)
    {
        path = NormalizePath(path);
        var lastSlash = path.LastIndexOf('/');
        if (lastSlash <= 0) return "/";
        return path.Substring(0, lastSlash);
    }

    public string GetFileNameWithoutExtension(string path)
    {
        var fileName = GetFileName(path);
        var lastDot = fileName.LastIndexOf('.');
        return lastDot > 0 ? fileName.Substring(0, lastDot) : fileName;
    }

    public string GetExtension(string path)
    {
        var fileName = GetFileName(path);
        var lastDot = fileName.LastIndexOf('.');
        return lastDot >= 0 ? fileName.Substring(lastDot) : string.Empty;
    }

    public long FileSize(string path)
    {
        path = NormalizePath(path);
        return _files.TryGetValue(path, out var file) ? file.Contents.Length : 0;
    }

    public bool MoveFile(string source, string destination)
    {
        source = NormalizePath(source);
        destination = NormalizePath(destination);
        
        if (!_files.TryGetValue(source, out var file))
            return false;

        _files[destination] = file with { Path = destination, ModifiedDate = DateTime.UtcNow };
        _files.Remove(source);
        
        NotifyChange(source);
        NotifyChange(destination);
        return true;
    }

    public bool MoveDirectory(string source, string destination)
    {
        source = NormalizePath(source);
        destination = NormalizePath(destination);
        
        if (!DirectoryExists(source))
            return false;

        // Move all files
        var filesToMove = _files.Keys
            .Where(f => f.StartsWith(source + "/", StringComparison.OrdinalIgnoreCase) || f == source)
            .ToList();
        
        foreach (var filePath in filesToMove)
        {
            var newPath = destination + filePath.Substring(source.Length);
            if (_files.TryGetValue(filePath, out var file))
            {
                _files[newPath] = file with { Path = newPath };
                _files.Remove(filePath);
            }
        }

        // Move all directories
        var dirsToMove = _directories
            .Where(d => d.StartsWith(source + "/", StringComparison.OrdinalIgnoreCase) || d == source)
            .ToList();
        
        foreach (var dirPath in dirsToMove)
        {
            var newPath = destination + dirPath.Substring(source.Length);
            _directories.Remove(dirPath);
            _directories.Add(newPath);
        }

        NotifyChange(source);
        NotifyChange(destination);
        return true;
    }

    public bool CopyFile(string source, string destination)
    {
        source = NormalizePath(source);
        destination = NormalizePath(destination);
        
        if (!_files.TryGetValue(source, out var file))
            return false;

        WriteAllBytes(destination, file.Contents.ToArray());
        return true;
    }

    public void NotifyChange(string path)
    {
        OnFileSystemChanged?.Invoke(path);
    }

    private static bool MatchesPattern(string fileName, string pattern)
    {
        if (pattern == "*") return true;
        if (pattern.StartsWith("*."))
        {
            var ext = pattern.Substring(1);
            return fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase);
        }
        return fileName.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Represents a file in the virtual file system
/// </summary>
public record VirtualFile
{
    public required string Path { get; init; }
    public byte[] Contents { get; set; } = Array.Empty<byte>();
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// A writable stream that saves to the virtual file system on close
/// </summary>
internal class VirtualFileStream : MemoryStream
{
    private readonly VirtualFileSystem _fs;
    private readonly string _path;

    public VirtualFileStream(VirtualFileSystem fs, string path)
    {
        _fs = fs;
        _path = path;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fs.WriteAllBytes(_path, ToArray());
        }
        base.Dispose(disposing);
    }
}
