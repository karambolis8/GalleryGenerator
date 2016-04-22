using System;
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
            this.FailedDirectoriesCount.Content = this.statistics.FailedDirectories.Count;
            this.FailedImagesCount.Content = this.statistics.FailedFiles.Count;
            this.UnknownFormatsCount.Content = this.statistics.IgnoredFormats.Count;
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
