## Historias y requisitos

<!-- Ejemplo: HU07, HU08 · RF03.1, RF03.2 · RNF08 -->
- HU:
- RF / RNF:
- Decisiones pendientes (DP) afectadas o supuestos tomados:

## Cambios

<!-- Qué se agrega o cambia, por proyecto: Cad.Server, Cad.Contratos, Cad.Movil. -->
-

## Pruebas

<!-- Qué se probó y cómo. Indica también lo que NO se probó. -->
- [ ] Compila sin errores ni advertencias (`dotnet build src/Cad.Server`, y la app si cambió)
- [ ] Pruebas automatizadas (`tests/`):
- [ ] Prueba manual contra PostgreSQL local:
- [ ] Probado en un dispositivo Android / en el navegador (Chrome, Firefox):
- No probado:

## Evidencias

<!-- Capturas, respuestas HTTP, consultas SQL o registros. Sin contraseñas, tokens ni coordenadas reales. -->

## Comprobaciones

- [ ] La rama parte de `main` actualizada y sigue `feature/HUxx-descripcion`
- [ ] Sin secretos ni `.env` en el cambio
- [ ] Migración de EF Core incluida si cambió el modelo
- [ ] Documentación actualizada si cambió el comportamiento (`README.md`, `docs/`)
