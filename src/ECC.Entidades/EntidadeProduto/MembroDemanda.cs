using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.Entidades.EntidadeComum;

namespace ECC.Entidades.EntidadeProduto
{
    /// <summary>
    /// MembroDemanda: Demanda média que o membro costuma comprar.
    /// </summary>
    public class MembroDemanda : EntidadeBase
    {
        public int MembroId { get; set; }
        public int SubCategoriaId { get; set; }
        public int PeriodicidadeId { get; set; }
        public int UnidadeMedidaId { get; set; }
        
        public int Quantidade { get; set; }
        public string Observacao { get; set; }

        public virtual Membro Membro { get; set; }
        public virtual SubCategoria SubCategoria { get; set; }
        public virtual Periodicidade Periodicidade { get; set; }
        public virtual UnidadeMedida UnidadeMedida { get; set; }


    }
}
