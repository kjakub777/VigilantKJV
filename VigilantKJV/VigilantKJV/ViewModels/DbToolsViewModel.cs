using Acr.UserDialogs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Services;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class DbToolsViewModel : BaseViewModel
    {
        public DataAccess.DataStore DBAccess { get; set; }

        string exportPath;


        string importPath;

        bool breakSqlIntoChunks = true;

        public bool BreakSqlIntoChunks
        {
            get => this.breakSqlIntoChunks;
            set => SetProperty(ref this.breakSqlIntoChunks, value);
        }

        string sqlEditorText;
        public Command DatabaseActivityCommand { get; set; }
        public string SqlEditorText { get => this.sqlEditorText; set => SetProperty(ref this.sqlEditorText, value); }

        string sqlFilePick;
        public string SqlFilePick
        {
            get => this.sqlFilePick;
            set => SetProperty(ref this.sqlFilePick, value);
        }

        private bool isInProgress;
        int position;
        private bool upFtp;
        bool inQuery;

        public bool InQuery
        {
            get => this.inQuery;
            set
            {
                if (this.inQuery == value)
                {
                    return;
                }

                SetProperty(ref this.inQuery, value);
            }
        }

        public DbToolsViewModel()
        {
            DBAccess = new DataAccess.DataStore();
            DatabaseActivityCommand = new Command(() => ExecuteDatabaseActivityCommand());
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
                .Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Db.txt");//"/storage/9C33-6BBD/temp/DBAccess.txt";
            this.ExportPath = System.IO.Path
                .Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Db.db3");// "/storage/9C33-6BBD/temp/";
        }

        public async Task<bool> ClearDb()
        {
            IsInProgress = true;
            try
            {
                await Task.Factory.StartNew(() => DBAccess.DeleteAll());
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

        internal async Task SetMemorizedFromCSV(Action<double, uint> action)
        {
            IsInProgress = true;
            try
            {
                await Task.Factory.StartNew(async () => await DBAccess.SetMemorizedDB(action));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"An error occurred:\n{ex}");

            }
            finally
            {
                IsInProgress = false;
            }
        }

        public async Task ExecuteSqlEmbedded(string filename, Action<double, uint> action)
        {
            IsInProgress = true;
            try
            {
                await Task.Factory
                    .StartNew(async () => SqlEditorText = await DBAccess.ExecuteSqlEmbeddedScripts(filename, action, BreakSqlIntoChunks));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"An error occurred:\n{ex}");

            }
            finally
            {
                IsInProgress = false;
            }
        }
        public void ExecuteDatabaseActivityCommand()
        {
            UserDialogs.Instance.Toast($"Yaya it worked!!");
        }
        bool isSqlFiles;
        public bool IsSqlFiles
        {
            get => this.isSqlFiles;
            set => SetProperty(ref this.isSqlFiles, value);
        }
        public async Task ExecuteSql(Action<double, uint> action)
        {
            IsInProgress = true;
            try
            {
                if (IsSqlFiles)
                {
                    await ExecuteSqlEmbedded(SqlFilePick, action);
                }
                else if (!string.IsNullOrEmpty(SqlEditorText))
                {
                    await Task.Factory.StartNew(async () => await DBAccess.ExecuteSql(SqlEditorText));
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"An error occurred:\n{ex}");

            }
            finally
            {
                IsInProgress = false;
            }
        }

        public async Task<bool> ExportDb(Action<double, uint> action)
        {
            IsInProgress = true;
            try
            {
                await Task.Factory.StartNew(() => DBAccess.ExportDb(ExportPath, action, UpFtp));

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
                await Task.Factory.StartNew(() => DBAccess.ImportDb(ImportPath, action));

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
                await Task.Factory.StartNew(() => DBAccess.SeedData(action, true));

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

            set => SetProperty<string>(ref this.importPath, value, nameof(ImportPath));
        }

        public bool CheckExistingFirst { get; set; }

        public bool IsInProgress
        {
            get => this.isInProgress;
            set => SetProperty<bool>(ref this.isInProgress, value, nameof(IsInProgress));
        }

        public int Position

        {
            get => this.position;
            set => SetProperty<int>(ref this.position, value, nameof(Position));
        }

        public bool UpFtp { get => upFtp; set => SetProperty<bool>(ref upFtp, value, nameof(UpFtp)); }
    }
}
