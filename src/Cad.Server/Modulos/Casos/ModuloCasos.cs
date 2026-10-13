using Cad.Server.Modulos.Casos.Aplicacion;

namespace Cad.Server.Modulos.Casos;

public static class ModuloCasos
{
    public static IServiceCollection AddModuloCasos(this IServiceCollection services)
    {
        services.AddScoped<ServicioSos>();
        return services;
    }
}
