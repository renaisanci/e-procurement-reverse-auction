using ECC.Entidades.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class HistoricoPromocaoConfig : EntidadeBaseConfig<HistoricoPromocao>
    {


        public HistoricoPromocaoConfig()
        {
            Property(p => p.FornecedorId).IsRequired();
            Property(p => p.ProdutoId).IsRequired();
            Property(p => p.QuantidadeProduto).IsRequired();
            Property(p => p.Preco).IsRequired();
            Property(p => p.MotivoPromocao).IsRequired().HasMaxLength(200);
            Property(p => p.InicioPromocao).IsRequired();
            Property(p => p.FimPromocao).IsRequired();
          
        }


    }
}
