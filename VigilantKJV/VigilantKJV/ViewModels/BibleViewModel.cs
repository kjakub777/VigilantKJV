using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using VigilantKJV.Models;
using VigilantKJV.Views;
using System.Linq;
using Xamarin.Forms.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Dynamic;
using VigilantKJV.Services;
using VigilantKJV.DataAccess;
using Acr.UserDialogs;
using MvvmCross.Core;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Xamarin.Forms.Xaml;

namespace VigilantKJV.ViewModels
{
    public class BibleViewModel : BaseViewModel
    {
        #region Vars
        Book book;

        Chapter chapter;
        private Xamarin.Forms.Color currentColorGradient = Xamarin.Forms.Color.Transparent;

        DataAccess.DataStore data;
        bool isBusy;
        int lastBookid;
        public double MinS = 0, MaxS = 6;

        bool newTestamentChecked = true;
        bool oldTestamentChecked = true;
        private double percentSlider;
        //intlastChapterid = Guid.Empty;

        double sliderVal = 0;
        int testamentInt;
        string testamentLabel;
        TestamentName testamentName;
        Verse verse;

        public Book Book
        {
            get => this.book;
            set
            {
                if (this.book == value)
                {
                    return;
                }

                this.book = value;
                RaisePropertyChanged();
                //if (update)
                //    lastBookid = value.Id;

            }
        }
        public ObservableCollection<Book> Books { get; set; }
        public Chapter Chapter
        {
            get => this.chapter;
            set
            {
                if (this.chapter == value)
                {
                    return;
                }

                this.chapter = value;
                this.RaisePropertyChanged(nameof(Chapter));
                //if (update)
                //    lastChapterid = value.Id;
            }
        }
        public ObservableCollection<Chapter> Chapters { get; set; }

        public Command LoadItemsCommand { get; set; }

        public bool NewTestamentChecked
        {
            get
            {
                return this.newTestamentChecked;
            }

            set
            {
                if (this.newTestamentChecked == value)
                {
                    return;
                }

                this.newTestamentChecked = value;
                RaisePropertyChanged();
            }
        }
        public bool OldTestamentChecked
        {
            get { return oldTestamentChecked; }
            set
            {
                if (oldTestamentChecked == value)
                {
                    return;
                }

                oldTestamentChecked = value;
                RaisePropertyChanged();
            }
        }

        public double PercentSlider
        {
            get => percentSlider; set
            {
                if (percentSlider == value)
                {
                    return;
                }

                percentSlider = value;
                RaisePropertyChanged();
            }
        }
        public double SliderVal
        {
            get => this.sliderVal;
            set
            {
                if (this.sliderVal == value)
                {
                    return;
                }
                // Console.WriteLine($"{value}");
                this.sliderVal = value;
                SetTestament(value);
            }
        }

        public Command SwipeCommand => new Command(() => ExecuteSwipeCommand());
        public int TestamentInt
        {
            get
            {
                return this.testamentInt;
            }
            set
            {
                if (this.testamentInt == value)
                {
                    return;
                }
                try
                {

                    var tn = (TestamentName)value;
                    TestamentName = tn;
                }
                catch (Exception ex)
                {

                }
                this.testamentInt = value;
                RaisePropertyChanged();
            }
        }
        public string TestamentLabel
        {
            get => this.testamentLabel; set
            {
                if (this.testamentLabel == value)
                {
                    return;
                }

                this.testamentLabel = value;
                RaisePropertyChanged();
            }
        }
        public TestamentName TestamentName
        {
            get => this.testamentName;
            set
            {
                if (value != this.testamentName)
                {
                    TestamentLabel = "" + value;
                    this.testamentName = value;
                    this.RaisePropertyChanged(nameof(this.TestamentName));
                    //  ExecuteLoadBooksCommand();
                }
            }
        }

        public Verse Verse
        {
            get => this.verse;
            set
            {
                if (this.verse == value)
                {
                    return;
                }

                this.verse = value;
                this.RaisePropertyChanged(nameof(this.Verse));
            }
        }

        public ObservableCollection<Verse> Verses { get; set; }
        #endregion

        public BibleViewModel()

        {

            Title = "The True Word (kjv)";
            Books = new ObservableCollection<Book>();
            Chapters = new ObservableCollection<Chapter>();
            Verses = new ObservableCollection<Verse>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadBooksCommand());
            this.PropertyChanged += BibleViewModel_PropertyChanged;
            //MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    //   Items.Add(newItem);
            //    // await DataStore.AddItemAsync(newItem);
            //});


            try
            {
                data = DataStoreFactory.GetNewDataContext();


                TestamentName = TestamentName.Old;// = data.Testament.First();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"{ex}");
                Console.WriteLine($"{ex}");
            }

        }

        #region Meths
        private async void BibleViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TestamentName))
            {
                await ExecuteLoadBooksCommand();
            }
            else if (e.PropertyName == nameof(this.Book))
            {
                await ExecuteLoadChaptersCommand();
            }
            else if (e.PropertyName == nameof(this.Chapter))
            {
                await ExecuteLoadVersesCommand();
            }
        }
        void Context_StateChanged(object sender, EntityStateChangedEventArgs e)
        {
            try
            {

                Console.WriteLine($"{e.Entry} { e.OldState}  { e.NewState }");
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex);
            }
        }
        void Context_Tracked(object sender, EntityTrackedEventArgs e)
        {
            Console.WriteLine("Tracking " + e.Entry);
            foreach (var item in e.Entry.CurrentValues.Properties)
            {
                try
                {

                    Console.WriteLine($"{item.Name}  ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("" + ex);
                }
            }
        }
        async Task ExecuteLoadBooksCommand()
        {
            IsBusy = true;
            try
            {


                Console.WriteLine($"This many  { data.Verses.Count()}");
                Books.Clear();
                IOrderedQueryable<Book> books;
                if (TestamentName == TestamentName.Both)
                    books = await Task.FromResult<IOrderedQueryable<Book>>(data.Books
                        .Include(x => x.Chapters)
                        .ThenInclude(x => x.Verses)
                        .OrderBy(x => x.Ordinal));
                else
                    books = await Task.FromResult(data.Books.Where(x => x.Testament.TestamentName == this.TestamentName)
                        .Include(x => x.Chapters)
                        .ThenInclude(x => x.Verses)
                        .OrderBy(x => x.Ordinal));


                foreach (var item in books)
                {
                    Books.Add(item);
                }
                if (Books.Count > 0 && (Book == null || !Books.Contains(Book)))//probably switched testaments
                {
                    Book = Books[0];
                }

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
        async Task ExecuteLoadChaptersCommand()
        {
            IsBusy = true;

            try
            {
                if (Book == null || Book.Id == lastBookid)
                {
                    //not reloading the same set!
                    return;
                }
                Chapters.Clear();

                var v = await Task.FromResult(Book.Chapters.OrderBy(x => x.Number));
                foreach (var ch in v)
                {

                    Chapters.Add(ch);

                }

                //can't have a chapter loaded that isnt in this book
                if (Chapter == null)
                {
                    Chapter = Chapters.FirstOrDefault();
                }

                lastBookid = Book.Id;
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
        async Task ExecuteLoadVersesCommand()
        {
            IsBusy = true;

            try
            {
                if (Chapter == null)
                    return;
                Verses.Clear();
                var vs = await Task.FromResult(Chapter.Verses.OrderBy(x => x.Number));

                foreach (var v in vs)
                {
                    Verses.Add(v);
                }
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

        private void ExecuteSwipeCommand()
        {
            UserDialogs.Instance.Toast($"Swiped!! {IsBusy}");
        }


        public async Task HandleVerseTapped(Verse v)
        {
            try
            {
                if (v != null)
                {
                    var ent = data.Entry(v);
                    ent.Reload();
                    //ent.State = EntityState.Modified;
                    var vsl = ent.GetDatabaseValues();
                    data.Attach(v);
                    v.IsMemorized = true;
                    v.LastRecited = DateTime.Now.AddDays(100);
                    data.Update(v);
                    await data.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                Acr.UserDialogs.UserDialogs.Instance.Alert($"{ex}");
                Console.WriteLine($"{ex}");
            }
        }
        public async Task RefreshCurrent()
        {

            var lastVerse = Verse;
            var lastBook = Book;
            var lastChapter = Chapter;
            await ExecuteLoadBooksCommand();
            if (lastVerse != null)
            {
                Book = lastVerse.Book;
                Chapter = lastVerse.Chapter;
                Verse = lastVerse;
            }
            else
            {
                if (lastBook != null)
                {
                    Book = lastBook;
                    Verse = lastVerse;
                }
                if (lastChapter != null)
                {
                    Chapter = lastChapter;
                }
            }
        }

        internal void SetSlider(double v)
        {
            SliderVal = v;
        }
        private void SetTestament(double val)
        {
            if (val < 2)
            {
                TestamentName = TestamentName.Old;
            }
            else if (val <= 4)
            {
                TestamentName = TestamentName.Both;
            }
            else
            {
                TestamentName = TestamentName.New;
            }
        }
        public async Task SetMemorizedAsync(Verse v)
        {
            if (v != null)
            {
                data.Update(v);
                v.IsMemorized = !v.IsMemorized;
                await data.SaveChangesAsync();
                Verse = v;
            }
        }
        #endregion
    }
}