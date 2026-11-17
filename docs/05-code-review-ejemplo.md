# Ejemplo de code review con criterio

PR revisado: **`feature/pago-transferencia` → `main`** (agrega el pago por transferencia bancaria
con una comisión fija).

## Primera versión del PR (commit `feat(pagos): pago por transferencia bancaria`)

```csharp
// clase de pago por transferencia
public sealed class PagoTransferencia : IMetodoPago
{
    public string Nombre => "Transferencia";
    public decimal CalcularMontoACobrar(decimal total) => total + 15m;
}
```

## Comentarios del review

> **[bloqueante]** `PagoTransferencia.cs` — `15m` es un número mágico y es dinero.
> Por favor una constante con nombre (`ComisionBancaria`), como `PagoTarjeta.Recargo`.

> **[bloqueante]** `MetodosPagoTests.cs` — Falta agregar `PagoTransferencia` a
> `TodosLosMetodosDePago`. Esa prueba de contrato (LSP) es justamente la que garantiza
> que un método de pago nuevo se comporta como los demás.

> **[sugerencia]** ¿Una transferencia se puede reembolsar? Si sí, debería implementar
> `IReembolsable` (y devolver lo cobrado sin la comisión, que el banco no devuelve).

> **[pregunta]** ¿Hay que validar el número de referencia de la transferencia?
> Si es otro requisito, mejor un issue aparte y otro PR: un PR = una cosa.

> **[nit]** El comentario `// clase de pago por transferencia` repite el nombre de la clase.

## Lo que hizo bien el PR (también se dice)

- No tocó `ServicioPedidos` ni `CalculadoraPrecios`: es exactamente lo que buscábamos con OCP.
- Trae su propia prueba unitaria en formato AAA.

## Decisión y seguimiento

1. **Cambios solicitados** (dos bloqueantes).
2. El autor respondió con el commit `fix(pagos): atender comentarios del code review`.
3. La pregunta del número de referencia se movió a un issue nuevo.
4. Con el CI en verde y los bloqueantes resueltos: **aprobado**.
