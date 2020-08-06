using ECC.Entidades;

namespace ECC.EntidadeRecebimento
{
    public class CartaoBandeira : EntidadeBase
    {
        public int Id { get; set; }
        public string Descricao { get; set; }        
        public bool Ativo { get; set; }
    }
}
