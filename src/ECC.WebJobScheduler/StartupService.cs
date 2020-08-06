using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECC.WebJobScheduler
{
    public class StartupService : System.Web.Hosting.IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            new JobScheduler().Init();

            if (!JobScheduler.CurrentScheduler.IsStarted)
                JobScheduler.CurrentScheduler.Start();
        }
    }
}