using CVision.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CVision.DAL.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CV> CVs { get; set; }

    public DbSet<CVAnalysis> CVAnalyses { get; set; }

    public DbSet<CVAnalysisRecommendation> CVAnalysisRecommendations { get; set; }

    public DbSet<Publication> Publications { get; set; }

    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        builder.Entity<IdentityRole<int>>(entity =>
        {
            entity.ToTable("Roles");
        });

        builder.Entity<IdentityUserRole<int>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<IdentityUserClaim<int>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<int>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<IdentityRoleClaim<int>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserToken<int>>(entity =>
        {
            entity.ToTable("IdentityUserTokens");
        });

        builder.Entity<CV>(entity =>
        {
            entity.ToTable("CVs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany(u => u.CVs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CVAnalysis>(entity =>
        {
            entity.ToTable("CVAnalyses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AnalyzedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.CV)
                .WithMany(c => c.Analyses)
                .HasForeignKey(e => e.CVId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CVAnalysisRecommendation>(entity =>
        {
            entity.ToTable("CVAnalysesRecommendations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.CVAnalysis)
                .WithMany(a => a.Recommendations)
                .HasForeignKey(e => e.CVAnalysisId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Publication>(entity =>
        {
            entity.ToTable("Publications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PublishedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Publications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CV)
                .WithMany(c => c.Publications)
                .HasForeignKey(e => e.CVId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Likes).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Publication)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
