using ECC.EntidadeFrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Dados.EFMapConfig.MapFrete
{
    public class FretePedidoFornecedorConfig : EntidadeBaseConfig<FretePedidoFornecedor>
    {
        public FretePedidoFornecedorConfig() {
            Property(t => t.TransportadoraId).IsRequired();
            Property(t => t.FornecedorId).IsRequired();
            Property(t => t.PedidoId).IsRequired();
            Property(t => t.ValorTotal).IsRequired();
        }
    }
}
