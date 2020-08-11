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
    public partial class VerseDetailPage : ContentPage
    {
        VerseViewModel viewmodel;
        public VerseDetailPage(Verse verse)
        {
            InitializeComponent();
            BindingContext = viewmodel = new VerseViewModel(verse);
        }

        private async void Recited_Clicked(object sender, EventArgs e)
        {
            await viewmodel.UpdateRecited(viewmodel.Verse);
        }

        private async void MyButton_LongPressed(object sender, EventArgs e)
        {
            await viewmodel.UpdateRecitedUndo(viewmodel.Verse);
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var chkBox = sender as CheckBox;
            viewmodel.IsMemorized = chkBox.IsChecked;
        }
    }
}