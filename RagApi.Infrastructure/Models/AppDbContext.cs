using Microsoft.EntityFrameworkCore;
using RagApi.Infrastructure.Models;


namespace RagApi.Infrastructure.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    // khai báo bảng 
    public DbSet<Users> Users => Set<Users>();
    public DbSet<Documents> Documents => Set<Documents>();
    public DbSet<Chunks> Chunks => Set<Chunks>();
    public DbSet<Conversations> Conversations => Set<Conversations>();
    public DbSet<Messages> Messages => Set<Messages>();

    //nối quan hệ 
    

}
