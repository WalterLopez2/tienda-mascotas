using TiendaMascotas.Core.Tiempo;

namespace TiendaMascotas.Tests.Dobles;

/// <summary>
/// STUB: devuelve una respuesta preparada de antemano. No verifica nada;
/// solo "congela" el tiempo para que la prueba no dependa del día en que corre.
/// </summary>
internal sealed class RelojFijo(DateTime ahora) : IReloj
{
    public DateTime Ahora { get; } = ahora;
}
