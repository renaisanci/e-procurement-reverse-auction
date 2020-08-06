using System;

namespace ECC.API_Web.Models
{
    public class RecuperaSenhaViewModel
    {

        public int Id { get; set; }
        public Guid Chave { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DtExpira { get; set; }
        public String Usuarioemail { get; set; }
        public String UsuarioNome { get; set; }
        public String URL { get; set; }
    }
}