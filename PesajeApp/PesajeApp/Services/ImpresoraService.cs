using Npgsql;
using PesajeApp.Models;

namespace PesajeApp.Services;

public class ImpresoraService
{
    public async Task<Impresora?> ObtenerAsync()
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            SELECT
                id,
                nombre,
                descripcion,
                estado
            FROM impresora
            WHERE estado = 1
            ORDER BY id
            LIMIT 1;
            """;

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Impresora
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Descripcion = reader.IsDBNull(2)
                ? null
                : reader.GetString(2),
            Estado = reader.GetInt32(3) == 1
        };
    }

    public async Task<List<Impresora>> ObtenerTodasAsync()
    {
        var impresoras = new List<Impresora>();

        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            SELECT
                id,
                nombre,
                descripcion,
                estado
            FROM impresora
            WHERE estado = 1
            ORDER BY id;
            """;

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            impresoras.Add(new Impresora
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Descripcion = reader.IsDBNull(2)
                    ? null
                    : reader.GetString(2),
                Estado = reader.GetInt32(3) == 1
            });
        }

        return impresoras;
    }

    public async Task<int> CrearAsync(Impresora impresora)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO impresora (
                nombre,
                descripcion,
                estado
            )
            VALUES (
                @nombre,
                @descripcion,
                @estado
            )
            RETURNING id;
            """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@nombre",
            impresora.Nombre);

        command.Parameters.AddWithValue(
            "@descripcion",
            (object?)impresora.Descripcion ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@estado",
            impresora.Estado ? 1 : 0);

        var id = await command.ExecuteScalarAsync();

        return Convert.ToInt32(id);
    }

    public async Task ActualizarAsync(Impresora impresora)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            UPDATE impresora
            SET nombre = @nombre,
                descripcion = @descripcion
            WHERE id = @id;
            """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@id",
            impresora.Id);

        command.Parameters.AddWithValue(
            "@nombre",
            impresora.Nombre);

        command.Parameters.AddWithValue(
            "@descripcion",
            (object?)impresora.Descripcion ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            UPDATE impresora
            SET estado = FALSE
            WHERE id = @id;
            """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@id",
            id);

        await command.ExecuteNonQueryAsync();
    }
}