# HanPOS

<img width="330" height="230" alt="image" src="https://github.com/user-attachments/assets/2428d4ac-2280-4936-825f-3d1908eb48cb" />
<img width="330" height="230" alt="image" src="https://github.com/user-attachments/assets/c3738aba-4d52-4a84-90a0-cc766a351363" />
<img width="330" height="230" alt="image" src="https://github.com/user-attachments/assets/ca20bba8-2aba-43ce-a3e3-a785690dcd1e" />

### Cada venta en orden. Cada turno bajo control.

MercaPOS es un punto de venta pensado para pequeñas tiendas y comercios que necesitan atender con agilidad y mantener sus operaciones claras. Reúne ventas, productos, clientes y control de caja en una interfaz directa, fácil de entender para el equipo del mostrador.

**Menos pasos al cobrar. Más claridad durante el día.**

---

## Lo que puedes hacer

| Módulo | Para qué sirve |
| --- | --- |
| **Ventas** | Busca productos por nombre o código, filtra por categoría, arma el carrito y registra el pago en efectivo, tarjeta o transferencia. |
| **Productos y categorías** | Mantén el catálogo, precios, existencias y organización por categorías. |
| **Clientes** | Consulta y administra los datos disponibles de tus clientes. |
| **Proveedores** | Organiza los contactos de abastecimiento y consulta los datos de cada proveedor. |
| **Caja** | Abre y cierra turnos, consulta el historial de cajas y revisa fondos iniciales y finales. |
| **Movimientos** | Registra entradas y salidas de efectivo y consulta la actividad del turno. |
| **Compras** | Registra entradas con varias líneas de productos, proveedor y turno de caja, y consulta el historial. |
| **Resumen** | Consulta indicadores básicos de ventas e inventario reciente. |

El flujo de venta muestra subtotal, impuesto, total y, al pagar en efectivo, el cambio. Las operaciones de caja requieren un turno abierto.

## Diseñado para el mostrador

- Navegación clara y controles en español.
- Carrito visible durante la venta.
- Búsqueda rápida y filtros de catálogo.
- Estados claros para caja abierta, caja cerrada, carga y errores.
- Diseño adaptable a escritorio, tabletas y móviles.
- Acceso protegido con tokens JWT y refresh tokens rotativos.

## Tecnologías

- **Frontend:** Vue 3, TypeScript, Vite, Vue Router y Pinia.
- **API:** ASP.NET Core 10 y Entity Framework Core.
- **Base de datos:** PostgreSQL 16.
- **Autenticación:** JWT con refresh tokens y roles `Admin` / `Vendedor`.
- **Contenedores:** Docker Compose para API y PostgreSQL.

## Requisitos

- Docker y Docker Compose para ejecutar API y base de datos en contenedores.
- Node.js `22.18+` o `24.12+` para el frontend.
- .NET SDK 10 para ejecutar la API localmente.

## Ejecución local de la API

Si ejecutas la API fuera de Docker, primero configura una instancia de PostgreSQL accesible desde tu equipo y guarda la cadena de conexión y el secreto JWT en User Secrets o variables de entorno. No guardes credenciales en archivos que vayas a publicar.

## Cuentas y acceso

No hay credenciales predeterminadas. El registro público asigna el rol `Vendedor`; el aprovisionamiento de cuentas `Admin` debe gestionarse por un procedimiento autorizado para la instalación. Los permisos también se validan en la API.

## Propiedad y licencia

MercaPOS es software propietario de **Haniel Hernándex**. La publicación o acceso al código no concede permiso para venderlo, sublicenciarlo, redistribuirlo ni explotarlo comercialmente. El titular conserva en exclusiva el derecho de comercializar el sistema y de autorizar su uso a terceros por escrito.

Consulta los términos completos en [`LICENSE`](LICENSE). Las dependencias de terceros conservan sus propias licencias.
