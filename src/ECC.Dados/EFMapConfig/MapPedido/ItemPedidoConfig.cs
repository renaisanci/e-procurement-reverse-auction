using ECC.EntidadePedido;

namespace ECC.Dados.EFMapConfig.MapPedido
{
    public class ItemPedidoConfig : EntidadeBaseConfig<ItemPedido>
    {

        public ItemPedidoConfig()
        {
            Property(b => b.PedidoId).IsRequired();
            Property(b => b.ProdutoId).IsRequired();
            Property(b => b.EntregaId).IsRequired();
            Property(b => b.PrecoMedioUnit).IsOptional().HasColumnType("money");
            Property(b => b.AprovacaoMembro).IsOptional();
            Property(b => b.AprovacaoFornecedor).IsOptional();
            Property(b => b.Desconto).IsOptional().HasPrecision(18,1);
            Property(b => b.TaxaEntrega).IsOptional().HasPrecision(18,2);
            Property(b => b.PrecoNegociadoUnit).IsOptional().HasColumnType("money");
            Property(b => b.FormaPagtoId).IsOptional();
            Property(b => b.DataEntregaMembro).IsOptional();
            Property(b => b.DataEntregaFornecedor).IsOptional();
            HasOptional(b => b.Fornecedor).WithMany().HasForeignKey(b => b.FornecedorId);
            Property(b => b.FlgOutraMarca).HasColumnAnnotation("DefaultValue", false);
        }

    }
}
