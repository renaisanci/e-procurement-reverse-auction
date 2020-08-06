using ECC.Servicos.Abstrato;
using System.Net.Http;
using ECC.Dados.Repositorio;
using System.Web.Http.Dependencies;
using ECC.Entidades;

namespace ECC.API_Web.InfraWeb.ExtensionsWeb
{
    public static class RequestMessageExtensions
    {

        internal static IMembershipService GetMembershipService(this HttpRequestMessage request)
        {
            return request.GetService<IMembershipService>();
        }

        internal static IEntidadeBaseRep<T> GetDataRepository<T>(this HttpRequestMessage request) where T : class, IEntidadeBase, new()
        {
            return request.GetService<IEntidadeBaseRep<T>>();
        }

        private static TService GetService<TService>(this HttpRequestMessage request)
        {
            IDependencyScope dependencyScope = request.GetDependencyScope();
            TService service = (TService)dependencyScope.GetService(typeof(TService));

            return service;
        }
    }
}