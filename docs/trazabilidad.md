# Trazabilidad · Sprint 1

Relación entre las historias del Sprint 1, los requisitos del Plan Scrum (numeración RF01.1–RF23.2) y el código. Los estados son los de [sprints/sprint-01.md](sprints/sprint-01.md).

- **Prueba "Manual":** comprobación hecha a mano contra PostgreSQL local el 13/10/2026 y descrita en el mensaje de commit. No hay prueba automatizada en el repositorio.
- **Prueba "Compilación":** solo se verificó que compila; sin ejecución funcional.

Rutas relativas a `src/`.

## Historias de usuario

| HU | RF relacionado | Archivo de implementación | Prueba | Estado |
|---|---|---|---|---|
| HU01 Registrar cuenta de ciudadano | RF01.1 · RNF01 | `Cad.Server/Modulos/Cuentas/Aplicacion/ServicioCuentas.cs` (`RegistrarCiudadanoAsync`), `Cad.Server/Modulos/Cuentas/Endpoints/CuentasEndpoints.cs`, `Cad.Server/Persistencia/ConfiguracionPersistencia.cs`, `Cad.Movil/Vistas/Comun/RegistroPage.xaml`, `Cad.Movil/VistasModelo/RegistroViewModel.cs` | Manual (API, incluidos 10 registros concurrentes); app: Compilación | Implementada |
| HU02 Iniciar sesión | RF01.2 | `ServicioCuentas.cs` (`ValidarCredencialesAsync`, `EmitirTokenAsync`), `Cad.Server/Compartido/Seguridad/EmisorJwt.cs`, `Cad.Server/Panel/Pages/Cuentas/Login.cshtml.cs`, `Cad.Movil/VistasModelo/LoginViewModel.cs` | Manual (API y login del panel); app: Compilación | Implementada |
| HU03 Acceder solo a mi rol | RF02 · RNF12 | `Cad.Server/Compartido/Seguridad/Politicas.cs`, `Cad.Server/Compartido/Seguridad/ConfiguracionSeguridad.cs`, `Cad.Server/Program.cs` (convenciones de páginas) | Manual (401/403 en SOS, central, panel y Hub); sin matriz endpoint × rol | Parcial |
| HU04 Cuentas institucionales | RF15 | — | Ninguna | Pendiente |
| HU06 Preparar la geolocalización | RF04 · RNF07 | `Cad.Movil/Servicios/UbicacionServicio.cs`, `Cad.Movil/VistasModelo/InicioCiudadanoViewModel.cs`, `Cad.Movil/Platforms/Android/AndroidManifest.xml` | Compilación (permisos del manifiesto revisados) | Implementada |
| HU07 Emitir SOS con una activación | RF03.1 · RNF06 · RNF08 | `Cad.Server/Modulos/Casos/Aplicacion/ServicioSos.cs`, `Cad.Server/Modulos/Casos/Endpoints/SosEndpoints.cs`, `Cad.Server/Modulos/Casos/Dominio/Caso.cs`, `Cad.Server/Persistencia/Configuraciones/CasoConfiguracion.cs`, `Cad.Movil/VistasModelo/SosViewModel.cs`, `Cad.Movil/Servicios/SosPendienteServicio.cs` | Manual (API, incluidos 10 envíos concurrentes); app: Compilación | Implementada |
| HU08 Asociar la ubicación actual al SOS | RF03.2 | `Cad.Movil/Servicios/UbicacionServicio.cs` (`ObtenerPosicionActualAsync`), `Cad.Server/Modulos/Casos/Dominio/UbicacionCaso.cs`, `Cad.Contratos/Sos/EmitirSos.cs` | Manual (API con y sin ubicación, latitud fuera de rango); captura en el dispositivo: Compilación | Implementada |
| HU11 Recibir SOS en la sección urgente | RF05.1 · RNF02 | `Cad.Server/TiempoReal/CadHub.cs`, `Cad.Server/TiempoReal/NotificadorSignalR.cs`, `Cad.Server/Modulos/Consultas/Aplicacion/ConsultasCentral.cs`, `Cad.Server/Modulos/Casos/Aplicacion/ServicioPresentacion.cs`, `Cad.Server/Panel/Pages/Central/Index.cshtml`, `Cad.Server/wwwroot/js/central.js` | Manual (cliente SignalR externo: evento, reintento sin aviso y acuse); página no revisada en un navegador | Implementada |

## Elementos técnicos

| Elemento | Requisito relacionado | Archivo de implementación | Prueba | Estado |
|---|---|---|---|---|
| TEC01 Entorno de validación | RNF09 · RNF10 · RNF14 | `docker-compose.yml`, `Dockerfile`, `global.json`, `Cad.slnx`, `Cad.Server/Program.cs` (`/salud`) | Manual (`/salud` en local) | Parcial: sin publicar |
| SPIKE01 Ubicación en primer plano, segundo plano, sin conexión y con permiso denegado | RNF07 · RNF11 | — | Ninguna | Pendiente: sin nota técnica |

## Huecos de trazabilidad

- Los criterios de aceptación (CA) de cada HU no están en el repositorio, así que no se pueden enlazar uno a uno.
- No hay pruebas automatizadas que enlazar (`tests/` está vacío).
- RNF02 tiene una sola medición de prueba, no el informe de 100 SOS con 20 sesiones.
