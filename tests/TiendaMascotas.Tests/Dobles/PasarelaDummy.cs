using TiendaMascotas.Core.Pagos;

namespace TiendaMascotas.Tests.Dobles;

internal sealed class PasarelaDummy : IPasarelaPago
{
    public bool Cobrar(Guid pedidoId, decimal monto) =>
        throw new InvalidOperationException("La pasarela no debía usarse en este escenario.");
}
