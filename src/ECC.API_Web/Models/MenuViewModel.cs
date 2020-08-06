namespace ECC.API_Web.Models
{
    public class MenuViewModel
    {
        public int Id { get; set; }

        public int MenuPaiId { get; set; }
      
        public int ModuloId { get; set; }

        public string DescModulo { get; set; }

        public string DescMenu { get; set; }

        public int Nivel { get; set; }

        public int Ordem { get; set; }

        public string Url { get; set; }

        public string FontIcon { get; set; }

        public string Feature1 { get; set; }

        public string Feature2 { get; set; }

  
        public bool Ativo { get; set; }
        public string DescAtivo { get; set; }

    }


    public class MenuPermissaoViewModel
    {
        public int Id { get; set; }

        public int MenuPaiId { get; set; }

        public int ModuloId { get; set; }

        public string DescMenu { get; set; }

        public int Nivel { get; set; }

        public int Ordem { get; set; }

        public string Url { get; set; }

        public string FontIcon { get; set; }

        public string Feature1 { get; set; }

        public string Feature2 { get; set; }

        public bool selected { get; set; }

        public bool Relacionado { get; set; }

    }
}