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
    public partial class VerseView : ContentPage
    {
        private VerseViewModel viewmodel;

        public VerseViewModel Viewmodel
        {
            get
            {
                if (viewmodel == null && BindingContext != null)
                {
                    viewmodel = BindingContext as VerseViewModel;
                }
                return viewmodel;
            }
            set
            {
                viewmodel = value;
            }
        }

         
        public VerseView(Verse verse)
        { 
            //   var nav = DependencyService.Get<INavigationService>();
            InitializeComponent();
            BindingContext = Viewmodel = new VerseViewModel(verse);
        }

        private async void Recited_Clicked(object sender, EventArgs e)
        {
            await Viewmodel.UpdateRecited();
      
        }

        private async void MyButton_LongPressed(object sender, EventArgs e)
        {
            await Viewmodel.UpdateRecitedUndo();
        }

        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        { 
           await Viewmodel.SetMemorized(e.Value );//IsMemorized = chkBox.IsChecked;
        }
    }
}