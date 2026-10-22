using Npgsql;
using PesajeApp.Models;

namespace PesajeApp.Services;

public class ClienteService
{
    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        var clientes = new List<Cliente>();

        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        SELECT
            c.id,
            c.sede_id,
            c.cod,
            c.nombres,
            c.estado,
            s.nombre AS sede_nombre
        FROM cliente c
        LEFT JOIN sede s ON s.id = c.sede_id
        WHERE c.estado = 1
        ORDER BY c.id;
        """;

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            clientes.Add(new Cliente
            {
                Id = reader.GetInt32(0),
                SedeId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                Cod = reader.IsDBNull(2) ? null : reader.GetInt64(2),
                Nombres = reader.IsDBNull(3) ? null : reader.GetString(3),
                Estado = reader.GetInt32(4) == 1,
                SedeNombre = reader.IsDBNull(5) ? null : reader.GetString(5)
            });
        }

        return clientes;
    }

    public async Task<int> CrearAsync(Cliente cliente)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        INSERT INTO cliente (sede_id, cod, nombres, estado)
        VALUES (@sede_id, @cod, @nombres, @estado)
        RETURNING id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@sede_id",
            (object?)cliente.SedeId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@cod",
            (object?)cliente.Cod ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@nombres",
            (object?)cliente.Nombres ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@estado",
            cliente.Estado ? 1 : 0 );

        var id = await command.ExecuteScalarAsync();

        return Convert.ToInt32(id);
    }

    public async Task ActualizarAsync(Cliente cliente)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        UPDATE cliente
        SET sede_id = @sede_id,
            cod = @cod,
            nombres = @nombres
        WHERE id = @id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", cliente.Id);

        command.Parameters.AddWithValue(
            "@sede_id",
            (object?)cliente.SedeId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@cod",
            (object?)cliente.Cod ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@nombres",
            (object?)cliente.Nombres ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        UPDATE cliente
        SET estado = FALSE
        WHERE id = @id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }
}