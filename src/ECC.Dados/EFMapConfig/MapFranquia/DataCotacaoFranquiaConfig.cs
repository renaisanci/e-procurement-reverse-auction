using ECC.EntidadeFranquia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Dados.EFMapConfig.MapFranquia
{
    public class DataCotacaoFranquiaConfig : EntidadeBaseConfig<DataCotacaoFranquia>
    {
        public DataCotacaoFranquiaConfig()
        {
            Property(x => x.FranquiaId).IsRequired();
            Property(x => x.DiaSemana).IsRequired();
        }
    }
}
