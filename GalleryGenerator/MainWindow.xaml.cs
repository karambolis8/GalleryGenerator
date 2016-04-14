using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Common;
using log4net;
using Configuration = Common.Configuration;

namespace GalleryGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MainWindow));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunButton_OnClick(object sender, RoutedEventArgs _)
        {
            if (ValidateInput())
            {
                var options = new UserOptions()
                {
                    GalleryName = ConfigurationManager.AppSettings["galleryName"],
                    InputDirectory = ConfigurationManager.AppSettings["inputDirectory"],
                    OutputDirectory = ConfigurationManager.AppSettings["outputDirectory"],
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
    }
}
