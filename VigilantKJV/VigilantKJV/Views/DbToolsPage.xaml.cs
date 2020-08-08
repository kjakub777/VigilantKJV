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
                    pbar.Progress = dval / (double)uimax;
                    pbar.IsVisible = pbar.Progress != 0d && pbar.Progress < .999d;
                });
            });
        }

        private void ExportDB_Clicked(object sender, EventArgs e)
        {

        }

        private async void ImportDB_Clicked(object sender, EventArgs e)
        {
            await viewmodel.ImportDb(animatePbar);
        }

        private async void ImportDBOrig_Clicked(object sender, EventArgs e)
        {
            await viewmodel.ImportDbFromCsv(animatePbar);
        }

        private void ClearDB_Clicked(object sender, EventArgs e)
        {

        }
    }
}