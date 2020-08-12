using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using VigilantKJV.Views;
using VigilantKJV;
using VigilantKJV.Models;

namespace VigilantKJV.Services
{
    public class NavigationService //: INavigationService
    {
        public async Task NavigateToBibleMain()
        {
            await VigilantKJV.App.Current.MainPage.Navigation.PushAsync(new BiblePage());
        }

        public async Task NavigateToDbTools()
        {
            await VigilantKJV.App.Current.MainPage.Navigation.PushAsync(new DbToolsPage());
        }

        public async Task NavigateToDetailVerseView(Verse v)
        {
            await VigilantKJV.App.Current.MainPage.Navigation.PushAsync(new VerseDetailPage(v));
        }

        public async Task NavigateToLastRecited()
        {
            await VigilantKJV.App.Current.MainPage.Navigation.PushAsync(new BiblePage());
        }

        public async Task NavigateToMemorized()
        {
            await VigilantKJV.App.Current.MainPage.Navigation.PushAsync(new BiblePage());
        }
    }
}
