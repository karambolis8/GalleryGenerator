using System;
using System.Configuration;
using System.IO;

namespace Common
{
    public static class Configuration
    {
        public static string[] ImageExtensions { get; private set; }
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
        public static string CopyrightYear { get; private set; }
        public static int DefaultMediumWidth { get; private set; }
        public static int DefaultMediumHeight { get; private set; }
        public static int DefaultThumbWidth { get; private set; }
        public static int DefaultThumbHeight { get; private set; }
        public static bool DefaultPreserveMediumAspectRatio { get; private set; }
        public static bool DefaultCopyOriginalFiles { get; private set; }
        
        static Configuration()
        {
            ImageExtensions = ConfigurationManager.AppSettings["imageExtensions"].Split('|');
            FileExtensions = ConfigurationManager.AppSettings["fileExtensions"].Split('|');

            CssDir = ConfigurationManager.AppSettings["cssDir"];
            JsDir = ConfigurationManager.AppSettings["jsDir"];
            IcoDir = ConfigurationManager.AppSettings["icoDir"];

            CopyrightYear = ConfigurationManager.AppSettings["copyrightYear"];

            bool parseBool;
            if (Boolean.TryParse(ConfigurationManager.AppSettings["defaultPreserveMediumAspectRatio"], out parseBool))
            {
                DefaultPreserveMediumAspectRatio = parseBool;
            }
            if (Boolean.TryParse(ConfigurationManager.AppSettings["defaultCopyOriginalFiles"], out parseBool))
            {
                DefaultCopyOriginalFiles = parseBool;
            }

            int parse;
            if (Int32.TryParse(ConfigurationManager.AppSettings["defaultMediumWidth"], out parse))
            {
                DefaultMediumWidth = parse;
            }
            if (Int32.TryParse(ConfigurationManager.AppSettings["defaultMediumHeight"], out parse))
            {
                DefaultMediumHeight = parse;
            }
            if (Int32.TryParse(ConfigurationManager.AppSettings["DefaultThumbWidth"], out parse))
            {
                DefaultThumbWidth = parse;
            }
            if (Int32.TryParse(ConfigurationManager.AppSettings["DefaultThumbHeight"], out parse))
            {
                DefaultThumbHeight = parse;
            }

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
            string ico = ConfigurationManager.AppSettings["ico-" + extension];

            if(ico == null)
                return ConfigurationManager.AppSettings["ico-default"];
            return ico;
        }
    }
}