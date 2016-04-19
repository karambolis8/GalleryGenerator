using System;
using System.Collections.Generic;
using System.IO;

namespace GalleryGeneratorEngine
{
    public class GeneratorStatistics
    {
        public IDictionary<string, IList<FileInfo>> IgnoredFormats;
        
        public IList<KeyValuePair<FileInfo, Exception>> FailedFiles;

        public IList<KeyValuePair<DirectoryInfo, Exception>> FailedDirectories;
    }
}