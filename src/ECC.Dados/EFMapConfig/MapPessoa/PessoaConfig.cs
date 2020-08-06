using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class PessoaConfig : EntidadeBaseConfig<Pessoa>
    {
        public PessoaConfig()
        {
            Property(p => p.TipoPessoa).IsRequired();
            HasMany(p => p.Enderecos).WithRequired(r => r.Pessoa).HasForeignKey(r => r.PessoaId);

            HasOptional(p => p.PessoaJuridica)
            .WithMany(e => e.Pessoas)
            .HasForeignKey(p => p.PessoaJuridicaId);


            HasOptional(p => p.PessoaFisica)
            .WithMany(e => e.Pessoas)
            .HasForeignKey(p => p.PessoaFisicaId);

            HasMany(t => t.Telefones).WithRequired(t => t.Pessoa).HasForeignKey(t => t.PessoaId);
            HasMany(t => t.Usuarios).WithRequired(t => t.Pessoa).HasForeignKey(t => t.PessoaId);
        }
    }
}
