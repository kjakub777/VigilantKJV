using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace VigilantKJV.Models
{
    public class Verse : IBibleObject
    {
        #region Vars
        private bool isMemorized;
        public Book Book { get; set; }

        [ForeignKey("FK_Verse_BookId")]
        public int BookId { get; set; }

        public Chapter Chapter { get; set; }

        [ForeignKey("FK_Verse_ChapterId")]
        public int ChapterId { get; set; }

        // [ConcurrencyCheck]


        public string ChapVerseText => $"{Chapter?.Number}:{Number}";

        public string FriendlyLabel => FullTitle;

        public string FullTitle => $"{Book?.BookName} {Chapter?.Number}:{Number}";

        [Key]
        public int Id { get; set; }
        public string Information { get; set; }

        public bool IsMemorized
        {
            get
            {
                return isMemorized;
            }
            set
            {
                if (isMemorized == value)
                {
                    return;
                }

                isMemorized = value;
            }
        }

        public DateTime LastRecited { get; set; }
        public string LastRecitedCaption
        {
            get => IsMemorized ?
$"This verse was last recited { TimeSinceRecited } ago!"
: "";
        }

        public int Length => Text.Length;

        public string Name { get => FullTitle; }

        public int Number { get; set; }
        public string sLastRecited { get => IsMemorized ? LastRecited.ToString() : "--"; }

        [MaxLength(1000)]
        public string Text { get; set; }

        public string TimeSinceRecited
        {
            get
            {
                var ts = new TimeSpan(DateTime.Now.ToLocalTime().Ticks - LastRecited.ToLocalTime().Ticks);
                return IsMemorized ? "" + ts.Days + " Days" : "";

            }
        }
        #endregion

        #region Meths
        public override string ToString() => $"{Chapter?.Number}:{Number}\n{Text}";
        #endregion
    }
    //public class VerseShell
    //{
    //    public static VerseShell GetVerseShell(Verse verse)
    //    {
    //        return Verse.GetVerseShell(verse);
    //    }
    //    public int Id { get; set; }
    //    public Dictionary<string, object> Values { get; set; }
    //    public bool IsMemorized { get; set; }

    //    public DateTime LastRecited { get; set; }
    //    public string sLastRecited { get; set; }

    //    public string ChapVerseText { get; set; }

    //    public string FullTitle { get; set; }

    //    public override string ToString() => $"{Number}\n{Text}";

    //    public string Text { get; set; }

    //    public int Number { get; set; }
    //    public string TimeSinceRecited

    //    { get; set; }

    //}
}