using ECC.EntidadeEmail;
using ECC.EntidadeUsuario;

namespace ECC.Servicos.Abstrato
{
   public  interface IEmailService
   {        
       void EnviaEmail(string to, string cc, string body, string subject);

       void EnviaEmail(string to, string msg, string subjectNumeroCel);
       string MontaEmail<T>(T entidade, string templateHtml);

       void EnviarEmailViaRobo(Usuario pUsu, string pAssuntoEmail, string pEmailDestino, string pCorpoEmail, Origem pOrigrem);
   }
}
