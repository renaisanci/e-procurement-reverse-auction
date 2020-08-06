using ECC.EntidadeUsuario;

namespace ECC.Dados.EFMapConfig.MapUsuario
{
    public class UsuarioCanceladoConfig : EntidadeBaseConfig<UsuarioCancelado>
    {
        public UsuarioCanceladoConfig()
        {
            Property(u => u.Id).IsRequired();
            Property(u => u.UsuarioId).IsRequired();
            Property(u => u.DataCancelamento).IsRequired();
            Property(u => u.Ativo).IsRequired();
        }
    }
}
