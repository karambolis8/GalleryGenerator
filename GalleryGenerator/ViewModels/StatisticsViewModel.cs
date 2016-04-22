using System;
using System.Collections.Generic;
using System.IO;
using Common.DataObjects;

namespace GalleryGenerator.ViewModels
{
    public class StatisticsViewModel
    {
        private GeneratorStatistics _statisticsData;

        public StatisticsViewModel(GeneratorStatistics statisticsData)
        {
            this._statisticsData = statisticsData;
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

        public IDictionary<string, IList<FileInfo>> UnknownFormats => this._statisticsData.IgnoredFormats;
    }
}