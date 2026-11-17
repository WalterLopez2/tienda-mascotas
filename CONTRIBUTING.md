# Cómo contribuir

## Flujo de trabajo (GitHub Flow)

1. `main` siempre compila y pasa las pruebas. Nadie hace push directo a `main`.
2. Cada cambio vive en su propia rama corta, con prefijo según el tipo:
   - `feature/...` funcionalidad nueva
   - `fix/...` corrección
   - `refactor/...` mejora de diseño sin cambiar comportamiento
   - `test/...` solo pruebas
   - `docs/...`, `chore/...`
3. Se abre un Pull Request hacia `main` usando la plantilla.
4. El CI (GitHub Actions) debe quedar en verde.
5. Al menos una aprobación en el code review.
6. Se fusiona con *merge commit* (`--no-ff`) para que la historia muestre cada PR.

## Commits que hablan (Conventional Commits)

```
tipo(alcance): resumen en imperativo, menos de 72 caracteres

Cuerpo opcional: POR QUÉ se hizo el cambio, no qué líneas cambiaron.
```

Ejemplos reales de este repo:

- `refactor(solid): S - separar persistencia, notificación y pagos`
- `test(cupon): RED - un cupón vencido no debe descontar`
- `feat(riesgo): idempotencia y kill switch (lección Knight Capital)`

Un commit = un cambio lógico. Si el mensaje necesita "y además...", son dos commits.

## Code review con criterio

Quien revisa clasifica cada comentario para que el autor sepa qué es obligatorio:

| Prefijo | Significado |
|---|---|
| **[bloqueante]** | Error, riesgo de seguridad o de dinero. No se fusiona sin arreglarlo. |
| **[sugerencia]** | Mejora razonable; el autor decide. |
| **[nit]** | Detalle de estilo menor. |
| **[pregunta]** | No entiendo algo; tal vez falta un nombre mejor o una prueba. |

Se revisa el código, no a la persona. Ver `docs/code-review-ejemplo.md`.

## Protección de la rama `main` (configurar en GitHub → Settings → Branches)

- Requerir Pull Request antes de fusionar
- Requerir 1 aprobación
- Requerir que el check **CI / compilar-y-probar** pase
- No permitir force push
