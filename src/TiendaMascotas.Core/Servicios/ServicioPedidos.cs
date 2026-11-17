using System.Globalization;
using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Precios;

namespace TiendaMascotas.Core.Servicios;

/// <summary>
/// Versión Clean Code del viejo Procesar(object[] d):
/// cláusulas de guarda en vez de la "flecha" de ifs, y un paso = una función con nombre.
/// Todavía crea sus propias dependencias (archivo, consola): eso lo arregla SOLID.
/// </summary>
public class ServicioPedidos
{
    private const decimal RecargoTarjeta = 0.03m;

    private readonly CalculadoraPrecios _calculadora = new();
    private readonly string _rutaArchivo;

    public List<string> Bitacora { get; } = new();

    public ServicioPedidos(string rutaArchivo = "pedidos.txt") => _rutaArchivo = rutaArchivo;

    public ResultadoPedido Procesar(Pedido pedido, TipoPago tipoPago)
    {
        if (!pedido.Cliente.EstaActivo) return ResultadoPedido.Fallo("Cliente inactivo");
        if (!pedido.TieneLineas) return ResultadoPedido.Fallo("El pedido no tiene productos");

        var total = _calculadora.CalcularTotal(pedido);
        var montoACobrar = Math.Round(AplicarRecargo(total, tipoPago), 2);

        Cobrar(montoACobrar, tipoPago);
        Guardar(pedido, montoACobrar);
        NotificarCliente(pedido.Cliente, montoACobrar);

        return ResultadoPedido.Exito(montoACobrar);
    }

    private static decimal AplicarRecargo(decimal total, TipoPago tipoPago) => tipoPago switch
    {
        TipoPago.Tarjeta => total * (1 + RecargoTarjeta),
        TipoPago.Efectivo => total,
        _ => throw new ArgumentOutOfRangeException(nameof(tipoPago))
    };

    private void Cobrar(decimal monto, TipoPago tipoPago) =>
        Bitacora.Add($"Cobrando {monto.ToString("N2", CultureInfo.InvariantCulture)} con {tipoPago}");

    private void Guardar(Pedido pedido, decimal monto) =>
        File.AppendAllText(_rutaArchivo, $"{pedido.Id},{pedido.Cliente.Nombre},{monto.ToString(CultureInfo.InvariantCulture)},{DateTime.Now:O}\n");

    private void NotificarCliente(Cliente cliente, decimal monto) =>
        Bitacora.Add($"Correo a {cliente.Correo}: su pedido fue procesado. Total: L {monto.ToString("N2", CultureInfo.InvariantCulture)}");
}
