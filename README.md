# 🏍️ BAIKUREDO — Plataforma de Gestión y Catálogo de Motocicletas Usadas

**Baikuredo** es una aplicación web construida con **ASP.NET Core 10 MVC**, **Entity Framework Core 10**, **SQL Server** y **ASP.NET Core Identity** para administrar el ciclo completo de comercialización de motocicletas usadas: desde la recepción del vehículo y su propietario, hasta su publicación en el catálogo público, recepción de interesados, gestión de reservas y trazabilidad mediante bitácora de auditoría.

---

## 📋 Características Principales

### 1. Catálogo Público (`[AllowAnonymous]`)
- **Exploración sin sesión (`/Catalog`):** Cualquier visitante puede consultar las motocicletas en estado **Disponible**, con buscador por marca, modelo, categoría, año o color, foto de portada y paginación.
- **Ficha técnica y galería (`/Catalog/Details/{id}`):** Muestra todas las fotografías de la publicación y las especificaciones de la motocicleta, ocultando datos internos sensibles (como propietario, placa, historial interno o reservas).
- **Registro de interés (`POST /Catalog/RegisterInterest`):** Formulario público protegido con token antifalsificación (`[ValidateAntiForgeryToken]`) y validaciones de datos para que los clientes potenciales dejen su contacto y mensaje.

### 2. Gestión Operativa y Comercial (Backoffice)
- **Categorías, Marcas y Modelos:** Catálogos maestros con activación/desactivación (`Toggle`), búsqueda y paginación.
- **Propietarios (`Owners`):** Registro de los dueños de las motocicletas con validación de documento único.
- **Motocicletas (`Motorcycles`):** Inventario físico con placa única, año, cilindraje, color, modelo, categoría, propietario y trazabilidad automática del usuario que registró la moto.
- **Publicaciones (`Publications`):** Oferta comercial con carga de múltiples fotografías, selección de foto de portada, control de estado (*Disponible*, *Reservada*, *Vendida*) y registro automático de **Historial de Precios** ante cada modificación.
- **Reservas (`Reservations`):** Apartado de publicaciones disponibles con monto de anticipo, fecha de vencimiento, cierre por venta (`Complete`) o liberación por expiración (`Expire`).
- **Interesados (`Interests`):** Bandeja administrativa para gestionar las solicitudes enviadas desde el catálogo público, permitiendo filtrarlas por placa o datos del cliente y marcarlas como atendidas o pendientes.

### 3. Seguridad, Roles, Permisos (PAM / RBAC) y Auditoría
- **Autenticación con ASP.NET Core Identity:** Manejo de sesión por cookie (`BaikuredoAuth`), edición de perfil propio y cambio de contraseña.
- **Autorización granular por acción (`[CustomAuthorize("modulo.accion")]`):** Cada acción de los controladores administrativos y cada botón o enlace de la interfaz valida los permisos asignados al rol del usuario (`show`, `create`, `update`, `delete`).
- **Bitácora de Auditoría (`AuditLogs`):** Registro automático de quién ejecutó cada acción relevante (creación, edición, cambio de precio, reservas, atención de interesados y eliminaciones), sobre qué tabla y en qué fecha/hora.

---

## 🗂️ Estructura del Proyecto

```text
Project-Csharp/
├── Baikuredo.sln
├── docker-compose.yml              ← Contenedor opcional de SQL Server 2022
└── Baikuredo.Web/
    ├── Program.cs                  ← Punto de entrada y pipeline de middlewares
    ├── CustomConfiguration.cs      ← Inyección de dependencias, Identity (IAM) y ejecución de SeedDb
    ├── appsettings.json            ← Configuración y cadena de conexión (MyConnection)
    ├── Controllers/                ← Controladores MVC (CatalogController público + 11 administrativos)
    ├── Core/
    │   ├── Attributes/             ← CustomAuthorizeAttribute y CustomAuthorizeFilter
    │   ├── Enums/                  ← Estados de publicaciones y reservas
    │   ├── Pagination/             ← PagedList, PaginationRequest y PaginationResponse
    │   └── Response.cs             ← Wrapper estándar Response<T>
    ├── Data/
    │   ├── DataContext.cs          ← IdentityDbContext<User> con relaciones e índices únicos
    │   ├── Entities/               ← Entidades del modelo de datos
    │   └── Seeders/                ← Poblado inicial automático (Permisos, Roles, Usuarios, Marcas, Motos, etc.)
    ├── DTOs/                       ← Objetos de transferencia de datos y validaciones por módulo
    ├── Helpers/                    ← SelectListHelper y CurrentUserHelper
    ├── Migrations/                 ← Migraciones de Entity Framework Core
    ├── Services/                   ← Contratos (Abstraction) e implementaciones de lógica de negocio
    └── Views/                      ← Vistas Razor con plantilla NiceAdmin y menú dinámico por permisos
```

---

## 🚀 Cómo Correr el Proyecto

### 1. Prerrequisitos
- **.NET SDK 10.0** instalado (`dotnet --version`).
- **SQL Server** (LocalDB, instancia local o mediante Docker con el `docker-compose.yml` incluido).
- Herramienta CLI de Entity Framework Core (`dotnet tool install --global dotnet-ef`).

### 2. Iniciar Base de Datos (Opcional con Docker)
Si utilizas Docker en Linux/Windows/Mac, puedes levantar SQL Server con un solo comando desde la carpeta `Project-Csharp`:

```bash
docker compose up -d
```

### 3. Configurar la Cadena de Conexión
Verifica la cadena `MyConnection` en `Baikuredo.Web/appsettings.json`:

- **Para Docker / SQL Server con usuario `sa`:**
  ```json
  "ConnectionStrings": {
    "MyConnection": "Server=localhost,1433;Database=BaikuredoDb;User Id=sa;Password=BaikuredoPassword123!;TrustServerCertificate=True;MultipleActiveResultSets=true;"
  }
  ```
- **Para Windows con SQL Server Express / LocalDB:**
  ```json
  "ConnectionStrings": {
    "MyConnection": "Server=localhost\\SQLEXPRESS;Database=BaikuredoDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;"
  }
  ```

### 4. Aplicar Migraciones y Ejecutar
Desde la carpeta `Project-Csharp`, ejecuta:

```bash
# 1. Restaurar paquetes NuGet
dotnet restore Baikuredo.sln

# 2. Crear / actualizar la base de datos con las migraciones
dotnet ef database update --project Baikuredo.Web

# 3. Iniciar la aplicación
dotnet run --project Baikuredo.Web
```

> 💡 **Poblado automático (`SeedDb`):** Al arrancar por primera vez, la aplicación inserta automáticamente los permisos, roles, usuarios de prueba, categorías, marcas, modelos, propietarios y motocicletas iniciales.

---

## 👥 Usuarios de Prueba (Creados por el Seeder)

Todos los usuarios iniciales tienen la contraseña por defecto: **`1234`**

| Rol | Correo Electrónico | Contraseña | Permisos y Alcance |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@baikuredo.com` | `1234` | **Acceso total** a todos los módulos del sistema, incluyendo Usuarios, Roles, Permisos y Bitácora de Auditoría. |
| **Auxiliar** | `auxiliar@baikuredo.com` | `1234` | **Gestión operativa y comercial:** Categorías, Marcas, Modelos, Propietarios, Motos, Publicaciones, Reservas e Interesados. *(No accede a Usuarios, Roles ni Auditoría)*. |
| **Cliente** | `cliente@baikuredo.com` | `1234` | **Usuario estándar:** Acceso a inicio, catálogo público, gestión de su propio perfil (`Mi cuenta`) y cambio de contraseña. |
| **Anónimo (Sin sesión)** | *No requiere login* | — | Acceso libre a **Inicio** y **Catálogo** (`/Catalog`) para ver motos disponibles, galería de fotos y registrar su interés en una publicación. |
