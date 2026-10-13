# Sistema Integrado CAD

**Sistema Integrado de Monitoreo, Alertas Georreferenciadas y Despacho CAT para la Gestión de Seguridad Municipal, tipo B.**

Proyecto académico del curso Ingeniería de Software II (Universidad de San Martín de Porres).

## Objetivo

Que el ciudadano pueda pedir ayuda urgente (SOS), reportar incidencias y compartir de forma voluntaria y temporal su ubicación, y que la central de la municipalidad pueda recibir, verificar, asignar manualmente a un sereno y dar seguimiento a cada caso, con trazabilidad, control de acceso por rol y protección de la privacidad.

## Alcance del MVP académico

- Prototipo demostrativo con **datos simulados** (usuarios ficticios y coordenadas de prueba).
- **No** es un canal oficial de atención de emergencias.
- Sin integración con PNP, Bomberos, SAMU ni RENIEC.
- El despacho es **manual**: el operador asigna; no hay despacho automático ni cálculo de rutas.
- La app se valida en Android con la aplicación en primer plano y con conexión.

## Tecnologías

| Área | Tecnología |
|---|---|
| Servidor | ASP.NET Core 10 (Minimal APIs, Razor Pages, SignalR) |
| App móvil | .NET MAUI 10 para Android, MVVM con CommunityToolkit.Mvvm |
| Base de datos | PostgreSQL 17 con EF Core 10 y Npgsql |
| Seguridad | ASP.NET Core Identity (PBKDF2-HMAC-SHA512), JWT para la app y cookie para el panel |
| Mapas | Leaflet 1.9.4 y teselas de OpenStreetMap |
| Contenedores | Docker y Docker Compose |

## Estructura

```
src/
  Cad.Server/      Servidor CAD: API REST, Hub SignalR (/hubs/cad) y panel web de central
    Modulos/       Cuentas, Casos, Consultas, Despacho (por funcionalidad)
    Persistencia/  CadDbContext, configuraciones y migraciones
    TiempoReal/    Hub y notificador de eventos
    Panel/Pages/   Páginas Razor del panel
  Cad.Contratos/   DTO y nombres de eventos compartidos por servidor y app
  Cad.Movil/       App Android (modo ciudadano y modo sereno)
tests/             Proyectos de prueba (aún vacíos)
docs/              Documentación
```

## Roles del sistema

| Rol | Interfaz | Uso |
|---|---|---|
| Ciudadano | App móvil | Se registra, envía SOS y comparte su ubicación |
| Sereno | App móvil | Estado operativo y despachos (en desarrollo) |
| Operador | Panel web | Recibe y atiende los SOS en la central |
| Administrador | Panel web | Gestiona cuentas de operadores y serenos (en desarrollo) |

## Funcionalidades implementadas

- **Registro de ciudadano** (`POST /api/cuentas/registro`) y pantalla de registro en la app.
- **Inicio de sesión:** JWT para la app (`POST /api/auth/token`) y cookie para el panel (`/cuentas/login`).
- **Control de acceso por rol** en el SOS, la central y el Hub.
- **Permiso de ubicación** al abrir el modo ciudadano, sin transmitir posiciones.
- **SOS con un solo toque** (`POST /api/sos`), con o sin ubicación e idempotente por `IdSolicitud` (no se duplica al reintentar).
- **Recepción en tiempo real** en `/central`: aviso visual y sonoro, mapa con la ubicación y acuse `Presentado` para medir el tiempo de presentación.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Para compilar la app: carga de trabajo de MAUI para Android (`dotnet workload install maui-android`), JDK 17 y SDK de Android

## Ejecución local

1. **Base de datos.** Copia `.env.example` a `.env`, define `POSTGRES_PASSWORD` y levanta PostgreSQL:
   ```powershell
   docker compose up -d
   ```
2. **Secretos del servidor.** Se guardan en tu perfil de usuario, nunca en el repositorio:
   ```powershell
   dotnet user-secrets --project src/Cad.Server set "ConnectionStrings:Cad" "Host=localhost;Port=5432;Database=cad;Username=cad;Password=TU_CONTRASEÑA"
   dotnet user-secrets --project src/Cad.Server set "Jwt:ClaveFirma" "una-clave-de-al-menos-32-caracteres"
   ```
3. **Servidor.** Al arrancar aplica las migraciones:
   ```powershell
   dotnet run --project src/Cad.Server
   ```
   - Salud: http://localhost:5080/salud
   - Panel: http://localhost:5080
4. **App Android.** Con un teléfono conectado por USB (depuración activada):
   ```powershell
   adb reverse tcp:5080 tcp:5080
   dotnet build src/Cad.Movil -t:Run -f net10.0-android
   ```

> Las cuentas de operador y administrador todavía no se pueden crear desde el sistema (HU04 pendiente).

## Estado del Sprint 1

| Historia | Estado |
|---|---|
| TEC01 Entorno de validación | Estructura y Docker listos; sin publicar |
| HU01 Registro de ciudadano | Implementada |
| HU02 Inicio de sesión | Implementada (app y panel) |
| HU03 Acceso por rol | Parcial: políticas aplicadas, sin matriz de pruebas |
| HU04 Cuentas institucionales | Pendiente |
| HU06 Permiso de ubicación | Implementada |
| HU07 / HU08 Emisión de SOS | Implementadas |
| HU11 Recepción en central | Implementada |

**Pendiente:** HU04, pruebas automatizadas, medición formal de RNF02, despliegue en Render y Neon, pruebas en un celular Android y aplicación del diseño de Figma.

## Documentación

- [Estado actual del repositorio](docs/estado-actual.md)
- [Librerías del panel: versiones y licencias](src/Cad.Server/wwwroot/lib/LEEME.md)
- [Variables de entorno](.env.example)
- `docs/adr/` y `docs/diagramas/`: reservadas para los ADR y los diagramas del plan de arquitectura (aún vacías)
