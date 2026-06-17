using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskFour;

public partial class Task4Context : DbContext
{
    private Task4Context()
    {
    }

    private Task4Context(DbContextOptions<Task4Context> options)
        : base(options)
    {
    }

    private static Task4Context? s_instance = null;
    public static Task4Context Instance { 
        get
        {
            s_instance ??= new();
            return s_instance;
        } }

    public virtual DbSet<Db.SignInTimestamp> SignInTimestamps { get; set; }

    public virtual DbSet<Db.User> Users { get; set; }

    public virtual DbSet<Db.VerifyGuid> VerifyGuids { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=task4.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Db.SignInTimestamp>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Timestamp });

            entity.ToTable("sign_in_timestamps");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("current_timestamp")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.User).WithMany(p => p.SignInTimestamps)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Db.User>(entity =>
        {
            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "IX_users_email").IsUnique();

            entity.HasIndex(e => e.Id, "IX_users_id").IsUnique();

            entity.HasIndex(e => new { e.Email, e.Password }, "IX_users_email_password").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Db.VerifyGuid>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("verify_guids");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Guid).HasColumnName("guid");

            entity.HasOne(d => d.User).WithOne(p => p.VerifyGuid)
                .HasForeignKey<Db.VerifyGuid>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
