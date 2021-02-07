using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MessengerIntegration.Persistance.Entity;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MessengerIntegration.Persistance
{
    public partial class MessengerIntegrationContext : DbContext
    {
        public MessengerIntegrationContext()
        {
        }

        public MessengerIntegrationContext(DbContextOptions<MessengerIntegrationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Imports> Imports { get; set; }
        public virtual DbSet<Statuses> Statuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MessengerIntegration;Username=messagestorer;Password=messagestorer");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Imports>(entity =>
            {
                entity.ToTable("imports");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(64);

                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.Property(e => e.FbUsername)
                    .IsRequired()
                    .HasColumnName("fb_username")
                    .HasMaxLength(256);

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Imports)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_imports_statuses");
            });

            modelBuilder.Entity<Statuses>(entity =>
            {
                entity.ToTable("statuses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(256);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
