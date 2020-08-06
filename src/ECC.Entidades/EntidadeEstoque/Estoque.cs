using ECC.EntidadeEndereco;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeEstoque
{
    public class Estoque : EntidadeBase
    {
        public Estoque() { 
        
        }

        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }
        public int MembroId { get; set; }
        public virtual Membro Membro { get; set; }
        public int EnderecoId { get; set; }
        public virtual Endereco Endereco { get; set; }
        public int MinimoEstoque { get; set; }
        public int MaximoEstoque { get; set; }
        public int QtdEstoque { get; set; }
        public int QtdEstoqueReceber { get; set; }
    }
}
