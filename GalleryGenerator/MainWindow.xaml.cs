using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Common;
using GalleryGenerator.Properties;
using GalleryGenerator.Resources.Translations;
using GalleryGeneratorEngine;
using log4net;

namespace GalleryGenerator
{
    public partial class MainWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MainWindow));

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            this.Title = string.Format("{0} v{1}", Translations.AppName, ConfigurationManager.AppSettings["appVersion"]);
            this.Left = Settings.Default.AppPositionLeft;
            this.Top = Settings.Default.AppPositionTop;
            this.Width = Settings.Default.AppWidth;
        }

        private void MainWindow_OnLocationChanged(object sender, EventArgs e)
        {
            Settings.Default.AppPositionLeft = this.Left;
            Settings.Default.AppPositionTop = this.Top;
            Settings.Default.Save();
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Settings.Default.AppWidth = this.Width;
            Settings.Default.Save();
        }

        private void RunButton_OnClick(object sender, RoutedEventArgs _)
        {
            if (ValidateInput())
            {
                StopButton.IsEnabled = true;
                RunButton.IsEnabled = false;
                EnableInputs(false);

                var options = GetUserOptionsFromUI();
                
                if (EstimateWorkTimeCheckBox.IsChecked.HasValue && EstimateWorkTimeCheckBox.IsChecked.Value)
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
                System.Windows.MessageBox.Show("You have to fill all fields.", "Input validation", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
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
                PreserveMediumAspectRatio = Settings.Default.PreserveMediumAspectRatio,
                CopyOriginalFiles = Settings.Default.CopyOriginalFiles,
                MediumX = Settings.Default.MediumImgWidth,
                MediumY = Settings.Default.MediumImgHeight,
                ThumbX = Settings.Default.ThumbImgWidth,
                ThumbY = Settings.Default.ThumbImgHeight
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
            var options = (UserOptions)doWorkEventArgs.Argument;
            
            var generator = new GalleryGeneratorEngine.GalleryGeneratorEngine(options, () => this.worker.CancellationPending, () => { doWorkEventArgs.Cancel = true; });

            var senderWorker = (BackgroundWorker) sender;
            long counter = 0;
            generator.ProcessingFileEvent += file =>
            {
                var p = (double)counter/(double)options.WorkSize;
                senderWorker.ReportProgress((int)(p*100), file);
                counter++;
            };

            doWorkEventArgs.Result = generator.StartTask();
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
            }
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
            Logger.Info("General Exception. Generating gallery failed.");
            StopButton.IsEnabled = false;
            RunButton.IsEnabled = true;
            EnableInputs(true);
        }

        private bool ConfirmBreakingProcess()
        {
            var result = System.Windows.Forms.MessageBox.Show("Are you sure You want to stop? Work can't be resumed.",
                "Stop work", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return result == System.Windows.Forms.DialogResult.Yes;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Top = Settings.Default.SettingsWindowTop;
            settingsWindow.Left = Settings.Default.SettingsWindowLeft;
            settingsWindow.ShowDialog();
        }
    }
}
