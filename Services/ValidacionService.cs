namespace ApiCatalogo_Galactico.Services;

public static class ValidacionService
{
    public static readonly string[] Facciones = ["Rebelde", "Imperio", "Neutral"];
    public static readonly string[] Estados = ["vivo", "muerto", "desconocido"];

    public static string? ValidarPersonaje(
        string nombre,
        string especie,
        string faccion,
        string afiliacion,
        string estado,
        int? anioMuerte)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "El nombre es obligatorio.";

        if (string.IsNullOrWhiteSpace(especie))
            return "La especie es obligatoria.";

        if (!Facciones.Contains(faccion))
            return "La facción debe ser Rebelde, Imperio o Neutral.";

        if (string.IsNullOrWhiteSpace(afiliacion))
            return "La afiliación es obligatoria.";

        if (!Estados.Contains(estado))
            return "El estado debe ser vivo, muerto o desconocido.";

        if (estado == "muerto" && !anioMuerte.HasValue)
            return "Si el personaje está muerto, debe indicarse el año de muerte.";

        if (estado != "muerto" && anioMuerte.HasValue)
            return "Solo un personaje con estado 'muerto' puede tener año de muerte.";

        return null;
    }

    public static string? ValidarCard(
        int personajeId,
        int poder,
        string habilidadEspecial,
        string arma,
        int nivelPeligrosidad,
        string imagenUrl)
    {
        if (personajeId <= 0)
            return "El PersonajeId no es válido.";

        if (poder < 1 || poder > 100)
            return "El poder debe estar entre 1 y 100.";

        if (string.IsNullOrWhiteSpace(habilidadEspecial))
            return "La habilidad especial es obligatoria.";

        if (string.IsNullOrWhiteSpace(arma))
            return "El arma es obligatoria.";

        if (nivelPeligrosidad < 1 || nivelPeligrosidad > 10)
            return "El nivel de peligrosidad debe estar entre 1 y 10.";

        if (string.IsNullOrWhiteSpace(imagenUrl))
            return "La URL de imagen es obligatoria.";

        return null;
    }

    public static string? ValidarEvento(
        string nombre,
        string ubicacion,
        string descripcion,
        int anio)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "El nombre del evento es obligatorio.";

        if (string.IsNullOrWhiteSpace(ubicacion))
            return "La ubicación es obligatoria.";

        if (string.IsNullOrWhiteSpace(descripcion))
            return "La descripción es obligatoria.";

        if (anio < -5000 || anio > 5000)
            return "El año del evento debe estar entre -5000 y 5000.";

        return null;
    }
}