using Acr.UserDialogs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

using VigilantKJV.Helpers;
using VigilantKJV.Models;

using Xamarin.Forms;

namespace VigilantKJV.DataAccess
{

    public class DataStore : MyKjvContext, IDisposable
    {
        public Action<Task, object> OnTasksCompleted { get; set; }
        public object OnTasksCompletedObjectState { get; set; }
        public static string EmbeddedFileRoot => "VigilantKJV.DataAccess";

        public static string BooksSql => $"{EmbeddedFileRoot}.books.sql";
        public static string ChaptersSql => $"{EmbeddedFileRoot}.chapters.sql";
        public static string VersesSql => $"{EmbeddedFileRoot}.verses.sql";
        public static string CSVResourceKJV => $"{EmbeddedFileRoot}.KJVWHOLE.csv";
        public static string CSVResourceMEM => $"{EmbeddedFileRoot}.MostRecentVerses.csv";
  
        public DataStore()
        {
          

        }

        #region Init 
        public async Task SeedData(Action<double, uint> animatePbar, bool force)
        {
            int count = -1;
            bool createAndInsert = false;
            if (!force)
            {
                try
                {
                    count =  Verses.Count();
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.Toast($"{ex.Message}.");
                }
                UserDialogs.Instance
                    .Confirm(new ConfirmConfig()
                    {
                        CancelText = "Forget it.",
                        Message = $"There are {count} rows in DB, import from csv?",
                        OkText = "Sounds like a plan!",
                        Title = "Data Issue",
                        OnAction =
                        delegate (bool confirm)
                        {
                            createAndInsert = confirm;
                        }
                    });
            }
            if (force || createAndInsert)
            {
                UserDialogs.Instance.Toast($"Data!..{count}");
                await GetDataFromCsv(animatePbar);

                //await SetMemorizedDB(animatePbar);
                //UserDialogs.Instance.Alert($"Created db records successfully.");
            }
        }
        public async Task GetDataFromCsv(Action<double, uint> animatePbar)
        {
            //string resource = CSVResourceKJV;
            //int lineNum = 0;
            //try

            //{
            //    var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Verse)).Assembly;

            //    //foreach (var res in assembly.GetManifestResourceNames())
            //    //    info += "found resource: " + res;
            //    //Debug.WriteLine(info);
            //    Stream stream = assembly.GetManifestResourceStream(resource);
            //    List<string> csv = new List<string>();
            //    if (stream != null)
            //    {
            //        using (var reader = new System.IO.StreamReader(stream))
            //        {
            //            while (-1 < reader.Peek())
            //            {
            //                csv.Add(await reader.ReadLineAsync());
            //            }
            //        }
            //    }
            //    else
            //        Debug.WriteLine("SHIT");
            //    var memorized = await GetMemorizedDictionary(animatePbar);
            //    //Book currentBook = null;
            //    //Chapter currentChapter = null;
            //    double dval = 0;
            //    uint length = (uint)csv.Count;
            //    Regex regexBook = new Regex(@"[\d]* ?[A-z]+\b");
            //    Regex regexChaptVerse = new Regex(@"\b[\d]+:[\d]+");
            //    Testament testament = Testament.Old;
            //    int bookIndex = 0;
            //    for (int i = 0; i < length; i++)
            //    {
            //        //if (i == max)
            //        //    break;
            //        lineNum++;
            //        string line = csv[i];
            //        string strRowBook = regexBook.Match(line).Value;
            //        if (testament == Testament.Old && strRowBook == "Matthew")
            //            testament = Testament.New;
            //        var arrChapterVerse = regexChaptVerse.Match(line).Value.Split(':');
            //        int intRowChapter = int.Parse(arrChapterVerse[0].Trim());
            //        int intVerseNum = int.Parse(arrChapterVerse[1].Trim());
            //        string strVerseContent = line.Substring(line.IndexOf(',') + 1);

            //        //we are on a new book, we must be on a new chapter too
            //        if (currentBook?.Name != strRowBook)
            //        {
            //            bookIndex++;
            //            if (currentBook != null)
            //            {
            //                await SaveChangesAsync(true);
            //            }
            //            //create book
            //            currentBook = new Book(BookNameFromString(strRowBook))
            //            {
            //                Chapters = new List<Chapter>(),
            //                Id = Guid.NewGuid(),
            //                Testament = testament   //(Testament) Enum.Parse(typeof(Testament),testament)
            //            };
            //            //add to db 
            //            Add(currentBook);

            //        }
            //        //we are on a new book, we must be on a new chapter too
            //        if (currentChapter?.Number != intRowChapter)
            //        {
            //            if (currentChapter != null)
            //            {
            //                await SaveChangesAsync(true);
            //            }
            //            //create chapter
            //            currentChapter = new Chapter()
            //            {
            //                Id = Guid.NewGuid(),
            //                BookId = currentBook.Id,
            //                Book = currentBook,
            //                Number = intRowChapter,
            //                Verses = new List<Verse>()
            //            };
            //            //add to db 
            //            Add(currentChapter);
            //        }
            //        var v = new Verse()
            //        {
            //            Text = strVerseContent,
            //            Id = Guid.NewGuid(),
            //            ChapterId = currentChapter.Id,
            //            Chapter = currentChapter,
            //            LastRecited = DateTime.Parse("04/15/2020 11:00:00"),
            //            IsMemorized = memorized.ContainsKey($"{strRowBook}{intRowChapter}:{intVerseNum}"),
            //            Number = intVerseNum,
            //            Information = $"{currentChapter.Book.Name} {currentChapter.Number}:{intVerseNum}"
            //        };
            //        Add(v);
            //        await SaveChangesAsync();
            //        animatePbar?.Invoke(++dval, length); ;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("THIS LINE " + lineNum + "\n" + ex.ToString());
            //}


        }

        public BookName BookNameFromString(string name)
        {
            name = name.Replace("1", "I").Replace("2", "II").Replace("3", "III").Replace(" ", "");
            return (BookName)Enum.Parse(typeof(BookName), name);
        }


        public async Task SetMemorizedDB(Action<double, uint> animatePbar)
        {
            //string resource = CSVResourceMEM;

            //int lineNum = 0;
            //try
            //{
            //    var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Verse)).Assembly;

            //    //foreach (var res in assembly.GetManifestResourceNames())
            //    //    info += "found resource: " + res;
            //    //Debug.WriteLine(info);
            //    Stream stream = assembly.GetManifestResourceStream(resource);
            //    //Stream stream = assembly.GetManifestResourceStream("BibleMemoryAssistant.Data.KJVWHOLE.csv");
            //    string text = "";
            //    if (stream != null)
            //    {
            //        using (var reader = new System.IO.StreamReader(stream))
            //        {
            //            text = await reader.ReadToEndAsync();
            //        }
            //    }
            //    else
            //        Debug.WriteLine("SHIT");

            //    var csv = text.Split('\n');
            //    double dval = 0;
            //    uint length = (uint)csv.Length;
            //    Regex regexBook = new Regex(@"[\d]* ?[A-z]+\b");
            //    Regex regexChaptVerse = new Regex(@"\b[\d]+:[\d]+");
            //    string book = ""; int ch = 0, vn = 0;
            //    for (int i = 0; i < csv.Length; i++)
            //    {
            //        lineNum++;
            //        string line = csv[i];
            //        book = regexBook.Match(line).Value;
            //        var arr = regexChaptVerse.Match(line).Value.Split(':');
            //        ch = int.Parse(arr[0]);
            //        vn = int.Parse(arr[1]);
            //        // string strVerseContent = line.Substring(line.IndexOf(',') + 1);
            //        var v = await GetVerseAsync((x => x.Chapter.Book.Name == book && x.Chapter.Number == ch && x.Number == vn));
            //        if (v != null)
            //        {
            //            v.IsMemorized = true;
            //            Update(v);
            //        }
            //        animatePbar?.Invoke(++dval, length);

            //    }
            //    animatePbar?.Invoke((double)length, length);
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("THIS LINE " + lineNum + "\n" + ex.ToString());
            //}

        }
        public async Task<StringDictionary> GetMemorizedDictionary(Action<double, uint> animatePbar)
        {
            string resource = CSVResourceMEM;
            StringDictionary dictionary = new StringDictionary();
            int lineNum = 0;
            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Verse)).Assembly;

                //foreach (var res in assembly.GetManifestResourceNames())
                //    info += "found resource: " + res;
                //Debug.WriteLine(info);
                Stream stream = assembly.GetManifestResourceStream(resource);
                //Stream stream = assembly.GetManifestResourceStream("BibleMemoryAssistant.Data.KJVWHOLE.csv");
                string text = "";
                if (stream != null)
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        text = await reader.ReadToEndAsync();
                    }
                }
                else
                    Debug.WriteLine("SHIT");

                var csv = text.Split('\n');
                double dval = 0;
                uint length = (uint)csv.Length;
                Regex regexBook = new Regex(@"[\d]* ?[A-z]+\b");
                Regex regexChaptVerse = new Regex(@"\b[\d]+:[\d]+");
                string book = ""; int ch = 0, vn = 0;
                for (int i = 0; i < csv.Length; i++)
                {
                    lineNum++;
                    string line = csv[i];
                    book = regexBook.Match(line).Value;
                    var arr = regexChaptVerse.Match(line).Value.Split(':');
                    ch = int.Parse(arr[0]);
                    vn = int.Parse(arr[1]);
                    var str = $"{book}{ch}:{vn}";
                    dictionary[str] = str;
                    animatePbar?.Invoke(++dval, length);

                }
                animatePbar?.Invoke((double)length, length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("THIS LINE " + lineNum + "\n" + ex.ToString());
            }
            return dictionary;
        }

        #endregion

        //       public async Task ClearDb(object p)
        //       {
        //           try
        //           {
        //DeleteAll>()Bible.RemoveRange(Bible.ToList());
        //               Book.RemoveRange(Book.ToList());
        //               Chapter.RemoveRange(Chapter.ToList());
        //               Verse.RemoveRange(Verse.ToList());
        //               UserDialogs.Instance.Toast("Not Implemented yet...");
        //           }
        //           catch (Exception ex)
        //           {
        //               UserDialogs.Instance.Toast($"{ex.Message}");
        //           }
        //       }
        public async Task<bool> ImportDb(string importPath, Action<double, uint> action)
        {
            var filepath = importPath ?? throw new ArgumentNullException("ImportPath"); //string.IsNullOrEmpty(importPath)? Path.Combine(path, "DBBack.txt"):importPath;
            if (File.Exists(filepath))
            {
                var vs = await Task.FromResult(File.ReadAllLines(filepath));
                double val = 0d;
                uint max = (uint)vs.Length;
                await Task.Factory.StartNew(() =>
                {
                    foreach (var row in vs)
                    {
                        try
                        {
                            Database.ExecuteSqlRaw(row);

                            action?.Invoke(++val, max);//progress bar invoke on main
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("" + ex);
                        }
                    }

                    UserDialogs.Instance.Alert($"DB imported {val} records.");
                    action?.Invoke((double)max, max);
                    return val > 0;
                });
            }
            return false;
        }
        public async Task<List<GroupedListItem<object, IEnumerable<Verse>, Verse>>> GetMemorizedGroupByBook()
        {
            List<GroupedListItem<object, IEnumerable<Verse>, Verse>> lst = new List<GroupedListItem<object, IEnumerable<Verse>, Verse>>();
            try
            {
                string sql = @"
                        select * from MemorizedVerseBookGroups";

                var verses = await this. Verses.FromSqlRaw(sql)
                    .Include(x=>x.Book)
                    .Include(x=>x.Chapter)                    
                           .ToListAsync();
                var keys = verses. Select(x => x.Book.BookName).Distinct().ToList();
                foreach (var key in keys)
                {
                    lst.Add(new GroupedListItem<object, IEnumerable<Verse>, Verse>()
                    {
                        GroupKey = key,
                        Collection = verses.
                        Where(x => x.Book.BookName == key)
                            .ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Error:\n{ex}");
            }
            return lst;
        }
        public async Task<List<GroupedListItem<object, IEnumerable<Verse>, Verse>>> GetMemorizedGroupByDate()
        {
            List<GroupedListItem<object, IEnumerable<Verse>, Verse>> lst = new List<GroupedListItem<object, IEnumerable<Verse>, Verse>>();
            try
            {
                string sql = @"
                        select * from MemorizedVerseDateGroups";

                var verses = await this.Verses.FromSqlRaw(sql)
                    .Include(x => x.Book)
                    .Include(x => x.Chapter)
                          .ToListAsync();
                var keys = verses.Select(x => x.LastRecited.ToString("MM-dd-yyyy")).Distinct().ToList();
                foreach (var key in keys)
                {
                    lst.Add(new GroupedListItem<object, IEnumerable<Verse>, Verse>()
                    {
                        GroupKey = key,
                        Collection = verses.
                        Where(x => x.LastRecited.ToString("MM-dd-yyyy") == key)
                            .ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Error:\n{ex}");
            }
            return lst;
        }
        public async Task<List<GroupedListItem<object, IEnumerable<Verse>, Verse>>> GetMemorizedGroupByNone()
        {
            List<GroupedListItem<object, IEnumerable<Verse>, Verse>> lst = new List<GroupedListItem<object, IEnumerable<Verse>, Verse>>();
            try
            {
                string sql = @"
                        select * from MemorizedVerseDateGroups";

                var verses = await this.Verses.FromSqlRaw(sql)
                    .Include(x => x.Book)
                    .Include(x => x.Chapter)
                          .ToListAsync();
     
                    lst.Add(new GroupedListItem<object, IEnumerable<Verse>, Verse>()
                    {
                        GroupKey = "All",
                        Collection = verses.ToList()
                    }); 
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Error:\n{ex}");
            }
            return lst;
        }
        public async Task ExportDatabaseScript()
        {
            this. Database.GenerateCreateScript();
        }
        public async Task DeleteAll()
        {
            await Task.Factory
             .StartNew(() =>
             {
                 UserDialogs.Instance
                     .Confirm(new ConfirmConfig()
                     {
                         CancelText = "Oops! No.",
                         Message = $"Are you sure you want to delete?",
                         OkText = "Very Sure.",
                         Title = "Data Issue",
                         OnAction =
                         delegate (bool confirm)
                         {
                             if (confirm)
                             {
                                Database.EnsureDeleted();
                                 UserDialogs.Instance.Alert("Db Deleted.");
                                  Database.EnsureCreated();
                             }
                         }
                     });
             }).ContinueWith(OnTasksCompleted, OnTasksCompletedObjectState);
        }
        public async Task<string> ExecuteSqlEmbeddedScripts(string filename, Action<double, uint> action, bool linedelimitted = true)
        {
            string ret = "";
            filename = string.IsNullOrEmpty(filename) ? "Verses.sql" : filename;
            filename =  $"{ EmbeddedFileRoot}.{filename}";
            if (!string.IsNullOrEmpty(filename))
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly;

                //foreach (var res in assembly.GetManifestResourceNames())
                //    info += "found resource: " + res;
                //Debug.WriteLine(info);
                Stream stream = assembly.GetManifestResourceStream(filename);
                List<string> sqlList = new List<string>();
                if (stream != null)
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        while (-1 < reader.Peek())
                        {
                            sqlList.Add(await reader.ReadLineAsync());
                        }
                    }
                    stream.Close();
                }

                double val = 0; uint unt = (uint)sqlList.Count;
                await Task.Factory
                    .StartNew(() =>
                    {
                        //do in pieces
                        if (linedelimitted)//each line is its own sql query
                        {
                            int chunksize = 100;
                            int intChunk = 0;
                            string sql = null;
                            bool ok = true;
                            bool success = true;
                            try
                            {
                                while (ok && intChunk * chunksize < sqlList.Count)
                                {
                                    var chunk = sqlList.Skip(intChunk * chunksize).Take(chunksize);
                                    intChunk++;
                                    try
                                    {
                                        sql = string.Join("\n", chunk).Replace('{', '[').Replace('}', ']');
                                        var rowsaffected = Database.ExecuteSqlRaw(sql);
                                        ret += "Rows affected: " + rowsaffected + "\n";
                                        val += (double)rowsaffected;
                                        action?.Invoke((double)val, unt);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("" + new String('!', 500) + "\n");
                                        Debug.WriteLine("" + new String('!', 500) + "\n" + ex);
                                        UserDialogs.Instance
                                            .Confirm(new ConfirmConfig()
                                            {
                                                CancelText = "Forget it.",
                                                Message = $"The following error occurred:\n {ex}",
                                                OkText = "Continue",
                                                Title = "Data Issue",
                                                OnAction =
                                                delegate (bool confirm)
                                                {
                                                    ok = confirm;
                                                }
                                            });
                                        success = false;
                                        if (!ok)
                                            throw;
                                    }
                                }
                                string msg = success
                                    ? $"{Path.GetFileName(filename)} Executed successfully."
                                    : "Sql execution completed with errors.";
                                UserDialogs.Instance.Alert($"{msg}");
                            }
                            catch (Exception ex)
                            {
                                UserDialogs.Instance.Toast("Exiting sql execution....");
                            }
                        }
                        else
                        {
                            try
                            {
                                string sql = string.Join(" ", sqlList);

                                var rowsaffected = Database.ExecuteSqlRaw(sql);
                                ret += "Rows affected: " + rowsaffected + "\n";
                            }
                            catch (Exception ex)
                            {
                                UserDialogs.Instance.Alert($"Error executing sql.\n{ex}");
                            }
                        }
                        //so pbar goes away
                        action?.Invoke((double)unt, unt);
                    })
                    .ContinueWith(OnTasksCompleted, OnTasksCompletedObjectState);
            }
            return ret;
        }
        public async Task ExecuteSql(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                await Task.Factory
                    .StartNew(async () =>
                    {
                        return "Rows affected:" + await Database.ExecuteSqlRawAsync(sql.Replace('{', '[').Replace('}', ']'));
                    })
                    //.ContinueWith(OnTasksCompleted, OnTasksCompletedObjectState)
                    ;
            }
        }
        public async Task<bool> ExportDb(string exportPath, Action<double, uint> action, bool toFtp)
        {            return false;
            //StringBuilder builder = new StringBuilder();

            //try
            //{
            //    //try to upload it to ftp

            //    var filepath = exportPath ?? throw new ArgumentNullException("ExportPath"); //string.IsNullOrEmpty(importPath)? Path.Combine(path, "DBBack.txt"):importPath;

            //    //var filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DBBack.txt");
            //    if (!Directory.Exists(Path.GetDirectoryName(filepath)))
            //        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            //    if (File.Exists(filepath))
            //        File.Delete(filepath);

            //    var bks = db.Books;
            //    var chs = db.Chapters;
            //    var vs = db.Verses;
            //    double val = 0d;
            //    uint max = (uint)(vs.Count() + chs.Count() + bks.Count());
            //    foreach (var v in bks)
            //    {
            //        string str = $"insert into book(Id,Name,Testament,Ordinal)  VALUES " +
            //        $"('{v.Id}',   '{v.Name}','{v.Testament}', '{v.Ordinal}');\n";
            //        builder.Append(str);
            //        action?.Invoke(++val, max);//progress bar invoke on main
            //    }
            //    foreach (var v in chs)
            //    {
            //        string str = $"insert into chapter(Id,BookId,Number) VALUES " +
            //        $"('{v.Id}',   '{v.BookId}','{v.Number}');\n";
            //        builder.Append(str);
            //        action?.Invoke(++val, max);//progress bar invoke on main
            //    }
            //    foreach (var v in vs)
            //    {
            //        string str = $"insert into verse(Id,  BookId, ChapterId, Number, Text, LastRecited, IsMemorized) VALUES " +
            //        $"('{v.Id}',   '{v.BookId}','{v.ChapterId}', '{v.Number}', '{v.Text.Replace("\r", "").Replace("'", "''")}'," +
            //        $"'{v.LastRecited.Ticks}', '{(v.IsMemorized ? "1" : "0")}');\n";
            //        builder.Append(str);
            //        action?.Invoke(++val, max);//progress bar invoke on main
            //    }

            //    FileStream fs = new FileStream(filepath, FileMode.CreateNew);
            //    fs.Flush();
            //    fs.Close();
            //    UserDialogs.Instance.Alert($"DB exported {vs.Count()} records.");
            //    action?.Invoke((double)max, max);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    UserDialogs.Instance.Alert($"Error writing text file.{ex}");
            //}
            //try
            //{
            //    if (toFtp)
            //    {
            //        await FtpHandler.SendDbToFTP("db.Txt", file: ASCIIEncoding.ASCII.GetBytes(builder.ToString()));

            //        await FtpHandler.SendDbToFTP();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    UserDialogs.Instance.Alert($"Error sending to ftp{ex}");
            //}
            //return false;

        }


        public void EagerLoad()
        {

        }
        public async Task<List<Verse>> GetLastRecitedAsync()
        {
            try
            {
                var lst =  Verses 
                    .Include(x=>x.Book)
                    .Include(x=>x.Chapter)
                       .Where(v => v.IsMemorized)
                       .OrderBy(v => v.LastRecited)
                       
                       .ToListAsync();
                return await lst;//await Task.FromResult(lst);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Now this...\n{ex}");
                return new List<Verse>();
            }
        }




        /// <summary>
        /// updates verse that id belongs to
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="param1"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRecited(int Id, DateTime? param1)
        {
            var item = await GetVerseAsync(Id);
            try
            {
                // assume Entity base class have an Id property for all items
                var entity = Verses.Find(Id);
                entity.LastRecited = param1 ?? DateTime.Now.ToLocalTime();
                if (entity == null)
                {
                    return false;
                }
                return await UpdateVerse(entity);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Problem updating last recited time.\n{ex}");
            }
            return false;
        }
      
    }


    public class DataStoreFactory
    {
        static bool firstGo = true;
    
        public static DataStore GetNewDataContext()
        {
            var ds = (DataStore)Activator.CreateInstance(typeof(DataStore));
            //ds.ChangeTracker.AutoDetectChangesEnabled = false;
            //ds.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
            //ds.ChangeTracker.LazyLoadingEnabled = true;
            //ds.ChangeTracker.AcceptAllChanges();
            if (firstGo)
            {
                firstGo = false;
             //    ds.Database.EnsureDeleted();
             //ds.Database.Migrate();
                ds.Database.EnsureCreated();
            }
            return ds;
            //var opts = Xamarin.Forms.DependencyService.Get<DbContextOptions<MyKjvContext>>();
            //return new MyKjvContext(opts);
        }
        public static MyKjvContext GetNewDataContext(bool newway)
        {
            var ds = (MyKjvContext)Activator.CreateInstance(typeof(MyKjvContext));

            if (firstGo)
            {
                firstGo = false;
                ds.Database.EnsureCreated();
                ds.Database.Migrate();
            }
            return ds;
            //var opts = Xamarin.Forms.DependencyService.Get<DbContextOptions<MyKjvContext>>();
            //return new MyKjvContext(opts);
        }
    }



}
