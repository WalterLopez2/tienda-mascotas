using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Precios;

/// <summary>Promoción que recibe el monto acumulado y lo devuelve con su descuento aplicado.</summary>
public interface IPoliticaDescuento
{
    decimal Aplicar(decimal monto, Pedido pedido);
}
