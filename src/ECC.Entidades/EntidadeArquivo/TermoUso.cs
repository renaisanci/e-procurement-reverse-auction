using ECC.Entidades;

namespace ECC.EntidadeArquivo
{
    public class TermoUso : EntidadeBase
    {
        public TipoTermoUso Tipo { get; set; }
        public string Documento { get; set; }
    }
}
