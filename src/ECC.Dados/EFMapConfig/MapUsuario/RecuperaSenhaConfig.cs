using ECC.EntidadeUsuario;

namespace ECC.Dados.EFMapConfig.MapUsuario
{
    public class RecuperaSenhaConfig : EntidadeBaseConfig<RecuperaSenha>
    {


        public RecuperaSenhaConfig()
        {

            Property(e => e.Chave).IsRequired();
            Property(e => e.UsuarioId).IsRequired();
            Property(e => e.DtExpira).IsRequired();

        }


    }
}
