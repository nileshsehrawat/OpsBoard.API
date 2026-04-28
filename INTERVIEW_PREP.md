# 🎯 OpsBoard.API - Complete Backend Interview Preparation Guide

**Last Updated:** April 2026 | **Framework:** .NET 10.0 | **Architecture:** ASP.NET Core Web API

---

## 📋 Table of Contents

1. [Project Architecture Overview](#project-architecture-overview)
2. [.NET Core Fundamentals](#net-core-fundamentals)
3. [ASP.NET Core Web API](#aspnet-core-web-api)
4. [Razor Views vs Other View Engines](#razor-views-vs-other-view-engines-your-weak-point)
5. [Entity Framework Core](#entity-framework-core)
6. [Authentication & Authorization](#authentication--authorization)
7. [REST API Design](#rest-api-design)
8. [Database & SQL](#database--sql)
9. [Advanced .NET Concepts](#advanced-net-concepts)
10. [Project-Specific Q&A](#project-specific-qa)
11. [Common Interview Pitfalls](#common-interview-pitfalls)

---

## 🏗️ Project Architecture Overview

### OpsBoard.API Structure

Your project is an **ASP.NET Core REST API** with:

- **Framework:** .NET 10.0 (latest LTS)
- **Database:** SQLite (development), can use SQL Server/PostgreSQL in production
- **Authentication:** ASP.NET Core Identity
- **ORM:** Entity Framework Core
- **API Documentation:** Swagger/OpenAPI

### Project Layers

```
OpsBoard.API/
├── Controllers/          → HTTP request handlers
├── Models/               → Domain entities (Organization, Resource)
├── DTOs/                 → Data Transfer Objects (request/response)
├── Data/                 → Database Context & repositories
├── Migrations/           → EF Core schema migrations
└── Program.cs            → Application configuration & middleware setup
```

### Key Dependencies

```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.5" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" /> <!-- Swagger -->
```

---

## 🔧 .NET Core Fundamentals

### Q1: What is .NET and How Does It Differ from .NET Framework?

**Answer:**

| Feature | .NET Framework | .NET Core / .NET 5+ |
|---------|---|---|
| **Open Source** | No | Yes |
| **Cross-Platform** | Windows only | Windows, Linux, macOS |
| **Performance** | Slower | 2-3x faster |
| **Latest Version** | 4.8 (2019) | 10.0 (2025) |
| **Deployment** | Framework required | Self-contained possible |
| **Use Case** | Legacy monoliths | Modern cloud-native apps |

**You would say:**
> ".NET (modern) is a cross-platform, open-source runtime that supports Windows, Linux, and macOS. Our OpsBoard.API uses .NET 10.0, which means it can run on any OS. The older .NET Framework was Windows-only and is now in maintenance mode."

---

### Q2: Explain the CLR and JIT Compilation

**Answer:**

```
C# Source Code
    ↓
Compiler (csc.exe)
    ↓
Intermediate Language (IL) / MSIL
    ↓
Just-In-Time (JIT) Compiler at runtime
    ↓
Native Machine Code (CPU-specific)
    ↓
Execution
```

**Key Points:**
- **JIT Compilation:** First time a method runs → JIT compiles it to native code
- **Performance Trade-off:** First call slower, subsequent calls fast
- **CLR Benefits:** Memory management, garbage collection, type safety

**In context of OpsBoard.API:**
> "When our API starts, Program.cs runs and the CLR loads assemblies. When an HTTP request hits AuthController.Register(), the JIT compiler converts that C# to machine code if it hasn't been already."

---

### Q3: What is a Namespace and Why Do We Use Them?

**Answer:**

```csharp
namespace OpsBoard.API.Models;  // Your project file-scoped namespace

public class Organization
{
    public int Id { get; set; }
}
```

**Purpose:**
- Organize code logically
- Avoid naming conflicts
- Structure: `CompanyName.ProductName.Layer`
- Your project uses: `OpsBoard.API.{Models|Controllers|Data|DTOs}`

**Interview Answer:**
> "Namespaces prevent naming collisions. In OpsBoard.API, we organize by feature layer. You can have an `Organization` class in Models and another in DTOs without conflict because they're in `OpsBoard.API.Models` and `OpsBoard.API.DTOs`."

---

### Q4: Explain Nullable Reference Types (NRT)

**Answer:**

In your .csproj:
```xml
<Nullable>enable</Nullable>  <!-- Enabled in OpsBoard.API -->
```

**What it does:**
```csharp
// WITHOUT NRT (nullable is allowed)
public string Name { get; set; }  // Can be null (dangerous!)

// WITH NRT (null-safe)
public required string Name { get; set; }  // Cannot be null
public string? OptionalValue { get; set; }  // Explicitly nullable with ?
```

**Interview Answer:**
> "Nullable Reference Types help prevent null reference exceptions at compile time. In OpsBoard.API, we have `<Nullable>enable</Nullable>`, so all strings must be explicitly marked with `?` if they can be null. This catches bugs early."

---

### Q5: What Are Implicit Usings?

**In your .csproj:**
```xml
<ImplicitUsings>enable</ImplicitUsings>
```

**What it does:**
```csharp
// You don't need to write:
// using System;
// using System.Collections.Generic;
// using Microsoft.AspNetCore.Mvc;

// They're automatically included by the compiler!

[ApiController]  // Can use this directly (from Microsoft.AspNetCore.Mvc)
public class AuthController : ControllerBase
{
}
```

**Interview Answer:**
> "Implicit Usings reduce boilerplate by automatically including common namespaces. Instead of writing 15 using statements, they're included based on your project type (Web API, Console, etc.). Saves time and cleaner code."

---

## 🌐 ASP.NET Core Web API

### Q6: What is ASP.NET Core and How Is It Different from ASP.NET Framework?

**Answer:**

| Aspect | ASP.NET Framework | ASP.NET Core |
|--------|---|---|
| **Architecture** | Monolithic (MVC + Web Forms) | Modular (minimal APIs, Controllers) |
| **Hosting** | IIS only | Self-hosted (Kestrel) or IIS |
| **Performance** | ~5000 req/sec | ~100,000+ req/sec |
| **Your Project** | ❌ | ✅ Using ASP.NET Core 10.0 |

**OpsBoard.API uses ASP.NET Core because:**
- ✅ Cross-platform (Linux, macOS, Windows)
- ✅ High performance for API endpoints
- ✅ Built-in dependency injection
- ✅ Modern middleware pipeline

---

### Q7: Explain the Request Pipeline in Program.cs

**Your Program.cs breakdown:**

```csharp
var builder = WebApplication.CreateBuilder(args);
// ↑ Create builder to register services

// DEPENDENCY INJECTION CONTAINER (Services)
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=opsboard.db"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
// ↑ Register what the app needs

var app = builder.Build();
// ↑ Build the middleware pipeline

// MIDDLEWARE PIPELINE (Executes in order for every request)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();              // Enable Swagger UI
    app.UseSwaggerUI();
}
// app.UseHttpsRedirection();     // Force HTTPS (commented out)
app.UseRouting();                  // Determine which endpoint
app.UseAuthorization();            // Check permissions
app.MapControllers();              // Route to controller actions

app.Run();
// ↑ Start the server
```

**Request Flow:**
```
HTTP Request
    ↓
Middleware 1 (Swagger)
    ↓
Middleware 2 (Routing)
    ↓
Middleware 3 (Authentication)
    ↓
Middleware 4 (Authorization)
    ↓
Controller Action
    ↓
Response
```

**Interview Answer:**
> "Program.cs configures the app in two phases: (1) Dependency Injection setup with AddXXX methods, (2) Middleware pipeline with UseXXX methods. Middleware intercepts every request. In OpsBoard.API, requests go through Swagger middleware first, then routing, then auth checks, then hit the controller action."

---

### Q8: What Are Controllers and Why Do We Use Them?

**Your AuthController example:**

```csharp
[ApiController]              // Indicates this is an API controller
[Route("api/auth")]          // All methods prefixed with /api/auth/
public class AuthController : ControllerBase  // Inherit from ControllerBase for APIs
{
    private readonly UserManager<IdentityUser> _userManager;

    [HttpPost("register")]   // POST /api/auth/register
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        
        return Ok("User created");
    }
}
```

**Why Controllers:**
- Organize endpoints logically (AuthController handles auth, ResourceController handles resources)
- Handle HTTP verbs (GET, POST, PUT, DELETE)
- Map URLs to methods via attributes
- Dependency injection integration

**Interview Answer:**
> "Controllers handle HTTP requests and return responses. In OpsBoard.API, AuthController handles login/register at /api/auth/*, ResourceController handles resources at /api/resources/*. They inherit from ControllerBase for APIs (not Controller, which includes view support)."

---

### Q9: Dependency Injection - How Does It Work in OpsBoard.API?

**Example from AuthController:**

```csharp
public AuthController(UserManager<IdentityUser> userManager,
                      SignInManager<IdentityUser> signInManager)
{
    _userManager = userManager;      // Injected by DI container
    _signInManager = signInManager;  // Injected by DI container
}
```

**Where are these registered?** In Program.cs:

```csharp
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// This automatically registers:
// - UserManager<IdentityUser>
// - SignInManager<IdentityUser>
// - And many others...
```

**How DI Works:**

```
1. Container asks: "I need UserManager<IdentityUser>, who has it?"
   ↓
2. Container looks up registration: AddIdentity() registered it
   ↓
3. Container instantiates UserManager and passes to constructor
   ↓
4. AuthController now has dependencies without "new" keyword
```

**Benefits:**
- ✅ Loose coupling (can swap implementations)
- ✅ Testable (can inject mock objects)
- ✅ Maintainable (centralized registration)

**Interview Answer:**
> "ASP.NET Core uses constructor injection. We register services in Program.cs with `builder.Services.AddXXX()`. When a controller needs a dependency, the DI container automatically instantiates it and passes it to the constructor. This makes code testable and loosely coupled."

---

## 👀 **Razor Views vs Other View Engines** (Your Weak Point - Study This!)

### Q10: What Are View Engines and Why Do We Need Them?

**Answer:**

A view engine converts server-side code into HTML sent to the browser.

**The Contenders:**

| View Engine | Syntax | Use Case | Status |
|---|---|---|---|
| **Razor** | `@` symbol, C# expressions | ASP.NET MVC/Core Web Apps | ✅ Standard |
| **Liquid** | `{{ variable }}`, Shopify-like | Templates, emails, CMS | Popular |
| **Handlebars** | `{{#if}} {{/if}}`, mustache-style | Universal, JavaScript-friendly | Popular |
| **Thymeleaf** | `th:*` attributes | Java web apps | Java world |
| **JSP (Java Server Pages)** | `<% %>` tags | Legacy Java | Outdated |
| **EJS** | `<%= %>` tags | Node.js/Express | JavaScript |
| **None (API Only)** | JSON only | REST APIs | ✅ OpsBoard.API uses this! |

---

### Q11: What is Razor and How Does It Work?

**Razor is Microsoft's view engine for ASP.NET Core.**

#### Syntax Features:

```html
<!-- Variable output -->
<p>Hello @Model.Name</p>

<!-- C# logic -->
@{
    int count = 5;
    bool isAdmin = User.IsInRole("Admin");
}

<!-- Conditionals -->
@if (Model.IsActive)
{
    <span>Active User</span>
}

<!-- Loops -->
@foreach (var item in Model.Items)
{
    <li>@item.Title</li>
}

<!-- HTML Encoding (automatic security) -->
<p>@Model.UserInput</p>  <!-- Automatically encoded if contains <script> -->

<!-- Expression shortcuts -->
<input value="@Model.Email" />  <!-- Implicitly closes @Model.Email -->
```

#### Razor Features:

```html
<!-- 1. Strongly typed (with @model directive) -->
@model OpsBoard.API.DTOs.OrganizationDto
<p>Organization: @Model.Name</p>

<!-- 2. Layout pages (Master pages equivalent) -->
@{
    Layout = "_Layout";
}

<!-- 3. Partial views (reusable components) -->
@await Html.PartialAsync("_NavBar", Model)

<!-- 4. HTML Helpers -->
@Html.DisplayFor(m => m.CreatedDate)
@Html.EditorFor(m => m.Email)

<!-- 5. Tag Helpers (modern, intuitive) -->
<form asp-action="Login" asp-controller="Auth" method="post">
    <input asp-for="Email" />
    <button type="submit">Submit</button>
</form>

<!-- 6. View Components (server-side widgets) -->
@await Component.InvokeAsync("RecentOrganizations")
```

---

### Q12: Razor vs Handlebars vs Liquid - Detailed Comparison

#### **Razor** (Microsoft, ASP.NET Core)

```csharp
// View (.cshtml file)
@model List<Organization>

<h1>Organizations</h1>
@foreach (var org in Model)
{
    <div>@org.Name (@org.Id)</div>
}

// Why OpsBoard.API doesn't use it:
// - REST APIs return JSON, not HTML
// - Razor is for server-rendered web apps
// - Razor requires DbContext, Models available in view (architectural tightly coupling)
```

#### **Liquid** (Shopify, Universal)

```liquid
{# Template syntax (no HTML helpers) #}
<h1>Organizations</h1>
{% for org in organizations %}
  <div>{{ org.name }} ({{ org.id }})</div>
{% endfor %}

{# Use Cases: #}
{# - Email templates #}
{# - CMS platforms #}
{# - Static site generators #}

{# Advantages: #}
{# - Shopify integrations #}
{# - Security (sandboxed, no arbitrary code) #}
{# - Simplicity #}
```

#### **Handlebars** (Universal, JavaScript-friendly)

```handlebars
{{!-- Mustache-like syntax --}}
<h1>Organizations</h1>
{{#each organizations}}
  <div>{{this.name}} ({{this.id}})</div>
{{/each}}

{{!-- Partials --}}
{{> organization-card org}}

{{!-- Helpers --}}
{{#if admin}}
  <button>Delete</button>
{{/if}}

{{!-- Use Cases: --}}
{{!-- - Frontend frameworks (Ember.js) --}}
{{!-- - Email templates (Nodemailer) --}}
{{!-- - Universal (works in JavaScript & backend) --}}
```

---

### Q13: **Why OpsBoard.API Doesn't Use Razor (Or Any View Engine)**

**Your project is a REST API, not a web app!**

```
Traditional Web App (uses Razor):
Browser → ASP.NET Core → Render HTML with Razor → HTML sent to browser → Browser displays

REST API (OpsBoard.API):
Client → ASP.NET Core → Return JSON → Client app renders (React, Vue, mobile app)
```

**Your Program.cs Evidence:**

```csharp
builder.Services.AddControllers();  // NOT AddControllersWithViews()
// ↑ This is API-only, no view engine

[ApiController]  // API attribute
[Route("api/auth")]
public class AuthController : ControllerBase  // ControllerBase, not Controller
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(string email, string password)
    {
        return Ok("User created");  // Returns JSON, not HTML view
    }
}
```

**If OpsBoard.API needed a web app:**

```csharp
// Would need to change to:
builder.Services.AddControllersWithViews();  // Adds Razor support
app.UseHttpsRedirection();

public class HomeController : Controller  // Controller, not ControllerBase
{
    public IActionResult Index()
    {
        return View();  // Returns HTML view, not JSON
    }
}
```

---

### Q14: When Should You Use Razor vs Others?

**Use Razor When:**
- ✅ Building server-rendered ASP.NET Core web applications
- ✅ Need tightly integrated C# and HTML
- ✅ Want Microsoft stack (AuthO, Entity Framework directly in views - though not recommended)
- ✅ Building traditional MVC web apps (not APIs)

**Don't Use Razor When:**
- ❌ Building REST APIs (like OpsBoard.API) - return JSON instead
- ❌ Need frontend-backend separation (modern architecture)
- ❌ Building mobile apps, SPAs (Single Page Apps)
- ❌ Need template sandboxing (Liquid is safer)

**Use Handlebars When:**
- ✅ Building universal templates (works in JavaScript and backend)
- ✅ Frontend framework (Ember.js)
- ✅ Email templates with client-side rendering option
- ✅ Need simple, readable template syntax

**Use Liquid When:**
- ✅ Shopify integrations
- ✅ Email templates (Sendgrid, Mailgun use Liquid)
- ✅ Need security sandboxing (no arbitrary code execution)
- ✅ CMS or platform templates

---

### Q15: **Interview Story - Razor vs Others**

**Practice saying this out loud:**

> "There are multiple view engines. Razor is specific to .NET, using `@` syntax for C# expressions. Handlebars is universal, using `{{}}` mustache syntax, popular in JavaScript frameworks. Liquid is Shopify-based, using `{%` logic blocks.
> 
> **Choosing between them:**
> - We use Razor in ASP.NET MVC web apps where the server renders HTML
> - OpsBoard.API is a REST API, so we don't use any view engine—we return JSON instead
> - If we needed email templates, we'd use Liquid for security
> - If we had a Node.js frontend, we'd use Handlebars for frontend-backend consistency
>
> **The key insight:** REST APIs and view engines don't mix. Modern architecture separates backend (REST API + JSON) from frontend (React, Vue, mobile app that consumes JSON)."

---

## 📊 Entity Framework Core

### Q16: What is Entity Framework Core?

**Answer:**

Entity Framework Core (EF Core) is an **Object-Relational Mapper (ORM)** for .NET.

**What it does:**

```csharp
// Without ORM (raw SQL - tedious, error-prone)
string query = "SELECT * FROM Organizations WHERE Id = @id";
using SqlCommand cmd = new SqlCommand(query, connection);
cmd.Parameters.AddWithValue("@id", 1);
SqlDataReader reader = cmd.ExecuteReader();
var org = new Organization { Id = 1, Name = reader["Name"].ToString() };

// With EF Core (LINQ - concise, safe, testable)
var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == 1);
```

**Benefits:**
- ✅ Compile-time SQL syntax checking
- ✅ Automatic SQL generation
- ✅ Lazy loading & eager loading
- ✅ Change tracking (knows what changed)
- ✅ Migrations (version control for database schema)

---

### Q17: DbContext - The Heart of EF Core

**Your AppDbContext:**

```csharp
public class AppDbContext : IdentityDbContext  // Inherits Identity tables
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Resource> Resources { get; set; }      // Maps to Resources table
    public DbSet<Organization> Organizations { get; set; } // Maps to Organizations table
}
```

**What it does:**

```csharp
using var context = new AppDbContext(options);

// Read
var org = context.Organizations.Find(1);  // SELECT * FROM Organizations WHERE Id = 1

// Create
var newOrg = new Organization { Name = "Acme Corp" };
context.Organizations.Add(newOrg);
context.SaveChanges();  // INSERT into Organizations

// Update
org.Name = "Updated Name";
context.SaveChanges();  // UPDATE Organizations SET Name = ...

// Delete
context.Organizations.Remove(org);
context.SaveChanges();  // DELETE FROM Organizations
```

**DbSet<T>:**
- Represents a table in the database
- Allows LINQ queries
- Tracks changes (you modify object, EF knows to update DB)

---

### Q18: Explain LINQ and Why It's Powerful

**LINQ = Language Integrated Query**

```csharp
// Traditional SQL
string sql = @"SELECT Name FROM Organizations 
              WHERE Id > @minId 
              ORDER BY Name";

// LINQ (compile-time safe, intellisense-enabled)
var names = context.Organizations
    .Where(o => o.Id > minId)        // Compile-time syntax check
    .OrderBy(o => o.Name)            // Intellisense available
    .Select(o => o.Name);
    
// EF Core translates LINQ to SQL automatically!
// Generated SQL: SELECT [Name] FROM [Organizations] WHERE [Id] > @p0 ORDER BY [Name]
```

**LINQ Advantages:**
- ✅ Compile-time verification (typos caught)
- ✅ Intellisense support
- ✅ Refactoring friendly
- ✅ Functional programming style

---

### Q19: Relationships in EF Core - Foreign Keys & Navigation

**Your Models:**

```csharp
public class Organization
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    // Navigation property (1-to-many)
    public List<Resource> Resources { get; set; } = new();
}

public class Resource
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int OrganizationId { get; set; }  // Foreign key
    public Organization Organization { get; set; }  // Navigation property
}
```

**Relationship Types:**

```csharp
// One-to-Many (Organization has many Resources)
var org = context.Organizations.First();
var resources = org.Resources;  // All Resources for this org

// Many-to-One (Resource belongs to one Organization)
var resource = context.Resources.First();
var org = resource.Organization;  // The owning Organization

// Many-to-Many (if needed)
public class User
{
    public int Id { get; set; }
    public List<Role> Roles { get; set; } = new();  // Join table auto-created
}
public class Role
{
    public int Id { get; set; }
    public List<User> Users { get; set; } = new();
}
```

---

### Q20: Migrations - Version Control for Databases

**Your Migrations folder contains:**

```
20260414084813_InitialCreate.cs
20260414084813_InitialCreate.Designer.cs
20260414125940_AddRelationships.cs
20260414125940_AddRelationships.Designer.cs
20260415132401_AddIdentity.cs
AppDbContextModelSnapshot.cs
```

**How Migrations Work:**

```bash
# Step 1: You modify a Model
public class Organization
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }  // NEW COLUMN
}

# Step 2: Create a migration
dotnet ef migrations add AddDescriptionToOrganization

# Generates a migration file:
```

```csharp
public partial class AddDescriptionToOrganization : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Organizations",
            type: "TEXT",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Description",
            table: "Organizations");
    }
}
```

```bash
# Step 3: Apply migration to database
dotnet ef database update

# Or revert
dotnet ef database update PreviousMigrationName
```

**Interview Answer:**
> "Migrations track database schema changes as C# code. When you modify your models, you create a migration that generates SQL to apply changes. This provides version control for your database, rollback capability, and a history of all schema changes."

---

## 🔐 Authentication & Authorization

### Q21: What is ASP.NET Core Identity?

**In your Program.cs:**

```csharp
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```

**What it does:**

```
User Credentials → IdentityUser validation → Tokens/Sessions → Authenticated requests
```

**Identity provides:**
- ✅ User registration & password hashing
- ✅ Login / logout management
- ✅ Password reset, email confirmation
- ✅ Role-based authorization (Admin, User, etc.)
- ✅ Automatic database tables (AspNetUsers, AspNetRoles, etc.)

**Your AuthController uses it:**

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register(string email, string password)
{
    var user = new IdentityUser { UserName = email, Email = email };
    var result = await _userManager.CreateAsync(user, password);  // Hashed!
    
    if (!result.Succeeded)
        return BadRequest(result.Errors);  // Password validation errors
    
    return Ok("User created");
}

[HttpPost("login")]
public async Task<IActionResult> Login(string email, string password)
{
    var result = await _signInManager.PasswordSignInAsync(
        email, password, 
        isPersistent: true,   // Remember me
        lockoutOnFailure: false);
    
    if (!result.Succeeded)
        return Unauthorized();
    
    return Ok("Logged in");
}
```

---

### Q22: Authentication vs Authorization

**Authentication (Who are you?)**
- ✅ User logs in with email/password
- ✅ System verifies credentials (hashed password comparison)
- ✅ User receives a token/session cookie
- ✅ Example: `login` endpoint in AuthController

**Authorization (What can you do?)**
- ✅ After authentication, does user have permission?
- ✅ Role-based (Admin, User, Moderator)
- ✅ Claim-based (specific permissions)
- ✅ Resource-based (can user access this organization?)
- ✅ Example: `[Authorize(Roles = "Admin")]`

```csharp
// AuthController - AUTHENTICATION
[HttpPost("login")]
public async Task<IActionResult> Login(string email, string password)
{
    // Verify who they are
    var result = await _signInManager.PasswordSignInAsync(email, password, true, false);
    return result.Succeeded ? Ok("Logged in") : Unauthorized();
}

// ResourceController - AUTHORIZATION
[Authorize]  // Must be authenticated
[HttpGet("{id}")]
public async Task<IActionResult> GetResource(int id)
{
    var resource = await _context.Resources.FindAsync(id);
    
    // Additional authorization: verify user owns this resource's organization
    if (resource.Organization.OwnerId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        return Forbid();  // 403 Forbidden
    
    return Ok(resource);
}
```

---

### Q23: Password Security - Why Hashing?

**Without hashing (NEVER DO THIS):**

```csharp
// ❌ BAD - passwords stored in plaintext
var hashedPassword = password;  // "MyPassword123"
await _context.Users.AddAsync(new User { Password = hashedPassword });

// If database leaked: hackers know everyone's passwords!
```

**With hashing (What Identity does):**

```csharp
// ✅ GOOD - passwords hashed with salt
var result = await _userManager.CreateAsync(user, password);
// Creates: $2a$11$abc123...xyz789 (can't be reversed)

// Login verification (also hashed)
var passwordMatches = await _userManager.CheckPasswordAsync(user, passwordAttempt);
// Both hashed, then compared
```

**Hash Properties:**
- ✅ One-way (can't decrypt)
- ✅ Same password = different hash each time (salt)
- ✅ Brute-force resistant (salting prevents rainbow tables)

---

### Q24: Cookies vs Tokens (Sessions vs JWT)

**Your Program.cs:**

```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;      // Not accessible via JavaScript
    options.Cookie.SameSite = SameSiteMode.Lax;  // CSRF protection
    options.ExpireTimeSpan = TimeSpan.FromDays(14);  // Cookie lifespan
    options.SlidingExpiration = true;    // Expiry resets on each request
});
```

**Cookies (Session-based):**

```
User logs in
    ↓
Server creates session, stores user data in-memory
    ↓
Server sends cookie to client: Set-Cookie: sessionId=xyz123
    ↓
Client automatically sends cookie with each request
    ↓
Server validates session still exists
    ↓
Grant access
```

**Stateful, server-heavy, not scalable for microservices**

**Tokens (JWT - JSON Web Tokens):**

```
User logs in
    ↓
Server signs a token with user data + secret key
    ↓
Server sends token to client (no server-side storage!)
    ↓
Client sends token with each request: Authorization: Bearer xyz123.abc456.def789
    ↓
Server verifies token signature (proves it wasn't tampered)
    ↓
Grant access
```

**Stateless, scalable, good for APIs & mobile**

**Your Project:**
- ✅ Uses cookie-based sessions (traditional, good for web apps)
- 🔄 Could upgrade to JWT for better API scalability

---

## 🔌 REST API Design

### Q25: What is REST and REST Principles?

**REST = Representational State Transfer**

**6 Principles:**

1. **Client-Server Architecture** - Client requests, Server responds
2. **Statelessness** - Each request contains all info needed (no session reliance)
3. **Uniform Interface** - Consistent, predictable API design
4. **Resource-Based URLs** - URLs are nouns, not verbs

| ❌ BAD (RPC-style) | ✅ GOOD (REST) |
|---|---|
| GET /api/getOrganizations | GET /api/organizations |
| POST /api/createOrganization | POST /api/organizations |
| POST /api/deleteOrganization?id=1 | DELETE /api/organizations/1 |

5. **HTTP Methods** - GET (read), POST (create), PUT (update), DELETE (remove)
6. **Cacheable** - Responses marked with cache headers

---

### Q26: HTTP Status Codes You Should Know

| Code | Meaning | When to Use |
|---|---|---|
| **200 OK** | Success | Request succeeded, data returned |
| **201 Created** | Created | POST succeeded, resource created |
| **204 No Content** | Success, no data | DELETE succeeded, nothing to return |
| **400 Bad Request** | Client error | Invalid request format/data |
| **401 Unauthorized** | Not authenticated | Login required |
| **403 Forbidden** | Authenticated but no permission | User verified but no access |
| **404 Not Found** | Resource doesn't exist | Endpoint or record missing |
| **500 Server Error** | Server broke | Unhandled exception |

**In OpsBoard.API:**

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register(string email, string password)
{
    var user = new IdentityUser { UserName = email, Email = email };
    var result = await _userManager.CreateAsync(user, password);
    
    if (!result.Succeeded)
        return BadRequest(result.Errors);  // 400
    
    return Ok("User created");  // 200 (should be 201 Created!)
}

// Better:
return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);  // 201
```

---

### Q27: RESTful Endpoint Design

**OpsBoard.API Resource Pattern:**

```
/api/organizations              GET   - List all organizations
/api/organizations              POST  - Create new organization
/api/organizations/{id}         GET   - Get specific organization
/api/organizations/{id}         PUT   - Update organization
/api/organizations/{id}         DELETE - Delete organization
/api/organizations/{id}/resources GET - List resources in org
```

**How to design:**

```csharp
[ApiController]
[Route("api/organizations")]
public class OrganizationController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // GET /api/organizations
        return Ok(await _context.Organizations.ToListAsync());
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationDto dto)
    {
        // POST /api/organizations
        var org = new Organization { Name = dto.Name };
        _context.Organizations.Add(org);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = org.Id }, org);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // GET /api/organizations/5
        var org = await _context.Organizations.FindAsync(id);
        return org != null ? Ok(org) : NotFound();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrganizationDto dto)
    {
        // PUT /api/organizations/5
        var org = await _context.Organizations.FindAsync(id);
        if (org == null) return NotFound();
        
        org.Name = dto.Name;
        await _context.SaveChangesAsync();
        return Ok(org);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // DELETE /api/organizations/5
        var org = await _context.Organizations.FindAsync(id);
        if (org == null) return NotFound();
        
        _context.Organizations.Remove(org);
        await _context.SaveChangesAsync();
        return NoContent();  // 204
    }
}
```

---

## 💾 Database & SQL

### Q28: What's the Difference Between SQL and NoSQL?

| Aspect | SQL (Relational) | NoSQL (Document/Key-Value) |
|---|---|---|
| **Structure** | Tables with rows/columns | JSON documents, key-value pairs |
| **Schema** | Rigid, predefined | Flexible, dynamic |
| **Transactions** | ACID guaranteed | Eventually consistent |
| **Query Language** | SQL | Collection-specific |
| **Scaling** | Vertical (bigger server) | Horizontal (more servers) |
| **OpsBoard.API** | ✅ SQLite (relational) | Could use MongoDB (NoSQL) |

**SQL Query:**
```sql
SELECT id, name FROM Organizations WHERE id = 1;
-- Structured, with schema

SELECT o.*, r.* FROM Organizations o
JOIN Resources r ON o.id = r.organization_id
WHERE o.id = 1;
```

**NoSQL Query:**
```javascript
// MongoDB
db.organizations.findOne({ _id: 1 });
// More flexible: fields can vary per document
```

---

### Q29: What is Normalization?

**Goal:** Eliminate data duplication and anomalies

**Your Schema (Normalized):**

```
Organizations Table:
ID | Name
1  | Acme Corp
2  | TechCorp

Resources Table:
ID | Name            | OrganizationId
1  | Server A        | 1
2  | Server B        | 1
3  | Database Prod   | 2
```

**❌ Denormalized (BAD - duplication):**

```
ID | OrgName   | ResourceName      | ResourceId
1  | Acme Corp | Server A          | 1
2  | Acme Corp | Server B          | 2
3  | TechCorp  | Database Prod     | 3
-- "Acme Corp" repeated - update nightmare!
```

**Benefits of Normalization:**
- ✅ Prevents update anomalies (change org name in one place)
- ✅ Saves storage (no duplication)
- ✅ Maintains data integrity
- ✅ Reduces redundancy

---

### Q30: N+1 Query Problem

**❌ Bad (N+1 Queries):**

```csharp
var organizations = _context.Organizations.ToList();  // 1 query

foreach (var org in organizations)
{
    var resources = org.Resources;  // N queries (one per org!)
    // If 100 organizations: 1 + 100 = 101 queries!
}
```

**✅ Good (Eager Loading):**

```csharp
var organizations = _context.Organizations
    .Include(o => o.Resources)  // Eager load (1 query with JOIN)
    .ToList();

foreach (var org in organizations)
{
    var resources = org.Resources;  // Already loaded, no extra query
}
```

**Performance Impact:**
- ❌ N+1: 100 orgs = 101 database calls (1 second+)
- ✅ Eager Load: 100 orgs = 1 database call (10ms)

---

## 🚀 Advanced .NET Concepts

### Q31: Async/Await - Why It Matters

**Synchronous (Blocking):**

```csharp
// Thread blocks, waiting for database
public IActionResult GetOrganization(int id)
{
    var org = _context.Organizations.FirstOrDefault(o => o.Id == id);  // WAITS
    return Ok(org);
}

// If 100 requests come in:
// Thread pool creates 100 threads, each blocked on I/O
// High memory, thread exhaustion
```

**Asynchronous (Non-blocking):**

```csharp
// Thread released while database query runs
public async Task<IActionResult> GetOrganization(int id)
{
    var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
    // Thread is freed to handle other requests!
    return Ok(org);
}

// If 100 requests come in:
// Same thread pool can handle all (thread reused while waiting)
// Low memory, better throughput
```

**Interview Answer:**
> "Async/await prevents thread starvation. In synchronous code, a thread blocks on I/O (database, API call), wasting it. With async, the thread is released to handle other requests while I/O completes. For I/O-heavy APIs like ours, async is essential for scalability."

**All your AuthController methods use async:**
```csharp
public async Task<IActionResult> Register(...)  // ← async keyword
public async Task<IActionResult> Login(...)     // ← async keyword
```

---

### Q32: Extension Methods

**Your project likely uses these:**

```csharp
// Extension method syntax: static class with static method, first param has "this"
public static class StringExtensions
{
    public static string Reverse(this string str)  // ← "this" makes it extension
    {
        return new string(str.Reverse().ToArray());
    }
}

// Usage (looks like it's a method on string):
string text = "Hello";
string reversed = text.Reverse();  // Hello → olleH

// This is what LINQ methods are!
var orgs = _context.Organizations
    .Where(o => o.Name.Contains("Corp"))    // Extension method
    .OrderBy(o => o.Name)                   // Extension method
    .ToList();                              // Extension method
```

**In .NET:**
```csharp
public static class DbSetExtensions
{
    // Add a custom extension for your DbSet
    public static IQueryable<Organization> ActiveOrganizations(this DbSet<Organization> dbSet)
    {
        return dbSet.Where(o => o.IsActive);
    }
}

// Usage:
var active = _context.Organizations.ActiveOrganizations().ToList();
```

---

### Q33: SOLID Principles

**S - Single Responsibility Principle**
- Each class does one thing
- AuthController handles auth (not resources)
- AppDbContext handles data (not business logic)

```csharp
// ❌ Bad: AuthController doing too much
public class AuthController
{
    public async Task<IActionResult> Register(string email, string password)
    {
        // 1. Validate email
        // 2. Hash password
        // 3. Create user
        // 4. Send email
        // 5. Log to file
        // 6. Update analytics
        // ^ Too many responsibilities!
    }
}

// ✅ Good: Separated concerns
public class AuthService
{
    public async Task<User> RegisterAsync(string email, string password)
    {
        ValidateEmail(email);
        var hashedPassword = HashPassword(password);
        return await _context.Users.AddAsync(new User { Email = email, Password = hashedPassword });
    }
}

public class AuthController
{
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = await _authService.RegisterAsync(email, password);
        await _emailService.SendWelcomeEmail(user.Email);  // Delegated
        return Ok(user);
    }
}
```

**O - Open/Closed Principle**
- Open for extension, closed for modification
- Use interfaces/inheritance instead of modifying existing code

```csharp
// ❌ Bad: Must modify class to add new functionality
public class AuthController
{
    public async Task Register(string email, string password)
    {
        if (email.Contains("@gmail"))
            // Gmail specific logic
        else if (email.Contains("@hotmail"))
            // Hotmail specific logic
        // Add new provider? Must modify this method!
    }
}

// ✅ Good: Extension via interface
public interface IEmailValidator
{
    Task<bool> ValidateAsync(string email);
}

public class GmailValidator : IEmailValidator { ... }
public class HotmailValidator : IEmailValidator { ... }
public class CustomValidator : IEmailValidator { ... }

public class AuthController
{
    private readonly IEmailValidator _validator;
    
    public async Task Register(string email, string password)
    {
        // Any validator works, no code change needed!
        var isValid = await _validator.ValidateAsync(email);
    }
}
```

**L - Liskov Substitution Principle**
- Derived classes must be substitutable for base classes

**I - Interface Segregation Principle**
- Clients shouldn't depend on interfaces they don't use

```csharp
// ❌ Bad: Fat interface
public interface IUserManager
{
    Task CreateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task SendEmailAsync(string to, string subject, string body);  // Not all implementations need this!
    Task LogToFileAsync(string message);  // Not all need this!
}

// ✅ Good: Segregated interfaces
public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task DeleteUserAsync(int id);
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public interface ILogger
{
    Task LogAsync(string message);
}
```

**D - Dependency Inversion Principle**
- Depend on abstractions, not concretions
- This is what your DI container does!

```csharp
// ❌ Bad: Tightly coupled to concrete class
public class AuthController
{
    private SqliteUserRepository _userRepository = new();  // Concrete!
    
    public async Task Register(string email, string password)
    {
        await _userRepository.CreateAsync(email, password);
    }
}
// Can't test with mock, can't switch databases

// ✅ Good: Depends on abstraction
public interface IUserRepository
{
    Task CreateAsync(string email, string password);
}

public class AuthController
{
    private readonly IUserRepository _userRepository;
    
    public AuthController(IUserRepository userRepository)  // Injected
    {
        _userRepository = userRepository;
    }
    
    public async Task Register(string email, string password)
    {
        await _userRepository.CreateAsync(email, password);
    }
}

// In Program.cs
builder.Services.AddScoped<IUserRepository, SqliteUserRepository>();
// Later: builder.Services.AddScoped<IUserRepository, PostgresUserRepository>();
```

---

### Q34: Design Patterns

**Singleton Pattern**
```csharp
// Register once, reuse forever
builder.Services.AddSingleton<IConfiguration>();

// Use case: AppDbContext should NOT be singleton (connection pooling issues)
builder.Services.AddScoped<AppDbContext>();  // New instance per request
```

**Factory Pattern**
```csharp
public class UserRepositoryFactory
{
    public IUserRepository CreateRepository(string dbType)
    {
        return dbType switch
        {
            "sqlite" => new SqliteUserRepository(),
            "postgres" => new PostgresUserRepository(),
            _ => throw new NotSupportedException()
        };
    }
}
```

**Repository Pattern** (Abstraction over data access)
```csharp
public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}

public class SqliteUserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    
    public async Task<User> GetByIdAsync(int id) =>
        await _context.Users.FindAsync(id);
}

// In controller:
public class AuthController
{
    private readonly IUserRepository _userRepository;
    
    public async Task Register(string email, string password)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
            return BadRequest("User exists");
        
        var user = new User { Email = email, Password = HashPassword(password) };
        await _userRepository.AddAsync(user);
    }
}
```

---

## ❓ Project-Specific Q&A

### Q35: Explain OpsBoard.API's Three-Layer Architecture

**1. Controller Layer (Presentation)**
```csharp
[ApiController]
[Route("api/organizations")]
public class OrganizationController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Handles HTTP, routing, status codes
        // Calls service layer
    }
}
```

**2. Service/Business Logic Layer** (Not in your project yet, but you should add)
```csharp
public interface IOrganizationService
{
    Task<OrganizationDto> GetAsync(int id);
    Task<OrganizationDto> CreateAsync(CreateOrganizationDto dto);
}

public class OrganizationService : IOrganizationService
{
    // Business logic, validation, orchestration
    // Calls data access layer
}
```

**3. Data Access Layer (Persistence)**
```csharp
public class AppDbContext : IdentityDbContext
{
    // Database queries, migrations, relationships
}

// Or with Repository pattern:
public interface IOrganizationRepository
{
    Task<Organization> GetAsync(int id);
    Task AddAsync(Organization org);
}
```

**Current Flow:**
```
HTTP Request → Controller → DbContext → Database → Response
```

**Improved Flow (recommended):**
```
HTTP Request → Controller → Service → Repository → DbContext → Database → Response
```

---

### Q36: What Does `IdentityDbContext` Add?

```csharp
public class AppDbContext : IdentityDbContext
{
    // IdentityDbContext automatically includes:
}
```

**Inherited Tables from IdentityDbContext:**

| Table | Purpose |
|---|---|
| AspNetUsers | Stores user accounts |
| AspNetRoles | Stores roles (Admin, User, etc.) |
| AspNetUserRoles | Junction table (many-to-many) |
| AspNetUserClaims | Custom user claims |
| AspNetUserLogins | External logins (Google, GitHub, etc.) |
| AspNetRoleClaims | Role-specific claims |
| AspNetUserTokens | API tokens, refresh tokens |

**Your additions:**
```csharp
public DbSet<Resource> Resources { get; set; }
public DbSet<Organization> Organizations { get; set; }
```

---

### Q37: CORS - Why Is It in Your Program.cs?

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000", "http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
```

**What is CORS?**
- Cross-Origin Resource Sharing
- Browser security: script from `localhost:3000` can't call `localhost:5000` by default

**Scenario:**
```
React App (localhost:3000) tries to fetch from API (localhost:5001)
    ↓
Browser: "Wait! Different origin. Need permission!"
    ↓
Browser sends OPTIONS request to API
    ↓
API responds: "Yes, localhost:3000 is allowed"
    ↓
Browser allows fetch request
```

**Security:**
- Without CORS, malicious site couldn't steal your data
- With CORS, you explicitly allow trusted origins

---

### Q38: Why Swagger in OpsBoard.API?

```csharp
builder.Services.AddSwaggerGen();
app.UseSwagger();
app.UseSwaggerUI();
```

**Benefits:**
- ✅ Interactive API documentation
- ✅ Test endpoints without Postman
- ✅ Auto-generated from controllers
- ✅ Live at `http://localhost:5000/swagger`

**What it generates:**
```
GET /api/organizations        List all organizations
POST /api/organizations       Create organization
GET /api/organizations/{id}   Get specific organization
...
```

---

## ⚠️ Common Interview Pitfalls

### Pitfall 1: Confusing Authentication & Authorization

```
You: "Our app uses authentication and authorization."
Interviewer: "What's the difference?"

❌ Bad Answer: "They're the same thing, for security."
✅ Good Answer: "Authentication verifies who you are (login). Authorization 
   checks what you can do (permissions). Our app authenticates with 
   Identity, then authorizes based on roles."
```

---

### Pitfall 2: Not Understanding Async/Await

```
Interviewer: "Why do you use async in all your controller actions?"

❌ Bad: "It makes code run faster."
✅ Good: "Async releases threads while waiting for I/O. In synchronous code, 
   a thread blocks on database query, wasting memory. With async, threads 
   handle multiple requests. For our API handling concurrent users, async 
   is essential for scalability."
```

---

### Pitfall 3: Forgetting about DTOs

```
Interviewer: "Why do you have separate DTOs from Models?"

❌ Bad: "DTOs are like models but different."
✅ Good: "Models represent database tables (Organization, Resource). DTOs 
   represent what we send/receive over HTTP. We keep them separate for:
   1. Security (don't expose internal fields)
   2. Contract stability (model changes don't break API)
   3. Validation (DTOs validate input before hitting database)"
```

---

### Pitfall 4: Missing Null Checks (Now with NRT!)

```csharp
// ❌ Bad (even with NRT)
var org = _context.Organizations.FirstOrDefault(o => o.Id == id);
return Ok(org);  // Might be null, API returns null
// Compiler warns because org is nullable

// ✅ Good
var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
if (org == null)
    return NotFound();
return Ok(org);
```

---

### Pitfall 5: N+1 Query Problem

```csharp
❌ Bad:
var orgs = _context.Organizations.ToList();
foreach (var org in orgs)
    foreach (var resource in org.Resources)  // N+1 queries!
        Print(resource.Name);

✅ Good:
var orgs = _context.Organizations
    .Include(o => o.Resources)  // Eager load
    .ToList();
foreach (var org in orgs)
    foreach (var resource in org.Resources)  // Already loaded
        Print(resource.Name);
```

---

### Pitfall 6: Razor Views Question (Your Original Weak Point!)

```
Interviewer: "Compare Razor views vs Handlebars vs Liquid"

❌ Bad: "Um... I don't know... they're all templating?"
✅ GOOD: "Razor is .NET's view engine using @-syntax, great for ASP.NET MVC 
   web apps. Handlebars uses {{}} mustache syntax, universal across 
   JavaScript frameworks. Liquid is Shopify's language, safer for 
   untrusted templates.
   
   OpsBoard.API is a REST API, so we don't use any view engine—we return 
   JSON. If we built a web UI, we'd use React (frontend framework) instead 
   of server-rendered Razor for better separation of concerns."
```

---

### Pitfall 7: Mixing Business Logic in Controllers

```csharp
❌ Bad (Tight Coupling):
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(string email, string password)
    {
        // 1. Validate email
        if (!Regex.IsMatch(email, @"^\S+@\S+$"))
            return BadRequest("Invalid email");
        
        // 2. Check if exists
        var existing = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
        if (existing != null)
            return BadRequest("Already registered");
        
        // 3. Hash password
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        
        // 4. Create user
        var user = new User { Email = email, PasswordHash = hash };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // 5. Hard to test, hard to reuse, hard to change
        return Ok(user);
    }
}

✅ Good (Service Layer):
public class AuthService
{
    public async Task<User> RegisterAsync(string email, string password)
    {
        ValidateEmail(email);
        if (await UserExistsAsync(email))
            throw new InvalidOperationException("User exists");
        
        var user = new User
        {
            Email = email,
            PasswordHash = HashPassword(password)
        };
        
        return await _userRepository.AddAsync(user);
    }
}

public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var user = await _authService.RegisterAsync(dto.Email, dto.Password);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
```

---

### Pitfall 8: Hardcoding Configuration

```csharp
❌ Bad:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")  // Hardcoded!
            .AllowAnyHeader()
            .AllowAnyMethod());
});

✅ Good:
// appsettings.json
{
    "Cors": {
        "AllowedOrigins": ["http://localhost:3000", "http://localhost:3001"]
    }
}

// Program.cs
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(allowedOrigins)  // From config
            .AllowAnyHeader()
            .AllowAnyMethod());
});
```

---

## 🎤 Interview Cheat Sheet - Quick Answers

### .NET Core / ASP.NET Core

1. **.NET vs .NET Framework?** 
   > Modern .NET is cross-platform, open-source, 2-3x faster. We use .NET 10.0 on OpsBoard.API.

2. **What's a CLR?**
   > Common Language Runtime. Executes IL code, manages memory, garbage collection, type safety.

3. **What's JIT compilation?**
   > Just-In-Time. Converts IL to machine code at runtime when method is first called.

4. **Async/Await benefit?**
   > Releases threads while waiting for I/O (database, API), allowing thread reuse. Better scalability.

5. **Dependency Injection?**
   > Inject dependencies via constructor instead of creating them. Loose coupling, testability. ASP.NET Core has built-in DI container.

### Entity Framework Core

6. **What's an ORM?**
   > Object-Relational Mapper. Translates objects to database queries. Allows LINQ queries instead of raw SQL.

7. **DbContext?**
   > Core of EF Core. Tracks entities, generates SQL, manages database connection.

8. **What's a migration?**
   > Version control for database schema. Track model changes as C# code, apply/rollback changes.

9. **Eager vs Lazy Loading?**
   > Eager: `.Include()` loads related data upfront. Lazy: Loads on access. Eager prevents N+1 queries.

10. **N+1 Problem?**
    > Executing 1 query to get parent, then N queries for each child. Use `.Include()` to load all at once.

### REST API

11. **What's REST?**
    > Architectural style. Resource-based URLs, HTTP methods (GET, POST, PUT, DELETE), stateless.

12. **Key HTTP Status Codes?**
    > 200 OK, 201 Created, 204 No Content, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found, 500 Server Error.

### Authentication & Authorization

13. **Authentication vs Authorization?**
    > Auth: Verify identity (login). Authz: Check permissions (roles/claims).

14. **What's Identity?**
    > ASP.NET Core's membership system. Handles user registration, password hashing, login, roles.

15. **Why hash passwords?**
    > One-way encryption. If DB leaks, passwords are useless. Identity automatically hashes with salt.

### Your Weak Point: View Engines

16. **Razor vs Handlebars vs Liquid?**
    > Razor: .NET @-syntax for ASP.NET MVC. Handlebars: {{}} universal syntax. Liquid: Shopify-style, safe.
    > OpsBoard.API is REST API → no views, returns JSON.

17. **When to use Razor?**
    > Server-rendered ASP.NET Core web apps (not APIs). OpsBoard.API doesn't need it.

### Advanced

18. **SOLID Principles?**
    > Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion.

19. **What's CORS?**
    > Cross-Origin Resource Sharing. Allows frontend (different origin) to call your API. Security feature.

20. **Migrations? Why not raw SQL?**
    > Version control for schema. Rollback capability. Type-safe. Track all changes. Database agnostic.

---

## 📝 Practice Questions for You

Try answering these out loud before checking answers:

1. **"Describe OpsBoard.API's architecture and how a request flows through it."**
   <details>
   <summary>Answer (click to reveal)</summary>
   "OpsBoard.API is an ASP.NET Core REST API. A request hits Program.cs middleware (Swagger, routing, auth), then routes to a controller action. Controllers return data from Entity Framework Core queries to AppDbContext. Data comes from SQLite database. We use Identity for authentication. DTOs represent what we send/receive over HTTP. Swagger documents all endpoints."
   </details>

2. **"Why use Entity Framework Core instead of raw SQL?"**
   <details>
   <summary>Answer (click to reveal)</summary>
   "EF Core provides ORM benefits: compile-time LINQ safety, automatic SQL generation, change tracking, migrations for schema versioning, and abstraction from database-specific SQL. Migrations especially are powerful—we can version and rollback schema changes like code."
   </details>

3. **"Explain the difference between using `.ToList()` and `.AsNoTracking().ToList()`"**
   <details>
   <summary>Answer (click to reveal)</summary>
   "`.ToList()` executes query and tracks changes—if you modify entities, SaveChanges() updates DB. `.AsNoTracking()` doesn't track, so modifications are ignored. Use AsNoTracking for read-only queries to save memory. For updates, keep tracking."
   </details>

4. **"Why do we configure CORS in Program.cs?"**
   <details>
   <summary>Answer (click to reveal)</summary>
   "CORS (Cross-Origin Resource Sharing) allows our frontend (localhost:3000) to call our API (localhost:5001). Without CORS, browsers block cross-origin requests for security. We whitelist trusted origins in configuration."
   </details>

5. **"How would you add a new feature: User Roles and Permissions?"**
   <details>
   <summary>Answer (click to reveal)</summary>
   "1. Add role claims to Organization model. 2. Create Role model + RoleDTO. 3. Add DbSet<Role> to AppDbContext. 4. Create migration: `dotnet ef migrations add AddRoles`. 5. Update AppDbContext to seed roles. 6. Add [Authorize(Roles = \"Admin\")] to protected controller actions. 7. Modify AuthController to assign roles on registration. 8. Add repository pattern to separate concerns."
   </details>

---

## 🎯 Final Tips for Your Interview

### Before the Interview
- [ ] Review this document 3-4 times
- [ ] Practice explaining concepts out loud (not just reading)
- [ ] Study your project code closely
- [ ] Prepare a 2-minute elevator pitch about OpsBoard.API
- [ ] Have examples ready (from your actual code)

### During the Interview
- ✅ **Ask clarifying questions** if prompt is vague
- ✅ **Think out loud** - explain your reasoning
- ✅ **Use specific examples** from OpsBoard.API
- ✅ **Admit if you don't know** - better than guessing
- ✅ **Pivot to what you DO know** - "I'm not familiar with that, but here's what I know about similar concepts..."

### The Razor Views Question (Your Nemesis!)
**If asked: "Compare Razor views vs other view engines"**

**Your Answer (say this):**
> "Razor is .NET's templating engine using @-syntax, designed for server-rendered ASP.NET MVC applications. Handlebars uses {{}} mustache syntax and is universal—works in JavaScript, Java, any language. Liquid is Shopify's language, commonly used in CMS and email templates because it's safe—no arbitrary code execution.
>
> The key point: **OpsBoard.API is a REST API, not a web application.** REST APIs return JSON data, not HTML views. So we don't use any view engine. If we were building a traditional server-rendered website, we'd use Razor. If we were building a JavaScript frontend (React, Vue), it would use its own templating, not Razor.
>
> Modern architectures separate frontend (React) from backend (REST API). Razor is old-school server-side rendering, which is outdated for scalable APIs."

**Why this works:**
- ✅ Shows understanding of each tool
- ✅ Explains when/why to use each
- ✅ Connects to your project (REST API = no views)
- ✅ Shows architectural awareness
- ✅ No freezing/awkwardness!

---

## 📚 Additional Resources

### Official Documentation
- [Microsoft Learn .NET](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Official Docs](https://learn.microsoft.com/en-us/aspnet/core)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Razor Documentation](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor)

### Topics to Research Further
- [ ] Unit testing with xUnit or MSTest
- [ ] Integration testing with testcontainers
- [ ] Logging (Serilog, Application Insights)
- [ ] Caching strategies (Redis)
- [ ] API versioning
- [ ] GraphQL as alternative to REST
- [ ] Docker & containerization
- [ ] CI/CD pipelines (GitHub Actions, Azure DevOps)

---

**Good luck with your interview! 💪 You've got this!**

Last Updated: April 28, 2026
