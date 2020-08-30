
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
using Microsoft.EntityFrameworkCore.Storage;

namespace VigilantKJV.Models
{
    public partial class MyKjvContext : DbContext, IDisposable
    {
        public override void Dispose()
        {

            base.Dispose();
        }
        public MyKjvContext()
        {
            // this.Database.EnsureCreated();
            //   this.Configuration.AutoDetectChangesEnabled = true;
        }

        public MyKjvContext(DbContextOptions<MyKjvContext> options)
            : base(options)
        {
            //    this.Database.EnsureCreated();
        }

        public virtual DbSet<Bible> Bible { get; set; }
        public virtual DbSet<Testament> Testament { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Chapter> Chapters { get; set; }
        public virtual DbSet<Verse> Verses { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Xamarin.Forms.DependencyService.Get<IDbPath>().GetPlatformDBPath();
            // optionsBuilder.UseSqlite($"Filename={dbPath}", opt =>
            optionsBuilder.UseSqlite($"Filename={dbPath}", opt =>
            {
                // opt.MaxBatchSize(1);

            }
        );
            //            if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //                optionsBuilder.UseSqlite(@"Filename=E:\Code\Mobile\VigilantKJV\Dependencies\Database\MyKjvVigilant.db");
            //            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<MyClass>(entity =>
            //{
            //    entity.HasKey("Id");
            //    entity.Property(e => e.Id).HasColumnType("uniqueidentifier");

            //    entity.Property(e => e.MyProperty).HasColumnType("nvarchar");

            //    entity.Property(e => e.MyTIme).HasColumnType("datetime");



            //});

            modelBuilder.Entity<Bible>(entity =>
            {
                entity.HasKey("Id");
                entity.Property(e => e.Id).HasColumnType("INTEGER");

                entity.Property(e => e.Information).HasColumnType("nvarchar");

                entity.Property(e => e.Label).HasColumnType("varchar");


                entity.Property(e => e.Version).HasColumnType("nvarchar");

            });

            modelBuilder.Entity<Testament>(entity =>
            {
                entity.HasKey("Id");

                //entity.Property(e => e.BibleId)
                //    .IsRequired()
                //    .HasColumnType("INTEGER");

                entity.Property(e => e.Id).IsRequired().HasColumnType("INTEGER");

                entity.Property(e => e.Information).HasColumnType("nvarchar");

                entity.Property(e => e.TestamentName)
                    .HasColumnType("nvarchar")
                    .HasConversion<string>();
                ;
            });


            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnType("INTEGER");
                entity.Property(e => e.TestamentId)
                             .IsRequired()
                             .HasColumnType("INTEGER");


                entity.Property(e => e.BookName)
                .HasColumnType("nvarchar")
                .HasConversion(new EnumToStringConverter<BookName>());

                entity.Property(e => e.Information).HasColumnType("nvarchar");
                entity.Property(e => e.Ordinal).HasColumnType("int");



            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasKey("Id");

                entity.Property(e => e.BookId)
                    .IsRequired()
                    .HasColumnType("INTEGER");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnType("INTEGER");

                entity.Property(e => e.Information).HasColumnType("nvarchar");

                entity.Property(e => e.Number).HasColumnType("tinyint");
            });

            //modelBuilder.Entity<Verse>()
            //  .ToTable("Verses");

            modelBuilder.Entity<Verse>(entity =>
            {
                entity.HasKey("Id");

                //entity.Property(e => e.Ordinal)
                //    .IsRequired()
                //    .HasColumnType("int");
                //entity.Property(e => e.BookName)
                //    .IsRequired()
                //    .HasColumnType("varchar");
                //entity.Property(e => e.Testament)
                //    .IsRequired()
                //    .HasColumnType("varchar");

                //entity.Property(e => e.ChapterNumber)
                //    .IsRequired()
                //    .HasColumnType("int");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnType("INTEGER");

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
        public async Task<Verse> GetVerseAsync(int id)
        {
            var item = await Verses.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return item;
        }
        public async Task<Verse> GetVerseAsync(Expression<Func<Verse, bool>> selector)
        {
            var item = await Verses.FirstOrDefaultAsync(selector).ConfigureAwait(false);
            return item;
        }
        /// <summary>
        /// Sets to now
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRecited(Verse item)
        {
            try
            {
                item.LastRecited = DateTime.Now.ToLocalTime();

                return await UpdateVerse(item);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Problem updating last recited time.\n{ex}");
            }
            return false;
        }
        /// <summary>
        /// pass in versse that has updated prop values
        /// </summary>
        /// <param name="verse"></param>
        /// <returns></returns>
        public async Task<bool> UpdateVerse(Verse verse)
        {
            try
            {
                this.Update(verse);
                return 1 == await this.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Problem updating Verse...\n{ex}");
            }
            return false;
        }     /// <summary>
              /// pass in verse that has updated prop values
              /// </summary>
              /// <param name="dbObject"></param>
              /// <returns></returns>
        public async Task<bool> Update<T>(T dbObject, object key) where T : class
        {
            try
            {
                var dbObjectInThisContext = Find<T>(key);
                if (dbObjectInThisContext != null)
                {
                    var ent = Entry(dbObjectInThisContext);
                    Entry(dbObject).State = EntityState.Detached;
                    this.Update(dbObjectInThisContext);
                    ent.State = EntityState.Modified;
                    ent.CurrentValues.SetValues(dbObject);
                    return 1 == await this.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Verse v)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // TODO: decide which value should be written to database
                            proposedValues[property] = proposedValues[property];
                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    //else if (entry.Entity is Book bk)
                    //{
                    //    entry.State = EntityState.Unchanged;
                    //    //var proposedValues = entry.CurrentValues;
                    //    var databaseValues = entry.GetDatabaseValues();

                    //    //foreach (var property in proposedValues.Properties)
                    //    //{
                    //    //    var proposedValue = proposedValues[property];
                    //    //    var databaseValue = databaseValues[property];

                    //    //    // TODO: decide which value should be written to database
                    //    //     proposedValues[property] = proposedValues[property];
                    //    //}

                    //    // Refresh original values to bypass next concurrency check
                    //    if (databaseValues != null) entry.OriginalValues.SetValues(databaseValues);
                    //}
                    //else if (entry.Entity is Chapter ch)
                    //{
                    //    var proposedValues = entry.CurrentValues;
                    //    var databaseValues = entry.GetDatabaseValues();

                    //    foreach (var property in proposedValues.Properties)
                    //    {
                    //        var proposedValue = proposedValues[property];
                    //        var databaseValue = databaseValues[property];

                    //        // TODO: decide which value should be written to database
                    //        proposedValues[property] = proposedValues[property];
                    //    }

                    //    // Refresh original values to bypass next concurrency check
                    //    entry.OriginalValues.SetValues(databaseValues);
                    //}
                    else
                    {
                        throw new NotSupportedException(
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Problem updating Verse...\n{ex}");
            }
            return false;
        }
        public async Task<bool> UpdateVerse(Dictionary<string, object> values, object key)
        {
            try
            {
                var dbObjectInThisContext = Verses.FirstOrDefault(x => x.Id == (int)key);
                if (dbObjectInThisContext != null)
                {
                    var ent = Entry(dbObjectInThisContext);

                    ent.CurrentValues.SetValues(values);
                    this.Update(dbObjectInThisContext);
                    ent.State = EntityState.Modified;
                    return 1 == await this.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Verse v)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // TODO: decide which value should be written to database
                            proposedValues[property] = proposedValues[property];
                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    //else if (entry.Entity is Book bk)
                    //{
                    //    entry.State = EntityState.Unchanged;
                    //    //var proposedValues = entry.CurrentValues;
                    //    var databaseValues = entry.GetDatabaseValues();

                    //    //foreach (var property in proposedValues.Properties)
                    //    //{
                    //    //    var proposedValue = proposedValues[property];
                    //    //    var databaseValue = databaseValues[property];

                    //    //    // TODO: decide which value should be written to database
                    //    //     proposedValues[property] = proposedValues[property];
                    //    //}

                    //    // Refresh original values to bypass next concurrency check
                    //    if (databaseValues != null) entry.OriginalValues.SetValues(databaseValues);
                    //}
                    //else if (entry.Entity is Chapter ch)
                    //{
                    //    var proposedValues = entry.CurrentValues;
                    //    var databaseValues = entry.GetDatabaseValues();

                    //    foreach (var property in proposedValues.Properties)
                    //    {
                    //        var proposedValue = proposedValues[property];
                    //        var databaseValue = databaseValues[property];

                    //        // TODO: decide which value should be written to database
                    //        proposedValues[property] = proposedValues[property];
                    //    }

                    //    // Refresh original values to bypass next concurrency check
                    //    entry.OriginalValues.SetValues(databaseValues);
                    //}
                    else
                    {
                        throw new NotSupportedException(
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Problem updating Verse...\n{ex}");
            }
            return false;
        }

    }
}

namespace VigilantKJV.Models
{
    public class TestamentValueConverter : ValueConverter
    {
        public TestamentValueConverter(LambdaExpression convertFromProviderExpression, LambdaExpression convertToProviderExpression) : base(convertFromProviderExpression, convertToProviderExpression)
        {

        }
        public override Func<object, object> ConvertToProvider => new Func<object, object>((testament) =>
        {
            return Enum.GetName(typeof(Testament), testament);
        });


        public override Func<object, object> ConvertFromProvider => new Func<object, object>((testament) =>
        {
            return Enum.Parse(typeof(Testament), "" + testament);
        });

        public override Type ModelClrType => typeof(TestamentName);

        public override Type ProviderClrType => typeof(int);

    }
}
