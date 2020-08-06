using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using ECC.EntidadeAvisos;

namespace ECC.Dados.EFMapConfig.MapAvisos
{
    public class UsuarioNotificacaoConfig : EntidadeBaseConfig<UsuarioNotificacao>
    {
        public UsuarioNotificacaoConfig()
        {
            HasRequired(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId);
            HasRequired(x => x.Notificacao).WithMany().HasForeignKey(x => x.NotificacaoId);

            var ixUsuarioNotificacao = "IX_UsuarioNotificacao";
            Property(x => x.UsuarioId).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute(ixUsuarioNotificacao, 1) { IsUnique = true }));
            Property(x => x.NotificacaoId).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute(ixUsuarioNotificacao, 2) { IsUnique = true }));
        }
    }
}
