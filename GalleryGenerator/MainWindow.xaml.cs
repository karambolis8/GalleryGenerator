using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Common;
using log4net;
using Configuration = Common.Configuration;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace GalleryGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
                worker.DoWork += DoWork;
                worker.RunWorkerCompleted += WorkerCompleted;
                worker.ProgressChanged += ProgressChanged;
                worker.WorkerReportsProgress = true;
                worker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("You have to fill all fields.", "Input validation", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //WorkerProgressBar. e.ProgressPercentage
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StopButton.IsEnabled = false;
            RunButton.IsEnabled = true;
        }

        private void DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            StopButton.IsEnabled = true;
            RunButton.IsEnabled = false;

            var options = new UserOptions()
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

            var generator = new GalleryGeneratorEngine.GalleryGeneratorEngine(options);

            try
            {
                generator.StartTask();
            }
            catch (Exception e)
            {
                Logger.Error("General exception", e);
            }
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
            this.worker.CancelAsync();
            this.StopButton.IsEnabled = false;
        }
    }
}
