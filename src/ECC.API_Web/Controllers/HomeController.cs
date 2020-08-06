using System.Web.Mvc;

namespace ECC.API_Web.Controllers
{
    public class HomeController : Controller
    {

      
        public ActionResult Index()
        {
            //TESTE FAZENDO O LOGIN NA WEBAPI E POSSIBILITANDO CONSUMIR OS SERVIÇOS DE UMA APLICAÇÃO MVC
            //IREMOS USAR ESTE EXEMPLO DE CODIGO CASO A GENTE FAÇA O MODULO DE FORNECEDOR EM MVC
            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost:1312/");

            //LoginViewModel login = new LoginViewModel()
            //{
            //UsuarioEmail = "renaisanci@gmail.com",
            //Senha = "homecinema"
            //};
            
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //HttpResponseMessage response = client.PostAsync("api/conta/autenticar", login, new JsonMediaTypeFormatter()).Result;
            
            //if (response.IsSuccessStatusCode)
            //{
            
            //    client.DefaultRequestHeaders.Add("Authorization",
            //        "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(login.UsuarioEmail + ":" + login.Senha)));

            //      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //      HttpResponseMessage response2 = client.GetAsync("api/painel/home").Result;

            //    var loginResult = response2.Content.ReadAsAsync<IEnumerable<LoginViewModel>>();

            //}
            //else
            //{

            //}

            return View();
        }
    }
}