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
    public partial class LastRecitedView : ContentPage
    {
        #region Vars
        const int CharsPERLINE = 33;
        static readonly Dictionary<ListView, Dictionary<VisualElement, int>> _listViewHeightDictionary = new Dictionary<ListView, Dictionary<VisualElement, int>>();
        private VigilantKJV.ViewModels.LastRecitedViewModel viewmodel;

        public VigilantKJV.ViewModels.LastRecitedViewModel Viewmodel
        {
            get
            {
                if (viewmodel == null && BindingContext != null)
                {
                    viewmodel = BindingContext as VigilantKJV.ViewModels.LastRecitedViewModel;
                }
                return viewmodel;
            }
            set
            {
                viewmodel = value;
            }
        }
        #endregion

        public LastRecitedView()
        {
            //NavigationPage.SetHasNavigationBar(this, false);
            //var nav=  DependencyService.Get<INavigationService>();
            InitializeComponent();
        }

        #region Meths
        private async void Button_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
            if (item != null)
                await Viewmodel.UpdateRecited(item);
        }

        private void ItemsCollectionView_QueryItemSize(object sender, Syncfusion.ListView.XForms.QueryItemSizeEventArgs e)
        {
            int textLen = $"{e.ItemData}".Length;
            int step = 25;
            //header is 20
            int header = 20;

            int len = ((textLen / CharsPERLINE) + 1) * step + header+30/*button*/;

            e.ItemSize = len > 50 ? len : 50;
            e.Handled = true;
            return;
        }

        private void ListView_OnSizeChanged(object sender, EventArgs e)
        {
            var listView = (ListView)sender;
            if (listView.ItemsSource == null || listView.ItemsSource.Cast<object>().Count() == 0)
            {
                listView.HeightRequest = 0;
            }
        }

        private async void MyButton_LongPressed(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
            await Viewmodel.UpdateRecitedUndo();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Viewmodel.ExecuteLoadVersesCommand();

        }

        private async void OnItemSelected(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext;
            await Navigation.PushAsync(new VerseView(item));
            // UserDialogs.Instance.Toast($"Item selected by {sender}");
        }

        private void VisualElement_OnSizeChanged(object sender, EventArgs e)
        {
            var frame = (VisualElement)sender;
            var listView = (ListView)frame.Parent.Parent;
            var height = (int)frame.Measure(1000, 1000, MeasureFlags.IncludeMargins).Minimum.Height;
            if (!_listViewHeightDictionary.ContainsKey(listView))
            {
                _listViewHeightDictionary[listView] = new Dictionary<VisualElement, int>();
            }
            if (!_listViewHeightDictionary[listView].TryGetValue(frame, out var oldHeight) || oldHeight != height)
            {
                _listViewHeightDictionary[listView][frame] = height;
                var fullHeight = _listViewHeightDictionary[listView].Values.Sum();
                if ((int)listView.HeightRequest != fullHeight &&
                    listView.ItemsSource.Cast<object>().Count() == _listViewHeightDictionary[listView].Count)
                {
                    listView.HeightRequest = fullHeight;
                    listView.Layout(new Rectangle(listView.X, listView.Y, listView.Width, fullHeight));
                }
            }
        }
        #endregion
    }
}