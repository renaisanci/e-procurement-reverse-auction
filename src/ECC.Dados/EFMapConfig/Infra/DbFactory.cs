namespace ECC.Dados.Infra
{

    public class DbFactory : Disposable, IDbFactory
    {
        EconomizajaContext dbContext;

        public EconomizajaContext Init()
        {
            return dbContext ?? (dbContext = new EconomizajaContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null) 
                dbContext.Dispose();
        }
    }
}
