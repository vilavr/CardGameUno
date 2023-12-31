﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231112163847_RedesigningGameProperties")]
    partial class RedesigningGameProperties
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.13");

            modelBuilder.Entity("Domain.Database.Card", b =>
                {
                    b.Property<int>("CardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CardColor")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("CardScore")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CardValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("CardId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("Domain.Database.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CurrentPlayerTurn")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CurrentRound")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SettingsFileName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool?>("SpecialCardEffectApplied")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TopCardId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TopCardId")
                        .IsUnique();

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Domain.Database.GameCard", b =>
                {
                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsInDiscard")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsTopCard")
                        .HasColumnType("INTEGER");

                    b.HasKey("GameId", "CardId");

                    b.HasIndex("CardId");

                    b.ToTable("GameCards");
                });

            modelBuilder.Entity("Domain.Database.GameSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SettingName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("SettingValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("FileName", "SettingName")
                        .IsUnique();

                    b.ToTable("GameSettings");
                });

            modelBuilder.Entity("Domain.Database.Player", b =>
                {
                    b.Property<int>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlayerNickname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("Score")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayerId");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerNickname")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Domain.Database.PlayerCard", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayerId", "CardId");

                    b.HasIndex("CardId");

                    b.ToTable("PlayerCards");
                });

            modelBuilder.Entity("Domain.Database.Game", b =>
                {
                    b.HasOne("Domain.Database.Card", "TopCard")
                        .WithOne()
                        .HasForeignKey("Domain.Database.Game", "TopCardId");

                    b.Navigation("TopCard");
                });

            modelBuilder.Entity("Domain.Database.GameCard", b =>
                {
                    b.HasOne("Domain.Database.Card", "Card")
                        .WithMany("GameCards")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Database.Game", "Game")
                        .WithMany("GameCards")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Domain.Database.GameSetting", b =>
                {
                    b.HasOne("Domain.Database.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Domain.Database.Player", b =>
                {
                    b.HasOne("Domain.Database.Game", null)
                        .WithMany("Players")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("Domain.Database.PlayerCard", b =>
                {
                    b.HasOne("Domain.Database.Card", "Card")
                        .WithMany("PlayerCards")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Database.Player", "Player")
                        .WithMany("PlayerCards")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Domain.Database.Card", b =>
                {
                    b.Navigation("GameCards");

                    b.Navigation("PlayerCards");
                });

            modelBuilder.Entity("Domain.Database.Game", b =>
                {
                    b.Navigation("GameCards");

                    b.Navigation("Players");
                });

            modelBuilder.Entity("Domain.Database.Player", b =>
                {
                    b.Navigation("PlayerCards");
                });
#pragma warning restore 612, 618
        }
    }
}
