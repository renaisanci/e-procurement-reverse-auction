
using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class MembroConfig : EntidadeBaseConfig<Membro>
    {


        public MembroConfig()
        {
            Property(p => p.PessoaId).IsRequired();
            Property(p => p.Comprador).IsOptional().HasMaxLength(150);
            Property(p => p.Vip).IsRequired();
            Property(p => p.FranquiaId).IsOptional();
            Property(p => p.PlanoMensalidadeId).IsOptional();
            Property(p => p.DataFimPeriodoGratuito).IsOptional();
            HasMany(p => p.MembroCategorias).WithRequired(r => r.Membro).HasForeignKey(r => r.MembroId);
            HasMany(p => p.MembroFornecedores).WithRequired(r => r.Membro).HasForeignKey(r => r.MembroId);
            HasMany(p => p.Mensalidades).WithRequired(r => r.Membro).HasForeignKey(r => r.MembroId);
        }
    }
}
