using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.IO;
using log4net;

namespace GalleryGeneratorEngine
{
    public abstract class GalleryGeneratorBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GalleryGeneratorBase));

        protected UserOptions options;

        protected IList<string> ignoredFormats;

        protected GalleryGeneratorBase(UserOptions options)
        {
            this.options = options;

            this.ignoredFormats = new List<string>();
        }

        public event Action<int> ReportProgressEvent;

        protected void ReportProgress(int percentage)
        {
            if (this.ReportProgressEvent != null)
                this.ReportProgressEvent(percentage);
        }

        protected abstract void ProcessFiles(DirectoryInfo directoryInfo);

        public void StartTask()
        {
            if (!Directory.Exists(this.options.OutputDirectory))
                Directory.CreateDirectory(this.options.OutputDirectory);

            AssureRelativeDirectoryExists(Configuration.MediumDir);
            AssureRelativeDirectoryExists(Configuration.ThumbDir);
            AssureRelativeDirectoryExists(Configuration.CssDir);
            AssureRelativeDirectoryExists(Configuration.JsDir);
            AssureRelativeDirectoryExists(Configuration.IcoDir);

            CopyGalleryFiles("css", Configuration.CssDir);
            CopyGalleryFiles("js", Configuration.JsDir);
            CopyGalleryFiles("ico", Configuration.IcoDir);

            var dirs = new Stack<string>(20);

            if(!Directory.Exists(options.InputDirectory))
                throw new ArgumentException();

            dirs.Push(options.InputDirectory);

            while(dirs.Any())
            {
                string currentDir = dirs.Pop();

                string[] subDirs;

                try
                {
                    subDirs = Directory.GetDirectories(currentDir).OrderBy(d => d).ToArray();
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.Error("GalleryGeneratorBase", e);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Logger.Error("GalleryGeneratorBase", e);
                    continue;
                }

                foreach (var dir in subDirs)
                    dirs.Push(dir);

                try
                {
                    var di = new DirectoryInfo(currentDir);
                    ProcessFiles(di);
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.Error("GalleryGeneratorBase", e);
                    //continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Logger.Error("GalleryGeneratorBase", e);
                    //continue;
                }
            }

            if (this.ignoredFormats.Any())
            {
                foreach (var ignoredFormat in ignoredFormats)
                {
                    Logger.Info("Ignored format: " + ignoredFormat);
                }
            }
            else
            {
                Logger.Info("No ignored formats");
            }
        }

        protected void AssureRelativeDirectoryExists(string relativePath)
        {
            var dir = Path.Combine(this.options.OutputDirectory, relativePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void CopyGalleryFiles(string inputDir, string outputDir)
        {
            var appDir = Directory.GetCurrentDirectory();
            var inputInfo = new DirectoryInfo(Path.Combine(appDir, Path.Combine("Gallerific", inputDir)));

            foreach (var fileInfo in inputInfo.GetFiles())
            {
                File.Copy(fileInfo.FullName, Path.Combine(this.options.OutputDirectory, Path.Combine(outputDir, fileInfo.Name)), true);
            }
        }
    }
}
