﻿using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Common.DataObjects;
using GalleryGenerator.Helpers;
using GalleryGenerator.Properties;
using GalleryGenerator.Resources.Translations;
using GalleryGeneratorEngine.DirectoryProcessors;
using log4net;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using DialogResult = System.Windows.Forms.DialogResult;

namespace GalleryGenerator.Windows
{
    public partial class MainWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MainWindow));

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            this.Title = string.Format("{0} v{1}", Translations.AppName, ConfigurationManager.AppSettings["appVersion"]);
            this.Left = WindowsSettings.Default.AppPositionLeft;
            this.Top = WindowsSettings.Default.AppPositionTop;
            this.Width = WindowsSettings.Default.AppWidth;

            SetAndStoreAppLanguage(WindowsSettings.Default.AppLanguage);
        }

        private void MainWindow_OnLocationChanged(object sender, EventArgs e)
        {
            WindowsSettings.Default.AppPositionLeft = this.Left;
            WindowsSettings.Default.AppPositionTop = this.Top;
            WindowsSettings.Default.Save();
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            WindowsSettings.Default.AppWidth = this.Width;
            WindowsSettings.Default.Save();
        }

        private void RunButton_OnClick(object sender, RoutedEventArgs _)
        {
            if (ValidateInput())
            {
                StopButton.IsEnabled = true;
                RunButton.IsEnabled = false;
                EnableInputs(false);

                var options = GetUserOptionsFromUI();
                
                if (UserSettings.Default.EstimateWorkTime)
                {
                    ProgressTextBlock.Text = Translations.EstimatingWorkTime;
                    WorkerProgressBar.IsIndeterminate = true;
                    worker.WorkerSupportsCancellation = true;
                    worker.DoWork += DoCountingWork;
                    worker.RunWorkerCompleted += CountingWorkCompleted;
                    worker.WorkerReportsProgress = false;
                    worker.RunWorkerAsync(options);
                }
                else
                {
                    SetAndRunMainJobInWorker(options, false);
                }
            }
            else
            {
                //replace with control highlighting
                MessageBox.Show(this, Translations.FillAllFields, Translations.InputValidation, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }
        }

        private void EnableInputs(bool enable)
        {
            MainMenu.IsEnabled = enable;
            GalleryNameTextBox.IsEnabled = enable;
            InputDirTextBox.IsEnabled = enable;
            BrowseInputButton.IsEnabled = enable;
            OutputDirTextBox.IsEnabled = enable;
            BrowseOutputButton.IsEnabled = enable;
            EstimateWorkTimeCheckBox.IsEnabled = enable;
        }

        private UserOptions GetUserOptionsFromUI()
        {
            return new UserOptions()
            {
                GalleryName = GalleryNameTextBox.Text,
                InputDirectory = InputDirTextBox.Text,
                OutputDirectory = OutputDirTextBox.Text,
                PreserveMediumAspectRatio = UserSettings.Default.PreserveMediumAspectRatio,
                CopyOriginalFiles = UserSettings.Default.CopyOriginalFiles,
                MediumX = UserSettings.Default.MediumImgWidth,
                MediumY = UserSettings.Default.MediumImgHeight,
                ThumbX = UserSettings.Default.ThumbImgWidth,
                ThumbY = UserSettings.Default.ThumbImgHeight,
                ThumbsGridSize = UserSettings.Default.ThumbGridSize,
                ImageExtensions = UserSettingsHelper.ImageExtenstions,
                FileExtensions = UserSettingsHelper.FileExtenstions
            };
        }

        private void SetAndRunMainJobInWorker(UserOptions options, bool workCounted)
        {
            ProgressTextBlock.Text = string.Empty;
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += WorkerCompleted;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += ProgressChanged;
            WorkerProgressBar.IsIndeterminate = !workCounted;
            worker.RunWorkerAsync(options);
        }

        private void DoCountingWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var options = (UserOptions)doWorkEventArgs.Argument;
            var counter = new FileCounter(options, () => this.worker.CancellationPending, () => { doWorkEventArgs.Cancel = true; });
            doWorkEventArgs.Result = counter.StartTask();
        }

        private void CountingWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerProgressBar.IsIndeterminate = false;
            worker.DoWork -= DoCountingWork;
            worker.RunWorkerCompleted -= CountingWorkCompleted;

            if (e.Cancelled)
            {
                ProgressTextBlock.Text = Translations.WorkCancelled;
                StopButton.IsEnabled = false;
                RunButton.IsEnabled = true;
                EnableInputs(true);
            }
            else if (e.Error != null)
            {
                HandleworkerException(e);
            }
            else
            {
                UserOptions options = GetUserOptionsFromUI();
                options.WorkSize = (long) e.Result;
                SetAndRunMainJobInWorker(options, options.WorkSize > 0);
            }
        }

        private void DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var start = DateTime.Now;

            var options = (UserOptions)doWorkEventArgs.Argument;
            
            var generator = new GalleryGeneratorEngine.DirectoryProcessors.GalleryGeneratorEngine(options, () => this.worker.CancellationPending, () => { doWorkEventArgs.Cancel = true; });

            var senderWorker = (BackgroundWorker) sender;
            long counter = 0;
            generator.ProcessingFileEvent += file =>
            {
                var p = (double)counter/(double)options.WorkSize;
                senderWorker.ReportProgress((int)(p*100), file);
                counter++;
            };

            var result = generator.StartTask();

            var end = DateTime.Now;
            result.TimeSpan = end - start;

            doWorkEventArgs.Result = result;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StopButton.IsEnabled = false;
            RunButton.IsEnabled = true;
            WorkerProgressBar.IsIndeterminate = false;
            
            worker.DoWork -= DoWork;
            worker.RunWorkerCompleted -= WorkerCompleted;
            worker.ProgressChanged -= ProgressChanged;

            EnableInputs(true);

            if (e.Cancelled)
            { 
                ProgressTextBlock.Text = Translations.WorkCancelled;
            }
            else if (e.Error != null)
            {
                HandleworkerException(e);
            }
            else
            {
                ProgressTextBlock.Text = Translations.WorkCompleted;
                WorkerProgressBar.Value = 100;
                var statistics = e.Result as GeneratorStatistics;

                OpenGeneratorSummary(statistics);

                if (UserSettings.Default.OpenAfterFinish)
                {
                    var path = Path.Combine(this.OutputDirTextBox.Text, this.GalleryNameTextBox.Text);

                    var explorerWindowProcess = new Process();
                    explorerWindowProcess.StartInfo.FileName = "explorer.exe";
                    explorerWindowProcess.StartInfo.Arguments = string.Format("/select,\"{0}.html\"", path);
                    explorerWindowProcess.Start();
                }
            }
        }

        private void OpenGeneratorSummary(GeneratorStatistics statistics)
        {
            var summaryWindow = new SummaryStatistics(statistics);
            summaryWindow.Top = WindowsSettings.Default.SummaryWindowTop;
            summaryWindow.Left = WindowsSettings.Default.SummaryWindowLeft;
            summaryWindow.Width = WindowsSettings.Default.SummaryWindowWidth;
            summaryWindow.Height = WindowsSettings.Default.SummaryWindowHeight;
            summaryWindow.Owner = this;
            summaryWindow.ShowDialog();
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgressBar.Value = e.ProgressPercentage;
            var fi = e.UserState as FileInfo;
            if (fi != null)
                ProgressTextBlock.Text = string.Format("{0}{1}{2}", fi.Directory?.Name , Path.DirectorySeparatorChar, fi.Name);
        }

        private bool ValidateInput()
        {
            return !(string.IsNullOrEmpty(GalleryNameTextBox.Text) ||
                   string.IsNullOrEmpty(InputDirTextBox.Text) ||
                   string.IsNullOrEmpty(OutputDirTextBox.Text));
        }

        private void browseInputButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                string inputDir = dialog.SelectedPath;
                InputDirTextBox.Text = inputDir;

                if (string.IsNullOrEmpty(GalleryNameTextBox.Text))
                {
                    var di = new DirectoryInfo(inputDir);
                    GalleryNameTextBox.Text = di.Name;
                }
            }
        }

        private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                string outputDir = dialog.SelectedPath;
                OutputDirTextBox.Text = outputDir;
            }
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.worker.WorkerSupportsCancellation)
            {
                if (ConfirmBreakingProcess())
                {
                    this.worker.CancelAsync();
                    this.StopButton.IsEnabled = false;
                }
            }
        }

        private void HandleworkerException(RunWorkerCompletedEventArgs eArgs)
        {
            ProgressTextBlock.Text = string.Format(Translations.ProcessInterruptedByError, eArgs.Error.GetType().Name);
            Logger.Error("General Exception. Generating gallery failed.", eArgs.Error);
            StopButton.IsEnabled = false;
            RunButton.IsEnabled = true;
            EnableInputs(true);
        }

        private bool ConfirmBreakingProcess()
        {
            return MessageBoxResult.Yes == MessageBox.Show(this, Translations.ConfirmBreakWorker, Translations.StopWork,
                MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Top = WindowsSettings.Default.SettingsWindowTop;
            settingsWindow.Left = WindowsSettings.Default.SettingsWindowLeft;
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void EstimateWorkTimeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UserSettings.Default.Save();
        }

        private void OpenAfterFinishCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UserSettings.Default.Save();
        }

        private void EnglishsLanguageMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SetAndStoreAppLanguage("en-GB");
        }

        private void PolishLanguageMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SetAndStoreAppLanguage("pl-PL");
        }

        private void SetAndStoreAppLanguage(string cultureCode)
        {
            var ci = new CultureInfo(cultureCode);
            CultureResources.ChangeCulture(ci);
            WindowsSettings.Default.AppLanguage = cultureCode;
            WindowsSettings.Default.Save();

            foreach (var item in LanguageMenuItem.Items)
            {
                var menuItem = item as MenuItem;

                if (menuItem == null)
                    continue;

                var tag = menuItem.Tag.ToString();
                menuItem.IsChecked = tag == cultureCode;
            }
        }
    }
}
