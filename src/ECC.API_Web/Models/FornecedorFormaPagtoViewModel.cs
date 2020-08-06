namespace ECC.API_Web.Models
{
    public class FornecedorFormaPagtoViewModel
    {

        public int FormaPagtoId { get; set; }
        public int FornecedorId { get; set; }
        public int Desconto { get; set; }
        private string _vlFormaPagto;
        public string VlFormaPagto
        {
            get
            {
                return _vlFormaPagto;
            }
            set
            {
                _vlFormaPagto = value.Replace("R$", "");
            }
        }

        public string ValorMinParcela { get; set; }
        public string ValorMinParcelaPedido { get; set; }
        public bool Ativo { get; set; }
    }
}