using MvvmCross.Base;

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class MemoryVerseGroupCollectionView : ContentPage
    {
        static readonly Dictionary<ListView, Dictionary<VisualElement, int>> _listViewHeightDictionary = new Dictionary<ListView, Dictionary<VisualElement, int>>();
        private MemoryVerseGroupCollectionViewModel viewmodel;

        public MemoryVerseGroupCollectionView()
        {
            //  var nav = DependencyService.Get<INavigationService>();
            InitializeComponent();
            //BindingContext = Viewmodel = new MemoryVerseGroupCollectionViewModel();
            Title = "Memorized";
        }
        //public MemoryVerseGroupCollectionView(MemoryVerseGroupCollectionViewModel viewModel)
        //{
        //    //NavigationPage.SetHasNavigationBar(this, false);
        //    InitializeComponent();
        //    this.Viewmodel = viewModel;
        //

        //}



        private async void Button_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext.GetType().GetProperty("Verse").GetValue(layout.BindingContext);
            if (item != null)
                await Viewmodel.UpdateRecited(item);
        }

        private async void IsToggledBooks_Toggled(object sender, ToggledEventArgs e)
        {
            await Viewmodel?.ExecuteLoadItemsCommandAsync();
        }

        private void ListView_OnSizeChanged(object sender, EventArgs e)
        {
            var listView = (ListView)sender;
            if (listView.ItemsSource == null || listView.ItemsSource.Cast<object>().Count() == 0)
            {
                listView.HeightRequest = 0;
            }
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

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (Viewmodel.Items.Count == 0)
                {
                    Viewmodel.LoadBooksCommand.Execute(null);
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }

        public MemoryVerseGroupCollectionViewModel Viewmodel
        {
            get
            {
                if (viewmodel == null && BindingContext != null)
                {
                   viewmodel = BindingContext as MemoryVerseGroupCollectionViewModel;
                }
                return viewmodel;
            }
            set
            {
                viewmodel = value;
            }
        }
    }
}