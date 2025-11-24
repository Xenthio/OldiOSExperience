using Microsoft.JSInterop;

namespace OldiOS.Shared.Services;

/// <summary>
/// Service to preload all image assets at startup to prevent delayed image loading
/// </summary>
public class ImagePreloadService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isPreloaded = false;
    private int _loadedCount = 0;
    private int _totalCount = 0;
    
    public event Action<int, int>? OnProgressChanged;
    public event Action? OnCompleted;
    
    public bool IsPreloaded => _isPreloaded;
    public int LoadedCount => _loadedCount;
    public int TotalCount => _totalCount;
    public double Progress => _totalCount > 0 ? (double)_loadedCount / _totalCount : 0;
    
    // All image paths relative to wwwroot
    private static readonly string[] ImagePaths = new[]
    {
        // App icons
        "images/icons/appstore.png",
        "images/icons/calculator.png",
        "images/icons/calendar.png",
        "images/icons/camera.png",
        "images/icons/clock.png",
        "images/icons/compass.png",
        "images/icons/contacts.png",
        "images/icons/facetime.png",
        "images/icons/gamecenter.png",
        "images/icons/itunes.png",
        "images/icons/mail.png",
        "images/icons/maps.png",
        "images/icons/messages.png",
        "images/icons/music.png",
        "images/icons/notes.png",
        "images/icons/phone.png",
        "images/icons/photos.png",
        "images/icons/reminders.png",
        "images/icons/safari.png",
        "images/icons/settings.png",
        "images/icons/stocks.png",
        "images/icons/videos.png",
        "images/icons/voicememos.png",
        "images/icons/weather.png",
        "images/icons/youtube.png",
        
        // Icon shadows
        "images/icon_shadow.png",
        "images/icon_shadow_dock.png",
        
        // Wallpapers and backgrounds
        "images/wallpaper.png",
        "images/wallpaper_gradient_bottom.png",
        "images/wallpaper_gradient_top.png",
        "images/dock.png",
        "images/folderswitcher_bg.png",
        "images/apple-logo.png",
        
        // Lock screen
        "images/lockscreen/controls_small.png",
        "images/lockscreen/lock-wallpaper.png",
        "images/lockscreen/slide_knob_grey.png",
        "images/lockscreen/slide_well.png",
        
        // Status bar - coloured
        "images/status/coloured/battery_charged.png",
        "images/status/coloured/battery_charging.png",
        "images/status/coloured/battery_draining.png",
        "images/status/coloured/battery_insides.png",
        "images/status/coloured/battery_insides_low.png",
        "images/status/coloured/wifi_0.png",
        "images/status/coloured/wifi_1.png",
        "images/status/coloured/wifi_2.png",
        "images/status/coloured/wifi_3.png",
        
        // Status bar - white on black etch
        "images/status/white_on_black_etch/battery_charged.png",
        "images/status/white_on_black_etch/battery_charging.png",
        "images/status/white_on_black_etch/battery_draining.png",
        "images/status/white_on_black_etch/battery_insides.png",
        "images/status/white_on_black_etch/battery_insides_low.png",
        "images/status/white_on_black_etch/wifi_0.png",
        "images/status/white_on_black_etch/wifi_1.png",
        "images/status/white_on_black_etch/wifi_2.png",
        "images/status/white_on_black_etch/wifi_3.png",
        
        // Status bar - white on black shadow
        "images/status/white_on_black_shadow/battery_charged.png",
        "images/status/white_on_black_shadow/battery_charging.png",
        "images/status/white_on_black_shadow/battery_draining.png",
        "images/status/white_on_black_shadow/battery_insides.png",
        "images/status/white_on_black_shadow/battery_insides_low.png",
        "images/status/white_on_black_shadow/wifi_0.png",
        "images/status/white_on_black_shadow/wifi_1.png",
        "images/status/white_on_black_shadow/wifi_2.png",
        "images/status/white_on_black_shadow/wifi_3.png",
        
        // Status bar - bases
        "images/status/black_base.png",
        "images/status/recording_base.png",
        "images/status/recording_glow.png",
        "images/status/silver_base.png",
        "images/status/tethering_base.png",
        "images/status/tethering_glow.png",
        "images/status/tethering_single_base.png",
        "images/status/top.png",
        "images/status/translucent_base.png",
        
        // UIKit components
        "images/uikit/UIButtonBarBackgroundTallTinted@2x.png",
        "images/uikit/UIButtonBarBlackOpaqueBackgroundTall@2x.png",
        "images/uikit/UINavigationBar_bg.png",
        "images/uikit/UINavigationBar_button.png",
        "images/uikit/UINavigationBar_button_back.png",
        "images/uikit/UINavigationBar_button_back_pressed.png",
        "images/uikit/UINavigationBar_button_pressed.png",
        "images/uikit/UIPinstripe.png",
        "images/uikit/UISwitch_shape_mask.png",
        "images/uikit/UISwitch_shape_shadow.png",
        "images/uikit/UISwitch_thumb.png",
        "images/uikit/UISwitch_thumb_pressed.png",
        "images/uikit/UITableNext@2x.png",
        "images/uikit/UITableSelection@2x.png",
        
        // Settings icons
        "images/Applications/Preferences/AppStore@2x.png",
        "images/Applications/Preferences/Camera@2x.png",
        "images/Applications/Preferences/Location@2x.png",
        "images/Applications/Preferences/Safari@2x.png",
        "images/Applications/Preferences/Settings-Air@2x.png",
        "images/Applications/Preferences/Settings-Display@2x.png",
        "images/Applications/Preferences/Settings-Sound@2x.png",
        "images/Applications/Preferences/Settings@2x.png",
        "images/Applications/Preferences/TwitterIcon@2x.png",
        "images/Applications/Preferences/airport@2x.png",
        "images/Applications/Preferences/cloud@2x.png",
        "images/Applications/Preferences/mmc@2x.png",
        "images/Applications/Preferences/music@2x.png",
        "images/Applications/Preferences/notes@2x.png",
        "images/Applications/Preferences/notifications_icon@2x.png",
        "images/Applications/Preferences/phone@2x.png",
        "images/Applications/Preferences/photos@2x.png",
        "images/Applications/Preferences/sms@2x.png",
        "images/Applications/Preferences/twitter@2x.png",
        "images/Applications/Preferences/vc@2x.png",
        "images/Applications/Preferences/video@2x.png",
        "images/Applications/Preferences/wallpaper@2x.png"
    };
    
    public ImagePreloadService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _totalCount = ImagePaths.Length;
    }
    
    /// <summary>
    /// Preload all images. Should be called once at app startup.
    /// </summary>
    public async Task PreloadAllImagesAsync()
    {
        if (_isPreloaded)
        {
            return; // Already preloaded
        }
        
        try
        {
            // Call JavaScript to preload all images
            await _jsRuntime.InvokeVoidAsync("preloadImages", ImagePaths, 
                DotNetObjectReference.Create(this));
        }
        catch (Exception ex)
        {
            global::System.Diagnostics.Debug.WriteLine($"Image preload error: {ex.Message}");
            // Mark as completed even on error to prevent blocking
            _isPreloaded = true;
            OnCompleted?.Invoke();
        }
    }
    
    /// <summary>
    /// Called from JavaScript when an image loads
    /// </summary>
    [JSInvokable]
    public void OnImageLoaded()
    {
        _loadedCount++;
        OnProgressChanged?.Invoke(_loadedCount, _totalCount);
        
        if (_loadedCount >= _totalCount)
        {
            _isPreloaded = true;
            OnCompleted?.Invoke();
        }
    }
    
    /// <summary>
    /// Called from JavaScript when an image fails to load
    /// </summary>
    [JSInvokable]
    public void OnImageError(string path)
    {
        global::System.Diagnostics.Debug.WriteLine($"Failed to preload image: {path}");
        // Still count it as loaded to avoid blocking
        _loadedCount++;
        OnProgressChanged?.Invoke(_loadedCount, _totalCount);
        
        if (_loadedCount >= _totalCount)
        {
            _isPreloaded = true;
            OnCompleted?.Invoke();
        }
    }
}
