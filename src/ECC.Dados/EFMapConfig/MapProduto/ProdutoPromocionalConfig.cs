using ECC.Entidades.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class ProdutoPromocionalConfig : EntidadeBaseConfig<ProdutoPromocional>
    {


        public ProdutoPromocionalConfig()
        {
            Property(p => p.FornecedorId).IsRequired();
            Property(p => p.QuantidadeProduto).IsRequired();
            Property(p => p.QtdMinVenda).IsOptional();
            Property(p => p.MotivoPromocao).IsRequired().HasMaxLength(200);
            Property(p => p.DescMotivoCancelamento).IsOptional().HasMaxLength(200);
            Property(p => p.ValidadeProduto).IsRequired();
            Property(p => p.InicioPromocao).IsRequired();
            Property(p => p.FimPromocao).IsRequired();
            Property(p => p.Aprovado).IsOptional();
            Property(p => p.PrecoPromocao).IsOptional();
            Property(p => p.ObsFrete).IsRequired().HasMaxLength(200);
        }


    }
}
