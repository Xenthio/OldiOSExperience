using System;
using System.Collections.Generic;
using System.IO;

namespace OldiOS.Shared.CoreServices.FileSystem;

/// <summary>
/// Core virtual file system interface for iOS-like sandboxed file operations
/// </summary>
public interface IVirtualFileSystem
{
    // Basic file operations
    bool FileExists(string path);
    bool DirectoryExists(string path);
    Stream OpenRead(string path);
    Stream OpenWrite(string path);
    byte[] ReadAllBytes(string path);
    string ReadAllText(string path);
    void WriteAllBytes(string path, byte[] contents);
    void WriteAllText(string path, string contents);

    // Directory operations
    IEnumerable<string> GetFiles(string path, string searchPattern = "*");
    IEnumerable<string> GetDirectories(string path);
    void CreateDirectory(string path);
    void DeleteFile(string path);
    void DeleteDirectory(string path, bool recursive = false);

    // Path operations
    string ResolvePath(string path);
    string GetFileName(string path);
    string GetDirectoryName(string path);
    string GetFileNameWithoutExtension(string path);
    string GetExtension(string path);

    // Misc
    long FileSize(string path);
    bool MoveFile(string source, string destination);
    bool MoveDirectory(string source, string destination);
    bool CopyFile(string source, string destination);

    // Event for when the file system changes
    event Action<string> OnFileSystemChanged;
    void NotifyChange(string path);
}
