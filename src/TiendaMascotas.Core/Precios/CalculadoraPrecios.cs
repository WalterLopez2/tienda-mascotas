using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Precios;

/// <summary>
/// Extraída del método Procesar() del legacy. Cada número mágico ahora tiene nombre
/// y la cadena de if/else de categorías pasó a ser una tabla.
/// </summary>
public class CalculadoraPrecios
{
    private const decimal MontoMinimoCompraGrande = 1000m;
    private const decimal DescuentoCompraGrande = 0.10m;
    private const string CuponHuellitas = "HUELLITAS10";
    private const decimal DescuentoCuponHuellitas = 0.10m;
    private const decimal TasaIsv = 0.15m;

    // Medicina no tiene descuento: es la regla vigente del negocio (ver pruebas de caracterización).
    private static readonly IReadOnlyDictionary<Categoria, decimal> DescuentoPorCategoria =
        new Dictionary<Categoria, decimal>
        {
            [Categoria.Alimento] = 0.05m,
            [Categoria.Juguete] = 0.10m,
            [Categoria.Accesorio] = 0.15m
        };

    public decimal CalcularTotal(Pedido pedido)
    {
        var subtotal = CalcularSubtotal(pedido);
        var conDescuentoCompraGrande = AplicarDescuentoCompraGrande(subtotal);
        var conCupon = AplicarCupon(conDescuentoCompraGrande, pedido.Cupon);
        return AgregarIsv(conCupon);
    }

    public decimal CalcularSubtotal(Pedido pedido) =>
        pedido.Lineas.Sum(ImporteConDescuentoDeCategoria);

    private static decimal ImporteConDescuentoDeCategoria(LineaPedido linea)
    {
        var descuento = DescuentoPorCategoria.GetValueOrDefault(linea.Producto.Categoria, 0m);
        return linea.Importe * (1 - descuento);
    }

    private static decimal AplicarDescuentoCompraGrande(decimal monto) =>
        monto > MontoMinimoCompraGrande ? monto * (1 - DescuentoCompraGrande) : monto;

    private static decimal AplicarCupon(decimal monto, string? cupon) =>
        cupon == CuponHuellitas ? monto * (1 - DescuentoCuponHuellitas) : monto;

    private static decimal AgregarIsv(decimal monto) => monto * (1 + TasaIsv);
}
