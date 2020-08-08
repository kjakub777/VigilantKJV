using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using VigilantKJV.Models;
using VigilantKJV.Views;
using System.Linq;

namespace VigilantKJV.ViewModels
{
    public class BibleViewModel : BaseViewModel
    {
        DataAccess.DataStore db;
        VigilantKJV.Models.Testament testament;
        public string Testament
        {
            get => "" + this.testament;
            set
            {
                if (value != "" + this.testament)
                {
                    SetProperty<Testament>(ref this.testament, 
                        value == "Old" ? Models.Testament.Old : value == "New" ? Models.Testament.New : Models.Testament.Both,
                        nameof(this.Testament));
                }
            }
        }

        Book book;
        public Book Book
        {
            get => this.book;
            set => SetProperty(ref this.book, value);
        }
        public ObservableCollection<Book> Books { get; set; }

        Chapter chapter;
        public Chapter Chapter
        {
            get => this.chapter;
            set
            {
                SetProperty(ref this.chapter, value);
            }
        }
        public ObservableCollection<Chapter> Chapters { get; set; }

        public ObservableCollection<Verse> Verses { get; set; }
        public Command LoadItemsCommand { get; set; }

        public BibleViewModel()
        {
            db = new DataAccess.DataStore();
            Title = "The True Word (kjv)";
            Books = new ObservableCollection<Book>();
            Chapters = new ObservableCollection<Chapter>();
            Verses = new ObservableCollection<Verse>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            Testament = "Old";
            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                //   Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Books.Clear();
                var books = await Task.FromResult<IOrderedQueryable<Book>>(db.DB.Book
                    .Where(x => x.Testament.ToString() == this.Testament)
                    .Select(x => x)
                    .OrderBy(x => x.Ordinal));
                foreach (var item in books)
                {
                    Books.Add(item);

                }
                if (Book == null || !Books.Contains(Book))//probably switched testaments
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
    }
}