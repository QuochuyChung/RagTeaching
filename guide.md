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

## Cheatsheet — EF Core Migration

**Điều kiện 1:** luôn chạy từ thư mục gốc solution (`SourceCode/`), **không** đứng trong `RagApi.API/` hay `RagApi.Infrastructure/` — vì `--project`/`--startup-project` là đường dẫn tương đối tính từ chỗ đang đứng.

**Điều kiện 2:** `Microsoft.EntityFrameworkCore.Design` phải cài ở **cả 2 project** — nơi có `DbContext` (`RagApi.Infrastructure`) **và** project khởi động (`RagApi.API`). Thiếu 1 trong 2 sẽ báo lỗi tương ứng.

```bash
dotnet tool install --global dotnet-ef      # cài tool, chỉ cần 1 lần cho máy
dotnet tool update --global dotnet-ef       # cập nhật tool lên bản mới nhất

# tạo migration mới
dotnet ef migrations add <TênMigration> --project RagApi.Infrastructure --startup-project RagApi.API

# áp migration lên database thật
dotnet ef database update --project RagApi.Infrastructure --startup-project RagApi.API

# liệt kê tất cả migration đã tạo
dotnet ef migrations list --project RagApi.Infrastructure --startup-project RagApi.API

# xóa migration cuối cùng (chỉ dùng khi CHƯA chạy database update)
dotnet ef migrations remove --project RagApi.Infrastructure --startup-project RagApi.API

# rollback về đúng 1 migration cũ hơn (ví dụ lùi về "Init")
dotnet ef database update Init --project RagApi.Infrastructure --startup-project RagApi.API

# rollback về trạng thái trống (bỏ hết migration khỏi DB, chưa xóa DB)
dotnet ef database update 0 --project RagApi.Infrastructure --startup-project RagApi.API

# xuất toàn bộ migration ra file SQL (để review hoặc đưa DBA, không chạy trực tiếp)
dotnet ef migrations script --project RagApi.Infrastructure --startup-project RagApi.API -o migration.sql

# xóa hẳn database — reset sạch, MẤT DATA
dotnet ef database drop --project RagApi.Infrastructure --startup-project RagApi.API
```

**Bug đã gặp:**
- `Unable to retrieve project metadata` — chạy lệnh khi đang đứng trong `RagApi.API/`, không phải solution root.
- `Unable to create a 'DbContext'...` — thiếu cờ `--project`/`--startup-project`.
- `project ... không reference EF Core Design` — quên cài `Microsoft.EntityFrameworkCore.Design` ở project startup (`RagApi.API`), không chỉ ở project chứa `DbContext`.
- `The entity type 'X' requires a primary key to be defined` — đặt tên class số nhiều (`Chunks`) nhưng property khóa chính lại số ít (`ChunkId`). EF Core convention tự nhận PK theo `Id` hoặc `<TênClass>Id` — không khớp thì không nhận ra. Fix bằng khai tường minh trong `OnModelCreating`:
  ```csharp
  modelBuilder.Entity<Chunks>().HasKey(c => c.ChunkId);
  ```