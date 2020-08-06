
using ECC.Dados.Repositorio;
using System.Net.Http;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.Entidades;

namespace ECC.API_Web.InfraWeb
{
    public class DataRepositoryFactory : IDataRepositoryFactory
    {
        public IEntidadeBaseRep<T> GetDataRepository<T>(HttpRequestMessage request) where T : class, IEntidadeBase, new()
        {
            return request.GetDataRepository<T>();
        }
    }

    public interface IDataRepositoryFactory
    {
        IEntidadeBaseRep<T> GetDataRepository<T>(HttpRequestMessage request) where T : class, IEntidadeBase, new();
    }
}