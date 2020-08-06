using ECC.EntidadeEstoque;

namespace ECC.Dados.EFMapConfig.MapEstoque
{
    public class EstoqueConfig : EntidadeBaseConfig<Estoque>
    {
        public EstoqueConfig()
        {
            Property(p => p.ProdutoId).IsRequired();
            Property(p => p.MembroId).IsRequired();
            Property(p => p.EnderecoId).IsRequired();
        }
    }
}
