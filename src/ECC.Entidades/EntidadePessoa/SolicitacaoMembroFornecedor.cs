using ECC.Entidades;

namespace ECC.EntidadePessoa
{
    public class SolicitacaoMembroFornecedor : EntidadeBase
    {
        public SolicitacaoMembroFornecedor()
        {
        }
        public int MembroId { get; set; }
        public virtual Membro Membro { get; set; }
        public int FornecedorId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public string MotivoRecusa { get; set; }
    }
}
