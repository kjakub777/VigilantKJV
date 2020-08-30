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
    public class BibleGridViewModel : BaseViewModel
    {
        #region Vars
     
        private Xamarin.Forms.Color currentColorGradient = Xamarin.Forms.Color.Transparent;

        DataAccess.DataStore data;
       
        Verse verse;

      

        public Command SwipeCommand => new Command(() => ExecuteSwipeCommand());
        

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
        public Command LoadItemsCommand { get; private set; }
        #endregion

        public BibleGridViewModel()

        {

            Title = "The True Word (kjv) *Memorized"; 
            Verses = new ObservableCollection<Verse>();
            LoadItemsCommand = new Command(async () => await ExecuteVersesCommand());
            this.PropertyChanged += BibleGridViewModel_PropertyChanged;
            //MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    //   Items.Add(newItem);
            //    // await DataStore.AddItemAsync(newItem);
            //});


            try
            {
                data = DataStoreFactory.GetNewDataContext();

      }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"{ex}");
                Console.WriteLine($"{ex}");
            }

        }

        #region Meths
        private async void BibleGridViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(TestamentName))
            //{
            //    await ExecuteVersesCommand();
            //}
            //else if (e.PropertyName == nameof(this.Book))
            //{
            //    await ExecuteLoadChaptersCommand();
            //}
            //else if (e.PropertyName == nameof(this.Chapter))
            //{
            //    await ExecuteLoadVersesCommand();
            //}
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
        async Task ExecuteVersesCommand()
        {
            IsBusy = true;
            try
            {


                Console.WriteLine($"This many  { data.Verses.Count()}");
                Verses.Clear();
                IOrderedQueryable<Verse> verses; 
                    verses = await Task.FromResult<IOrderedQueryable<Verse>>(data.Verses
                        .Where(x=>x.IsMemorized)
                        .Include(x => x.Chapter)
                        .ThenInclude(x => x.Book)
                        .OrderBy(x => x.Book.Ordinal)
                        .ThenBy(x=>x.Chapter.Number)
                        .ThenBy(x=>x.Number)
                        );
                

                foreach (var item in verses)
                {
                    Verses.Add(item);
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
        //async Task ExecuteLoadChaptersCommand()
        //{
        //    IsBusy = true;

        //    try
        //    {
        //        if (Book == null || Book.Id == lastBookid)
        //        {
        //            //not reloading the same set!
        //            return;
        //        }
        //        Chapters.Clear();

        //        var v = await Task.FromResult(Book.Chapters.OrderBy(x => x.Number));
        //        foreach (var ch in v)
        //        {

        //            Chapters.Add(ch);

        //        }

        //        //can't have a chapter loaded that isnt in this book
        //        if (Chapter == null)
        //        {
        //            Chapter = Chapters.FirstOrDefault();
        //        }

        //        lastBookid = Book.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}
        //async Task ExecuteLoadVersesCommand()
        //{
        //    IsBusy = true;

        //    try
        //    {
        //        if (Chapter == null)
        //            return;
        //        Verses.Clear();
        //        var vs = await Task.FromResult(Chapter.Verses.OrderBy(x => x.Number));

        //        foreach (var v in vs)
        //        {
        //            Verses.Add(v);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}

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