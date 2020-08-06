namespace ECC.Dados.Infra
{
    public interface IUnitOfWork
    {
        IDbFactory DbFactory { get; }
        void Commit();
    }
}
