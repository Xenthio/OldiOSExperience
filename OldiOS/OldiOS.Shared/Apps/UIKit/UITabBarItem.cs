namespace OldiOS.Shared.Apps.UIKit
{
    public class UITabBarItem
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public int BadgeCount { get; set; } = 0;
        public bool IsSelected { get; set; } = false;
        public string Tag { get; set; } = "";
    }
}
