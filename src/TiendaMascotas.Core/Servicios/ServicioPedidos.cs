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
    private readonly CalculadoraPrecios _calculadora = new([new DescuentoCompraGrande(), new DescuentoCupon()]);
    private readonly PasarelaPagoSimulada _pasarela = new();
    private readonly NotificadorConsola _notificador = new();
    private readonly RepositorioPedidosArchivo _repositorio;

    public ServicioPedidos(string rutaArchivo = "pedidos.jsonl") =>
        _repositorio = new RepositorioPedidosArchivo(rutaArchivo);

    public ResultadoPedido Procesar(Pedido pedido, IMetodoPago metodoPago)
    {
        if (!pedido.Cliente.EstaActivo) return ResultadoPedido.Fallo("Cliente inactivo");
        if (!pedido.TieneLineas) return ResultadoPedido.Fallo("El pedido no tiene productos");

        var total = _calculadora.CalcularTotal(pedido);
        var montoACobrar = Math.Round(metodoPago.CalcularMontoACobrar(total), 2);

        if (!_pasarela.Cobrar(pedido.Id, montoACobrar)) return ResultadoPedido.Fallo("El pago fue rechazado");

        _repositorio.Guardar(new PedidoProcesado(pedido.Id, pedido.Cliente.Nombre, montoACobrar, metodoPago.Nombre, DateTime.Now));
        _notificador.Notificar(pedido.Cliente, $"Su pedido fue procesado. Total: L {montoACobrar.ToString("N2", CultureInfo.InvariantCulture)}");

        return ResultadoPedido.Exito(montoACobrar);
    }

}
