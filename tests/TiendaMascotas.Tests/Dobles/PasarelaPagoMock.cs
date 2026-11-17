using TiendaMascotas.Core.Pagos;

namespace TiendaMascotas.Tests.Dobles;

/// <summary>
/// MOCK: se programa ANTES con lo que se espera que pase ("espero exactamente un cobro
/// de L 1,061.63 para este pedido") y él mismo VERIFICA al final si se cumplió.
/// Diferencia con el Spy: el Spy solo graba; el Mock trae las expectativas adentro.
/// </summary>
internal sealed class PasarelaPagoMock : IPasarelaPago
{
    private readonly bool _aprobarCobros;
    private readonly List<(Guid PedidoId, decimal Monto)> _cobrosRecibidos = [];
    private (Guid PedidoId, decimal Monto)? _cobroEsperado;

    public PasarelaPagoMock(bool aprobarCobros = true) => _aprobarCobros = aprobarCobros;

    public int CantidadDeCobros => _cobrosRecibidos.Count;

    public PasarelaPagoMock EsperarUnSoloCobro(Guid pedidoId, decimal monto)
    {
        _cobroEsperado = (pedidoId, monto);
        return this;
    }

    public bool Cobrar(Guid pedidoId, decimal monto)
    {
        _cobrosRecibidos.Add((pedidoId, monto));
        return _aprobarCobros;
    }

    public void Verificar()
    {
        if (_cobroEsperado is null)
            throw new InvalidOperationException("El mock no tiene expectativas configuradas.");

        var cobro = Assert.Single(_cobrosRecibidos);
        Assert.Equal(_cobroEsperado.Value, cobro);
    }
}
