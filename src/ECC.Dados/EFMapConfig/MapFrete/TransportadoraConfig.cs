using ECC.EntidadeFrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Dados.EFMapConfig.MapFrete
{
    public class TransportadoraConfig:  EntidadeBaseConfig<Transportadora>
    {
        public TransportadoraConfig()
        {
            Property(t => t.cnpj).IsRequired();
            Property(t => t.RazaoSocial).IsRequired();
        }
    }
}
