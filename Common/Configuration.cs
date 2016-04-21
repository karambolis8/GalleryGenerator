using System;
using System.Configuration;
using System.IO;

namespace Common
{
    public static class Configuration
    {
        public static string[] FileExtensions { get; private set; }

        public static string PageFormat { get; private set; }
        public static string ImageFormat { get; private set; }
        public static string GalleryTemplate { get; private set; }
        public static string RootMenuTemplate { get; private set; }
        public static string SiblingMenuTemplate { get; private set; }
        public static string FilesTableTemplate { get; private set; }
        public static string FilesTableRowTemplate { get; private set; }
        public static string CssDir { get; private set; }
        public static string JsDir { get; private set; }
        public static string IcoDir { get; private set; }
        public static string MediumDir { get; private set; }
        public static string ThumbDir { get; private set; }
        public static string CopyrightYear { get; private set; }
        
        static Configuration()
        {
            CssDir = ConfigurationManager.AppSettings["cssDir"];
            JsDir = ConfigurationManager.AppSettings["jsDir"];
            IcoDir = ConfigurationManager.AppSettings["icoDir"];
            MediumDir = ConfigurationManager.AppSettings["mediumDir"];
            ThumbDir = ConfigurationManager.AppSettings["thumbDir"];

            CopyrightYear = ConfigurationManager.AppSettings["copyrightYear"];

            string pageTemplatePath = ConfigurationManager.AppSettings["galleryPageTemplate"];
            using (var sr = new StreamReader(pageTemplatePath))
            {
                PageFormat = sr.ReadToEnd();
            } 
            
            string itemTemplatePath = ConfigurationManager.AppSettings["galleryItemTemplate"];
            using (var sr = new StreamReader(itemTemplatePath))
            {
                ImageFormat = sr.ReadToEnd();
            } 
            
            string rootMenuTemplatePath = ConfigurationManager.AppSettings["rootMenuTemplate"];
            using (var sr = new StreamReader(rootMenuTemplatePath))
            {
                RootMenuTemplate = sr.ReadToEnd();
            }

            string siblingMenuTemplatePath = ConfigurationManager.AppSettings["siblingMenuTemplate"];
            using (var sr = new StreamReader(siblingMenuTemplatePath))
            {
                SiblingMenuTemplate = sr.ReadToEnd();
            }

            string galleryTemplatePath = ConfigurationManager.AppSettings["galleryTemplate"];
            using (var sr = new StreamReader(galleryTemplatePath))
            {
                GalleryTemplate = sr.ReadToEnd();
            }

            string filesTableTemplatePath = ConfigurationManager.AppSettings["filesTableTemplate"];
            using (var sr = new StreamReader(filesTableTemplatePath))
            {
                FilesTableTemplate = sr.ReadToEnd();
            }

            string filesTableRowTemplatePath = ConfigurationManager.AppSettings["filesTableRowTemplate"];
            using (var sr = new StreamReader(filesTableRowTemplatePath))
            {
                FilesTableRowTemplate = sr.ReadToEnd();
            }
        }

        public static string GetFileIcon(string extension)
        {
            extension = extension.Replace(".", string.Empty);
            return ConfigurationManager.AppSettings["ico-" + extension];
        }

        public static string GetDefaultIcon()
        {
            return ConfigurationManager.AppSettings["ico-default"];
        }
    }
}