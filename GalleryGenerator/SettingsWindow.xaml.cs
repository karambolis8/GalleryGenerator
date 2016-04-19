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
            int val;
            bool checkbox;

            this.MediumImgWidthTextBox.Text = ParseDefaultIntSetting("MediumImgWidth", out val);
            Settings.Default.MediumImgWidth = val;

            this.MediumImgHeightTextBox.Text = ParseDefaultIntSetting("MediumImgHeight", out val);
            Settings.Default.MediumImgHeight = val;

            this.ThumbImgWidthTextBox.Text = ParseDefaultIntSetting("ThumbImgWidth", out val);
            Settings.Default.ThumbImgWidth = val;

            this.ThumbImgHeightTextBox.Text = ParseDefaultIntSetting("ThumbImgHeight", out val);
            Settings.Default.ThumbImgHeight = val;

            checkbox = ParseDefaultBoolSetting("PreserveMediumAspectRatio");
            this.PreserveAspectRatioCheckBox.IsChecked = checkbox;
            Settings.Default.PreserveMediumAspectRatio = checkbox;

            checkbox = ParseDefaultBoolSetting("CopyOriginalFiles");
            this.CopyOriginalFilesCheckBox.IsChecked = checkbox;
            Settings.Default.CopyOriginalFiles = checkbox;
        }

        private string ParseDefaultIntSetting(string settingKey, out int value)
        {
            var stringVal = (string) Settings.Default.Properties[settingKey].DefaultValue;
            value = Int32.Parse(stringVal);
            return stringVal;
        }

        private bool ParseDefaultBoolSetting(string settingKey)
        {
            return Boolean.Parse((string)Settings.Default.Properties[settingKey].DefaultValue);
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
