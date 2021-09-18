using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using API.Persistance.Entity;

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

        public virtual DbSet<Aliases> Aliases { get; set; }
        public virtual DbSet<AliasesMembers> AliasesMembers { get; set; }
        public virtual DbSet<AppUsers> AppUsers { get; set; }
        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<Attachments> Attachments { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<ContactsMembers> ContactsMembers { get; set; }
        public virtual DbSet<Imports> Imports { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<WriterTypes> WriterTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MessagesStore;Username=messagestorer;Password=messagestorer");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aliases>(entity =>
            {
                entity.ToTable("aliases");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Internal).HasColumnName("internal");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(256);

                entity.Property(e => e.UserGivenName)
                    .HasColumnName("user_given_name")
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<AliasesMembers>(entity =>
            {
                entity.ToTable("aliases_members");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AliasId).HasColumnName("alias_id");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.HasOne(d => d.Alias)
                    .WithMany(p => p.AliasesMembers)
                    .HasForeignKey(d => d.AliasId)
                    .HasConstraintName("fk_aliases_members_aliases");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.AliasesMembers)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("fk_aliases_members_contacts");
            });

            modelBuilder.Entity<AppUsers>(entity =>
            {
                entity.ToTable("app_users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(256);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(60);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Applications>(entity =>
            {
                entity.ToTable("applications");

                entity.HasIndex(e => e.Name)
                    .HasName("applications_con_unq_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Attachments>(entity =>
            {
                entity.ToTable("attachments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ContentType)
                    .HasColumnName("content_type")
                    .HasMaxLength(100);

                entity.Property(e => e.Filename)
                    .HasColumnName("filename")
                    .HasMaxLength(64);

                entity.Property(e => e.MessageId).HasColumnName("message_id");

                entity.Property(e => e.SaveAsFilename)
                    .HasColumnName("save_as_filename")
                    .HasMaxLength(256);

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.MessageId)
                    .HasConstraintName("fk_attachments_messages");
            });

            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.ToTable("contacts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppUserId).HasColumnName("app_user_id");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.InApplicationId)
                    .HasColumnName("in_application_id")
                    .HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(256);

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("fk_contacts_app_users");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("fk_contacts_applications");
            });

            modelBuilder.Entity<ContactsMembers>(entity =>
            {
                entity.ToTable("contacts_members");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.Property(e => e.InternalId)
                    .HasColumnName("internal_id")
                    .HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(256);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactsMembers)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_contacts_members_contacts");
            });

            modelBuilder.Entity<Imports>(entity =>
            {
                entity.ToTable("imports");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.ImportId)
                    .IsRequired()
                    .HasColumnName("import_id")
                    .HasMaxLength(64);

                entity.Property(e => e.IsBeingDeleted).HasColumnName("is_being_deleted");
            });

            modelBuilder.Entity<Messages>(entity =>
            {
                entity.ToTable("messages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.Property(e => e.ContactMemberId).HasColumnName("contact_member_id");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasMaxLength(307200);

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.HasError).HasColumnName("has_error");

                entity.Property(e => e.ImportId).HasColumnName("import_id");

                entity.Property(e => e.WriterTypeId).HasColumnName("writer_type_id");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("fk_messages_contacts");

                entity.HasOne(d => d.ContactMember)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ContactMemberId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_messages_contacts_members");

                entity.HasOne(d => d.Import)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ImportId)
                    .HasConstraintName("fk_messages_imports");

                entity.HasOne(d => d.WriterType)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.WriterTypeId)
                    .HasConstraintName("fk_messages_writer_types");
            });

            modelBuilder.Entity<WriterTypes>(entity =>
            {
                entity.ToTable("writer_types");

                entity.HasIndex(e => e.Name)
                    .HasName("writer_types_con_unq_name")
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
