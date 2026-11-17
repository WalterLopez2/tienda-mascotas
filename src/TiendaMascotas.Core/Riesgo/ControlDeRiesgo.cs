namespace TiendaMascotas.Core.Riesgo;

/// <summary>
/// Lección de Knight Capital (1 de agosto de 2012): un sistema de órdenes con código
/// viejo reactivado por error envió órdenes sin parar durante 45 minutos y la empresa
/// perdió unos 440 millones de dólares. No había un límite automático que lo frenara.
///
/// Este "kill switch" es ese límite: ningún pedido individual puede pasar de un monto
/// y, si el total cobrado se dispara, el sistema se DETIENE solo y deja de cobrar.
/// </summary>
public sealed class ControlDeRiesgo
{
    private decimal _montoAcumulado;

    public decimal MontoMaximoPorPedido { get; }
    public decimal MontoMaximoAcumulado { get; }
    public bool EstaDetenido { get; private set; }

    public ControlDeRiesgo(decimal montoMaximoPorPedido, decimal montoMaximoAcumulado)
    {
        if (montoMaximoPorPedido <= 0 || montoMaximoAcumulado <= 0)
            throw new ArgumentOutOfRangeException(nameof(montoMaximoPorPedido), "Los límites deben ser positivos.");

        MontoMaximoPorPedido = montoMaximoPorPedido;
        MontoMaximoAcumulado = montoMaximoAcumulado;
    }

    public bool Autoriza(decimal monto)
    {
        if (EstaDetenido) return false;
        if (monto > MontoMaximoPorPedido) return false;
        if (_montoAcumulado + monto <= MontoMaximoAcumulado) return true;

        EstaDetenido = true;
        return false;
    }

    public void RegistrarCobro(decimal monto) => _montoAcumulado += monto;
}
