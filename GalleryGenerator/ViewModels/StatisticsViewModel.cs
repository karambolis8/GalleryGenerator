using System;
using System.Collections.Generic;
using System.IO;
using Common.DataObjects;

namespace GalleryGenerator.ViewModels
{
    public class StatisticsViewModel
    {
        private GeneratorStatistics _statisticsData;

        private List<KeyValuePair<string, FileInfo>> unknownExtensions; 

        public StatisticsViewModel(GeneratorStatistics statisticsData)
        {
            this._statisticsData = statisticsData;
            
            this.unknownExtensions = new List<KeyValuePair<string, FileInfo>>();

            foreach (var pair in statisticsData.IgnoredFormats)
                foreach(var file in pair.Value)
                    this.unknownExtensions.Add(new KeyValuePair<string, FileInfo>(pair.Key, file));
        }

        public string WorkingTime
        {
            get
            {
                var time = this._statisticsData.TimeSpan;

                if (time.Hours > 0)
                    return string.Format("{0}:{1}:{2}h", time.Hours, time.Minutes, time.Seconds);

                if (time.Minutes > 0)
                    return string.Format("{0}:{1}m", time.Minutes, time.Seconds);

                return string.Format("{0}s", time.Seconds);

            }
        }

        public string FilesCount => this._statisticsData.FilesCount.ToString();

        public string ImagesCount => this._statisticsData.ImagesCount.ToString();

        public string OtherFilesCount => this._statisticsData.OtherFilesCount.ToString();

        public string FailedDirectoriesCount => this._statisticsData.FailedDirectories.Count.ToString();

        public string FailedImagesCount => this._statisticsData.FailedFiles.Count.ToString();

        public string UnknownFormatsCount => this._statisticsData.IgnoredFormats.Count.ToString();

        public IList<KeyValuePair<FileInfo, Exception>> FailedImages => this._statisticsData.FailedFiles;

        public IList<KeyValuePair<DirectoryInfo, Exception>> FailedDirectories => this._statisticsData.FailedDirectories;

        public List<KeyValuePair<string, FileInfo>> UnknownFormats => this.unknownExtensions;
    }
}