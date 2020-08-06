using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ECC.Servicos;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace JobGeraCotacao
{
    public class GeraCotacao : IJob
    {


        public virtual async Task Execute(IJobExecutionContext context)
        {

            var service = new CriarCotacaoService();
            service.GeraCotacaoPedidos();
        }
    }
}