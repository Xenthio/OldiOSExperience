using System;
using System.Threading.Tasks;

namespace OldiOS.Shared.Services
{
    public interface IWebViewService
    {
        event Action<string> OnUrlChanged;
        event Action<bool> OnScroll; // true = down (hide header), false = up (show header)

        void Show(string url, double x, double y, double width, double height);
        void Hide();
        void SetOpacity(double opacity);
        void UpdatePosition(double x, double y, double width, double height);
        void Navigate(string url);
        void GoBack();
        void GoForward();
        void Reload();
        Task<string> CaptureSnapshotAsync();
    }
}
