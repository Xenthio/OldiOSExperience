using Microsoft.Maui.Controls;
using OldiOS.Shared.Services;
using System;
using System.Threading.Tasks;

namespace OldiOS.Services
{
    public class MauiWebViewService : IWebViewService
    {
        private readonly WebView _nativeWebView;
        private readonly MainPage _mainPage;

        public event Action<string> OnUrlChanged;
        public event Action<bool>? OnScroll;

        public MauiWebViewService(MainPage mainPage)
        {
            _mainPage = mainPage;
            _nativeWebView = mainPage.FindByName<WebView>("NativeWebView");

            if (_nativeWebView != null)
            {
                // Set iPhone 4 User Agent
                _nativeWebView.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3";

                _nativeWebView.Navigating += (s, e) =>
                {
                    if (e.Url.StartsWith("oldios://scroll/"))
                    {
                        e.Cancel = true;
                        bool isDown = e.Url.EndsWith("down");
                        OnScroll?.Invoke(isDown);
                    }
                };

                _nativeWebView.Navigated += async (s, e) =>
                {
                    OnUrlChanged?.Invoke(e.Url);

                    // Inject scroll listener with capture phase to catch all scroll events
                    string script = @"
                        (function() {
                            var lastScrollTop = 0;
                            var ticking = false;
                            var scrollHandler = function(e) {
                                if (!ticking) {
                                    window.requestAnimationFrame(function() {
                                        // Try to get scroll position from the target or window
                                        var scrollTop = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || (e.target ? e.target.scrollTop : 0);
                                        
                                        // If scrollTop is undefined or null, ignore
                                        if (scrollTop === undefined || scrollTop === null) {
                                            ticking = false;
                                            return;
                                        }

                                        if (Math.abs(scrollTop - lastScrollTop) > 10) { // Threshold
                                            if (scrollTop > lastScrollTop && scrollTop > 50) {
                                                window.location.href = 'oldios://scroll/down';
                                            } else if (scrollTop < lastScrollTop) {
                                                window.location.href = 'oldios://scroll/up';
                                            }
                                            lastScrollTop = scrollTop;
                                        }
                                        ticking = false;
                                    });
                                    ticking = true;
                                }
                            };
                            
                            // Use capture: true to detect scroll on any element (like overflow: scroll divs)
                            window.addEventListener('scroll', scrollHandler, true);
                        })();
                    ";
                    try
                    {
                        await _nativeWebView.EvaluateJavaScriptAsync(script);
                    }
                    catch { }
                };
            }
        }

        public void Show(string url, double x, double y, double width, double height)
        {
            if (_nativeWebView == null) return;

            _mainPage.Dispatcher.Dispatch(() =>
            {
                _nativeWebView.IsVisible = true;
                _nativeWebView.Source = url;
                UpdatePosition(x, y, width, height);
            });
        }

        public void Hide()
        {
            if (_nativeWebView == null) return;

            _mainPage.Dispatcher.Dispatch(() =>
            {
                _nativeWebView.IsVisible = false;
            });
        }

        public void SetOpacity(double opacity)
        {
            if (_nativeWebView == null) return;

            _mainPage.Dispatcher.Dispatch(() =>
            {
                _nativeWebView.Opacity = opacity;
            });
        }

        public void UpdatePosition(double x, double y, double width, double height)
        {
            if (_nativeWebView == null) return;

            _mainPage.Dispatcher.Dispatch(() =>
            {
                _nativeWebView.Margin = new Thickness(x, y, 0, 0);
                _nativeWebView.WidthRequest = width;
                _nativeWebView.HeightRequest = height;
            });
        }

        public void Navigate(string url)
        {
            if (_nativeWebView == null) return;

            _mainPage.Dispatcher.Dispatch(() =>
            {
                _nativeWebView.Source = url;
            });
        }

        public void GoBack()
        {
            if (_nativeWebView != null && _nativeWebView.CanGoBack)
            {
                _mainPage.Dispatcher.Dispatch(() => _nativeWebView.GoBack());
            }
        }

        public void GoForward()
        {
            if (_nativeWebView != null && _nativeWebView.CanGoForward)
            {
                _mainPage.Dispatcher.Dispatch(() => _nativeWebView.GoForward());
            }
        }

        public void Reload()
        {
            _mainPage.Dispatcher.Dispatch(() => _nativeWebView?.Reload());
        }

        public async Task<string> CaptureSnapshotAsync()
        {
            if (_nativeWebView == null) return "";

            return await _mainPage.Dispatcher.DispatchAsync(async () =>
            {
                try
                {
                    var result = await _nativeWebView.CaptureAsync();
                    if (result != null)
                    {
                        using var stream = await result.OpenReadAsync();
                        using var memoryStream = new System.IO.MemoryStream();
                        await stream.CopyToAsync(memoryStream);
                        var bytes = memoryStream.ToArray();
                        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Snapshot failed: {ex.Message}");
                }
                return "";
            });
        }
    }
}
