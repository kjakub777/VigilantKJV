using System;
using System.Collections.Generic;
using System.Text; 
using System.ComponentModel.DataAnnotations; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigilantKJV.Models
{
    public class Chapter: IBibleObject,IDbObject
    {
        public Chapter()
        {
            //Id = Guid.NewGuid();
        }
        [Key]
        public int Id { get; set; }
        public ICollection<Verse> Verses { get; set; }

        [ForeignKey("FK_Chapter_BookId")]
        public int BookId { get; set; }

        public Book Book { get; set; }
        public int Number { get; set; }

        public string Information { get; set; }
        public string FriendlyLabel => $"{Book?.Name} {Number}";

        public string Name => FriendlyLabel;

        public override string ToString()
        {
            return $"{Number.ToString("")}";
        }
    }
}