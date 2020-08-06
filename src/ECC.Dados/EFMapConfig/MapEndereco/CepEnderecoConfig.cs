
using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeEndereco;

namespace ECC.Dados.EFMapConfig.MapEndereco
{
    public class CepEnderecoConfig : EntityTypeConfiguration<CepEndereco>
    {



        public CepEnderecoConfig()
        {
           
            Property(b => b.DescLogradouro).IsRequired().HasMaxLength(100);
            Property(b => b.Complemento).IsRequired().HasMaxLength(100);
            Property(b => b.CidadeId).IsRequired();
            Property(b => b.EstadoId).IsRequired();
            Property(b => b.BairroId).IsRequired();
            Property(b => b.LogradouroId).IsRequired();


        }


    }
}
