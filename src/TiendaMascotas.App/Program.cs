using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Notificaciones;
using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Core.Persistencia;
using TiendaMascotas.Core.Precios;
using TiendaMascotas.Core.Servicios;
using TiendaMascotas.Core.Tiempo;
using ProcesadorLegacy = TiendaMascotas.Legacy.ProcesadorPedidos;

Console.WriteLine("=== ANTES: legacy ===");
var legacy = new ProcesadorLegacy();
var resultadoLegacy = legacy.Procesar(new object[]
{
    "Camila", true, "Croquetas;Alimento;250;2|Pelota;Juguete;45;3|Cama;Accesorio;600;1",
    "tarjeta", "HUELLITAS10", "camila@correo.com"
});
Console.WriteLine($"  Resultado: {resultadoLegacy}");

Console.WriteLine();
Console.WriteLine("=== DESPUÉS: Clean Code + SOLID ===");

// Composition root: el ÚNICO lugar que conoce las clases concretas (DIP).
var reloj = new RelojSistema();
var servicio = new ServicioPedidos(
    new CalculadoraPrecios([new DescuentoCompraGrande(), new DescuentoCupon()]),
    new PasarelaPagoSimulada(),
    new RepositorioPedidosArchivo("pedidos.jsonl"),
    new NotificadorConsola(),
    reloj);

var camila = new Cliente("Camila", "camila@correo.com", EstaActivo: true);
var pedido = new Pedido(camila,
[
    new LineaPedido(new Producto("Croquetas", Categoria.Alimento, 250m), 2),
    new LineaPedido(new Producto("Pelota", Categoria.Juguete, 45m), 3),
    new LineaPedido(new Producto("Cama", Categoria.Accesorio, 600m), 1)
], cupon: "HUELLITAS10");

var resultado = servicio.Procesar(pedido, new PagoTarjeta());
Console.WriteLine($"  Resultado: {resultado.Mensaje} - L {resultado.MontoCobrado:N2}");
