namespace TiendaMascotas.Core.Riesgo;

/// <summary>Límite automático de cobros: rechaza montos mayores al máximo por pedido y detiene todos los cobros si el acumulado supera el máximo permitido.</summary>
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
