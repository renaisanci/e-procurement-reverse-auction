using ECC.Entidades;

namespace ECC.EntidadePessoa
{
    public class FranquiaFornecedor : EntidadeBase
    {
        public int FranquiaId { get; set; }

        public int FornecedorId { get; set; }

        public virtual Franquia Franquia { get; set; }

        public virtual  Fornecedor Fornecedor { get; set; }
    }
}
