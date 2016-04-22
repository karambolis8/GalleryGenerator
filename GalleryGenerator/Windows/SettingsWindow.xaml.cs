using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalleryGenerator.Properties;
using GalleryGenerator.Resources.Translations;

namespace GalleryGenerator.Windows
{
    public partial class SettingsWindow : Window
    {
        private bool unsavedChanges;

        public SettingsWindow()
        {
            InitializeComponent();

            this.ReactOnChanges(false);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            int val;
            bool checkbox;

            this.MediumImgWidthTextBox.Text = ParseDefaultIntSetting("MediumImgWidth", out val);
            UserSettings.Default.MediumImgWidth = val;

            this.MediumImgHeightTextBox.Text = ParseDefaultIntSetting("MediumImgHeight", out val);
            UserSettings.Default.MediumImgHeight = val;

            this.ThumbImgWidthTextBox.Text = ParseDefaultIntSetting("ThumbImgWidth", out val);
            UserSettings.Default.ThumbImgWidth = val;

            this.ThumbImgHeightTextBox.Text = ParseDefaultIntSetting("ThumbImgHeight", out val);
            UserSettings.Default.ThumbImgHeight = val;

            this.ThumbsGridSizeTextBox.Text = ParseDefaultIntSetting("ThumbGridSize", out val);
            UserSettings.Default.ThumbGridSize = val;

            checkbox = ParseDefaultBoolSetting("PreserveMediumAspectRatio");
            this.PreserveAspectRatioCheckBox.IsChecked = checkbox;
            UserSettings.Default.PreserveMediumAspectRatio = checkbox;

            checkbox = ParseDefaultBoolSetting("CopyOriginalFiles");
            this.CopyOriginalFilesCheckBox.IsChecked = checkbox;
            UserSettings.Default.CopyOriginalFiles = checkbox;

            this.ImagesExtensionsTextBox.Text = GetDefaultStringSetting("ImageExtensions");
            UserSettings.Default.ImageExtensions = GetDefaultStringSetting("ImageExtensions");

            this.OtherFilesExtensionsTextBox.Text = GetDefaultStringSetting("FileExtensions");
            UserSettings.Default.FileExtensions = GetDefaultStringSetting("FileExtensions");

            this.ReactOnChanges(true);
        }

        private string ParseDefaultIntSetting(string settingKey, out int value)
        {
            var stringVal = (string) UserSettings.Default.Properties[settingKey].DefaultValue;
            value = Int32.Parse(stringVal);
            return stringVal;
        }

        private bool ParseDefaultBoolSetting(string settingKey)
        {
            return Boolean.Parse((string)UserSettings.Default.Properties[settingKey].DefaultValue);
        }

        private string GetDefaultStringSetting(string settingKey)
        {
            return UserSettings.Default.Properties[settingKey].DefaultValue.ToString();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            UserSettings.Default.Save();
            this.ReactOnChanges(false);
            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            UserSettings.Default.Reload();
            this.ReactOnChanges(false);
            this.Close();
        }

        private void SettingsWindow_OnLocationChanged(object sender, EventArgs e)
        {
            WindowsSettings.Default.SettingsWindowLeft = this.Left;
            WindowsSettings.Default.SettingsWindowTop = this.Top;
            WindowsSettings.Default.Save();
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.ReactOnChanges(true);
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            this.ReactOnChanges(true);
        }

        private void ReactOnChanges(bool changes)
        {
            if (this.SaveButton == null || this.CancelButton == null)
            {
                this.unsavedChanges = false;
                return;
            }

            this.unsavedChanges = changes;
            this.SaveButton.IsEnabled = changes;
            this.CancelButton.IsEnabled = changes;
        }

        private void SettingsWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (this.unsavedChanges)
            {
                var result = MessageBox.Show(this, Translations.CloseWithoutSavingChanges,
                    Translations.SettingsChanges, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    UserSettings.Default.Reload();
            }
        }
    }
}
