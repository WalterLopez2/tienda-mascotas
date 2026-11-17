# Tienda de Mascotas "Huellitas" — de "funciona" a "es profesional"

Proyecto de Ingeniería de Software II. Toma un procesador de pedidos **legacy** que funciona
pero no es profesional, y lo lleva paso a paso a código limpio, con diseño SOLID,
pruebas automatizadas y un flujo de trabajo profesional con Git y GitHub.

```bash
dotnet run --project src/TiendaMascotas.App
```

```bash
dotnet test
```

```bash
git log --oneline --graph --all
```

## Estructura

```
src/
  TiendaMascotas.Legacy/   ← ANTES: el código original, sin modificar
  TiendaMascotas.Core/     ← DESPUÉS: Clean Code + SOLID
    Dominio/               Pedido, LineaPedido, Producto, Cliente, Categoria, ResultadoPedido
    Precios/               CalculadoraPrecios, IPoliticaDescuento, DescuentoCompraGrande, DescuentoCupon, Cupon
    Pagos/                 IMetodoPago, PagoTarjeta, PagoEfectivo, IReembolsable, IPasarelaPago
    Persistencia/          IRepositorioPedidos, RepositorioPedidosArchivo
    Notificaciones/        INotificador, NotificadorConsola
    Riesgo/                ControlDeRiesgo (lección Knight Capital)
    Tiempo/                IReloj, RelojSistema
    Servicios/             ServicioPedidos (orquestador)
  TiendaMascotas.App/      ← Demo: corre el antes y el después lado a lado
tests/TiendaMascotas.Tests/
  Caracterizacion/         Golden master del legacy + equivalencia legacy vs nuevo
  Unitarias/               Pruebas AAA
  KnightCapital/           Idempotencia, kill switch y código muerto
  Integracion/             Flujo con archivo real
  Dobles/                  Dummy, Stub, Spy, Mock, Fake
docs/                      Explicación de cada módulo
.github/                   CI, plantilla de PR e issues
```

## Mapa de los módulos

| Módulo | Tema | Dónde verlo |
|---|---|---|
| **1 · Clean Code** | Nombres, funciones y comentarios | `Legacy/ProcesadorPedidos.cs` vs `Core/Servicios/ServicioPedidos.cs` |
| | Code smells y deuda técnica | [docs/01-antes-y-despues.md](docs/01-antes-y-despues.md) (16 smells inventariados) |
| | Refactoring seguro | `tests/.../Caracterizacion/` + commits pequeños de `refactor/clean-code` |
| **2 · Git** | Modelo mental, ramas, commits que hablan | `git log --graph --all`, [CONTRIBUTING.md](CONTRIBUTING.md) |
| | Pull Requests y code review | [.github/pull_request_template.md](.github/pull_request_template.md), [docs/05-code-review-ejemplo.md](docs/05-code-review-ejemplo.md) |
| | GitHub como plataforma de equipo | [.github/workflows/ci.yml](.github/workflows/ci.yml), plantillas de PR e issues |
| **3 · SOLID** | Acoplamiento, cohesión, S·O·L·I·D, DRY/KISS/YAGNI | [docs/02-solid-y-principios.md](docs/02-solid-y-principios.md), un commit por principio en `refactor/solid` |
| | Auditar el diseño de una IA | [docs/03-auditoria-diseno-ia.md](docs/03-auditoria-diseno-ia.md) |
| **4 · Testing** | Pirámide, AAA, TDD, dobles, Knight Capital | [docs/04-pruebas-y-knight-capital.md](docs/04-pruebas-y-knight-capital.md), rama `feature/cupones-tdd` |

## Historia del repositorio (cada rama = un Pull Request)

| Rama | Qué demuestra |
|---|---|
| `test/caracterizacion-legacy` | Red de seguridad ANTES de tocar el código |
| `refactor/clean-code` | Tipos con nombre, sin números mágicos, cláusulas de guarda |
| `refactor/solid` | Un commit por principio: S, O, L+I, D |
| `feature/cupones-tdd` | Commits RED → GREEN → REFACTOR |
| `feature/control-riesgo` | Dobles de prueba + lecciones de Knight Capital |
| `chore/github-equipo` | CI y plantillas de PR e issues |
| `docs/documentacion` | Esta documentación |
| `feature/pago-transferencia` | Pull Request abierto: método de pago nuevo sin tocar el servicio (OCP) |

## Resultado

- El mismo pedido cobra **L 1,061.63** en el legacy y en la versión nueva (verificado por pruebas).
- **59 pruebas** automatizadas en ~0.2 s.
- El legacy cobra dos veces el mismo pedido; la versión nueva lo rechaza.
