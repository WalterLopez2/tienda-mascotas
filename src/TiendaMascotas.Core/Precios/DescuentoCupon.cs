using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Precios;

public sealed class DescuentoCupon : IPoliticaDescuento
{
    private const string CuponHuellitas = "HUELLITAS10";
    private const decimal Porcentaje = 0.10m;

    public decimal Aplicar(decimal monto, Pedido pedido) =>
        pedido.Cupon?.Trim().ToUpperInvariant() == CuponHuellitas ? monto * (1 - Porcentaje) : monto;
}
