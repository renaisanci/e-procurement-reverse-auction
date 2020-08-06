using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECC.Servicos.ModelService;

namespace ECC.Servicos.Abstrato
{
    public interface IGeraCotacao
    {
        GeraCotacaoViewModel GeraCotacaoPedidos();
    }
}
