# Instalación y ejecución

Guía para preparar un equipo Windows y ejecutar el Sistema Integrado CAD en local. Los comandos se ejecutan en PowerShell desde la raíz del repositorio.

## 1. Requisitos previos

| Herramienta | Para qué | Obligatoria |
|---|---|---|
| .NET SDK 10 | Compilar y ejecutar el servidor y la app | Sí |
| Docker Desktop (con WSL 2 en Windows Home) | PostgreSQL local | Sí |
| Git | Clonar el repositorio | Sí |
| Carga de trabajo `maui-android` | Compilar la app móvil | Solo para la app |
| JDK 17 y SDK de Android | Compilar el APK | Solo para la app |
| Teléfono Android con depuración USB | Probar la app | Solo para la app |

## 2. Versiones utilizadas

| Componente | Versión | Dónde se define |
|---|---|---|
| .NET SDK | 10.0.100 o superior dentro de la línea 10.0 (`rollForward: latestFeature`) | `global.json` |
| Marcos de destino | `net10.0` (servidor y contratos), `net10.0-android` (app) | `*.csproj` |
| PostgreSQL | 17 (imagen `postgres:17`) | `docker-compose.yml` |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 | `Cad.Server.csproj` |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 10.0.12 | `Cad.Server.csproj` |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.12 | `Cad.Server.csproj` |
| EFCore.NamingConventions | 10.0.1 | `Cad.Server.csproj` |
| dotnet-ef (herramienta local) | 10.0.12 | `dotnet-tools.json` |
| CommunityToolkit.Mvvm | 8.4.2 | `Cad.Movil.csproj` |
| Android mínimo | API 24 (Android 7.0) | `Cad.Movil.csproj` |
| Leaflet / cliente JS de SignalR | 1.9.4 / 10.0.11 | `src/Cad.Server/wwwroot/lib/LEEME.md` |

## 3. Instalación de dependencias

```powershell
git clone https://github.com/Leonardo213-pe/cad-mvp-usmp.git
cd cad-mvp-usmp
dotnet --list-sdks                 # debe mostrar una versión 10.0.x
dotnet tool restore                # instala dotnet-ef según dotnet-tools.json
dotnet restore src/Cad.Server
```

Solo para la app móvil:

```powershell
dotnet workload install maui-android
# Instala el SDK de Android (requiere un JDK 17 ya descargado)
dotnet build src/Cad.Movil -t:InstallAndroidDependencies -f net10.0-android `
  "-p:AndroidSdkDirectory=$env:LOCALAPPDATA\Android\Sdk" "-p:JavaSdkDirectory=<ruta del JDK 17>" `
  "-p:AcceptAndroidSDKLicenses=True"
```

Después define `JAVA_HOME` y `ANDROID_HOME` en tus variables de usuario y reinicia la terminal.

## 4. Configuración de PostgreSQL

1. Copia la plantilla de variables y completa la contraseña. `.env` está en `.gitignore` y nunca se sube:
   ```powershell
   Copy-Item .env.example .env
   ```
2. Levanta el contenedor `cad-postgres` (puerto 5432, volumen `cad-pgdata`):
   ```powershell
   docker compose up -d
   docker compose ps          # debe aparecer como "healthy"
   ```
3. No hace falta crear tablas: el servidor aplica las migraciones al arrancar (`BaseDatos:MigrarAlArrancar = true`).

Otros comandos útiles:

```powershell
docker compose stop        # detiene PostgreSQL y conserva los datos
docker compose down -v     # borra el contenedor y los datos
```

## 5. Variables de entorno y secretos

**Docker Compose (`.env`)**

| Variable | Valor por defecto |
|---|---|
| `POSTGRES_DB` | `cad` |
| `POSTGRES_USER` | `cad` |
| `POSTGRES_PASSWORD` | Sin valor por defecto: obligatoria |
| `POSTGRES_PORT` | `5432` |

**Servidor CAD.** En local se guardan con `user-secrets`; en un servidor publicado, como variables de entorno con `__`.

| Clave (user-secrets) | Variable de entorno | Obligatoria | Descripción |
|---|---|---|---|
| `ConnectionStrings:Cad` | `ConnectionStrings__Cad` | Sí | Cadena de conexión a PostgreSQL |
| `Jwt:ClaveFirma` | `Jwt__ClaveFirma` | Sí | Clave de firma del JWT, de al menos 32 caracteres |
| `Cuentas:TelefonoObligatorio` | `Cuentas__TelefonoObligatorio` | No (`false`) | Exige el teléfono al registrarse (DP01) |
| `Mediciones:RegistrarPresentacion` | `Mediciones__RegistrarPresentacion` | No (`true` solo en Development) | Guarda la medición de RNF02 |
| `Panel:Mapa:Latitud` / `Longitud` / `Zoom` | `Panel__Mapa__…` | No | Centro inicial del mapa de la central |

```powershell
dotnet user-secrets --project src/Cad.Server set "ConnectionStrings:Cad" "Host=localhost;Port=5432;Database=cad;Username=cad;Password=<POSTGRES_PASSWORD>"
dotnet user-secrets --project src/Cad.Server set "Jwt:ClaveFirma" "<clave de 32 caracteres o más>"
dotnet user-secrets --project src/Cad.Server list
```

`Admin__Email` y `Admin__ClaveInicial` aparecen en `.env.example`, pero el servidor todavía no las usa (HU04 pendiente).

## 6. Ejecutar el servidor

```powershell
dotnet run --project src/Cad.Server                           # perfil "http": http://localhost:5080
dotnet run --project src/Cad.Server --launch-profile red-local # escucha en 0.0.0.0:5080 (pruebas desde otro equipo)
```

- Salud: http://localhost:5080/salud
- Panel: http://localhost:5080 (login en `/cuentas/login`)

Imagen Docker del servidor, definida en el `Dockerfile` (puerto 8080):

```powershell
docker build -t cad-server .
```

## 7. Ejecutar la aplicación móvil

Con el teléfono conectado por USB y la depuración activada:

```powershell
adb reverse tcp:5080 tcp:5080       # en el teléfono, "localhost:5080" apunta al PC
dotnet build src/Cad.Movil -t:Run -f net10.0-android
```

- En Debug, la app usa `http://localhost:5080` y permite HTTP. En Release solo admite HTTPS (RNF09).
- La dirección del servidor se puede cambiar en la pantalla de inicio de sesión, con "Configurar servidor".
- `adb` está en `$env:ANDROID_HOME\platform-tools`.

## 8. Problemas comunes comprobados

| Síntoma | Causa | Solución |
|---|---|---|
| VS Code: "Hubo problemas al cargar el proyecto X.cs" | VS Code no encuentra el SDK de .NET (`sdk-not-found` en el registro de la extensión C#) | Añade el SDK al `PATH` y **cierra por completo** VS Code antes de reabrirlo |
| `NETSDK1147: … maui-android` al compilar la solución | Falta la carga de trabajo de MAUI | `dotnet workload install maui-android`, o compila solo `src/Cad.Server` |
| `MAUIR0004` al compilar la app | Un SVG de icono o de splash contiene texto | Usa SVG sin texto (conviértelo a contornos) |
| `MSB3026 … being used by another process: Cad.Server` | El servidor sigue en ejecución y bloquea sus DLL | Detén el servidor antes de compilar |
| `PendingModelChangesWarning` al arrancar | Se añadió una migración pero el servidor se ejecutó sin recompilar (`--no-build`) | `dotnet build src/Cad.Server` y vuelve a arrancar |
| Error de arranque por `ConnectionStrings__Cad` u `OpcionesJwt` | Faltan los secretos del paso 5 | Configúralos con `user-secrets` |
| Comandos `docker` no reconocidos | Docker Desktop no está instalado o no está en marcha | Abre Docker Desktop y espera a "Engine running" |
