using ECC.EntidadeFormaPagto;

namespace ECC.Dados.EFMapConfig.MapFormaPagto
{
    public class FormaPagtoConfig : EntidadeBaseConfig<FormaPagto>
    {
        public FormaPagtoConfig()
        {
            Property(f => f.DescFormaPagto).IsRequired().HasMaxLength(100);
            Property(f => f.Avista).IsRequired();
            Property(f => f.QtdParcelas).IsOptional();

        }


    }
}
