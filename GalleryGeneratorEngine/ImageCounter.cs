using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using log4net;

namespace GalleryGeneratorEngine
{
    public class ImageCounter : DirectoryTreeProcessorBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ImageCounter));
        
        protected override ILog Logger => logger;

        public ImageCounter(UserOptions options, Func<bool> cancellationPending, Action cancelWork)
            : base(options, cancellationPending, cancelWork)
        {
        }

        public long CountImages()
        {
            Logger.Info("Counting images started");
            var start = DateTime.Now;

            long filesCount = 0;
            var dirs = new Stack<string>(20);

            if (!Directory.Exists(options.InputDirectory))
                throw new ArgumentException();

            dirs.Push(options.InputDirectory);

            while (dirs.Any())
            {
                if (this.cancellationPending())
                {
                    this.HandleCancelWork();
                    return 0;
                }

                string currentDir = dirs.Pop();

                Logger.Info("Counting directory: " + currentDir);

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
                    filesCount += ProcessFiles(di);
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

            var end = DateTime.Now;

            Logger.Info(string.Format("Found {0} image files", filesCount));
            Logger.Info(string.Format("Counting images comleted in {0}", end - start));

            return filesCount;
        }

        private int ProcessFiles(DirectoryInfo directoryInfo)
        {
            var files = directoryInfo.GetFiles();
            return files.Count(f => Configuration.ImageExtensions.Contains(f.Extension.ToLower()));
        }
    }
}