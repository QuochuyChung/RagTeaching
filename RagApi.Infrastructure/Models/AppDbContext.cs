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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>().HasKey(u => u.UserId);
        modelBuilder.Entity<Documents>().HasKey(d => d.DocumentId);
        modelBuilder.Entity<Chunks>().HasKey(c => c.ChunkId);
        modelBuilder.Entity<Conversations>().HasKey(c => c.ConversationId);
        modelBuilder.Entity<Messages>().HasKey(m => m.MessageId);

        // Conversations <- Users
        modelBuilder.Entity<Conversations>()
            .HasOne(c => c.Users)
            .WithMany(u => u.Conversations)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Messages>()
            .HasOne(m => m.Conversations)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Chunks>()
            .HasOne(c => c.Documents)
            .WithMany(d => d.Chunks)
            .HasForeignKey(c => c.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Documents>()
            .HasOne(d => d.Users)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
    

}
