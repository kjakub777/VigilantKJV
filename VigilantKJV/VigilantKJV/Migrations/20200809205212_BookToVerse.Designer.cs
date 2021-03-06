﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VigilantKJV.Models;

namespace VigilantKJV.Migrations
{
    [DbContext(typeof(MyKjvContext))]
    [Migration("20200809205212_BookToVerse")]
    partial class BookToVerse :Migration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("VigilantKJV.Models.Bible", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Information")
                        .HasColumnType("nvarchar");

                    b.Property<string>("Label")
                        .HasColumnType("varchar");

                    b.Property<string>("Version")
                        .HasColumnType("nvarchar");

                    b.HasKey("Id");

                    b.ToTable("Bible");
                });

            modelBuilder.Entity("VigilantKJV.Models.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Information")
                        .HasColumnType("nvarchar");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar");

                    b.Property<int>("Ordinal")
                        .HasColumnType("int");

                    b.Property<Testament>("Testament")
                        .HasColumnType("string");

                    b.HasKey("Id");

                    b.ToTable("Book");
                });

            modelBuilder.Entity("VigilantKJV.Models.Chapter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Information")
                        .HasColumnType("nvarchar");

                    b.Property<int>("Number")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.ToTable("Chapter");
                });

            modelBuilder.Entity("VigilantKJV.Models.Verse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BookId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ChapterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Information")
                        .HasColumnType("nvarchar");

                    b.Property<bool>("IsMemorized")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastRecited")
                        .HasColumnType("datetime");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("varchar")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("ChapterId");

                    b.ToTable("Verse");
                });

            modelBuilder.Entity("VigilantKJV.Models.Chapter", b =>
                {
                    b.HasOne("VigilantKJV.Models.Book", "Book")
                        .WithMany("Chapters")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VigilantKJV.Models.Verse", b =>
                {
                    b.HasOne("VigilantKJV.Models.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VigilantKJV.Models.Chapter", "Chapter")
                        .WithMany("Verses")
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
