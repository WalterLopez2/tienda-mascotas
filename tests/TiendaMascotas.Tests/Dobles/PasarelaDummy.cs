using TiendaMascotas.Core.Pagos;

namespace TiendaMascotas.Tests.Dobles;

/// <summary>
/// DUMMY: se pasa solo porque el constructor lo exige. La prueba asume que NUNCA se usa;
/// si alguien lo llama, la prueba revienta y nos avisa que el flujo hizo algo inesperado.
/// </summary>
internal sealed class PasarelaDummy : IPasarelaPago
{
    public bool Cobrar(Guid pedidoId, decimal monto) =>
        throw new InvalidOperationException("La pasarela no debía usarse en este escenario.");
}
