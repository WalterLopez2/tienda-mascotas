using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Precios;

public sealed class DescuentoCupon : IPoliticaDescuento
{
    public decimal Aplicar(decimal monto, Pedido pedido)
    {
        var cupon = pedido.Cupon?.Trim().ToUpperInvariant();
        if (cupon == "HUELLITAS10") return monto * (1 - 0.10m);
        if (cupon == "PRIMERACOMPRA") return monto * (1 - 0.15m);
        return monto;
    }
}
