using System;
using System.Configuration;
using System.Windows;
using Common;
using Configuration = Common.Configuration;

namespace GalleryGenerator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            
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
                Console.WriteLine(e);
            }
        }
    }
}
