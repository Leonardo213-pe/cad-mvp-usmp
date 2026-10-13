using Cad.Server.Modulos.Casos.Dominio;
using Cad.Server.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Cad.Server.Modulos.Casos.Aplicacion;

public sealed class OpcionesMedicion
{
    public const string Seccion = "Mediciones";

    /// <summary>Activa el registro en mediciones_presentacion durante la prueba de RNF02.</summary>
    public bool RegistrarPresentacion { get; set; }
}

/// <summary>Acuse "Presentado" del panel: mide aceptación → presentación de cada SOS (RNF02).</summary>
public sealed class ServicioPresentacion(
    CadDbContext db, TimeProvider reloj, IOptions<OpcionesMedicion> opciones, ILogger<ServicioPresentacion> log)
{
    public async Task RegistrarAsync(Guid casoId, Guid operadorId, CancellationToken ct)
    {
        var aceptadoEn = await db.Casos.AsNoTracking()
            .Where(c => c.Id == casoId)
            .Select(c => (DateTimeOffset?)c.CreadoEn)
            .SingleOrDefaultAsync(ct);
        if (aceptadoEn is null)
            return;

        var acuseEn = reloj.GetUtcNow();
        var medicion = MedicionPresentacion.Crear(casoId, operadorId, aceptadoEn.Value, acuseEn);

        // Solo identificadores y tiempos: nunca coordenadas ni datos del ciudadano (sección 9.7).
        log.LogInformation("SOS {CasoId} presentado en central en {Milisegundos:F0} ms", casoId, medicion.Milisegundos);

        if (!opciones.Value.RegistrarPresentacion)
            return;

        db.MedicionesPresentacion.Add(medicion);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Otro panel ya acusó este SOS: vale el primer acuse.
        }
    }
}
