﻿// <auto-generated />
using DnDTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DnDTrackerAPI.Migrations
{
    [DbContext(typeof(DnDTrackerContext))]
    partial class DnDTrackerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DnDTrackerAPI.Models.Encounter", b =>
                {
                    b.Property<int>("EncounterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EncounterName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("EncounterId");

                    b.HasIndex("UserId");

                    b.ToTable("Encounters");
                });

            modelBuilder.Entity("DnDTrackerAPI.Models.Monster", b =>
                {
                    b.Property<string>("Index")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("WaveId")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.HasKey("Index", "WaveId");

                    b.HasIndex("WaveId");

                    b.ToTable("Monsters");
                });

            modelBuilder.Entity("DnDTrackerAPI.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasFilter("[UserName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DnDTrackerAPI.Models.Wave", b =>
                {
                    b.Property<int>("WaveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EncounterId")
                        .HasColumnType("int");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.HasKey("WaveId");

                    b.HasIndex("EncounterId");

                    b.ToTable("Waves");
                });

            modelBuilder.Entity("DnDTrackerAPI.Models.Encounter", b =>
                {
                    b.HasOne("DnDTrackerAPI.Models.User", "User")
                        .WithMany("Encounters")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DnDTrackerAPI.Models.Monster", b =>
                {
                    b.HasOne("DnDTrackerAPI.Models.Wave", "Wave")
                        .WithMany("Monsters")
                        .HasForeignKey("WaveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DnDTrackerAPI.Models.Wave", b =>
                {
                    b.HasOne("DnDTrackerAPI.Models.Encounter", "Encounter")
                        .WithMany("Waves")
                        .HasForeignKey("EncounterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
