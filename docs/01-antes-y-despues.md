# Antes y después: code smells, deuda técnica y cómo se pagó

Archivo "antes": [`src/TiendaMascotas.Legacy/ProcesadorPedidos.cs`](../src/TiendaMascotas.Legacy/ProcesadorPedidos.cs)
Código "después": [`src/TiendaMascotas.Core/`](../src/TiendaMascotas.Core/)

## Inventario de deuda técnica (lo que encontré en el legacy)

| # | Code smell | Dónde estaba | Riesgo | Cómo se pagó | Commit |
|---|---|---|---|---|---|
| 1 | **Método largo / God method** | `Procesar(object[] d)` hace TODO: valida, calcula, cobra, guarda, notifica | Alto | Se dividió en `CalculadoraPrecios`, `IMetodoPago`, `IPasarelaPago`, `IRepositorioPedidos`, `INotificador`; `ServicioPedidos` solo orquesta | `refactor(solid): S` |
| 2 | **Nombres sin intención** | `d`, `r`, `t`, `x`, `p`, `c`, `its`, `calc2` | Medio | `pedido`, `montoACobrar`, `CalcularSubtotal`, `ImporteConDescuentoDeCategoria` | `refactor(dominio)` |
| 3 | **Obsesión por primitivos** | El pedido es un `object[]`; `d[3]` es "tipo de pago"; ítems en un string `"a;b;c|..."` | Alto | `Pedido`, `LineaPedido`, `Producto`, `Cliente`, `Categoria` (enum) con validación | `refactor(dominio)` |
| 4 | **Números mágicos** | `1000`, `0.1`, `0.05`, `0.15`, `0.03` | Medio | Constantes con nombre: `TasaIsv`, `MontoMinimo`, `Recargo` | `refactor(precios)` |
| 5 | **Flecha de ifs anidados** | 4 niveles de `if/else` | Medio | Cláusulas de guarda en `ServicioPedidos.Procesar` | `refactor(pedidos)` |
| 6 | **Cadena de if/else por categoría** | `if (x[1] == "Alimento") ... else if ...` | Medio | Método de tabla: `Dictionary<Categoria, decimal>` | `refactor(precios)` |
| 7 | **Código duplicado (DRY)** | `calc2()` es una copia del cálculo de `Procesar()` | Medio | Un solo lugar: `CalculadoraPrecios.CalcularSubtotal` | `refactor(precios)` |
| 8 | **Comentario que miente** | `// descuento del 10% para clientes VIP` sobre un descuento que es por monto, no por VIP | Medio | El nombre lo dice: `DescuentoCompraGrande`; el comentario sobra | `refactor(solid): O` |
| 9 | **Comentarios que repiten el código** | `// Clase que procesa los pedidos`, `// Procesa el pedido` | Bajo | Eliminados; los nombres explican el qué y los comentarios que quedan explican el por qué | — |
| 10 | **Código muerto + bandera misteriosa** | `modoViejo // NO TOCAR` duplica el total si se activa | **Crítico** | Eliminada. Prueba que lo documenta en el legacy (`KnightCapitalTests`) | `feat(riesgo)` |
| 11 | **`double` para dinero** | `double t` | Alto | `decimal` en todo el dominio | `refactor(dominio)` |
| 12 | **Dependencias ocultas** | `File.AppendAllText`, `DateTime.Now`, `Console` dentro de la lógica | Alto | Interfaces inyectadas (`IRepositorioPedidos`, `IReloj`, `INotificador`) | `refactor(solid): D` |
| 13 | **Errores como strings** | Devuelve `"OK:1061.63"` o `"ERROR"` | Medio | `ResultadoPedido` (Exitoso, MontoCobrado, Mensaje) | `refactor(dominio)` |
| 14 | **Violación de LSP** | `PagoEfectivo.Reembolsar()` lanza `NotImplementedException` | Alto | `IReembolsable` aparte; efectivo no la implementa | `refactor(solid): L e I` |
| 15 | **Interfaz gorda (ISP)** | `INotificador` con 4 métodos; 3 lanzan `NotImplementedException` | Medio | `INotificador.Notificar(...)` un solo método | `refactor(solid): L e I` |
| 16 | **Sin idempotencia** | Procesar el mismo pedido dos veces lo cobra dos veces | **Crítico** | `YaFueProcesado` + `ControlDeRiesgo` | `feat(riesgo)` |

## Refactoring seguro: cómo limpié sin romper

1. **Primero la red, después la cirugía.** Antes de tocar una línea escribí
   [pruebas de caracterización](../tests/TiendaMascotas.Tests/Caracterizacion/ProcesadorLegacyCaracterizacionTests.cs):
   no dicen si el legacy está bien, fotografían lo que hace hoy (incluidas sus rarezas,
   como que Medicina no tiene descuento).
2. **Pruebas de equivalencia.** [`EquivalenciaLegacyTests`](../tests/TiendaMascotas.Tests/Caracterizacion/EquivalenciaLegacyTests.cs)
   le pasa las MISMAS entradas al legacy y a la versión nueva y exige el mismo monto.
3. **Pasos pequeños, un commit por paso.** Cada commit compila y deja las pruebas en verde.
4. **Cambios de comportamiento explícitos.** Cuando el cupón empezó a aceptar minúsculas,
   la prueba de equivalencia falló. No borré el caso: lo moví a una prueba con nombre
   `..._AhoraSiDescuentaAunqueElLegacyNo` que documenta el cambio intencional.

## Lo que NO cambié a propósito

- **Medicina sin descuento.** Parece un olvido del legacy, pero es una regla de negocio
  hasta que alguien de negocio diga lo contrario. Refactorizar no es cambiar reglas.
- **El legacy sigue en el repo** y no se modificó: es la referencia del "antes" y la base
  de las pruebas de equivalencia.
