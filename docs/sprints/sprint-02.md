# Sprint 2 · Verificación, despacho y ciclo de atención del SOS

Documento de planificación inicial. **No hay código de este sprint todavía.**

| Dato | Valor |
|---|---|
| Fechas en el Plan Scrum | 17/09/2026 – 26/09/2026 (referencia del plan; el calendario está desfasado, ver [sprint-01.md](sprint-01.md)) |
| SP planificados | 36 (Plan Scrum, sección 3.2) |
| Capacidad del equipo | No informada; debe revalidarse en el Sprint Planning |

## Objetivo del sprint

Completar el ciclo del SOS: verificación por el operador, estado y ubicación de los serenos, asignación, desplazamiento, inicio de atención y cierre, que libera la disponibilidad del sereno (Plan Scrum, objetivo propuesto).

## Historias previstas

| ID | Historia | SP | RF | Dependencias (Plan Scrum) |
|---|---|---|---|---|
| HU12 | Contactar al ciudadano y registrar la verificación del SOS | 3 | RF08 | I: HU07, HU03 · A: HU20-A (CA06) |
| HU18 | Activar mi estado operativo y compartir mi ubicación | 5 | RF06 | I: HU02, HU03 · P: sereno creado (HU04) · R: SPIKE01 |
| HU19 | Monitorear el estado y la disponibilidad de los serenos | 3 | RF07 | I: HU18 · R: HU20-A, HU24 |
| HU20-A | Asignar un SOS pendiente a un sereno disponible | 8 | RF09 | I: HU07, HU19, HU03 · A: HU12 (CA06) |
| HU13 | Clasificar un SOS después de su registro | 3 | RF16.2 | I: HU07, HU03 · R: catálogo compartido (DP07) |
| HU21 | Recibir y consultar un despacho | 3 | RF10.1 | I: HU20-A |
| HU22 | Confirmar mi desplazamiento al caso | 3 | RF10.2 | I: HU21 |
| HU23 | Registrar el inicio de atención | 3 | RF11.1 | I: HU22 |
| HU24 | Cerrar un caso con su tipificación y resultado | 5 | RF11.2 | I: HU23 · R: HU13 (catálogo), HU19 (CA07) |

I = implementación · A = aceptación · P = precondición de uso · R = relación funcional.

## Dependencias con el Sprint 1

| Necesidad del Sprint 2 | Estado en el Sprint 1 | Efecto |
|---|---|---|
| Cuentas de sereno y operador (HU04) | **Pendiente** | Bloquea HU18–HU24 en uso real; hoy solo se crean cambiando el rol en la BD |
| SOS en "Pendiente" (HU07/HU08) | Implementado | Base de HU12, HU13 y HU20-A |
| Control de acceso por rol (HU03) | Parcial | Cada endpoint nuevo debe sumarse a la matriz endpoint × rol |
| Canal de tiempo real (HU11) | Implementado para el panel | Falta autenticar la app (JWT) en el Hub para notificar al sereno (HU21) |
| SPIKE01 (ubicación en el dispositivo) | Pendiente | Condiciona HU18 y su posible reestimación |
| Token de concurrencia `xmin` en `casos` | Implementado | Base para las transiciones concurrentes (HU20-A, HU22–HU24) |

**Recomendación:** cerrar HU04 (y la semilla del administrador, DP02) antes o al inicio del Sprint 2.

## Orden recomendado de implementación

Sigue la cadena de dependencias del Plan Scrum (tareas S2-T01 a S2-T17):

1. **HU12:** registro de intentos de contacto y verificación (solo panel; no depende de los serenos).
2. **HU18:** estado operativo y transmisión periódica de la posición del sereno.
3. **HU19:** monitoreo de estado y disponibilidad (operativo y sin asignación vigente), actualizado por tiempo real.
4. **HU20-A:** asignación con protección frente a concurrencia, y verificación del vínculo con HU12 (CA06).
5. **HU13:** catálogo de categorías y clasificación posterior, sin bloquear la emisión ni la asignación.
6. **HU21 → HU22 → HU23 → HU24:** despacho al sereno, desplazamiento, inicio de atención y cierre con liberación de la disponibilidad.

HU13 va antes de HU24 porque el cierre usa el catálogo de tipificación.

## Riesgos y decisiones pendientes

| DP / riesgo | Afecta a | Tratamiento propuesto en el plan |
|---|---|---|
| DP01 Obligatoriedad del teléfono | HU12 | Ya configurable (`Cuentas:TelefonoObligatorio`) |
| DP05 Visibilidad para el ciudadano | HU24 (resultado público) | Mapeo único de estado interno → público |
| DP06 Datos del caso visibles para el sereno | HU21 | DTO mínimo del sereno; se amplía al decidir |
| DP07 Catálogo de categorías | HU13, HU24 | Tabla con semilla; contenido mínimo de RF16.2 |
| DP09 Quién clasifica un SOS | HU13 | Política `ClasificarSos`, al inicio igual al Operador |
| DP10 Referencia textual en el mapa | HU12 | Se guarda separada de las coordenadas, sin marcador |
| DP15 Intervalo de posición del sereno | HU18 | Parámetro configurable, sin valor de negocio fijado |
| DP16 Sereno que se desactiva con asignación vigente | HU18 | Transición no implementada hasta decidir |
| Concurrencia en asignación y transiciones | HU20-A, HU22–HU24 | `xmin` e índices únicos parciales; conflicto → 409 |
| SignalR en MAUI y reconexión (SP-B) | HU21 | Validar pronto en un dispositivo |
| Mapa en la app (Mapsui, SP-C) | HU21 (ubicación del caso) | Sin ruteo (D4); solo el punto del caso |
| Sin pruebas en un dispositivo Android | HU18, HU21–HU24 | Necesario para aceptar las historias del sereno |

## Tareas técnicas resumidas

- **Dominio Casos:** transiciones `Asignar`, `ConfirmarDesplazamiento`, `IniciarAtencion` y `Cerrar` con validación del estado de origen. Separación entre resultado público y notas internas.
- **Despacho:** entidad `Asignacion` con índices únicos parciales (vigente por caso y por sereno). Uso de `EstadoSereno` (operativo y última posición). Disponibilidad calculada, no almacenada.
- **Verificación:** registro de intentos de contacto en el historial (`EventoCaso`), sin sobrescribir registros previos.
- **Catálogo:** tabla de categorías con semilla y "Tipo por determinar".
- **Endpoints:** según la tabla 16 del plan (`/api/sereno/*` y `/api/casos/{id}/contactos`, `/clasificacion`, `/asignacion`, `/desplazamiento`, `/inicio-atencion` y `/cierre`), con política de rol y titularidad.
- **Tiempo real:** eventos `CasoActualizado`, `DisponibilidadSereno` y `DespachoAsignado`; JWT de la app en el Hub y grupo por sereno.
- **Panel:** verificación, clasificación, monitoreo de serenos en el mapa y asignación.
- **App (modo sereno):** estado operativo, transmisión de la posición, despacho y botones de desplazamiento, atención y cierre.
- **Persistencia:** nuevas migraciones para las tablas y restricciones anteriores.

## Pruebas necesarias

- **Concurrencia (RNF08):** dos asignaciones simultáneas del mismo caso o del mismo sereno → una gana y la otra recibe 409, sin registros parciales. Lo mismo en cierres concurrentes.
- **Idempotencia:** repetir una transición con el mismo actor no crea otro evento (HU22).
- **Autorización y titularidad:** solo el sereno asignado confirma el desplazamiento; un operador no puede (HU22-CA04). Ampliar la matriz endpoint × rol de HU03.
- **Visibilidad (RNF12):** el ciudadano no accede a las notas internas; el sereno solo ve los datos del DTO de DP06.
- **Disponibilidad:** al cerrar, el sereno vuelve a figurar disponible y HU19 lo refleja sin recargar.
- **Vínculo HU12 ↔ HU20-A (CA06):** verificación previa y condición "puede asignarse".
- **Tiempo real:** el sereno recibe el despacho; al reconectar consulta la asignación vigente.
- **Dispositivo:** transmisión de la posición del sereno en primer plano en un Android real.
- **RNF02:** repetir la medición con el flujo ampliado (repetición propuesta en el plan, S2-T15).
