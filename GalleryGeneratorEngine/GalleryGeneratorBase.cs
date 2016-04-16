using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.IO;
using log4net;

namespace GalleryGeneratorEngine
{
    public abstract class GalleryGeneratorBase : DirectoryTreeProcessorBase<GeneratorStatistics>
    {
        protected IDictionary<string, int> ignoredFormats;

        protected IList<string> failedFiles;

        protected GalleryGeneratorBase(UserOptions options, Func<bool> cancellationPending, Action cancelWork)
            : base(options, cancellationPending, cancelWork)
        {
            this.ignoredFormats = new Dictionary<string, int>();
            this.failedFiles = new List<string>();
        }

        public event Action<FileInfo> ProcessingFileEvent;

        protected abstract void ProcessFiles(DirectoryInfo directoryInfo);

        protected override GeneratorStatistics DoJob()
        {
            Logger.Info("Generating gallery started");
            var start = DateTime.Now;

            if (!Directory.Exists(this.options.OutputDirectory))
                Directory.CreateDirectory(this.options.OutputDirectory);

            AssureRelativeDirectoryExists(Configuration.MediumDir);
            AssureRelativeDirectoryExists(Configuration.ThumbDir);
            AssureRelativeDirectoryExists(Configuration.CssDir);
            AssureRelativeDirectoryExists(Configuration.JsDir);
            AssureRelativeDirectoryExists(Configuration.IcoDir);
            
            Logger.Info("Created output directories");

            CopyGalleryFiles("css", Configuration.CssDir);
            CopyGalleryFiles("js", Configuration.JsDir);
            CopyGalleryFiles("ico", Configuration.IcoDir);

            Logger.Info("Copied Gallerific files");

            var dirs = new Stack<string>(20);

            if(!Directory.Exists(options.InputDirectory))
                throw new ArgumentException();

            dirs.Push(options.InputDirectory);

            Logger.Info("Started processing directory tree");

            while(dirs.Any())
            {
                if (this.cancellationPending())
                {
                    this.HandleCancelWork();
                    return null;
                }

                string currentDir = dirs.Pop();
                
                Logger.Info("Current directory: " + currentDir);

                string[] subDirs;

                try
                {
                    subDirs = Directory.GetDirectories(currentDir).OrderBy(d => d).ToArray();
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.Error(this.GetType().Name, e);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Logger.Error(this.GetType().Name, e);
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
                    Logger.Error(this.GetType().Name, e);
                    //continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Logger.Error(this.GetType().Name, e);
                    //continue;
                }
            }

            this.WriteToLogList(this.ignoredFormats.Select(p => p.Key).ToList(), "Unknown formats:");
            this.WriteToLogList(this.failedFiles, "Failed files:");
            var end = DateTime.Now;
            Logger.Info(string.Format("Gallery generated in {0}", end - start));

            return new GeneratorStatistics()
            {
                FailedFiles = this.failedFiles,
                IgnoredFormats = this.ignoredFormats
            };
        }

        protected void AssureRelativeDirectoryExists(string relativePath)
        {
            var dir = Path.Combine(this.options.OutputDirectory, relativePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        protected void ReportProcessingFile(FileInfo fileInfo)
        {
            if (ProcessingFileEvent != null)
                ProcessingFileEvent(fileInfo);
        }

        private void WriteToLogList(IList<string> list, string title)
        {
            if (list.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine(title);
                foreach (var listItem in list)
                {
                    sb.Append("\t");
                    sb.AppendLine(listItem);
                }
                sb.Remove(sb.Length - 1, 1);
                Logger.Info(sb);
            }
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
