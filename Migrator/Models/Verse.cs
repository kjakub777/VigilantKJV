using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;
//using Xamarin.Forms;

namespace VigilantKJV.Models
{
    public class Verse : IBibleObject
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(1000)]
        public string Text { get; set; }

        public int Number { get; set; }

        [ForeignKey("FK_Verse_BookId")]
        public Guid BookId { get; set; }

        public Book Book { get; set; }

        [ForeignKey("FK_Verse_ChapterId")]
        public Guid ChapterId { get; set; }

        public Chapter Chapter { get; set; }

        //public Book Book { get; set; }
        public bool IsMemorized { get; set; }

        public DateTime LastRecited { get; set; }

        public string ChapVerseText => $"{Chapter?.Number}:{Number}";

        public string FullTitle => $"{Chapter?.Book?.Name} {Chapter?.Number}:{Number}";

        public override string ToString() => $"{Chapter?.Number}:{Number}\n{Text}";

        public string TimeSinceRecited => new TimeSpan(DateTime.Now.ToLocalTime().Ticks -
            LastRecited.ToLocalTime().Ticks).ToString(@"dd\.hh\:mm\:ss");

        public string Information { get; set; }

        public string FriendlyLabel => FullTitle;

        public string Name { get => FullTitle; }
    }
}