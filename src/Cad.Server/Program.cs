using Cad.Contratos.Salud;
using Cad.Contratos.TiempoReal;
using Cad.Server.Compartido;
using Cad.Server.Compartido.Seguridad;
using Cad.Server.Modulos.Casos;
using Cad.Server.Modulos.Casos.Endpoints;
using Cad.Server.Modulos.Consultas;
using Cad.Server.Modulos.Consultas.Endpoints;
using Cad.Server.Modulos.Cuentas;
using Cad.Server.Modulos.Cuentas.Endpoints;
using Cad.Server.Persistencia;
using Cad.Server.TiempoReal;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddPersistenciaCad(builder.Configuration);
builder.Services.AddSeguridadCad(builder.Configuration);
builder.Services.AddModuloCuentas(builder.Configuration);
builder.Services.AddModuloCasos(builder.Configuration);
builder.Services.AddModuloConsultas();

// Tiempo real: solo eventos hacia los clientes, después de cada COMMIT (ADR-04).
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificador, NotificadorSignalR>();

// Panel de central servido por el mismo proceso (sección 3.2). Todas sus páginas exigen sesión de panel.
builder.Services.AddRazorPages(o =>
{
    o.RootDirectory = "/Panel/Pages";
    o.Conventions.AuthorizeFolder("/", Politicas.PanelCentral);
    o.Conventions.AuthorizeFolder("/Central", Politicas.OperadorCentral);
    o.Conventions.AllowAnonymousToFolder("/Cuentas");
});

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
app.MapCentralEndpoints();
app.MapHub<CadHub>(TiempoRealCad.RutaHub);
app.MapRazorPages();

app.Run();

// Expuesto para WebApplicationFactory en Cad.Server.Tests.
public partial class Program;
