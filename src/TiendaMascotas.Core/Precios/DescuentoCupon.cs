using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Precios;

public sealed class DescuentoCupon : IPoliticaDescuento
{
    // Método de tabla: un cupón nuevo es una fila nueva, no otro if.
    private static readonly IReadOnlyDictionary<string, decimal> PorcentajePorCupon =
        new Dictionary<string, decimal>
        {
            ["HUELLITAS10"] = 0.10m,
            ["PRIMERACOMPRA"] = 0.15m,
            ["BLACKFRIDAY"] = 0.20m
        };

    public decimal Aplicar(decimal monto, Pedido pedido)
    {
        var codigo = Normalizar(pedido.Cupon);
        return PorcentajePorCupon.TryGetValue(codigo, out var porcentaje)
            ? monto * (1 - porcentaje)
            : monto;
    }

    private static string Normalizar(string? cupon) => cupon?.Trim().ToUpperInvariant() ?? string.Empty;
}
