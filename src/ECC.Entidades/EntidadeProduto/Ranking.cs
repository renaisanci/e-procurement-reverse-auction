using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeProduto
{
   public  class Ranking :  EntidadeBase   
    {

       public Ranking()
        {      


        }

       public int ProdutoId { get; set; }
       public virtual Produto Produto { get; set; }
       public int Nota { get; set; }


    }
}
