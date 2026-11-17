using System.Globalization;
using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Notificaciones;
using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Core.Persistencia;
using TiendaMascotas.Core.Precios;
using TiendaMascotas.Core.Tiempo;

namespace TiendaMascotas.Core.Servicios;

/// <summary>
/// SRP: solo ORQUESTA el flujo de un pedido; no calcula, no guarda, no notifica por sí mismo.
/// DIP: depende de abstracciones que recibe por constructor, no de clases concretas que
/// crea con "new". Por eso en producción usa archivo + consola y en las pruebas, dobles.
/// </summary>
public class ServicioPedidos
{
    private readonly CalculadoraPrecios _calculadora;
    private readonly IPasarelaPago _pasarela;
    private readonly IRepositorioPedidos _repositorio;
    private readonly INotificador _notificador;
    private readonly IReloj _reloj;

    public ServicioPedidos(
        CalculadoraPrecios calculadora,
        IPasarelaPago pasarela,
        IRepositorioPedidos repositorio,
        INotificador notificador,
        IReloj reloj)
    {
        _calculadora = calculadora;
        _pasarela = pasarela;
        _repositorio = repositorio;
        _notificador = notificador;
        _reloj = reloj;
    }

    public ResultadoPedido Procesar(Pedido pedido, IMetodoPago metodoPago)
    {
        if (!pedido.Cliente.EstaActivo) return ResultadoPedido.Fallo("Cliente inactivo");
        if (!pedido.TieneLineas) return ResultadoPedido.Fallo("El pedido no tiene productos");

        var montoACobrar = CalcularMontoACobrar(pedido, metodoPago);

        if (!_pasarela.Cobrar(pedido.Id, montoACobrar)) return ResultadoPedido.Fallo("El pago fue rechazado");

        RegistrarPedido(pedido, metodoPago, montoACobrar);
        NotificarCliente(pedido.Cliente, montoACobrar);

        return ResultadoPedido.Exito(montoACobrar);
    }

    private decimal CalcularMontoACobrar(Pedido pedido, IMetodoPago metodoPago)
    {
        var total = _calculadora.CalcularTotal(pedido);
        return Math.Round(metodoPago.CalcularMontoACobrar(total), 2);
    }

    private void RegistrarPedido(Pedido pedido, IMetodoPago metodoPago, decimal montoCobrado) =>
        _repositorio.Guardar(new PedidoProcesado(pedido.Id, pedido.Cliente.Nombre, montoCobrado, metodoPago.Nombre, _reloj.Ahora));

    private void NotificarCliente(Cliente cliente, decimal montoCobrado) =>
        _notificador.Notificar(cliente, $"Su pedido fue procesado. Total: L {montoCobrado.ToString("N2", CultureInfo.InvariantCulture)}");
}
