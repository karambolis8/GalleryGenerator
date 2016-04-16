using System;
using Common;
using log4net;

namespace GalleryGeneratorEngine
{
    public abstract class DirectoryTreeProcessorBase
    {
        protected UserOptions options;

        protected Func<bool> cancellationPending;

        protected Action cancelWork;

        protected abstract ILog Logger
        {
            get;
        }

        protected DirectoryTreeProcessorBase(UserOptions options, Func<bool> cancellationPending, Action cancelWork)
        {
            this.options = options;
            this.cancellationPending = cancellationPending;
            this.cancelWork = cancelWork;
        }

        protected void HandleCancelWork()
        {
            this.cancelWork();
            this.Logger.Info("Work cancelled.");
        }
    }
}