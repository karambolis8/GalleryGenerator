using GalleryGenerator.Properties;

namespace GalleryGenerator
{
    public static class UserSettingsHelper
    {
        public static string[] ImageExtenstions => UserSettings.Default.ImageExtensions.Split('|');

        public static string[] FileExtenstions => UserSettings.Default.FileExtensions.Split('|');
    }
}