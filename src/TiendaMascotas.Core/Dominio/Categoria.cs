namespace TiendaMascotas.Core.Dominio;

/// <summary>
/// Antes era un string ("Alimento", "Juguete"...): un typo compilaba y fallaba en silencio.
/// Como enum, el compilador lo atrapa.
/// </summary>
public enum Categoria
{
    Alimento,
    Juguete,
    Accesorio,
    Medicina
}
