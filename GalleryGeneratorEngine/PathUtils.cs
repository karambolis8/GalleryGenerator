using System.IO;

namespace GalleryGeneratorEngine
{
    public static class PathUtils
    {
        public static string GetAbsoluteBrowserPath(this string path)
        {
            return "file://" + GetBrowserPath(path);
        }

        public static string GetBrowserPath(this string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        public static string JpgFileName(this FileInfo image)
        {
            int dot = image.Name.LastIndexOf('.');
            string fileName = image.Name.Remove(dot);
            return fileName + ".jpg";
        }
    }
}