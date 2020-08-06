using ECC.EntidadePedido;

namespace ECC.Dados.EFMapConfig.MapPedido
{
    public class AvaliacaoFornecedorConfig : EntidadeBaseConfig<AvaliacaoFornecedor>
    {

        public AvaliacaoFornecedorConfig()
        {
            Property(s => s.PedidoId).IsRequired();
            Property(s => s.FornecedorId).IsRequired();
            Property(s => s.NotaQualidadeProdutos).IsRequired();
            Property(s => s.NotaTempoEntrega).IsRequired();
            Property(s => s.NotaAtendimento).IsRequired();
            Property(s => s.ObsQualidade).IsOptional();
            Property(s => s.ObsEntrega).IsOptional();
            Property(s => s.ObsAtendimento).IsOptional();

        }



    }
}
