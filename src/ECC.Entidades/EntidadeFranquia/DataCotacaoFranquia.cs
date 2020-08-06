using ECC.EntidadePessoa;
using ECC.Entidades;
 

namespace ECC.EntidadeFranquia
{
    public class DataCotacaoFranquia : EntidadeBase
    {

        public int DiaSemana { get; set; }
        public int FranquiaId { get; set; }
        public virtual Franquia Franquia { get; set; }


    }
}
