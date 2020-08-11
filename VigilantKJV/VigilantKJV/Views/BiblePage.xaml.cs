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
    public partial class BiblePage : ContentPage
    {
        public BibleViewModel viewmodel;
        public BiblePage()
        {
            InitializeComponent();
            BindingContext = viewmodel = new BibleViewModel();
        }

        public Command SwipeCommand => new Command(() => SwipeGestureRecognizer_Swiped(this, null));
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // IsBusy = true;
        }


        private void OnEdit(object sender, EventArgs e)
        {
            UserDialogs.Instance.Toast($"OnEdit e{e} {sender}  ");
            viewmodel.TestamentToggle();
        }

        private void OnTapped(object sender, EventArgs e)
        {
            UserDialogs.Instance.Toast($"OnTapped e{e} {sender}  ");
            viewmodel.TestamentToggle();
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            UserDialogs.Instance.Toast($"SwipeGestureRecognizer_Swiped e{e} {sender}  ");

            viewmodel.TestamentToggle();
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            UserDialogs.Instance.Toast($"Slider_ValueChanged e{e} {sender}  ");

            try
            {
                viewmodel.SliderVal = e.NewValue;

            }
            catch (Exception)
            {

            }
        }

        private void OnItemSelected(object sender, EventArgs e)
        {
            UserDialogs.Instance.Toast($"OnItemSelected e{e} {sender}  ");
        }

        private void SwipeGestureRecognizer_Swiped(object sender, EventArgs e)
        {
            UserDialogs.Instance.Toast($"SwipeGestureRecognizer_Swiped2 e{e} {sender}  ");
        }

        private async void OnVerseTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new VerseDetailPage(viewmodel.Verse));
            viewmodel.IsBusy = true;
            UserDialogs.Instance.Toast($" {sender} {e}");
            // await viewmodel.HandleVerseTapped();
        }

        private async void OnVerseTapped(object sender, ItemTappedEventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
            await Navigation.PushAsync(new VerseDetailPage(viewmodel.Verse));
            viewmodel.IsBusy = true;
            UserDialogs.Instance.Toast($" {sender} {item}");
        }
    }
}