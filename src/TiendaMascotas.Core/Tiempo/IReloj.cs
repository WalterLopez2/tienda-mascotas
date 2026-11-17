namespace TiendaMascotas.Core.Tiempo;

/// <summary>
/// DateTime.Now escondido dentro de un método lo vuelve imposible de probar.
/// Detrás de una interfaz, una prueba puede "congelar" el tiempo.
/// </summary>
public interface IReloj
{
    DateTime Ahora { get; }
}

public sealed class RelojSistema : IReloj
{
    public DateTime Ahora => DateTime.Now;
}
