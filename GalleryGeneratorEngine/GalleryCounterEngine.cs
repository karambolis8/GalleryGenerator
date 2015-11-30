using Common;
using System.IO;

namespace GalleryGeneratorEngine
{
    class GalleryCounterEngine : GalleryGeneratorBase
    {
        private int counter;

        public GalleryCounterEngine(GalleryOptions options)
            : base(options)
        {
            counter = 0;
        }

        protected override void ProcessFiles(FileInfo[] files)
        {
            counter += files.Length;
        }
    }
}
