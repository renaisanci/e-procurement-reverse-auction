using System;
using ECC.EntidadePessoa;


namespace ECC.Entidades.EntidadeProduto
{
    public class IndisponibilidadeProduto : EntidadeBase
    {
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }
        public int FornecedorId { get; set; }
        public virtual Fornecedor Fornecedor{ get; set; }
        public Boolean IndisponivelPermanente { get; set; }
        public DateTime? InicioIndisponibilidade { get; set; }
        public DateTime? FimIndisponibilidade { get; set; }
    }
}
