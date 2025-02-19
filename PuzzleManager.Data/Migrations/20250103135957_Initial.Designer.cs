﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PuzzleManager.Data;

#nullable disable

namespace PuzzleManager.Data.Migrations
{
    [DbContext(typeof(PuzzleManagerContext))]
    [Migration("20250103135957_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PuzzleManager.Domain.Puzzle", b =>
                {
                    b.Property<int>("PuzzleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PuzzleId"));

                    b.Property<double>("DifficultyRating")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PieceCount")
                        .HasColumnType("int");

                    b.Property<int>("PuzzleMakerId")
                        .HasColumnType("int");

                    b.HasKey("PuzzleId");

                    b.HasIndex("PuzzleMakerId");

                    b.ToTable("Puzzles");
                });

            modelBuilder.Entity("PuzzleManager.Domain.PuzzleCheckout", b =>
                {
                    b.Property<int>("PuzzleCheckoutId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PuzzleCheckoutId"));

                    b.Property<DateTime>("CheckoutDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PuzzleHolderId")
                        .HasColumnType("int");

                    b.Property<int>("PuzzleId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("datetime2");

                    b.Property<double?>("TimeToComplete")
                        .HasColumnType("float");

                    b.Property<double?>("UserDifficultyRating")
                        .HasColumnType("float");

                    b.HasKey("PuzzleCheckoutId");

                    b.HasIndex("PuzzleHolderId");

                    b.HasIndex("PuzzleId");

                    b.ToTable("PuzzleCheckouts");
                });

            modelBuilder.Entity("PuzzleManager.Domain.PuzzleHolder", b =>
                {
                    b.Property<int>("PuzzleHolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PuzzleHolderId"));

                    b.Property<string>("Contact")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PuzzleHolderId");

                    b.ToTable("PuzzleHolders");
                });

            modelBuilder.Entity("PuzzleManager.Domain.PuzzleMaker", b =>
                {
                    b.Property<int>("PuzzleMakerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PuzzleMakerId"));

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PuzzleMakerId");

                    b.ToTable("PuzzleMakers");
                });

            modelBuilder.Entity("PuzzleManager.Domain.Puzzle", b =>
                {
                    b.HasOne("PuzzleManager.Domain.PuzzleMaker", "Maker")
                        .WithMany("Puzzles")
                        .HasForeignKey("PuzzleMakerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Maker");
                });

            modelBuilder.Entity("PuzzleManager.Domain.PuzzleCheckout", b =>
                {
                    b.HasOne("PuzzleManager.Domain.PuzzleHolder", "PuzzleHolder")
                        .WithMany("PuzzleCheckouts")
                        .HasForeignKey("PuzzleHolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PuzzleManager.Domain.Puzzle", "Puzzle")
                        .WithMany("PuzzleCheckouts")
                        .HasForeignKey("PuzzleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Puzzle");

                    b.Navigation("PuzzleHolder");
                });

            modelBuilder.Entity("PuzzleManager.Domain.Puzzle", b =>
                {
                    b.Navigation("PuzzleCheckouts");
                });

            modelBuilder.Entity("PuzzleManager.Domain.PuzzleHolder", b =>
                {
                    b.Navigation("PuzzleCheckouts");
                });

            modelBuilder.Entity("PuzzleManager.Domain.PuzzleMaker", b =>
                {
                    b.Navigation("Puzzles");
                });
#pragma warning restore 612, 618
        }
    }
}
