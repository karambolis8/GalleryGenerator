using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Common;
using GalleryGeneratorEngine;
using log4net;
using Configuration = Common.Configuration;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace GalleryGenerator
{
    public partial class MainWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MainWindow));

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            this.Title = string.Format("{0} v{1}", this.Title, ConfigurationManager.AppSettings["appVersion"]);
        }

        private void RunButton_OnClick(object sender, RoutedEventArgs _)
        {
            if (ValidateInput())
            {
                StopButton.IsEnabled = true;
                RunButton.IsEnabled = false;

                var options = GetUserOptionsFromUI();

                if (EstimateWorkTimeCheckBox.IsChecked.HasValue && EstimateWorkTimeCheckBox.IsChecked.Value)
                {
                    ProgressTextBlock.Text = "Estmating work time...";
                    WorkerProgressBar.IsIndeterminate = true;
                    worker.DoWork += DoCountingWork;
                    worker.RunWorkerCompleted += CountingWorkCompleted;
                    worker.WorkerReportsProgress = false;
                    worker.RunWorkerAsync(options);
                }
                else
                {
                    SetAndRunMaoinJobInWorker(options, false);
                }
            }
            else
            {
                MessageBox.Show("You have to fill all fields.", "Input validation", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private UserOptions GetUserOptionsFromUI()
        {
            return new UserOptions()
            {
                GalleryName = GalleryNameTextBox.Text,
                InputDirectory = InputDirTextBox.Text,
                OutputDirectory = OutputDirTextBox.Text,
                PreserveMediumAspectRatio = Configuration.DefaultPreserveMediumAspectRatio,
                CopyOriginalFiles = Configuration.DefaultCopyOriginalFiles,
                MediumX = Configuration.DefaultMediumWidth,
                MediumY = Configuration.DefaultMediumHeight,
                ThumbX = Configuration.DefaultThumbWidth,
                ThumbY = Configuration.DefaultThumbHeight
            };
        }

        private void SetAndRunMaoinJobInWorker(UserOptions options, bool workCounted)
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
            var counter = new ImageCounter(options);
            doWorkEventArgs.Result = counter.CountImages();
        }

        private void CountingWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerProgressBar.IsIndeterminate = false;
            worker.DoWork -= DoCountingWork;
            worker.RunWorkerCompleted -= CountingWorkCompleted;
            
            UserOptions options = GetUserOptionsFromUI();
            options.WorkSize = (long)e.Result;
            SetAndRunMaoinJobInWorker(options, options.WorkSize > 0);
        }

        private void DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var options = (UserOptions)doWorkEventArgs.Argument;
            
            var generator = new GalleryGeneratorEngine.GalleryGeneratorEngine(options);

            var senderWorker = (BackgroundWorker) sender;
            long counter = 0;
            generator.ProcessingFileEvent += file =>
            {
                var p = (double)counter/(double)options.WorkSize;
                senderWorker.ReportProgress((int)(p*100), file);
                counter++;
            };

            try
            {
                generator.StartTask();
            }
            catch (Exception e)
            {
                Logger.Error("General exception", e);
            }
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StopButton.IsEnabled = false;
            RunButton.IsEnabled = true;
            WorkerProgressBar.IsIndeterminate = false;
            WorkerProgressBar.Value = 100;
            ProgressTextBlock.Text = "Completed!";
            
            worker.DoWork -= DoWork;
            worker.RunWorkerCompleted -= WorkerCompleted;
            worker.ProgressChanged -= ProgressChanged;
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
            if(this.worker.WorkerSupportsCancellation)
                this.worker.CancelAsync();
            this.StopButton.IsEnabled = false;
        }
    }
}
