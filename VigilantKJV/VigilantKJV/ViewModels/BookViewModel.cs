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
using VigilantKJV.Services;

namespace VigilantKJV.ViewModels
{
    public class BookViewModel : ObservableRangeCollection<VerseViewModel>,INotifyPropertyChanged 
    {
        
        public string Title;
        // It's a backup variable for storing CountryViewModel objects
        ObservableRangeCollection<VerseViewModel> verses = new ObservableRangeCollection<VerseViewModel>();
        Book book;

        public   DataAccess.DataStore DBAccess { get; set; }
        public BookViewModel(Book book, bool expanded,DataAccess.DataStore DBAccess )
        { this.DBAccess = DBAccess;
            Title = "Memorized";
            this._expanded = expanded;
            this.Book = book;
            LoadVerses();
        }

        async void LoadVerses()
        {
            try
            {
                  verses.Clear();
                //add  each chapter manually 
                // var nav= DependencyService.Get<INavigationService>();
                await Task.Run(() =>
              {
                  verses.AddRange((from chapter in Book.Chapters
                     from verse in chapter.Verses
                      where verse.IsMemorized
                      select new VerseViewModel(verse,DBAccess))
                    .OrderBy(x => x.Verse.Chapter.Number)
                      .ThenBy(x => x.Verse.Number));
              });
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
                    return "arrow_b.png";
                }
                else
                {
                    return "arrow_a.png";
                }
            }
        }
        public string Name { get { return Book.Name; } }

        public Book Book { get => this.book; set => this.book = value; }
        }
}
