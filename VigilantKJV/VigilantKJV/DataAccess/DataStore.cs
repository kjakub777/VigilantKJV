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
        public static string CSVRoot => "VigilantKJV.DataAccess";

        public static string CSVResourceKJV => $"{CSVRoot}.KJVWHOLE.csv";
        public static string CSVResourceMEM =>$"{CSVRoot}.MostRecentVerses.csv";
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
                await GetDataInsertDB(animatePbar);

                await SetMemorizedDB(animatePbar);
                UserDialogs.Instance.Alert($"Created db records successfully.");
            }
        }
        public async Task GetDataInsertDB(Action<double, uint> animatePbar)
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
                Testament testament = Testament.Old;
                int bookIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    //if (i == max)
                    //    break;
                    lineNum++;
                    string line = csv[i];
                    string strRowBook = regexBook.Match(line).Value;
                    if (testament == Testament.Old && strRowBook == "Matthew")
                        testament = Testament.New;
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
                            Testament = testament
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
        public async Task<StringDictionary> GetMemorizedDictionary( Action<double, uint> animatePbar)
        {            string resource = CSVResourceMEM;
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

        public async Task ClearDb(object p)
        {
            try
            {
                //Context .C;
                UserDialogs.Instance.Toast("Not Implemented yet...");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast($"{ex.Message}");
            }
        }
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

                var vs = db.Verse;
                double val = 0d;
                uint max = (uint)vs.Count();
                foreach (var v in vs)
                {
                    string str = $"insert into verse(Id,  BookPosition, BookName, ChapterId, Number, Text, LastRecited, IsMemorized) VALUES " +
                    $"('{v.Id}',   '{v.ChapterId}', '{v.Number}', '{v.Text.Replace("\r", "").Replace("'", "''")}'," +
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

    }
}
