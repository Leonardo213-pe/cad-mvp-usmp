using Cad.Server.Modulos.Casos.Aplicacion;

namespace Cad.Server.Modulos.Casos;

public static class ModuloCasos
{
    public static IServiceCollection AddModuloCasos(this IServiceCollection services, IConfiguration configuracion)
    {
        services.Configure<OpcionesMedicion>(configuracion.GetSection(OpcionesMedicion.Seccion));
        services.AddScoped<ServicioSos>();
        services.AddScoped<ServicioPresentacion>();
        return services;
    }
}
