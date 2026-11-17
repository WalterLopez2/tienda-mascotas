namespace TiendaMascotas.Core.Pagos;

/// <summary>
/// ISP + LSP: en el legacy, MetodoPago obligaba a TODOS a implementar Reembolsar(),
/// y PagoEfectivo lanzaba NotImplementedException (rompía a quien lo usara como MetodoPago).
/// Ahora reembolsar es una capacidad aparte: solo la implementa quien realmente puede.
/// </summary>
public interface IReembolsable
{
    decimal CalcularReembolso(decimal montoCobrado);
}
