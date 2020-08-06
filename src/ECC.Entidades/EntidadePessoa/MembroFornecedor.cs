using ECC.Entidades;

namespace ECC.EntidadePessoa
{
    public class MembroFornecedor : EntidadeBase
    {
        public int FornecedorId { get; set; }
        public int MembroId { get; set; }
        public virtual Membro Membro { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
    }
}
