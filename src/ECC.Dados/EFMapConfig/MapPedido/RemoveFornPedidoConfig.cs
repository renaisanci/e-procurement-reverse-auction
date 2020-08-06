using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECC.EntidadePedido;

namespace ECC.Dados.EFMapConfig.MapPedido
{
    public class RemoveFornPedidoConfig : EntidadeBaseConfig<RemoveFornPedido>
    {

        public RemoveFornPedidoConfig()
        {
            Property(b => b.PedidoId).IsRequired();
            Property(b => b.FonecedorId).IsRequired();
            Property(b => b.ProdutoId).IsRequired();
        }
    }
}
