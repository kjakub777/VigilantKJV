using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VigilantKJV.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BiblePage : ContentPage
    {
        public BiblePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new BibleViewModel();
            IsBusy = true;
        }

        private void OnTeamTapped(object sender, ItemTappedEventArgs e)
        {
            UserDialogs.Instance.Toast($"{e.Group} {e.Item} {e.ItemIndex} ");
        }
    }
}