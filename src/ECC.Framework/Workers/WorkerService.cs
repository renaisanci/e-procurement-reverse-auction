using System.ComponentModel;
using System.ServiceProcess;

namespace ECC.Framework.Workers
{
    public abstract class WorkerService : ServiceBase
    {
        private BackgroundWorker _worker;

        protected WorkerService()
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

        public virtual new void Stop()
        {
            this._worker?.CancelAsync();
            this._worker?.Dispose();
            base.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Stop();
            }

            base.Dispose(disposing);
        }
    }
}

