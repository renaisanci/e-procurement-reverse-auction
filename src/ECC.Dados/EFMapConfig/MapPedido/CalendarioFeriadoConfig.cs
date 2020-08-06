using ECC.EntidadePedido;

namespace ECC.Dados.EFMapConfig.MapPedido
{
   public  class CalendarioFeriadoConfig : EntidadeBaseConfig<CalendarioFeriado>
    {

        public CalendarioFeriadoConfig()
        {

            Property(b => b.Estado).IsRequired();
            Property(b => b.Cidade).IsRequired();
            Property(b => b.DtEvento).IsRequired();
            Property(b => b.TipoFeriado).IsRequired();


        }

    }
}
