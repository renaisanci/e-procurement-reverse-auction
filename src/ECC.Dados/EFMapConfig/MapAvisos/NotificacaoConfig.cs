using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using ECC.EntidadeAvisos;

namespace ECC.Dados.EFMapConfig.MapAvisos
{
    public class NotificacaoConfig : EntidadeBaseConfig<Notificacao>
    {
        public NotificacaoConfig()
        {
            HasRequired(x => x.TipoAvisos).WithMany().HasForeignKey(x => x.TipoAvisosId);
            HasMany(x => x.UsuariosNotificacoes).WithRequired(x => x.Notificacao).HasForeignKey(x => x.NotificacaoId);

            var ixAvisoTipoAlerta = "IX_AvisoTipoAlerta";
            Property(x => x.TipoAvisosId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute(ixAvisoTipoAlerta, 1) { IsUnique = true }));
            Property(x => x.TipoAlerta).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute(ixAvisoTipoAlerta, 2) { IsUnique = true }));
        }
    }
}
