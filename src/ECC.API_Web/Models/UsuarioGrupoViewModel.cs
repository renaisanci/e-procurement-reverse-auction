namespace ECC.API_Web.Models
{
    public class UsuarioGrupoViewModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int GrupoId { get; set; }
        public string DescGrupo { get; set; }
        public bool Selecionado { get; set; }
        public bool Relacionado { get; set; }

        
    }
}