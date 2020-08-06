using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class PessoaJuridicaConfig : EntidadeBaseConfig<PessoaJuridica>
    {

        public PessoaJuridicaConfig()
        {
            Property(p => p.PessoaId).IsRequired();
            Property(p => p.NomeFantasia).IsRequired().HasMaxLength(100);
            Property(p => p.RazaoSocial).IsRequired().HasMaxLength(100);
            Property(p => p.Cnpj).IsRequired().HasMaxLength(15);
            Property(p => p.InscEstadual).IsOptional().HasMaxLength(15); ;
            Property(p => p.DtFundacao).IsOptional();
            Property(p => p.Email).IsOptional();

            HasMany(p => p.Pessoas)
                .WithOptional(p => p.PessoaJuridica)
                .HasForeignKey(p => p.PessoaJuridicaId);

  
        }
    }
}
