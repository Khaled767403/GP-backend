using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace otherServices.Models;

public partial class AppDbContext2 : DbContext
{
    public AppDbContext2()
    {
    }

    public AppDbContext2(DbContextOptions<AppDbContext2> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<SavedPost> SavedPosts { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Proposal> Proposals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server =XZ-BRHOM7\\SQLEXPRESS ; Database =otherServices; Integrated Security = SSPI ; TrustServerCertificate = True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {

            entity.HasKey(e => e.CommentId).HasName("PK__Comments__99FC14DBFDFD4C38");

            entity.Property(e => e.CommentId).HasColumnName("CommentId");
            entity.Property(e => e.DateComment)
                .HasColumnType("datetime")
                .HasColumnName("date_comment");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.PostId).HasColumnName("Post_Id");
            entity.Property(e => e.UserId).HasColumnName("user_Id");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comments_PostId");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Comments_UserId");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__messages__0BBC6ACE1FC3A57E");

            entity.ToTable("messages");

            entity.Property(e => e.MessageId).HasColumnName("message_Id");
            entity.Property(e => e.DateMessge)
                .HasColumnType("datetime")
                .HasColumnName("date_messge");
            entity.Property(e => e.SenderId).HasColumnName("senderId");
            entity.Property(e => e.Message1)
                .HasMaxLength(255)
                .HasColumnName("Message");
            entity.Property(e => e.ReceiverId).HasColumnName("recieverId");

            entity.HasOne(d => d.Sender).WithMany(p => p.SentMessages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__messages__date_m__18EBB532");

            entity.HasOne(d => d.Receiver).WithMany(p => p.ReceivedMessages)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__messages__tenant__19DFD96B");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__posts__3ED48BBE6C5F48C0");

            entity.ToTable("posts");

            entity.Property(e => e.PostId).HasColumnName("post_Id");
            entity.Property(e => e.DatePost)
                .HasColumnType("datetime")
                .HasColumnName("date_post");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FlagWaitingPost).HasColumnName("flag_waiting_post");
            entity.Property(e => e.LandlordId).HasColumnName("Landlord_id");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.RentalStatus)
                .HasMaxLength(255)
                .HasColumnName("rental_status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Landlord).WithMany(p => p.Posts)
                .HasForeignKey(d => d.LandlordId)
                .HasConstraintName("FK__posts__date_post__66603565");
        });

        modelBuilder.Entity<SavedPost>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.PostId }).HasName("PK__Saved_Po__1370CC3CF5ACF434");

            entity.ToTable("Saved_Post");

            entity.Property(e => e.TenantId).HasColumnName("tenant_Id");
            entity.Property(e => e.PostId).HasColumnName("Post_Id");
            entity.Property(e => e.DateSaved)
                .HasColumnType("datetime")
                .HasColumnName("date_saved");

            entity.HasOne(d => d.Post).WithMany(p => p.SavedPosts)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Saved_Pos__Post___04E4BC85");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SavedPosts)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Saved_Pos__tenan__03F0984C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__CB9A1CFFDCE0066F");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164944D8BF6").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FName)
                .HasMaxLength(255)
                .HasColumnName("fName");
            entity.Property(e => e.FlagWaitingUser).HasColumnName("flag_waiting_user");
            entity.Property(e => e.LName)
                .HasMaxLength(255)
                .HasColumnName("lName");
            entity.Property(e => e.Pass).HasMaxLength(255);
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("Role_name");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("userName");

            entity.HasMany(d => d.PostsNavigation).WithMany(p => p.Tenants)
                .UsingEntity<Dictionary<string, object>>(
                    "ReservedSaved",
                    r => r.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .HasConstraintName("FK__reserved___Post___7D439ABD"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__reserved___tenan__7C4F7684"),
                    j =>
                    {
                        j.HasKey("TenantId", "PostId").HasName("PK__reserved__1370CC3C9E94BEED");
                        j.ToTable("reserved_Saved");
                        j.IndexerProperty<long>("TenantId").HasColumnName("tenant_Id");
                        j.IndexerProperty<long>("PostId").HasColumnName("Post_Id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
