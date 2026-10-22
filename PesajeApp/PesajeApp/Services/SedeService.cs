using PesajeApp.Models;
using Npgsql;
using System.Text;

namespace PesajeApp.Services
{
    internal class SedeService
    {
        public async Task<List<Sede>> ObtenerTodosAsync()
        {
            var sedes = new List<Sede>();

            using var connection = App.Database.CreateConnection();
            await connection.OpenAsync();

            const string sql = """
                SELECT id, cod, nombre, descripcion, estado
                FROM sede
                WHERE estado = 1
                ORDER BY id;
                """;
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sedes.Add(new Sede
                {
                    Id = reader.GetInt32(0),
                    Cod = reader.GetInt64(1),
                    Nombre = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Estado = reader.GetInt32(4) == 1
                });
            }

            return sedes;
        }

        public async Task<int> CrearAsync(Sede sede)
        {
            using var connection = App.Database.CreateConnection();
            await connection.OpenAsync();

            const string sql = """
            INSERT INTO sede (cod, nombre, descripcion, estado)
            VALUES (@cod, @nombre, @descripcion, @estado)
            RETURNING id;
            """;

            using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@cod", sede.Cod);
            command.Parameters.AddWithValue("@nombre", (object?)sede.Nombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@descripcion", (object?)sede.Descripcion ?? DBNull.Value);
            command.Parameters.AddWithValue("@estado", sede.Estado ? 1 : 0 );

            var id = await command.ExecuteScalarAsync();

            return Convert.ToInt32(id);
        }

        public async Task ActualizarAsync(Sede sede)
        {
            using var connection = App.Database.CreateConnection();
            await connection.OpenAsync();

            const string sql = """
                UPDATE sede
                SET cod = @cod,
                    nombre = @nombre,
                    descripcion = @descripcion
                WHERE id = @id;
                """;

            using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", sede.Id);
            command.Parameters.AddWithValue("@cod", sede.Cod);
            command.Parameters.AddWithValue("@nombre", (object?)sede.Nombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@descripcion", (object?)sede.Descripcion ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            using var connection = App.Database.CreateConnection();
            await connection.OpenAsync();

            const string sql = """
                UPDATE sede
                SET estado = FALSE
                WHERE id = @id;
                """;

            using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
        public async Task<List<Sede>> ObtenerActivasAsync()
        {
            var sedes = new List<Sede>();

            using var connection = App.Database.CreateConnection();
            await connection.OpenAsync();

            const string sql = """
            SELECT id, cod, nombre, descripcion, estado
            FROM sede
            WHERE estado = 1
            ORDER BY nombre;
            """;

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sedes.Add(new Sede
                {
                    Id = reader.GetInt32(0),
                    Cod = reader.GetInt64(1),
                    Nombre = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Estado = reader.GetInt32(4) == 1
                });
            }

            return sedes;
        }
    }
}
