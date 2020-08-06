

using ECC.Servicos.Util;
using ECC.EntidadeUsuario;


namespace ECC.Servicos.Abstrato
{
    public interface IMembershipService
    {
        MembershipContext ValidateUser(string email, string senha, int perfilModulo);
        Usuario CreateUser(string nome, string email, string senha, int perfilId, int pessoaId, int usuarioCriacaoId, bool flgMaster,
             string DddTelComl = null,
              string TelefoneComl = null,
              string DddCel = null,
               string Celular = null,
               string Contato = null
            );
        Usuario GetUser(int userId);
        Usuario GetUserPorEmail(string UserEmail, int UserPerfilModulo);
        void CreateRecuperaSenha(RecuperaSenha objRecuperaSenha);
        RecuperaSenha getRecuperaSenha(RecuperaSenha objRecuperaSenha);
        void AlterarSenhaUsuario(Usuario objUser);
        bool PagamentosFaturasMensalidadeVencidos(int modulo, string email);

    }
}

