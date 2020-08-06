
using ECC.EntidadeListaCompras;
using ECC.EntidadePedido;

namespace ECC.Dados.EFMapConfig.MapPedido
{
    public class ListaComprasConfig : EntidadeBaseConfig<ListaCompras>
    {
        public ListaComprasConfig()
        {            
            Property(b => b.NomeLista).IsRequired();
            HasMany(e => e.ListaComprasItens).WithRequired(r => r.ListaCompras).HasForeignKey(r => r.ListaComprasId);
            HasMany(e => e.ListaComprasRemoveFornecedores).WithRequired(r => r.ListaCompras).HasForeignKey(r => r.ListaComprasId);

        }
    }
}
