using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Text;
using log4net;

namespace GalleryGeneratorEngine
{
    static class ImageResizer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ImageResizer));

        public static void ResizeImage(this FileInfo image, int maxWidth, int maxHeight, string destDir, bool preserveAspectRatio = false)
        {
            try
            {
                var bmp = new Bitmap(image.FullName);

                if (preserveAspectRatio)
                {
                    decimal ratio = ((decimal) bmp.Height)/bmp.Width;
                    var newHeight = (int) (maxWidth*ratio);

                    if (newHeight > maxHeight)
                    {
                        maxWidth = (int) (maxHeight/ratio);
                    }
                    else
                    {
                        maxHeight = newHeight;
                    }
                }

                var dest = new Bitmap(maxWidth, maxHeight);
                var g = Graphics.FromImage(dest);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(bmp, 0, 0, maxWidth, maxHeight);
                g.Dispose();
                bmp.Dispose();

                dest.Save(Path.Combine(destDir, image.JpgFileName()), ImageFormat.Jpeg);
                dest.Dispose();
            }
            catch (Exception e)
            {
                var sb = new StringBuilder();
                sb.AppendLine("File resize error: ");
                sb.Append(image.FullName);
                Logger.Error(sb.ToString(), e);
            }
        }
    }
}