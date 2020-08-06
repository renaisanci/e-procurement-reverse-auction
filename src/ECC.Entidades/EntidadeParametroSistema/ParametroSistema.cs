using ECC.Entidades;


namespace ECC.EntidadeParametroSistema
{
    public class ParametroSistema : EntidadeBase
    {
        public ParametroSistema() { }

        public string Descricao { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }

    }
}
