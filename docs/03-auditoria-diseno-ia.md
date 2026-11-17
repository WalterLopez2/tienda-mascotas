# Auditar el diseño que te propone una IA

Antes de refactorizar le pedí a un asistente de IA: *"Diseña una arquitectura SOLID para el
procesador de pedidos de una tienda de mascotas"*. Esta fue (resumida) su propuesta:

```csharp
public interface IEntity<TKey> { TKey Id { get; } }
public interface IGenericRepository<T, TKey> where T : IEntity<TKey>
{
    T GetById(TKey id); IEnumerable<T> GetAll(); IEnumerable<T> Find(Expression<Func<T, bool>> p);
    void Add(T e); void AddRange(IEnumerable<T> e); void Update(T e); void Remove(T e);
    void RemoveRange(IEnumerable<T> e); int Count(); bool Exists(TKey id); void SaveChanges();
}
public interface IUnitOfWork : IDisposable { IGenericRepository<Pedido, Guid> Pedidos { get; } void Commit(); }
public interface IDiscountStrategyFactory { IDiscountStrategy Create(DiscountType type); }
public interface IPaymentProcessorFactory { IPaymentProcessor Create(PaymentType type); }
public interface IEventBus { void Publish<T>(T evento); void Subscribe<T>(Action<T> handler); }
public class OrderProcessingManagerService : IOrderProcessingManagerService
{
    public OrderProcessingManagerService(IUnitOfWork uow, IDiscountStrategyFactory dsf,
        IPaymentProcessorFactory ppf, IEventBus bus, ILogger<OrderProcessingManagerService> log,
        IMapper mapper, IValidator<Pedido> validator, IOptions<OrderSettings> settings) { ... }
    public async Task<OrderResultDto> ProcessOrderAsync(OrderRequestDto dto)
    {
        switch (dto.PaymentType) { case PaymentType.Card: ... case PaymentType.Cash: ... }
        ...
    }
}
```

## Mi checklist para auditarla

| Pregunta | Resultado | Decisión |
|---|---|---|
| ¿Resuelve un problema que **tengo hoy**? | `IEventBus`, `IUnitOfWork`, `IMapper`: no hay ningún suscriptor, ni transacciones, ni DTOs | **Rechazado (YAGNI)** |
| ¿Las interfaces son pequeñas (ISP)? | `IGenericRepository` tiene 11 métodos; yo uso 2 (`Guardar`, `YaFueProcesado`) | **Rechazado** → `IRepositorioPedidos` con 2 métodos |
| ¿Cumple OCP de verdad? | Dice "Strategy" pero `ProcessOrderAsync` tiene un `switch` sobre `PaymentType`: un pago nuevo obliga a editar el servicio **y** la fábrica | **Rechazado** → `IMetodoPago` polimórfico, sin `switch` |
| ¿Los nombres revelan intención? | `OrderProcessingManagerService`: "Manager" y "Service" no dicen nada | **Corregido** → `ServicioPedidos` |
| ¿Cuántas dependencias tiene la clase principal? | 8 en el constructor: señal de que hace demasiado (SRP) | **Corregido** → 6, todas con un propósito claro |
| ¿Respeta las reglas de negocio actuales? | Usa `double` para dinero y no menciona la regla de Medicina sin descuento | **Corregido** → `decimal` + pruebas de caracterización |
| ¿Contempla el riesgo financiero? | No tiene idempotencia ni límites de cobro | **Agregado** → `ControlDeRiesgo` (lección Knight Capital) |
| ¿Puedo probarlo fácil? | Sí, todo son interfaces… demasiadas: 8 mocks por prueba | Me quedo con la idea de **inyectar dependencias**, sin la ceremonia |

## Qué SÍ tomé de la IA

- Inyectar dependencias por constructor.
- Separar la estrategia de descuento en su propia abstracción (`IPoliticaDescuento`).
- Usar un objeto de resultado en vez de strings (`ResultadoPedido`).

## Conclusión

La IA produce código que **parece** profesional porque usa muchos patrones, pero el diseño
profesional es el más simple que resuelve el problema real y se puede cambiar barato.
La responsabilidad del diseño sigue siendo del ingeniero: la IA propone, yo audito con
criterio (SOLID + YAGNI + KISS) y con pruebas.
