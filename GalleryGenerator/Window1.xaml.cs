using System;
using System.Windows;
using Common;

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
                GalleryName = "test",
                InputDirectory = @"G:\modele\dokumentacja\A-10 Thunderbolt II",
                OutputDirectory = @"C:\test_galerii",
                MediumImgDir = "medium",
                ThumbImgDir = "thumb",
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
