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
    public partial class VerseDetailPage : ContentPage
    {
        public DataAccess.DataStore DBAccess { get; set; }
        VerseViewModel viewmodel;
        public VerseDetailPage(Verse verse)
        {
            DBAccess = new DataAccess.DataStore();
            //   var nav = DependencyService.Get<INavigationService>();
            InitializeComponent();
            BindingContext = viewmodel = new VerseViewModel(verse,DBAccess);
        }

        private async void Recited_Clicked(object sender, EventArgs e)
        {
            await viewmodel.UpdateRecited(null);
        }

        private async void MyButton_LongPressed(object sender, EventArgs e)
        {
            await viewmodel.UpdateRecitedUndo(null);
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var chkBox = sender as CheckBox;
            viewmodel.SetMemorized();//IsMemorized = chkBox.IsChecked;
        }
    }
}