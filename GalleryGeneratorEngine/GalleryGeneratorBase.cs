﻿using System;
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
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(GalleryGeneratorBase));

        protected UserOptions options;

        protected IDictionary<string, int> ignoredFormats;

        protected IList<string> failedFiles;

        protected GalleryGeneratorBase(UserOptions options)
        {
            this.options = options;

            this.ignoredFormats = new Dictionary<string, int>();

            this.failedFiles = new List<string>();
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
            Logger.Info("Application started");

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
                string currentDir = dirs.Pop();
                
                Logger.Info("Current directory: " + currentDir);

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

            this.WriteToLogList(this.ignoredFormats.Select(p => p.Key).ToList(), "Unknown formats:");
            this.WriteToLogList(this.failedFiles, "Failed files:");
            Logger.Info("Application pending");
        }

        protected void AssureRelativeDirectoryExists(string relativePath)
        {
            var dir = Path.Combine(this.options.OutputDirectory, relativePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
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
