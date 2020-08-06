using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeEndereco;

namespace ECC.Dados.EFMapConfig.MapEndereco
{
    public class HorasEntregaMembroConfig : EntityTypeConfiguration<HorasEntregaMembro>
    {
        public HorasEntregaMembroConfig()
        {
            Property(b => b.PeriodoId).IsRequired();
            Property(b => b.EnderecoId).IsRequired();
            Property(b => b.DescHorarioEntrega).IsOptional().HasMaxLength(200);
         
        }
    }
}
