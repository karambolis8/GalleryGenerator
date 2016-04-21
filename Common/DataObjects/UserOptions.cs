
namespace Common.DataObjects
{
    public class UserOptions
    {
        /// <summary>
        /// Defines if original files should be copied to OriginalImgDir
        /// </summary>
        public bool CopyOriginalFiles { get; set; }

        /// <summary>
        /// Output base directory for original files. If CopyOriginalFiles == false should be ignored
        /// </summary>
        public string OriginalImgDir { get; set; }

        /// <summary>
        /// Defines width of small thumbnails
        /// </summary>
        public int ThumbX { get; set; }

        /// <summary>
        /// Defines height of small thumbnails
        /// </summary>
        public int ThumbY { get; set; }

        /// <summary>
        /// Defines width of medium thumbnails
        /// </summary>
        public int MediumX { get; set; }

        /// <summary>
        /// Defines height of medium thumbnails
        /// </summary>
        public int MediumY { get; set; }

        /// <summary>
        /// If true when generating medium thumbnail only MediumX will be respected
        /// </summary>
        public bool PreserveMediumAspectRatio { get; set; }

        /// <summary>
        /// Input directory of images to generate gallery
        /// </summary>
        public string InputDirectory { get; set; }

        /// <summary>
        /// Output base directory of gallery html structure
        /// </summary>
        public string OutputDirectory { get; set; }

        public string[] ImageExtensions { get; set; }

        public string[] FileExtensions { get; set; }

        /// <summary>
        /// Gallery name
        /// </summary>
        public string GalleryName { get; set; }

        public long WorkSize { get; set; }
    }
}