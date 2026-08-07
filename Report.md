# Report

## Ngày 1

### Hạ tầng dự án
- **Ollama**: nơi chứa các model LLM/embedding mình sẽ xài
- **Qdrant**: Vector Database
- **Postgres**: Relational Database, ghi lại những thông tin quan trọng (metadata, lịch sử chat...)

### Docker
- Để dựng được hạ tầng thì phải xài Docker
- **Docker Image**: bản thảo của 1 app — chứa source code, hệ điều hành sẽ chạy, thư viện, tất tần tật những thứ liên quan về app
- Để app riêng của mình được đóng gói thành 1 image thì bắt buộc phải viết **Dockerfile**
- **Dockerfile**: nơi định nghĩa những thư viện, hệ điều hành, source code sẽ để đâu trong container
- **Docker Container**: môi trường chạy app

2 cách setup container:
1. Chạy riêng từng câu lệnh CLI — khởi tạo được 1 container tại 1 thời điểm
2. Chạy bằng file **Docker Compose** — 1 file cho chạy cùng lúc nhiều container, viết bằng định dạng YAML

## Ngày 2

### Setup Docker & WSL2
- Bật **WSL Integration** trong Docker Desktop (Settings → Resources → WSL Integration) — thiếu bước này thì lệnh `docker` không nhận trong WSL.
- Gặp lỗi `permission denied while trying to connect to the docker API` → user chưa nằm trong group `docker`:
  ```bash
  sudo usermod -aG docker $USER
  ```
  Bắt buộc restart session mới có hiệu lực: `wsl --shutdown` (chạy trong PowerShell) rồi mở lại terminal.
- Gặp lỗi pull model `Error: pull model manifest: file does not exist` → do container `ollama` khởi động lúc mạng WSL2 còn lỗi. Test bằng `docker run --rm curlimages/curl -sI https://ollama.com` để xác nhận Docker network ổn, rồi `docker restart <container>` cho container `ollama` là pull được.

### Dựng project .NET theo kiến trúc Clean Architecture
Tạo solution `RagApi` (.NET 10 sinh ra file `.slnx`) + 3 project:
- `RagApi.API` — Presentation layer (Controllers, Program.cs)
- `RagApi.Application` — Application layer (business logic)
- `RagApi.Infrastructure` — Infrastructure layer (EF Core, entity)

Chiều reference nối tiếp: `API → Application → Infrastructure`. `API` không reference thẳng `Infrastructure`, nhưng vẫn gọi được nhờ `ProjectReference` trong .NET là transitive (bắc cầu).

NuGet cài theo đúng layer cần dùng:
- `RagApi.Infrastructure`: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`
- `RagApi.Application`: `Qdrant.Client`, `OllamaSharp`
- `RagApi.API`: `Swashbuckle.AspNetCore`

### Health check + Swagger
- Viết `HealthController` → `GET /health` trả về `{"status":"ok"}`.
- Đăng ký Swagger UI trong `Program.cs`: `AddEndpointsApiExplorer()`, `AddSwaggerGen()`, `UseSwagger()`, `UseSwaggerUI()` → mở được trang `/swagger` để test API.

### Bug đã gặp
- `var app = builder.Build();` đặt **trước** các dòng `builder.Services.AddXxx()` → lỗi `The service collection cannot be modified because it is read-only`. `Build()` khóa `IServiceCollection` lại, nên mọi đăng ký service phải đứng trước nó.
- Thiếu `app.MapControllers()` → Controller không được gắn route, gọi API ra 404.
