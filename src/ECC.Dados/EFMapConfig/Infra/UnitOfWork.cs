namespace ECC.Dados.Infra
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory _dbFactory;
        private EconomizajaContext _dbContext;

        public IDbFactory DbFactory => _dbFactory;

        public UnitOfWork(IDbFactory dbFactory)
        {
            this._dbFactory = dbFactory;
        }

        public EconomizajaContext DbContext => _dbContext ?? (_dbContext = _dbFactory.Init());

        public void Commit()
        {           
            DbContext.Commit();
        }
    }
}
