using ECC.EntidadeUsuario;

namespace ECC.Dados.EFMapConfig.MapUsuario
{
    public class UsuarioConfig : EntidadeBaseConfig<Usuario>
    {
        public UsuarioConfig()
        {
            Property(u => u.PessoaId).IsRequired();
            Property(u => u.PerfilId).IsRequired();
            Property(u => u.UsuarioNome).IsRequired().HasMaxLength(100);
            Property(u => u.UsuarioEmail).IsRequired().HasMaxLength(100);
            Property(u => u.Senha).IsRequired().HasMaxLength(100);
            Property(u => u.Chave).IsRequired().HasMaxLength(100);

            HasMany(x => x.Notificacoes).WithRequired(x => x.Usuario).HasForeignKey(x => x.UsuarioId);
        }
    }
}
