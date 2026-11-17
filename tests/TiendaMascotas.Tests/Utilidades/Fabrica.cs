using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Notificaciones;
using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Core.Persistencia;
using TiendaMascotas.Core.Precios;
using TiendaMascotas.Core.Riesgo;
using TiendaMascotas.Core.Servicios;
using TiendaMascotas.Core.Tiempo;
using TiendaMascotas.Tests.Dobles;
using static TiendaMascotas.Tests.Utilidades.PedidoBuilder;

namespace TiendaMascotas.Tests.Utilidades;

/// <summary>Arma un ServicioPedidos con dobles por defecto; cada prueba reemplaza solo lo que necesita.</summary>
internal static class Fabrica
{
    public static readonly DateTime FechaFija = new(2026, 11, 20, 10, 30, 0);

    public static ServicioPedidos Servicio(
        IPasarelaPago? pasarela = null,
        IRepositorioPedidos? repositorio = null,
        INotificador? notificador = null,
        IReloj? reloj = null,
        ControlDeRiesgo? controlDeRiesgo = null)
    {
        reloj ??= new RelojFijo(FechaFija);

        return new ServicioPedidos(
            new CalculadoraPrecios([new DescuentoCompraGrande(), new DescuentoCupon(reloj)]),
            pasarela ?? new PasarelaPagoMock(),
            repositorio ?? new RepositorioEnMemoria(),
            notificador ?? new NotificadorEspia(),
            reloj,
            controlDeRiesgo ?? new ControlDeRiesgo(montoMaximoPorPedido: 50_000m, montoMaximoAcumulado: 500_000m));
    }

    /// <summary>Pedido de ejemplo: con tarjeta cobra L 1,061.63.</summary>
    public static Pedido PedidoDeCamila(Guid? id = null)
    {
        var builder = UnPedido()
            .Con("Croquetas", Categoria.Alimento, 250m, cantidad: 2)
            .Con("Pelota", Categoria.Juguete, 45m, cantidad: 3)
            .Con("Cama", Categoria.Accesorio, 600m)
            .ConCupon("HUELLITAS10");

        return (id is null ? builder : builder.ConId(id.Value)).Construir();
    }
}
