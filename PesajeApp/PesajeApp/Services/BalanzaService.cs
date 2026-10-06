using Npgsql;
using PesajeApp.Models;

namespace PesajeApp.Services;

public class BalanzaService
{
    public async Task<Balanza?> ObtenerAsync()
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            SELECT
                id,
                modelo,
                modo,
                port_name,
                baud_rate,
                data_bits,
                parity
            FROM balanza
            ORDER BY id
            LIMIT 1;
            """;

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Balanza
        {
            Id = reader.GetInt32(0),
            Modelo = reader.GetString(1),
            Modo = reader.GetString(2),
            PortName = reader.GetString(3),
            BaudRate = reader.GetInt32(4),
            DataBits = reader.GetInt32(5),
            Parity = reader.GetString(6)
        };
    }

    public async Task GuardarAsync(Balanza balanza)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO balanza
                (modelo, modo, port_name, baud_rate, data_bits, parity)
            VALUES
                (@modelo, @modo, @port_name, @baud_rate, @data_bits, @parity);
            """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@modelo", balanza.Modelo);
        command.Parameters.AddWithValue("@modo", balanza.Modo);
        command.Parameters.AddWithValue("@port_name", balanza.PortName);
        command.Parameters.AddWithValue("@baud_rate", balanza.BaudRate);
        command.Parameters.AddWithValue("@data_bits", balanza.DataBits);
        command.Parameters.AddWithValue("@parity", balanza.Parity);

        await command.ExecuteNonQueryAsync();
    }

    public async Task ActualizarAsync(Balanza balanza)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            UPDATE balanza
            SET modelo = @modelo,
                modo = @modo,
                port_name = @port_name,
                baud_rate = @baud_rate,
                data_bits = @data_bits,
                parity = @parity
            WHERE id = @id;
            """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", balanza.Id);
        command.Parameters.AddWithValue("@modelo", balanza.Modelo);
        command.Parameters.AddWithValue("@modo", balanza.Modo);
        command.Parameters.AddWithValue("@port_name", balanza.PortName);
        command.Parameters.AddWithValue("@baud_rate", balanza.BaudRate);
        command.Parameters.AddWithValue("@data_bits", balanza.DataBits);
        command.Parameters.AddWithValue("@parity", balanza.Parity);

        await command.ExecuteNonQueryAsync();
    }
}