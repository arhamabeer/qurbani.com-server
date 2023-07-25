using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Qurabani.com_Server.Models;

public partial class QurbaniContext : DbContext
{
    public QurbaniContext()
    {
    }

    public QurbaniContext(DbContextOptions<QurbaniContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<AnimalDetail> AnimalDetails { get; set; }

    public virtual DbSet<AnimalPart> AnimalParts { get; set; }

    
    public virtual DbSet<Dealing> Dealings { get; set; }

    
    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=ChampionPC\\SQLEXPRESS;Database=qurbani;Trusted_Connection=True;TrustServerCertificate=true;");
      //=> optionsBuilder.UseSqlServer("Server=DESKTOP-4VAOE89;Database=qurbani;Trusted_Connection=True;TrustServerCertificate=true;");



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.AnimalId).HasName("PK__Animal__A21A73270A32355A");

            entity.ToTable("Animal");

            entity.Property(e => e.AnimalId).HasColumnName("AnimalID");
            entity.Property(e => e.Memo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Parts).HasColumnName("parts");
            entity.Property(e => e.Type)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AnimalDetail>(entity =>
        {
            entity.HasKey(e => e.Adid).HasName("PK__AnimalDe__7930D5A04A254952");

            entity.ToTable("AnimalDetail");

            entity.Property(e => e.Adid).HasColumnName("ADID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Memo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PartFinalPrice).HasColumnType("money");
            entity.Property(e => e.PartSellPrice).HasColumnType("money");

            entity.HasOne(d => d.Animal).WithMany(p => p.AnimalDetails)
                .HasForeignKey(d => d.AnimalId)
                .HasConstraintName("AnimalDetail_Animal");
        });

        modelBuilder.Entity<AnimalPart>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__AnimalPa__7C3F0D50E4249F20");

            entity.ToTable("AnimalPart");

            entity.Property(e => e.Memo)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

       

        modelBuilder.Entity<Dealing>(entity =>
        {
            entity.HasKey(e => e.DealId).HasName("PK__Dealing__E5B281660F92AC21");

            entity.ToTable("Dealing");

            entity.Property(e => e.Adid).HasColumnName("ADID");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsConfirm).HasColumnName("isConfirm");
            entity.Property(e => e.Memo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PickedUp).HasColumnName("pickedUp");

            entity.HasOne(d => d.Ad).WithMany(p => p.Dealings)
                .HasForeignKey(d => d.Adid)
                .HasConstraintName("Deal_AnimalDetail");

            entity.HasOne(d => d.Part).WithMany(p => p.Dealings)
                .HasForeignKey(d => d.PartId)
                .HasConstraintName("Deal_Part");

            entity.HasOne(d => d.Person).WithMany(p => p.Dealings)
                .HasForeignKey(d => d.PersonId)
                .HasConstraintName("Deal_Person");
        });

        
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Person__AA2FFBE57ED58C8C");

            entity.ToTable("Person");

            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Contact)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmergencyContact)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Memo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Nic)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NIC");
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("userInfo");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(60)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
