

namespace ECC.Servicos.Abstrato
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Cria uma chave randomica
        /// </summary>
        /// <returns></returns>
        string CriaChave();
        /// <summary>
        /// Gera o Hashed da senha
        /// </summary>
        /// <param name="senha"></param>
        /// <param name="chave"></param>
        /// <returns></returns>
        string EncryptSenha(string senha, string chave);

        /// <summary>
        /// Gera o Hashed da senha de segurança do cartão
        /// </summary>
        /// <param name="cvv">Senha de segurança</param>
        /// <returns></returns>
        string EncryptCvv(string cvv);


        /// <summary>
        /// Decodifica senha de seguraça do cartão
        /// </summary>
        /// <param name="cvv">Senha de segurança</param>
        /// <returns></returns>
        string DecryptCvv(string cvv);


    }
}
