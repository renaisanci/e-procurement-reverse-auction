using ECC.EntidadeSms;

namespace ECC.Servicos.Abstrato
{
   public  interface ISmsService
   {
       void EnviaSms(string number, string message, TipoOrigemSms? origemSms = null, int? idEntidadeOrigemSms = null);
       string MontaSms<T>(T entidade, string template);
   }
}
