using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class PessoaFisicaConfig : EntidadeBaseConfig<PessoaFisica>
    {

        public PessoaFisicaConfig()
        {
            Property(p => p.PessoaId).IsRequired();
            Property(p => p.Nome).IsRequired().HasMaxLength(100);
            Property(p => p.Cpf).IsRequired().HasMaxLength(14);
            Property(p => p.Email).IsOptional();
            Property(p => p.Cro).IsRequired().HasMaxLength(10);


            HasMany(p => p.Pessoas)
                .WithOptional(p => p.PessoaFisica)
                .HasForeignKey(p => p.PessoaFisicaId);
 

        }
    }
}
