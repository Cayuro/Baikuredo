# BAIKUREDO - バイクレード

# 📚 PrivateBlog — Curso Explicativo + README desde Cero

> Proyecto ASP.NET Core MVC (.NET 10) con Entity Framework Core, AutoMapper, paginación genérica y patrón de servicios.

---

## 🗺️ Índice

1. [Arquitectura general del proyecto](#1-arquitectura-general-del-proyecto)
2. [Prerrequisitos y setup inicial](#2-prerrequisitos-y-setup-inicial)
3. [NuGet Packages — por qué cada uno](#3-nuget-packages--por-qué-cada-uno)
4. [Módulo 1 — La Entidad y el DbContext](#4-módulo-1--la-entidad-y-el-dbcontext)
5. [Módulo 2 — Migrations con EF Core](#5-módulo-2--migrations-con-ef-core)
6. [Módulo 3 — DTOs y por qué no exponer la entidad directamente](#6-módulo-3--dtos-y-por-qué-no-exponer-la-entidad-directamente)
7. [Módulo 4 — El patrón Response<T>](#7-módulo-4--el-patrón-responset)
8. [Módulo 5 — AutoMapper](#8-módulo-5--automapper)
9. [Módulo 6 — El servicio genérico CustomQueryableOperationsService](#9-módulo-6--el-servicio-genérico-customqueryableoperationsservice)
10. [Módulo 7 — Paginación](#10-módulo-7--paginación)
11. [Módulo 8 — ISectionsService + SectionsService](#11-módulo-8--isectionsservice--sectionsservice)
12. [Módulo 9 — Inyección de dependencias y CustomConfiguration](#12-módulo-9--inyección-de-dependencias-y-customconfiguration)
13. [Módulo 10 — El Controller](#13-módulo-10--el-controller)
14. [Flujo completo de una petición](#14-flujo-completo-de-una-petición)
15. [Replicar el proyecto desde cero — paso a paso](#15-replicar-el-proyecto-desde-cero--paso-a-paso)

---

## 1. Arquitectura general del proyecto

```
PrivateBlog/
├── PrivateBlog.sln
└── PrivateBlog.Web/           ← único proyecto (MVC Web App)
    ├── Program.cs             ← punto de entrada
    ├── CustomConfiguration.cs ← registro de servicios separado
    ├── appsettings.json       ← cadena de conexión
    │
    ├── Data/                  ← capa de datos
    │   ├── DataContext.cs     ← DbContext (EF Core)
    │   ├── Abstractions/
    │   │   └── IId.cs         ← interfaz que obliga a tener Guid Id
    │   └── Entities/
    │       └── Section.cs     ← entidad de base de datos
    │
    ├── DTOs/                  ← objetos de transferencia de datos
    │   └── Section/
    │       ├── SectionDTO.cs
    │       ├── CreateSectionDTO.cs
    │       ├── UpdateSectionDTO.cs
    │       └── ToggleSectionStatusDTO.cs
    │
    ├── Core/                  ← utilidades transversales
    │   ├── Response.cs        ← wrapper genérico de respuesta
    │   ├── AutoMapperProfiles.cs
    │   ├── Extensions/
    │   │   └── QueryableExtensions.cs  ← .Skip().Take() como extension method
    │   └── Pagination/
    │       ├── PaginationRequest.cs
    │       ├── PaginationResponse.cs
    │       └── PagedList.cs
    │
    ├── Services/              ← lógica de negocio
    │   ├── CustomQueryableOperationsService.cs  ← servicio genérico base
    │   ├── Abstractions/
    │   │   └── ISectionsService.cs     ← contrato (interfaz)
    │   └── Implementations/
    │       └── SectionsService.cs      ← implementación concreta
    │
    ├── Controllers/
    │   ├── HomeController.cs
    │   └── SectionsController.cs
    │
    └── Views/
        ├── Home/
        ├── Sections/
        └── Shared/
```

### ¿Por qué esta estructura?

El proyecto sigue el principio de **Separación de Responsabilidades (SoC)**:
- Los **Controllers** solo reciben peticiones HTTP y delegan al servicio.
- Los **Services** contienen la lógica de negocio.
- El **DataContext** es el único que habla con la base de datos.
- Los **DTOs** son lo que viaja entre capas (nunca se expone la entidad cruda).

---

## 2. Prerrequisitos y setup inicial

| Herramienta | Versión mínima | Para qué |
|---|---|---|
| .NET SDK | 10.0 | Runtime y compilador |
| Visual Studio / Rider / VS Code | cualquiera | IDE |
| SQL Server | 2019+ | Base de datos |
| dotnet-ef (CLI) | 10.0 | Crear y aplicar migrations |

### Instalar la herramienta EF CLI globalmente
```bash
dotnet tool install --global dotnet-ef
```

### Crear la solución y el proyecto desde cero
```bash
# Crear carpeta de la solución
mkdir MiBlog && cd MiBlog

# Crear el archivo .sln
dotnet new sln -n MiBlog

# Crear proyecto MVC
dotnet new mvc -n MiBlog.Web

# Agregar el proyecto a la solución
dotnet sln add MiBlog.Web/MiBlog.Web.csproj
```

---

## 3. NuGet Packages — por qué cada uno

```bash
cd MiBlog.Web

# Entity Framework Core — ORM para comunicarse con SQL Server
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools   # para migrations

# AutoMapper — mapear entidades ↔ DTOs automáticamente
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# Toast Notifications — mostrar mensajes de éxito/error en la UI
dotnet add package AspNetCoreHero.ToastNotification
```

| Package | Rol en el proyecto |
|---|---|
| `EF Core` | Traduce tus clases C# a tablas SQL y viceversa |
| `EF Core.SqlServer` | Driver específico de SQL Server |
| `EF Core.Tools` | Comandos `dotnet ef migrations add` y `dotnet ef database update` |
| `AutoMapper` | Evita escribir `dto.Name = entity.Name` a mano para cada propiedad |
| `AspNetCoreHero.ToastNotification` | Mensajes toast (popup) en la esquina inferior |

---

## 4. Módulo 1 — La Entidad y el DbContext

### 4.1 La interfaz `IId`

```csharp
// Data/Abstractions/IId.cs
public interface IId
{
    public Guid Id { get; set; }
}
```

**¿Por qué existe?**  
El servicio genérico (`CustomQueryableOperationsService`) necesita saber que *cualquier entidad* tiene un `Guid Id`. En lugar de repetir esa propiedad en cada entidad, se crea una interfaz que la garantiza. Esto permite escribir código genérico como:

```csharp
TEntity? entity = await _context.Set<TEntity>()
    .FirstOrDefaultAsync(e => e.Id == id); // solo funciona porque TEntity : IId
```

> [!IMPORTANT]
> Se usa `Guid` (identificador único universal) en lugar de `int` para el Id. Los Guids son globalmente únicos, no necesitan autoincremento en la base de datos, y son más seguros de exponer en URLs (un int `1, 2, 3` es predecible; un Guid no).

### 4.2 La entidad `Section`

```csharp
// Data/Entities/Section.cs
public class Section : IId      // implementa IId → tiene Guid Id garantizado
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(32)]              // máximo 32 chars en la columna SQL
    public required string Name { get; set; }

    [MaxLength(128)]
    public string? Description { get; set; }  // ? = nullable (puede ser null)

    public bool IsHidden { get; set; } = false;
}
```

**Atributos Data Annotations:**
- `[Key]` → le dice a EF que esta propiedad es la Primary Key.
- `[MaxLength(N)]` → EF crea la columna como `NVARCHAR(N)` en SQL Server, no `NVARCHAR(MAX)`.
- `required` → el compilador de C# 11+ garantiza que no puedas crear un `Section` sin `Name`.
- `string?` → el `?` indica que la propiedad **puede ser null** (C# Nullable Reference Types activado).

### 4.3 El `DataContext`

```csharp
// Data/DataContext.cs
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Section> Sections { get; set; }
}
```

**¿Qué es `DbContext`?**  
Es la clase principal de Entity Framework. Representa la sesión con la base de datos. Hereda de `DbContext` que ya sabe cómo conectarse, hacer queries, trackear cambios y guardar.

**¿Qué es `DbSet<Section>`?**  
Es la representación de la tabla `Sections` en C#. Con `_context.Sections` puedes hacer LINQ queries que EF traduce a SQL:

```csharp
// LINQ C#  →  EF Core lo traduce a  →  SQL
_context.Sections.Where(s => s.IsHidden == false)
// SELECT * FROM Sections WHERE IsHidden = 0
```

**El constructor con `DbContextOptions`:**  
ASP.NET Core inyecta las opciones (cadena de conexión, proveedor SQL Server) desde `appsettings.json`. Tú nunca instancias `DataContext` directamente con `new`; el sistema lo hace por ti.

---

## 5. Módulo 2 — Migrations con EF Core

Las migrations son la forma de versionar el esquema de tu base de datos. EF Core lee tus entidades y genera el SQL necesario.

### Flujo de trabajo

```bash
# 1. Crear la primera migration (lee tus entidades y genera el SQL)
dotnet ef migrations add InitialCreate --project MiBlog.Web

# 2. Aplicar la migration a la base de datos
dotnet ef database update --project MiBlog.Web
```

Esto crea la tabla `Sections` con columnas `Id (uniqueidentifier)`, `Name (nvarchar(32))`, `Description (nvarchar(128))`, `IsHidden (bit)`.

### Cada vez que cambias una entidad

```bash
dotnet ef migrations add NombreDescriptivo
dotnet ef database update
```

> [!TIP]
> Nombra tus migrations con lo que hacen, por ejemplo: `AddPostEntity`, `AddIsHiddenToSection`, `RemoveObsoleteColumns`.

---

## 6. Módulo 3 — DTOs y por qué no exponer la entidad directamente

**DTO = Data Transfer Object**. Son clases "planas" que solo tienen las propiedades necesarias para una operación específica.

### ¿Por qué no usar la entidad `Section` directamente en el Controller?

Si expusieras `Section` directamente:
- El usuario podría intentar enviar `Id`, `IsHidden` u otras propiedades que no debería controlar.
- No puedes tener validaciones diferentes para crear vs editar.
- Expones la estructura interna de la base de datos.

### Los 4 DTOs de `Section`

| DTO | Propósito | Tiene `Id`? |
|---|---|---|
| `CreateSectionDTO` | Crear una nueva sección | ❌ (el server genera el Id) |
| `SectionDTO` | Leer / mostrar una sección | ✅ (para identificarla) |
| `UpdateSectionDTO` | Editar una sección existente | ✅ (necesita saber cuál editar) |
| `ToggleSectionStatusDTO` | Solo cambiar `IsHidden` | ✅ + `bool Hide` |

### `CreateSectionDTO` — validaciones con Data Annotations

```csharp
public class CreateSectionDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MaxLength(32, ErrorMessage = "El campo {0} debe tener como máximo {1} caracteres.")]
    public string Name { get; set; }

    [MaxLength(128, ErrorMessage = "El campo {0} debe tener como máximo {1} caracteres.")]
    public string? Description { get; set; }

    public bool IsHidden { get; set; } = false;
}
```

- `{0}` → nombre del campo, `{1}` → el valor del parámetro (32, 128).
- MVC valida estas anotaciones automáticamente antes de llegar al action del Controller.

### `ToggleSectionStatusDTO` — DTO para operaciones específicas

```csharp
public class ToggleSectionStatusDTO
{
    [Required]
    public Guid Id { get; set; }
    public bool Hide { get; set; } = true;
}
```

En lugar de enviar el objeto completo para cambiar solo una propiedad, se crea un DTO mínimo. Esto es más eficiente y más explícito sobre la intención de la operación.

---

## 7. Módulo 4 — El patrón `Response<T>`

```csharp
// Core/Response.cs
public class Response<TResult>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public TResult? Result { get; set; }
    
    // Métodos estáticos de fábrica
    public static Response<TResult> Success(TResult result, string message = "...") { ... }
    public static Response<TResult> Success(string message = "...") { ... }
    public static Response<TResult> Failure(Exception ex, string message = "...") { ... }
    public static Response<TResult> Failure(string message, List<string>? errors = null) { ... }
}
```

### ¿Por qué este patrón?

Sin `Response<T>`, un servicio normalmente lanzaría una excepción al fallar o devolvería `null`. Eso obliga al Controller a usar try/catch o verificar nulls en todas partes.

Con `Response<T>`:

```csharp
// En el Controller — limpio, sin try/catch
Response<PaginationResponse<SectionDTO>> response = await _sectionsService.GetPaginationAsync(request);

if (!response.IsSuccess)
{
    _notyfService.Error(response.Message);    // mostrar toast de error
    return RedirectToAction("Index", "Home"); // redirigir
}

return View(response.Result); // datos están en .Result
```

### Los métodos de fábrica estáticos

En lugar de escribir siempre:
```csharp
return new Response<SectionDTO> { IsSuccess = true, Result = dto, Message = "OK" };
```

Se usan métodos estáticos que actúan como **constructores nombrados**:
```csharp
return Response<SectionDTO>.Success(dto, "Registro obtenido con éxito");
return Response<object>.Failure("No existe sección con ese id");
return Response<object>.Failure(ex); // captura el mensaje de la excepción
```

> [!NOTE]
> El comentario `// TODO: Remove in production` en el método `Failure(Exception ex)` indica que actualmente se expone el mensaje de la excepción (útil para debug). En producción se ocultaría para no filtrar información sensible.

---

## 8. Módulo 5 — AutoMapper

AutoMapper mapea propiedades de un objeto a otro automáticamente por nombre.

### El perfil de mapeo

```csharp
// Core/AutoMapperProfiles.cs
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Section, SectionDTO>()
            .ForMember(dto => dto.Name, entity => entity.MapFrom(s => s.Name))
            .ReverseMap();  // permite mapear en ambas direcciones
    }
}
```

**¿Por qué `.ForMember(...)`?**  
Aquí es redundante (ambas propiedades se llaman `Name`), pero sirve como ejemplo de cómo configurar mapeos especiales. Si la entidad tuviera `FullName` y el DTO `Name`, aquí lo especificarías.

**`.ReverseMap()`** → Genera automáticamente el mapeo `SectionDTO → Section` también.

### Cómo se usa en el servicio

```csharp
// Sin AutoMapper (manual, tedioso):
Section entity = new Section 
{
    Name = dto.Name,
    Description = dto.Description,
    IsHidden = dto.IsHidden
};

// Con AutoMapper (una línea):
Section entity = _mapper.Map<Section>(dto);
```

### Registro en DI

```csharp
// En CustomConfiguration.cs
builder.Services.AddAutoMapper(typeof(Program));
// Le dice a AutoMapper: "busca todos los Profile en el assembly del Program.cs"
// Encuentra AutoMapperProfiles y la registra automáticamente
```

---

## 9. Módulo 6 — El servicio genérico `CustomQueryableOperationsService`

Esta es una de las decisiones de diseño más interesantes del proyecto.

### El problema que resuelve

Sin este servicio genérico, para cada entidad tendrías que repetir el mismo código de CRUD:

```csharp
// SectionsService sin genérico — código repetido
public async Task<Response<SectionDTO>> GetOneAsync(Guid id)
{
    Section? section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == id);
    if (section is null) return Response<SectionDTO>.Failure(...);
    return Response<SectionDTO>.Success(_mapper.Map<SectionDTO>(section));
}

// PostsService sin genérico — misma lógica duplicada
public async Task<Response<PostDTO>> GetOneAsync(Guid id)
{
    Post? post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
    if (post is null) return Response<PostDTO>.Failure(...);
    return Response<PostDTO>.Success(_mapper.Map<PostDTO>(post));
}
```

### La solución — Generics en C#

```csharp
public class CustomQueryableOperationsService
{
    // Un solo método que funciona para CUALQUIER entidad que implemente IId
    public async Task<Response<TDto>> GetOneAsync<TDto, TEntity>(Guid id)
        where TEntity : class, IId
    {
        TEntity? entity = await _context.Set<TEntity>()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity is null)
            return Response<TDto>.Failure($"No existe registro con id {id}");

        TDto dto = _mapper.Map<TDto>(entity);
        return Response<TDto>.Success(dto, "Registro obtenido con éxito");
    }
}
```

- `_context.Set<TEntity>()` → EF busca el `DbSet` correspondiente a `TEntity` (ej: si pasas `Section`, usa `Sections`).
- `where TEntity : class, IId` → restricción de tipo: `TEntity` debe ser clase Y tener la interfaz `IId`.

### El método `CreateAsync` — por qué se asigna el Id aquí

```csharp
public async Task<Response<TDto>> CreateAsync<TDto, TEntity>(TDto dto) where TEntity : IId
{
    TEntity entity = _mapper.Map<TEntity>(dto);

    Guid id = Guid.NewGuid();   // ← el servidor genera el Id
    entity.Id = id;             // ← se asigna al entity, no al dto

    await _context.AddAsync(entity);
    await _context.SaveChangesAsync();
    
    return Response<TDto>.Success(dto, "Entidad creada con éxito");
}
```

> [!IMPORTANT]
> El `Id` **no viene del cliente** (DTO no tiene Id en `CreateSectionDTO`). El servidor lo genera con `Guid.NewGuid()`. Esto previene que un cliente malicioso fije un Id específico.

### El método `UpdateAsync` — `EntityState.Modified`

```csharp
public async Task<Response<TDto>> UpdateAsync<TDto, TEntity>(TDto dto, Guid id) where TEntity : IId
{
    TEntity entity = _mapper.Map<TEntity>(dto);
    entity.Id = id;

    _context.Entry(entity).State = EntityState.Modified;  // ← ojo aquí

    await _context.SaveChangesAsync();
    ...
}
```

**¿Por qué `EntityState.Modified` en lugar de `.Update()`?**  
EF Core tiene dos formas de actualizar:
- `_context.Update(entity)` → marca la entidad como modificada Y hace tracking.
- `_context.Entry(entity).State = EntityState.Modified` → es más explícito, no requiere que EF ya esté rastreando ese objeto. Útil cuando el objeto fue creado fuera del contexto (como aquí, donde viene de AutoMapper).

---

## 10. Módulo 7 — Paginación

La paginación evita cargar toda la base de datos de una vez. Implementada con 3 clases + 1 extension method.

### Flujo

```
Request HTTP: /Sections?Page=2&RecordsPerPage=15
     ↓
PaginationRequest { Page=2, RecordsPerPage=15 }
     ↓
QueryableExtensions.PaginateAsync() → .Skip(15).Take(15)  [página 2]
     ↓
PagedList<T> → lista de items + metadatos (total, páginas, etc.)
     ↓
PaginationResponse<T> → lo que recibe la View
```

### `PaginationRequest` — con validación integrada

```csharp
public class PaginationRequest
{
    private int _page = 1;
    private int _recordsPerPage = 15;
    private const int MAX_RECORDS_PER_PAGE = 50;

    public int Page
    {
        get => _page;
        set => _page = value > 0 ? value : 1;  // nunca puede ser 0 o negativo
    }

    public int RecordsPerPage
    {
        get => _recordsPerPage;
        set => _recordsPerPage = value <= MAX_RECORDS_PER_PAGE ? value : MAX_RECORDS_PER_PAGE;  // máximo 50
    }
    
    // Propiedad estática de conveniencia
    public static PaginationRequest Default => new PaginationRequest { Page = 1, RecordsPerPage = 15 };
}
```

> [!TIP]
> La validación en los setters (propiedades `Page` y `RecordsPerPage`) es **self-defending code**: aunque el cliente mande `Page = -5` o `RecordsPerPage = 9999`, la clase se auto-corrige. Esto evita que tengas que validar esto en cada controller.

### `QueryableExtensions` — extension method

```csharp
public static IQueryable<T> PaginateAsync<T>(this IQueryable<T> queryable, PaginationRequest request)
{
    return queryable.Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage);
}
```

`Skip((page-1) * size).Take(size)` es la fórmula clásica de paginación:
- Página 1: `Skip(0).Take(15)` → registros 1-15
- Página 2: `Skip(15).Take(15)` → registros 16-30
- Página 3: `Skip(30).Take(15)` → registros 31-45

> [!NOTE]
> El nombre `PaginateAsync` es un poco engañoso — el método no es `async`. Los extension methods en `IQueryable` aplican las operaciones de forma diferida (lazy), el SQL real se ejecuta cuando llamas `.ToListAsync()` o `.CountAsync()`.

### `PagedList<T>` — hereda de `List<T>`

```csharp
public class PagedList<T> : List<T>  // ← ES una lista, pero con metadatos extras
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int RecordsPerPage { get; set; }
    public int TotalCount { get; set; }

    public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> queryable, PaginationRequest request)
    {
        int count = await queryable.CountAsync();   // 1 query para el total
        List<T> items = await queryable.PaginateAsync<T>(request).ToListAsync(); // 1 query para los datos
        return new PagedList<T>(items, count, request.Page, request.RecordsPerPage);
    }
}
```

**2 queries a la base de datos:** una para contar el total (necesario para calcular páginas totales) y otra para traer los datos de la página actual.

### `PaginationResponse<T>` — lo que recibe la View

```csharp
public class PaginationResponse<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int RecordsPerPage { get; set; }
    public string? Filter { get; set; }
    public int TotalCount { get; set; }
    public PagedList<T> List { get; set; } = new();

    // Propiedades calculadas — no necesitan ser guardadas
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
```

`HasPrevious` y `HasNext` son propiedades de solo lectura calculadas. La View las puede usar directamente para mostrar/ocultar los botones de "Anterior" / "Siguiente" sin hacer ningún cálculo.

---

## 11. Módulo 8 — `ISectionsService` + `SectionsService`

### La interfaz — el contrato

```csharp
public interface ISectionsService
{
    Task<Response<CreateSectionDTO>> CreateAsync(CreateSectionDTO dto);
    Task<Response<object>> DeleteAsync(Guid id);
    Task<Response<SectionDTO>> GetOneAsync(Guid id);
    Task<Response<PaginationResponse<SectionDTO>>> GetPaginationAsync(PaginationRequest request);
    Task<Response<SectionDTO>> UpdateAsync(SectionDTO dto);
    Task<Response<object>> ToggleAsync(ToggleSectionStatusDTO dto);
}
```

**¿Por qué una interfaz?**  
- **Testabilidad**: en tests puedes inyectar un `MockSectionsService` que implementa `ISectionsService` sin tocar la base de datos.
- **Desacoplamiento**: el Controller solo conoce la interfaz, no la implementación concreta.
- **Sustitución**: podrías cambiar `SectionsService` por otra implementación (ej: que use Redis) sin cambiar el Controller.

### La implementación — herencia del servicio genérico

```csharp
public class SectionsService : CustomQueryableOperationsService, ISectionsService
{
    // Hereda de CustomQueryableOperationsService para tener los métodos genéricos
    // Implementa ISectionsService para cumplir el contrato

    public async Task<Response<CreateSectionDTO>> CreateAsync(CreateSectionDTO dto)
    {
        return await CreateAsync<CreateSectionDTO, Section>(dto);
        // ↑ llama al método genérico de la clase base, especificando los tipos
    }

    public async Task<Response<object>> ToggleAsync(ToggleSectionStatusDTO dto)
    {
        // Toggle no está en la clase base porque es lógica específica de Section
        Section? section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (section is null) return Response<object>.Failure($"No existe sección con id: {dto.Id}");
        
        section.IsHidden = dto.Hide;
        _context.Sections.Update(section);
        await _context.SaveChangesAsync();
        
        return Response<object>.Success("Sección actualizada con éxito");
    }
}
```

**Decisión de diseño:** Las operaciones genéricas (CRUD básico) van en la clase base. Las operaciones específicas de cada entidad (como `Toggle`) van en el servicio concreto.

---

## 12. Módulo 9 — Inyección de dependencias y `CustomConfiguration`

### El problema de `Program.cs` inflado

En proyectos grandes, `Program.cs` puede tener 200+ líneas de registros de servicios. La solución es extraerlos a extension methods.

### `CustomConfiguration.cs` — extension methods sobre `WebApplicationBuilder`

```csharp
public static class CustomConfiguration
{
    // Extiende WebApplicationBuilder (la clase de Microsoft)
    public static WebApplicationBuilder AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        // EF Core + SQL Server
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"));
        });

        builder.Services.AddAutoMapper(typeof(Program));
        
        AddServices(builder);   // ← extrae los servicios a un método privado

        // Toast notifications
        builder.Services.AddNotyf(config =>
        {
            config.DurationInSeconds = 10;
            config.IsDismissable = true;
            config.Position = NotyfPosition.BottomRight;
        });

        return builder;
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISectionsService, SectionsService>();
        // Aquí irían todos los servicios de la app
    }

    public static WebApplication AddCustomWebApplicationConfiguration(this WebApplication app)
    {
        app.UseNotyf();
        return app;
    }
}
```

### El ciclo de vida de los servicios

```csharp
builder.Services.AddScoped<ISectionsService, SectionsService>();
//                └─────────────────────────────────────────
//                Scoped = una instancia por request HTTP
```

| Ciclo | Método | Cuándo usar |
|---|---|---|
| **Singleton** | `AddSingleton` | Una sola instancia en toda la app (ej: cache, configuración) |
| **Scoped** | `AddScoped` | Una instancia por request HTTP. **Usar para services con DbContext** |
| **Transient** | `AddTransient` | Nueva instancia cada vez que se pide. Para objetos stateless simples |

> [!IMPORTANT]
> `DataContext` de EF Core se registra como **Scoped** internamente por `AddDbContext`. Todos los servicios que lo usen (`SectionsService`, etc.) también deben ser **Scoped** para evitar problemas de threading.

### `Program.cs` — limpio gracias a la extensión

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.AddCustomConfiguration();  // ← una sola línea en lugar de 30

WebApplication app = builder.Build();
// ... middleware pipeline ...
app.AddCustomWebApplicationConfiguration();
app.Run();
```

---

## 13. Módulo 10 — El Controller

```csharp
public class SectionsController : Controller
{
    private readonly INotyfService _notyfService;     // toast notifications
    private readonly ISectionsService _sectionsService; // servicio

    // Constructor Injection — ASP.NET Core inyecta automáticamente
    public SectionsController(INotyfService notyfService, ISectionsService sectionsService)
    {
        _notyfService = notyfService;
        _sectionsService = sectionsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        PaginationRequest request = PaginationRequest.Default;  // página 1, 15 por página
        Response<PaginationResponse<SectionDTO>> response = await _sectionsService.GetPaginationAsync(request);

        if (!response.IsSuccess)
        {
            _notyfService.Error(response.Message);       // toast de error
            return RedirectToAction("Index", "Home");    // redirigir a Home
        }

        return View(response.Result);  // View recibe PaginationResponse<SectionDTO>
    }
}
```

**Responsabilidades del Controller:**
1. Recibir el request HTTP.
2. Llamar al servicio con los parámetros necesarios.
3. Verificar si la respuesta fue exitosa.
4. Si no: mostrar toast de error y redirigir.
5. Si sí: pasar el resultado a la View.

El Controller **nunca** contiene lógica de negocio ni queries de base de datos.

---

## 14. Flujo completo de una petición

```
Usuario navega a /Sections
        │
        ▼
SectionsController.Index()
        │ llama con PaginationRequest { Page=1, RecordsPerPage=15 }
        ▼
ISectionsService.GetPaginationAsync(request)
        │
        ▼
CustomQueryableOperationsService.GetPagedListAsync<SectionDTO, Section>(request)
        │
        ├─ _context.Set<Section>().AsQueryable()
        │        │ (IQueryable — solo construye el query, no lo ejecuta)
        │        ▼
        ├─ PagedList<Section>.ToPagedListAsync(query, request)
        │        │
        │        ├─ queryable.CountAsync()
        │        │   → SELECT COUNT(*) FROM Sections
        │        │
        │        └─ queryable.PaginateAsync(request).ToListAsync()
        │            → SELECT * FROM Sections ORDER BY ... OFFSET 0 ROWS FETCH NEXT 15 ROWS ONLY
        │
        ├─ _mapper.Map<PagedList<SectionDTO>>(list)
        │   → AutoMapper convierte cada Section en SectionDTO
        │
        └─ Response<PaginationResponse<SectionDTO>>.Success(paginationResponseDto)
                │
                ▼
SectionsController recibe Response
        │ response.IsSuccess == true
        ▼
return View(response.Result)
        │
        ▼
Views/Sections/Index.cshtml renderiza la tabla con los datos
```

---

## 15. Replicar el proyecto desde cero — paso a paso

### Paso 1 — Crear el proyecto

```bash
mkdir MiBlog && cd MiBlog
dotnet new sln -n MiBlog
dotnet new mvc -n MiBlog.Web
dotnet sln add MiBlog.Web/MiBlog.Web.csproj
```

### Paso 2 — Instalar paquetes

```bash
cd MiBlog.Web
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.0
dotnet add package AutoMapper --version 12.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add package AspNetCoreHero.ToastNotification --version 1.1.0
```

### Paso 3 — Configurar la cadena de conexión

En `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "MyConnection": "Server=.\\TUSERVIDOR;Database=MiBlog;User Id=tuusuario;Password=tupassword;encrypt=false;"
  }
}
```

### Paso 4 — Crear las carpetas

```bash
mkdir -p Data/Abstractions Data/Entities
mkdir -p DTOs/Section
mkdir -p Core/Pagination Core/Extensions
mkdir -p Services/Abstractions Services/Implementations
```

### Paso 5 — Crear los archivos en orden

**Orden recomendado** (de menor a mayor dependencia):

1. `Data/Abstractions/IId.cs`
2. `Data/Entities/Section.cs`
3. `Data/DataContext.cs`
4. `Core/Pagination/PaginationRequest.cs`
5. `Core/Pagination/PaginationResponse.cs`
6. `Core/Extensions/QueryableExtensions.cs`
7. `Core/Pagination/PagedList.cs`
8. `Core/Response.cs`
9. `DTOs/Section/SectionDTO.cs`
10. `DTOs/Section/CreateSectionDTO.cs`
11. `DTOs/Section/UpdateSectionDTO.cs`
12. `DTOs/Section/ToggleSectionStatusDTO.cs`
13. `Core/AutoMapperProfiles.cs`
14. `Services/CustomQueryableOperationsService.cs`
15. `Services/Abstractions/ISectionsService.cs`
16. `Services/Implementations/SectionsService.cs`
17. `CustomConfiguration.cs`
18. Actualizar `Program.cs`
19. `Controllers/SectionsController.cs`

### Paso 6 — Crear la Migration y la base de datos

```bash
# Desde la carpeta MiBlog.Web
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Paso 7 — Ejecutar

```bash
dotnet run
```

---

## 📋 Resumen de decisiones de diseño

| Decisión | Alternativa | ¿Por qué esta? |
|---|---|---|
| `Guid` como Primary Key | `int` autoincremental | Más seguro, globalmente único, no predecible |
| Interface `IId` | Clase base abstracta | Más flexible, permite implementación múltiple |
| Patrón `Response<T>` | Lanzar excepciones | Controllers más limpios, manejo de errores consistente |
| DTOs separados por operación | Un solo DTO | Validaciones específicas por operación, mejor control |
| Servicio genérico base | Repetir CRUD en cada servicio | DRY (Don't Repeat Yourself), fácil agregar nuevas entidades |
| `AddScoped` para servicios | `AddSingleton` / `AddTransient` | Compatible con el ciclo de vida del DbContext |
| Extension methods en `CustomConfiguration` | Todo en `Program.cs` | `Program.cs` limpio y legible |
| `EntityState.Modified` | `_context.Update()` | Explícito, funciona con entidades no trackeadas |

---

*Curso generado para PrivateBlog — ASP.NET Core 10 MVC con EF Core, AutoMapper y paginación genérica.*
