# SOLID, acoplamiento y cohesión en este proyecto

## Acoplamiento, cohesión y el costo del cambio

| | Legacy | Nuevo |
|---|---|---|
| **Cohesión** | Baja: un método mezcla precios, pagos, archivos y correos | Alta: cada clase tiene un tema (`CalculadoraPrecios` solo precios) |
| **Acoplamiento** | Alto y oculto: `File`, `DateTime.Now`, `Console` metidos en la lógica | Bajo y explícito: todo lo externo entra por el constructor como interfaz |
| **Costo de agregar un cupón** | Editar el método gigante y rezar | Agregar una fila en la tabla de `DescuentoCupon` |
| **Costo de agregar un método de pago** | Otro `else if` en `Procesar` | Una clase nueva que implementa `IMetodoPago` (ver rama `feature/pago-transferencia`) |
| **Costo de probar** | Escribe en disco y depende del reloj real | Dobles de prueba, milisegundos |

## Los cinco principios

| Principio | Antes (legacy) | Después | Archivo |
|---|---|---|---|
| **S** — Responsabilidad única | `Procesar` tiene 5 razones para cambiar | `ServicioPedidos` solo orquesta; cálculo, cobro, guardado y notificación en clases propias | `Servicios/ServicioPedidos.cs` |
| **O** — Abierto/Cerrado | Promoción nueva = editar el método | `IPoliticaDescuento`: promoción nueva = clase nueva; la calculadora no se toca | `Precios/IPoliticaDescuento.cs` |
| **L** — Sustitución de Liskov | `PagoEfectivo.Reembolsar()` lanza excepción: no puede sustituir a `MetodoPago` | Todo `IMetodoPago` cumple el contrato; prueba de contrato contra todas las implementaciones | `Pagos/IMetodoPago.cs`, `MetodosPagoTests` |
| **I** — Segregación de interfaces | `INotificador` con 4 métodos, 3 sin implementar | `INotificador` con 1 método; `IReembolsable` separado | `Notificaciones/INotificador.cs`, `Pagos/IReembolsable.cs` |
| **D** — Inversión de dependencias | Crea `File`, `DateTime.Now`, `Console` adentro | Recibe `IPasarelaPago`, `IRepositorioPedidos`, `INotificador`, `IReloj`; `Program.cs` es el composition root | `App/Program.cs` |

## DRY, KISS y YAGNI

- **DRY:** `calc2()` duplicaba el cálculo del legacy. Ahora existe un solo `CalcularSubtotal`.
  En las pruebas, `PedidoBuilder` y `Fabrica` evitan repetir el armado de pedidos y servicios.
- **KISS:** los dobles de prueba están escritos a mano (≈15 líneas cada uno) en lugar de
  agregar una librería de mocking. Para el tamaño de este proyecto es más fácil de leer y explicar.
- **YAGNI:** no hay `IFabricaDeCalculadoras`, ni repositorio genérico, ni bus de eventos.
  `ControlDeRiesgo` es una clase concreta (no interfaz) porque hoy hay una sola forma de controlar riesgo.
  Ver [la auditoría del diseño propuesto por una IA](03-auditoria-diseno-ia.md).
