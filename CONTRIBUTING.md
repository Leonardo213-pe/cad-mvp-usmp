# Cómo contribuir

Flujo de trabajo del equipo del Sistema Integrado CAD. Para preparar el entorno, ver [docs/instalacion.md](docs/instalacion.md).

## Flujo

1. Actualiza `main`: `git switch main` y `git pull`.
2. Crea una rama por HU (o por grupo de HU que se aceptan juntas).
3. Implementa, compila y prueba.
4. Haz commit y `git push -u origin <rama>`.
5. Abre un **pull request hacia `main`** usando la plantilla.
6. Otro integrante lo revisa y lo fusiona desde GitHub. **No se hace push directo a `main`.**

## Nombres de ramas

| Tipo | Formato | Ejemplo |
|---|---|---|
| Historia de usuario | `feature/HUxx-descripcion-corta` | `feature/HU07-08-emision-sos` |
| Tarea técnica | `feature/TECxx-descripcion` | `feature/TEC01-base-desplegable` |
| Corrección | `fix/HUxx-descripcion` | `fix/HU11-reconexion-panel` |
| Documentación | `docs/descripcion` | `docs/sprint-01` |

## Mensajes de commit

- Primera línea: `HUxx: qué se hizo` (por ejemplo, `HU06: preparar la geolocalización al abrir el modo ciudadano`).
- En el cuerpo: los cambios por proyecto y lo que se probó.
- En español.

## Antes de abrir el pull request

- `dotnet build src/Cad.Server` sin errores ni advertencias, y la app si cambió (`dotnet build src/Cad.Movil -f net10.0-android`).
- Si cambió el modelo de datos, incluye la migración:
  `dotnet ef migrations add <Nombre> --project src/Cad.Server --output-dir Persistencia/Migraciones`.
- Indica en el pull request qué se probó y **qué no** (por ejemplo, "no probado en un dispositivo").
- No marques una HU como terminada si falta verificar alguno de sus criterios de aceptación.

## Reglas del proyecto

- **Secretos:** nunca en el repositorio. Usa `user-secrets` en local y variables de entorno en un servidor publicado; `.env` no se sube.
- **Privacidad:** sin coordenadas, contraseñas, tokens ni datos personales en los registros ni en las evidencias. Todos los datos son simulados.
- **Arquitectura:** respeta los módulos y capas de [docs/arquitectura.md](docs/arquitectura.md). Las escrituras van por REST y SignalR solo notifica.
- **Decisiones pendientes (DP):** no se inventan reglas; si se avanza, el supuesto se documenta en el pull request.
