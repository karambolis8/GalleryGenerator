using System;
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
            this.MediumImgWidthTextBox.Text = (string) Settings.Default.Properties["MediumImgWidth"].DefaultValue;
            this.MediumImgHeightTextBox.Text = (string) Settings.Default.Properties["MediumImgHeight"].DefaultValue;
            this.ThumbImgWidthTextBox.Text = (string) Settings.Default.Properties["ThumbImgWidth"].DefaultValue;
            this.ThumbImgHeightTextBox.Text = (string) Settings.Default.Properties["ThumbImgHeight"].DefaultValue;

            string boolStr = (string)Settings.Default.Properties["PreserveMediumAspectRatio"].DefaultValue;
            this.PreserveAspectRatioCheckBox.IsChecked = boolStr == "True";
            boolStr = (string)Settings.Default.Properties["CopyOriginalFiles"].DefaultValue;
            this.CopyOriginalFilesCheckBox.IsChecked = boolStr == "True";

        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reload();
            this.Close();
        }

        private void SettingsWindow_OnLocationChanged(object sender, EventArgs e)
        {
            Settings.Default.SettingsWindowLeft = this.Left;
            Settings.Default.SettingsWindowTop = this.Top;
            Settings.Default.Save();
        }
    }
}
