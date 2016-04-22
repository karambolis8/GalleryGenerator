using System;
using System.ComponentModel;
using System.Windows;
using Common.DataObjects;
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
