using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

namespace GalleryGeneratorEngine
{
    static class ImageResizer
    {
        public static void ResizeImage(this FileInfo image, int maxWidth, int maxHeight, string destDir,
            bool preserveAspectRatio = false)
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
    }
}