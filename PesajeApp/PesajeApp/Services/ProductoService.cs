using Npgsql;
using PesajeApp.Models;

namespace PesajeApp.Services;

public class ProductoService
{
    public async Task<List<Producto>> ObtenerTodosAsync()
    {
        var productos = new List<Producto>();

        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            SELECT
                p.id,
                p.sede_id,
                p.cod,
                p.descripcion,
                p.precio,
                p.caducidad,
                p.cantidad,
                p.estado,
                s.nombre AS sede_nombre
            FROM producto p
            LEFT JOIN sede s ON s.id = p.sede_id
            WHERE p.estado = 1
            ORDER BY p.id;
            """;

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            productos.Add(new Producto
            {
                Id = reader.GetInt32(0),
                SedeId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                Cod = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                Descripcion = reader.GetString(3),
                Precio = reader.GetDecimal(4),
                Caducidad = reader.GetInt32(5),
                Cantidad = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                Estado = reader.GetInt32(7) == 1,
                SedeNombre = reader.IsDBNull(8) ? null : reader.GetString(8)
            });
        }

        return productos;
    }

    public async Task<int> CrearAsync(Producto producto)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        INSERT INTO producto (
            sede_id,
            cod,
            descripcion,
            precio,
            caducidad,
            cantidad,
            estado
        )
        VALUES (
            @sede_id,
            @cod,
            @descripcion,
            @precio,
            @caducidad,
            @cantidad,
            @estado
        )
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@sede_id",
            (object?)producto.SedeId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@cod",
            (object?)producto.Cod ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@descripcion",
            producto.Descripcion);

        command.Parameters.AddWithValue(
            "@precio",
            producto.Precio);

        command.Parameters.AddWithValue(
            "@caducidad",
            producto.Caducidad);

        command.Parameters.AddWithValue(
            "@cantidad",
            (object?)producto.Cantidad ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@estado",
            producto.Estado ? 1 : 0 );

        var id = await command.ExecuteScalarAsync();

        return Convert.ToInt32(id);
    }

    public async Task ActualizarAsync(Producto producto)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        UPDATE producto
        SET sede_id = @sede_id,
            cod = @cod,
            descripcion = @descripcion,
            precio = @precio,
            caducidad = @caducidad,
            cantidad = @cantidad
        WHERE id = @id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", producto.Id);

        command.Parameters.AddWithValue(
            "@sede_id",
            (object?)producto.SedeId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@cod",
            (object?)producto.Cod ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@descripcion",
            producto.Descripcion);

        command.Parameters.AddWithValue(
            "@precio",
            producto.Precio);

        command.Parameters.AddWithValue(
            "@caducidad",
            producto.Caducidad);

        command.Parameters.AddWithValue(
            "@cantidad",
            (object?)producto.Cantidad ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        UPDATE producto
        SET estado = FALSE
        WHERE id = @id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }
}