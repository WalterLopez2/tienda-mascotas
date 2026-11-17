using System.Globalization;

namespace TiendaMascotas.Core.Pagos;

/// <summary>Única responsabilidad: hablar con el procesador de pagos (aquí, simulado).</summary>
public sealed class PasarelaPagoSimulada : IPasarelaPago
{
    public bool Cobrar(Guid pedidoId, decimal monto)
    {
        Console.WriteLine($"  [pasarela] cobrando L {monto.ToString("N2", CultureInfo.InvariantCulture)} del pedido {pedidoId.ToString()[..8]}");
        return true;
    }
}
