using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Models;
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
            InitializeComponent();
            viewmodel = new LastRecitedViewModel();
        }

        private async void OnItemSelected(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
            await Navigation.PushAsync(new VerseDetailPage(item));
            UserDialogs.Instance.Toast($"Item selected by {sender}");
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
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