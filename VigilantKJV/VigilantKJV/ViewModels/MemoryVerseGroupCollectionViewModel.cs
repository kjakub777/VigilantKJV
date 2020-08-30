using Acr.UserDialogs;

using MvvmHelpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using VigilantKJV.DataAccess;
using VigilantKJV.Models;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public enum ArrangeBy
    {
        Book,
        Date,
        None
    }

;

    public class MemoryVerseGroupCollectionViewModel : BaseGroupViewModel
    {

        private bool _expanded;
        private VerseCollectionViewModel _oldBook;
        DataStore data;

        private ObservableCollection<VerseCollectionViewModel> items;

        string name;
        private VerseCollection verseCollection_formerlyBook;

        ObservableRangeCollection<VerseViewModel> verses = new ObservableRangeCollection<VerseViewModel>();

        public MemoryVerseGroupCollectionViewModel()
        {
            data = DataStoreFactory.GetNewDataContext();
            Items = new ObservableCollection<VerseCollectionViewModel>();
            LoadBooksCommand = new Command(async () => await ExecuteLoadItemsCommandAsync());
            RefreshItemsCommand = new Command<VerseCollectionViewModel>((item) => ExecuteRefreshItemsCommand(item));
        }

        public MemoryVerseGroupCollectionViewModel(VerseCollection collection, bool expanded)
        {
            data = DataStoreFactory.GetNewDataContext();
            Title = "Memorized";
            this._expanded = expanded;
            VerseGroup = collection;// = new VerseCollection() { Name = Name };
            LoadVerses();
        }

        //public MemoryVerseGroupCollectionViewModel(Book book, bool expanded)
        //{
        //    Title = "Memorized";
        //    this._expanded = expanded;
        //    VerseCollection_formerlyBook = new VerseCollection() { Key = "" + book.BookName };
        //    ;// = new VerseCollection() { Name = Name };
        //    LoadVerses();
        //}

        //public MemoryVerseGroupCollectionViewModel(string Name, bool expanded)
        //{
        //    data = DataStoreFactory.GetNewDataContext();
        //    Title = "Memorized";
        //    this._expanded = expanded;
        //    VerseGroup = new VerseCollection() { Key = Name };
        //    ;// = new VerseCollection() { Name = Name };
        //    LoadVerses();
        //}


        private void ExecuteRefreshItemsCommand(VerseCollectionViewModel item)
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


        internal async Task UpdateRecited(Verse item)
        {
            using (var data = DataStoreFactory.GetNewDataContext())
            {
                await data.UpdateRecited(item);
                RaisePropertyChangedEvent(new PropertyChangedEventArgs(nameof(Items)));
            }
        }
        ArrangeBy arrangeBy;

        int groupInt;
        public int GroupInt
        {
            get
            {
                return this.groupInt;
            }
            set
            {
                if (this.groupInt == value)
                {
                    return;
                }
                ArrangeBy = (ArrangeBy)value;
                SetProperty(ref this.groupInt, value);
            }
        }
        public virtual async Task ExecuteLoadItemsCommandAsync()
        {
            if (IsBusy)
                return;



            try
            {
                IsBusy = true;

                List<GroupedListItem<object, IEnumerable<Verse>, Verse>> dateList = null;
                switch (arrangeBy)
                {
                    case ArrangeBy.Book:
                        dateList = await data.GetMemorizedGroupByBook();
                        break;
                    case ArrangeBy.Date:
                        dateList = await data.GetMemorizedGroupByDate();
                        break;
                    default:
                        dateList = await data.GetMemorizedGroupByNone();
                        break;
                }
                if (dateList != null && dateList.Count > 0)
                {
                    try

                    {
                        Items.Clear();
                        foreach (var b in dateList)
                            Items.Add(new VerseCollectionViewModel("" + b.GroupKey, b.Collection, false));
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.Toast($"{ex}");
                    }
                    UserDialogs.Instance.Toast($"Loaded dateviewmodels.");
                }
                else
                {
                    IsEmpty = true;
                }


            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"The following error occurred:\n{ex}");
                Console.WriteLine($"The following error occurred:\n{ex}");
                System.Diagnostics.Debug.WriteLine($"The following error occurred:\n{ex}");
                throw;
            }

            finally
            {
                IsBusy = false;
            }

        }


        public virtual void LoadVerses()
        {
        }

        public ArrangeBy ArrangeBy
        {
            get => this.arrangeBy; set
            {
                if (this.arrangeBy == value)
                {
                    return;
                }

                SetProperty(ref this.arrangeBy, value);
                LoadBooksCommand?.Execute(null);
            }
        }





        public ObservableCollection<VerseCollectionViewModel> Items
        {
            get => items;

            set => SetProperty(ref items, value);
        }

        public Xamarin.Forms.Command LoadBooksCommand { get; set; }


        public string Name { get => this.name; set { this.name = value; } }

        public Xamarin.Forms.Command<VerseCollectionViewModel> RefreshItemsCommand { get; set; }





        public VerseCollection VerseGroup
        {
            get { return verseCollection_formerlyBook; }
            set
            {
                if (verseCollection_formerlyBook == value)
                {
                    return;
                }

                verseCollection_formerlyBook = value;
            }
        }
    }

    public class BaseGroupViewModel : FreshMvvm.FreshBasePageModel, INotifyPropertyChanged
    {

        string busyText = string.Empty;
        bool isBusy = false;

        bool isEmpty = false;
        string title = string.Empty;

        private void OnEmptyChanged(BaseGroupViewModel baseViewModel, PropertyChangedEventArgs propertyChangedEventArgs)
        { UserDialogs.Instance.Toast("No Data Found"); }

        protected bool SetProperty<T>(ref T backingStore,
                                      T value,
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

        public void RaisePropertyChangedEvent(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public string BusyText { get => busyText; set => SetProperty(ref busyText, value); }

        public bool IsBusy { get { return isBusy; } set { SetProperty(ref isBusy, value); } }

        public bool IsEmpty
        {
            get { return isEmpty; }
            set
            {
                isEmpty = value;
                OnEmptyChanged(this, new PropertyChangedEventArgs("IsEmpty"));
            }
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

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



