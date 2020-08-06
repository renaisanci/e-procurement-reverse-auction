using System;
using System.ComponentModel;

namespace ECC.Framework.Workers
{
    public abstract class WorkerBase : IDisposable
    {
        private readonly BackgroundWorker _worker;

        protected WorkerBase()
        {
            this._worker = new BackgroundWorker();
            this._worker.RunWorkerCompleted += this.CompletedProcess;
            this._worker.DoWork += this.ExecuteProcess;
        }

        public abstract void ExecuteProcess(object sender, DoWorkEventArgs e);

        public virtual void CompletedProcess(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Start();
        }

        public virtual void Start()
        {
            this._worker?.RunWorkerAsync();
        }

        public virtual void Stop()
        {
            this._worker?.CancelAsync();
            this._worker?.Dispose();
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
