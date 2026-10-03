# POS System - API Documentation

## 📋 Overview

REST API for Point of Sale (POS) system built with ASP.NET Core 10, PostgreSQL, and JWT Authentication.

**Base URL:** `http://localhost:5000/api` (Development)

**Swagger UI:** `http://localhost:5000/swagger`

**Health Check:** `http://localhost:5000/health`

---

## 🔐 Authentication & Authorization

### JWT Token Structure

The API uses **JWT Bearer tokens** with **Refresh Token** rotation.

#### Access Token
- **Lifetime:** 60 minutes (configurable via `JwtSettings:ExpirationMinutes`)
- **Algorithm:** HS256
- **Claims:**
  - `sub` - User ID
  - `unique_name` - Username
  - `email` - User email
  - `nombre` - Full name
  - `role` - User roles (Admin, Vendedor)
  - `jti` - Token ID

#### Refresh Token
- **Lifetime:** 7 days (configurable via `JwtSettings:RefreshTokenDays`)
- **Rotation:** New refresh token issued on each refresh (old one revoked)
- **Storage:** Hashed (SHA256) in database

### Authorization Header

```http
Authorization: Bearer <access_token>
```

### Role-Based Access Control

| Role | Description |
|------|-------------|
| **Admin** | Full access to all endpoints |
| **Vendedor** | Sales operations, read-only access to catalogs |

---

## 🚀 Auth Endpoints

### POST `/api/auth/login`
**Public** | Rate Limited: 5 req/min

**Request:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
  "expiration": "2026-01-15T14:30:00Z",
  "usuario": {
    "id": 1,
    "nombre": "Juan Pérez",
    "username": "juanp",
    "activo": true
  }
}
```

**Errors:**
- `401` - Invalid credentials
- `429` - Too many requests

---

### POST `/api/auth/register`
**Public** | Rate Limited: 5 req/min

**Request:**
```json
{
  "nombre": "string",
  "username": "string",
  "email": "string",
  "password": "string",
  "confirmPassword": "string"
}
```

**Response (200):** Same as login

**Errors:**
- `400` - Username/email already exists
- `429` - Too many requests

---

### POST `/api/auth/refresh-token`
**Public** | Rate Limited: 10 req/min

**Request:**
```json
{
  "refreshToken": "string"
}
```

**Response (200):** New token pair (same structure as login)

**Errors:**
- `401` - Invalid or expired refresh token
- `429` - Too many requests

---

### POST `/api/auth/revoke-token`
**Protected** (Bearer Token)

**Request:**
```json
{
  "refreshToken": "string"
}
```

**Response (200):**
```json
{ "message": "Token revocado exitosamente" }
```

---

### POST `/api/auth/change-password`
**Protected** (Bearer Token)

**Request:**
```json
{
  "currentPassword": "string",
  "newPassword": "string",
  "confirmNewPassword": "string"
}
```

**Response (200):**
```json
{ "message": "Contraseña cambiada exitosamente" }
```

**Errors:**
- `400` - Current password incorrect
- `401` - Unauthorized

---

### GET `/api/auth/me`
**Protected** (Bearer Token)

**Response (200):**
```json
{
  "id": 1,
  "nombre": "Juan Pérez",
  "username": "juanp",
  "activo": true
}
```

---

## 📦 Common Response Formats

### Success Response
```json
{
  "data": { ... },
  "message": "Optional success message"
}
```

### Error Response (ProblemDetails)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Error title",
  "status": 400,
  "detail": "Detailed error message",
  "instance": "/api/endpoint",
  "traceId": "00-xxxxxxxx-..."
}
```

### Paginated Response
```json
{
  "items": [...],
  "pagina": 1,
  "cantidad": 10,
  "total": 100,
  "totalPaginas": 10
}
```

---

## 🏷️ Rate Limiting Headers

All responses include:
- `X-RateLimit-Limit` - Max requests allowed
- `X-RateLimit-Remaining` - Requests remaining in window
- `Retry-After` - Seconds until next request allowed (on 429)

---

## 📚 Entity Endpoints

### 👤 Usuarios (`/api/usuario`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/activos` | Admin | List active users |
| GET | `/{id}` | Admin | Get user by ID |
| GET | `/username/{username}` | Admin | Get user by username |
| GET | `/{id}/ventas` | Admin, Vendedor | User's sales |
| GET | `/{id}/compras` | Admin | User's purchases |
| GET | `/{id}/cajas` | Admin, Vendedor | User's cash registers |
| POST | `/` | Admin | Create user |
| PUT | `/{id}` | Admin | Update user |
| DELETE | `/{id}` | Admin | Soft delete user |

**Create/Update DTO:**
```json
{
  "id": 0,
  "nombre": "string",
  "username": "string",
  "email": "string",
  "password": "string",      // Only for create
  "activo": true
}
```

---

### 📦 Productos (`/api/producto`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/activos` | Admin, Vendedor | Paginated active products |
| GET | `/{id}` | Admin, Vendedor | Get by ID |
| GET | `/codigo/{codigo}` | Admin, Vendedor | Get by code |
| GET | `/{id}/ventas` | Admin | Product sales history |
| GET | `/{id}/compras` | Admin | Product purchase history |
| POST | `/` | Admin | Create product |
| PUT | `/{id}` | Admin | Update product |
| DELETE | `/{id}` | Admin | Soft delete |

**Query Params for `/activos`:** `pagina=1`, `cantidad=10`

**Create/Update DTO:**
```json
{
  "id": 0,
  "codigo": "string",
  "nombre": "string",
  "descripcion": "string",
  "precioVenta": 0.00,
  "precioCompra": 0.00,
  "stock": 0,
  "stockMinimo": 0,
  "categoriaId": 1,
  "activo": true
}
```

---

### 🏷️ Categorías (`/api/categoria`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/activas` | Admin, Vendedor | List active categories |
| GET | `/{id}` | Admin, Vendedor | Get by ID |
| GET | `/{id}/productos` | Admin, Vendedor | Category products |
| POST | `/` | Admin | Create category |
| PUT | `/{id}` | Admin | Update category |
| DELETE | `/{id}` | Admin | Soft delete |

**DTO:**
```json
{
  "id": 0,
  "nombre": "string",
  "descripcion": "string",
  "activo": true
}
```

---

### 👥 Clientes (`/api/cliente`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/` | Admin, Vendedor | List all clients |
| GET | `/{id}` | Admin, Vendedor | Get by ID |
| GET | `/rtn/{rtn}` | Admin, Vendedor | Get by RTN |
| GET | `/{id}/ventas` | Admin, Vendedor | Client's sales |
| POST | `/` | Admin, Vendedor | Create client |
| PUT | `/{id}` | Admin | Update client |
| DELETE | `/{id}` | Admin | Soft delete |

**DTO:**
```json
{
  "id": 0,
  "nombre": "string",
  "rtn": "string",
  "telefono": "string",
  "email": "string",
  "direccion": "string",
  "activo": true
}
```

---

### 🏭 Proveedores (`/api/proveedor`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/` | Admin | List all providers |
| GET | `/{id}` | Admin | Get by ID |
| GET | `/rtn/{rtn}` | Admin | Get by RTN |
| GET | `/{id}/compras` | Admin | Provider's purchases |
| POST | `/` | Admin | Create provider |
| PUT | `/{id}` | Admin | Update provider |
| DELETE | `/{id}` | Admin | Soft delete |

**DTO:**
```json
{
  "id": 0,
  "nombre": "string",
  "rtn": "string",
  "telefono": "string",
  "email": "string",
  "direccion": "string",
  "contacto": "string",
  "activo": true
}
```

---

### 💰 Ventas (`/api/venta`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/` | Admin, Vendedor | Paginated sales |
| GET | `/{id}` | Admin, Vendedor | Get sale details |
| GET | `/por-fecha/{fecha}` | Admin, Vendedor | Sales by date |
| GET | `/por-usuario/{usuarioId}` | Admin, Vendedor | Sales by user |
| GET | `/{id}/detalles` | Admin, Vendedor | Sale line items |
| POST | `/` | Admin, Vendedor | Create sale |
| PUT | `/{id}` | Admin | Update sale |
| DELETE | `/{id}` | Admin | Delete sale |

**Query Params:** `pagina=1`, `cantidad=10`, `fecha=yyyy-MM-dd`

**Create DTO:**
```json
{
  "clienteId": 1,
  "usuarioId": 1,
  "cajaId": 1,
  "observaciones": "string",
  "detalles": [
    {
      "productoId": 1,
      "cantidad": 2,
      "precioUnitario": 100.00,
      "descuento": 0
    }
  ]
}
```

---

### 📦 Compras (`/api/compra`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/` | Admin | Paginated purchases |
| GET | `/{id}` | Admin | Get purchase details |
| GET | `/por-fecha/{fecha}` | Admin | Purchases by date |
| GET | `/por-proveedor/{proveedorId}` | Admin | Purchases by provider |
| GET | `/{id}/detalles` | Admin | Purchase line items |
| POST | `/` | Admin | Create purchase |
| PUT | `/{id}` | Admin | Update purchase |
| DELETE | `/{id}` | Admin | Delete purchase |

**Create DTO:** Similar to Venta but with `proveedorId` instead of `clienteId`

---

### 💵 Cajas (`/api/caja`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/` | Admin | Paginated cash registers |
| GET | `/{id}` | Admin, Vendedor | Get by ID |
| GET | `/abierta` | Admin, Vendedor | Currently open register |
| GET | `/{cajaId}/movimientos` | Admin, Vendedor | Cash movements |
| POST | `/` | Admin, Vendedor | Open new register |
| POST | `/{id}/cerrar` | Admin, Vendedor | Close register |
| PUT | `/{id}` | Admin | Update register |
| DELETE | `/{id}` | Admin | Delete register |

**Create DTO:**
```json
{
  "usuarioId": 1,
  "montoInicial": 500.00,
  "observaciones": "string"
}
```

---

### 💸 Movimientos de Caja (`/api/movimientocaja`)

| Method | Endpoint | Roles | Description |
|--------|----------|-------|-------------|
| GET | `/` | Admin | List all movements |
| GET | `/{id}` | Admin, Vendedor | Get by ID |
| GET | `/por-caja/{cajaId}` | Admin, Vendedor | Movements by register |
| POST | `/` | Admin, Vendedor | Create movement |
| PUT | `/{id}` | Admin | Update movement |
| DELETE | `/{id}` | Admin | Delete movement |

**DTO:**
```json
{
  "id": 0,
  "cajaId": 1,
  "tipo": "Entrada|Salida",
  "concepto": "string",
  "monto": 100.00,
  "observaciones": "string"
}
```

---

## 🔄 Frontend Integration Guide

### Token Storage (Recommended)

```javascript
// Store in memory (not localStorage for security)
let accessToken = null;
let refreshToken = null;

// On login
const response = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ username, password })
});
const data = await response.json();
accessToken = data.token;
refreshToken = data.refreshToken;

// Axios interceptor for auto-refresh
axios.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401 && !error.config._retry) {
      error.config._retry = true;
      const refreshResponse = await fetch('/api/auth/refresh-token', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken })
      });
      if (refreshResponse.ok) {
        const newData = await refreshResponse.json();
        accessToken = newData.token;
        refreshToken = newData.refreshToken;
        error.config.headers.Authorization = `Bearer ${accessToken}`;
        return axios(error.config);
      }
      // Refresh failed - redirect to login
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);
```

### CORS Configuration

The API allows origins:
- `http://localhost:3000` (React/Next.js)
- `http://localhost:5173` (Vite)
- `http://localhost:4200` (Angular)

Add your frontend URL to `appsettings.json` → `Cors:AllowedOrigins` for production.

---

## 🐳 Docker Quick Start

```yaml
# docker-compose.yml
version: '3.8'
services:
  api:
    build: ./Api
    ports:
      - "5000:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=PosDb;Username=postgres;Password=postgres
      - JwtSettings__SecretKey=your-super-secret-key-min-32-chars
    depends_on:
      - db
  
  db:
    image: postgres:16
    environment:
      POSTGRES_DB: PosDb
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"

volumes:
  postgres_data:
```

---

## 🛠️ Development

### Run API
```bash
cd Api
dotnet run
```

### Run Migrations
```bash
cd Api
dotnet ef database update
```

### Generate Migration
```bash
cd Api
dotnet ef migrations add MigrationName
```

---

## 📝 Notes for Frontend Team

1. **Always handle 401/403** - Redirect to login or show permission denied
2. **Handle 429** - Show retry-after message, disable buttons temporarily
3. **Pagination** - Use `pagina` and `cantidad` params, display `totalPaginas`
4. **Soft Deletes** - Deleted entities return 404, not removed from DB
5. **Decimal Precision** - Money values use 2 decimal places
6. **Dates** - Use ISO 8601 format (`yyyy-MM-ddTHH:mm:ssZ`)
7. **Validation** - Check `ProblemDetails` response for field-level errors

---

## 🔗 Quick Reference

| Resource | Base Path | Key Query Params |
|----------|-----------|------------------|
| Auth | `/api/auth` | - |
| Usuarios | `/api/usuario` | - |
| Productos | `/api/producto` | `pagina`, `cantidad` |
| Categorías | `/api/categoria` | - |
| Clientes | `/api/cliente` | - |
| Proveedores | `/api/proveedor` | - |
| Ventas | `/api/venta` | `pagina`, `cantidad`, `fecha` |
| Compras | `/api/compra` | `pagina`, `cantidad`, `fecha` |
| Cajas | `/api/caja` | `pagina`, `cantidad` |
| Movimientos | `/api/movimientocaja` | - |

---

*Generated for POS API v1.0 - Last updated: 2026*