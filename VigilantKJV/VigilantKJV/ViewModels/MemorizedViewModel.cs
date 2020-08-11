using Acr.UserDialogs;
//using MvvmHelpers.Commands; 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Models;
using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{

    public class MemorizedViewModel : BaseViewModel
    {


        private BookViewModel _oldBook;

        private ObservableCollection<BookViewModel> items;
        public ObservableCollection<BookViewModel> Items
        {
            get => items;

            set => SetProperty(ref items, value);
        }

        public Xamarin.Forms.Command LoadBooksCommand { get; set; }
        public Xamarin.Forms.Command<BookViewModel> RefreshItemsCommand { get; set; }

        public MemorizedViewModel()
        {
            Items = new ObservableCollection<BookViewModel>();
            LoadBooksCommand = new Command(async () => await ExecuteLoadItemsCommandAsync());
            RefreshItemsCommand = new Command<BookViewModel>((item) => ExecuteRefreshItemsCommand(item));
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
        async System.Threading.Tasks.Task ExecuteLoadItemsCommandAsync()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                Items.Clear();
                var iquery = await DBAccess.DB.GetMemorizedBooks();
                    iquery?.ToList()
                        .ForEach((b) => Items.Add(new BookViewModel(b, false)));
                if (Items.Count == 0)
                    IsEmpty = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        internal async Task UpdateRecited(Verse item)
        {
            await DBAccess.UpdateRecited(item);
        }
    }
}
