using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Models;
using VigilantKJV.Services;
using VigilantKJV.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VigilantKJV.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LastRecitedPage : ContentPage
    {
        public LastRecitedViewModel viewmodel;
        public LastRecitedPage()
        {
          //var nav=  DependencyService.Get<INavigationService>();
            InitializeComponent();
            viewmodel = new LastRecitedViewModel( );
        }

        private async void OnItemSelected(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
            await Navigation.PushAsync(new VerseDetailPage(item));
            UserDialogs.Instance.Toast($"Item selected by {sender}");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //if (viewmodel.Items.Count == 0)
            //{
            viewmodel.ExecuteLoadVersesCommand();
           // }
            //if (viewModel.Items.Count == 0)
            //    viewModel.IsBusy = true;
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
            if (item != null)
                await viewmodel.UpdateRecited(item);
        }

        private async void MyButton_LongPressed(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
            await viewmodel.UpdateRecitedUndo(item);
        }
    }
}