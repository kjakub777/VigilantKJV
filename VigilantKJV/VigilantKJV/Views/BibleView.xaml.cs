using Acr.UserDialogs;

using Syncfusion.ListView.XForms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Helpers;
using VigilantKJV.Models;
using VigilantKJV.Services;
using VigilantKJV.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VigilantKJV.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BibleView : ContentPage
    {
        #region Vars
        const int CharsPERLINE = 58;
        static readonly Dictionary<SfListView, Dictionary<VisualElement, int>> _listViewHeightDictionary = new Dictionary<SfListView, Dictionary<VisualElement, int>>();
        Syncfusion.ListView.XForms.SwipeDirection endSwipe;
        private bool isExpanderExpanded;
        Syncfusion.ListView.XForms.SwipeDirection startSwipe; Stopwatch timeSwipe =

new Stopwatch();
        private BibleViewModel viewmodel;

        public bool IsExpanderExpanded
        {
            get { return isExpanderExpanded; }
            set
            {
                if (isExpanderExpanded == value)
                {
                    return;
                }

                isExpanderExpanded = value;
                OnPropertyChanged();
            }
        }


        public BibleViewModel Viewmodel
        {
            get
            {
                if (viewmodel == null && BindingContext != null)
                {
                    viewmodel = BindingContext as BibleViewModel;
                }
                return viewmodel;
            }
            set
            {
                viewmodel = value;
            }
        }
        #endregion

        public BibleView()
        {
            //NavigationPage.SetHasNavigationBar(this, false);
            //var nav = DependencyService.Get<INavigationService>();
            InitializeComponent();
            //IsExpanderExpanded = true;
            //ListHeaderExpander.
            // BindingContext = Viewmodel = new BibleViewModel();
            // Viewmodel.Navigation=Navigation
            // BindingContext = Viewmodel;  
            //    pickerStack.BindingContext =  CurrentColorGradient;
        }

        #region Meths


        private void Expander_Collapsed(object sender, Syncfusion.XForms.Expander.ExpandedAndCollapsedEventArgs e)
        {
            ///    IsExpanderExpanded = false;
        }
        private void Expander_Expanded(object sender, Syncfusion.XForms.Expander.ExpandedAndCollapsedEventArgs e)
        {
            ///  IsExpanderExpanded = true;
        }
        private void Frame_SizeChanged(object sender, EventArgs e)
        {

            //var frame = (VisualElement)sender;
            ////  var c =   frame.Cont
            //Console.WriteLine($"{frame}");
            //var listView = (SfListView)frame.Parent.Parent.Parent.Parent;
            //var height = (int)frame.Measure(1000, 1000, MeasureFlags.IncludeMargins).Minimum.Height;
            //if (!_listViewHeightDictionary.ContainsKey(listView))
            //{
            //    _listViewHeightDictionary[listView] = new Dictionary<VisualElement, int>();
            //}
            //if (!_listViewHeightDictionary[listView].TryGetValue(frame, out var oldHeight) || oldHeight != height)
            //{
            //    _listViewHeightDictionary[listView][frame] = height;
            //    var fullHeight = _listViewHeightDictionary[listView].Values.Sum();
            //    if ((int)listView.HeightRequest != fullHeight
            //        //&& listView.ItemsSource.Cast<object>().Count() == _listViewHeightDictionary[listView].Count
            //        )
            //    {
            //        listView.HeightRequest = fullHeight;
            //        listView.Layout(new Rectangle(listView.X, listView.Y, listView.Width, fullHeight));
            //    }
            //}
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //  Viewmodel.LblColor1 = tSlider.MinimumTrackColor;
            //Viewmodel.   LblColor2 = tSlider.MaximumTrackColor;
            //tSlider.AnchorX = (this.Width - tSlider.Width) / 2d;// = Viewmodel.MaxS;
            //tSlider.Maximum = Viewmodel.MaxS;
            //tSlider.Minimum = Viewmodel.MinS;
            Viewmodel?.LoadItemsCommand?.Execute(null);
        }


        private void OnEdit(object sender, EventArgs e)
        {
            UserDialogs.Instance.Toast($"OnEdit e{e} {sender}  ");
        }

        private void OnItemSelected(object sender, EventArgs e)
        {
            UserDialogs.Instance.Toast($"OnItemSelected e{e} {sender}  ");
        }

        private void OnTapped(object sender, EventArgs e)
        {
            UserDialogs.Instance.Toast($"OnTapped e{e} {sender}  ");
        }

        private async void OnVerseTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            if (e.ItemData is Verse v)
            {
                await Navigation.PushAsync(new VerseView(v));
                IsBusy = true;
            }
        }



        private void SfChipGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Viewmodel?.LoadItemsCommand?.Execute(null);
        }

        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            //try
            //{
            //    tSlider.ValueChanged -= Slider_ValueChanged;
            //    var val = tSlider.Value * 3d;
            //    if (val < 1d)
            //    {
            //        Viewmodel.SetSlider(.1);
            //    }
            //    else if (val < 2d)
            //    {
            //        Viewmodel.SetSlider(.5);
            //    }
            //    else
            //    {
            //        Viewmodel.SetSlider(.9);
            //    }
            //}
            //catch (Exception ex)
            //{

            //    Acr.UserDialogs.UserDialogs.Instance.Alert($"{ex}");
            //    Console.WriteLine($"{ex}");
            //}
            //finally
            //{
            //    tSlider.ValueChanged += Slider_ValueChanged;
            //}
        }


        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            //try
            //{
            //    double sliderW = this.tSlider.Width;
            //    double pix = (sliderW - 40) / Viewmodel.MaxS;
            //    var num = Math.Round(e.NewValue);
            //    // tSlider.Value = Viewmodel.SliderVal = num;
            //    Viewmodel.SliderVal = num;
            //    Viewmodel.PercentSlider = pix * Viewmodel.SliderVal / Viewmodel.MaxS;
            //    await lblSlider.TranslateTo(num * pix, 0, 500);// lblSlider.TranslateTo(num * pix/*percent * ((tSlider.Width - 40) / tSlider.Maximum)*/, 0, 100);

            //}
            //catch (Exception ex)
            //{

            //    Acr.UserDialogs.UserDialogs.Instance.Alert($"{ex}");
            //    Console.WriteLine($"{ex}");
            //}
            //finally
            //{
            //    //     tSlider.ValueChanged += Slider_ValueChanged;
            //}
        }

        private async void verseListView_ItemHolding(object sender, ItemHoldingEventArgs e)
        {            if(e.ItemData is Verse v
                ) { 
            await Viewmodel.SetMemorizedAsync(v);
            await Viewmodel.RefreshCurrent();}

            Acr.UserDialogs.UserDialogs.Instance.Toast($"{Viewmodel?.Verse?.FullTitle} updated.");

        }
        private void verseListView_QueryItemSize(object sender, Syncfusion.ListView.XForms.QueryItemSizeEventArgs e)
        {
            int textLen = $"{e.ItemData}".Length;
            int step = 25;
            //header is 20
            int header = 20;

            int len = ((textLen / CharsPERLINE) + 1) * step + header;

            e.ItemSize = len > 50 ? len : 50;
            e.Handled = true;
            return;
        }
        private async void verseListView_SwipeEnded(object sender, Syncfusion.ListView.XForms.SwipeEndedEventArgs e)
        {
            timeSwipe.Stop();

            Acr.UserDialogs.UserDialogs.Instance.Toast($"Swipe Ended."); 
            //if (timeSwipe.Elapsed.TotalMilliseconds > 1000 && startSwipe == endSwipe)
            //{
            //    Viewmodel.SetMemorizedAsync();
            //    await Viewmodel.RefreshCurrent();
            //}
        }
        private void verseListView_SwipeStarted(object sender, Syncfusion.ListView.XForms.SwipeStartedEventArgs e)
        {
            timeSwipe.Restart();
            startSwipe = e.SwipeDirection;
        }
        #endregion
    }
}