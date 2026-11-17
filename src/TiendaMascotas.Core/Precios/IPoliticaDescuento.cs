using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Precios;

/// <summary>
/// OCP: la calculadora está CERRADA a modificación y ABIERTA a extensión.
/// Una promoción nueva = una clase nueva que implementa esta interfaz.
/// </summary>
public interface IPoliticaDescuento
{
    decimal Aplicar(decimal monto, Pedido pedido);
}
