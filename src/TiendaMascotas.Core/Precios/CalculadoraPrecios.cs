using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Precios;

/// <summary>
/// Calcula el total de un pedido: subtotal con descuento por categoría,
/// luego las políticas de descuento EN ORDEN, y al final el ISV.
/// </summary>
public class CalculadoraPrecios
{
    private const decimal TasaIsv = 0.15m;

    // Método de tabla: agregar una categoría = agregar una fila, no otro else-if.
    // Medicina no tiene descuento: es la regla vigente del negocio (ver pruebas de caracterización).
    private static readonly IReadOnlyDictionary<Categoria, decimal> DescuentoPorCategoria =
        new Dictionary<Categoria, decimal>
        {
            [Categoria.Alimento] = 0.05m,
            [Categoria.Juguete] = 0.10m,
            [Categoria.Accesorio] = 0.15m
        };

    private readonly IReadOnlyList<IPoliticaDescuento> _politicas;

    public CalculadoraPrecios(IEnumerable<IPoliticaDescuento> politicas) => _politicas = politicas.ToList();

    public decimal CalcularTotal(Pedido pedido)
    {
        var subtotal = CalcularSubtotal(pedido);
        var conDescuentos = _politicas.Aggregate(subtotal, (monto, politica) => politica.Aplicar(monto, pedido));
        return conDescuentos * (1 + TasaIsv);
    }

    public decimal CalcularSubtotal(Pedido pedido) =>
        pedido.Lineas.Sum(ImporteConDescuentoDeCategoria);

    private static decimal ImporteConDescuentoDeCategoria(LineaPedido linea)
    {
        var descuento = DescuentoPorCategoria.GetValueOrDefault(linea.Producto.Categoria, 0m);
        return linea.Importe * (1 - descuento);
    }
}
