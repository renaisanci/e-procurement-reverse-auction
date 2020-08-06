using ECC.API_Web.Models;

namespace ECC.API_Web.InfraWeb.Menu
{
    public class MenuAux : MenuViewModel
    {


       public MontaMenu Children { get; set; }

       public MenuAux()
       {
           Children = new MontaMenu();
       }

    }
}