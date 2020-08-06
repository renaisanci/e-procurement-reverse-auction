namespace ECC.API_Web.Comum.Util
{
	public class CEPService
	{
		///// <summary>
		///// Realiza pesquisa no WebService dos correios e retorna o endereço.
		///// </summary>
		///// <param name="cep"></param>
		///// <returns></returns>
		//public CepEndereco PesquisaCep(string cep)
		//{
		//	var cepService = new AtendeClienteService();
		//	var result = cepService.consultaCEP(cep);
		//	IUnitOfWork _unitOfWork;
		//	var estado = new Estado();
		//	if(result != null)
		//	{ 
		//		IEntidadeBaseRep<Estado> _estadoRep;
		//		estado = _estadoRep.GetAll().Where(x => x.Uf == result.uf).FirstOrDefault();

		//	}

			
		//	return new CepEndereco()
		//	{

		//		Cep = result.cep,
		//		Complemento = result.complemento,
		//		DescLogradouro = result.end,
		//		Logradouro = new EntidadeEndereco.Logradouro() { DescLogradouro = result.end },
		//		Bairro = new EntidadeEndereco.Bairro() { DescBairro = result.bairro },
		//		Estado = new EntidadeEndereco.Estado() { Uf = result.uf },
		//		Cidade = new EntidadeEndereco.Cidade() { DescCidade = result.cidade }

		//	};
		//}

	}
}
