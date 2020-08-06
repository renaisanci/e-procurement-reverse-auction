using System;


namespace ECC.Dados.Infra
{
 
    public interface IDbFactory : IDisposable
    {
        EconomizajaContext Init();
    }
}
