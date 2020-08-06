using System.Data.Entity.ModelConfiguration;
using ECC.Entidades;

namespace ECC.Dados.EFMapConfig
{
    public class EntidadeBaseConfig<T> : EntityTypeConfiguration<T> where T : class, IEntidadeBase
    {
        public EntidadeBaseConfig()
        {
            HasKey(e => e.Id);
            
            HasRequired(e => e.UsuarioCriacao)
                .WithMany()
                .HasForeignKey(e => e.UsuarioCriacaoId)
                .WillCascadeOnDelete(false);

            HasOptional(e => e.UsuarioAlteracao)
                .WithMany()
                .HasForeignKey(e => e.UsuarioAlteracaoId);

            //HasOptional(e => e.UsuarioCricao)
            //    .WithMany()
            //    .HasForeignKey(e => e.UsuarioCricaoId);

            Property(e => e.DtCriacao).IsRequired();
            Property(e => e.DtAlteracao).IsOptional();
            Property(e => e.Ativo).IsRequired();
        }
    }
}
