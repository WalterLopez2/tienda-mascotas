namespace TiendaMascotas.Core.Dominio;

public sealed record ResultadoPedido(bool Exitoso, decimal MontoCobrado, string Mensaje)
{
    public static ResultadoPedido Exito(decimal montoCobrado) =>
        new(true, montoCobrado, "Pedido procesado");

    public static ResultadoPedido Fallo(string motivo) =>
        new(false, 0m, motivo);
}
