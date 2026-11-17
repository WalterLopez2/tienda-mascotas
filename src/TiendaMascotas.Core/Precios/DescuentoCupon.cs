using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Tiempo;

namespace TiendaMascotas.Core.Precios;

public sealed class DescuentoCupon : IPoliticaDescuento
{
    private static readonly IReadOnlyDictionary<string, Cupon> CuponesVigentes =
        new Cupon[]
        {
            new("HUELLITAS10", 0.10m),
            new("PRIMERACOMPRA", 0.15m),
            new("BLACKFRIDAY", 0.20m, VenceEl: new DateOnly(2026, 11, 30))
        }.ToDictionary(cupon => cupon.Codigo);

    private readonly IReloj _reloj;

    public DescuentoCupon(IReloj reloj) => _reloj = reloj;

    public decimal Aplicar(decimal monto, Pedido pedido)
    {
        if (!CuponesVigentes.TryGetValue(Normalizar(pedido.Cupon), out var cupon)) return monto;
        if (!cupon.EstaVigenteEl(_reloj.Ahora)) return monto;

        return monto * (1 - cupon.Porcentaje);
    }

    private static string Normalizar(string? cupon) => cupon?.Trim().ToUpperInvariant() ?? string.Empty;
}
