using ECC.EntidadePessoa;
using ECC.Entidades.EntidadeProduto;
using ECC.Entidades;

namespace ECC.EntidadeProduto
{
    public class FranquiaProduto:  EntidadeBase
    {
        public int FranquiaId { get; set; }

        public int ProdutoId { get; set; }

        public virtual Franquia Franquia { get; set; }

        public virtual Produto Produto { get; set; }
    }
}
