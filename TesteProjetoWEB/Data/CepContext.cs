using System.Data;

public class CepContext
{
    public delegate Task<IDbConnection> GetConnection();
}