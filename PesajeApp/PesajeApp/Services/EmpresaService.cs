using Npgsql;
using PesajeApp.Models;

namespace PesajeApp.Services;

public class EmpresaService
{
    public async Task<Empresa?> ObtenerAsync()
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        SELECT
            id,
            nombre,
            titulo2,
            titulo3
        FROM empresa
        ORDER BY id
        LIMIT 1;
        """;

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Empresa
        {
            Id = reader.GetInt32(0),
            Nombre = reader.IsDBNull(1) ? null : reader.GetString(1),
            Titulo2 = reader.IsDBNull(2) ? null : reader.GetString(2),
            Titulo3 = reader.IsDBNull(3) ? null : reader.GetString(3)
        };
    }

    public async Task<int> CrearAsync(Empresa empresa)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        INSERT INTO empresa (
            nombre,
            titulo2,
            titulo3
        )
        VALUES (
            @nombre,
            @titulo2,
            @titulo3
        )
        RETURNING id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@nombre",
            (object?)empresa.Nombre ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@titulo2",
            (object?)empresa.Titulo2 ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@titulo3",
            (object?)empresa.Titulo3 ?? DBNull.Value);

        var id = await command.ExecuteScalarAsync();

        return Convert.ToInt32(id);
    }

    public async Task ActualizarAsync(Empresa empresa)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        UPDATE empresa
        SET nombre = @nombre,
            titulo2 = @titulo2,
            titulo3 = @titulo3
        WHERE id = @id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", empresa.Id);

        command.Parameters.AddWithValue(
            "@nombre",
            (object?)empresa.Nombre ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@titulo2",
            (object?)empresa.Titulo2 ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@titulo3",
            (object?)empresa.Titulo3 ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }
}