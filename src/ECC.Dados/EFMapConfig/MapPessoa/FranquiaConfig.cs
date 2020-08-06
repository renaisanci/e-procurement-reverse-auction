using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class FranquiaConfig : EntidadeBaseConfig<Franquia>
    {
        public FranquiaConfig()
        {
            Property(x => x.PessoaId).IsRequired();
            Property(x => x.Responsavel).IsRequired().HasMaxLength(100); 
            Property(x => x.Descricao).IsRequired();
            Property(x => x.DataCotacao).IsOptional();

            HasMany(p => p.Membros).WithOptional(r => r.Franquia).HasForeignKey(r => r.FranquiaId);
            HasMany(p => p.Fornecedores).WithRequired(r => r.Franquia).HasForeignKey(r => r.FranquiaId);
            HasMany(p => p.Produtos).WithRequired(r => r.Franquia).HasForeignKey(r => r.FranquiaId);
        }
    }
}
