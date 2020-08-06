namespace ECC.API_Web.Models
{
    public class PermissaoGrupoViewModel
    {

        public int Id { get; set; }
        public int MenuId { get; set; }
        
        public int PerfilId { get; set; }
        
        public int GrupoId { get; set; }
        
        public bool FlgVisualizarMenu { get; set; }
        public bool FlgAlterar { get; set; }
        public bool FlgInserir { get; set; }
        public bool FlgExcluir { get; set; }
        public bool FlgConsultar { get; set; }

    }
}