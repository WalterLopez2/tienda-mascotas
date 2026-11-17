using System.Globalization;
using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Tests.Caracterizacion;

/// <summary>
/// Traduce el formato viejo "nombre;categoria;precio;cantidad|..." al modelo nuevo,
/// para poder alimentar las DOS versiones con exactamente los mismos datos.
/// </summary>
internal static class ConvertidorLegacy
{
    public static Pedido APedido(string items, string? cupon) =>
        new(new Cliente("Camila", "camila@correo.com", EstaActivo: true), items.Split('|').Select(ALinea), cupon);

    private static LineaPedido ALinea(string item)
    {
        var campos = item.Split(';');
        var producto = new Producto(
            nombre: campos[0],
            categoria: Enum.Parse<Categoria>(campos[1]),
            precio: decimal.Parse(campos[2], CultureInfo.InvariantCulture));

        return new LineaPedido(producto, int.Parse(campos[3]));
    }

    public static decimal MontoDelResultadoLegacy(string resultado) =>
        decimal.Parse(resultado["OK:".Length..], CultureInfo.InvariantCulture);
}
