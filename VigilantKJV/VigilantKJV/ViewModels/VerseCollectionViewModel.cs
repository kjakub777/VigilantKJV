using Acr.UserDialogs;

using MvvmHelpers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using VigilantKJV.DataAccess;
using VigilantKJV.Models;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    /// <summary>
    /// is to Bookview model before
    /// </summary>  
    public class VerseCollectionViewModel : ObservableRangeCollection<VerseViewModel>, INotifyPropertyChanged
    {

        private bool _expanded;
        VerseCollection book;
        // It's a backup variable for storing CountryViewModel objects
        ObservableRangeCollection<VerseViewModel> verses = new ObservableRangeCollection<VerseViewModel>();
        public string Title;

        public VerseCollectionViewModel(string Key, IEnumerable<Verse> vs, bool expanded)
        {
            foreach (var v in vs)
            {
                verses.Add(new VerseViewModel(v));
            }
            Title = "Memorized";
            this._expanded = expanded;
            Book = new VerseCollection() { Key = Key, Collection = vs.ToImmutableList() };
            if (expanded)
                this.AddRange(verses);
        }

        public int? Count => verses?.Count;
        async void LoadVerses()
        {
            //try
            //{
            //    var vs = await DataStore.GetVersesAsync(Book.Name, true);
            //    foreach (var v in vs)
            //    {
            //        verses.Add(new VerseViewModel(v));
            //    }
            //}
            //catch (Exception ex)
            //{

            //    UserDialogs.Instance.Toast($"{ex}");
            //}
            //if (this._expanded)
            //    this.AddRange(verses);
        }

        public string Label => $"{Name}\t\t\t{verses.Count}";
        public VerseCollection Book { get => this.book; set => this.book = value; }

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


        public bool IsBusy { get; private set; }
        public Command LoadItemsCommand { get; set; }
        public string Name { get { return Book.Key; } }
        public string StateIcon
        {
            get
            {
                if (Expanded)
                {
                    return "arrow_b.png";
                }
                else
                { return "arrow_a.png"; }
            }
        }
    }

    public interface IMemorizedVerseCollection
    {
        void LoadVerses();



        bool Expanded { get; set; }
        Command LoadItemsCommand { get; set; }

        string Title { get; set; }

        VerseCollection VerseCollection_formerlyBook { get; set; }

    }

    public class VerseCollection
    {
        public ICollection<Verse> Collection { get; set; }
        public string Key { get; set; }
    }
}
