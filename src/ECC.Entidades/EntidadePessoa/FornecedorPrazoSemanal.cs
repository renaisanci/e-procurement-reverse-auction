using ECC.EntidadeEndereco;
using ECC.EntidadePessoa;

namespace ECC.Entidades.EntidadePessoa
{
    public class FornecedorPrazoSemanal : EntidadeBase
    {
        public int FornecedorId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public int CidadeId { get; set; }
        public virtual Cidade Cidade { get; set; }
        public int DiaSemana { get; set; }
        public decimal? VlPedMinRegiao { get; set; }
        public decimal? TaxaEntrega { get; set; }
        public bool? Cif { get; set; }
    }
}
