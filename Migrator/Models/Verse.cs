using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigilantKJV.Models
{ 
    public class Verse
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(1000)]
        public string Text { get; set; }
        public int Number { get; set; }
        [ForeignKey("FK_Verse_ChapterId")]
        public Guid ChapterId { get; set; } 
        public Chapter Chapter { get;set;}
        //public Book Book { get; set; }
        public bool IsMemorized { get; set; } 
        public DateTime LastRecited { get; set; }

        public string ChapVerseText => $"{Chapter?.Number}:{Number}";
        public string FullTitle => $"{Chapter?.Book?.Name} {Chapter?.Number}:{Number}";
        public override string ToString() => $"{ChapterId}:{Number}";
      
        public string Information { get; set; }

    }
}