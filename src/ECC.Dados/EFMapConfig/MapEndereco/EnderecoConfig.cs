
using ECC.EntidadeEndereco;

namespace ECC.Dados.EFMapConfig.MapEndereco
{
    public class EnderecoConfig : EntidadeBaseConfig<Endereco>
    {


        public EnderecoConfig()
        {
            Property(u => u.PessoaId).IsRequired();
            Property(u => u.EstadoId).IsRequired();
            Property(u => u.CidadeId).IsRequired();
            Property(u => u.BairroId).IsRequired();
            Property(u => u.LogradouroId).IsRequired();
            Property(u => u.Complemento).IsRequired().HasMaxLength(100);
            Property(u => u.DescEndereco).IsRequired().HasMaxLength(100);
            Property(u => u.Cep).IsRequired().HasMaxLength(8);
            Property(u => u.Localizacao).IsOptional();

            HasMany(e => e.HorasEntregaMembro).WithRequired(r => r.Endereco).HasForeignKey(r => r.EnderecoId);

        }
    }
}
