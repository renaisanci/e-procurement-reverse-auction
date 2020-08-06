using ECC.EntidadePessoa;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeListaCompras
{
 

    public class ListaComprasRemoveForn : EntidadeBase
    {

        public int ListaComprasId { get; set; }
        public virtual ListaCompras ListaCompras { get; set; }
        public int FonecedorId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

    }
}
