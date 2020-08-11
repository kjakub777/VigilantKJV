using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VigilantKJV.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DbToolsPage : ContentPage
    {
        Action<double, uint> animatePbar;

        VigilantKJV.ViewModels.DbToolsViewModel viewmodel;
        public DbToolsPage()
        {
            InitializeComponent();
            BindingContext = viewmodel = new VigilantKJV.ViewModels.DbToolsViewModel();

            //EntryImp
            //cbFtp
            pbar.IsVisible = pbar.Progress != 0d && pbar.Progress < .999d;
            animatePbar = new Action<double, uint>((dval, uimax) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblpbar.Text = $"{(((dval / (double)uimax) )* 100).ToString("#.0")}%   {dval} of {uimax} ";
                    pbar.Progress = dval / (double)uimax;
                    pbar.IsVisible = pbar.Progress != 0d && pbar.Progress < .999d;
                    lblpbar.IsVisible = pbar.IsVisible;
                });
            });
        }

        private async void ExportDB_Clicked(object sender, EventArgs e)
        {
            await viewmodel.ExportDb(animatePbar);
        }


        private async void ImportDB_Clicked(object sender, EventArgs e)
        {
            await viewmodel.ImportDb(animatePbar);
        }

        private async void ImportDBOrig_Clicked(object sender, EventArgs e)
        {
            await viewmodel.ImportDbFromCsv(animatePbar);
        }

        private async void ClearDB_Clicked(object sender, EventArgs e)
        {
            await viewmodel.ClearDb();
        }

        private async void btnSql_Clicked(object sender, EventArgs e)
        {
            await viewmodel.ExecuteSql(animatePbar);
        }

    }
}