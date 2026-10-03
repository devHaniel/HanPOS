# MercaPOS

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
| **Caja** | Abre y cierra turnos, consulta el historial de cajas y revisa fondos iniciales y finales. |
| **Movimientos** | Registra entradas y salidas de efectivo y consulta la actividad del turno. |
| **Compras** | Registra entradas de mercancía y consulta el historial de compras. |
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

## Puesta en marcha con Docker

1. Crea un archivo `.env` en la raíz del proyecto. No lo publiques ni lo incluyas en commits.

```dotenv
POSTGRES_DB=mercapos
POSTGRES_USER=postgres
POSTGRES_PASSWORD=cambia-esta-clave
JWT_SECRET_KEY=reemplaza-por-un-secreto-aleatorio-de-al-menos-32-caracteres
```

Puedes generar un secreto con `openssl rand -base64 48`. Usa valores propios para tu entorno.

2. Inicia la API y PostgreSQL:

```bash
docker compose up --build -d
```

3. Configura el frontend en `Front/.env`:

```dotenv
VITE_API_BASE_URL=http://localhost:8080/api
```

4. Instala y ejecuta el frontend:

```bash
cd Front
npm install
npm run dev
```

Abre `http://localhost:5173`. La API estará disponible en `http://localhost:8080`; el estado de salud está en `/health`.

El contenedor de API aplica las migraciones pendientes al iniciar. Swagger se habilita solo en entorno de desarrollo.

## Ejecución local de la API

Si ejecutas la API fuera de Docker, primero configura una instancia de PostgreSQL accesible desde tu equipo y guarda la cadena de conexión y el secreto JWT en User Secrets o variables de entorno. No guardes credenciales en archivos que vayas a publicar.

## Cuentas y acceso

No hay credenciales predeterminadas. El registro público asigna el rol `Vendedor`; el aprovisionamiento de cuentas `Admin` debe gestionarse por un procedimiento autorizado para la instalación. Los permisos también se validan en la API.

## Propiedad y licencia

MercaPOS es software propietario de **Haniel Hernándex**. La publicación o acceso al código no concede permiso para venderlo, sublicenciarlo, redistribuirlo ni explotarlo comercialmente. El titular conserva en exclusiva el derecho de comercializar el sistema y de autorizar su uso a terceros por escrito.

Consulta los términos completos en [`LICENSE`](LICENSE). Las dependencias de terceros conservan sus propias licencias.
