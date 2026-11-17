namespace TiendaMascotas.Core.Tiempo;

/// <summary>Hora actual detrás de una interfaz para poder fijarla en las pruebas.</summary>
public interface IReloj
{
    DateTime Ahora { get; }
}

public sealed class RelojSistema : IReloj
{
    public DateTime Ahora => DateTime.Now;
}
