
using ECC.EntidadeCotacao;

namespace ECC.Dados.EFMapConfig.MapCotacao
{
    public class ResultadoCotacaoConfig : EntidadeBaseConfig<ResultadoCotacao>
    {


        public ResultadoCotacaoConfig()
        {
            Property(b => b.CotacaoId).IsRequired();
            Property(b => b.FornecedorId).IsRequired();
            Property(b => b.ProdutoId).IsRequired();
            Property(b => b.FlgOutraMarca).HasColumnAnnotation("DefaultValue", false);
        }
    }
}
