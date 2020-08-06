namespace ECC.API_Web.Models
{
    public class FormaPagtoViewModel
    {

        public int Id { get; set; }
        public string DescFormaPagto{ get; set; }
        public bool Avista{ get; set; }
        public int QtdParcelas { get; set; }
        public decimal Desconto { get; set; }

    }
}