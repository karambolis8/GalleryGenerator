using System;
using System.ComponentModel;
using System.Windows;
using Common.DataObjects;
using GalleryGenerator.Properties;
using GalleryGenerator.ViewModels;

namespace GalleryGenerator.Windows
{
    public partial class SummaryStatistics : Window
    {
        private StatisticsViewModel statisticsViewModel;

        public SummaryStatistics(GeneratorStatistics statistics)
        {
            InitializeComponent();

            this.statisticsViewModel = new StatisticsViewModel(statistics);
            this.DataContext = this.statisticsViewModel;
        }

        private void SummaryWindow_OnLocationChanged(object sender, EventArgs e)
        {
            WindowsSettings.Default.SummaryWindowTop = this.Top;
            WindowsSettings.Default.SummaryWindowLeft = this.Left;
            WindowsSettings.Default.Save();
        }

        private void SummaryWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            WindowsSettings.Default.SummaryWindowWidth = this.Width;
            WindowsSettings.Default.SummaryWindowHeight = this.Height;
            WindowsSettings.Default.Save();
        }

        private void SummaryWindow_OnClosing(object sender, CancelEventArgs e)
        {
            // ask if one want to add unknown extensions if occurs
        }

        private void OKButton_OnClickButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
