# Declaración de Uso de Inteligencia Artificial

## 1. Herramientas utilizadas

Durante el desarrollo y migración de este proyecto utilicé **ChatGPT / Gemini** como asistente de consulta técnica y apoyo para tareas mecánicas de refactorización.

---

## 2. Qué le pedí en los momentos importantes

- **Guía sobre la estructura del proyecto**: Consulté cómo organizar correctamente las carpetas (`Controllers`, `Dtos`, `Data`, `Services` y `Models`) para pasar de la estructura que tenía en Minimal API al patrón con controladores e interfaces visto en clase (`BackofficeApi`).
- **Trabajo mecánico de conversión a Controladores**: Le pedí ayuda para agilizar la parte repetitiva de transformar las firmas de mis endpoints básicos (`MapGet`, `MapPost`, `MapPut`, `MapDelete`) en métodos dentro de clases que heredan de `ControllerBase` con sus atributos `[HttpGet]`, `[HttpPost]` y respuestas `ActionResult`.
- **Consultas sobre `AppDbContext` y comandos de migraciones**: Le consulté cómo adaptar el `AppDbContext` de referencia para configurar las relaciones `1:1` (`CardPersonaje`) y `1:N` (`Nave`), además de repasar el orden de los comandos de `dotnet user-secrets` y `dotnet ef migrations` para registrar las dos migraciones del proyecto.
- **Formato del archivo README**: Le pedí que me ayudara a dar formato en Markdown a la guía de instalación y configuración del archivo `README.md`.

---

## 3. Qué acepté, qué corregi y por que

- **Lo que acepté**:
  - Acepté el esqueleto base de los controladores CRUD y la separación de los DTOs en archivos independientes porque me ahorró tiempo de escritura manual repetitiva y mantenía los mismos códigos de respuesta HTTP (`200`, `201`, `204`, `400`, `404` y `409`).
  - Acepté el uso de `AesGcm` en `CifradoService` y `PasswordHasher<Nave>` siguiendo la misma implementación vista en el ejemplo de la materia.

- **Lo que cuestioné y corregí**:
  1. **Persistencia de listas de IDs en `Eventos` dentro de `AppDbContext`**: En una primera sugerencia para el `AppDbContext`, las listas `Participantes` y `Muertos` de la entidad `Evento` se intentaban mapear como propiedades simples, lo cual daba error al crear la migración en MySQL. Lo corregí implementando un `ValueConverter` con `JsonSerializer` y un `ValueComparer` en `OnModelCreating`, permitiendo guardar y leer las listas en MySQL sin perder la lógica original de mi proyecto.
  2. **Falla en la validación temporal y año de muerte en `EventoService`**: Al revisar la lógica de actualización de eventos, detecté una referencia incorrecta a la propiedad del año de muerte del personaje (`personaje.Anio` en lugar de `personaje.AnioMuerte`) y problemas cuando se volvía a validar un evento en el que el personaje ya había sido marcado como muerto. Corregí la condición temporal y ajusté el servicio para que actualice el estado del personaje en la base de datos antes de ejecutar `SaveChangesAsync()`.
  3. **Integridad al eliminar personajes con naves y diseño de los endpoints propios**: Cuando se generó el borrado básico en `PersonajeService`, inicialmente solo verificaba si el personaje tenía una carta o participaba en eventos. Lo corregí agregando el estado `TieneNaves` en el enum de eliminación para devolver un `409 Conflict` si el piloto aún tiene naves registradas en su hangar. Asimismo, definí y ajusté la lógica de los tres endpoints de mi componente propio (`despliegue-flota`, `expediente-piloto` y `refuerzos-disponibles`) asegurándome de conservar también el endpoint de simulación del proyecto original.