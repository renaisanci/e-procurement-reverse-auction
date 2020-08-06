using ECC.EntidadeEndereco;
using ECC.Entidades;


namespace ECC.EntidadePessoa
{
    public class FornecedorRegiao : EntidadeBase
    {
        public int FornecedorId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public int CidadeId { get; set; }
        public virtual Cidade Cidade { get; set; }
        public decimal? VlPedMinRegiao { get; set; }
        public int Prazo { get; set; }
        public decimal? TaxaEntrega { get; set; }
        public bool? Cif { get; set; }

    }
}
