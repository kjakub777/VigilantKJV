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
    public partial class MemorizedPage : ContentPage
    {
        public MemorizedViewModel viewmodel;
        public MemorizedPage()
        {
            InitializeComponent();
            BindingContext = viewmodel = new MemorizedViewModel();
        }

        static readonly Dictionary<ListView, Dictionary<VisualElement, int>> _listViewHeightDictionary = new Dictionary<ListView, Dictionary<VisualElement, int>>();

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

        private void ListView_OnSizeChanged(object sender, EventArgs e)
        {
            var listView = (ListView)sender;
            if (listView.ItemsSource == null || listView.ItemsSource.Cast<object>().Count() == 0)
            {
                listView.HeightRequest = 0;
            }
        }



        private async void Button_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Verse)layout.BindingContext.GetType().GetProperty("Verse").GetValue(layout.BindingContext);
            if (item != null)
                await viewmodel.UpdateRecited(item);
        }
    }
}