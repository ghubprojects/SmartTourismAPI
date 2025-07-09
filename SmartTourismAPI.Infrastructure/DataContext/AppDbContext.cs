using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmartTourismAPI.Domain.Entities;

namespace SmartTourismAPI.Infrastructure.DataContext;

public partial class AppDbContext : DbContext {
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public virtual DbSet<MFile> MFiles { get; set; }

    public virtual DbSet<MPlaceDetail> MPlaceDetails { get; set; }

    public virtual DbSet<MPlaceType> MPlaceTypes { get; set; }

    public virtual DbSet<MUser> MUsers { get; set; }

    public virtual DbSet<TPlacePhoto> TPlacePhotos { get; set; }

    public virtual DbSet<TPlaceReview> TPlaceReviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<MFile>(entity => {
            entity.HasKey(e => e.FileId).HasName("m_file_pk");

            entity.ToTable("m_file");

            entity.Property(e => e.FileId).HasColumnName("file_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.FileName).HasColumnName("file_name");
            entity.Property(e => e.FilePath).HasColumnName("file_path");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(50)
                .HasColumnName("last_modified_by");
            entity.Property(e => e.LastModifiedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_modified_date");
        });

        modelBuilder.Entity<MPlaceDetail>(entity => {
            entity.HasKey(e => e.DetailId).HasName("m_place_detail_pk");

            entity.ToTable("m_place_detail");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LastModifiedBy).HasColumnName("last_modified_by");
            entity.Property(e => e.LastModifiedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_modified_date");
            entity.Property(e => e.OpeningHours)
                .HasMaxLength(100)
                .HasColumnName("opening_hours");
            entity.Property(e => e.OsmId)
                .HasMaxLength(12)
                .HasColumnName("osm_id");
            entity.Property(e => e.TypeId)
                .HasColumnName("type_id");

            entity.HasOne(d => d.Type).WithMany(p => p.MPlaceDetails)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("m_place_detail_m_place_type_fk");
        });

        modelBuilder.Entity<MPlaceType>(entity => {
            entity.HasKey(e => e.TypeId).HasName("m_place_type_pk");

            entity.ToTable("m_place_type");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.Fclass)
                .HasMaxLength(255)
                .HasColumnName("fclass");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(50)
                .HasColumnName("last_modified_by");
            entity.Property(e => e.LastModifiedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_modified_date");
            entity.Property(e => e.TypeName)
                .HasMaxLength(255)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<MUser>(entity => {
            entity.HasKey(e => e.UserId).HasName("m_user_pk");

            entity.ToTable("m_user");

            entity.HasIndex(e => e.AvatarId, "IX_m_user_avatar_id");

            entity.HasIndex(e => e.Username, "m_user_name_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvatarId)
                .HasDefaultValue(77)
                .HasColumnName("avatar_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(50)
                .HasColumnName("last_modified_by");
            entity.Property(e => e.LastModifiedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_modified_date");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Avatar).WithMany(p => p.MUsers)
                .HasForeignKey(d => d.AvatarId)
                .HasConstraintName("m_user_m_file_fk");
        });

        modelBuilder.Entity<TPlacePhoto>(entity => {
            entity.HasKey(e => e.PhotoId).HasName("t_place_photo_pk");

            entity.ToTable("t_place_photo");

            entity.HasIndex(e => e.DetailId, "IX_t_place_photo_detail_id");

            entity.HasIndex(e => e.FileId, "IX_t_place_photo_file_id");

            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.Caption)
                .HasMaxLength(255)
                .HasColumnName("caption");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.FileId).HasColumnName("file_id");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(50)
                .HasColumnName("last_modified_by");
            entity.Property(e => e.LastModifiedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_modified_date");

            entity.HasOne(d => d.Detail).WithMany(p => p.TPlacePhotos)
                .HasForeignKey(d => d.DetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_place_photo_m_place_detail_fk");

            entity.HasOne(d => d.File).WithMany(p => p.TPlacePhotos)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_place_photo_m_file_fk");
        });

        modelBuilder.Entity<TPlaceReview>(entity => {
            entity.HasKey(e => e.ReviewId).HasName("t_place_review_pk");

            entity.ToTable("t_place_review");

            entity.HasIndex(e => e.DetailId, "IX_t_place_review_detail_id");

            entity.HasIndex(e => e.UserId, "IX_t_place_review_user_id");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(50)
                .HasColumnName("last_modified_by");
            entity.Property(e => e.LastModifiedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_modified_date");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Detail).WithMany(p => p.TPlaceReviews)
                .HasForeignKey(d => d.DetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_place_review_m_place_detail_fk");

            entity.HasOne(d => d.User).WithMany(p => p.TPlaceReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_place_review_m_user_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
