using Cad.Server.Modulos.Cuentas.Aplicacion;

namespace Cad.Server.Modulos.Cuentas;

public static class ModuloCuentas
{
    public static IServiceCollection AddModuloCuentas(this IServiceCollection services, IConfiguration configuracion)
    {
        services.Configure<OpcionesCuentas>(configuracion.GetSection(OpcionesCuentas.Seccion));
        services.AddScoped<ServicioCuentas>();
        return services;
    }
}
