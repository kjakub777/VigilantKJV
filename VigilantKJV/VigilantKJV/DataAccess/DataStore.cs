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

    public class DataStore
    {
        public static string EmbeddedFileRoot => "VigilantKJV.DataAccess";

        public static string BooksSql => $"{EmbeddedFileRoot}.books.sql";
        public static string ChaptersSql => $"{EmbeddedFileRoot}.chapters.sql";
        public static string VersesSql => $"{EmbeddedFileRoot}.verses.sql";
        public static string CSVResourceKJV => $"{EmbeddedFileRoot}.KJVWHOLE.csv";
        public static string CSVResourceMEM => $"{EmbeddedFileRoot}.MostRecentVerses.csv";
        MyKjvContext db;

        public DataStore()
        {
            DB = Xamarin.Forms.DependencyService.Get<MyKjvContext>();
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
                    count = DB.Verse.Count();
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
            string resource = CSVResourceKJV;
            int lineNum = 0;
            try

            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Verse)).Assembly;

                //foreach (var res in assembly.GetManifestResourceNames())
                //    info += "found resource: " + res;
                //Debug.WriteLine(info);
                Stream stream = assembly.GetManifestResourceStream(resource);
                List<string> csv = new List<string>();
                if (stream != null)
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        while (-1 < reader.Peek())
                        {
                            csv.Add(await reader.ReadLineAsync());
                        }
                    }
                }
                else
                    Debug.WriteLine("SHIT");
                var memorized = await GetMemorizedDictionary(animatePbar);
                Book currentBook = null;
                Chapter currentChapter = null;
                double dval = 0;
                uint length = (uint)csv.Count;
                Regex regexBook = new Regex(@"[\d]* ?[A-z]+\b");
                Regex regexChaptVerse = new Regex(@"\b[\d]+:[\d]+");
                string testament = "Old";
                int bookIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    //if (i == max)
                    //    break;
                    lineNum++;
                    string line = csv[i];
                    string strRowBook = regexBook.Match(line).Value;
                    if (testament == "Old" && strRowBook == "Matthew")
                        testament = "New";
                    var arrChapterVerse = regexChaptVerse.Match(line).Value.Split(':');
                    int intRowChapter = int.Parse(arrChapterVerse[0].Trim());
                    int intVerseNum = int.Parse(arrChapterVerse[1].Trim());
                    string strVerseContent = line.Substring(line.IndexOf(',') + 1);

                    //we are on a new book, we must be on a new chapter too
                    if (currentBook?.Name != strRowBook)
                    {
                        bookIndex++;
                        if (currentBook != null)
                        {
                            await DB.SaveChangesAsync(true);
                        }
                        //create book
                        currentBook = new Book()
                        {
                            Name = strRowBook,
                            Chapters = new List<Chapter>(),
                            Id = Guid.NewGuid(),
                            Ordinal = bookIndex,
                            Testament = testament   //(Testament) Enum.Parse(typeof(Testament),testament)
                        };
                        //add to db 
                        DB.Add(currentBook);

                    }
                    //we are on a new book, we must be on a new chapter too
                    if (currentChapter?.Number != intRowChapter)
                    {
                        if (currentChapter != null)
                        {
                            await DB.SaveChangesAsync(true);
                        }
                        //create chapter
                        currentChapter = new Chapter()
                        {
                            Id = Guid.NewGuid(),
                            BookId = currentBook.Id,
                            Book = currentBook,
                            Number = intRowChapter,
                            Verses = new List<Verse>()
                        };
                        //add to db 
                        DB.Add(currentChapter);
                    }
                    var v = new Verse()
                    {
                        Text = strVerseContent,
                        Id = Guid.NewGuid(),
                        ChapterId = currentChapter.Id,
                        Chapter = currentChapter,
                        LastRecited = DateTime.Parse("04/15/2020 11:00:00"),
                        IsMemorized = memorized.ContainsKey($"{strRowBook}{intRowChapter}:{intVerseNum}"),
                        Number = intVerseNum,
                        Information = $"{currentChapter.Book.Name} {currentChapter.Number}:{intVerseNum}"
                    };
                    DB.Add(v);
                    await DB.SaveChangesAsync();
                    animatePbar?.Invoke(++dval, length); ;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("THIS LINE " + lineNum + "\n" + ex.ToString());
            }


        }


        public async Task SetMemorizedDB(Action<double, uint> animatePbar)
        {
            string resource = CSVResourceMEM;

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
                    // string strVerseContent = line.Substring(line.IndexOf(',') + 1);
                    var v = await DB.Verse.FirstOrDefaultAsync(x => x.Chapter.Book.Name == book && x.Chapter.Number == ch && x.Number == vn);
                    if (v != null)
                    {
                        v.IsMemorized = true;
                        DB.Update(v);
                    }
                    animatePbar?.Invoke(++dval, length);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("THIS LINE " + lineNum + "\n" + ex.ToString());
            }

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
            }
            catch (Exception ex)
            {
                Debug.WriteLine("THIS LINE " + lineNum + "\n" + ex.ToString());
            }
            return dictionary;
        }

        public MyKjvContext DB { get => this.db; set => this.db = value; }
        #endregion

        //       public async Task ClearDb(object p)
        //       {
        //           try
        //           {
        //DB.DeleteAll>()Bible.RemoveRange(DB.Bible.ToList());
        //               DB.Book.RemoveRange(DB.Book.ToList());
        //               DB.Chapter.RemoveRange(DB.Chapter.ToList());
        //               DB.Verse.RemoveRange(DB.Verse.ToList());
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
                            DB.Database.ExecuteSqlRaw(row);

                            action?.Invoke(++val, max);//progress bar invoke on main
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("" + ex);
                        }
                    }

                    UserDialogs.Instance.Alert($"DB imported {val} records.");
                    return val > 0;
                });
            }
            return false;
        }
        public async Task ExecuteSqlEmbeddedScripts(string filename, Action<double, uint> action, bool linedelimitted = true)
        {
            filename = $"{EmbeddedFileRoot}.{filename}";
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

                //do in pieces
                if (linedelimitted)//each line is its own sql query
                {
                    int chunksize = 100;
                    int intChunk = 0;
                    string sql;
                    bool ok = true;
                    while (ok && intChunk * chunksize < sqlList.Count)
                    {
                        var chunk = sqlList.Skip(intChunk * chunksize).Take(chunksize);
                        intChunk++;
                        try
                        {
                            sql = string.Join("\n", chunk);

                            await db.Database.ExecuteSqlRawAsync(sql);
                            val += (double)chunksize;
                            action(val, unt);
                        }
                        catch (Exception ex)
                        {
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
                        }
                    }
                }
                else
                {
                    try
                    {
                        string sql = string.Join(" ", sqlList);

                        await db.Database.ExecuteSqlRawAsync(sql);
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.Alert($"Error executing sql.\n{ex}");
                    }
                }

            }
        }
        public async Task ExecuteSql(string sql, Action<double, uint> action)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                await db.Database.ExecuteSqlRawAsync(sql);
            }
        }
        public async Task<bool> ExportDb(string exportPath, Action<double, uint> action, bool toFtp)
        {
            StringBuilder builder = new StringBuilder();

            try
            {
                //try to upload it to ftp

                var filepath = exportPath ?? throw new ArgumentNullException("ExportPath"); //string.IsNullOrEmpty(importPath)? Path.Combine(path, "DBBack.txt"):importPath;

                //var filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DBBack.txt");
                if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                if (File.Exists(filepath))
                    File.Delete(filepath);

                var bks = db.Book;
                var chs = db.Chapter;
                var vs = db.Verse;
                double val = 0d;
                uint max = (uint)(vs.Count() + chs.Count() + bks.Count());
                foreach (var v in bks)
                {
                    string str = $"insert into book(Id,Name,Testament,Ordinal)  VALUES " +
                    $"('{v.Id}',   '{v.Name}','{v.Testament}', '{v.Ordinal}');\n";
                    builder.Append(str);
                    action?.Invoke(++val, max);//progress bar invoke on main
                }
                foreach (var v in chs)
                {
                    string str = $"insert into chapter(Id,BookId,Number) VALUES " +
                    $"('{v.Id}',   '{v.BookId}','{v.Number}');\n";
                    builder.Append(str);
                    action?.Invoke(++val, max);//progress bar invoke on main
                }
                foreach (var v in vs)
                {
                    string str = $"insert into verse(Id,  BookId, ChapterId, Number, Text, LastRecited, IsMemorized) VALUES " +
                    $"('{v.Id}',   '{v.BookId}','{v.ChapterId}', '{v.Number}', '{v.Text.Replace("\r", "").Replace("'", "''")}'," +
                    $"'{v.LastRecited.Ticks}', '{(v.IsMemorized ? "1" : "0")}');\n";
                    builder.Append(str);
                    action?.Invoke(++val, max);//progress bar invoke on main
                }

                FileStream fs = new FileStream(filepath, FileMode.CreateNew);
                fs.Flush();
                fs.Close();
                UserDialogs.Instance.Alert($"DB exported {vs.Count()} records.");
                return true;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Error writing text file.{ex}");
            }
            try
            {
                if (toFtp)
                {
                    await FtpHandler.SendDbToFTP("db.Txt", file: ASCIIEncoding.ASCII.GetBytes(builder.ToString()));

                    await FtpHandler.SendDbToFTP();
                }

            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Error sending to ftp{ex}");
            }
            return false;

        }
        public async Task UpdateRecited(Verse item, DateTime? dateTime = null)
        {
            DB.Update(item.LastRecited = dateTime ?? DateTime.Now.ToLocalTime());
            await DB.SaveChangesAsync();
        }
        public async Task UpdateRecited(Guid Id, DateTime dateTime)
        {
            var item = DB.Verse.FirstOrDefaultAsync(x => x.Id == Id);
            await UpdateRecited(await item, new DateTime?(dateTime));

        }
        public void EagerLoad()
        {

        }
        public async Task<List<Verse>> GetLastRecitedAsync()
        {
            return await DB.Verse
                .Where(v => v.IsMemorized)
                .Include(v => v.Book)
                .ThenInclude(v => v.Chapters)
                .OrderBy(v => v.LastRecited)
                .ThenBy(v => v.Book.Ordinal)
                .ThenBy(v => v.Chapter.Number)
                .ThenBy(v => v.Number).ToListAsync();
        }

    }
}
