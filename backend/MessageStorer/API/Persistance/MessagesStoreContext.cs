using System;
using API.Persistance.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Persistance
{
    public partial class MessagesStoreContext : DbContext
    {
        public MessagesStoreContext()
        {
        }

        public MessagesStoreContext(DbContextOptions<MessagesStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alias> Alias { get; set; }
        public virtual DbSet<AliasMember> AliasMember { get; set; }
        public virtual DbSet<AppUser> AppUser { get; set; }
        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<WriterType> WriterType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alias>(entity =>
            {
                entity.ToTable("alias");

                entity.HasIndex(e => e.Name)
                    .HasName("alias_con_unq_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Internal).HasColumnName("internal");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<AliasMember>(entity =>
            {
                entity.ToTable("alias_member");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AliasId).HasColumnName("alias_id");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.HasOne(d => d.Alias)
                    .WithMany(p => p.AliasMember)
                    .HasForeignKey(d => d.AliasId)
                    .HasConstraintName("fk_alias_member_alias");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.AliasMember)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("fk_alias_member_contact");
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("app_user");

                entity.HasIndex(e => e.Username)
                    .HasName("app_user_con_unq_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(60);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("application");

                entity.HasIndex(e => e.Name)
                    .HasName("application_con_unq_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("contact");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppUserId).HasColumnName("app_user_id");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.InApplicationId)
                    .HasColumnName("in_application_id")
                    .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20);

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("fk_contact_app_user");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("fk_contact_application");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attachment).HasColumnName("attachment");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasMaxLength(1000);

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.WriterTypeId).HasColumnName("writer_type_id");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("fk_message_contact");

                entity.HasOne(d => d.WriterType)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.WriterTypeId)
                    .HasConstraintName("fk_message_writer_type");
            });

            modelBuilder.Entity<WriterType>(entity =>
            {
                entity.ToTable("writer_type");

                entity.HasIndex(e => e.Name)
                    .HasName("writer_type_con_unq_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(10);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
