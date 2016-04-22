﻿using System;
using System.ComponentModel;
using System.Windows;
using Common.DataObjects;

namespace GalleryGenerator.Windows
{
    public partial class SummaryStatistics : Window
    {
        private readonly GeneratorStatistics statistics;

        public SummaryStatistics(GeneratorStatistics statistics)
        {
            InitializeComponent();

            this.statistics = statistics;

            FillStatistics();
        }

        private void FillStatistics()
        {
            this.FileCountLabel.Content = this.statistics.FilesCount;
            this.ImageCountLabel.Content = this.statistics.ImagesCount;
            this.OtherFilesCountLabel.Content = this.statistics.OtherFilesCount;
            this.FailedDirectoriesCountLabel.Content = this.statistics.FailedDirectories.Count;
            this.FailedImagesCountLabel.Content = this.statistics.FailedFiles.Count;
            this.UnknownFormatsCountLabel.Content = this.statistics.IgnoredFormats.Count;
            this.TimespanLabel.Content = FormatWorkingTime();
        }

        private string FormatWorkingTime()
        {
            var time = this.statistics.TimeSpan;

            if (time.Hours > 0)
                return string.Format("{0}:{1}:{2}h", time.Hours, time.Minutes, time.Seconds);

            if(time.Minutes > 0)
                return string.Format("{0}:{1}m", time.Minutes, time.Seconds);

            return string.Format("{0}s", time.Seconds);
        }

        private void SummaryWindow_OnLocationChanged(object sender, EventArgs e)
        {
        }

        private void SummaryWindow_OnClosing(object sender, CancelEventArgs e)
        {
        }

        private void OKButton_OnClickButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
