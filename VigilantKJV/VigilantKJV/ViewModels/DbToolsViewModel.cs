using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class DbToolsViewModel : BaseViewModel
    {
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

        DataAccess.DataStore db;
        Action<double, uint> animatePbar;
     

        string importPath;
        public string ImportPath
        {
            get => this.importPath;

            set => SetProperty<string>(ref this.importPath, value,
                nameof(ImportPath));
        }
        public bool UpFtp
        {
            get => upFtp;
            set => SetProperty<bool>(ref upFtp, value, nameof(UpFtp));
        }
        string exportPath;
        public string ExportPath
        {
            get => this.exportPath;

            set => SetProperty<string>(ref this.exportPath, value, nameof(ExportPath));
        }
        int position;
        public int Position

        {
            get => this.position;
            set => SetProperty<int>(ref this.position, value, nameof(Position));
        }
        public bool IsInProgress { get => this.isInProgress; set => SetProperty<bool>(ref this.isInProgress, value, nameof(IsInProgress)); }

        private bool isInProgress;
        private bool upFtp;
        public Command ImportDbCommand ;
        public Command ExportDbCommand ;
        public Command ClearDbCommand ;
        public Command ImportCsvDbCommand ;
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
        public async Task<bool> ClearDb()
        {
            IsInProgress = true;
            try
            {
                await Task.Factory.StartNew(() => db.ClearDb(null));
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
    }
}
