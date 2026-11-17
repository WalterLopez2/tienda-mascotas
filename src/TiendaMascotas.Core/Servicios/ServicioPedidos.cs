using System.Globalization;
using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Notificaciones;
using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Core.Persistencia;
using TiendaMascotas.Core.Precios;

namespace TiendaMascotas.Core.Servicios;

/// <summary>
/// SRP: ya no calcula, ni escribe archivos, ni manda correos. Solo ORQUESTA el flujo.
/// Cada responsabilidad vive en su propia clase y tiene una sola razón para cambiar.
/// </summary>
public class ServicioPedidos
{
    private const decimal RecargoTarjeta = 0.03m;

    private readonly CalculadoraPrecios _calculadora = new();
    private readonly PasarelaPagoSimulada _pasarela = new();
    private readonly NotificadorConsola _notificador = new();
    private readonly RepositorioPedidosArchivo _repositorio;

    public ServicioPedidos(string rutaArchivo = "pedidos.jsonl") =>
        _repositorio = new RepositorioPedidosArchivo(rutaArchivo);

    public ResultadoPedido Procesar(Pedido pedido, TipoPago tipoPago)
    {
        if (!pedido.Cliente.EstaActivo) return ResultadoPedido.Fallo("Cliente inactivo");
        if (!pedido.TieneLineas) return ResultadoPedido.Fallo("El pedido no tiene productos");

        var total = _calculadora.CalcularTotal(pedido);
        var montoACobrar = Math.Round(AplicarRecargo(total, tipoPago), 2);

        if (!_pasarela.Cobrar(pedido.Id, montoACobrar)) return ResultadoPedido.Fallo("El pago fue rechazado");

        _repositorio.Guardar(new PedidoProcesado(pedido.Id, pedido.Cliente.Nombre, montoACobrar, tipoPago.ToString(), DateTime.Now));
        _notificador.Notificar(pedido.Cliente, $"Su pedido fue procesado. Total: L {montoACobrar.ToString("N2", CultureInfo.InvariantCulture)}");

        return ResultadoPedido.Exito(montoACobrar);
    }

    private static decimal AplicarRecargo(decimal total, TipoPago tipoPago) => tipoPago switch
    {
        TipoPago.Tarjeta => total * (1 + RecargoTarjeta),
        TipoPago.Efectivo => total,
        _ => throw new ArgumentOutOfRangeException(nameof(tipoPago))
    };
}
