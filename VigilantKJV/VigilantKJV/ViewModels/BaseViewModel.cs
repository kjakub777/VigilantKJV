using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using VigilantKJV.Models;
using VigilantKJV.Services;
using Acr.UserDialogs;
using System.Threading.Tasks;
using VigilantKJV.Views;

namespace VigilantKJV.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, IBaseViewModel
    {

        bool isBusy = false;
        bool isEmpty = false;

        string title = string.Empty;

        public BaseViewModel()
        {
           
        }

        private void OnEmptyChanged(BaseViewModel baseViewModel, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UserDialogs.Instance.Toast("No Data Found");
        }

        //public async Task NavigateTo<T>(object inobj) where T : Page
        //{
        //    if (typeof(T) == typeof(BiblePage))
        //        await Navigation.NavigateToBibleMain();
        //    else if (typeof(T) == typeof(MemorizedPage))
        //        await Navigation.NavigateToMemorized();
        //    else if (typeof(T) == typeof(VerseDetailPage) && inobj is Verse v)
        //        await Navigation.NavigateToDetailVerseView(v);

        //    else if (typeof(T) == typeof(DbToolsPage))
        //        await Navigation.NavigateToDbTools();

        //    else if (typeof(T) == typeof(LastRecitedPage))
        //        await Navigation.NavigateToLastRecited();
        //}
        //  protected INavigationService Navigation { get => this.navigation; set => this.navigation = value; }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        public bool IsEmpty
        {
            get { return isEmpty; }
            set
            {
                isEmpty = value;
                OnEmptyChanged(this, new PropertyChangedEventArgs("IsEmpty"));
            }
        }
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
