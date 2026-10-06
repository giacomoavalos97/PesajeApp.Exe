using Npgsql;
using PesajeApp.Models;
using PesajeApp.Printing;
using System.Windows;
using NpgsqlTypes;

namespace PesajeApp.Services;

public class PesajeService
{

    public async Task<int> CrearAsync(Pesaje pesaje)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO pesaje (
                sede_id,
                producto_id,
                cantidad,
                usuario_id,
                turno,
                fechalocal,
                fechaserver,
                peso,
                precio,
                importe,
                estado,
                lote,
                codalt,
                op,
                kanban,
                pesotara,
                pesoneto,
                flagprecio,
                flaglote,
                flagbar,
                flagfechavencimiento,
                flagfechaperiodo,
                flagpesoneto,
                flagpesotara,
                empresa_nombre,
                empresa_titulo2,
                empresa_titulo3
            )
            VALUES (
                @sede_id,
                @producto_id,
                @cantidad,
                @usuario_id,
                @turno,
                @fechalocal,
                CURRENT_TIMESTAMP,
                @peso,
                @precio,
                @importe,
                @estado,
                @lote,
                @codalt,
                @op,
                @kanban,
                @pesotara,
                @pesoneto,
                @flagprecio,
                @flaglote,
                @flagbar,
                @flagfechavencimiento,
                @flagfechaperiodo,
                @flagpesoneto,
                @flagpesotara,
                @empresa_nombre,
                @empresa_titulo2,
                @empresa_titulo3
            )
            RETURNING id;
            """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@sede_id",
            (object?)pesaje.SedeId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@producto_id",
            (object?)pesaje.ProductoId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@cantidad",
            (object?)pesaje.Cantidad ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@usuario_id",
            (object?)pesaje.UsuarioId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@turno",
            (object?)pesaje.Turno ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@fechalocal",
            (object?)pesaje.FechaLocal ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@peso",
            (object?)pesaje.Peso ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@precio",
            (object?)pesaje.Precio ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@importe",
            (object?)pesaje.Importe ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@estado",
            pesaje.Estado ? 1 : 0 );

        command.Parameters.AddWithValue(
            "@lote",
            (object?)pesaje.Lote ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@codalt",
            (object?)pesaje.CodAlt ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@op",
            (object?)pesaje.OP ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@kanban",
            (object?)pesaje.Kanban ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@pesotara",
            (object?)pesaje.PesoTara ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@pesoneto",
            (object?)pesaje.PesoNeto ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@flagprecio",
            (object?)pesaje.FlagPrecio ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@flaglote",
            (object?)pesaje.FlagLote ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@flagbar",
            (object?)pesaje.FlagBar ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@flagfechavencimiento",
            (object?)pesaje.FlagFechaVencimiento ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@flagfechaperiodo",
            (object?)pesaje.FlagFechaPeriodo ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@flagpesoneto",
            (object?)pesaje.FlagPesoNeto ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@flagpesotara",
            (object?)pesaje.FlagPesoTara ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@empresa_nombre",
            (object?)pesaje.EmpresaNombre ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@empresa_titulo2",
            (object?)pesaje.EmpresaTitulo2 ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@empresa_titulo3",
            (object?)pesaje.EmpresaTitulo3 ?? DBNull.Value);

        var id = await command.ExecuteScalarAsync();

        return Convert.ToInt32(id);
    }

    public async Task<List<Pesaje>> ObtenerTodosAsync()
    {
        return await ObtenerFiltradosAsync(null, null, null, null, null);
    }

    public async Task<List<Pesaje>> ObtenerFiltradosAsync(
        DateTime? fechaDesde,
        DateTime? fechaHastaExclusiva,
        string? turno,
        string? busqueda,
        int? sedeId)
    {
        var pesajes = new List<Pesaje>();

        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        SELECT
            p.id,
            p.codalt,
            p.op,
            p.kanban,
            p.sede_id,
            p.cantidad,
            p.producto_id,
            p.usuario_id,
            p.turno,
            p.fechalocal,
            p.fechaserver,
            p.peso,
            p.precio,
            p.importe,
            p.estado,
            p.lote,
            p.pesotara,
            p.pesoneto,
            p.empresa_nombre,
            p.empresa_titulo2,
            p.empresa_titulo3,

            pr.cod AS producto_codigo,
            pr.descripcion AS producto_descripcion,

            u.nombre AS usuario_nombre,

            s.nombre AS sede_nombre

        FROM pesaje p

        LEFT JOIN producto pr
            ON pr.id = p.producto_id

        LEFT JOIN usuario u
            ON u.id = p.usuario_id

        LEFT JOIN sede s
            ON s.id = p.sede_id

        WHERE
            (@fecha_desde IS NULL OR p.fechalocal >= @fecha_desde)
            AND (@fecha_hasta IS NULL OR p.fechalocal < @fecha_hasta)
            AND (@turno IS NULL OR p.turno = @turno)
            AND (@sede_id IS NULL OR p.sede_id = @sede_id)
            AND (
                @busqueda IS NULL
                OR pr.descripcion ILIKE @busqueda
                OR pr.cod::text ILIKE @busqueda
                OR p.lote ILIKE @busqueda
                OR p.op ILIKE @busqueda
                OR p.kanban ILIKE @busqueda
                OR p.codalt ILIKE @busqueda
                OR u.nombre ILIKE @busqueda
            )

        ORDER BY
            p.fechalocal DESC NULLS LAST,
            p.id DESC;
        """;

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("@fecha_desde", NpgsqlDbType.Timestamp).Value =
            (object?)fechaDesde ?? DBNull.Value;
        command.Parameters.Add("@fecha_hasta", NpgsqlDbType.Timestamp).Value =
            (object?)fechaHastaExclusiva ?? DBNull.Value;
        command.Parameters.Add("@turno", NpgsqlDbType.Text).Value =
            (object?)turno ?? DBNull.Value;
        command.Parameters.Add("@sede_id", NpgsqlDbType.Integer).Value =
            (object?)sedeId ?? DBNull.Value;
        command.Parameters.Add("@busqueda", NpgsqlDbType.Text).Value =
            busqueda is null ? DBNull.Value : $"%{busqueda}%";

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var pesaje = new Pesaje
            {
                Id = reader.GetInt32(0),

                CodAlt = reader.IsDBNull(1)
                    ? null
                    : reader.GetString(1),

                OP = reader.IsDBNull(2)
                    ? null
                    : reader.GetString(2),

                Kanban = reader.IsDBNull(3)
                    ? null
                    : reader.GetString(3),

                SedeId = reader.IsDBNull(4)
                    ? null
                    : reader.GetInt32(4),

                Cantidad = reader.IsDBNull(5)
                    ? null
                    : reader.GetInt32(5),

                ProductoId = reader.IsDBNull(6)
                    ? null
                    : reader.GetInt32(6),

                UsuarioId = reader.IsDBNull(7)
                    ? null
                    : reader.GetInt32(7),

                Turno = reader.IsDBNull(8)
                    ? null
                    : reader.GetString(8),

                FechaLocal = reader.IsDBNull(9)
                    ? null
                    : reader.GetDateTime(9),

                FechaServer = reader.IsDBNull(10)
                    ? null
                    : reader.GetDateTime(10),

                Peso = reader.IsDBNull(11)
                    ? null
                    : reader.GetDecimal(11),

                Precio = reader.IsDBNull(12)
                    ? null
                    : reader.GetDecimal(12),

                Importe = reader.IsDBNull(13)
                    ? null
                    : reader.GetDecimal(13),

                // PostgreSQL guarda estado como integer (0/1)
                Estado = !reader.IsDBNull(14)
                    && reader.GetInt32(14) == 1,

                Lote = reader.IsDBNull(15)
                    ? null
                    : reader.GetString(15),

                PesoTara = reader.IsDBNull(16)
                    ? null
                    : reader.GetDecimal(16),

                PesoNeto = reader.IsDBNull(17)
                    ? null
                    : reader.GetDecimal(17),

                EmpresaNombre = reader.IsDBNull(18)
                    ? null
                    : reader.GetString(18),

                EmpresaTitulo2 = reader.IsDBNull(19)
                    ? null
                    : reader.GetString(19),

                EmpresaTitulo3 = reader.IsDBNull(20)
                    ? null
                    : reader.GetString(20),

                ProductoCodigo = reader.IsDBNull(21)
                    ? null
                    : reader.GetValue(21).ToString(),

                ProductoDescripcion = reader.IsDBNull(22)
                    ? null
                    : reader.GetString(22),

                UsuarioNombre = reader.IsDBNull(23)
                    ? null
                    : reader.GetString(23),

                SedeNombre = reader.IsDBNull(24)
                    ? null
                    : reader.GetString(24)
            };

            pesajes.Add(pesaje);
        }

        return pesajes;
    }

    public async Task<PesajeReporteData?> ObtenerDatosReporteAsync(int pesajeId)
    {
        using var connection = App.Database.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
        SELECT
            p.id,

            -- Empresa histórica del pesaje
            p.empresa_nombre,
            p.empresa_titulo2,
            p.empresa_titulo3,

            -- Sede
            s.nombre,

            -- Producto
            pr.cod,
            pr.descripcion,
            p.cantidad,

            -- Pesaje
            p.peso,
            p.pesotara,
            p.pesoneto,

            -- Precio
            p.precio,
            p.importe,

            -- Datos adicionales
            p.lote,
            p.codalt,
            p.op,
            p.kanban,

            -- Usuario / turno
            p.turno,
            u.nombre,

            -- Fecha
            p.fechalocal,

            -- Flags
            p.flagprecio,
            p.flaglote,
            p.flagbar,
            p.flagfechavencimiento,
            p.flagfechaperiodo,
            p.flagpesoneto,
            p.flagpesotara

        FROM pesaje p

        LEFT JOIN producto pr
            ON pr.id = p.producto_id

        LEFT JOIN usuario u
            ON u.id = p.usuario_id

        LEFT JOIN sede s
            ON s.id = p.sede_id

        WHERE p.id = @id;
        """;

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", pesajeId);

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new PesajeReporteData
        {
            Id = reader.GetInt32(0),

            // Empresa
            EmpresaNombre = reader.IsDBNull(1)
                ? null
                : reader.GetString(1),

            EmpresaTitulo2 = reader.IsDBNull(2)
                ? null
                : reader.GetString(2),

            EmpresaTitulo3 = reader.IsDBNull(3)
                ? null
                : reader.GetString(3),

            // Sede
            SedeNombre = reader.IsDBNull(4)
                ? null
                : reader.GetString(4),

            // Producto
            ProductoCodigo = reader.IsDBNull(5)
                ? null
                : reader.GetInt32(5).ToString(),

            ProductoDescripcion = reader.IsDBNull(6)
                ? string.Empty
                : reader.GetString(6),

            Cantidad = reader.IsDBNull(7)
                ? null
                : reader.GetInt32(7),

            // Pesaje
            Peso = reader.IsDBNull(8)
                ? 0
                : reader.GetDecimal(8),

            PesoTara = reader.IsDBNull(9)
                ? 0
                : reader.GetDecimal(9),

            PesoNeto = reader.IsDBNull(10)
                ? 0
                : reader.GetDecimal(10),

            // Precio
            Precio = reader.IsDBNull(11)
                ? 0
                : reader.GetDecimal(11),

            Importe = reader.IsDBNull(12)
                ? 0
                : reader.GetDecimal(12),

            // Datos adicionales
            Lote = reader.IsDBNull(13)
                ? null
                : reader.GetString(13),

            CodAlt = reader.IsDBNull(14)
                ? null
                : reader.GetString(14),

            OP = reader.IsDBNull(15)
                ? null
                : reader.GetString(15),

            Kanban = reader.IsDBNull(16)
                ? null
                : reader.GetString(16),

            // Usuario / turno
            Turno = reader.IsDBNull(17)
                ? null
                : reader.GetString(17),

            Usuario = reader.IsDBNull(18)
                ? null
                : reader.GetString(18),

            // Fecha
            FechaLocal = reader.IsDBNull(19)
                ? null
                : reader.GetDateTime(19),

            // Flags
            MostrarPrecio = !reader.IsDBNull(20)
                && reader.GetInt32(20) == 1,

            MostrarLote = !reader.IsDBNull(21)
                && reader.GetInt32(21) == 1,

            MostrarCodigoBarras = !reader.IsDBNull(22)
                && reader.GetInt32(22) == 1,

            MostrarFechaVencimiento = !reader.IsDBNull(23)
                && reader.GetInt32(23) == 1,

            MostrarFechaPeriodo = !reader.IsDBNull(24)
                && reader.GetInt32(24) == 1,

            MostrarPesoNeto = !reader.IsDBNull(25)
                && reader.GetInt32(25) == 1,

            MostrarPesoTara = !reader.IsDBNull(26)
                && reader.GetInt32(26) == 1
        };
    }
}
