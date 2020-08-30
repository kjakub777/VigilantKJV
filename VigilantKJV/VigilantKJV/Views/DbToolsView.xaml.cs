using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Services;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VigilantKJV.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DbToolsView : ContentPage
    {
        Action<double, uint> animatePbar;

        private VigilantKJV.ViewModels.DbToolsViewModel viewmodel;

        public VigilantKJV.ViewModels.DbToolsViewModel Viewmodel
        {
            get
            {
                if (viewmodel == null && BindingContext != null)
                {
                    viewmodel = BindingContext as VigilantKJV.ViewModels.DbToolsViewModel;
                }
                return viewmodel;
            }
            set
            {
                viewmodel = value;
            }
        }


        public DbToolsView()
        {
            //NavigationPage.SetHasNavigationBar(this, false);
            //   var nav = DependencyService.Get<INavigationService>();
            InitializeComponent();

            //EntryImp
            //cbFtp
            pbar.IsVisible = pbar.Progress != 0d && pbar.Progress < .999d;
            animatePbar = new Action<double, uint>((dval, uimax) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    pbar.Minimum = 0;
                    pbar.Maximum = uimax;
                    lblpbar.Text = $"{(((dval / (double)uimax)) * 100).ToString("#.0")}%   {dval} of {uimax} ";
                    pbar.Progress = dval;// / (double)uimax;
                    pbar.IsVisible = pbar.Progress != 0d && pbar.Progress < uimax;
                    lblpbar.IsVisible = pbar.IsVisible;
                });
            });
        }

        private async void btnSql_Clicked(object sender, EventArgs e)
        {
            await Viewmodel.ExecuteSql(animatePbar);
        }

        private async void ClearDB_Clicked(object sender, EventArgs e)
        {
            await Viewmodel.ClearDb();
        }

        private async void ExportDB_Clicked(object sender, EventArgs e)
        {
            await Viewmodel.ExportDb(animatePbar);
        }


        private async void ImportDB_Clicked(object sender, EventArgs e)
        {
            await Viewmodel.ImportDb(animatePbar);
        }

        private async void ImportDBOrig_Clicked(object sender, EventArgs e)
        {
            await Viewmodel.ImportDbFromCsv(animatePbar);
        }

        private async void MyButton_LongPressed(object sender, EventArgs e)
        {
            UserDialogs.Instance.Toast($"Setting memorized from Csv...");
            Viewmodel.SetMemorizedFromCSV(animatePbar);
        }

        //internal override void OnIsVisibleChanged(bool oldValue, bool newValue)
        //{
        //    base.OnIsVisibleChanged(oldValue, newValue);
        //}
    }
}