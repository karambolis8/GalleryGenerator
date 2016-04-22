using System;
using System.Collections.Generic;
using System.IO;

namespace Common.DataObjects
{
    public class GeneratorStatistics
    {
        public IDictionary<string, IList<FileInfo>> IgnoredFormats { get; set; }

        public IList<KeyValuePair<FileInfo, Exception>> FailedFiles { get; set; }

        public IList<KeyValuePair<DirectoryInfo, Exception>> FailedDirectories { get; set; }

        public long FilesCount { get; set; }

        public long ImagesCount { get; set; }

        public long OtherFilesCount { get; set; }

        public TimeSpan TimeSpan { get; set; }
    }
}