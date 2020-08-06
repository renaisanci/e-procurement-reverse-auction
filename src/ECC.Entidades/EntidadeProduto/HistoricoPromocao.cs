using System;
using ECC.EntidadePessoa;



namespace ECC.Entidades.EntidadeProduto
{
    public class HistoricoPromocao : EntidadeBase
    {
        public virtual Fornecedor Fornecedor { get; set; }
        public int FornecedorId { get; set; }
        public virtual Produto Produto{ get; set; }
        public int ProdutoId { get; set; }
        public int QuantidadeProduto { get; set; }

        public decimal Preco { get; set; }
        public string MotivoPromocao { get; set; }
        public DateTime InicioPromocao { get; set; }
        public DateTime FimPromocao { get; set; }
    }
}
