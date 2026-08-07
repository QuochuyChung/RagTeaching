# Docker CLI — Cheatsheet buổi 1
**Điêu kiện**: phải add user đang chạy vào trong group của docker
**Câu lệnh kiểm tra group**: groups
**Câu lệnh để thêm user hiện tại vào trong group:** sudo usermod -aG docker $USER

## Quản lý container (docker compose)

```bash
docker compose up -d          # dựng cả 3 service, chạy nền
docker compose down           # tắt + xóa container (giữ volume/data)
docker compose down -v        # tắt + xóa luôn volume — MẤT DATA, dùng khi muốn reset
docker compose stop           # tạm dừng, không xóa container
docker compose start          # chạy lại container đã stop
docker compose restart        # khởi động lại (thêm tên service để restart riêng 1 cái)
```

## Kiểm tra trạng thái

```bash
docker ps                     # container đang chạy, cột STATUS phải là "Up"
docker compose ps             # giống trên, chỉ trong phạm vi file compose hiện tại
docker compose logs -f        # xem log real-time cả 3 service
docker compose logs -f ollama # xem log riêng 1 service
```

## Chạy lệnh vào trong container

```bash
docker compose exec ollama ollama list
docker compose exec postgres psql -U rag -d ragdb -c "\dt"
docker compose exec -it ollama ollama run llama3.2 "xin chào"   # -it = interactive terminal
```

## Dọn dẹp

```bash
docker system prune           # xóa image/container/network thừa, không đụng volume
docker volume ls              # liệt kê volume — hữu ích khi debug "data đâu mất rồi"
```

## Câu lệnh để pull model của ollama về 
```bash
docker exec <ollama_container> ollama pull llama3.2 # LLM chat
docker exec <ollama_container> ollama pull nomic-embed-text # Model embedding 768 chiều
```