using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.IO;
using Common.Resources;
using log4net;

// http://findicons.com/pack/1637/file_icons_vs_2

namespace GalleryGeneratorEngine
{
    public class GalleryGeneratorEngine : GalleryGeneratorBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GalleryGeneratorEngine));

        private const string COPYRIGHT_YEAR = "{COPYRIGHT_YEAR}";
        private const string GALLERY = "{GALLERY}";
        private const string LIST_ITEMS = "{LIST_ITEMS}";
        private const string PAGE_NAME = "{PAGE_NAME}";
        private const string JS_DIRECTORY = "{JS_DIRECTORY}";
        private const string CSS_DIRECTORY = "{CSS_DIRECTORY}";
        private const string GALLERY_NAME = "{GALLERY_NAME}";

        private const string MENU_LABEL = "{MENU_LABEL}";
        private const string MENU = "{MENU}";
        private const string ROOT_LINK = "{ROOT_LINK}";
        private const string ROOT_NAME = "{ROOT_NAME}";
        private const string PARENT_LINK = "{PARENT_LINK}";
        private const string PARENT_NAME = "{PARENT_NAME}";
        private const string SIBLINGS = "{SIBLINGS}";
        private const string ITEM_LINK = "{ITEM_LINK}";
        private const string ITEM_NAME = "{ITEM_NAME}";
        private const string ACTIVE = "{ACTIVE}";
        private const string SUBMENU = "{SUBMENU}";

        private const string FILES_TABLE = "{FILES_TABLE}";
        private const string FILES_ROWS = "{FILES_ROWS}";
        private const string FILE_IMG_LINK = "{FILE_IMG_LINK}";
        private const string FILE_LINK = "{FILE_LINK}";
        private const string FILE_NAME = "{FILE_NAME}";

        private const string THUMBNAIL_PATH = "{THUMBNAIL_PATH}";
        private const string MEDIUM_PATH = "{MEDIUM_PATH}";
        private const string ORIGINAL_PATH = "{ORIGINAL_PATH}";
        private const string DOWNLOAD_ORIGINAL = "{DOWNLOAD_ORIGINAL}";
        private const string IMAGE_TITLE = "{IMAGE_TITLE}";

        protected override ILog Logger => logger;

        public GalleryGeneratorEngine(UserOptions options, Func<bool> cancellationPending, Action cancelWork)
            : base(options, cancellationPending, cancelWork)
        {
        }

        protected override void ProcessFiles(DirectoryInfo directoryInfo)
        {
            if (this.cancellationPending())
            {
                this.HandleCancelWork();
                return;
            }

            var files = directoryInfo.GetFiles();

            var images = files
                .Where(f => Configuration.ImageExtensions.Contains(f.Extension.ToLower()))
                .ToArray();

            var otherFiles = files
                .Where(f => Configuration.FileExtensions.Contains(f.Extension.ToLower()))
                .ToArray();

            var ignoredExts = files
                .Where(f =>
                        !Configuration.FileExtensions.Contains(f.Extension.ToLower()) &&
                        !Configuration.ImageExtensions.Contains(f.Extension.ToLower()));

            foreach (var f in ignoredExts)
            {
                var ext = f.Extension.ToLower();
                if (!this.ignoredFormats.ContainsKey(ext))
                    this.ignoredFormats.Add(ext, new List<FileInfo>());
                this.ignoredFormats[ext].Add(f);
            }

            string nesting = this.GetNesting(directoryInfo);

            string mediumDirWithNesting = Path.Combine(this.options.OutputDirectory, Path.Combine(Configuration.MediumDir, nesting));
            if (!Directory.Exists(mediumDirWithNesting))
            {
                Directory.CreateDirectory(mediumDirWithNesting);
            }

            string thumbDirWithNesting = Path.Combine(this.options.OutputDirectory, Path.Combine(Configuration.ThumbDir, nesting));
            if (!Directory.Exists(thumbDirWithNesting))
            {
                Directory.CreateDirectory(thumbDirWithNesting);
            }

            this.GenerateThumbnails(images, nesting);
            this.GeneratePage(directoryInfo, images, otherFiles, nesting);
        }

        private string GetNesting(DirectoryInfo directory)
        {
            var nesting = directory.FullName;
            return nesting.Replace(this.options.InputDirectory, string.Empty).Trim(Path.DirectorySeparatorChar);
        }

        private string GetReverseNesting(string nesting)
        {
            if (nesting == string.Empty)
                return string.Empty;

            if (!nesting.Contains(Path.DirectorySeparatorChar))
                return "..";

            int level = nesting.Count(c => c == Path.DirectorySeparatorChar);
            var reverseNesting = new StringBuilder(".." + Path.DirectorySeparatorChar);
            for (int i = 0; i < level; i++)
            {
                reverseNesting.Append("..");
                reverseNesting.Append(Path.DirectorySeparatorChar);
            }
            return reverseNesting.ToString().Trim(Path.DirectorySeparatorChar);
        }

        private void GenerateThumbnails(IEnumerable<FileInfo> images, string nesting)
        {
            string mediumWithNesting = Path.Combine(this.options.OutputDirectory, Path.Combine(Configuration.MediumDir, nesting));
            string thumbWithNesting = Path.Combine(this.options.OutputDirectory, Path.Combine(Configuration.ThumbDir, nesting));

            foreach (FileInfo image in images)
            {
                if (this.cancellationPending())
                {
                    this.HandleCancelWork();
                    return;
                }

                ReportProcessingFile(image);

                try
                {
                    image.ResizeImage(this.options.MediumX, this.options.MediumY, mediumWithNesting,
                        this.options.PreserveMediumAspectRatio);
                    image.ResizeImage(this.options.ThumbX, this.options.ThumbY, thumbWithNesting);
                }
                catch (Exception e)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("File resize error: ");
                    sb.Append(image.FullName);
                    Logger.Error(sb.ToString(), e);

                    this.failedFiles.Add(new KeyValuePair<FileInfo, Exception>(image, e));
                }

                if (this.options.CopyOriginalFiles)
                {
                    if (this.cancellationPending())
                    {
                        this.HandleCancelWork();
                        return;
                    }

                    string destPath = Path.Combine(this.options.OriginalImgDir, Path.Combine(nesting, image.Name));
                    File.Copy(image.FullName, destPath);
                }
            }
        }

        private void GeneratePage(DirectoryInfo directoryInfo, IEnumerable<FileInfo> images, IEnumerable<FileInfo> otherFiles, string nesting)
        {
            if (this.cancellationPending())
            {
                this.HandleCancelWork();
                return;
            }

            string pageName = directoryInfo.Name;

            var menu = nesting == string.Empty ? this.GenerateMenuRoot(directoryInfo) : this.GenerateMenu(directoryInfo, nesting);
            string gallery = images.Any() ? this.GenerateGallery(images, nesting) : string.Empty;
            var filesTable = otherFiles.Any() ? this.GenerateFilesTable(otherFiles, nesting) : string.Empty;

            string pageContent = string.Empty;

            string reverseNesting = this.GetReverseNesting(nesting);
            string cssPath = Path.Combine(reverseNesting, Configuration.CssDir).GetBrowserPath();
            string jsPath = Path.Combine(reverseNesting, Configuration.JsDir).GetBrowserPath();

            var pageFormatSb = new StringBuilder(Configuration.PageFormat);
            pageContent = pageFormatSb
                .Replace(COPYRIGHT_YEAR, Configuration.CopyrightYear)
                .Replace(GALLERY_NAME, this.options.GalleryName)
                .Replace(PAGE_NAME, pageName)
                .Replace(GALLERY, gallery)
                .Replace(CSS_DIRECTORY, cssPath)
                .Replace(JS_DIRECTORY, jsPath)
                .Replace(MENU, menu)
                .Replace(FILES_TABLE, filesTable)
                .ToString();

            AssureRelativeDirectoryExists(nesting);
            string pagePath = nesting == string.Empty ? Path.Combine(this.options.OutputDirectory, this.options.GalleryName + ".html") : Path.Combine(this.options.OutputDirectory, Path.Combine(nesting, pageName + ".html"));

            if (this.cancellationPending())
            {
                this.HandleCancelWork();
                return;
            }

            try
            {
                using (var sw = new StreamWriter(pagePath, false, Encoding.UTF8))
                {
                    sw.Write(pageContent);
                }
            }
            catch (PathTooLongException e)
            {
                this.failedDirectories.Add(new KeyValuePair<DirectoryInfo,Exception>(directoryInfo, e));
            }
        }

        private string GenerateFilesTable(IEnumerable<FileInfo> otherFiles, string nesting)
        {
            var sb = new StringBuilder();

            string reverseNesting = this.GetReverseNesting(nesting);
            string icoWithNesting = Path.Combine(reverseNesting, Configuration.IcoDir);

            var i = 0;
            foreach (var file in otherFiles)
            {
                this.ReportProcessingFile(file);

                string icon = Configuration.GetFileIcon(file.Extension);

                if (string.IsNullOrEmpty(icon))
                {
                    icon = Configuration.GetDefaultIcon();
                }

                icon = Path.Combine(icoWithNesting, icon);


                var row = new StringBuilder(Configuration.FilesTableRowTemplate)
                    .Replace(FILE_IMG_LINK, icon.GetBrowserPath())
                    .Replace(FILE_LINK, file.FullName.GetAbsoluteBrowserPath())
                    .Replace(FILE_NAME, file.Name)
                    .ToString();

                sb.AppendLine(row);
                i++;

                if (i%5 == 0)
                {
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr>");
                    i = 0;
                }
            }

            return Configuration.FilesTableTemplate.Replace(FILES_ROWS, sb.ToString());
        }

        private string GenerateGallery(IEnumerable<FileInfo> images, string nesting)
        {
            string reverseNesting = this.GetReverseNesting(nesting);
            string mediumWithNesting = Path.Combine(reverseNesting, Path.Combine(Configuration.MediumDir, nesting));
            string thumbWithNesting = Path.Combine(reverseNesting, Path.Combine(Configuration.ThumbDir, nesting));

            var galleryItems = new StringBuilder();
            foreach (var image in images)
            {
                galleryItems.AppendLine(GenerateItem(image, mediumWithNesting, thumbWithNesting));
            }

            return Configuration.GalleryTemplate.Replace(LIST_ITEMS, galleryItems.ToString());
        }

        private string GenerateMenu(DirectoryInfo directoryInfo, string nesting)
        {
            var parent = directoryInfo.Parent;
            List<DirectoryInfo> siblings = parent.GetDirectories().OrderBy(d => d.Name).ToList();

            string reverseNesting = this.GetReverseNesting(nesting);

            string rootLink = Path.Combine(reverseNesting, this.options.GalleryName + ".html");
            rootLink = rootLink.GetBrowserPath();
            
            string parentLink;
            string parentName;
            
            if(nesting.Contains(Path.DirectorySeparatorChar))
            {
                parentLink = Path.Combine("..", parent.Name + ".html");
                parentLink = parentLink.GetBrowserPath();
                parentName = parent.Name;
            }
            else
            {
                parentLink = rootLink;
                parentName = this.options.GalleryName;
            }

            var siblingList = new StringBuilder();

            if(siblings.Any())
            {
                foreach (var sibling in siblings)
                {
                    var itemLink = Path.Combine("..", Path.Combine(sibling.Name, sibling.Name + ".html"));
                    itemLink = itemLink.GetBrowserPath();
                    var item = new StringBuilder(Configuration.SiblingMenuTemplate)
                        .Replace(ITEM_NAME, sibling.Name)
                        .Replace(ITEM_LINK, itemLink);

                    if (sibling.Name == directoryInfo.Name)
                    {
                        var submenu = this.GetSubmenu(sibling);
                        item
                            .Replace(ACTIVE, " class=\"active\"")
                            .Replace(SUBMENU, submenu);
                    }
                    else
                    {
                        item
                            .Replace(ACTIVE, string.Empty)
                            .Replace(SUBMENU, string.Empty);
                    }

                    siblingList.AppendLine(item.ToString());
                }
            }

            var menu = new StringBuilder(Configuration.RootMenuTemplate)
                .Replace(MENU_LABEL, Templates.MenuLabel)
                .Replace(ROOT_LINK, rootLink)
                .Replace(ROOT_NAME, this.options.GalleryName)
                .Replace(PARENT_LINK, parentLink)
                .Replace(PARENT_NAME, parentName)
                .Replace(SIBLINGS, siblingList.ToString())
                .ToString();

            return menu;
        }

        private string GenerateMenuRoot(DirectoryInfo directoryInfo)
        {
            string rootLink = this.options.GalleryName + ".html";

            var siblingList = new StringBuilder();
            var item = new StringBuilder(Configuration.SiblingMenuTemplate)
                .Replace(ITEM_NAME, this.options.GalleryName)
                .Replace(ITEM_LINK, rootLink);

            var submenu = this.GetSubmenu(directoryInfo);
            item
                .Replace(ACTIVE, " class=\"active\"")
                .Replace(SUBMENU, submenu);
            siblingList.AppendLine(item.ToString());

            var menu = new StringBuilder(Configuration.RootMenuTemplate)
                .Replace(MENU_LABEL, Templates.MenuLabel)
                .Replace(ROOT_LINK, rootLink)
                .Replace(ROOT_NAME, this.options.GalleryName)
                .Replace(PARENT_LINK, rootLink)
                .Replace(PARENT_NAME, this.options.GalleryName)
                .Replace(SIBLINGS, siblingList.ToString())
                .ToString();

            return menu;
        }

        private string GetSubmenu(DirectoryInfo activeSibling)
        {
            var children = activeSibling.GetDirectories();
            if (children.Length < 1)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("<ul>");

            foreach (var child in children)
            {
                var itemLink = Path.Combine(child.Name, child.Name + ".html");
                itemLink = itemLink.GetBrowserPath();
                var childBuilder = new StringBuilder(Configuration.SiblingMenuTemplate)
                    .Replace(ACTIVE, string.Empty)
                    .Replace(SUBMENU, string.Empty)
                    .Replace(ITEM_NAME, child.Name)
                    .Replace(ITEM_LINK, itemLink)
                    .ToString();
                sb.AppendLine(childBuilder);
            }

            sb.AppendLine("</ul>");

            return sb.ToString();
        }

        private string GenerateItem(FileInfo image, string mediumDirWithNesting, string thumbDirWithNesting)
        {

            string originalPath;
            if(this.options.CopyOriginalFiles)
                originalPath = Path.Combine(this.options.OriginalImgDir, image.Name).GetBrowserPath();
            else
                originalPath = image.FullName.GetAbsoluteBrowserPath();

            string mediumThumPath = Path.Combine(mediumDirWithNesting, image.JpgFileName()).GetBrowserPath();
            string smallThumPath = Path.Combine(thumbDirWithNesting, image.JpgFileName()).GetBrowserPath();

            var imageFormatSb = new StringBuilder(Configuration.ImageFormat);
            return imageFormatSb
                .Replace(THUMBNAIL_PATH, smallThumPath)
                .Replace(MEDIUM_PATH, mediumThumPath)
                .Replace(ORIGINAL_PATH, originalPath)
                .Replace(DOWNLOAD_ORIGINAL, Templates.DownloadOriginal)
                .Replace(IMAGE_TITLE, image.Name)
                .ToString();
        }
    }
}
