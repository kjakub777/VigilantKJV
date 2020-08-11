using Acr.UserDialogs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class DbToolsViewModel : BaseViewModel
    { 

        DataAccess.DataStore db;
        string exportPath;


        string importPath;

        public bool IsSqlFiles { get; set; } = true;
        public string SqlEditorText { get; set; }
        public string SqlFilePick { get; set; }
        private bool isInProgress;
        int position;
        private bool upFtp;
        public Command ClearDbCommand;
        public Command ExportDbCommand;
        public Command ImportCsvDbCommand;
        public Command ImportDbCommand;

        public DbToolsViewModel()
        {
            db = new DataAccess.DataStore();

            //    ImportDbCommand = new Command(async () => await ExecuteImportDbCommand());
            //ExportDbCommand = new Command(async () => await ExecuteExportDbCommand()); 
            //    ClearDbCommand = new Command(async () => await ExecuteClearDbCommand()); 
            //    ImportCsvDbCommand = new Command(async () => await ExecuteImportCsvDbCommand()); 
            Init();
        }

        private void Init()
        {
            IsInProgress = false;
            Position = 0;
            this.ImportPath = System.IO.Path
                .Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Db.txt");//"/storage/9C33-6BBD/temp/db.txt";
            this.ExportPath = System.IO.Path
                .Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Db.db3");// "/storage/9C33-6BBD/temp/";
        }

        public async Task<bool> ClearDb()
        {
            IsInProgress = true;
            try
            {
                await Task.Factory.StartNew(() => db.DB.DeleteAll());
                return true;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"An error occurred while attempting to clear Db:\n{ex}");
                return false;
            }

            finally
            {
                IsInProgress = false;

            }
        }

        public async Task ExecuteSqlEmbedded(string filename, Action<double, uint> action)
        {
            string[] arr;
            if (CheckExistingFirst && (arr = new string[] { "Books.sql", "Chapters.sql", "Verses.sql" }).Contains(filename))
            {
                filename = filename.Replace(".sql", "Check.sql");
            }

            await Task.Factory.StartNew(() => db.ExecuteSqlEmbeddedScripts(filename, action));
        }
        public async Task ExecuteSql(Action<double, uint> action)
        {
            if (IsSqlFiles)
            {
                await ExecuteSqlEmbedded(SqlFilePick, action);
            }
            else if (!string.IsNullOrEmpty(SqlEditorText))
            {
                await Task.Factory.StartNew(() => db.ExecuteSql(SqlEditorText, action));
            }
        }
        public async Task<bool> ExportDb(Action<double, uint> action)
        {
            IsInProgress = true;
            try
            {
                await Task.Factory.StartNew(() => db.ExportDb(ExportPath, action, UpFtp));

                return true;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"An error occurred while attempting to export:\n{ex}");
                return false;
            }
            finally
            {
                IsInProgress = false;
            }
        }
        public async Task<bool> ImportDb(Action<double, uint> action)
        {
            IsInProgress = true;
            try
            {
                await Task.Factory.StartNew(() => db.ImportDb(ImportPath, action));

                return true;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"An error occurred while attempting to import:\n{ex}");
                return false;
            }
            finally
            {
                IsInProgress = false;
            }
        }
        public async Task<bool> ImportDbFromCsv(Action<double, uint> action)
        {
            IsInProgress = true;
            try
            {
                await Task.Factory.StartNew(() => db.SeedData(action, true));

                return true;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"An error occurred while attempting to import:\n{ex}");
                return false;
            }
            finally
            {
                IsInProgress = false;
            }
        }

        public string ExportPath
        {
            get => this.exportPath;

            set => SetProperty<string>(ref this.exportPath, value, nameof(ExportPath));
        }
        public string ImportPath
        {
            get => this.importPath;

            set => SetProperty<string>(ref this.importPath, value,
                nameof(ImportPath));
        }
        public bool CheckExistingFirst { get; set; }
        public bool IsInProgress { get => this.isInProgress; set => SetProperty<bool>(ref this.isInProgress, value, nameof(IsInProgress)); }
        public int Position

        {
            get => this.position;
            set => SetProperty<int>(ref this.position, value, nameof(Position));
        }
        public bool UpFtp
        {
            get => upFtp;
            set => SetProperty<bool>(ref upFtp, value, nameof(UpFtp));
        }
    }
}
