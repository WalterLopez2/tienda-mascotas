namespace TiendaMascotas.Core.Dominio;

/// <summary>
/// Antes se devolvía "OK:1061.63" o "ERROR" y quien llamaba tenía que partir el texto.
/// </summary>
public sealed record ResultadoPedido(bool Exitoso, decimal MontoCobrado, string Mensaje)
{
    public static ResultadoPedido Exito(decimal montoCobrado) =>
        new(true, montoCobrado, "Pedido procesado");

    public static ResultadoPedido Fallo(string motivo) =>
        new(false, 0m, motivo);
}
