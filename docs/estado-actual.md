# Estado actual del repositorio · Sprint 1

Resumen de lo que existe hoy en el código (rama `feature/HU11-tiempo-real-sos`, que incluye todo lo fusionado en `main` más HU11).

## Proyectos

| Proyecto | Tecnología | Contenido |
|---|---|---|
| `src/Cad.Server` | ASP.NET Core 10 (Minimal APIs, Razor Pages, SignalR) | API REST, panel web de central, Hub de tiempo real |
| `src/Cad.Contratos` | .NET 10 (biblioteca) | DTO y nombres de eventos compartidos por servidor y app |
| `src/Cad.Movil` | .NET MAUI 10 (solo Android), MVVM con CommunityToolkit.Mvvm | App con modo ciudadano y modo sereno |
| `tests/Cad.Server.Tests`, `tests/Cad.PruebaCarga` | — | Solo carpetas vacías (`.gitkeep`); aún sin pruebas |

## Tecnologías presentes

- **Base de datos:** PostgreSQL 17 (`docker-compose.yml`) con EF Core 10 + Npgsql y nombres en snake_case. Migraciones: `Inicial`, `CasosSos`, `MedicionPresentacion`.
- **Seguridad:** ASP.NET Core Identity (hash PBKDF2-HMAC-SHA512, formato V3), JWT Bearer para la app y cookie HttpOnly para el panel.
- **Tiempo real:** SignalR (`/hubs/cad`).
- **Mapas:** Leaflet 1.9.4 con teselas de OpenStreetMap (en `wwwroot/lib`).
- **Despliegue:** `Dockerfile` (imagen aspnet:10.0). Aún no publicado en Render ni Neon.
- **Herramientas:** `global.json` (SDK 10), `dotnet-tools.json` (dotnet-ef), `Cad.slnx`.

## Estructura del servidor

- `Modulos/Cuentas`: registro, inicio de sesión y perfil del ciudadano.
- `Modulos/Casos`: dominio del caso (SOS), historial de eventos y medición de presentación.
- `Modulos/Consultas`: lecturas para la central (SOS pendientes).
- `Modulos/Despacho`: solo la entidad `EstadoSereno` (sin funcionalidad todavía).
- `Modulos/Acompanamiento`, `Temporizador`: carpetas vacías.
- `Persistencia`, `TiempoReal`, `Compartido`, `Panel/Pages`.

## Funcionalidades del Sprint 1 visibles en el código

| HU | Estado en el código |
|---|---|
| TEC01 Entorno | Estructura, Docker, `/salud`. Falta publicar en Render y Neon. |
| HU01 Registro de ciudadano | `POST /api/cuentas/registro` y pantalla en la app. |
| HU02 Inicio de sesión | `POST /api/auth/token` (JWT, app) y `/cuentas/login` (cookie, panel). |
| HU03 Acceso por rol | Políticas por rol en el SOS, la central y el Hub. Sin matriz de pruebas. |
| HU04 Cuentas institucionales | **No implementada.** No hay alta de operadores ni serenos ni semilla de administrador. |
| HU06 Permiso de ubicación | Comprobación y solicitud del permiso en el modo ciudadano; no transmite posición. |
| HU07/HU08 SOS | `POST /api/sos` idempotente por `IdSolicitud`, con o sin ubicación; botón SOS con reintento. |
| HU11 Recepción en central | Evento `SosNuevo`, página `/central` con aviso visual y sonoro, mapa y acuse `Presentado`. |

## Documentación existente

- No hay `README.md` ni documentos en `docs/` (solo `docs/adr/` y `docs/diagramas/` vacías).
- `src/Cad.Server/wwwroot/lib/LEEME.md`: versiones y licencias de Leaflet y SignalR.
- `.env.example`: nombres de variables de entorno sin valores.

## Pendientes para cerrar el Sprint 1

- HU04 (cuentas de operador y sereno y semilla del administrador).
- Pruebas automatizadas (`Cad.Server.Tests`) y la medición formal de RNF02 (`Cad.PruebaCarga`).
- Despliegue en Render y Neon con HTTPS.
- Pruebas de la app en un celular Android real.
- Diseño de Figma (decisión pendiente: iniciar sesión con DNI o con email).
