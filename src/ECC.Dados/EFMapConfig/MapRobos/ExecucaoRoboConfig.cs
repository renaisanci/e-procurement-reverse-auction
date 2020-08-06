using ECC.EntidadeRobos;

namespace ECC.Dados.EFMapConfig.MapRobos
{
    public class ExecucaoRoboConfig : EntidadeBaseConfig<ExecucaoRobo>
    {
        public ExecucaoRoboConfig()
        {           
            Property(t => t.NomeRotina).IsRequired();
            Property(t => t.TipoAviso).IsRequired();
            
        }
    }
}
