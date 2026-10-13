# Sprint 1 · Entorno, cuentas, permisos y emisión/recepción inicial de SOS

| Dato | Valor |
|---|---|
| Fechas planificadas (Plan Scrum) | 07/09/2026 – 16/09/2026 |
| Desarrollo real registrado en Git | 13/10/2026 (commits `4a97cac` a `3f489df`) |
| SP planificados | 36 (0 acreditados: ninguna historia tiene todos sus CA verificados formalmente) |
| Rama con el último avance | `feature/HU11-tiempo-real-sos` (pendiente de *pull request* a `main`) |

> El calendario del Plan Scrum no coincide con el avance real: el código empezó el 13/10/2026 (discrepancia D13 del Plan de Arquitectura). La replanificación corresponde al Product Owner.

## Objetivo del sprint

Disponer, en el entorno de validación, de registro e inicio de sesión, cuentas institucionales, control de acceso por rol y el primer flujo de SOS (emisión por el ciudadano y recepción por el operador), reduciendo la incertidumbre técnica de la ubicación con SPIKE01.

## Historias planificadas

Estados usados: **Implementada** (el código existe y se probó manualmente), **Parcial** (falta parte de lo planificado), **Pendiente** (no hay código). Ninguna está **Aceptada**: falta verificar sus CA uno a uno en el entorno de validación.

| ID | Historia | SP | Estado | Observación |
|---|---|---|---|---|
| TEC01 | Entorno de validación integrado y publicado | 3 | Parcial | Monorepo, Docker Compose, Dockerfile y `/salud`. **No publicado** en Render ni Neon |
| SPIKE01 | Captura y transmisión de ubicación | — | Pendiente | Sin nota técnica ni pruebas en un dispositivo |
| HU01 | Registrar mi cuenta de ciudadano | 3 | Implementada | API y pantalla en la app; app no probada en un teléfono |
| HU02 | Iniciar sesión | 3 | Implementada | JWT (app) y cookie (panel); cuentas desactivadas rechazadas |
| HU03 | Acceder solo a las funciones de mi rol | 5 | Parcial | Políticas por rol aplicadas; falta la matriz acumulativa de pruebas (CA04) |
| HU04 | Crear, consultar y modificar cuentas institucionales | 3 | Pendiente | Sin código; no hay semilla del administrador (DP02) |
| HU06 | Preparar la geolocalización al abrir la app | 3 | Implementada | Solo compilada; no probada en un dispositivo |
| HU07 | Emitir un SOS con una única activación | 5 | Implementada | API probada; botón de la app no probado en un dispositivo |
| HU08 | Asociar la ubicación actual al SOS | 3 | Implementada | Se acepta junto con HU07; captura real no probada |
| HU11 | Recibir los SOS en la sección urgente | 8 | Implementada | Probada de servidor a cliente; página `/central` no revisada en un navegador |

## Funcionalidades implementadas

- **Cuentas:** registro de ciudadano con validaciones en español y email único, incluso con registros concurrentes. Hash de contraseñas con Identity (PBKDF2-HMAC-SHA512, RNF01).
- **Sesión:** JWT para la app y cookie HttpOnly para el panel, validados contra cuenta activa y sello de seguridad.
- **Ubicación:** comprobación y solicitud del permiso en el modo ciudadano, explicando su finalidad; no transmite posiciones (RNF07).
- **SOS:** `POST /api/sos` solo para ciudadanos, con o sin ubicación, idempotente por `IdSolicitud` (RNF08). Botón con un toque y reintento sin duplicar.
- **Central:** evento `SosNuevo` por SignalR tras el COMMIT, sección urgente con aviso visual y sonoro, mapa Leaflet, resincronización al reconectar y acuse `Presentado`.

## Funcionalidades pendientes

- HU04 completa: alta, consulta y modificación de operadores y serenos, y la semilla del administrador.
- Matriz de autorización endpoint × rol (HU03-CA04).
- SPIKE01: nota técnica y pruebas de ubicación en un dispositivo real.
- Publicación del entorno con HTTPS (TEC01, RNF09).
- Medición formal de RNF02 (100 SOS con 20 sesiones).

## Evidencias disponibles

| Evidencia | Dónde |
|---|---|
| Código de cada historia y descripción de los cambios | Commits `4a97cac` a `3f489df` y ramas `feature/*` en GitHub |
| Migraciones de base de datos | `src/Cad.Server/Persistencia/Migraciones/` |
| Configuración del hash de contraseñas (RNF01) | `src/Cad.Server/Persistencia/ConfiguracionPersistencia.cs` |
| Documentación | `README.md`, `docs/estado-actual.md`, `docs/arquitectura.md`, `docs/instalacion.md` |

No hay capturas, vídeos ni informes de prueba guardados en el repositorio.

## Pruebas existentes

- **Automatizadas:** ninguna. `tests/Cad.Server.Tests` y `tests/Cad.PruebaCarga` están vacíos y no hay CI.
- **Manuales durante el desarrollo (13/10/2026),** contra PostgreSQL local. Los resultados figuran en los mensajes de commit y no en archivos de prueba:
  - Registro e inicio de sesión: 201/409/400/401/403 según el caso; 10 registros simultáneos con el mismo email → 1 cuenta.
  - SOS: 401 sin token, 403 con rol Operador, 201 nuevo, 200 en el reintento; 10 envíos simultáneos con el mismo `IdSolicitud` → 1 caso.
  - Tiempo real: un operador conectado al Hub recibió `SosNuevo` unos 190 ms después del envío, sin aviso repetido en el reintento.
- **Compilación:** servidor y app Android con 0 errores y 0 advertencias.
- **Sin probar:** la app en un teléfono real y el panel `/central` en un navegador.

## Impedimentos

| Impedimento | Efecto | Estado |
|---|---|---|
| Desfase del calendario del Plan Scrum | Las fechas del sprint no reflejan el avance real | Abierto (decisión del PO) |
| HU04 sin implementar | No se pueden crear operadores ni administradores desde el sistema; para probar se cambió el rol directamente en la BD local | Abierto |
| Decisiones pendientes DP01, DP02 y DP04 | Teléfono opcional, administrador inicial y "operador autorizado" resueltos con supuestos | Abierto |
| Sin pruebas en un dispositivo Android | HU06, HU07 y HU08 sin validar en uso real | Abierto |
| Diseño de Figma no accesible (archivo en borradores de otra persona) y decisión DNI o email | La interfaz no sigue el diseño aprobado | Aplazado al cierre del sprint |
| Entorno de desarrollo sin SDK, Docker ni herramientas de Android al inicio | Retrasó la compilación y las pruebas | Resuelto |

## Estado del incremento

- **Implementado e integrado en local:** registro, inicio de sesión, permiso de ubicación, SOS y recepción en la central.
- **No integrado en el entorno de validación:** no hay entorno publicado.
- **No aceptado:** faltan HU04, la verificación formal de los CA, las pruebas por rol, la medición de RNF02 y la nota de SPIKE01.
- **No liberado:** es un MVP académico con datos simulados.

El flujo de punta a punta del Sprint 1 (ciudadano envía un SOS y el operador lo ve con aviso sonoro) funciona en local a nivel de API y tiempo real. Falta demostrarlo con la app en un teléfono y el panel en un navegador sobre el entorno publicado.
