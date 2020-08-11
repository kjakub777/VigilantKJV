using Acr.UserDialogs;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using System.Collections.Generic;

using System.Text;


using VigilantKJV.Models;
using System.Linq;
using Xamarin.Forms.Internals;

using System.ComponentModel;

using MvvmHelpers;

namespace VigilantKJV.ViewModels
{
    public class BookViewModel : ObservableRangeCollection<VerseViewModel>, INotifyPropertyChanged//,BaseViewModel
    {
        DataAccess.DataStore db;
        public string Title;
        // It's a backup variable for storing CountryViewModel objects
        ObservableRangeCollection<VerseViewModel> verses = new ObservableRangeCollection<VerseViewModel>();
        Book book;
        public BookViewModel(string BookName, bool expanded) : base()
        {
            db = new DataAccess.DataStore();
            Title = "Memorized";
            this._expanded = expanded;
            Book = new Book() { Name = BookName };
            LoadVerses();
        }
        public BookViewModel(Book book, bool expanded) : base()
        {
            db = new DataAccess.DataStore();
            Title = "Memorized";
            this._expanded = expanded;
            this.Book = book;
            LoadVerses();
        }

        async void LoadVerses()
        {
            try
            {
                //add  each chapter manually 

                verses.Clear();
                verses.AddRange(
                    (from b in Book.Chapters
                     from x in b.Verses
                     where x.IsMemorized
                     select new VerseViewModel(x))
                    .OrderBy(x => x.Verse.Chapter.Number)
                    .ThenBy(x => x.Verse.Number)
                    );
            }
            catch (Exception ex)
            {

                UserDialogs.Instance.Toast($"{ex}");
            }
            if (this._expanded)
                this.AddRange(verses);
        }
        public BookViewModel()
        {

        }








        private bool _expanded;

        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Expanded"));
                    if (_expanded)
                    {
                        this.AddRange(verses);
                    }
                    else
                    {
                        this.Clear();
                    }
                    OnPropertyChanged(new PropertyChangedEventArgs("StateIcon"));
                }
            }
        }
        public Command LoadItemsCommand { get; set; }
        public string StateIcon
        {
            get
            {
                if (Expanded)
                {
                    return "arrow_b2.png";
                }
                else
                {
                    return "arrow_a2.png";
                }
            }
        }
        public string Name { get { return Book.Name; } }

        public Book Book { get => this.book; set => this.book = value; }
        public bool IsBusy { get; private set; }
    }
}
