### Hướng dẫn làm hệ thống RAG
**Các câu lệnh để setup project**
```bash
dotnet new sln -n RagApi # tạo 1 sln mới cho project

dotnet new webapi -n RagApi.API # tạo 1 project web api làm tầng API - Presentation trong kiến trúc 3 Layers
dotnet new classlib -n RagApi.Infrastructure # tạo 1 project infrastucture - Dùng để cấu hình hạ tầng
dotnet new classlib -n RagApi.Application # tạo 1 project application - Dùng để thao tác nghiệp 

dotnet sln RagApi.slnx add RagApi.API/RagApi.API.csproj RagApi.Infrastructure/RagApi.Infrastructure.csproj RagApi.Application/RagApi.Application.csproj # add các projects vào trong sln 

dotnet add RagApi.API/RagApi.API.csproj reference RagApi.Application/RagApi.Application.csproj # API -> Application
dotnet add RagApi.Application/RagApi.Application.csproj reference RagApi.Infrastructure/RagApi.Infrastructure.csproj # Application -> Infrastructure

# Infrastructure layer — EF Core + Postgres provider
dotnet add RagApi.Infrastructure/RagApi.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add RagApi.Infrastructure/RagApi.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design

# Application layer — client Qdrant + Ollama
dotnet add RagApi.Application/RagApi.Application.csproj package Qdrant.Client
dotnet add RagApi.Application/RagApi.Application.csproj package OllamaSharp

# API layer — chỉ cần Swagger UI (.NET 9+ KHÔNG còn sẵn)
dotnet add RagApi.API/RagApi.API.csproj package Swashbuckle.AspNetCore
```

**Health check controller** — `RagApi.API/Controllers/HealthController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;

namespace RagApi.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });
}
```

**`Program.cs`** — đăng ký Controllers + Swagger, đúng thứ tự (đăng ký service → `Build()` → cấu hình middleware → `Run()`):
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
```
> Bug đã gặp: để `var app = builder.Build();` **trước** mấy dòng `builder.Services.Add...` → lỗi `The service collection cannot be modified because it is read-only`. `Build()` khóa `IServiceCollection` lại, nên mọi `AddXxx()` phải đứng trước nó.

```bash
dotnet run --project RagApi.API
# GET https://localhost:xxxx/health → {"status":"ok"}
# Swagger: https://localhost:xxxx/swagger
```