
using ECC.Entidades;

namespace ECC.Dados.EFMapConfig
{
    public class ErroConfig : EntidadeBaseConfig<Erro>
    {
        public ErroConfig()
        {
            Property(p => p.StackTrace).IsMaxLength();
            Property(p => p.Mensagem).IsMaxLength();
        }
    }
}
