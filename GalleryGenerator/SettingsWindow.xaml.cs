using System.Windows;
using GalleryGenerator.Properties;

namespace GalleryGenerator
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            //var defWidth = Settings.Default.Properties["AppWidth"].DefaultValue;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
