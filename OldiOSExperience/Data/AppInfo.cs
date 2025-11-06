namespace OldiOSExperience.Data // Make sure this namespace matches your project
{
    public class AppInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconPath { get; set; }

        // We can add more later, like which component to launch
        // public Type ComponentToLaunch { get; set; }
    }
}