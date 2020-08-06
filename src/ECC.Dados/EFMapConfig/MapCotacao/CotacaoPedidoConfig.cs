 

using ECC.EntidadeCotacao;

namespace ECC.Dados.EFMapConfig.MapCotacao
{
    public class CotacaoPedidoConfig : EntidadeBaseConfig<CotacaoPedidos>
    {



        public CotacaoPedidoConfig()
       {
 
           Property(b => b.CotacaoId).IsRequired();
           Property(b => b.PedidoId).IsRequired();
         
       }

    }
}
