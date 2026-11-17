using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Notificaciones;
using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Core.Persistencia;
using TiendaMascotas.Core.Precios;
using TiendaMascotas.Core.Riesgo;
using TiendaMascotas.Core.Servicios;
using TiendaMascotas.Core.Tiempo;
using ProcesadorLegacy = TiendaMascotas.Legacy.ProcesadorPedidos;

const string ItemsLegacy = "Croquetas;Alimento;250;2|Pelota;Juguete;45;3|Cama;Accesorio;600;1";

Titulo("ANTES: legacy (ProcesadorPedidos.Procesar con object[])");
var legacy = new ProcesadorLegacy();
var datos = new object[] { "Camila", true, ItemsLegacy, "tarjeta", "HUELLITAS10", "camila@correo.com" };
Console.WriteLine($"  Resultado: {legacy.Procesar(datos)}");
Console.WriteLine($"  ¿Y si proceso el MISMO pedido otra vez?  {legacy.Procesar(datos)}  <- lo cobra dos veces");

Titulo("DESPUÉS: Clean Code + SOLID");

// Composition root: el ÚNICO lugar que conoce las clases concretas (DIP).
var reloj = new RelojSistema();
var servicio = new ServicioPedidos(
    new CalculadoraPrecios([new DescuentoCompraGrande(), new DescuentoCupon(reloj)]),
    new PasarelaPagoSimulada(),
    new RepositorioPedidosArchivo("pedidos.jsonl"),
    new NotificadorConsola(),
    reloj,
    new ControlDeRiesgo(montoMaximoPorPedido: 50_000m, montoMaximoAcumulado: 500_000m));

var camila = new Cliente("Camila", "camila@correo.com", EstaActivo: true);
var pedido = new Pedido(camila,
[
    new LineaPedido(new Producto("Croquetas", Categoria.Alimento, 250m), 2),
    new LineaPedido(new Producto("Pelota", Categoria.Juguete, 45m), 3),
    new LineaPedido(new Producto("Cama", Categoria.Accesorio, 600m), 1)
], cupon: "HUELLITAS10");

Mostrar(servicio.Procesar(pedido, new PagoTarjeta()));

Titulo("Lección Knight Capital: el mismo pedido otra vez");
Mostrar(servicio.Procesar(pedido, new PagoTarjeta()));

static void Titulo(string texto)
{
    Console.WriteLine();
    Console.WriteLine($"=== {texto} ===");
}

static void Mostrar(ResultadoPedido resultado) =>
    Console.WriteLine(resultado.Exitoso
        ? $"  Resultado: {resultado.Mensaje} - L {resultado.MontoCobrado:N2}"
        : $"  Resultado: RECHAZADO - {resultado.Mensaje}");
