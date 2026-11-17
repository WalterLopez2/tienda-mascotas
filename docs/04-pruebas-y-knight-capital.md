# Pruebas: pirámide, AAA, TDD, dobles y el caso Knight Capital

## La pirámide de pruebas de este proyecto

```
            /\        Manual / E2E: dotnet run --project src/TiendaMascotas.App
           /  \       (demo antes vs después)
          /----\
         / Integ.\    2 pruebas  · Integracion/FlujoCompletoTests (archivo real en disco)
        /--------\
       /Caracteriz.\  15 pruebas · Caracterizacion/ (legacy vs nuevo)
      /------------\
     /  Unitarias   \ 42 pruebas · Unitarias/ + KnightCapital/ (milisegundos, sin disco ni red)
    /________________\
```

Muchas pruebas rápidas abajo, pocas lentas arriba. Total: **59 pruebas**, ~0.2 s.

## Anatomía AAA

```csharp
[Fact]
public void CalcularTotal_SinPoliticas_SoloAgregaIsvDel15PorCiento()
{
    // Arrange: preparo el escenario
    var pedido = UnPedido().Con("Vacuna", Categoria.Medicina, 100m).Construir();

    // Act: UNA sola acción
    var total = SinPoliticas().CalcularTotal(pedido);

    // Assert: verifico el resultado
    Assert.Equal(115m, total);
}
```

Nombres: `Método_Escenario_ResultadoEsperado`. Si la prueba falla, el nombre ya dice qué se rompió.

## TDD: Red → Green → Refactor

La funcionalidad de cupones se hizo con TDD. Se ve en el historial:

```bash
git log --oneline feature/cupones-tdd
```

| Ciclo | RED (prueba que falla) | GREEN (lo mínimo) | REFACTOR |
|---|---|---|---|
| 1 | Cupón en minúsculas/espacios | `Trim().ToUpperInvariant()` | — |
| 2 | Nuevo cupón `PRIMERACOMPRA` 15% | Un `if` más (feo a propósito) | De cadena de `if` a tabla |
| 3 | Cupón `BLACKFRIDAY` 20% | Una fila en la tabla | — |
| 4 | Cupón vencido no descuenta | Fecha escrita a mano en un `if` | Record `Cupon` con `EstaVigenteEl()` |

## Dobles de prueba (carpeta `tests/.../Dobles`)

| Doble | Clase | Qué hace | Dónde se usa |
|---|---|---|---|
| **Dummy** | `PasarelaDummy` | Se pasa solo para cumplir la firma; si se usa, revienta | Cliente inactivo, pedido vacío |
| **Stub** | `RelojFijo` | Respuesta fija: congela la fecha | Vencimiento de cupones, fecha guardada |
| **Spy** | `NotificadorEspia` | Graba las llamadas; la prueba pregunta después | "¿Se notificó al cliente con el total?" |
| **Mock** | `PasarelaPagoMock` | Se programa con la expectativa y se verifica a sí mismo | "Espero UN cobro de L 1,061.63" |
| **Fake** | `RepositorioEnMemoria` | Implementación real pero simplificada (lista en memoria) | Guardado e idempotencia |

## El colapso de Knight Capital (1 de agosto de 2012)

**Qué pasó:**
1. Knight reutilizó una bandera de configuración que antes activaba una función vieja
   ("Power Peg"), código muerto que nunca se borró.
2. El despliegue manual llegó a 7 de 8 servidores. El octavo seguía con el código viejo.
3. Al activar la bandera, ese servidor ejecutó el código muerto y empezó a enviar órdenes
   al mercado sin parar.
4. No había un freno automático. En unos 45 minutos perdieron cerca de **440 millones de dólares**
   y la empresa tuvo que ser rescatada y vendida.

**Cómo cada lección quedó en código y en pruebas:**

| Lección | En este repo |
|---|---|
| Borrar el código muerto, no "desactivarlo" | El legacy tiene `modoViejo // NO TOCAR`. `Legacy_BanderaViejaReactivada_DuplicaElCobroSinAvisar` demuestra el daño; la versión nueva no tiene esa bandera (`ServicioNuevo_NoTieneBanderasOcultas`). |
| Nunca repetir una operación de dinero | `Procesar_MismoPedidoDosVeces_CobraUnaSolaVez` (idempotencia) y su versión de integración que sobrevive a un reinicio. |
| Límites automáticos ("kill switch") | `ControlDeRiesgo`: `Procesar_AlSuperarElLimiteAcumulado_SeDetieneYNoCobraMas`. |
| Despliegues automáticos y verificados | GitHub Actions compila y corre las 59 pruebas en cada PR; nadie fusiona a mano sin el check en verde. |
| Probar los caminos de error, no solo el feliz | Pago rechazado, cliente inactivo, pedido vacío, monto excesivo. |
