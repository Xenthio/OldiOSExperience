using System.Collections.Generic;
using System.Threading.Tasks;
using OldiOS.Shared.Models;

namespace OldiOS.Shared.Services
{
    public interface IMediaLibraryService
    {
        Task<List<Song>> GetAllSongsAsync();
        Task<List<Album>> GetAlbumsAsync();
        Task<List<Artist>> GetArtistsAsync();
        Task<List<Playlist>> GetPlaylistsAsync();

        bool HasPermission { get; }
        Task<bool> RequestPermissionAsync();
    }
}
