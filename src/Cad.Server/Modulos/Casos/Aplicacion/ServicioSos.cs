using Cad.Contratos.Sos;
using Cad.Server.Compartido;
using Cad.Server.Modulos.Casos.Dominio;
using Cad.Server.Persistencia;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cad.Server.Modulos.Casos.Aplicacion;

/// <param name="Creado">Falso si el IdSolicitud ya existía y se devolvió el SOS registrado antes.</param>
public sealed record SosRegistrado(Caso Caso, bool Creado);

public sealed class ServicioSos(CadDbContext db, TimeProvider reloj)
{
    /// <summary>Margen para relojes de teléfono adelantados respecto del servidor.</summary>
    private static readonly TimeSpan ToleranciaReloj = TimeSpan.FromMinutes(2);

    /// <summary>
    /// HU07/HU08: registra un SOS "Pendiente" (caso + evento) en una sola transacción.
    /// RNF08: si el IdSolicitud ya existe para este ciudadano, devuelve el SOS existente sin duplicarlo.
    /// </summary>
    public async Task<Resultado<SosRegistrado>> EmitirAsync(Guid ciudadanoId, EmitirSosSolicitud solicitud, CancellationToken ct)
    {
        // PostgreSQL guarda microsegundos: así la respuesta de un reintento coincide con la original.
        var ahora = reloj.GetUtcNow();
        ahora = ahora.AddTicks(-(ahora.Ticks % 10));

        var errores = Validar(solicitud, ahora);
        if (errores.Count > 0)
            return new Error(TipoError.Validacion, "sos_invalido", "La solicitud de SOS no es válida.", errores);

        var existente = await BuscarAsync(ciudadanoId, solicitud.IdSolicitud, ct);
        if (existente is not null)
            return Resultado<SosRegistrado>.Exito(new SosRegistrado(existente, Creado: false));

        var ubicacion = solicitud.Ubicacion is { } u
            ? new UbicacionCaso(u.Latitud, u.Longitud, u.PrecisionMetros, u.ObtenidaEn)
            : null;

        var (caso, evento) = Caso.CrearSos(Guid.CreateVersion7(ahora), ciudadanoId, solicitud.IdSolicitud, ubicacion, ahora);
        db.Casos.Add(caso);
        db.EventosCaso.Add(evento);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Dos envíos simultáneos del mismo IdSolicitud: el índice único decide y se devuelve el que ganó.
            db.ChangeTracker.Clear();
            existente = await BuscarAsync(ciudadanoId, solicitud.IdSolicitud, ct)
                ?? throw new InvalidOperationException("Conflicto de unicidad sin SOS existente.", ex);
            return Resultado<SosRegistrado>.Exito(new SosRegistrado(existente, Creado: false));
        }

        return Resultado<SosRegistrado>.Exito(new SosRegistrado(caso, Creado: true));
    }

    private Task<Caso?> BuscarAsync(Guid ciudadanoId, Guid idSolicitud, CancellationToken ct) =>
        db.Casos.AsNoTracking()
            .SingleOrDefaultAsync(c => c.CiudadanoId == ciudadanoId && c.IdSolicitud == idSolicitud, ct);

    private static Dictionary<string, string[]> Validar(EmitirSosSolicitud s, DateTimeOffset ahora)
    {
        var errores = new Dictionary<string, string[]>();

        if (s.IdSolicitud == Guid.Empty)
            errores["idSolicitud"] = ["El IdSolicitud es obligatorio."];

        if (s.Ubicacion is { } u)
        {
            if (!UbicacionCaso.LatitudValida(u.Latitud))
                errores["ubicacion.latitud"] = ["La latitud debe estar entre -90 y 90."];
            if (!UbicacionCaso.LongitudValida(u.Longitud))
                errores["ubicacion.longitud"] = ["La longitud debe estar entre -180 y 180."];
            if (!UbicacionCaso.PrecisionValida(u.PrecisionMetros))
                errores["ubicacion.precisionMetros"] = ["La precisión no puede ser negativa."];
            if (u.ObtenidaEn > ahora + ToleranciaReloj)
                errores["ubicacion.obtenidaEn"] = ["La hora de obtención de la ubicación está en el futuro."];
        }

        return errores;
    }
}
