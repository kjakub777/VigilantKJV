﻿
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;
using VigilantKJV.Helpers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using Acr.UserDialogs;
using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace VigilantKJV.Models
{
    public partial class MyKjvContext : DbContext
    {
        public MyKjvContext()
        {
            this.Database.EnsureCreated();
        }

        public MyKjvContext(DbContextOptions<MyKjvContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        public virtual DbSet<Bible> Bible { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<Chapter> Chapter { get; set; }
        public virtual DbSet<Verse> Verse { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Xamarin.Forms.DependencyService.Get<IDbPath>().GetPlatformDBPath();
            optionsBuilder.UseSqlite($"Filename={dbPath}");
            //            if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //                optionsBuilder.UseSqlite(@"Filename=E:\Code\Mobile\VigilantKJV\Dependencies\Database\MyKjvVigilant.db");
            //            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bible>(entity =>
            {
                entity.HasKey("Id");
                entity.Property(e => e.Id).HasColumnType("uniqueidentifier");

                entity.Property(e => e.Information).HasColumnType("nvarchar");

                entity.Property(e => e.Label).HasColumnType("varchar");


                entity.Property(e => e.Version).HasColumnType("nvarchar");

            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnType("uniqueidentifier");

                entity.Property(e => e.Information).HasColumnType("nvarchar");

                entity.Property(e => e.Name).HasColumnType("nvarchar");

                entity.Property(e => e.Ordinal).HasColumnType("int");

                entity.Property(e => e.Testament)
                    .IsRequired()
                    .HasColumnType("string");
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.BookId)
                    .IsRequired()
                    .HasColumnType("uniqueidentifier");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnType("uniqueidentifier");

                entity.Property(e => e.Information).HasColumnType("nvarchar");

                entity.Property(e => e.Number).HasColumnType("tinyint");
            });

            //modelBuilder.Entity<Testament>(entity =>
            //{
            //    entity.HasKey("Id");

            //    entity.Property(e => e.BibleId)
            //        .IsRequired()
            //        .HasColumnType("uniqueidentifier");

            //    entity.Property(e => e.Id)
            //        .IsRequired()
            //        .HasColumnType("uniqueidentifier");

            //    entity.Property(e => e.Information).HasColumnType("nvarchar");

            //    entity.Property(e => e.Name).HasColumnType("nvarchar");
            //});

            modelBuilder.Entity<Verse>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.ChapterId)
                    .IsRequired()
                    .HasColumnType("uniqueidentifier");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnType("uniqueidentifier");

                entity.Property(e => e.Information).HasColumnType("nvarchar");

                entity.Property(e => e.IsMemorized).HasColumnType("bit");

                entity.Property(e => e.LastRecited).HasColumnType("datetime");

                entity.Property(e => e.Number).HasColumnType("int");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnType("varchar");
            });

            OnModelCreatingPartial(modelBuilder);
        }

      
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    
    }
}

//namespace VigilantKJV.Models
/*{
    public class TestamentValueConverter : ValueConverter
    {
        public TestamentValueConverter(LambdaExpression convertToProviderExpression):base(convertToProviderExpression)
        {

        }
        public override Func<object, object> ConvertToProvider => new Func<object, object>((testament) =>
        {     return       Enum.GetName(typeof(Testament), testament);
        });


        public override Func<object, object> ConvertFromProvider => new Func<object, object>((testament) =>
        {
            return Enum.Parse(typeof(Testament), ""+testament);
        });

        public override Type ModelClrType => typeof(int);

        public override Type ProviderClrType => typeof(int);

    }
}*/
