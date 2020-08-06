using System.Security;
using System.Configuration.Install;

namespace ECC.Servicos
{
    [SecuritySafeCritical]
    public class InstallWindowsService
    {
        [SecuritySafeCritical]
        public bool InstallMe(string exePath)
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        [SecuritySafeCritical]
        public bool InstallMe(string exePath, string nomeServico = "")
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { string.Format("/nomeservico={0}", nomeServico), exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        [SecuritySafeCritical]
        public bool UninstallMe(string exePath)
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { "/u", exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        [SecuritySafeCritical]
        public bool UninstallMe(string exePath, string nomeServico = "")
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { "/u", string.Format("/nomeservico={0}", nomeServico), exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
