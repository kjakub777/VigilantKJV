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
using System.Dynamic;

namespace VigilantKJV.ViewModels
{
    public class BibleViewModel : BaseViewModel
    {

        Book book;

        Chapter chapter;
        string testament;

        public BibleViewModel() : base()
        {
            Title = "The True Word (kjv)";
            Books = new ObservableCollection<Book>();
            Chapters = new ObservableCollection<Chapter>();
            Verses = new ObservableCollection<Verse>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadBooksCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                //   Items.Add(newItem);
                // await DataStore.AddItemAsync(newItem);
            });

            PropertyChanged += BibleViewModel_PropertyChanged;
            Testament = "Old";
        }

        double sliderVal = 0;
        public double SliderVal
        {
            get => this.sliderVal;
            set
            {
                Console.WriteLine($"{value}");
                this.sliderVal = value;
                TestamentToggle(value);
            }
        }
        internal void TestamentToggle(double val)
        {
            //decimal, 0 - 1, break in thirds
            double onethird = .3333333334;
            if (val < onethird)
            {
                Testament = "Old";
            }
            else if (sliderVal < 2 * onethird)
            {
                Testament = "New";
            }
            else
            {
                Testament = "Both";
            }
            //switch (Testament)
            //{
            //    case "Old": Testament = "New"; break;
            //    case "New": Testament = "Both"; break;
            //    case "Both": Testament = "Old"; break;
            //}
        }
        internal void TestamentToggle()
        {
            switch (Testament)
            {
                case "Old": Testament = "New"; break;
                case "New": Testament = "Both"; break;
                case "Both": Testament = "Old"; break;
            }
        }

        private void BibleViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Testament))
            {
                ExecuteLoadBooksCommand();
            }
            else if (e.PropertyName == nameof(this.Book))
            {
                ExecuteLoadChaptersCommand();
            }
            else if (e.PropertyName == nameof(this.Chapter))
            {
                ExecuteLoadVersesCommand();
            }
        }

        async Task ExecuteLoadBooksCommand()
        {
            IsBusy = true;

            try
            {
                // var testamentvalue = (Testament)Enum.Parse(typeof(Testament), this.Testament);
                Books.Clear();//        Testament =(Testament) Enum.Parse(typeof(Testament),testament)
                var books = await Task.FromResult<IOrderedQueryable<Book>>(DBAccess.DB.Book
                    .Where(x => x.Testament == this.Testament || this.Testament == "Both")
                    .Distinct()
                    .Include(b => b.Chapters)
                    .ThenInclude(c => c.Verses)
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
        Guid? lastBookid = Guid.Empty;
        Guid? lastChapterid = Guid.Empty;
        public async Task HandleVerseTapped()
        {
            //var layout = (BindableObject)sender;
            //var item = (Verse)layout.BindingContext;
            //await Xamarin.Forms.INavigation.PushAsync(new VerseDetailPage(new VerseViewModel(item)));
            //viewmodel.IsBusy = true;
            //UserDialogs.Instance.Toast($" {sender} {item}");
        }
        async Task ExecuteLoadChaptersCommand()
        {
            IsBusy = true;

            try
            {
                await Task.Factory
                    .StartNew(() =>
                    {
                        if (Book?.Id == lastBookid)
                        {
                            //not reloading the same set!
                            return;
                        }
                        Chapters.Clear();
                        Book?.Chapters?.OrderBy(x => x.Number).ForEach((x) => Chapters.Add(x));
                        //can't have a chapter loaded that isnt in this book
                        if (Chapter == null ||  //has to be a member of the collectiom
                            !Chapters.Any(x => x.Id == Chapter.Id))
                        {
                            Chapter = Chapters.FirstOrDefault();
                        }
                        lastBookid = Book.Id;
                    });

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
                await Task.Factory
                    .StartNew(() =>
                    {
                        Verses.Clear();
                        Chapter?.Verses.OrderBy(v => v.Number).ForEach((x) => Verses.Add(x));
                    });
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
        public Book Book
        {
            get => this.book;
            set
            {
                bool update = value != null && value != book;

                SetProperty(ref this.book, value);
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
                bool update = value != null && value != chapter;

                SetProperty(ref this.chapter, value);
                //if (update)
                //    lastChapterid = value.Id;
            }
        }
        public ObservableCollection<Chapter> Chapters { get; set; }
        public Command LoadItemsCommand { get; set; }
        public string Testament
        {
            get => "" + this.testament;
            set
            {
                if (value != "" + this.testament)
                {
                    SetProperty<string>(ref this.testament,
                        value,
                        nameof(this.Testament));
                    //  ExecuteLoadBooksCommand();
                }
            }
        }

        public ObservableCollection<Verse> Verses { get; set; }
        Verse verse;
        public Verse Verse
        {
            get => this.verse;
            set => SetProperty(ref this.verse, value, nameof(this.Verse));
        }
    }
}