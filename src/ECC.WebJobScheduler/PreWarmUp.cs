﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ECC.WebJobScheduler
{
    public class PreWarmUp : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            JobScheduler.Start();
        }
    }
}