using System.ComponentModel;

namespace ECC.SrvWin.Cotacao
{
    [RunInstaller(true)]
    public partial class Instalador : System.Configuration.Install.Installer
    {
        public Instalador()
        {
            InitializeComponent();
        }


        public override void Install(System.Collections.IDictionary stateSaver)
        {
            RetrieveServiceName();
            base.Install(stateSaver);
        }

        public override void Uninstall(System.Collections.IDictionary stateSaver)
        {
            RetrieveServiceName();
            base.Uninstall(stateSaver);
        }

        private void RetrieveServiceName()
        {
            var serviceName = Context.Parameters["nomeservico"];
            if (!string.IsNullOrEmpty(serviceName))
            {
                this.serviceInstaller1.ServiceName = serviceName.Substring(12);
                this.serviceInstaller1.DisplayName = serviceName.Substring(12);
            }
        }
    }
}
