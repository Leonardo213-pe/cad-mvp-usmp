using Cad.Contratos.Sos;
using Cad.Server.Modulos.Casos.Dominio;
using Cad.Server.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Cad.Server.Modulos.Consultas.Aplicacion;

/// <summary>Lecturas para el panel de central. Solo lectura y sin seguimiento (sección 3.5).</summary>
public sealed class ConsultasCentral(CadDbContext db)
{
    private const int MaximoSosPendientes = 200;

    /// <summary>SOS en "Pendiente" para la sección urgente, del más antiguo al más reciente.</summary>
    public Task<List<SosCentralDto>> SosPendientesAsync(CancellationToken ct) =>
        ConsultarSosAsync(
            db.Casos.AsNoTracking().Where(c => c.Origen == OrigenCaso.Sos && c.Estado == EstadoCaso.Pendiente),
            MaximoSosPendientes, ct);

    /// <summary>Un SOS concreto, para emitirlo en el evento SosNuevo.</summary>
    public async Task<SosCentralDto?> SosAsync(Guid casoId, CancellationToken ct) =>
        (await ConsultarSosAsync(
            db.Casos.AsNoTracking().Where(c => c.Id == casoId && c.Origen == OrigenCaso.Sos), 1, ct))
        .SingleOrDefault();

    // Los filtros se aplican antes del join; la proyección es anónima para que EF Core la traduzca a SQL.
    private async Task<List<SosCentralDto>> ConsultarSosAsync(IQueryable<Caso> casos, int maximo, CancellationToken ct)
    {
        var filas = await (
                from caso in casos
                join perfil in db.PerfilesCiudadano.AsNoTracking() on caso.CiudadanoId equals perfil.UsuarioId into perfiles
                from perfil in perfiles.DefaultIfEmpty()
                orderby caso.CreadoEn
                select new
                {
                    Caso = caso,
                    Nombre = perfil != null ? perfil.NombreCompleto : null,
                    Telefono = perfil != null ? perfil.Telefono : null
                })
            .Take(maximo)
            .ToListAsync(ct);

        return filas.Select(f => ADto(f.Caso, f.Nombre, f.Telefono)).ToList();
    }

    private static SosCentralDto ADto(Caso c, string? nombre, string? telefono)
    {
        var ubicacion = c.TieneUbicacion
            ? new UbicacionDto(c.Latitud!.Value, c.Longitud!.Value, c.PrecisionMetros!.Value, c.UbicacionObtenidaEn!.Value)
            : null;

        return new SosCentralDto(c.Id, c.Estado.ToString(), c.CreadoEn, nombre ?? "Ciudadano sin perfil", telefono, ubicacion);
    }
}
