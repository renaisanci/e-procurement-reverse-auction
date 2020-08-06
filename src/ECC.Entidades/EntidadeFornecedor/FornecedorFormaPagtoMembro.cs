using ECC.EntidadeFormaPagto;
using ECC.EntidadePessoa;
using ECC.Entidades;

namespace ECC.EntidadeFornecedor
{
    public class FornecedorFormaPagtoMembro : EntidadeBase
    {
        public int FornecedorId { get; set; }
        public int MembroId { get; set; }
        public virtual Membro Membro { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public  int FornecedorFormaPagtoId { get; set; }
        public virtual FornecedorFormaPagto FornecedorFormaPagto { get; set; }

    }
}
