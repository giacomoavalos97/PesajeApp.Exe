using Npgsql;
using PesajeApp.Models;

namespace PesajeApp.Services;

public class UsuarioService
{
    public async Task<Usuario?> AutenticarAsync(
        string nombre,
        string clave)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            SELECT
                u.id,
                u.sede_id,
                u.nombre,
                u.clave,
                u.rol,
                u.turno,
                u.estado,
                s.nombre AS sede_nombre
            FROM usuario u
            LEFT JOIN sede s ON s.id = u.sede_id
            WHERE u.nombre = @nombre
              AND u.clave = @clave
              AND u.estado = 1
            LIMIT 1;
            """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@nombre", nombre);
        command.Parameters.AddWithValue("@clave", clave);

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Usuario
        {
            Id = reader.GetInt32(0),
            SedeId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
            Nombre = reader.GetString(2),
            Clave = reader.GetString(3),
            Rol = reader.IsDBNull(4) ? null : reader.GetString(4),
            Turno = reader.GetString(5),
            Estado = reader.GetInt32(6) == 1,
            SedeNombre = reader.IsDBNull(7) ? null : reader.GetString(7)
        };
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        var usuarios = new List<Usuario>();

        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        SELECT
            u.id,
            u.sede_id,
            u.nombre,
            u.clave,
            u.rol,
            u.turno,
            u.estado,
            s.nombre AS sede_nombre
        FROM usuario u
        LEFT JOIN sede s ON s.id = u.sede_id
        WHERE u.estado = 1
        ORDER BY u.id;
        """;

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            usuarios.Add(new Usuario
            {
                Id = reader.GetInt32(0),
                SedeId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                Nombre = reader.GetString(2),
                Clave = reader.GetString(3),
                Rol = reader.IsDBNull(4) ? null : reader.GetString(4),
                Turno = reader.GetString(5),
                Estado = reader.GetInt32(6) == 1,
                SedeNombre = reader.IsDBNull(7) ? null : reader.GetString(7)
            });
        }

        return usuarios;
    }

    public async Task<int> CrearAsync(Usuario usuario)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        INSERT INTO usuario
            (sede_id, nombre, clave, rol, turno, estado)
        VALUES
            (@sede_id, @nombre, @clave, @rol, @turno, @estado)
        RETURNING id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@sede_id",
            (object?)usuario.SedeId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@nombre",
            usuario.Nombre);

        command.Parameters.AddWithValue(
            "@clave",
            usuario.Clave);

        command.Parameters.AddWithValue(
            "@rol",
            (object?)usuario.Rol ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@turno",
            usuario.Turno);

        command.Parameters.AddWithValue(
            "@estado",
            usuario.Estado ? 1 : 0 );

        var id = await command.ExecuteScalarAsync();

        return Convert.ToInt32(id);
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        UPDATE usuario
        SET sede_id = @sede_id,
            nombre = @nombre,
            clave = @clave,
            rol = @rol,
            turno = @turno
        WHERE id = @id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", usuario.Id);

        command.Parameters.AddWithValue(
            "@sede_id",
            (object?)usuario.SedeId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@nombre",
            usuario.Nombre);

        command.Parameters.AddWithValue(
            "@clave",
            usuario.Clave);

        command.Parameters.AddWithValue(
            "@rol",
            (object?)usuario.Rol ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@turno",
            usuario.Turno);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        UPDATE usuario
        SET estado = FALSE
        WHERE id = @id;
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }
}
