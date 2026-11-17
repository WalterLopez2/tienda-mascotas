namespace TiendaMascotas.Core.Dominio;

public sealed record PedidoProcesado(Guid PedidoId, string Cliente, decimal MontoCobrado, string MetodoPago, DateTime Fecha);
