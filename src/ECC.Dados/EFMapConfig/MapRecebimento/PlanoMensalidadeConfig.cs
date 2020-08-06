using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class PlanoMensalidadeConfig : EntidadeBaseConfig<PlanoMensalidade>
    {
        public PlanoMensalidadeConfig()
        {
            Property(b => b.Descricao).IsRequired();
            Property(b => b.Valor).IsRequired();
            Property(b => b.QtdMeses).IsRequired();
            Property(b => b.Ativo).IsRequired();
        }
    }
}
