using System.Data;
using System.Data.SqlClient;
namespace DotnetCore.Data
{
    public class ConnectionHelper
    {
        private readonly string _connString;
        public ConnectionHelper(string connString)
        {
            this._connString = connString;
            connString = this._connString;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connString);
        }
    }
}
