using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using log4net;

namespace GalleryGeneratorEngine
{
    public class FileCounter : DirectoryTreeProcessorBase<long>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FileCounter));
        
        protected override ILog Logger => logger;

        public FileCounter(UserOptions options, Func<bool> cancellationPending, Action cancelWork)
            : base(options, cancellationPending, cancelWork)
        {
        }

        protected override long DoJob()
        {
            Logger.Info("Counting files started");
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

                Logger.Debug("Counting directory: " + currentDir);

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

            Logger.Info(string.Format("Found {0} files", filesCount));
            Logger.Info(string.Format("Counting images comleted in {0}", end - start));

            return filesCount;
        }

        private int ProcessFiles(DirectoryInfo directoryInfo)
        {
            var files = directoryInfo.GetFiles();
            return files.Count(f => Configuration.ImageExtensions.Contains(f.Extension.ToLower()) 
            || Configuration.FileExtensions.Contains(f.Extension.ToLower()));
        }
    }
}