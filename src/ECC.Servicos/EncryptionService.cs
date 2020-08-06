using ECC.Servicos.Abstrato;
using System;
using System.Security.Cryptography;
using System.Text;


namespace ECC.Servicos
{
    public class EncryptionService : IEncryptionService
    {
        public string CriaChave()
        {
            var data = new byte[0x10];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetBytes(data);
                return Convert.ToBase64String(data);
            }
        }

        public string EncryptSenha(string senha, string chave)
        {
            using (var sha256 = SHA256.Create())
            {
                var chaveSenha = string.Format("{0}{1}", chave, senha);
                byte[] chaveSenhaAsBytes = Encoding.UTF8.GetBytes(chaveSenha);
                return Convert.ToBase64String(sha256.ComputeHash(chaveSenhaAsBytes));
            }
        }

        public string EncryptCvv(string cvv)
        {
            byte[] chaveSenhaAsBytes = Encoding.UTF8.GetBytes(cvv);
            return Convert.ToBase64String(chaveSenhaAsBytes);
        }

        public string DecryptCvv(string cvv)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(cvv);
            var retono = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            return retono;
        }
    }
}
