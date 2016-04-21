using System;
using Common.DataObjects;
using log4net;

namespace GalleryGeneratorEngine.DirectoryProcessors
{
    public abstract class DirectoryTreeProcessorBase<T>
    {
        protected UserOptions options;

        protected Func<bool> cancellationPending;

        protected Action cancelWork;

        protected bool cancelHandled;

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

        public T StartTask()
        {
            this.cancelHandled = false;

            return this.DoJob();
        }

        protected void HandleCancelWork()
        {
            if (!cancelHandled)
            {
                this.Logger.Info("Work cancelled.");
                this.cancelWork();
            }

            this.cancelHandled = true;
        }

        protected abstract T DoJob();
    }
}