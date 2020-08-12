using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.DataAccess;
using VigilantKJV.Models;
using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class BookGroupViewModel : BaseViewModel, INotifyPropertyChanged, IBaseViewModel
    {
        private BookViewModel _oldBook;

        public DataAccess.DataStore DBAccess { get; set; }
        string busyText = string.Empty;

        //bool isBusy = false;
        //public bool IsBusy
        //{
        //    get { return isBusy; }
        //    set { SetProperty(ref isBusy, value); }
        //}
        bool isEmpty = false;

        private ObservableCollection<BookViewModel> items;

        public BookGroupViewModel()
        {            DBAccess = new DataStore();
            Items = new ObservableCollection<BookViewModel>();
            LoadBooksCommand = new Command(async () => await ExecuteLoadItemsCommandAsync());
            RefreshItemsCommand = new Command<BookViewModel>((item) => ExecuteRefreshItemsCommand(item));
        }

        async System.Threading.Tasks.Task ExecuteLoadItemsCommandAsync()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                Items.Clear();
                var booklist = (await DBAccess.GetMemorizedBooks()).ToList();


                if (booklist != null && booklist.Count > 0)
                {
                    try

                    {
                        foreach (var b in booklist)
                            Items.Add(new BookViewModel(b, false,DBAccess));
                    }
                    catch (Exception ex)
                    {

                        UserDialogs.Instance.Toast($"{ex}");
                    }
                    UserDialogs.Instance.Toast($"Loaded bookviewmodels.");
                }
                else { IsEmpty = true; }

            }
            catch (Exception ex)
            {
                IsBusy = false;
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ExecuteRefreshItemsCommand(BookViewModel item)
        {
            if (_oldBook == item)
            {
                // click twice on the same item will hide it
                item.Expanded = !item.Expanded;
            }
            else
            {
                if (_oldBook != null)
                {
                    // hide previous selected item
                    _oldBook.Expanded = false;
                }
                // show selected item
                item.Expanded = true;
            }

            _oldBook = item;
        }

        private void OnEmptyChanged(BaseViewModel baseViewModel, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UserDialogs.Instance.Toast("No Data Found");
        }

        internal async Task UpdateRecited(Verse item)
        {
            await DBAccess.UpdateRecited(item.Id, null);
        }


        public string BusyText
        {
            get => busyText;
            set => SetProperty(ref busyText, value);
        }
        public new bool IsEmpty
        {
            get { return isEmpty; }
            set
            {
                isEmpty = value;
                OnEmptyChanged(this, new PropertyChangedEventArgs("IsEmpty"));
            }
        }
        public ObservableCollection<BookViewModel> Items
        {
            get => items;

            set => SetProperty(ref items, value);
        }

        public Xamarin.Forms.Command LoadBooksCommand { get; set; }
        public Xamarin.Forms.Command<BookViewModel> RefreshItemsCommand { get; set; }


    }
}
