using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.DataObjects;
using Common.Helpers;

namespace GalleryGeneratorEngine.DirectoryProcessors
{
    public abstract class GalleryGeneratorBase : DirectoryTreeProcessorBase<GeneratorStatistics>
    {
        protected IDictionary<string, IList<FileInfo>> ignoredFormats;

        protected IList<KeyValuePair<FileInfo, Exception>> failedFiles;

        protected IList<KeyValuePair<DirectoryInfo, Exception>> failedDirectories;

        protected long totalFilesCount;

        protected long imagesCount;

        protected long otherFilesCount;

        protected GalleryGeneratorBase(UserOptions options, Func<bool> cancellationPending, Action cancelWork)
            : base(options, cancellationPending, cancelWork)
        {
            this.ignoredFormats = new Dictionary<string, IList<FileInfo>>();
            this.failedFiles = new List<KeyValuePair<FileInfo, Exception>>();
            this.failedDirectories = new List<KeyValuePair<DirectoryInfo, Exception>>();
            this.totalFilesCount = 0;
            this.imagesCount = 0;
            this.otherFilesCount = 0;
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
                
                Logger.Debug("Current directory: " + currentDir);

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
            this.WriteToLogList(this.failedFiles.Select(p => string.Format("{0}: {1}", p.Value.GetType().Name, p.Key.FullName)), "Failed files:");
            this.WriteToLogList(this.failedDirectories.Select(p => string.Format("{0}: {1}", p.Value.GetType().Name, p.Key.FullName)), "Failed directories:");

            var end = DateTime.Now;
            Logger.Info(string.Format("Gallery generated in {0}", end - start));

            return new GeneratorStatistics()
            {
                FailedFiles = this.failedFiles,
                FailedDirectories = this.failedDirectories,
                IgnoredFormats = this.ignoredFormats,
                FilesCount =  this.totalFilesCount,
                ImagesCount = this.imagesCount,
                OtherFilesCount = this.otherFilesCount
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

        private void WriteToLogList(IEnumerable<string> list, string title)
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
