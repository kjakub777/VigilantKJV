
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text; 
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Chapter> Chapters { get; set; }
        public virtual DbSet<Verse> Verses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {  optionsBuilder.UseSqlite(@"Filename=E:\Code\Mobile\VigilantKJV\Dependencies\Database\MyKjvVigilant.db");
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

                entity.Property(e => e.BookName)
                .HasColumnType("nvarchar")
                .HasConversion(new EnumToStringConverter<BookName>());

                entity.Property(e => e.Ordinal).HasColumnType("int");

                entity.Property("Testament")
                    .HasColumnType("string")
                    .HasConversion(new EnumToStringConverter<Testament>());

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
        //public async Task<Verse> GetVerseAsync(Guid id)
        //{
        //    var item = await Verses.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
        //    return item;
        //}
        //public async Task<Verse> GetVerseAsync(Expression<Func<Verse, bool>> selector)
        //{
        //    var item = await Verses.FirstOrDefaultAsync(selector).ConfigureAwait(false);
        //    return item;
        //}
    }
}
