

using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class TelefoneConfig : EntidadeBaseConfig<Telefone>
    {

        public TelefoneConfig()
        {

            Property(t => t.PessoaId).IsRequired();





            HasOptional(p => p.UsuarioTel)
         .WithMany(e => e.Telefones)
         .HasForeignKey(p => p.UsuarioTelId);

            Property(t => t.DddTelComl).IsRequired().HasMaxLength(3);
            Property(t => t.TelefoneComl).IsRequired().HasMaxLength(10);
            Property(t => t.DddCel).IsRequired().HasMaxLength(3);
            Property(t => t.Celular).IsRequired().HasMaxLength(10);
            Property(t => t.PessoaId).IsRequired();
        }

    }
}
