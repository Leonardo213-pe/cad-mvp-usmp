using Cad.Contratos.Salud;
using Cad.Server.Compartido.Seguridad;
using Cad.Server.Modulos.Casos;
using Cad.Server.Modulos.Casos.Endpoints;
using Cad.Server.Modulos.Cuentas;
using Cad.Server.Modulos.Cuentas.Endpoints;
using Cad.Server.Persistencia;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddPersistenciaCad(builder.Configuration);
builder.Services.AddSeguridadCad(builder.Configuration);
builder.Services.AddModuloCuentas(builder.Configuration);
builder.Services.AddModuloCasos();

// Panel de central servido por el mismo proceso (sección 3.2).
builder.Services.AddRazorPages(o => o.RootDirectory = "/Panel/Pages");

var app = builder.Build();

app.UseExceptionHandler();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

if (app.Configuration.GetValue("BaseDatos:MigrarAlArrancar", true))
{
    await app.MigrarBaseDeDatosAsync();
}

// Liveness: no toca la base de datos para no despertar Neon (ADR-05).
app.MapGet("/salud", () => Results.Ok(new EstadoSaludDto("ok", DateTimeOffset.UtcNow)));

app.MapCuentasEndpoints();
app.MapSosEndpoints();
app.MapRazorPages();

app.Run();

// Expuesto para WebApplicationFactory en Cad.Server.Tests.
public partial class Program;
