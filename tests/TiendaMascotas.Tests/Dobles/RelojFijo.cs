using TiendaMascotas.Core.Tiempo;

namespace TiendaMascotas.Tests.Dobles;

internal sealed class RelojFijo(DateTime ahora) : IReloj
{
    public DateTime Ahora { get; } = ahora;
}
