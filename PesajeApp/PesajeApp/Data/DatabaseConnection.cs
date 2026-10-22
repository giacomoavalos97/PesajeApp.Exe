using Npgsql;
using Microsoft.Extensions.Configuration;

namespace PesajeApp.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;
        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostgreSQL")
                ?? throw new InvalidOperationException(
                    "No se encontro la cadena de conexion PostgreSQL.");
        }
        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
